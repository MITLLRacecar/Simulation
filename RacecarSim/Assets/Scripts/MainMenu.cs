using System.Collections.Generic;
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
    private static readonly List<string> levelNames = new List<string>()
    {
        "Demo",
        "Lab 1: Driving in Shapes",
        "Lab 2A: Color tape following",
        "Lab 2B: Color Cone following",
        "Lab 3B: Depth Cone following",
        "Lab 3C: Depth Wall parking",
        "Lab 4B: LIDAR Wall following",
        "Phase 1 Challenge: Cone Slaloming",
        "Lab 6B: Sensor Fusion"
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
        this.dropdown.AddOptions(MainMenu.levelNames);
    }
}
