using System;
using System.Collections.Generic;
using System.Linq;
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
    /// The current version of the protocol used to communicate with racecar_core.
    /// </summary>
    /// <remarks>
    /// When the communication protocol between RacecarSim and racecar_core are changed, this version number
    /// should be incremented both here and in racecar_core_sim.py. This allows us to immediately detect
    /// if a user attempts to use incompatible versions of RacecarSim and racecar_core.
    /// </remarks>
    private const int version = 1;

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
    /// <summary>
    /// An array in which each entry indicates whether the racecar of the same index is connected to a Python script.
    /// </summary>
    public bool[] ConnectedPrograms
    {
        get
        {
            return this.pythonEndPoints.Select(x => x != null).ToArray();
        }
    }

    public PythonInterface()
    {
        this.wasExitHandled = false;
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
                    if (endpoint != null)
                    {
                        this.udpClient.Send(new byte[] { (byte)Header.unity_exit }, 1, endpoint);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Unable to send exit command to Python. Error: {e}");
                }
            }
            this.pythonEndPoints.Clear();
            LevelManager.UpdateConnectedPrograms();

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
    /// Header bytes used in the communication protocol.
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
        python_exit,
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
    /// The error codes used in the communication protocol.
    /// </summary>
    private enum Error
    {
        generic,
        timeout,
        python_exception,
        no_free_car,
        python_outdated,
        racecarsim_outdated,
        fragment_mismatch
    }

    /// <summary>
    /// True if exit was properly handled.
    /// </summary>
    private bool wasExitHandled;

    #region Sync
    /// <summary>
    /// The UDP client used to send packets to Python.
    /// </summary>
    private readonly UdpClient udpClient;

    /// <summary>
    /// The UDP endpoints of the Python scripts(s) currently connected to RacecarSim.
    /// </summary>
    private readonly List<IPEndPoint> pythonEndPoints;

    /// <summary>
    /// Connect the sync client to a Python script.
    /// </summary>
    /// <param name="pythonPort">The port used by the Python script.</param>
    /// <returns>The index of the car with which the script is paired, or null if the script could not be paired.</returns>
    private int? ConnectSyncClient(int pythonPort)
    {
        IPEndPoint endPoint = new IPEndPoint(PythonInterface.ipAddress, pythonPort);
        int index = -1;

        // Replace the first null end point, if any exist
        for (int i = 0; i < this.pythonEndPoints.Count; i++)
        {
            if (this.pythonEndPoints[i] == null)
            {
                this.pythonEndPoints[i] = endPoint;
                index = i;
                break;
            }
        }

        // Otherwise, add the end point to the end of the list
        if (index == -1)
        {
            if (this.pythonEndPoints.Count < LevelManager.NumPlayers)
            {
                index = this.pythonEndPoints.Count;
                this.pythonEndPoints.Add(endPoint);
            }
            else
            {
                // Every race car is already connected
                return null;
            }
        }
        
        LevelManager.UpdateConnectedPrograms();
        return index;
    }

    /// <summary>
    /// Disconnects a Python script from the sync client.
    /// </summary>
    /// <param name="pythonPort">The port of the Python script to remove.</param>
    private void RemoveSyncClient(int pythonPort)
    {
        // Set the endpoint to null rather than removing it from the list to maintain
        // the mapping of remaining endpoints to cars
        for (int i = 0; i < this.pythonEndPoints.Count; i++)
        {
            if (this.pythonEndPoints[i]?.Port == pythonPort)
            {
                this.pythonEndPoints[i] = null;
                LevelManager.GetCar(i).Drive.Stop();
                break;
            }
        }

        // We can safely remove any trailing null endpoints at the end of the list
        for (int i = this.pythonEndPoints.Count - 1; i >= 0; i--)
        {
            if (this.pythonEndPoints[i] == null)
            {
                this.pythonEndPoints.RemoveAt(i);
            }
            else
            {
                break;
            }
        }

        LevelManager.UpdateConnectedPrograms();
    }

    /// <summary>
    /// Calls a Python function on all connected scripts.
    /// </summary>
    /// <param name="function">The Python function to call (start or update)</param>
    private void PythonCall(Header function)
    {
        for (int i = 0; i < this.pythonEndPoints.Count; i++)
        {
            if (this.pythonEndPoints[i] == null)
            {
                continue;
            }

            Racecar racecar = LevelManager.GetCar(i);
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

                bool shouldSendController = LevelManager.LevelManagerMode != LevelManagerMode.Race || Settings.CheatMode;

                // Send the appropriate response if it was an API call, or break if it was a python_finished message
                byte[] sendData;
                switch (header)
                {
                    case Header.error:
                        Error errorCode = (Error)data[1];
                        HandleError($"Error code [{errorCode}] sent from the Python script controlling car {i}.", errorCode);
                        pythonFinished = true;
                        break;

                    case Header.python_finished:
                        pythonFinished = true;
                        break;

                    case Header.python_exit:
                        this.RemoveSyncClient(endPoint.Port);
                        pythonFinished = true;
                        break;

                    case Header.racecar_get_delta_time:
                        sendData = BitConverter.GetBytes(Time.deltaTime);
                        this.udpClient.Send(sendData, sendData.Length, endPoint);
                        break;

                    case Header.camera_get_color_image:
                        pythonFinished = !this.SendFragmented(racecar.Camera.ColorImageRaw, 32, endPoint);
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

                    // Always return null controller data when in race mode (except in cheat mode)
                    case Header.controller_is_down:
                        Controller.Button buttonDown = (Controller.Button)data[1];
                        sendData = BitConverter.GetBytes(Controller.IsDown(buttonDown) && shouldSendController);
                        this.udpClient.Send(sendData, sendData.Length, endPoint);
                        break;

                    case Header.controller_was_pressed:
                        Controller.Button buttonPressed = (Controller.Button)data[1];
                        sendData = BitConverter.GetBytes(Controller.WasPressed(buttonPressed) && shouldSendController);
                        this.udpClient.Send(sendData, sendData.Length, endPoint);
                        break;

                    case Header.controller_was_released:
                        Controller.Button buttonReleased = (Controller.Button)data[1];
                        sendData = BitConverter.GetBytes(Controller.WasReleased(buttonReleased) && shouldSendController);
                        this.udpClient.Send(sendData, sendData.Length, endPoint);
                        break;

                    case Header.controller_get_trigger:
                        Controller.Trigger trigger = (Controller.Trigger)data[1];
                        float triggerValue = shouldSendController ? Controller.GetTrigger(trigger) : 0;
                        sendData = BitConverter.GetBytes(triggerValue);
                        this.udpClient.Send(sendData, sendData.Length, endPoint);
                        break;

                    case Header.controller_get_joystick:
                        Controller.Joystick joystick = (Controller.Joystick)data[1];
                        Vector2 joystickValues = shouldSendController ? Controller.GetJoystick(joystick) : Vector2.zero;
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
                        pythonFinished = true;
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
    /// <returns>True if the entire message was sent successfully.</returns>
    private bool SendFragmented(byte[] bytes, int numPackets, IPEndPoint endPoint)
    {
        int blockSize = bytes.Length / numPackets;
        byte[] sendData = new byte[blockSize];
        for (int i = 0; i < numPackets; i++)
        {
            Buffer.BlockCopy(bytes, i * blockSize, sendData, 0, blockSize);
            this.udpClient.Send(sendData, sendData.Length, endPoint);

            byte[] response = this.SafeRecieve(endPoint);
            Header responseHeader = (Header)response[0];
            switch(responseHeader)
            {
                case Header.python_send_next:
                    continue;

                case Header.python_exit:
                    this.RemoveSyncClient(endPoint.Port);
                    return false;

                default:
                    this.HandleError("Unity and Python became out of sync while sending a block message.", Error.fragment_mismatch);
                    return false;
            }
        }
        return true;
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
                this.HandleError("No message received from Python within the alloted time.", Error.timeout);
                Debug.LogError(">> Troubleshooting:" +
                    "\n1. Make sure that your Python program does not block or wait. For example, your program should never call time.sleep()." +
                    "\n2. Make sure that your program is not too computationally intensive. Your start and update functions should be able to run in under 10 milliseconds." +
                    "\n3. Make sure that your Python program did not crash or close unexpectedly." +
                    "\n4. Unless you experience an error, do not force-quit your Python application (ctrl+c or ctrl+d).  Instead, end the simulation by pressing the start and back button simultaneously on your Xbox controller (escape and enter on keyboard).");
            }
            else
            {
                this.HandleError("An error occurred when attempting to receive data from Python.", Error.generic);
                Debug.LogError($"SocketException: [{e}]");
            }
        }
        return null;
    }

    /// <summary>
    /// Sends an error response to all Python scripts and passes the error to the LevelManager.
    /// </summary>
    /// <param name="errorText">The error text to show.</param>
    /// <param name="errorCode">The error code to send to the Python scripts.</param>
    private void HandleError(string errorText, Error errorCode)
    {
        foreach (IPEndPoint endPoint in this.pythonEndPoints)
        {
            try
            {
                this.udpClient.Send(new byte[] { (byte)Header.error, (byte)errorCode }, 1, endPoint);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
        LevelManager.HandleError(errorText);

        this.pythonEndPoints.Clear();
        LevelManager.UpdateConnectedPrograms();
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
    private readonly Thread asyncClientThread;

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

            Racecar racecar = LevelManager.GetCar();

            byte[] sendData;
            switch (header)
            {
                case Header.connect:
                    int pythonVersion = data.Length > 1 ? data[1] : 0;
                    if (PythonInterface.version == pythonVersion)
                    {
                        int? index = this.ConnectSyncClient(receiveEndPoint.Port);
                        if (index.HasValue)
                        {
                            sendData = new byte[] { (byte)Header.connect, (byte)index };
                        }
                        else
                        {
                            // TODO: display this message to the screen?
                            Debug.LogError($"Attempted to connect a Python script from port [{receiveEndPoint.Port}], but every racecar already has a connected Python script.");
                            sendData = new byte[] { (byte)Header.error, (byte)Error.no_free_car };
                        }
                    }
                    else if (PythonInterface.version > pythonVersion)
                    {
                        // TODO: display this message to the screen?
                        Debug.LogError("The Python script is using an outdated and incompatible version of racecar_core. Please update you Python racecar libraries to the newest version.");
                        sendData = new byte[] { (byte)Header.error, (byte)Error.python_outdated };
                    }
                    else
                    {
                        // TODO: display this message to the screen?
                        Debug.LogError("The Python script is using a newer and incompatible version of racecar_core. Please download the newest version of RacecarSim.");
                        sendData = new byte[] { (byte)Header.error, (byte)Error.racecarsim_outdated };
                    }
                    this.udpClientAsync.Send(sendData, sendData.Length, receiveEndPoint);
                    break;

                case Header.python_exit:
                    this.RemoveSyncClient(receiveEndPoint.Port);
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
                this.udpClientAsync.Send(new byte[] { (byte)Header.error, (byte)Error.fragment_mismatch }, 1, destination);
                break;
            }
        }
    }
    #endregion
}
