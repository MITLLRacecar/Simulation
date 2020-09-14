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
    Wait,
    Paused
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
    /// The prefabs containing the screen managers for races with increasing numbers of cars.
    /// The 0th prefab supports 1 car, the 1st prefab supports 2 cars, and so on.
    /// </summary>
    [SerializeField]
    private GameObject[] raceScreenPrefabs;

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
    /// <param name="carIndex"></param>
    public static void HandleFinish(int carIndex)
    {
        if (LevelManager.IsEvaluation)
        {
            LevelManager.instance.finishTimes[carIndex] = LevelManager.instance.CurTime;

            if (!LevelManager.instance.finishTimes.Contains(0))
            {
                LevelManager.instance.screenManager.HandleWin(LevelManager.instance.finishTimes);
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
        Time.timeScale = 1.0f;
        this.defaultFixedDeltaTime = Time.fixedDeltaTime;
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
            this.screenManager.UpdateTimes(0.0f);
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
                this.pythonInteraface.HandleUpdate();
                if (LevelManager.IsEvaluation)
                {
                    this.screenManager.UpdateTimes(this.CurTime);
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

        // Handle timeFactor change on alt keys
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            this.ScaleTimeFactor(0.5f);
        }
        else if (Input.GetKeyDown(KeyCode.RightAlt) || Input.GetKeyDown(KeyCode.AltGr))
        {
            this.ScaleTimeFactor(2);
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
        this.pythonInteraface.HandleExit();
    }

    /// <summary>
    /// Spawns the correct number of players and creates the corresponding screen manager.
    /// </summary>
    private void SpawnPlayers()
    {
        this.players = new Racecar[LevelManager.NumPlayers];

        if (LevelManager.NumPlayers == 1)
        {
            this.players[0] = GameObject.Instantiate(this.playerPrefab, this.oneCarPosition, Quaternion.Euler(this.oneCarRotation)).GetComponentInChildren<Racecar>();
            this.players[0].Index = 0;

            this.resetPositions = new Vector3[1] { this.oneCarPosition };
            this.resetRotations = new Vector3[1] { this.oneCarRotation };
            
            // If there is only one player, create a HUD as the screen manager.
            Hud hud = GameObject.Instantiate(this.hudPrefab).GetComponent<Hud>();
            this.players[0].GetComponentInChildren<Racecar>().Hud = hud;
            this.screenManager = hud;
        }
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

            for (int i = 0; i < LevelManager.NumPlayers; i++)
            {
                this.players[i] = GameObject.Instantiate(this.playerPrefab, this.resetPositions[i], Quaternion.Euler(this.resetRotations[i])).GetComponentInChildren<Racecar>();
                this.players[i].Index = i;
            }

            // If there are multiple players, create a RaceScreen as the screen manager.
            this.screenManager = GameObject.Instantiate(this.raceScreenPrefabs[LevelManager.NumPlayers - 1]).GetComponent<RaceScreen>();
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
            this.mode = SimulationMode.Paused;
            this.ScaleTimeFactor(0);
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
    /// Change the current time factor, the rate at which time progresses.
    /// </summary>
    /// <param name="changeFactor">The number by which to multiply the current time factor.</param>
    private void ScaleTimeFactor(float changeFactor)
    {
        Time.timeScale = Mathf.Max(Mathf.Min(Time.timeScale * changeFactor, 1.0f), 1 / 64.0f);
        Time.fixedDeltaTime = this.defaultFixedDeltaTime * Time.timeScale;
        this.screenManager.UpdateTimeScale(Time.timeScale);
    }
}
