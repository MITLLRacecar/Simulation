using UnityEngine;

/// <summary>
/// A platform which moves up and down.
/// </summary>
public class Elevator : MonoBehaviour
{
    #region Set in Unity Editor
    /// <summary>
    /// The distance to move upward.
    /// </summary>
    [SerializeField]
    private float height = 10;

    /// <summary>
    /// The maximum speed to move.
    /// </summary>
    [SerializeField]
    private float speed = 5;

    /// <summary>
    /// The time in seconds to stop at the top and bottom.
    /// </summary>
    [SerializeField]
    private float stopTime = 3;

    /// <summary>
    /// The fraction of the height which is spent accelerating or decelerating.
    /// </summary>
    [SerializeField]
    private float accelerationHeightFraction = Elevator.defaultAccelerationHeightFraction;

    /// <summary>
    /// The platform game object which will be raised and lowered.
    /// </summary>
    [SerializeField]
    private GameObject platform;

    /// <summary>
    /// Optional: an object to change colors to indicate when the elevator is ready at the bottom.
    /// </summary>
    [SerializeField]
    private Renderer lightRenderer;
    #endregion

    #region Constants
    /// <summary>
    /// The color of the light when the platform is stopped at the bottom.
    /// </summary>
    static readonly Color goColor = new Color(0, 0.5f, 1); // Blue

    /// <summary>
    /// The color of the light when the platform is not at the bottom.
    /// </summary>
    static readonly Color stopColor = Color.red;

    /// <summary>
    /// The color of the light when the platform is about to leave the bottom.
    /// </summary>
    static readonly Color slowColor = new Color(1, 0.5f, 0); // Orange

    /// <summary>
    /// The default value of accelerationHeightFraction
    /// </summary>
    const float defaultAccelerationHeightFraction = 0.25f;

    /// <summary>
    /// The fraction of stopTime for which the slow color is shown.
    /// </summary>
    const float slowColorTimeFraction = 0.25f;
    #endregion

    /// <summary>
    /// The possible states of the elevator.
    /// </summary>
    private enum State
    {
        StopBottom,
        Raise,
        StopTop,
        Lower,
    }

    /// <summary>
    /// The current state of the elevator.
    /// </summary>
    private State state = State.StopBottom;

    /// <summary>
    /// The world height at which the platform starts.
    /// </summary>
    private float startHeight;

    /// <summary>
    /// The number of seconds for which the platform will remain stopped.
    /// </summary>
    private float counter;

    /// <summary>
    /// The rigidbody of the platform.
    /// </summary>
    private Rigidbody rbody;

    /// <summary>
    /// The current height of the platform relative to the starting height.
    /// </summary>
    private float CurHeight
    {
        get
        {
            return this.platform.transform.position.y - this.startHeight;
        }
    }

    /// <summary>
    /// The number of seconds to show the slow color.
    /// </summary>
    private float SlowTime
    {
        get
        {
            return this.stopTime * Elevator.slowColorTimeFraction;
        }
    }

    private void Start()
    {
        this.startHeight = this.transform.position.y;
        this.counter = stopTime;
        this.rbody = platform.GetComponent<Rigidbody>();

        if (this.accelerationHeightFraction < 0 || this.accelerationHeightFraction > 0.5f)
        {
            Debug.LogError("accelerationHeightFraction must in the range [0, 0.5]");
            this.accelerationHeightFraction = Elevator.defaultAccelerationHeightFraction;
        }
    }

    private void FixedUpdate()
    {
        switch (this.state)
        {
            case State.StopBottom:
                this.rbody.velocity = Vector3.zero;
                float prevCounter = this.counter;
                this.counter -= Time.fixedDeltaTime;
                if (prevCounter > this.SlowTime && this.counter < this.SlowTime)
                {
                    this.SetLightColor(Elevator.slowColor);
                }
                else if (this.counter <= 0)
                {
                    this.SetLightColor(Elevator.stopColor);
                    this.state++;
                }
                break;

            case State.Raise:
                this.rbody.velocity = Vector3.up * this.CalculateSpeed();
                if (this.CurHeight >= this.height)
                {
                    this.counter = this.stopTime;
                    this.state = State.StopTop;
                }
                break;

            case State.StopTop:
                this.rbody.velocity = Vector3.zero;
                this.counter -= Time.fixedDeltaTime;
                if (this.counter <= 0)
                {
                    this.state++;
                }
                break;

            case State.Lower:
                this.rbody.velocity = Vector3.down * this.CalculateSpeed();
                if (this.CurHeight <= 0)
                {
                    this.counter = this.stopTime;
                    this.SetLightColor(Elevator.goColor);
                    this.state = State.StopBottom;
                }
                break;
        }
    }

    /// <summary>
    /// Calculates the current speed of the platform based on the current height and other parameters.
    /// </summary>
    /// <returns>The current vertical speed of the platform.</returns>
    private float CalculateSpeed()
    {
        float closerDist = Mathf.Min(this.CurHeight, this.height - this.CurHeight);
        return Mathf.Lerp(this.speed / 4, this.speed, closerDist / this.accelerationHeightFraction / height);
    }

    /// <summary>
    /// Attempts to set the color of the light if one was provided.
    /// </summary>
    /// <param name="color">The color to set the light.</param>
    private void SetLightColor(Color color)
    {
        if (this.lightRenderer != null)
        {
            this.lightRenderer.material.color = color;
        }
    }
}
