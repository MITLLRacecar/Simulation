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
    /// Maps evaluable levels to the (best overall time, best checkpoint times) for that level.
    /// </summary>
    public Dictionary<LevelInfo, Tuple<float, float[]>> BestTimes;

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
            return new SavedData()
            {
                BestTimes = new Dictionary<LevelInfo, Tuple<float, float[]>>(),
                CarCustomizations = new CarCustomization[]
                {
                    new CarCustomization(Color.white),
                    new CarCustomization(Color.red),
                    new CarCustomization(Color.blue),
                    new CarCustomization(Color.yellow)
                }
            };
        }
    }
}
