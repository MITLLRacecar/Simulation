using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// The possible modes of the simulation.
/// </summary>
public enum SimulationMode
{
    DefaultDrive,
    UserProgram,
    Wait,
    Finished
}

/// <summary>
/// The top level controller managing a simulation level. 
/// </summary>
public class LevelManager : MonoBehaviour
{
    #region Set in Unity Editor
    /// <summary>
    /// The prefab which is copied to spawn players.
    /// </summary>
    [SerializeField]
    private GameObject playerPrefab;

    /// <summary>
    /// The prefab which is copied to create the Heads Up Display (HUD).
    /// </summary>
    [SerializeField]
    private GameObject hudPrefab;

    /// <summary>
    /// The prefab which is copied to create the screen manager for a race with multiple cars.
    /// </summary>
    [SerializeField]
    private GameObject raceScreenPrefab;

    /// <summary>
    /// The default start key point, with a position and rotation of zero.
    /// </summary>
    [SerializeField]
    private GameObject defaultStart;

    // The Unity Editor does not support jagged arrays,
    // so we resort to hard coding settable positions for up to four cars.

    /// <summary>
    /// The offsets from which two cars will be spawned from the start point.
    /// </summary>
    [SerializeField]
    private Vector3[] twoCarSpawnOffsets = { new Vector3(-4, 0, 0), new Vector3(4, 0, 0) };

    /// <summary>
    /// The offsets from which three cars will be spawned from the start point.
    /// </summary>
    [SerializeField]
    private Vector3[] threeCarSpawnOffsets = { new Vector3(-5, 0, 0), new Vector3(0, 0, 0), new Vector3(5, 0, 0) };

    /// <summary>
    /// The offsets from which two cars will be spawned from the start point.
    /// </summary>
    [SerializeField]
    private Vector3[] fourCarSpawnOffsets = { new Vector3(-4, 0, 0), new Vector3(4, 0, 0), new Vector3(-4, 0, -8), new Vector3(4, 0, -8) };
    #endregion

    #region Constants
    /// <summary>
    /// The width and height of a race camera view texture.
    /// </summary>
    private static readonly int[] raceScreenTextureDimensions = { 960, 600 };

    /// <summary>
    /// The minimum time scale at which the simulation can run.
    /// </summary>
    private const float minTimeScale = 1.0f / 64.0f;
    #endregion

    #region Public Interface
    /// <summary>
    /// The number of players (racecars) in the level.
    /// </summary>
    public static int NumPlayers = 1;

    /// <summary>
    /// True if the current simulation is an evaluation run.
    /// </summary>
    public static bool IsEvaluation = false;

    /// <summary>
    /// The info of the current level.
    /// </summary>
    public static LevelInfo LevelInfo = LevelInfo.DefaultLevel;

    /// <summary>
    /// Displays a simulation error to the screen manager and logs it to standard error.
    /// </summary>
    /// <param name="errorText">A message describing the error.</param>
    public static void HandleError(string errorText)
    {
        LevelManager.instance.mode = LevelManager.IsEvaluation ? SimulationMode.Wait : SimulationMode.DefaultDrive;
        LevelManager.instance.screenManager.UpdateMode(LevelManager.instance.mode);
        
        errorText = $">> Error: {errorText} Returning to {LevelManager.instance.mode} mode.";
        Debug.LogError(errorText);
        LevelManager.instance.screenManager.ShowError(errorText);
    }

    /// <summary>
    /// Show a text message to the user on the screen.
    /// </summary>
    /// <param name="message">The text to show.</param>
    /// <param name="color">The color of the text.</param>
    /// <param name="persistTime">The time in seconds the text is shown.</param>
    public static void ShowMessage(string text, Color color, float persistTime)
    {
        LevelManager.instance.screenManager.ShowMessage(text, color, persistTime);
    }

