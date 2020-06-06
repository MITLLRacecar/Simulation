using UnityEngine;

/// <summary>
/// Simulates the IMU.
/// </summary>
public class PhysicsModule : MonoBehaviour
{
    #region Constants
    /// <summary>
    /// The number of past samples to average for linear acceleration.
    /// </summary>
    private const int accelerationSamples = 4;
    #endregion

    #region Public Interface
    /// <summary>
    /// The linear acceleration of the car relative to the car's transform (in meters/second^2).
    /// </summary>
    public Vector3 LinearAccceleration { get; private set; } = Vector3.zero;

    /// <summary>
    /// The angular velocity of the car (in radians/second).
    /// </summary>
    public Vector3 AngularVelocity
    {
        get
        {
            return this.rBody.angularVelocity;
        }
    }

    /// <summary>
    /// The linear velocity of the car relative to the car's transform (in meters/second)
    /// </summary>
    public Vector3 LinearVelocity
    {
        get
        {
            return this.transform.InverseTransformDirection(this.rBody.velocity) / 10;
        }
    }
    #endregion

    /// <summary>
    /// The rigidbody of the car.
    /// </summary>
    private Rigidbody rBody;

    /// <summary>
    /// The previous linear velocity of the car using the car's transform as basis vectors.
    /// </summary>
    private Vector3 prevVelocity;

    private void Start()
    {
        this.rBody = this.GetComponent<Rigidbody>();
        this.prevVelocity = this.LinearVelocity;
    }

    private void FixedUpdate()
    {
        Vector3 curAcceleration = (this.LinearVelocity - this.prevVelocity) / Time.deltaTime;
        this.LinearAccceleration += (curAcceleration - this.LinearAccceleration) / PhysicsModule.accelerationSamples;

        prevVelocity = this.LinearVelocity;
    }
}
