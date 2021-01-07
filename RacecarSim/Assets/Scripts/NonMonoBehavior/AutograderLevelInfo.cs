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
}
