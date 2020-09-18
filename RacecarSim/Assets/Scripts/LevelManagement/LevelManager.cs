using System;
using System.Collections.Generic;
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
    Wait
}

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

    // The Unity Editor does not support jagged arrays,
    // so we resort to hard coding settable positions for up to four cars.

    /// <summary>
    /// The position at which a single car will be spawned.
    /// </summary>
    [SerializeField]
    private Vector3 oneCarPosition = Vector3.zero;

    /// <summary>
    /// The rotation with which a single car will be spawned.
    /// </summary>
    [SerializeField]
    private Vector3 oneCarRotation = Vector3.zero;

    /// <summary>
    /// The positions at which two cars will be spawned.
    /// </summary>
    [SerializeField]
    private Vector3[] twoCarPositions = { new Vector3(-4, 0, 0), new Vector3(4, 0, 0) };

    /// <summary>
    /// The rotations with which a two cars will be spawned.
    /// </summary>
    [SerializeField]
    private Vector3[] twoCarRotations = { Vector3.zero, Vector3.zero };

    /// <summary>
    /// The positions at which three cars will be spawned.
    /// </summary>
    [SerializeField]
    private Vector3[] threeCarPositions = { new Vector3(-5, 0, 0), new Vector3(0, 0, 0), new Vector3(5, 0, 0) };

    /// <summary>
    /// The rotations with which a three cars will be spawned.
    /// </summary>
    [SerializeField]
    private Vector3[] threeCarRotations = { Vector3.zero, Vector3.zero, Vector3.zero };

    /// <summary>
    /// The positions at which four cars will be spawned.
    /// </summary>
    [SerializeField]
    private Vector3[] fourCarPositions = { new Vector3(-4, 0, 0), new Vector3(4, 0, 0), new Vector3(-4, 0, -8), new Vector3(4, 0, -8) };

    /// <summary>
    /// The rotations with which a four cars will be spawned.
    /// </summary>
    [SerializeField]
    private Vector3[] fourCarRotations = { Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero};
    #endregion

    #region Constants
    /// <summary>
    /// The texture which is copied to create all race camera views.
    /// </summary>
    private static readonly RenderTexture raceScreenTexture = new RenderTexture(960, 600, 24);

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
    /// <param name="errorText"></param>
    public static void HandleError(string errorText)
    {
        LevelManager.instance.mode = LevelManager.IsEvaluation ? SimulationMode.Wait : SimulationMode.DefaultDrive;
        errorText = $">> Error: {errorText} Returning to {LevelManager.instance.mode} mode.";

        Debug.LogError(errorText);
        LevelManager.instance.screenManager.ShowMessage(errorText, Color.red, 5, 1);
    }

    /// <summary>
    /// Handles when a car passes a checkpoint.
    /// </summary>
    /// <param name="carIndex">The index of the car which passed the checkpoint.</param>
    /// <param name="checkpointIndex">The index of the checkpoint which was passed.</param>
    /// <param name="checkpointPosition">The position of the checkpoint which was passed.</param>
    /// <param name="checkpointRotation">The rotation of the checkpoint which was passed.</param>
    public static void HandleCheckpoint(int carIndex, int checkpointIndex, Vector3 checkpointPosition, Vector3 checkpointRotation)
    {
        // Only count if we have not passed this checkpoint yet
        if (LevelManager.instance.checkpointTimes[carIndex, checkpointIndex] == 0)
        {
            LevelManager.instance.resetPositions[carIndex] = checkpointPosition;
            LevelManager.instance.resetRotations[carIndex] = checkpointRotation;
            LevelManager.instance.checkpointTimes[carIndex, checkpointIndex] = LevelManager.instance.CurTime;

            if (LevelManager.IsEvaluation)
            {
                LevelManager.instance.screenManager.UpdateCheckpointTimes(LevelManager.instance.checkpointTimes);
            }
        }
    }

    /// <summary>
    /// Handles when a car passes the finish line.
    /// </summary>
    /// <param name="carIndex">The index of the car which passed the finish line.</param>
    public static void HandleFinish(int carIndex)
    {
        if (LevelManager.IsEvaluation && LevelManager.instance.finishTimes[carIndex] == 0)
        {
            LevelManager.instance.finishTimes[carIndex] = LevelManager.instance.CurTime;

            if (!LevelManager.instance.finishTimes.Contains(0))
            {
                LevelManager.instance.screenManager.HandleWin(LevelManager.instance.finishTimes);
                LevelManager.instance.screenManager.UpdateTime(LevelManager.instance.CurTime);
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
        LevelManager.instance.screenManager.HandleFailure(carIndex, failureMessage);
    }

    /// <summary>
    /// Adds a time penalty for a car.
    /// </summary>
    /// <param name="carIndex">The car to penalize.</param>
    /// <param name="penalty">The time penalty in seconds.</param>
    public static void AddTimePenalty(int carIndex, float penalty)
    {
        // TODO
    }

    public static void RegisterCheckpoint()
    {
        LevelManager.instance.numCheckpoints++;
    }

    /// <summary>
    /// Reset a car to its last checkpoint.
    /// </summary>
    /// <param name="carIndex">The index of the car to reset.</param>
    public static void ResetCar(int carIndex)
    {
        // Relocate the car to its current reset point
        LevelManager.instance.players[carIndex].transform.position = LevelManager.instance.resetPositions[carIndex];
        LevelManager.instance.players[carIndex].transform.rotation = Quaternion.Euler(LevelManager.instance.resetRotations[carIndex]);

        // Bring the car to a complete stop
        Rigidbody carRigidBody = LevelManager.instance.players[carIndex].GetComponent<Rigidbody>();
        carRigidBody.velocity = Vector3.zero;
        carRigidBody.angularVelocity = Vector3.zero;
    }

    public static void UpdateNumConnectedPrograms(int numConnectedPrograms)
    {
        LevelManager.instance.numConnectedPrograms = numConnectedPrograms;

        // We cannot update the screen manager immediately since this may not be the main thread,
        // so we instead wait until the next call to Update
        LevelManager.instance.wasConnectedProgramsChanged = true;
    }
    #endregion

    /// <summary>
    /// A static reference to the current LevelManager (there is only ever one at a time).
    /// </summary>
    private static LevelManager instance;

    /// <summary>
    /// Encapsulates the interaction with Python scripts.
    /// </summary>
    private PythonInterface pythonInteraface;

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
    /// The number of checkpoints in the current level.
    /// </summary>
    private int numCheckpoints;

    /// <summary>
    /// The position to which each car should be reset.
    /// </summary>
    private Vector3[] resetPositions;

    /// <summary>
    /// The rotation with which each car should be reset.
    /// </summary>
    private Vector3[] resetRotations;

    /// <summary>
    /// The most recent checkpoint which each car passed.
    /// </summary>
    private int[] lastCheckpoint;

    /// <summary>
    /// The time at which the race begun.
    /// </summary>
    private float startTime;

    /// <summary>
    /// The times at which each car first reaches each checkpoint.
    /// 
    /// Indexed by car, then by checkpoint.
    /// </summary>
    private float[,] checkpointTimes;

    /// <summary>
    /// The times at which each car passes the finish line, indexed by car.
    /// </summary>
    private float[] finishTimes;

    /// <summary>
    /// The default fixed delta time for the current level.
    /// </summary>
    private float defaultFixedDeltaTime;

    /// <summary>
    /// The number of Python programs currently connected to RacecarSim.
    /// </summary>
    private int numConnectedPrograms;

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
    private int currentRaceCamera;

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
            return Time.time - this.startTime;
        }
    }

    private void Awake()
    {
        LevelManager.instance = this;
        this.mode = LevelManager.IsEvaluation ? SimulationMode.Wait : SimulationMode.DefaultDrive;
        this.defaultFixedDeltaTime = Time.fixedDeltaTime;

        this.raceCameras = this.GetComponentsInChildren<Camera>();
    }

    private void Start()
    {
        this.SpawnPlayers();
        this.pythonInteraface = new PythonInterface(this.players);
        this.lastCheckpoint = new int[this.numCheckpoints];

        if (LevelManager.IsEvaluation)
        {
            this.checkpointTimes = new float[LevelManager.NumPlayers, this.numCheckpoints];
            this.finishTimes = new float[LevelManager.NumPlayers];
            this.screenManager.UpdateTime(0.0f);
        }

        this.SetTimeScale(1.0f);
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
                this.pythonInteraface.HandleUpdate();
                if (LevelManager.IsEvaluation && this.finishTimes.Contains(0))
                {
                    this.screenManager.UpdateTime(this.CurTime);
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
            this.screenManager.UpdateConnectedPrograms(numConnectedPrograms);
            this.wasConnectedProgramsChanged = false; // TODO: there is probably a data race here
        }
    }

    private void OnApplicationQuit()
    {
        this.pythonInteraface?.HandleExit();
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
            this.players[0] = GameObject.Instantiate(this.playerPrefab, this.oneCarPosition, Quaternion.Euler(this.oneCarRotation)).GetComponentInChildren<Racecar>();
            this.players[0].Index = 0;

            this.resetPositions = new Vector3[1] { this.oneCarPosition };
            this.resetRotations = new Vector3[1] { this.oneCarRotation };
            
            Hud hud = GameObject.Instantiate(this.hudPrefab).GetComponent<Hud>();
            this.players[0].GetComponentInChildren<Racecar>().Hud = hud;
            this.screenManager = hud;
        }

        // If there are multiple players, spawn multiple cars and create a RaceScreen as the screen manager
        else
        {
            switch (LevelManager.NumPlayers)
            {
                case 2:
                    this.resetPositions = this.twoCarPositions;
                    this.resetRotations = this.twoCarRotations;
                    break;
                case 3:
                    this.resetPositions = this.threeCarPositions;
                    this.resetRotations = this.threeCarRotations;
                    break;
                case 4:
                    this.resetPositions = this.fourCarPositions;
                    this.resetRotations = this.fourCarRotations;
                    break;
                default:
                    Debug.LogError($"{LevelManager.NumPlayers} players is not supported.");
                    break;
            }

            RenderTexture[] playerCameraTextures = new RenderTexture[LevelManager.NumPlayers];
            for (int i = 0; i < LevelManager.NumPlayers; i++)
            {
                this.players[i] = GameObject.Instantiate(this.playerPrefab, this.resetPositions[i], Quaternion.Euler(this.resetRotations[i])).GetComponentInChildren<Racecar>();
                this.players[i].Index = i;

                playerCameraTextures[i] = new RenderTexture(LevelManager.raceScreenTexture);
                this.players[i].SetPlayerCameraFeatures(playerCameraTextures[i], false);
            }

            // Create a render texture for the race cameras
            RenderTexture raceCameraTexture = new RenderTexture(LevelManager.raceScreenTexture);
            foreach(Camera raceCamera in this.raceCameras)
            {
                raceCamera.targetTexture = raceCameraTexture;
            }

            // Start with only the first race camera enabled
            if (this.raceCameras.Length > 0)
            {
                this.raceCameras[0].enabled = true;
                this.currentRaceCamera = 0;
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
        if (this.mode != SimulationMode.UserProgram)
        {
            if (this.numConnectedPrograms > 0)
            {
                this.mode = SimulationMode.UserProgram;
                this.pythonInteraface.HandleStart();
                this.screenManager.UpdateMode(this.mode);
                this.startTime = Time.time;
            }
            else
            {
                this.screenManager.ShowMessage("You must connect a Python program before entering User Program mode.", Color.yellow, 5);
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
            this.HandlePause();
        }
        else
        {
            this.mode = SimulationMode.DefaultDrive;
            foreach (Racecar player in this.players)
            {
                player.DefaultDriveStart();
            }
        }
        this.screenManager.UpdateMode(this.mode);
    }

    /// <summary>
    /// Handles when the user restarts the current level (START + BACK).
    /// </summary>
    private void HandleRestart()
    {
        // Reload current level with the ReloadBuffer
        this.pythonInteraface.HandleExit();
        ReloadBuffer.BuildIndexToReload = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadSceneAsync(ReloadBuffer.BuildIndex, LoadSceneMode.Single);
    }

    /// <summary>
    /// Handles when the user exits to the main menu (escape).
    /// </summary>
    private void HandleExit()
    {
        this.pythonInteraface.HandleExit();
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// Handles when the user pauses the game (BACK in evaluation mode).
    /// </summary>
    private void HandlePause()
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
        Time.fixedDeltaTime = this.defaultFixedDeltaTime * timeScale;

        if (timeScale > 0)
        {
            this.screenManager.UpdateTimeScale(timeScale);
        }
    }
}
