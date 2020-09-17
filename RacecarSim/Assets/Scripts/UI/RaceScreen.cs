using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the screen shown to the user during races with multiple cars.
/// </summary>
public class RaceScreen : ScreenManager
{
    #region Constants
    private static readonly Color disabledScreenColor = new Color(0.5f, 0.5f, 0.5f);

    private const string awaitingConnectionMessage = "Awaiting connection from a Python program...";
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
            RaceCameraView view = GetCameraView(i);
            view.Image.color = Color.white;
            view.Text.text = string.Empty;
        }
    }

    public override void UpdateMode(SimulationMode mode)
    {
        this.images[(int)Images.WaitMessage].gameObject.SetActive(mode == SimulationMode.Wait);
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

        foreach (RaceCameraView cameraView in this.cameraViews)
        {
            cameraView.gameObject.SetActive(false);
        }

        for (int i = 0; i < carCameras.Length; i++)
        {
            RaceCameraView carCameraView = GetCameraView(i);
            carCameraView.Image.texture = carCameras[i];
            carCameraView.Image.color = RaceScreen.disabledScreenColor;
            carCameraView.Text.text = RaceScreen.awaitingConnectionMessage;
            carCameraView.gameObject.SetActive(true);
        }

        // We only have space for the race camera if there are 3 or less cars
        if (this.numCars <= 3)
        {
            RaceCameraView raceCameraView = GetCameraView(this.numCars);
            raceCameraView.Image.texture = raceCamera;
            raceCameraView.Text.text = string.Empty;
            raceCameraView.gameObject.SetActive(false);
        }
    }
    #endregion

    private enum Images
    {
        WaitMessage = 9
    }

    private RaceCameraView[] cameraViews;

    private int numCars;

    protected override void Awake()
    {
        base.Awake();

        this.messageTextIndex = 5;
        this.mainTimeTextIndex = 6;

        this.cameraViews = this.GetComponentsInChildren<RaceCameraView>();

        // Unity requires one camera rendering to the display, so create a dummy camera
        // (it will be fully blocked by the race screen background)
        this.gameObject.AddComponent<Camera>();
    }

    private RaceCameraView GetCameraView(int index)
    {
        if (this.numCars == 2 && index == 2)
        {
            index = 4;
        }
        return this.cameraViews[index];
    }
}
