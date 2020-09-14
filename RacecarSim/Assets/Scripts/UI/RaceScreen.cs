/// <summary>
/// 
/// </summary>
public class RaceScreen : ScreenManager
{
    public override void HandleFailure(int carIndex, string reason)
    {
        throw new System.NotImplementedException();
    }

    public override void HandleWin(float[] times)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Updates the element showing the current simulation mode.
    /// </summary>
    /// <param name="mode">The current mode of the simulation.</param>
    public override void UpdateMode(SimulationMode mode)
    {
        // Intentionally blank for now
    }

    /// <summary>
    /// Updates the element showing the current time elapsed in the race.
    /// </summary>
    /// <param name="mainTime">The overall time (in seconds) that the current level has been running.</param>
    public override void UpdateTime(float mainTime)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Update the element(s) showing the time it took each car to reach each checkpoint.
    /// </summary>
    /// <param name="checkpointTimes">The time at which each car reached each checkpoint, indexed by car, then checkpoint.</param>
    public override void UpdateCheckpointTimes(float[,] checkpointTimes)
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateTimeScale(float timeScale)
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateConnectedPrograms(int numConnectedPrograms)
    {
        throw new System.NotImplementedException();
    }
}