    /// <summary>
    /// Handles when a car passes a checkpoint.
    /// </summary>
    /// <param name="carIndex">The index of the car which passed the checkpoint.</param>
    /// <param name="checkpointIndex">The index of the checkpoint which was passed.</param>
    public static void HandleCheckpoint(int carIndex, int checkpointIndex)
    {
        // Only count the checkpoint if the car has not passed this checkpoint (or a later one) yet
        if (!LevelManager.instance.failed && LevelManager.instance.curKeyPoints[carIndex] <= checkpointIndex)
        {
            // Add 1 to account for the start, making this a key point index
            checkpointIndex++;
            LevelManager.instance.curKeyPoints[carIndex] = checkpointIndex;

            if (LevelManager.IsEvaluation && carIndex == 0)
            {
                LevelManager.instance.keyPointDurations[checkpointIndex] = LevelManager.instance.CurTime - LevelManager.instance.prevKeyPointTime;
                LevelManager.instance.prevKeyPointTime = LevelManager.instance.CurTime;
            }
        }
    }

    /// <summary>
    /// Handles when a car passes the finish line.
    /// </summary>
    /// <param name="carIndex">The index of the car which passed the finish line.</param>
    public static void HandleFinish(int carIndex)
    {
        int finishIndex = LevelManager.instance.keyPoints.Length - 1;

        // Only count if the car has not passed the finish yet
        if (!LevelManager.instance.failed && LevelManager.instance.curKeyPoints[carIndex] < finishIndex)
        {
            LevelManager.instance.curKeyPoints[carIndex] = finishIndex;

            if (LevelManager.IsEvaluation)
            {
                if (carIndex == 0)
                {
                    LevelManager.instance.keyPointDurations[finishIndex] = LevelManager.instance.CurTime - LevelManager.instance.prevKeyPointTime;
                    LevelManager.instance.totalDuration = LevelManager.instance.CurTime;
                }

                // Trigger a win when all cars have finished
                if (LevelManager.instance.curKeyPoints.All(x => x == finishIndex))
                {
                    bool isNewBestTime =
                        LevelManager.NumPlayers == 1 &&
                        SavedDataManager.Data.BestTimes[LevelManager.LevelInfo.WinableIndex].OverallTime > LevelManager.instance.CurTime;

                    LevelManager.instance.mode = SimulationMode.Finished;
                    LevelManager.instance.screenManager.HandleWin(LevelManager.instance.CurTime, isNewBestTime);
                    LevelManager.instance.screenManager.UpdateTime(LevelManager.instance.CurTime, LevelManager.instance.keyPointDurations);
                    LevelManager.instance.screenManager.UpdateMode(LevelManager.instance.mode);
                }
            }
        }
    }

    /// <summary>
    /// Handles when a car fails the objective.
    /// </summary>
    /// <param name="carIndex">The car which failed.</param>
    /// <param name="failureMessage">The message to display describing the reason the objective was failed.</param>
    public static void HandleFailure(int carIndex, string failureMessage)
    {
        // Do not count the failure if the car has already finished
        if (!(LevelManager.IsEvaluation && LevelManager.instance.curKeyPoints[carIndex] == LevelManager.instance.keyPoints.Length - 1))
        {
            LevelManager.instance.screenManager.HandleFailure(carIndex, failureMessage);

            if (LevelManager.NumPlayers > 1)
            {
                LevelManager.ResetCar(carIndex);
            }
            else if (LevelManager.IsEvaluation)
            {
                LevelManager.instance.failed = true;
            }
        }
    }

    /// <summary>
    /// Adds a time penalty for the entire race.
    /// </summary>
    /// <param name="penalty">The time penalty in seconds.</param>
    /// <remarks>If there are multiple cars in the race, this will penalize all cars.</remarks>
    public static void AddTimePenalty(float penalty)
    {
        if (LevelManager.IsEvaluation)
        {
            LevelManager.instance.timePenalty += penalty;
        }
        else
        {
            LevelManager.instance.screenManager.ShowWarning($"Time penalty: -{penalty:F1} seconds");
        }
    }

