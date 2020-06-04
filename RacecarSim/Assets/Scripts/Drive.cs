using System;
using UnityEngine;

public class Drive : MonoBehaviour
{
    #region Set in Unity
    public GameObject[] Wheels = new GameObject[4];
    public WheelCollider[] WheelColliders = new WheelCollider[4];
    #endregion

    #region Constants
    private const float torqueScale = 10000.0f;
    private const float brakeTorqueScale = 1f;
    private const float maxDriveAngle = 20;

    private const int vehicalSubsteps = 20;

    private static readonly Vector3 centerOfMass = new Vector3(0, -0.2f, -0.5f);
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

    private Rigidbody rBody;

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
            wheel.ConfigureVehicleSubsteps(1, Drive.vehicalSubsteps, Drive.vehicalSubsteps);
        }
        
        this.rBody = this.GetComponent<Rigidbody>();
        this.rBody.centerOfMass = Drive.centerOfMass;
    }

    private void FixedUpdate()
    {
        float brakeTorque = this.Speed == 0 ? Mathf.Pow(this.rBody.velocity.magnitude, 2) * Drive.brakeTorqueScale : 0;
        foreach (WheelCollider wheel in this.WheelColliders)
        {
            wheel.brakeTorque = brakeTorque;
        }

        float torque = this.Speed * this.MaxSpeed * Drive.torqueScale;
        this.WheelColliders[WheelPosition.BackLeft.GetHashCode()].motorTorque = torque;
        this.WheelColliders[WheelPosition.BackRight.GetHashCode()].motorTorque = torque;

        float driveAngle = this.Angle * Drive.maxDriveAngle;
        this.WheelColliders[WheelPosition.FrontLeft.GetHashCode()].steerAngle = driveAngle;
        this.WheelColliders[WheelPosition.FrontRight.GetHashCode()].steerAngle = driveAngle;
    }
}
