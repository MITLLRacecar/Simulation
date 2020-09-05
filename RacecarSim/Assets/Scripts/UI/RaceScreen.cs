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

    public override void UpdateTimes(float mainTime, float[] checkpointTimes = null)
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateTimeScale(float timeScale)
    {
        throw new System.NotImplementedException();
    }
}
