using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The data which is serialized to disk to persist between simulations.
/// </summary>
[Serializable]
public class SavedData
{
    /// <summary>
    /// Maps evaluable levels to the best time information for that level.
    /// </summary>
    public Dictionary<LevelInfo, BestTimeInfo> BestTimes;

    /// <summary>
    /// The customization for each car, indexed by car.
    /// </summary>
    public CarCustomization[] CarCustomizations;

    /// <summary>
    /// Returns the default data, which should be used when no saved data can be found.
    /// </summary>
    public static SavedData Default
    {
        get
        {
            SavedData data = new SavedData()
            {
                BestTimes = new Dictionary<LevelInfo, BestTimeInfo>(),
                CarCustomizations = new CarCustomization[]
                {
                    new CarCustomization(Color.white),
                    new CarCustomization(Color.red),
                    new CarCustomization(Color.blue),
                    new CarCustomization(Color.yellow)
                }
            };

            data.ClearBestTimes();
            return data;
        }
    }

    /// <summary>
    /// Reset all best times to store no progress toward any levels.
    /// </summary>
    public void ClearBestTimes()
    {
        this.BestTimes.Clear();

        foreach (LevelCollection collection in LevelCollection.LevelCollections)
        {
            foreach (LevelInfo level in collection.Levels)
            {
                if (level.IsWinable)
                {
                    this.BestTimes.Add(level, new BestTimeInfo(level.NumCheckpoints));
                }
            }
        }
    }
}
