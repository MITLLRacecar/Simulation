using UnityEngine;

/// <summary>
/// Encapsulates information about an autograder level.
/// </summary>
public class AutograderLevelInfo
{
    /// <summary>
    /// A short word or phrase describing the objective of the level.
    /// </summary>
    public string Title;

    /// <summary>
    /// A longer description of the objective of the level.
    /// </summary>
    public string Description;

    /// <summary>
    /// The maximum number of points which the user can receive on this level.
    /// </summary>
    public float MaxPoints;

    /// <summary>
    /// The total time in seconds which the user can spend on this level.
    /// </summary>
    public float TimeLimit = 10;

    /// <summary>
    /// If true, the entire autograder run finishes if this level is not passed with full points.
    /// </summary>
    public bool IsRequired = false;

    /// <summary>
    /// The index of the player camera to initially render.
    /// </summary>
    public int DefaultCameraIndex = 0;

    /// <summary>
    /// True if we should not continue to the next level until the car stops.
    /// </summary>
    /// <remarks>If the car does not stop before the time limit, no points are deducted.</remarks>
    public bool DoNotProceedUntilStopped = false;

    /// <summary>
    /// An array of times bonuses for the level, where each x value is a time in seconds and each y value is the point bonus for finishing under that time.
    /// </summary>
    public Vector2[] TimeBonuses;
}
