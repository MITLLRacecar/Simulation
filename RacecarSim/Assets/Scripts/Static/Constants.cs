public static class Constants
{
    /// <summary>
    /// The maximum speed which the car can travel in m/s and still be considered "stopped".
    /// </summary>
    public const float MaxStopSeed = 0.02f;

    /// <summary>
    /// A bitmask which ignores the UI and IgnoreRaycast layers.
    /// </summary>
    public const int IgnoreUIMask = ~((1 << 5) | (1 << 2));

    /// <summary>
    /// A bitmask which ignores the UI, Player, and IgnoreRaycast layers.
    /// </summary>
    public const int IgnoreUIAndPlayerMask = ~((1 << 5) | (1 << 2) | (1 << 9));

    /// <summary>
    /// Default maxDistance to use for a raycast in world space.
    /// </summary>
    public const float RaycastMaxDistance = 1000;
}
