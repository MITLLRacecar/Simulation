using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

/// <summary>
/// Manages UDP communication with one or more Python scripts.
/// </summary>
public class PythonInterface
{
    #region Constants
    /// <summary>
    /// The UDP port used by the Unity simulation (this program).
    /// </summary>
    private const int unityPort = 5065;

    /// <summary>
    /// The UDP port used by the Unity simulation (this program) for async (Jupyter Notebook) calls.
    /// </summary>
    private const int unityPortAsync = 5064;

    /// <summary>
    /// The IP address used for communication.
    /// </summary>
    private static readonly IPAddress ipAddress = IPAddress.Parse("127.0.0.1");

    /// <summary>
    /// The time (in ms) to wait for Python to respond.
    /// </summary>
    private const int timeoutTime = 5000;

    /// <summary>
    /// The maximum UDP packet size allowed on Windows.
    /// </summary>
    private const int maxPacketSize = 65507;
    #endregion

    #region Public
    public PythonInterface(Racecar[] cars)
    {
        this.wasExitHandled = false;
        this.racecars = cars;
        this.pythonEndPoints = new List<IPEndPoint>();

        // Create a UDP client for handling sync calls
        this.udpClient = new UdpClient(new IPEndPoint(PythonInterface.ipAddress, PythonInterface.unityPort));
        this.udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        this.udpClient.Client.ReceiveTimeout = PythonInterface.timeoutTime;

        // Create a new thread with a UDP client for handling async calls
        this.asyncClientThread = new Thread(new ThreadStart(this.ProcessAsyncCalls))
        {
            IsBackground = true
        };
        this.asyncClientThread.Start();
    }

    /// <summary>
    /// Closes all UDP clients and sends an exit command to each connected Python script.
    /// </summary>
    public void HandleExit()
    {
        if (!this.wasExitHandled)
        {
            foreach (IPEndPoint endpoint in this.pythonEndPoints)
            {
                try
                {
                    this.udpClient.Send(new byte[] { (byte)Header.unity_exit }, 1, endpoint);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Unable to send exit command to Python. Error: {e}");
                }
            }
            this.pythonEndPoints.Clear();
            LevelManager.UpdateNumConnectedPrograms(this.pythonEndPoints.Count);

            this.udpClient.Close();
            this.udpClientAsync.Close();
            this.wasExitHandled = true;
        }
    }

    /// <summary>
    /// Tells Python to run the user's start function.
    /// </summary>
    public void HandleStart()
    {
        this.PythonCall(Header.unity_start);
    }

    /// <summary>
    /// Tells Python to run the user's update function.
    /// </summary>
    public void HandleUpdate()
    {
        this.PythonCall(Header.unity_update);
    }
    #endregion

    /// <summary>
    /// Header bytes used in our communication protocol.
    /// </summary>
    private enum Header
    {
        error,
        connect,
        unity_start,
        unity_update,
        unity_exit,
        python_finished,
        python_send_next,
        racecar_go,
        racecar_set_start_update,
        racecar_get_delta_time,
        racecar_set_update_slow_time,
        camera_get_color_image,
        camera_get_depth_image,
        camera_get_width,
        camera_get_height,
        controller_is_down,
        controller_was_pressed,
        controller_was_released,
        controller_get_trigger,
        controller_get_joystick,
        display_show_image,
        drive_set_speed_angle,
        drive_stop,
        drive_set_max_speed,
        lidar_get_num_samples,
        lidar_get_samples,
        physics_get_linear_acceleration,
        physics_get_angular_velocity,
    }

    /// <summary>
    /// True if exit was properly handled.
    /// </summary>
    private bool wasExitHandled;

    #region Sync
    /// <summary>
    /// The RACECAR(s) controlled by the user Python script(s).
    /// </summary>
    private Racecar[] racecars;

    /// <summary>
    /// The UDP client used to send packets to Python.
    /// </summary>
    private UdpClient udpClient;

    /// <summary>
    /// The UDP endpoints of the Python scripts(s) currently connected to RacecarSim.
    /// </summary>
    private List<IPEndPoint> pythonEndPoints;

