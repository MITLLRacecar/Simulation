using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Handles the main menu.
/// </summary>
public class MainMenu : MonoBehaviour
{
    #region Constants
    /// <summary>
    /// The names of the simulation levels (in order).
    /// </summary>
    private static readonly Dictionary<string, int> levelNames = new Dictionary<string, int>()
    {
        { "Demo", 2 },
        { "Lab 1: Driving in Shapes", 3 },
        { "Lab 2A: Color Image Line Following", 4 },
        { "Lab 2B: Color Image Cone Parking", 5 },
        { "Lab 3A: Depth Camera Safety Stop", 6 },
        { "Lab 3B: Depth Camera Cone Parking", 7 },
        { "Lab 3C: Depth Camera Wall Parking", 8 },
        { "Lab 4: IMU Driving in Shapes", 3 },
        { "Phase 1 Challenge: Cone Slaloming", 10 },
        { "Lab 5A: LIDAR Safety Stop", 6 },
        { "Lab 5B: LIDAR Wall Following", 9 },
        { "Lab 6B: Sensor Fusion", 11 }
    };

    /// <summary>
    /// The number of levels in the Build Order before the first level in levelNames.
    /// </summary>
    private const int levelOffset = 2;
    #endregion

    #region Public Interface
    /// <summary>
    /// Begins the level selected in the dropdown menu.
    /// </summary>
    public void BeginSimulation()
    {
        SceneManager.LoadScene(this.dropdown.value + MainMenu.levelOffset, LoadSceneMode.Single);
    }

    /// <summary>
    /// Close the program.
    /// </summary>
    public void Exit()
    {
        Application.Quit(0);
    }
    #endregion

    /// <summary>
    /// The dropdown menu used to select the simulation level.
    /// </summary>
    private Dropdown dropdown;

    private void Awake()
    {
        this.dropdown = this.GetComponentInChildren<Dropdown>();
    }

    private void Start()
    {
        this.dropdown.ClearOptions();
        this.dropdown.AddOptions(MainMenu.levelNames.Keys.ToList<string>());
    }
}
