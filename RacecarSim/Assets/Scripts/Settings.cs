using UnityEngine;

public static class Settings
{
    #region Constants
    /// <summary>
    /// The default value of DepthRes.
    /// </summary>
    private const DepthResolution defaultDepthRes = DepthResolution.High;

    /// <summary>
    /// The default value of IsRealism. 
    /// </summary>
    private const bool defaultIsRealism = true;

    /// <summary>
    /// The divide factor corresponding to each DepthResolution.
    /// </summary>
    private static readonly int[] depthDivideFactors =
    {
        32,
        16,
        8
    };
    #endregion

    #region Public Interface
    /// <summary>
    /// Quality levels supported by the RacecarSim (higher quality is more computationally intensive).
    /// </summary>
    public enum DepthResolution
    {
        Low,
        Moderate,
        High,
    }

    /// <summary>
    /// If true, Gaussian error is added to all sensor readings.
    /// </summary>
    public static bool IsRealism = true;

    /// <summary>
    /// The resolution to use for the depth camera.
    /// </summary>
    public static DepthResolution DepthRes;

    /// <summary>
    /// The factor that the depth image is smaller than the color image along each axis.
    /// </summary>
    public static int DepthDivideFactor
    {
        get
        {
            return Settings.depthDivideFactors[Settings.DepthRes.GetHashCode()];
        }
    }

    static Settings()
    {
        if (!(PlayerPrefs.HasKey("IsRealism") && PlayerPrefs.HasKey("DepthRes")))
        {
            Settings.RestoreDefaults();
            Settings.SaveSettings();
        }
        else
        {
            Settings.LoadSettings();
        }
    }

    /// <summary>
    /// Restore all settings to the default values.
    /// </summary>
    public static void RestoreDefaults()
    {
        Settings.IsRealism = Settings.defaultIsRealism;
        Settings.DepthRes = Settings.defaultDepthRes;
    }

    /// <summary>
    /// Save all settings to PlayerPrefs.
    /// </summary>
    public static void SaveSettings()
    {
        PlayerPrefs.SetInt("IsRealism", System.Convert.ToInt32(Settings.IsRealism));
        PlayerPrefs.SetInt("DepthRes", (int)Settings.DepthRes);
    }
    #endregion

    /// <summary>
    /// Load all settings from PlayerPrefs.
    /// </summary>
    private static void LoadSettings()
    {
        Settings.IsRealism = System.Convert.ToBoolean(PlayerPrefs.GetInt("IsRealism"));
        Settings.DepthRes = (DepthResolution)PlayerPrefs.GetInt("DepthRes");
    }
}
