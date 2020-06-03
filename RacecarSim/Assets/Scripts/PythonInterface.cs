using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

public class PythonInterface : MonoBehaviour
{
    public Racecar Racecar;

    public static PythonInterface Instance;

    private enum Header
    {
        error,
        unity_start,
        unity_update,
        unity_exit,
        python_finished,
        python_send_next,
        racecar_go,
        racecar_set_start_update,
        racecar_get_delta_time,
        racecar_set_update_slow_time,
        camera_get_image,
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
        drive_set_max_speed_scale_factor,
        gpio_pin_mode,
        gpio_pin_write,
        lidar_get_length,
        lidar_get_ranges,
        physics_get_linear_acceleration,
        physics_get_angular_velocity,
    }

    #region Constants
    private const int unityPort = 5065;
    private const int pythonPort = 5066;
    private static readonly IPAddress ipAddress = IPAddress.Parse("127.0.0.1");

    private const int timeoutTime = 1000;

    private const int maxPacketSize = 65507;
    #endregion

    private UdpClient client;
    private IPEndPoint pythonEndpoint;
    private IPEndPoint unityEndpoint;

    public void HandleExit()
    {
        this.client.Send(new byte[] { (byte)Header.unity_exit.GetHashCode() }, 1);
    }

    public void PythonStart()
    {
        this.PythonCall(Header.unity_start);
    }

    public void PythonUpdate()
    {
        this.PythonCall(Header.unity_update);
    }

    private void Start()
    {
        // QualitySettings.vSyncCount = 2;

        Instance = this;
        this.unityEndpoint = new IPEndPoint(PythonInterface.ipAddress, PythonInterface.unityPort);
        this.pythonEndpoint = new IPEndPoint(PythonInterface.ipAddress, PythonInterface.pythonPort);
        this.client = new UdpClient(unityEndpoint);
        this.client.Connect(this.pythonEndpoint);
        this.client.Client.ReceiveTimeout = PythonInterface.timeoutTime;
    }

