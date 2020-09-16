using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the screen shown to the user during races with multiple cars.
/// </summary>
public class RaceScreen : ScreenManager
{
    #region Constants
    private static readonly Color disabledScreenColor = new Color(0.5f, 0.5f, 0.5f);
    #endregion 

    #region Public Interface
    #region Overrides
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
        for (int i = 0; i < numConnectedPrograms; i++)
        {
            GetScreen(i).color = Color.white;
            GetScreenText(i).gameObject.SetActive(false);
        }
    }

    public override void UpdateMode(SimulationMode mode)
    {
        // Intentionally blank for now
    }

    public override void UpdateTime(float mainTime)
    {
        // TODO
    }

    public override void UpdateCheckpointTimes(float[,] checkpointTimes)
    {
        // Intentionally blank for now
    }

    public override void UpdateTimeScale(float timeScale)
    {
        // Intentionally blank for now
    }
    #endregion

    public void SetCameras(Texture[] carCameras, Texture raceCamera)
    {
        this.numCars = carCameras.Length;

        for (int i = 0; i < carCameras.Length; i++)
        {
            RawImage screen = GetScreen(i);
            screen.gameObject.SetActive(true);
            screen.texture = carCameras[i];
            screen.color = RaceScreen.disabledScreenColor;

            GetScreenText(i).gameObject.SetActive(true);
        }

        if (this.numCars >= 3)
        {
            GetScreen(this.numCars).texture = raceCamera;
        }
    }
    #endregion

    protected override void Awake()
    {
        base.Awake();

        foreach (RawImage screen in this.images)
        {
            screen.gameObject.SetActive(false);
        }
    }

    private int numCars;

    private RawImage GetScreen(int index)
    {
        if (this.numCars == 2 && index == 2)
        {
            index = 4;
        }
        return this.images[index + 1]; // +1 to account for the background image
    }

    private Text GetScreenText(int index)
    {
        if (this.numCars == 2 && index == 2)
        {
            index = 4;
        }
        return this.texts[index];
    }
}
