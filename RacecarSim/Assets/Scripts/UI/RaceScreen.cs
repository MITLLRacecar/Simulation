/// <summary>
/// Manages the screen shown to the user during races with multiple cars.
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

    public override void UpdateConnectedPrograms(int numConnectedPrograms)
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateMode(SimulationMode mode)
    {
        // Intentionally blank for now
    }

    public override void UpdateTime(float mainTime)
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateCheckpointTimes(float[,] checkpointTimes)
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateTimeScale(float timeScale)
    {
        throw new System.NotImplementedException();
    }
}
