using System;

/// <summary>
/// Encapsulates the best time information about a single level.
/// </summary>
[Serializable]
public class BestTimeInfo
{
    /// <summary>
    /// The best time for completing the entire level.
    /// </summary>
    public float OverallTime;

    /// <summary>
    /// The best time for each checkpoint in the level, or null if the level contains no checkpoints.
    /// </summary>
    /// <remarks>
    /// Checkpoint time is calculated as the time spent working towards that checkpoint only. For example, if the user completes
    /// checkpoint 1 at 2.35 seconds and checkpoint 2 at 5.60 seconds, then the time for checkpoint 2 is 5.60 - 2.35 = 3.25 seconds.
    /// </remarks>
    public float[] CheckpointTimes { get; private set; }

    /// <summary>
    /// Creates new best time information for a level indicating no completions yet.
    /// </summary>
    /// <param name="numCheckpoints">The number of checkpoints in the level.</param>
    public BestTimeInfo(int numCheckpoints)
    {
        this.OverallTime = float.MaxValue;
        this.CheckpointTimes = new float[numCheckpoints + 1];
        for (int i = 0; i < this.CheckpointTimes.Length; i++)
        {
            this.CheckpointTimes[i] = float.MaxValue;
        }
    }
}
