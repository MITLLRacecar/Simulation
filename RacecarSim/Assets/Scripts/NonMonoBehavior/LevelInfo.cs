/// <summary>
/// Information about a level.
/// </summary>
public class LevelInfo
{
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
    public bool IsWinable;

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
