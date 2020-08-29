using System.Collections.Generic;
using UnityEngine;

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

    // Once again, it would be preferable to use a jagged array to set spawn positions,
    // but we have had to fall back to hard coding settable positions for up to four cars.

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

    public static void HandleError(string errorText)
    {
        Debug.LogError($">> Error: {errorText} Returning to default drive mode.");
        if (instance.hud != null)
        {
            instance.hud.SetMessage($"Error: {errorText} Returning to default drive mode.", Color.red, 5, 1);
        }
    }
    #endregion

    private static LevelManager instance;

    /// <summary>
    /// Encapsulates the interaction with Python scripts.
    /// </summary>
    private PythonInterface pythonInteraface;

    private Hud hud;

    private void Awake()
    {
        LevelManager.instance = this;
    }

    private void Start()
    {
        this.pythonInteraface = new PythonInterface(this.SpawnPlayers());
    }

    /// <summary>
    /// Spawns the correct number of players and creates a HUD if there is only one player.
    /// </summary>
    /// <returns>An array containing the spawned players.</returns>
    private Racecar[] SpawnPlayers()
    {
        Racecar[] output = new Racecar[LevelManager.NumPlayers];
        if (LevelManager.NumPlayers == 1)
        {
            output[0] = GameObject.Instantiate(this.playerPrefab, this.oneCarSpawn, Quaternion.identity).GetComponent<Racecar>();
            this.hud = GameObject.Instantiate(this.hudPrefab).GetComponent<Hud>();
            output[0].GetComponentInChildren<Racecar>().Hud = this.hud;
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
                output[i] = GameObject.Instantiate(this.playerPrefab, spawnLocations[i], Quaternion.identity).GetComponent<Racecar>();
            }
        }

        return output;
    }
}
