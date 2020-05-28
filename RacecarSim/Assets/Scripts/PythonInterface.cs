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

    private enum FunctionCode
    {
        go,
        set_start_update,
        get_delta_time,
        set_update_slow_time,
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
        sound_set_speaker,
        sound_set_mic,
        sound_set_output_stream,
        sound_set_input_stream,
        sound_play_audio,
        sound_record_audio,
        sound_play,
        sound_rec,
        sound_set_file,
        sound_list_devices
    }

    public enum DataCode
    {
        start,
        update,
        updateSlow,
        dataInt,
        dataBool,
        dataFloat,
        dataColorImage,
        dataDepthImage,
        dataLidar
    }

    #region Constants
    private const int functionPort = 5065;
    private const int clockPort = 5066;
    private static readonly IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
    #endregion

    private Thread receiveThread;

    private bool running;


    public void HandleExit()
    {
        this.running = false;
    }

    private void Start()
    {
        Instance = this;
        InitUDP();
    }

    private void InitUDP()
    {
        this.running = true;
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    private void ReceiveData()
    {
        IPEndPoint endpoint = new IPEndPoint(PythonInterface.ipAddress, PythonInterface.functionPort);
        using (UdpClient client = new UdpClient(functionPort))
        {
            while (this.running)
            {
                byte[] data = client.Receive(ref endpoint);
                FunctionCode functionCode = (FunctionCode)data[0];

                byte[] sendData;

                switch (functionCode)
                {
                    case FunctionCode.camera_get_width:
                        sendData = BitConverter.GetBytes(this.Racecar.Camera.get_width());
                        client.Send(sendData, sendData.Length, endpoint);
                        break;

                    case FunctionCode.camera_get_height:
                        sendData = BitConverter.GetBytes(this.Racecar.Camera.get_height());
                        client.Send(sendData, sendData.Length, endpoint);
                        break;

                    case FunctionCode.controller_is_down:
                        Controller.Button button = (Controller.Button)BitConverter.ToInt32(data, 4);
                        sendData = BitConverter.GetBytes(this.Racecar.Controller.is_down(button));
                        client.Send(sendData, sendData.Length, endpoint);
                        break;

                    case FunctionCode.drive_set_speed_angle:
                        float speed = BitConverter.ToSingle(data, 4);
                        float angle = BitConverter.ToSingle(data, 8);
                        this.Racecar.Drive.set_speed_angle(speed, angle);
                        break;

                    case FunctionCode.drive_stop:
                        this.Racecar.Drive.stop();
                        break;

                    case FunctionCode.drive_set_max_speed_scale_factor:
                        float forwardFactor = BitConverter.ToSingle(data, 4);
                        float backFactor = BitConverter.ToSingle(data, 8);
                        this.Racecar.Drive.set_max_speed_scale_factor(new float[] { forwardFactor, backFactor });
                        break;

                    case FunctionCode.lidar_get_length:
                        sendData = BitConverter.GetBytes(this.Racecar.Lidar.get_length());
                        client.Send(sendData, sendData.Length, endpoint);
                        break;

                    //case FunctionCode.physics_get_angular_velocity:
                    //    Vector3 angularVelocity = this.Racecar.Physics.get_angular_velocity();
                    //    sendData = BitConverter.GetBytes();
                    //    client.Send(sendData, sendData.Length, endpoint);
                    //    break;

                    default:
                        print($"The function {functionCode} is not supported by the RACECAR-MN Unity simulation");
                        break;
                }

            }
        }
    }
}