    /// <summary>
    /// Reset a car to its last checkpoint.
    /// </summary>
    /// <param name="carIndex">The index of the car to reset.</param>
    public static void ResetCar(int carIndex)
    {
        // Relocate the car to its current reset point
        Transform resetLocation = LevelManager.instance.GetResetLocation(carIndex);
        LevelManager.instance.players[carIndex].transform.position = resetLocation.position;
        LevelManager.instance.players[carIndex].transform.rotation = resetLocation.rotation;

        // Bring the car to a complete stop
        Rigidbody carRigidBody = LevelManager.instance.players[carIndex].GetComponent<Rigidbody>();
        carRigidBody.velocity = Vector3.zero;
        carRigidBody.angularVelocity = Vector3.zero;
    }

    /// <summary>
    /// Updates which cars are connected to a Python script.
    /// </summary>
    /// <param name="connectedPrograms">An array in which each entry indicates whether the car of the same index is connected to a Python script.</param>
    public static void UpdateConnectedPrograms(bool[] connectedPrograms)
    {
        LevelManager.instance.connectedPrograms = connectedPrograms;

        // We cannot update the screen manager immediately since this may not be the main thread,
        // so we instead wait until the next call to Update
        LevelManager.instance.wasConnectedProgramsChanged = true;
    }

    /// <summary>
    /// Returns a racecar in the current level.
    /// </summary>
    /// <param name="index">The index of the racecar to return.</param>
    /// <returns>The racecar with the specified index.</returns>
    public static Racecar GetCar(int index = 0)
    {
        return LevelManager.instance.players[index];
    }
    #endregion

    /// <summary>
    /// A static reference to the current LevelManager (there is only ever one at a time).
    /// </summary>
    private static LevelManager instance;

    /// <summary>
    /// Encapsulates the interaction with Python scripts.
    /// </summary>
    private PythonInterface pythonInterface;

    /// <summary>
    /// The object managing the 2D screen content.
    /// </summary>
    private ScreenManager screenManager;

    /// <summary>
    /// The racecars in the current simulation.
    /// </summary>
    private Racecar[] players;

    /// <summary>
    /// The current simulation mode.
    /// </summary>
    private SimulationMode mode;

    /// <summary>
    /// The key points in the current level.
    /// </summary>
    private KeyPoint[] keyPoints;

    /// <summary>
    /// The most recent key point which each car passed.
    /// </summary>
    private int[] curKeyPoints;

    /// <summary>
    /// True if the level has been failed.
    /// </summary>
    private bool failed;

    /// <summary>
    /// The time at which the race began.
    /// </summary>
    private float startTime;

    /// <summary>
    /// The total time penalty in seconds added to the level.
    /// </summary>
    private float timePenalty;

    /// <summary>
    /// The time in seconds which car 0 spends towards each key point after passing the previous key point, indexed by key point.
    /// </summary>
    private float[] keyPointDurations;

    /// <summary>
    /// The total time in seconds which it took car 0 to complete the race.
    /// </summary>
    private float totalDuration;

    /// <summary>
    /// The time in seconds since the start of the race at which car 0 passed the previous key point.
    /// </summary>
    private float prevKeyPointTime;

    /// <summary>
    /// An array indicating which cars are currently connected to a python script.
    /// </summary>
    private bool[] connectedPrograms = new bool[0];

    /// <summary>
    /// True when the number of Python programs connected to the sync client changed since the last call to Update.
    /// </summary>
    private bool wasConnectedProgramsChanged;

    /// <summary>
    /// The fixed camera(s) which capture the race.
    /// </summary>
    /// <remarks>These cameras are children of the LevelManager game object.</remarks>
    private Camera[] raceCameras;

    /// <summary>
    /// The index of the current active race camera.
    /// </summary>
    private int curRaceCamera;

