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
    /// The default value of HideCarsInColorCamera.
    /// </summary>
    private const bool defaultHideCarsInColorCamera = false;

    /// <summary>
    /// The default value of Username.
    /// </summary>
    public const string DefaultUsername = "Default";

    /// <summary>
    /// The default fixed delta time (in seconds) chosen by Unity.
    /// </summary>
    public static readonly float DefaultFixedDeltaTime = Time.fixedDeltaTime;

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
    public static bool IsRealism;

    /// <summary>
    /// If true, other cars do not show up in the color camera.
    /// </summary>
    public static bool HideCarsInColorCamera;

    /// <summary>
    /// If true, controller input is still sent in evaluation mode but best times are not recorded.
    /// </summary>
    public static bool CheatMode = false;

    /// <summary>
    /// The resolution to use for the depth camera.
    /// </summary>
    public static DepthResolution DepthRes;

    /// <summary>
    /// The user's OpenEdx username.
    /// </summary>
    public static string Username;

    /// <summary>
    /// The factor that the depth image is smaller than the color image along each axis.
    /// </summary>
    public static int DepthDivideFactor
    {
        get
        {
            return Settings.depthDivideFactors[(int)Settings.DepthRes];
        }
    }

    /// <summary>
    /// Restore all settings to the default values.
    /// </summary>
    public static void RestoreDefaults()
    {
        Settings.IsRealism = Settings.defaultIsRealism;
        Settings.HideCarsInColorCamera = Settings.defaultHideCarsInColorCamera;
        Settings.DepthRes = Settings.defaultDepthRes;
        Settings.Username = Settings.DefaultUsername;
    }

    /// <summary>
    /// Save all settings to PlayerPrefs.
    /// </summary>
    public static void SaveSettings()
    {
        PlayerPrefs.SetInt("IsRealism", System.Convert.ToInt32(Settings.IsRealism));
        PlayerPrefs.SetInt("HideCarsInColorCamera", System.Convert.ToInt32(Settings.HideCarsInColorCamera));
        PlayerPrefs.SetInt("DepthRes", (int)Settings.DepthRes);
        PlayerPrefs.SetString("Username", Settings.Username);
    }
    #endregion

    static Settings()
    {
        Settings.LoadSettings();
    }

    /// <summary>
    /// Load all settings from PlayerPrefs.
    /// </summary>
    private static void LoadSettings()
    {
        Settings.IsRealism = System.Convert.ToBoolean(PlayerPrefs.GetInt("IsRealism", System.Convert.ToInt32(Settings.defaultIsRealism)));
        Settings.HideCarsInColorCamera = System.Convert.ToBoolean(PlayerPrefs.GetInt("HideCarsInColorCamera", System.Convert.ToInt32(Settings.defaultHideCarsInColorCamera)));
        Settings.DepthRes = (DepthResolution)PlayerPrefs.GetInt("DepthRes", (int)Settings.defaultDepthRes);
        Settings.Username = PlayerPrefs.GetString("Username", Settings.DefaultUsername);
    }
}