    /// <summary>
    /// Connect the sync client to a Python script.
    /// </summary>
    /// <param name="pythonPort">The port used by the Python script.</param>
    private void ConnectSyncClient(int pythonPort)
    {
        this.pythonEndPoints.Add(new IPEndPoint(PythonInterface.ipAddress, pythonPort));
        LevelManager.UpdateNumConnectedPrograms(this.pythonEndPoints.Count);
    }

    /// <summary>
    /// Calls a Python function on all connected scripts.
    /// </summary>
    /// <param name="function">The Python function to call (start or update)</param>
    private void PythonCall(Header function)
    {
        for (int i = 0; i < this.pythonEndPoints.Count; i++)
        { 
            Racecar racecar = this.racecars[i];
            IPEndPoint endPoint = this.pythonEndPoints[i];

            // Tell Python what function to call
            this.udpClient.Send(new byte[] { (byte)function }, 1, endPoint);

            // Respond to API calls from Python until we receive a python_finished message
            bool pythonFinished = false;
            while (!pythonFinished)
            {
                // Receive a response from Python
                byte[] data = this.SafeRecieve(endPoint);
                if (data == null)
                {
                    break;
                }
                Header header = (Header)data[0];

                // Send the appropriate response if it was an API call, or break if it was a python_finished message
                byte[] sendData;
                switch (header)
                {
                    case Header.error:
                        HandleError($"Error code sent from the Python script controlling car {i}.");
                        break;

                    case Header.python_finished:
                        pythonFinished = true;
                        break;

                    case Header.racecar_get_delta_time:
                        sendData = BitConverter.GetBytes(Time.deltaTime);
                        this.udpClient.Send(sendData, sendData.Length, endPoint);
                        break;

                    case Header.camera_get_color_image:
                        this.SendFragmented(racecar.Camera.ColorImageRaw, 32, endPoint);
                        break;

                    case Header.camera_get_depth_image:
                        sendData = racecar.Camera.DepthImageRaw;
                        this.udpClient.Send(sendData, sendData.Length, endPoint);
                        break;

                    case Header.camera_get_width:
                        sendData = BitConverter.GetBytes(CameraModule.ColorWidth);
                        this.udpClient.Send(sendData, sendData.Length, endPoint);
                        break;

                    case Header.camera_get_height:
                        sendData = BitConverter.GetBytes(CameraModule.ColorHeight);
                        this.udpClient.Send(sendData, sendData.Length, endPoint);
                        break;

                    // Always return null controller data when in evaluation mode
                    case Header.controller_is_down:
                        Controller.Button buttonDown = (Controller.Button)data[1];
                        sendData = BitConverter.GetBytes(Controller.IsDown(buttonDown) && !LevelManager.IsEvaluation);
                        this.udpClient.Send(sendData, sendData.Length, endPoint);
                        break;

                    case Header.controller_was_pressed:
                        Controller.Button buttonPressed = (Controller.Button)data[1];
                        sendData = BitConverter.GetBytes(Controller.WasPressed(buttonPressed) && !LevelManager.IsEvaluation);
                        this.udpClient.Send(sendData, sendData.Length, endPoint);
                        break;

                    case Header.controller_was_released:
                        Controller.Button buttonReleased = (Controller.Button)data[1];
                        sendData = BitConverter.GetBytes(Controller.WasReleased(buttonReleased) && !LevelManager.IsEvaluation);
                        this.udpClient.Send(sendData, sendData.Length, endPoint);
                        break;

                    case Header.controller_get_trigger:
                        Controller.Trigger trigger = (Controller.Trigger)data[1];
                        float triggerValue = LevelManager.IsEvaluation ? 0 : Controller.GetTrigger(trigger);
                        sendData = BitConverter.GetBytes(triggerValue);
                        this.udpClient.Send(sendData, sendData.Length, endPoint);
                        break;

                    case Header.controller_get_joystick:
                        Controller.Joystick joystick = (Controller.Joystick)data[1];
                        Vector2 joystickValues = LevelManager.IsEvaluation ? Vector2.zero : Controller.GetJoystick(joystick);
                        sendData = new byte[sizeof(float) * 2];
                        Buffer.BlockCopy(new float[] { joystickValues.x, joystickValues.y }, 0, sendData, 0, sendData.Length);
                        this.udpClient.Send(sendData, sendData.Length, endPoint);
                        break;

                    case Header.drive_set_speed_angle:
                        racecar.Drive.Speed = BitConverter.ToSingle(data, 4);
                        racecar.Drive.Angle = BitConverter.ToSingle(data, 8);
                        break;

                    case Header.drive_stop:
                        racecar.Drive.Stop();
                        break;

                    case Header.drive_set_max_speed:
                        racecar.Drive.MaxSpeed = BitConverter.ToSingle(data, 4);
                        break;

                    case Header.lidar_get_num_samples:
                        sendData = BitConverter.GetBytes(Lidar.NumSamples);
                        this.udpClient.Send(sendData, sendData.Length, endPoint);
                        break;

                    case Header.lidar_get_samples:
                        sendData = new byte[sizeof(float) * Lidar.NumSamples];
                        Buffer.BlockCopy(racecar.Lidar.Samples, 0, sendData, 0, sendData.Length);
                        this.udpClient.Send(sendData, sendData.Length, endPoint);
                        break;

                    case Header.physics_get_linear_acceleration:
                        Vector3 linearAcceleration = racecar.Physics.LinearAccceleration;
                        sendData = new byte[sizeof(float) * 3];
                        Buffer.BlockCopy(new float[] { linearAcceleration.x, linearAcceleration.y, linearAcceleration.z }, 0, sendData, 0, sendData.Length);
                        this.udpClient.Send(sendData, sendData.Length, endPoint);
                        break;

                    case Header.physics_get_angular_velocity:
                        Vector3 angularVelocity = racecar.Physics.AngularVelocity;
                        sendData = new byte[sizeof(float) * 3];
                        Buffer.BlockCopy(new float[] { angularVelocity.x, angularVelocity.y, angularVelocity.z }, 0, sendData, 0, sendData.Length);
                        this.udpClient.Send(sendData, sendData.Length, endPoint);
                        break;

                    default:
                        Debug.LogError($">> Error: The function {header} is not supported by RacecarSim.");
                        break;
                }
            }
        }
    }