    /// <summary>
    /// The rate at which time in the simulation progresses, independent of pausing.
    /// </summary>
    /// <remarks>This field is necessary to store the time scale during pausing, when Time.timeScale is set to 0.</remarks>
    private float timeScale;

    /// <summary>
    /// The time in seconds since the race began, accounting for pausing and time scale changes.
    /// </summary>
    private float CurTime
    {
        get
        {
            // Time.time already accounts for changes in the time scale
            return Time.time - this.startTime + this.timePenalty;
        }
    }

    /// <summary>
    /// The number of checkpoints in the current level.
    /// </summary>
    private int NumCheckpoints
    {
        get
        {
            return Math.Max(this.keyPoints.Length - 2, 0);
        }
    }

    private void Awake()
    {
        // If the level info has a negative build index, it was never set, meaning we did not start from the main menu
        // Thus, we should return directly to the main menu
        if (LevelManager.LevelInfo.BuildIndex < 0)
        {
            SceneManager.LoadScene(LevelCollection.MainMenuBuildIndex);
        }

        LevelManager.instance = this;
        this.mode = LevelManager.IsEvaluation ? SimulationMode.Wait : SimulationMode.DefaultDrive;

        this.raceCameras = this.GetComponentsInChildren<Camera>();
    }

    private void Start()
    {
        this.FindKeyPoints();
        this.SpawnPlayers();
        this.pythonInterface = new PythonInterface(this.players);

        if (LevelManager.IsEvaluation)
        {
            this.keyPointDurations = new float[this.keyPoints.Length];
            this.screenManager.UpdateTime(0.0f, this.keyPointDurations);
        }

        this.SetTimeScale(1.0f);

        if (LevelManager.LevelInfo.HelpMessage != null)
        {
            this.screenManager.ShowMessage(LevelManager.LevelInfo.HelpMessage, Color.white);
        }
    }

    private void Update()
    {
        switch (this.mode)
        {
            case SimulationMode.DefaultDrive:
                foreach (Racecar player in this.players)
                {
                    player.DefaultDriveUpdate();
                }
                break;

            case SimulationMode.UserProgram:
                this.pythonInterface.HandleUpdate();
                if (LevelManager.IsEvaluation && !LevelManager.instance.failed && this.curKeyPoints[0] < this.keyPoints.Length - 1)
                {
                    this.keyPointDurations[this.curKeyPoints[0] + 1] = this.CurTime - this.prevKeyPointTime;
                    this.screenManager.UpdateTime(this.CurTime, this.keyPointDurations);
                }
                break;
        }

        // Handle START and BACK buttons
        if (Controller.IsDown(Controller.Button.START) && Controller.IsDown(Controller.Button.BACK))
        {
            this.HandleRestart();
        }
        else if (Controller.WasPressed(Controller.Button.START))
        {
            this.HandleStart();
        }
        else if (Controller.WasPressed(Controller.Button.BACK))
        {
            this.HandleBack();
        }

        // Handle timeScale change on alt keys
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            this.ScaleTimeScale(0.5f);
        }
        else if (Input.GetKeyDown(KeyCode.RightAlt) || Input.GetKeyDown(KeyCode.AltGr))
        {
            this.ScaleTimeScale(2);
        }

