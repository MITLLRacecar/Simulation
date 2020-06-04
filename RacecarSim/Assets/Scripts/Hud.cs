using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.IO;
using UnityEngine.UI;
using System;

public class Hud : MonoBehaviour
{
    public Racecar Racecar;

    private enum Texts
    {
        TrueSpeed = 4,
        LinearAcceleration = 8,
        AngularVelocity = 11
    }

    private enum Images
    {
        ColorFeed = 2,
        DepthFeed = 4,
        LidarMap = 6
    }

    private Text[] texts;
    private RawImage[] images;

    #region Constants
    public static readonly Color SensorBackgroundColor = new Color(0.2f, 0.2f, 0.2f);
    private const int lidarMapScale = 4;
    #endregion

    private void Start()
    {
        this.texts = GetComponentsInChildren<Text>();
        this.images = this.GetComponentsInChildren<RawImage>();

        this.images[Images.LidarMap.GetHashCode()].texture = new Texture2D(CameraModule.ColorWidth / Hud.lidarMapScale, CameraModule.ColorHeight / Hud.lidarMapScale);
        this.images[Images.DepthFeed.GetHashCode()].texture = new Texture2D(CameraModule.DepthWidth, CameraModule.DepthHeight);
    }

    private void Update()
    {
        this.texts[Texts.TrueSpeed.GetHashCode()].text = this.Racecar.GetComponent<Rigidbody>().velocity.magnitude.ToString("F2");
        this.texts[Texts.LinearAcceleration.GetHashCode()].text = FormatVector3(this.Racecar.Physics.get_linear_acceleration());
        this.texts[Texts.AngularVelocity.GetHashCode()].text = FormatVector3(this.Racecar.Physics.get_angular_velocity());

        this.Racecar.Lidar.VisualizeLidar((Texture2D)this.images[Images.LidarMap.GetHashCode()].texture);
        this.Racecar.Camera.VisualizeDepth((Texture2D)this.images[Images.DepthFeed.GetHashCode()].texture);
    }

    private string FormatVector3(Vector3 vector)
    {
        return $"({FormatFloat(vector.x)},{FormatFloat(vector.y)},{FormatFloat(vector.z)})";       
    }

    private string FormatFloat(float value)
    {
        string str = value.ToString("F1");
        if (str[0] != '-')
        {
            return $" {str}";
        }

        return str;
    }
}
