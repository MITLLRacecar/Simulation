using System;
using System.Collections.Generic;

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
    /// True if the level has a completable objective.
    /// </summary>
    public bool IsWinable = false;

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
    /// Default level info which can be used as a fallback when no level info is found.
    /// </summary>
    public static LevelInfo DefaultLevel = new LevelInfo()
    {
        DisplayName = "Default Level",
        BuildIndex = -1,
        IsWinable = false
    };
}
