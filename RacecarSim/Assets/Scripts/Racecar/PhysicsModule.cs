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
    private const float linearErrorFactor = 0.001f;

    /// <summary>
    /// The average fixed error applied to all linear acceleration measurements.
    /// This value is made up (it is NOT specified in the Intel RealSense D435i datasheet).
    /// </summary>
    private const float linearErrorFixed = 0.05f;

    /// <summary>
    /// The average relative error of angular velocity measurements.
    /// This value is made up (it is NOT specified in the Intel RealSense D435i datasheet).
    /// </summary>
    private const float angularErrorFactor = 0.001f;

    /// <summary>
    /// The average fixed error applied to all angular velocity measurements.
    /// This value is made up (it is NOT specified in the Intel RealSense D435i datasheet).
    /// </summary>
    private const float angularErrorFixed = 0.005f;
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
                this.linearVelocity = this.transform.InverseTransformDirection(this.rBody.velocity) / 10;
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
                // Unity uses a left-handed coordinate system, but our API is right-handed
                Vector3 angVel = -this.rBody.angularVelocity;

                if (Settings.IsRealism)
                {
                    angVel *= NormalDist.Random(1, PhysicsModule.angularErrorFactor);
                    angVel.x += NormalDist.Random(0, PhysicsModule.angularErrorFixed);
                    angVel.y += NormalDist.Random(0, PhysicsModule.angularErrorFixed);
                    angVel.z += NormalDist.Random(0, PhysicsModule.angularErrorFixed);
                }

                this.angularVelocity = angVel;
            }
            return this.angularVelocity.Value;
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
        this.rBody = this.GetComponent<Rigidbody>();
    }

    private void Start()
    {
        this.prevVelocity = this.LinearVelocity;
    }

    private void FixedUpdate()
    {
        // Calculate current linear acceleration, incorporating gravity and error rate
        Vector3 curAcceleration = (this.LinearVelocity - this.prevVelocity) / Time.deltaTime;
        curAcceleration += this.transform.InverseTransformDirection(Vector3.down * 9.81f);
        if (Settings.IsRealism)
        {
            curAcceleration *= NormalDist.Random(1, PhysicsModule.linearErrorFactor);
            curAcceleration.x += NormalDist.Random(0, PhysicsModule.linearErrorFixed);
            curAcceleration.y += NormalDist.Random(0, PhysicsModule.linearErrorFixed);
            curAcceleration.z += NormalDist.Random(0, PhysicsModule.linearErrorFixed);
        }

        // Update linear acceleration running average
        this.LinearAccceleration += (curAcceleration - this.LinearAccceleration) / PhysicsModule.accelerationSamples;
        this.prevVelocity = this.LinearVelocity;
    }

    private void LateUpdate()
    {
        this.linearVelocity = null;
        this.angularVelocity = null;
    }
}
