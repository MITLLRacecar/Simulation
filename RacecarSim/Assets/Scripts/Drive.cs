using System;
using UnityEngine;

public class Drive : MonoBehaviour
{
    private enum WheelPosition
    {
        FrontLeft,
        FrontRight,
        BackLeft,
        BackRight
    }

    public GameObject[] Wheels = new GameObject[4];
    public WheelCollider[] WheelColliders = new WheelCollider[4];

    private static readonly float[] unitySpeedScaleFactor = { 1f / 0.25f, 1f / 0.33f };
    public const float unityMaxAngle = 20;

    private float[] userSpeedScaleFactor = { 0.25f, 0.33f };
    private float curSpeed = 0;
    private float curAngle = 0;

    private void Update()
    {
        this.WheelColliders[WheelPosition.BackLeft.GetHashCode()].motorTorque = this.curSpeed;
        this.WheelColliders[WheelPosition.BackRight.GetHashCode()].motorTorque = this.curSpeed;

        this.WheelColliders[WheelPosition.FrontLeft.GetHashCode()].steerAngle = this.curAngle;
        this.WheelColliders[WheelPosition.FrontRight.GetHashCode()].steerAngle = this.curAngle;
    }

    public void set_speed_angle(float speed, float angle)
    {
        int isBackward = Convert.ToInt32(speed < 0);
        this.curSpeed = speed * this.userSpeedScaleFactor[isBackward] * Drive.unitySpeedScaleFactor[isBackward];
        this.curAngle = angle;
    }

    public void stop()
    {
        this.set_speed_angle(0, 0);
    }

    public void set_max_speed_scale_factor(float[] scale_factor)
    {
        this.userSpeedScaleFactor = scale_factor;
    }

}
