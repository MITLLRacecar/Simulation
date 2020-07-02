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
    /// The screen which shows the simulation controls.
    /// </summary>
    [SerializeField]
    private GameObject ControllsPane;

    /// <summary>
    /// The screen which allows the user to adjust settings.
    /// </summary>
    [SerializeField]
    private GameObject SettingsPane;
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
        MainMenu.lastLevel = this.dropdowns[Dropdowns.LevelSelect.GetHashCode()].value;
        SceneManager.LoadScene(levelMap[levelNames[MainMenu.lastLevel]], LoadSceneMode.Single);
    }

    /// <summary>
    /// Shows the controls screen.
    /// </summary>
    public void ShowControls()
    {
        this.ControllsPane.SetActive(!this.ControllsPane.activeInHierarchy);
    }

    /// <summary>
    /// Shows the settings screen.
    /// </summary>
    public void ShowSettings()
    {
        this.SettingsPane.SetActive(true);
    }

    /// <summary>
    /// Restore the default settings.
    /// </summary>
    public void RestoreDefaultSetting()
    {
        Settings.RestoreDefaults();
        this.UpdateSettingsUi();
    }

    /// <summary>
    /// Save current settings and close the settings screen.
    /// </summary>
    public void SaveSettings()
    {
        Settings.IsRealism = this.toggles[Toggles.IsRealism.GetHashCode()].isOn;
        Settings.DepthRes = (Settings.DepthResolution)this.dropdowns[Dropdowns.DepthRes.GetHashCode()].value;
        Settings.SaveSettings();
        this.SettingsPane.SetActive(false);
    }

    /// <summary>
    /// Close the settings screen without saving any changes.
    /// </summary>
    public void CancelSettings()
    {
        this.UpdateSettingsUi();
        this.SettingsPane.SetActive(false);
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
    /// The dropdown menus in the main menu.
    /// </summary>
    private enum Dropdowns
    {
        LevelSelect,
        DepthRes,
    }

    /// <summary>
    /// The toggles (check boxes) in the main menu.
    /// </summary>
    private enum Toggles
    {
        IsRealism,
    }

    /// <summary>
    /// The dropdown menus present in the main menu.
    /// </summary>
    private Dropdown[] dropdowns;

    /// <summary>
    /// The toggles (check boxes) present in the main menu.
    /// </summary>
    private Toggle[] toggles;

    /// <summary>
    /// The level previously selected with the level dropdown.
    /// </summary>
    private static int lastLevel;

    private void Awake()
    {
        this.dropdowns = this.GetComponentsInChildren<Dropdown>();
        this.toggles = this.GetComponentsInChildren<Toggle>();
    }

    private void Start()
    {
        // Hide panes
        this.ControllsPane.SetActive(false);
        this.SettingsPane.SetActive(false);

        // Populate level select dropdown
        Dropdown levelSelect = this.dropdowns[Dropdowns.LevelSelect.GetHashCode()];
        levelSelect.ClearOptions();
        levelSelect.AddOptions(MainMenu.levelNames);
        levelSelect.value = MainMenu.lastLevel;

        this.UpdateSettingsUi();
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            this.ControllsPane.SetActive(false);
        }
    }

    /// <summary>
    /// Update all UI elements on the settings screen with the current Settings.
    /// </summary>
    private void UpdateSettingsUi()
    {
        this.toggles[Toggles.IsRealism.GetHashCode()].isOn = Settings.IsRealism;
        this.dropdowns[Dropdowns.DepthRes.GetHashCode()].value = Settings.DepthRes.GetHashCode();
    }
}
