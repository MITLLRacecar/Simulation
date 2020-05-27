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
        TrueSpeed = 1,
        LinearAcceleration = 5,
        AngularVelocity = 8
    }

    private enum Images
    {
        ColorFeed = 1,
        DepthFeed = 3,
        LidarMap = 5
    }

    private Text[] texts;
    private RawImage[] images;

    private static readonly int[] lidarMapDimensions = { 128, 128 };

    private void Start()
    {
        this.texts = this.GetComponentsInChildren<Text>();
        this.images = this.GetComponentsInChildren<RawImage>();

        this.images[Images.LidarMap.GetHashCode()].texture = new Texture2D(Hud.lidarMapDimensions[0], Hud.lidarMapDimensions[1]);
        this.images[Images.DepthFeed.GetHashCode()].texture = new Texture2D(CameraModule.Width, CameraModule.Height);
    }

    private void Update()
    {
        this.texts[Texts.TrueSpeed.GetHashCode()].text = this.Racecar.GetComponent<Rigidbody>().velocity.magnitude.ToString("F1");
        this.texts[Texts.LinearAcceleration.GetHashCode()].text = FormatVector3(this.Racecar.Physics.get_linear_acceleration());
        this.texts[Texts.AngularVelocity.GetHashCode()].text = FormatVector3(this.Racecar.Physics.get_angular_velocity());

        this.Racecar.Lidar.UpdateMap((Texture2D)this.images[Images.LidarMap.GetHashCode()].texture);
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
