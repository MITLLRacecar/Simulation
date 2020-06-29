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
    #region Set in Unity Editor
    /// <summary>
    /// Images and text showing the simulation controls.
    /// </summary>
    [SerializeField]
    private GameObject Controlls;
    #endregion

    #region Constants
    /// <summary>
    /// Maps each level name to the build index of that level.
    /// </summary>
    private static readonly Dictionary<string, int> levelMap = new Dictionary<string, int>()
    {
        { "Demo", 2 },
        { "Lab 1: Driving in Shapes", 3 },
        { "Lab 2 Jupyter Notebook", 4 },
        { "Lab 2A: Color Image Line Following", 5 },
        { "Lab 2B: Color Image Cone Parking", 6 },
        { "Lab 3 Jupyter Notebook", 7 },
        { "Lab 3A: Depth Camera Safety Stop", 8 },
        { "Lab 3B: Depth Camera Cone Parking", 9 },
        { "Lab 3C: Depth Camera Wall Parking", 10 },
        { "Lab 4A: IMU Roll Prevention", 11 },
        { "Lab 4B: IMU Driving in Shapes", 3 },
        { "Phase 1 Challenge: Cone Slaloming", 12 },
        { "Lab 5A: LIDAR Safety Stop", 8 },
        { "Lab 5B: LIDAR Wall Following", 13 },
        { "Lab 6: Sensor Fusion", 14 }
    };

    /// <summary>
    /// The names of the simulation levels (in the order).
    /// </summary>
    private static readonly List<string> levelNames = MainMenu.levelMap.Keys.ToList<string>();
    #endregion

    #region Public Interface
    /// <summary>
    /// Loads the level selected in the dropdown menu.
    /// </summary>
    public void BeginSimulation()
    {
        MainMenu.lastSelected = this.dropdown.value;
        SceneManager.LoadScene(levelMap[levelNames[dropdown.value]], LoadSceneMode.Single);
    }

    /// <summary>
    /// Toggles whether the control information is shown.
    /// </summary>
    public void ToggleControls()
    {
        this.Controlls.SetActive(!this.Controlls.activeInHierarchy);
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

    /// <summary>
    /// The previously selected dropdown value.
    /// </summary>
    private static int lastSelected;

    private void Awake()
    {
        this.dropdown = this.GetComponentInChildren<Dropdown>();
    }

    private void Start()
    {
        this.dropdown.ClearOptions();
        this.dropdown.AddOptions(MainMenu.levelNames);
        this.dropdown.value = MainMenu.lastSelected;
        this.Controlls.SetActive(false);
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            this.Controlls.SetActive(false);
        }
    }
}
