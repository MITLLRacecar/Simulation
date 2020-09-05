using UnityEngine;

public static class BestTimes
{
    #region Constants
    /// <summary>
    /// The full names of each level.
    /// </summary>
    private static readonly string[] fullNames =
    {
        "Phase 1 Challenge: Cone Slaloming",
        "Phase 1 Challenge: Cone Slaloming (Hard)",
        "Lab 5B: LIDAR Wall following",
        "Lab 6: Sensor Fusion"
    };
    #endregion

    #region Public Interface
    /// <summary>
    /// The levels for which best times are recorded.
    /// </summary>
    public enum Level
    {
        P1Challenge,
        P1ChallengeHard,
        Lab5B,
        Lab6,
        None
    }

    /// <summary>
    /// Update the best time for a level (if necessary) and save to disk.
    /// </summary>
    /// <param name="level">The level completed.</param>
    /// <param name="time">The time it took the user to complete the level.</param>
    public static void UpdateBestTime(Level level, float time)
    {
        BestTimes.times[level.GetHashCode()] = Mathf.Min(BestTimes.times[level.GetHashCode()], time);
        PlayerPrefs.SetFloat(level.ToString(), time);
    }

    /// <summary>
    /// Return the full name of each level formatted as a string.
    /// </summary>
    /// <returns>A string containing each level name on its own line.</returns>
    public static string GetFormattedNames()
    {
        string output = BestTimes.fullNames[0];
        for (int i = 1; i < BestTimes.fullNames.Length; i++)
        {
            output += $"\n{BestTimes.fullNames[i]}";
        }

        return output;
    }

    /// <summary>
    /// Return the best times for each level formatted as a string.
    /// </summary>
    /// <returns>A string containing each best time on its own line.</returns>
    public static string GetFormattedTimes()
    {
        string output = string.Empty;
        for (int i = 0; i < BestTimes.times.Length; i++)
        {
            if (BestTimes.times[i] == float.MaxValue)
            {
                output += "Not yet complete\n";
            }
            else
            {
                output += $"{BestTimes.times[0]:F2} seconds\n";
            }
        }

        // Trim trailing newline
        return output.Substring(0, output.Length - 1);
    }

    /// <summary>
    /// Clear all recorded best times.
    /// </summary>
    public static void Clear()
    {
        for (int i = 0; i < times.Length; i++)
        {
            PlayerPrefs.SetFloat(((Level)i).ToString(), float.MaxValue);
            times[i] = float.MaxValue;
        }
    }
    #endregion

    /// <summary>
    /// The user's best time for each level.
    /// </summary>
    private static float[] times = new float[BestTimes.fullNames.Length];

    static BestTimes()
    {
        BestTimes.LoadTimes();
    }

    /// <summary>
    /// Load the user's best times from disk.
    /// </summary>
    private static void LoadTimes()
    {
        // TODO: PlayerPrefs is not a secure way to store data.  Use serialization instead.
        for (int i = 0; i < times.Length; i++)
        {
            times[i] = PlayerPrefs.GetFloat(((Level)i).ToString(), float.MaxValue);
        }
    }
}