    /// <summary>
    /// Sends a large amount of data split across several packets.
    /// </summary>
    /// <param name="bytes">The bytes to send (must be divisible by numPackets).</param>
    /// <param name="numPackets">The number of packets to split the data across.</param>
    /// <param name="endPoint">The end point of the Python script to which to send data.</param>
    private void SendFragmented(byte[] bytes, int numPackets, IPEndPoint endPoint)
    {
        int blockSize = bytes.Length / numPackets;
        byte[] sendData = new byte[blockSize];
        for (int i = 0; i < numPackets; i++)
        {
            Buffer.BlockCopy(bytes, i * blockSize, sendData, 0, blockSize);
            this.udpClient.Send(sendData, sendData.Length, endPoint);

            byte[] response = this.SafeRecieve(endPoint);
            if (response == null || (Header)response[0] != Header.python_send_next)
            {
                this.HandleError("Unity and Python became out of sync while sending a block message.");
                break;
            }
        }
    }

    /// <summary>
    /// Receives a packet from Python and safely handles UDP exceptions (broken socket, timeout, etc.).
    /// </summary>
    /// <param name="endPoint">The end point of the Python script from which to receive data.</param>
    /// <returns>The data in the packet, or null if an error occurred.</returns>
    private byte[] SafeRecieve(IPEndPoint endPoint)
    {
        try
        {
            return this.udpClient.Receive(ref endPoint);
        }
        catch (SocketException e)
        {
            if (e.SocketErrorCode == SocketError.TimedOut)
            {
                this.HandleError("No message received from Python within the alloted time.");
                Debug.LogError(">> Troubleshooting:" +
                    "\n1. Make sure that your Python program does not block or wait. For example, your program should never call time.sleep()." +
                    "\n2. Make sure that your program is not too computationally intensive. Your start and update functions should be able to run in under 10 milliseconds." +
                    "\n3. Make sure that your Python program did not crash or close unexpectedly." +
                    "\n4. Unless you experience an error, do not force-quit your Python application (ctrl+c or ctrl+d).  Instead, end the simulation by pressing the start and back button simultaneously on your Xbox controller (escape and enter on keyboard).");
            }
            else
            {
                this.HandleError("An error occurred when attempting to receive data from Python.");
            }
        }
        return null;
    }

