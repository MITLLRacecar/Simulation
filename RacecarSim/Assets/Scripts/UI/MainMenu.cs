using System.Collections.Generic;
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
    /// The game object containing the elements which allow the user to select the number of cars to spawn in the level.
    /// </summary>
    [SerializeField]
    private GameObject numCars;
    #endregion

    #region Constants
    /// <summary>
    /// The keys of the Konami Code.
    /// </summary>
    public static KeyCode[] KonamiCodes =
    {
        KeyCode.UpArrow,
        KeyCode.UpArrow,
        KeyCode.DownArrow,
        KeyCode.DownArrow,
        KeyCode.LeftArrow,
        KeyCode.RightArrow,
        KeyCode.LeftArrow,
        KeyCode.RightArrow,
        KeyCode.B,
        KeyCode.A
    };
    #endregion

    #region Public Interface
    /// <summary>
    /// Loads the level selected in the dropdown menu.
    /// </summary>
    public void BeginSimulation()
    {
        // Cache the current level and collection indices so we remember them the next time we load the main menu
        MainMenu.prevCollectionIndex = this.dropdowns[(int)Dropdowns.CollectionSelect].value;
        MainMenu.prevLevelIndex = this.dropdowns[(int)Dropdowns.LevelSelect].value;

        LevelManager.IsEvaluation = this.toggles[(int)Toggles.IsEvaluation].isOn;
        LevelManager.NumPlayers = this.dropdowns[(int)Dropdowns.NumCars].value + 1;

        LevelManager.LevelInfo = this.SelectedLevel;
        SceneManager.LoadScene(this.SelectedLevel.BuildIndex, LoadSceneMode.Single);
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
        this.bestTimesPane.UpdateEntries();
        this.bestTimesPane.gameObject.SetActive(true);
    }

    /// <summary>
    /// Handles when the user selects a new value in the level collection dropdown.
    /// </summary>
    /// <remarks>This overload exists to be called from Unity, since Unity cannot call functions with multiple parameters.</remarks>
    public void HandleLevelCollectionDropdownChange()
    {
        this.HandleLevelCollectionDropdownChange(0, false, 1);
    }

    /// <summary>
    /// Handles when the user selects a new value in the level collection dropdown.
    /// </summary>
    /// <param name="selectedLevel">The index of the level which should be selected by default in the level select menu.</param>
    /// <param name="isEvaluation">True if the isEvaluation toggle should be checked.</param>
    /// <param name="numCars">The number of cars to spawn in the level.</param>
    public void HandleLevelCollectionDropdownChange(int selectedLevel, bool isEvaluation, int numCars)
    {
        Dropdown levelSelect = this.dropdowns[(int)Dropdowns.LevelSelect];
        levelSelect.ClearOptions();
        levelSelect.AddOptions(this.SelectedLevelCollection.LevelNames);
        levelSelect.value = selectedLevel;

        this.HandleLevelDropdownChange(isEvaluation, numCars);
    }

    /// <summary>
    /// Handles when the user selects a new value in the level dropdown.
    /// </summary>
    /// <remarks>This overload exists to be called from Unity, since Unity cannot call functions with multiple parameters.</remarks>
    public void HandleLevelDropdownChange()
    {
        this.HandleLevelDropdownChange(false, 1);
    }

    /// <summary>
    /// Handles when the user selects a new value in the level dropdown.
    /// </summary>
    /// <param name="isEvaluation">True if the isEvaluation toggle should be checked.</param>
    /// <param name="numCars">The number of cars to spawn in the level.</param>
    public void HandleLevelDropdownChange(bool isEvaluation, int numCars)
    {
        // Show and set the IsEvaluation toggle if the level is winnable
        this.toggles[(int)Toggles.IsEvaluation].gameObject.SetActive(this.SelectedLevel.IsWinable);
        this.toggles[(int)Toggles.IsEvaluation].isOn = isEvaluation;

        // Show and populate the numCars dropdown if the level supports multiple cars
        Dropdown numCarDropdown = this.dropdowns[(int)Dropdowns.NumCars];
        if (this.SelectedLevel.MaxCars > 1)
        {
            if (this.SelectedLevel.MaxCars != numCarDropdown.options.Count)
            {
                List<string> options = new List<string>(this.SelectedLevel.MaxCars);
                for (int i = 1; i <= this.SelectedLevel.MaxCars; i++)
                {
                    options.Add(i.ToString());
                }
                numCarDropdown.ClearOptions();
                numCarDropdown.AddOptions(options);
            }
            this.numCars.SetActive(true);
        }
        else
        {
            this.numCars.SetActive(false);
        }

        // Regardless of whether it is shown, we always set the dropdown value since it determines NumPlayers when the level is loaded
        numCarDropdown.value = numCars - 1;
    }

    /// <summary>
    /// Close the program.
    /// </summary>
    public void Exit()
    {
        Application.Quit(0);
    }

    /// <summary>
    /// The current level collection selected in the dropdown menu.
    /// </summary>
    public LevelCollection SelectedLevelCollection
    {
        get
        {
            return LevelCollection.LevelCollections[this.dropdowns[(int)Dropdowns.CollectionSelect].value];
        }
    }

    /// <summary>
    /// The current level selected in the dropdown menu.
    /// </summary>
    public LevelInfo SelectedLevel
    {
        get
        {
            return SelectedLevelCollection.Levels[this.dropdowns[(int)Dropdowns.LevelSelect].value];
        }
    }
    #endregion

    /// <summary>
    /// The dropdown menus in the main menu, with values corresponding to the index in dropdowns.
    /// </summary>
    private enum Dropdowns
    {
        CollectionSelect = 0,
        LevelSelect = 1,
        NumCars = 2
    }

    /// <summary>
    /// The toggles (check boxes) in the main menu, with values corresponding to the index in toggles.
    /// </summary>
    private enum Toggles
    {
        IsEvaluation = 0
    }

    /// <summary>
    /// The index of the level collection selected the last time we loaded the main menu.
    /// </summary>
    private static int prevCollectionIndex = 0;

    /// <summary>
    /// The index of the level selected the last time we loaded the main menu.
    /// </summary>
    private static int prevLevelIndex = 0;

    /// <summary>
    /// The dropdown menus in the main menu.
    /// </summary>
    private Dropdown[] dropdowns;

    /// <summary>
    /// The toggles (check boxes) in the main menu.
    /// </summary>
    private Toggle[] toggles;

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

    /// <summary>
    /// The index of the next key in the Konami Code which the user must press.
    /// </summary>
    private int konamiCodeIndex = 0;

    private void Awake()
    {
        this.dropdowns = this.GetComponentsInChildren<Dropdown>();
        this.toggles = this.GetComponentsInChildren<Toggle>();

        this.controllsPane = this.GetComponentInChildren<ControllsUI>();
        this.settingsPane = this.GetComponentInChildren<SettingsUI>();
        this.bestTimesPane = this.GetComponentInChildren<BestTimesUI>();

        if (LevelInfo.WinableLevels.Count != SavedDataManager.Data.BestTimes.Length)
        {
            Debug.LogError("Best times do not align with current levels. Clearing best time data.");
            SavedDataManager.Data.ClearBestTimes();
            SavedDataManager.Save();
        }
    }

    private void Start()
    {
        // Hide panes
        this.controllsPane.gameObject.SetActive(false);
        this.settingsPane.gameObject.SetActive(false);
        this.bestTimesPane.gameObject.SetActive(false);

        this.numCars.SetActive(false);

        // Populate level collection dropdown
        Dropdown collectionSelect = this.dropdowns[(int)Dropdowns.CollectionSelect];
        collectionSelect.ClearOptions();
        List<string> collectionDisplayNames = new List<string>(LevelCollection.LevelCollections.Length);
        foreach (LevelCollection levelCollection in LevelCollection.LevelCollections)
        {
            collectionDisplayNames.Add(levelCollection.DisplayName);
        }
        collectionSelect.AddOptions(collectionDisplayNames);
        collectionSelect.value = MainMenu.prevCollectionIndex;

        // Begin with the previous level selection
        this.HandleLevelCollectionDropdownChange(MainMenu.prevLevelIndex, LevelManager.IsEvaluation, LevelManager.NumPlayers);
    }

    private void Update()
    {
        if (Input.GetKeyDown(MainMenu.KonamiCodes[this.konamiCodeIndex]))
        {
            this.konamiCodeIndex++;
            if (this.konamiCodeIndex == MainMenu.KonamiCodes.Length)
            {
                print("Cheat mode activated");
                Settings.CheatMode = true;
                this.konamiCodeIndex = 0;
            }
        }
        else if (Input.anyKeyDown)
        {
            this.konamiCodeIndex = 0;
        }
    }
}
