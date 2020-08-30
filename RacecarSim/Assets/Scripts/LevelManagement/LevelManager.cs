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
    /// The position at which a singe car will be spawned.
    /// </summary>
    [SerializeField]
    private Vector3 oneCarSpawn = new Vector3(0, 0, 0);

    /// <summary>
    /// The positions at which two cars will be spawned.
    /// </summary>
    [SerializeField]
    private Vector3[] twoCarSpawn = { new Vector3(-4, 0, 0), new Vector3(4, 0, 0) };

    /// <summary>
    /// The positions at which three cars will be spawned.
    /// </summary>
    [SerializeField]
    private Vector3[] threeCarSpawn = { new Vector3(-5, 0, 0), new Vector3(0, 0, 0), new Vector3(5, 0, 0) };

    /// <summary>
    /// The positions at which four cars will be spawned.
    /// </summary>
    [SerializeField]
    private Vector3[] fourCarSpawn = { new Vector3(-4, 0, 0), new Vector3(4, 0, 0), new Vector3(-4, 0, -8), new Vector3(4, 0, -8) };
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

    public static void HandleError(string errorText)
    {
        Debug.LogError($">> Error: {errorText} Returning to default drive mode.");
        instance.screenManager.ShowMessage($"Error: {errorText} Returning to default drive mode.", Color.red, 5, 1);
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

    private void Awake()
    {
        LevelManager.instance = this;
        this.mode = LevelManager.IsEvaluation ? SimulationMode.Wait : SimulationMode.DefaultDrive;
    }

    private void Start()
    {
        this.SpawnPlayers();
        this.pythonInteraface = new PythonInterface(this.players);
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

        // Handle exit on escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            this.HandleExit();
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
            this.players[0] = GameObject.Instantiate(this.playerPrefab, this.oneCarSpawn, Quaternion.identity).GetComponent<Racecar>();
            
            Hud hud = GameObject.Instantiate(this.hudPrefab).GetComponent<Hud>();
            this.players[0].GetComponentInChildren<Racecar>().Hud = hud;
            this.screenManager = hud;
        }
        else
        {
            Vector3[] spawnLocations = new Vector3[0];
            switch (LevelManager.NumPlayers)
            {
                case 2:
                    spawnLocations = this.twoCarSpawn;
                    break;
                case 3:
                    spawnLocations = this.threeCarSpawn;
                    break;
                case 4:
                    spawnLocations = this.fourCarSpawn;
                    break;
                default:
                    Debug.LogError($"{LevelManager.NumPlayers} players is not supported.");
                    break;
            }

            for (int i = 0; i < LevelManager.NumPlayers; i++)
            {
                this.players[i] = GameObject.Instantiate(this.playerPrefab, spawnLocations[i], Quaternion.identity).GetComponent<Racecar>();
            }

            this.screenManager = GameObject.Instantiate(this.raceScreenPrefabs[LevelManager.NumPlayers - 1]).GetComponent<RaceScreen>();
        }
        this.screenManager.UpdateMode(this.mode);
    }

    /// <summary>
    /// Handles when the user presses the START button.
    /// </summary>
    private void HandleStart()
    {
        this.mode = SimulationMode.UserProgram;
        this.pythonInteraface.HandleStart();
        this.screenManager.UpdateMode(this.mode);
    }

    /// <summary>
    /// Handles when the user presses the BACK button.
    /// </summary>
    private void HandleBack()
    {
        if (LevelManager.IsEvaluation)
        {
            this.mode = SimulationMode.Paused;
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
}