    private void PythonCall(Header function)
    {
        // Tell Python what fuction to call
        this.client.Send(new byte[] { (byte)function.GetHashCode() }, 1);

        // Respond to API calls until we receive a Header.python_finished
        bool pythonFinished = false;

        while (!pythonFinished)
        {
            byte[] data = this.SafeRecieve();
            if (data == null)
            {
                break;
            }

            Header header = (Header)data[0];
            byte[] sendData;
            switch (header)
            {
                case Header.python_finished:
                    pythonFinished = true;
                    break;

                case Header.racecar_get_delta_time:
                    sendData = BitConverter.GetBytes(Time.deltaTime);
                    client.Send(sendData, sendData.Length);
                    break;

                case Header.camera_get_image:
                    this.SendFragmented(this.Racecar.Camera.ColorImageRaw, 32);
                    break;

                case Header.camera_get_depth_image:
                    client.Send(this.Racecar.Camera.DepthImageRaw, this.Racecar.Camera.DepthImageRaw.Length);
                    break;

                case Header.camera_get_width:
                    sendData = BitConverter.GetBytes(CameraModule.ColorWidth);
                    client.Send(sendData, sendData.Length);
                    break;

                case Header.camera_get_height:
                    sendData = BitConverter.GetBytes(CameraModule.ColorHeight);
                    client.Send(sendData, sendData.Length);
                    break;

                case Header.controller_is_down:
                    Controller.Button buttonDown = (Controller.Button)data[1];
                    sendData = BitConverter.GetBytes(this.Racecar.Controller.is_down(buttonDown));
                    client.Send(sendData, sendData.Length);
                    break;

                case Header.controller_was_pressed:
                    Controller.Button buttonPressed = (Controller.Button)data[1];
                    sendData = BitConverter.GetBytes(this.Racecar.Controller.was_pressed(buttonPressed));
                    client.Send(sendData, sendData.Length);
                    break;

                case Header.controller_was_released:
                    Controller.Button buttonReleased = (Controller.Button)data[1];
                    sendData = BitConverter.GetBytes(this.Racecar.Controller.was_released(buttonReleased));
                    client.Send(sendData, sendData.Length);
                    break;

                case Header.controller_get_trigger:
                    Controller.Trigger trigger = (Controller.Trigger)data[1];
                    sendData = BitConverter.GetBytes(this.Racecar.Controller.get_trigger(trigger));
                    client.Send(sendData, sendData.Length);
                    break;

                case Header.controller_get_joystick:
                    Controller.Joystick joystick = (Controller.Joystick)data[1];
                    Vector2 joystickValues = this.Racecar.Controller.get_joystick(joystick);
                    sendData = new byte[sizeof(float) * 2];
                    Buffer.BlockCopy(new float[] { joystickValues.x, joystickValues.y }, 0, sendData, 0, sendData.Length);
                    client.Send(sendData, sendData.Length);
                    break;

                case Header.drive_set_speed_angle:
                    float speed = BitConverter.ToSingle(data, 4);
                    float angle = BitConverter.ToSingle(data, 8);
                    this.Racecar.Drive.set_speed_angle(speed, angle);
                    break;

                case Header.drive_stop:
                    this.Racecar.Drive.stop();
                    break;

                case Header.drive_set_max_speed_scale_factor:
                    float forwardFactor = BitConverter.ToSingle(data, 4);
                    float backFactor = BitConverter.ToSingle(data, 8);
                    this.Racecar.Drive.set_max_speed_scale_factor(new float[] { forwardFactor, backFactor });
                    break;

                case Header.lidar_get_length:
                    sendData = BitConverter.GetBytes(this.Racecar.Lidar.get_length());
                    client.Send(sendData, sendData.Length);
                    break;

                case Header.lidar_get_ranges:
                    float[] ranges = this.Racecar.Lidar.get_ranges();
                    sendData = new byte[sizeof(float) * ranges.Length];
                    Buffer.BlockCopy(ranges, 0, sendData, 0, sendData.Length);
                    print($"float: {ranges[0]}");
                    print(string.Format("bytes: {0:X} {1:X} {2:X} {3:X}", sendData[0], sendData[1], sendData[2], sendData[3]));
                    client.Send(sendData, sendData.Length);
                    break;

                case Header.physics_get_linear_acceleration:
                    Vector3 linearAcceleration = this.Racecar.Physics.get_linear_acceleration();
                    sendData = new byte[sizeof(float) * 3];
                    Buffer.BlockCopy(new float[] { linearAcceleration.x, linearAcceleration.y, linearAcceleration.z }, 0, sendData, 0, sendData.Length);
                    client.Send(sendData, sendData.Length);
                    break;

                case Header.physics_get_angular_velocity:
                    Vector3 angularVelocity = this.Racecar.Physics.get_angular_velocity();
                    sendData = new byte[sizeof(float) * 3];
                    Buffer.BlockCopy(new float[] { angularVelocity.x, angularVelocity.y, angularVelocity.z }, 0, sendData, 0, sendData.Length);
                    client.Send(sendData, sendData.Length);
                    break;

                default:
                    print($"The function {header} is not supported by the RACECAR-MN Unity simulation");
                    break;
            }
        }
    }

    private void SendFragmented(byte[] bytes, int numFragments)
    {
        int blockSize = bytes.Length / numFragments;
        byte[] sendData = new byte[blockSize];
        for (int i = 0; i < numFragments; i++)
        {
            Buffer.BlockCopy(bytes, i * blockSize, sendData, 0, blockSize);
            client.Send(sendData, sendData.Length);

            byte[] response = this.SafeRecieve();
            if (response == null)
            {
                break;
            }
            else if ((Header)response[0] != Header.python_send_next)
            {
                print(">> Error: Unity and Python became out of sync when sending a block message.  Returning to default drive mode.");
                this.Racecar.EnterDefaultDrive();
                break;
            }
        }
    }

    private byte[] SafeRecieve()
    {
        try
        {
            return client.Receive(ref this.pythonEndpoint);
        }
        catch (SocketException e)
        {
            if (e.SocketErrorCode == SocketError.TimedOut)
            {
                print(">> Error: No message received from Python within the alloted time.  Returning to default drive mode.");
            }
            else
            {
                print(">> Error: An error occured when attempting to receive data from Python.  Returning to default drive mode.");
            }
            this.Racecar.EnterDefaultDrive();
        }
        return null;
    }
}
