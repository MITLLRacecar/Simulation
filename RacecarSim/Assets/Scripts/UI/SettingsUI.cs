using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the settings pane of the main menu.
/// </summary>
public class SettingsUI : MonoBehaviour
{
    #region Set in Unity Editor
    [SerializeField]
    private Material[] carMaterials = new Material[6];
    #endregion

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
        Car1FrontShiny,
        Car1BackShiny,
        Car2FrontShiny,
        Car2BackShiny,
        Car3FrontShiny,
        Car3BackShiny
    }

    private enum Sliders
    {
        Car1Front = 0,
        Car1Back = 3,
        Car2Front = 6,
        Car2Back = 9,
        Car3Front = 12,
        Car3Back = 15
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

    private void Awake()
    {
        this.dropdowns = this.GetComponentsInChildren<Dropdown>();
        this.toggles = this.GetComponentsInChildren<Toggle>();
        this.sliders = this.GetComponentsInChildren<Slider>();
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

        this.LoadColors();
    }

    private void ApplyColorInputs()
    {
        for (int i = 0; i < this.carMaterials.Length; i++)
        {
            this.carMaterials[i].color = new Color(this.sliders[3 * i].value, this.sliders[3 * i + 1].value, this.sliders[3 * i + 2].value);

            float metallic = this.toggles[(int)Toggles.Car1FrontShiny + i].isOn ? 1 : 0;
            this.carMaterials[i].SetFloat("_Metallic", metallic);
        }
    }

    private void LoadColors()
    {
        for (int i = 0; i < this.carMaterials.Length; i++)
        {
            Color color = this.carMaterials[i].color;
            this.sliders[3 * i].value = color.r;
            this.sliders[3 * i + 1].value = color.g;
            this.sliders[3 * i + 2].value = color.b;

            this.toggles[(int)Toggles.Car1FrontShiny + i].isOn = this.carMaterials[i].GetFloat("_Metallic") > 0;
        }
    }
}
