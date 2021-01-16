using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Information about a level.
/// </summary>
[Serializable]
public class LevelInfo
{
    /// <summary>
    /// A list of the levels for which IsWinable is true.
    /// </summary>
    public static List<LevelInfo> WinableLevels = new List<LevelInfo>();

    /// <summary>
    /// The name of the level displayed to users.
    /// </summary>
    public string DisplayName;

    /// <summary>
    /// The index of the level in the build settings.
    /// </summary>
    public int BuildIndex;

    /// <summary>
    /// The index of the first corresponding autograder level in the build settings.
    /// </summary>
    public int AutograderBuildIndex;

    /// <summary>
    /// The way the level is encoded in the autograder score code.
    /// </summary>
    public string AutograderLevelCode;

    /// <summary>
    /// The information about each autograder level for this lab.
    /// </summary>
    public AutograderLevelInfo[] AutograderLevels;

    /// <summary>
    /// True if the level supports LevelManagerMode.Race.
    /// </summary>
    public bool IsRaceable = false;

    /// <summary>
    /// The index of a winable level in LevelInfo.WinableLevels.
    /// </summary>
    public int WinableIndex = -1;

    /// <summary>
    /// The maximum number of cars the level supports.
    /// </summary>
    public int MaxCars = 1;

    /// <summary>
    /// The number of checkpoints in the level.
    /// </summary>
    public int NumCheckpoints = 0;

    /// <summary>
    /// The short name of the level collection to which the level belongs.
    /// </summary>
    public string CollectionName;

    /// <summary>
    /// The message shown at the beginning of the level describing any special features or requirements.
    /// </summary>
    public string HelpMessage;

    /// <summary>
    /// The full name of the level, including the level collection to which it belongs.
    /// </summary>
    public string FullName
    {
        get
        {
            return $"{this.CollectionName}-{this.DisplayName}";
        }
    }

    /// <summary>
    /// The sum of the max score for each autograder level for this lab.
    /// </summary>
    public float AutograderMaxScore
    {
        get
        {
            return this.AutograderLevels.Aggregate(0.0f, (total, next) => total + next.MaxPoints);
        }
    }

    /// <summary>
    /// Default level info which can be used as a fallback when no level info is found.
    /// </summary>
    public static LevelInfo DefaultLevel = new LevelInfo()
    {
        DisplayName = "Default Level",
        BuildIndex = -1,
        IsRaceable = false
    };
}
