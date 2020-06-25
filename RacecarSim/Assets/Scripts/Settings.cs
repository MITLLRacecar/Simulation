using UnityEngine;

public class Settings
{
    #region Constants
    /// <summary>
    /// vSyncCounts corresponding to each QualityLevel
    /// </summary>
    private static readonly int[] vSyncCounts = { 2, 1 };
    #endregion

    #region Public Interface
    /// <summary>
    /// Quality levels supported by the RacecarSim (higher quality is more computationally intensive).
    /// </summary>
    public enum QualityLevel
    {
        Low,
        High
    }

    /// <summary>
    /// If true, Gaussian error is added to all sensor readings.
    /// </summary>
    public bool isRealism = true;

    /// <summary>
    /// The quality level of the current simulation.
    /// </summary>
    public QualityLevel Quality
    {
        get
        {
            return this.quality;
        }
        set
        {
            this.quality = value;
            // QualitySettings.vSyncCount = Settings.vSyncCounts[value.GetHashCode()];
        }
    }
    #endregion Public Interface

    /// <summary>
    /// Private member for the Quality accessor
    /// </summary>
    private QualityLevel quality = Settings.QualityLevel.High;
}
