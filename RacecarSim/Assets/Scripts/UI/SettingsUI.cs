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
        Settings.IsRealism = this.toggles[Toggles.IsRealism.GetHashCode()].isOn;
        Settings.DepthRes = (Settings.DepthResolution)this.dropdowns[Dropdowns.DepthRes.GetHashCode()].value;
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
        IsRealism
    }

    /// <summary>
    /// The dropdown menus in the settings pane.
    /// </summary>
    private Dropdown[] dropdowns;

    /// <summary>
    /// The toggles (check boxes) in the settings pane.
    /// </summary>
    private Toggle[] toggles;

    private void Awake()
    {
        this.dropdowns = this.GetComponentsInChildren<Dropdown>();
        this.toggles = this.GetComponentsInChildren<Toggle>();
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
        this.toggles[Toggles.IsRealism.GetHashCode()].isOn = Settings.IsRealism;
        this.dropdowns[Dropdowns.DepthRes.GetHashCode()].value = Settings.DepthRes.GetHashCode();
    }
}
