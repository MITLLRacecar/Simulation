using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Handles the main menu.
/// </summary>
public class MainMenu : MonoBehaviour
{
    #region Public Interface
    /// <summary>
    /// Loads the level selected in the dropdown menu.
    /// </summary>
    public void BeginSimulation()
    {
        // Cache the current level and collection indices so we remember them the next time we load the main menu
        MainMenu.prevCollectionIndex = this.dropdowns[(int)Dropdowns.CollectionSelect].value;
        MainMenu.prevLevelIndex = this.dropdowns[(int)Dropdowns.LevelSelect].value;

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
        this.bestTimesPane.UpdateTexts();
        this.bestTimesPane.gameObject.SetActive(true);
    }

    /// <summary>
    /// Update the options in the level selection dropdown menu.
    /// </summary>
    /// <param name="selection">The index of the level which should be selected by default in the menu.</param>
    public void UpdateLevelDropDown(int selection = 0)
    {
        Dropdown levelSelect = this.dropdowns[(int)Dropdowns.LevelSelect];
        levelSelect.ClearOptions();
        levelSelect.AddOptions(this.SelectedLevelCollection.LevelNames);
        levelSelect.value = selection;
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
        CollectionSelect,
        LevelSelect
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
    /// The dropdown menus used to select the level to load.
    /// </summary>
    private Dropdown[] dropdowns;

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
        this.dropdowns = this.GetComponentsInChildren<Dropdown>();

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

        this.UpdateLevelDropDown(MainMenu.prevLevelIndex);
    }
}
