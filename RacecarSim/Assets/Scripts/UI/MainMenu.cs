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
        { "Phase 1 Challenge: Cone Slaloming (Hard)", 13 },
        { "Lab 5A: LIDAR Safety Stop", 8 },
        { "Lab 5B: LIDAR Wall Following", 14 },
        { "Lab 6: Sensor Fusion", 15 },
        { "Lab 7: AR Tags", 16 },
        { "Time Trial", 17 }
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
        MainMenu.lastLevel = this.levelSelect.value;
        SceneManager.LoadScene(levelMap[levelNames[MainMenu.lastLevel]], LoadSceneMode.Single);
    }

    /// <summary>
    /// Shows the controls screen.
    /// </summary>
    public void ShowControls()
    {
        this.controllsPane.gameObject.SetActive(true);
    }

    /// <summary>
    /// Shows the settings screen.
    /// </summary>
    public void ShowSettings()
    {
        this.settingsPane.gameObject.SetActive(true);
    }

    /// <summary>
    /// Show the best times screen.
    /// </summary>
    public void ShowBestTimes()
    {
        this.bestTimesPane.UpdateTexts();
        this.bestTimesPane.gameObject.SetActive(true);
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
    /// The dropdown menus used to select the level to load.
    /// </summary>
    private Dropdown levelSelect;

    /// <summary>
    /// The level previously selected with the level dropdown.
    /// </summary>
    private static int lastLevel;

    /// <summary>
    /// The screen which shows the controls.
    /// </summary>
    private ControllsUI controllsPane;

    /// <summary>
    /// The screen which allows the user to adjust settings.
    /// </summary>
    private SettingsUI settingsPane;

    /// <summary>
    /// The screen which shows the user's best times.
    /// </summary>
    private BestTimesUI bestTimesPane;

    private void Awake()
    {
        this.levelSelect = this.GetComponentInChildren<Dropdown>();

        this.controllsPane = this.GetComponentInChildren<ControllsUI>();
        this.settingsPane = this.GetComponentInChildren<SettingsUI>();
        this.bestTimesPane = this.GetComponentInChildren<BestTimesUI>();
    }

    private void Start()
    {
        // Hide panes
        this.controllsPane.gameObject.SetActive(false);
        this.settingsPane.gameObject.SetActive(false);
        this.bestTimesPane.gameObject.SetActive(false);

        // Populate level select dropdown
        this.levelSelect.ClearOptions();
        this.levelSelect.AddOptions(MainMenu.levelNames);
        this.levelSelect.value = MainMenu.lastLevel;
    }
}
