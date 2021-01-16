using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the settings pane of the main menu.
/// </summary>
public class SettingsUI : MonoBehaviour
{
    #region Public Interface
    /// <summary>
    /// Restore the default settings.
    /// </summary>
    public void RestoreDefaultSetting()
    {
        Settings.RestoreDefaults();
        this.UpdateInputs();
    }

    /// <summary>
    /// Save current settings and close the settings screen.
    /// </summary>
    public void SaveSettings()
    {
        Settings.IsRealism = this.toggles[(int)Toggles.IsRealism].isOn;
        Settings.DepthRes = (Settings.DepthResolution)this.dropdowns[(int)Dropdowns.DepthRes].value;
        Settings.Username = this.username.text;
        
        this.ApplyColorInputs();

        Settings.SaveSettings();
        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// Close the settings screen without saving any changes.
    /// </summary>
    public void Cancel()
    {
        this.UpdateInputs();
        this.gameObject.SetActive(false);
    }

    public void ColorChanged()
    {
        // TODO: update on fly
    }
    #endregion

    /// <summary>
    /// The dropdown menus in the main menu.
    /// </summary>
    private enum Dropdowns
    {
        DepthRes
    }

    /// <summary>
    /// The toggles (check boxes) in the main menu.
    /// </summary>
    private enum Toggles
    {
        IsRealism,
        FirstShiny,
    }

    /// <summary>
    /// The dropdown menus in the settings pane.
    /// </summary>
    private Dropdown[] dropdowns;

    /// <summary>
    /// The toggles (check boxes) in the settings pane.
    /// </summary>
    private Toggle[] toggles;

    /// <summary>
    /// The sliders in the settings pane.
    /// </summary>
    private Slider[] sliders;

    /// <summary>
    /// The input field in which the user enters their OpenEdx username.
    /// </summary>
    private InputField username;

    private void Awake()
    {
        this.dropdowns = this.GetComponentsInChildren<Dropdown>();
        this.toggles = this.GetComponentsInChildren<Toggle>();
        this.sliders = this.GetComponentsInChildren<Slider>();
        this.username = this.GetComponentInChildren<InputField>();
    }

    private void Start()
    {
        this.UpdateInputs();
    }

    /// <summary>
    /// Update all input values on the settings pane with the current settings.
    /// </summary>
    private void UpdateInputs()
    {
        this.toggles[(int)Toggles.IsRealism].isOn = Settings.IsRealism;
        this.dropdowns[(int)Dropdowns.DepthRes].value = (int)Settings.DepthRes;
        this.username.text = Settings.Username;

        this.LoadColors();
    }

    /// <summary>
    /// Applies the current customization options to the saved data.
    /// </summary>
    private void ApplyColorInputs()
    {
        // TODO: 3 is a magic number
        for (int i = 0; i < 3; i++)
        {
            SavedDataManager.Data.CarCustomizations[i].FrontColor = new SerializableColor(
                this.sliders[6 * i].value,
                this.sliders[6 * i + 1].value,
                this.sliders[6 * i + 2].value);

            SavedDataManager.Data.CarCustomizations[i].BackColor = new SerializableColor(
                this.sliders[6 * i + 3].value,
                this.sliders[6 * i + 4].value,
                this.sliders[6 * i + 5].value);

            SavedDataManager.Data.CarCustomizations[i].IsFrontShiny = this.toggles[(int)Toggles.FirstShiny + 2 * i].isOn;
            SavedDataManager.Data.CarCustomizations[i].IsBackShiny = this.toggles[(int)Toggles.FirstShiny + 2 * i + 1].isOn;
        }

        SavedDataManager.Save();
    }

    /// <summary>
    /// Update the customization interface with the current saved data.
    /// </summary>
    private void LoadColors()
    {
        // TODO: 3 is a magic number
        for (int i = 0; i < 3; i++)
        {
            CarCustomization customization = SavedDataManager.Data.CarCustomizations[i];
            this.sliders[6 * i].value = customization.FrontColor.r;
            this.sliders[6 * i + 1].value = customization.FrontColor.g;
            this.sliders[6 * i + 2].value = customization.FrontColor.b;

            this.sliders[6 * i + 3].value = customization.BackColor.r;
            this.sliders[6 * i + 4].value = customization.BackColor.g;
            this.sliders[6 * i + 5].value = customization.BackColor.b;

            this.toggles[(int)Toggles.FirstShiny + 2 * i].isOn = customization.IsFrontShiny;
            this.toggles[(int)Toggles.FirstShiny + 2 * i + 1].isOn = customization.IsBackShiny;
        }
    }
}