    /// <summary>
    /// Sends an error response to all Python scripts and passes the error to the LevelManager.
    /// </summary>
    /// <param name="errorText">The error text to show.</param>
    private void HandleError(string errorText)
    {
        foreach (IPEndPoint endPoint in this.pythonEndPoints)
        {
            try
            {
                this.udpClient.Send(new byte[] { (byte)Header.error }, 1, endPoint);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
        LevelManager.HandleError(errorText);

        this.pythonEndPoints.Clear();
        LevelManager.UpdateNumConnectedPrograms(this.pythonEndPoints.Count);
    }
    #endregion

    #region Async
    /// <summary>
    /// The UDP client used to handle async API calls from Python.
    /// </summary>
    private UdpClient udpClientAsync;

    /// <summary>
    /// A thread containing a UDP client to process asynchronous API calls from Python.
    /// </summary>
    private Thread asyncClientThread;

    /// <summary>
    /// Creates a UDP client to process asynchronous API calls from Python (for use by Jupyter).
    /// </summary>
    private void ProcessAsyncCalls()
    {
        this.udpClientAsync = new UdpClient(new IPEndPoint(PythonInterface.ipAddress, PythonInterface.unityPortAsync));
        this.udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        while (true)
        {
            IPEndPoint receiveEndPoint = new IPEndPoint(PythonInterface.ipAddress, 0);
            byte[] data = this.udpClientAsync.Receive(ref receiveEndPoint);
            Header header = (Header)data[0];

            Racecar racecar = this.racecars[0];

            byte[] sendData;
            switch (header)
            {
                case Header.connect:
                    this.ConnectSyncClient(receiveEndPoint.Port);
                    sendData = new byte[1] { (byte)Header.connect };
                    this.udpClientAsync.Send(sendData, sendData.Length, receiveEndPoint);
                    break;

                case Header.camera_get_color_image:
                    this.SendFragmentedAsync(racecar.Camera.GetColorImageRawAsync(), 32, receiveEndPoint);
                    break;

                case Header.camera_get_depth_image:
                    sendData = racecar.Camera.GetDepthImageRawAsync();
                    this.udpClientAsync.Send(sendData, sendData.Length, receiveEndPoint);
                    break;

                case Header.lidar_get_samples:
                    sendData = new byte[sizeof(float) * Lidar.NumSamples];
                    Buffer.BlockCopy(racecar.Lidar.Samples, 0, sendData, 0, sendData.Length);
                    this.udpClientAsync.Send(sendData, sendData.Length, receiveEndPoint);
                    break;

                default:
                    Debug.LogError($">> Error: The function {header} is not supported by RacecarSim for async calls.");
                    break;
            }
        }
    }

    /// <summary>
    /// Sends a large amount of data split across several packets via the async client.
    /// </summary>
    /// <param name="bytes">The bytes to send (must be divisible by numPackets).</param>
    /// <param name="numPackets">The number of packets to split the data across.</param>
    private void SendFragmentedAsync(byte[] bytes, int numPackets, IPEndPoint destination)
    {
        int blockSize = bytes.Length / numPackets;
        byte[] sendData = new byte[blockSize];
        for (int i = 0; i < numPackets; i++)
        {
            Buffer.BlockCopy(bytes, i * blockSize, sendData, 0, blockSize);
            this.udpClientAsync.Send(sendData, sendData.Length, destination);

            byte[] response = this.udpClientAsync.Receive(ref destination);
            if (response == null || (Header)response[0] != Header.python_send_next)
            {
                this.udpClientAsync.Send(new byte[] { (byte)Header.error }, 1, destination);
                break;
            }
        }
    }
    #endregion
}