        // Handle exit on escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            this.HandleExit();
        }

        if (this.wasConnectedProgramsChanged)
        {
            this.screenManager.UpdateConnectedPrograms(this.connectedPrograms);
            if (this.connectedPrograms.Length == 0)
            {
                // If the user forcibly exits all programs, this is equivalent to pressing back
                this.HandleBack();
            }
            this.wasConnectedProgramsChanged = false; // TODO: there is probably a data race here
        }

        // In non-evaluation mode, skip between checkpoints on tab key
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (LevelManager.IsEvaluation)
            {
                this.screenManager.ShowWarning("Checkpoint skipping (TAB key) is disabled in evaluation mode.");
            }
            else
            {
                this.curKeyPoints[0] = Math.Min(this.curKeyPoints[0] + 1, this.NumCheckpoints);
                LevelManager.ResetCar(0);
            }
        }

        // In non-evaluation mode, reset to current checkpoint on Caps Lock key
        if (Input.GetKeyDown(KeyCode.CapsLock))
        {
            if (LevelManager.IsEvaluation)
            {
                this.screenManager.ShowWarning("Checkpoint reset (CAPS LOCK key) is disabled in evaluation mode.");
            }
            else
            {
                LevelManager.ResetCar(0);
            }
        }

        // For multi-car races, manage switching between race cameras 
        if (LevelManager.NumPlayers > 1)
        {
            this.ManageRaceCameras();
        }
    }

    private void OnApplicationQuit()
    {
        this.UpdateBestTimes();
        this.pythonInterface?.HandleExit();
    }


    /// <summary>
    /// Gets the location at which a certain car should be spawned.
    /// </summary>
    /// <param name="carIndex">The index of the car to spawn.</param>
    /// <returns>The (position, rotation) at which the car should be spawned.</returns>
    private (Vector3, Quaternion) GetSpawnLocation(int carIndex)
    {
        // Calculate position offset if there are multiple cars.
        Vector3 offset = Vector3.zero;
        switch (LevelManager.NumPlayers)
        {
            case 2:
                offset = this.twoCarSpawnOffsets[carIndex];
                break;
            case 3:
                offset = this.threeCarSpawnOffsets[carIndex];
                break;
            case 4:
                offset = this.fourCarSpawnOffsets[carIndex];
                break;
        }

        Transform start = this.keyPoints[0].transform;
        return (start.position + offset, start.rotation);
    }

    /// <summary>
    /// Gets the location to which a certain car should be reset. 
    /// </summary>
    /// <param name="carIndex">The index of the car to reset.</param>
    /// <returns>The transform of the key point at which the car should be reset.</returns>
    private Transform GetResetLocation(int carIndex)
    {
        return this.keyPoints[this.curKeyPoints[carIndex]].transform;
    }

    /// <summary>
    /// Register changes between race cameras triggered by keyboard input.
    /// </summary>
    private void ManageRaceCameras()
    {
        int prevRaceCamera = this.curRaceCamera;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            this.curRaceCamera += this.raceCameras.Length - 1;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            this.curRaceCamera++;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            this.curRaceCamera = 0;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            this.curRaceCamera = this.raceCameras.Length - 1;
        }
        this.curRaceCamera %= this.raceCameras.Length;

        for (int i = 0; i < this.raceCameras.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                this.curRaceCamera = i;
                break;
            }
        }

        if (prevRaceCamera != this.curRaceCamera)
        {
            this.raceCameras[prevRaceCamera].enabled = false;
            this.raceCameras[this.curRaceCamera].enabled = true;
        }
    }

    /// <summary>
    /// Spawns the correct number of players and creates the corresponding screen manager.
    /// </summary>
    private void SpawnPlayers()
    {
        this.players = new Racecar[LevelManager.NumPlayers];

        // If there is only one player, spawn a single car and create a HUD as the screen manager
        if (LevelManager.NumPlayers == 1)
        {
            (Vector3 spawnPosition, Quaternion spawnRotation) = this.GetSpawnLocation(0);
            this.players[0] = GameObject.Instantiate(this.playerPrefab, spawnPosition, spawnRotation).GetComponentInChildren<Racecar>();
            this.players[0].SetIndex(0);
            
            Hud hud = GameObject.Instantiate(this.hudPrefab).GetComponent<Hud>();
            this.players[0].GetComponentInChildren<Racecar>().Hud = hud;
            this.screenManager = hud;
        }

        // If there are multiple players, spawn multiple cars and create a RaceScreen as the screen manager
        else
        {
            RenderTexture[] playerCameraTextures = new RenderTexture[LevelManager.NumPlayers];
            RenderTextureDescriptor textureDescriptor = new RenderTextureDescriptor(LevelManager.raceScreenTextureDimensions[0], LevelManager.raceScreenTextureDimensions[1]);
            for (int i = 0; i < LevelManager.NumPlayers; i++)
            {
                (Vector3 spawnPosition, Quaternion spawnRotation) = this.GetSpawnLocation(i);
                this.players[i] = GameObject.Instantiate(this.playerPrefab, spawnPosition, spawnRotation).GetComponentInChildren<Racecar>();
                this.players[i].SetIndex(i);
                playerCameraTextures[i] = new RenderTexture(textureDescriptor);
                this.players[i].SetPlayerCameraFeatures(playerCameraTextures[i], false);
            }

            // Create a render texture for the race cameras
            RenderTexture raceCameraTexture = new RenderTexture(textureDescriptor);
            foreach(Camera raceCamera in this.raceCameras)
            {
                raceCamera.targetTexture = raceCameraTexture;
            }

            // Start with only the first race camera enabled
            if (this.raceCameras.Length > 0)
            {
                this.raceCameras[0].enabled = true;
                this.curRaceCamera = 0;
            }
            for (int i = 1; i < this.raceCameras.Length; i++)
            {
                this.raceCameras[i].enabled = false;
            }

            RaceScreen raceScreen = GameObject.Instantiate(this.raceScreenPrefab).GetComponent<RaceScreen>();
            raceScreen.SetCameras(playerCameraTextures, raceCameraTexture);
            this.screenManager = raceScreen;
        }
        this.screenManager.UpdateMode(this.mode);
    }

    /// <summary>
    /// Handles when the user presses the START button.
    /// </summary>
    private void HandleStart()
    {
        this.HandleUnpause();
        if (this.mode != SimulationMode.UserProgram)
        {
            if (this.connectedPrograms.Length > 0)
            {
                this.mode = SimulationMode.UserProgram;
                this.pythonInterface.HandleStart();
                this.screenManager.UpdateMode(this.mode);

                if (this.startTime == 0)
                {
                    this.startTime = Time.time;
                }
            }
            else
            {
                this.screenManager.ShowWarning("You must connect a Python program before entering User Program mode.");
            }
        }
    }

    /// <summary>
    /// Handles when the user presses the BACK button.
    /// </summary>
    private void HandleBack()
    {
        if (LevelManager.IsEvaluation)
        {
            this.TogglePause();
        }
        else if (this.mode == SimulationMode.UserProgram)
        {
            this.mode = SimulationMode.DefaultDrive;
            foreach (Racecar player in this.players)
            {
                player.DefaultDriveStart();
            }
            this.screenManager.UpdateMode(this.mode);
        }
    }

    /// <summary>
    /// Handles when the user restarts the current level (START + BACK).
    /// </summary>
    private void HandleRestart()
    {
        // Reload current level with the ReloadBuffer
        this.pythonInterface.HandleExit();
        ReloadBuffer.BuildIndexToReload = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadSceneAsync(ReloadBuffer.BuildIndex, LoadSceneMode.Single);
    }

    /// <summary>
    /// Handles when the user exits to the main menu (escape).
    /// </summary>
    private void HandleExit()
    {
        this.SetTimeScale(1.0f);
        this.UpdateBestTimes();
        this.pythonInterface.HandleExit();
        SceneManager.LoadScene(LevelCollection.MainMenuBuildIndex);
    }

    /// <summary>
    /// Toggles whether the game is paused (BACK in evaluation mode).
    /// </summary>
    private void TogglePause()
    {
        if (Time.timeScale > 0)
        {
            Time.timeScale = 0;
            this.screenManager.SetPause(true);
        }
        else
        {
            Time.timeScale = this.timeScale;
            this.screenManager.SetPause(false);
        }
    }

    /// <summary>
    /// Unpauses the game if it is currently paused.
    /// </summary>
    private void HandleUnpause()
    {
        if (Time.timeScale == 0)
        {
            Time.timeScale = this.timeScale;
            this.screenManager.SetPause(false);
        }
    }

    /// <summary>
    /// Adjust the current time scale by a multiplicative factor.
    /// </summary>
    /// <param name="scaleFactor">The number with which to multiply the current time scale.</param>
    private void ScaleTimeScale(float scaleFactor)
    {
        this.SetTimeScale(Mathf.Max(Mathf.Min(Time.timeScale * scaleFactor, 1.0f), LevelManager.minTimeScale));
    }

    /// <summary>
    /// Set the current time scale, the rate at which time progresses.
    /// </summary>
    /// <param name="timeScale">The new value for the time scale.</param>
    private void SetTimeScale(float timeScale)
    {
        this.timeScale = timeScale;
        Time.timeScale = timeScale;
        Time.fixedDeltaTime = Settings.DefaultFixedDeltaTime * timeScale;

        if (timeScale > 0)
        {
            this.screenManager.UpdateTimeScale(timeScale);
        }
    }

    /// <summary>
    /// Initializes keyPoints and curKeyPoint by finding the key points in the level.
    /// </summary>
    private void FindKeyPoints()
    {
        this.keyPoints = FindObjectsOfType<KeyPoint>();
        Array.Sort(this.keyPoints);

        // If the level does not have a start key point, create and add a default one
        if (this.keyPoints.Length == 0)
        {
            this.keyPoints = new KeyPoint[] { GameObject.Instantiate(this.defaultStart, Vector3.zero, Quaternion.identity).GetComponent<KeyPoint>() };
        }
        else if (this.keyPoints[0].Type != KeyPoint.KeyPointType.Start)
        {
            KeyPoint start = GameObject.Instantiate(this.defaultStart, Vector3.zero, Quaternion.identity).GetComponent<KeyPoint>();
            KeyPoint[] existingKeyPoints = this.keyPoints;

            this.keyPoints = new KeyPoint[this.keyPoints.Length + 1];
            this.keyPoints[0] = start;
            Array.ConstrainedCopy(existingKeyPoints, 0, this.keyPoints, 1, existingKeyPoints.Length);
        }

        this.curKeyPoints = new int[LevelManager.NumPlayers];

        // Verify that the number of checkpoints listed in the level info is correct
        if (LevelManager.LevelInfo.NumCheckpoints != this.NumCheckpoints)
        {
            Debug.LogError($"Incorrect number of checkpoints found for level [{LevelManager.LevelInfo.DisplayName}]: " +
                $"Expected [{LevelManager.LevelInfo.NumCheckpoints}], but found [{this.NumCheckpoints}].");

            LevelManager.LevelInfo.NumCheckpoints = this.NumCheckpoints;
        }
    }

    /// <summary>
    /// Update the level's saved best times with the current times, if necessary.
    /// </summary>
    private void UpdateBestTimes()
    {
        if (LevelManager.IsEvaluation &&
            LevelManager.NumPlayers == 1 &&
            this.curKeyPoints[0] > 0 &&
            !Settings.CheatMode)
        {
            BestTimeInfo bestTimeInfo = SavedDataManager.Data.BestTimes[LevelManager.LevelInfo.WinableIndex];

            // Update overall time if we finished the level
            if (this.curKeyPoints[0] == this.keyPoints.Length - 1)
            {
                bestTimeInfo.OverallTime = Mathf.Min(bestTimeInfo.OverallTime, this.totalDuration);
            }

            // Update the times for the checkpoints we completed
            for (int i = 0; i < this.curKeyPoints[0]; i++)
            {
                bestTimeInfo.CheckpointTimes[i] = Mathf.Min(bestTimeInfo.CheckpointTimes[i], this.keyPointDurations[i + 1]);
            }

            SavedDataManager.Save();
        }
    }
}
