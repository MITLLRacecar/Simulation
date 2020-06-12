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

    /// <summary>
    /// The average relative error of linear acceleration measurements.
    /// This value is made up (it is NOT specified in the Intel RealSense D435i datasheet).
    /// </summary>
    private const float averageLinearErrorFactor = 0.001f;

    /// <summary>
    /// The average relative error of angular velocity measurements.
    /// This value is made up (it is NOT specified in the Intel RealSense D435i datasheet).
    /// </summary>
    private const float averageAngularErrorFactor = 0.02f;
    #endregion

    #region Public Interface
    /// <summary>
    /// The linear acceleration of the car relative to the car's transform (in meters/second^2).
    /// </summary>
    public Vector3 LinearAccceleration { get; private set; } = Vector3.zero;

    /// <summary>
    /// The linear velocity of the car relative to the car's transform (in meters/second)
    /// </summary>
    public Vector3 LinearVelocity
    {
        get
        {
            if (!this.linearVelocity.HasValue)
            {
                this.linearVelocity = this.racecar.Settings.isRealism
                    ? this.transform.InverseTransformDirection(this.rBody.velocity) * NormalDist.Random(1, PhysicsModule.averageLinearErrorFactor) / 10
                    : this.transform.InverseTransformDirection(this.rBody.velocity) / 10;
            }
            return this.linearVelocity.Value;
        }
    }

    /// <summary>
    /// The angular velocity of the car (in radians/second).
    /// </summary>
    public Vector3 AngularVelocity
    {
        get
        {
            if (!this.angularVelocity.HasValue)
            {
                // Unity uses a left-handed coordinate system, but our IMU is right-handed
                this.angularVelocity = this.racecar.Settings.isRealism
                    ? -this.rBody.angularVelocity * NormalDist.Random(1, PhysicsModule.averageAngularErrorFactor)
                    : -this.rBody.angularVelocity;
            }
            return this.angularVelocity.Value;
        }
    }
    #endregion

    /// <summary>
    /// The parent racecar to which this module belongs.
    /// </summary>
    private Racecar racecar;

    /// <summary>
    /// The rigidbody of the car.
    /// </summary>
    private Rigidbody rBody;

    /// <summary>
    /// The previous linear velocity of the car using the car's transform as basis vectors.
    /// </summary>
    private Vector3 prevVelocity;

    /// <summary>
    /// Private member for the LinearVelocity accessor
    /// </summary>
    private Vector3? linearVelocity = null;

    /// <summary>
    /// Private member for the AngularVelocity accessor
    /// </summary>
    private Vector3? angularVelocity = null;

    private void Awake()
    {
        this.racecar = this.GetComponent<Racecar>();
        this.rBody = this.GetComponent<Rigidbody>();
    }

    private void Start()
    {
        this.prevVelocity = this.LinearVelocity;
    }

    private void FixedUpdate()
    {
        Vector3 curAcceleration = (this.LinearVelocity - this.prevVelocity) / Time.deltaTime;
        this.LinearAccceleration += (curAcceleration - this.LinearAccceleration) / PhysicsModule.accelerationSamples;

        prevVelocity = this.LinearVelocity;
    }

    private void LateUpdate()
    {
        this.linearVelocity = null;
        this.angularVelocity = null;
    }
}
