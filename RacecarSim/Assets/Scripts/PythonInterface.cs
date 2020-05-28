using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class PythonInterface : MonoBehaviour
{
    public Racecar Racecar;

    private enum FunctionCode
    {
        go,
        set_start_update,
        get_delta_time,
        set_update_slow_time,
        get_image,
        get_depth_image,
        get_width,
        get_height,
        is_down,
        was_pressed,
        was_released,
        get_trigger,
        get_joystick,
        show_image,
        set_speed_angle,
        stop,
        set_max_speed_scale_factor,
        pin_mode,
        pin_write,
        get_length,
        get_ranges,
        get_linear_acceleration,
        get_angular_velocity,
        set_speaker,
        set_mic,
        set_output_stream,
        set_input_stream,
        play_audio,
        record_audio,
        play,
        rec,
        set_file,
        list_devices
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
    private const int sendPort = 5065;
    private const int receivePort = 5066;
    private static readonly IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
    #endregion

    private Thread receiveThread;
    IPEndPoint receiveEndPoint = new IPEndPoint(ipAddress, recievePort);

    private UdpClient sender = new UdpClient();
    private IPEndPoint sendEndPoint = new IPEndPoint(ipAddress, sendPort);

    void Start()
    {
        InitUDP();
    }

    private void InitUDP()
    {
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    private void ReceiveData()
    {
        using (UdpClient receiver = new UdpClient(receivePort))
        {
            while (true)
            {
                try
                {
                    byte[] data = receiver.Receive(ref this.receiveEndPoint);
                    FunctionCode functionCode = (FunctionCode)data[0];

                    switch (functionCode)
                    {
                        case FunctionCode.set_speed_angle:
                            this.Racecar.Drive.set_speed_angle(1, 1);
                            break;

                        default:
                            print($"The function {functionCode} is not supported by the Unity Layer");
                            break;
                    }

                }
                catch (Exception e)
                {
                    print(e.ToString());
                }
            }
        }
    }

    //public void SendBool(DataCode dataCode, bool value)
    //{
    //    try
    //    {
    //        byte[] data = new byte[sizeof(bool) + 1];
    //        data[0] = (byte)dataCode;
    //        data.CopyTo(BitConverter.GetBytes(value), 1);
    //        sender.Send(data, data.Length, this.receiveEndPoint);
    //    }
    //    catch (Exception e)
    //    {
    //        print(e.ToString());
    //    }
    //}

    public void SendData()
    {
        using (UdpClient sender = new UdpClient())
        {

            for (int i = 0; i < 30; i++)
            {
                try
                {
                    byte[] data = Encoding.UTF8.GetBytes("Hello Python!");
                    sender.Send(data, data.Length, iPEndPoint);

                    print("Sending data to Python...");
                }
                catch (Exception e)
                {
                    print(e.ToString());
                }
                Thread.Sleep(1000);
            }
        }
    }
}
