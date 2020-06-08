using System;
using UnityEngine;

/// <summary>
/// Controls car physics and movement.
/// </summary>
public class Drive : MonoBehaviour
{
    #region Set in Unity Editor
    /// <summary>
    /// The models which visualize the car's wheels.
    /// </summary>
    public GameObject[] Wheels = new GameObject[4];

    /// <summary>
    /// The invisible colliders responsible for the physics of the car's wheels.
    /// </summary>
    public WheelCollider[] WheelColliders = new WheelCollider[4];
    #endregion

    #region Constants
    /// <summary>
    /// The scale factor applied to the Speed input provided by the user.
    /// </summary>
    private const float torqueScale = 10000.0f;

    /// <summary>
    /// The natural braking torque applied to all wheels when no input torque is given.
    /// </summary>
    private const float brakeTorqueScale = 1.0f;

    /// <summary>
    /// The maximum angle that the front wheels can turn.
    /// </summary>
    private const float maxDriveAngle = 20;

    /// <summary>
    /// The number of substeps used in wheel physics calculations.
    /// </summary>
    private const int vehicalSubsteps = 20;

    /// <summary>
    /// The center of mass of the vehicle relative to its physical center.
    /// </summary>
    private static readonly Vector3 centerOfMass = new Vector3(0, -0.2f, -0.5f);

    /// <summary>
    /// The distance the wheel model is offset from the wheel collider along the axle axis.
    /// </summary>
    private const float wheelOffset = 0.2f;
    #endregion

    #region Public Interface
    /// <summary>
    /// The input torque applied to the rear wheels, ranging from -1 (full reverse) to 1 (full forward).
    /// </summary>
    public float Speed { get; set; } = 0;

    /// <summary>
    /// The current angle of the car's front wheels, ranging from -1 (full left) to 1 (full right).
    /// </summary>
    public float Angle { get; set; } = 0;

    /// <summary>
    /// The max speed set by the user, ranging from 0 to 1 (Default = 0.25).
    /// </summary>
    public float MaxSpeed { get; set; } = 0.25f;

    /// <summary>
    /// Stops the car (equivalent to setting Speed and Angle to 0).
    /// </summary>
    public void Stop()
    {
        this.Speed = 0;
        this.Angle = 0;
    }
    #endregion

    /// <summary>
    /// The rigidbody of the car.
    /// </summary>
    private Rigidbody rBody;

    /// <summary>
    /// The four wheel positions in the order they appear in the car prefab.
    /// </summary>
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
        // Apply resting brake torque if Speed input is 0
        float brakeTorque = this.Speed == 0 ? Mathf.Pow(this.rBody.velocity.magnitude, 2) * Drive.brakeTorqueScale : 0;
        foreach (WheelCollider wheel in this.WheelColliders)
        {
            wheel.brakeTorque = brakeTorque;
        }

        // Set torque and angle of wheel colliders
        float torque = this.Speed * this.MaxSpeed * Drive.torqueScale;
        this.WheelColliders[WheelPosition.BackLeft.GetHashCode()].motorTorque = torque;
        this.WheelColliders[WheelPosition.BackRight.GetHashCode()].motorTorque = torque;

        float driveAngle = this.Angle * Drive.maxDriveAngle;
        this.WheelColliders[WheelPosition.FrontLeft.GetHashCode()].steerAngle = driveAngle;
        this.WheelColliders[WheelPosition.FrontRight.GetHashCode()].steerAngle = driveAngle;

        // Update position and rotation of wheel models to match wheel colliders
        foreach(WheelPosition wheelPosition in Enum.GetValues(typeof(WheelPosition)))
        {
            this.WheelColliders[wheelPosition.GetHashCode()].GetWorldPose(out Vector3 position, out Quaternion rotation);
            this.Wheels[wheelPosition.GetHashCode()].transform.rotation = rotation;
            this.Wheels[wheelPosition.GetHashCode()].transform.position = wheelPosition.GetHashCode() % 2 == 0 
                ? position + this.Wheels[wheelPosition.GetHashCode()].transform.right * Drive.wheelOffset
                : position - this.Wheels[wheelPosition.GetHashCode()].transform.right * Drive.wheelOffset;
        }
    }
}
