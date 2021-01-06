using UnityEngine;

/// <summary>
/// An autograder task in which the user must park at a specific point.
/// </summary>
public class DestinationStop : AutograderTask
{
    #region Set in Unity Editor
    /// <summary>
    /// The maximum speed which the car can travel in m/s and still be considered "stopped".
    /// </summary>
    [SerializeField]
    private float maxStopSpeed = Constants.MaxStopSeed;
    #endregion

    #region Constants
    /// <summary>
    /// The time in seconds which the car must stop at the point to complete the task.
    /// </summary>
    private const float stopDuration = 1.0f;

    /// <summary>
    /// The minimum alpha (opaqueness) of the mesh showing the region in which the user can park.
    /// </summary>
    private const float minAlpha = 0.25f;
    #endregion

    /// <summary>
    /// The Time.time at which the user was first stopped at the point.
    /// </summary>
    private float startTime = float.MaxValue;

    /// <summary>
    /// The material of the mesh showing the region in which the user can park.
    /// </summary>
    private Material material;

    /// <summary>
    /// The current alpha (opaqueness) of the mesh, which increases the longer the user has been parked.
    /// </summary>
    private float Alapha
    {
        get
        {
            float timeStopped = Mathf.Max(Time.time - this.startTime, 0);
            return Mathf.Min(DestinationStop.minAlpha + (1.0f - DestinationStop.minAlpha) * (timeStopped / DestinationStop.stopDuration), 1.0f);
        }
    }

    protected override void Awake()
    {
        base.Awake();
        this.material = this.GetComponentInChildren<Renderer>().material;
    }

    private void Start()
    {
        this.material.color = new Color(this.material.color.r, this.material.color.g, this.material.color.b, this.Alapha);
    }

    private void OnTriggerStay(Collider other)
    {
        Racecar racecar = other.GetComponentInParent<Racecar>();
        if (racecar != null)
        {
            if (racecar.Physics.LinearVelocity.magnitude < this.maxStopSpeed)
            {
                this.startTime = Mathf.Min(this.startTime, Time.time);
                if (Time.time - this.startTime >= DestinationStop.stopDuration)
                {
                    AutograderManager.CompleteTask(this);
                }
            }
            else
            {
                this.startTime = float.MaxValue;
            }

            this.material.color = new Color(this.material.color.r, this.material.color.g, this.material.color.b, this.Alapha);
        }
    }
}