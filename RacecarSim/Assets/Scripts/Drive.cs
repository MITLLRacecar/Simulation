using System;
using UnityEngine;

public class Drive : MonoBehaviour
{
    #region Set in Unity
    public GameObject[] Wheels = new GameObject[4];
    public WheelCollider[] WheelColliders = new WheelCollider[4];
    #endregion

    #region Constants
    private const float torqueScale = 0.6f;
    private const float maxDriveAngle = 20;
    #endregion

    #region Public Interface
    public float Speed { get; set; } = 0;
    
    public float Angle { get; set; } = 0;

    public float MaxSpeed { get; set; } = 0.25f;

    public void Stop()
    {
        this.Speed = 0;
        this.Angle = 0;
    }
    #endregion

    private enum WheelPosition
    {
        FrontLeft,
        FrontRight,
        BackLeft,
        BackRight
    }

    private void Start()
    {
        foreach (WheelCollider wheel in this.WheelColliders)
        {
            wheel.ConfigureVehicleSubsteps(0.5f, 15, 20);
        }
    }

    private void FixedUpdate()
    {
        float torque = this.Speed * this.MaxSpeed * Drive.torqueScale;
        this.WheelColliders[WheelPosition.BackLeft.GetHashCode()].motorTorque = torque;
        this.WheelColliders[WheelPosition.BackRight.GetHashCode()].motorTorque = torque;

        float driveAngle = this.Angle * Drive.maxDriveAngle;
        this.WheelColliders[WheelPosition.FrontLeft.GetHashCode()].steerAngle = driveAngle;
        this.WheelColliders[WheelPosition.FrontRight.GetHashCode()].steerAngle = driveAngle;
    }
}
