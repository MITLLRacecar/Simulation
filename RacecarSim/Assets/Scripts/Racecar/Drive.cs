using System;
using UnityEngine;
using NWH.WheelController3D;

/// <summary>
/// Controls car physics and movement.
/// </summary>
public class Drive : RacecarModule
{
    private RacecarNWH carController;

    #region Constants
    /// <summary>
    /// The default value of MaxSpeed.
    /// </summary>
    public const float DefaultMaxSpeed = 1f;
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
    /// The max speed set by the user, ranging from 0 to 1.
    /// </summary>
    public float MaxSpeed { get; set; } = Drive.DefaultMaxSpeed;

    /// <summary>
    /// Stops the car (equivalent to setting Speed and Angle to 0).
    /// </summary>
    public void Stop()
    {
        this.Speed = 0;
        this.Angle = 0;
    }
    #endregion

    protected override void Awake()
    {
        this.carController = this.GetComponent<RacecarNWH>();

        base.Awake();
    }

    private void FixedUpdate()
    {
        // For NWH WheelController
        this.carController.driveAxis = this.Speed * this.MaxSpeed;
        this.carController.steerAxis = this.Angle;
    }
}