using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Information about a collection of related levels.
/// </summary>
public class LevelCollection
{
    #region Constants
    /// <summary>
    /// The build index of the main menu level.
    /// </summary>
    public const int MainMenuBuildIndex = 0;

    /// <summary>
    /// The largest level build index.
    /// </summary>
    public const int LargestBuildIndex = 23;
    #endregion

    /// <summary>
    /// The name of the collection displayed to users.
    /// </summary>
    public string DisplayName;

    /// <summary>
    /// A shorter name for the collection which can be prepended to the names of the levels in the collection.
    /// </summary>
    public string ShortName;

    /// <summary>
    /// The levels in the collection.
    /// </summary>
    public LevelInfo[] Levels;

    /// <summary>
    /// The display names of the levels in the collection.
    /// </summary>
    public List<string> LevelNames
    {
        get
        {
            List<string> levelNames = new List<string>(this.Levels.Length);
            foreach (LevelInfo level in this.Levels)
            {
                levelNames.Add(level.DisplayName);
            }
            return levelNames;
        }
    }

    #region All Level Info
    public static readonly LevelCollection[] LevelCollections =
    {
        new LevelCollection()
        {
            DisplayName = "Miscellaneous",
            ShortName = "Misc",
            Levels = new LevelInfo[]
            {
                new LevelInfo()
                {
                    DisplayName = "Demo",
                    BuildIndex = 2,
                }
            }
        },
        new LevelCollection()
        {
            DisplayName = "Lab 1: Driving",
            ShortName = "Lab 1",
            Levels = new LevelInfo[]
            {
                new LevelInfo()
                {
                    DisplayName = "Driving in Shapes",
                    BuildIndex = 3,
                }
            }
        },
        new LevelCollection()
        {
            DisplayName = "Lab 2: Color Camera",
            ShortName = "Lab 1",
            Levels = new LevelInfo[]
            {
                new LevelInfo()
                {
                    DisplayName = "Jupyter Notebook",
                    BuildIndex = 4,
                },
                new LevelInfo()
                {
                    DisplayName = "A: Line Following",
                    BuildIndex = 5,
                },
                new LevelInfo()
                {
                    DisplayName = "B: Cone Parking",
                    BuildIndex = 6,
                }
            }
        },
        new LevelCollection()
        {
            DisplayName = "Lab 3: Depth Camera",
            ShortName = "Lab 3",
            Levels = new LevelInfo[]
            {
                new LevelInfo()
                {
                    DisplayName = "Jupyter Notebook",
                    BuildIndex = 7,
                },
                new LevelInfo()
                {
                    DisplayName = "A: Safety Stop",
                    BuildIndex = 8,
                },
                new LevelInfo()
                {
                    DisplayName = "B: Cone Parking (Revisited)",
                    BuildIndex = 9,
                },
                new LevelInfo()
                {
                    DisplayName = "C: Wall Parking",
                    BuildIndex = 10,
                }
            }
        },
        new LevelCollection()
        {
            DisplayName = "Phase 1 Challenge",
            ShortName = "P1",
            Levels = new LevelInfo[]
            {
                new LevelInfo()
                {
                    DisplayName = "Cone Slalom: Regular",
                    BuildIndex = 11,
                    IsWinable = true
                },
                new LevelInfo()
                {
                    DisplayName = "Cone Slalom: Hard",
                    BuildIndex = 12,
                    IsWinable = true
                }
            }
        },
        new LevelCollection()
        {
            DisplayName = "Lab 4: LIDAR",
            ShortName = "Lab 4",
            Levels = new LevelInfo[]
            {
                new LevelInfo()
                {
                    DisplayName = "Jupyter Notebook",
                    BuildIndex = 13,
                },
                new LevelInfo()
                {
                    DisplayName = "A: Safety Stop (Revisited)",
                    BuildIndex = 8,
                },
                new LevelInfo()
                {
                    DisplayName = "B: Wall Following",
                    BuildIndex = 14,
                    IsWinable = true
                }
            }
        },
        new LevelCollection()
        {
            DisplayName = "Lab 5: AR Tags",
            ShortName = "Lab 5",
            Levels = new LevelInfo[]
            {
                new LevelInfo()
                {
                    DisplayName = "Jupyter Notebook",
                    BuildIndex = 15,
                },
                new LevelInfo()
                {
                    DisplayName = "AR Tag Decisions",
                    BuildIndex = 16,
                    IsWinable = true
                },
            }
        },
        new LevelCollection()
        {
            DisplayName = "Final Challenge",
            ShortName = "Final",
            Levels = new LevelInfo[]
            {
                new LevelInfo()
                {
                    DisplayName = "Time Trial",
                    BuildIndex = 17,
                    IsWinable = true,
                    NumCheckpoints = 3
                },
                new LevelInfo()
                {
                    DisplayName = "Grand Prix",
                    BuildIndex = 18,
                    IsWinable = true,
                    NumCheckpoints = 5,
                    MaxCars = 4
                },
            }
        },
        new LevelCollection()
        {
            DisplayName = "Bonus 1: IMU",
            ShortName = "Bonus 1",
            Levels = new LevelInfo[]
            {
                new LevelInfo()
                {
                    DisplayName = "A: Roll Prevention",
                    BuildIndex = 19,
                },
                new LevelInfo()
                {
                    DisplayName = "B: Driving in Shapes (Revisited)",
                    BuildIndex = 3,
                },
            }
        },
        new LevelCollection()
        {
            DisplayName = "Bonus 2: Sensor Fusion",
            ShortName = "Bonus 2",
            Levels = new LevelInfo[]
            {
                new LevelInfo()
                {
                    DisplayName = "Speed Limit",
                    BuildIndex = 20,
                    IsWinable = true
                },
            }
        },
        new LevelCollection()
        {
            DisplayName = "Testing",
            ShortName = "Test",
            Levels = new LevelInfo[]
            {
                new LevelInfo()
                {
                    DisplayName = "Basic Finish",
                    BuildIndex = 21,
                    IsWinable = true
                },
                new LevelInfo()
                {
                    DisplayName = "Checkpoints and Finish",
                    BuildIndex = 22,
                    IsWinable = true,
                    NumCheckpoints = 2
                },
                new LevelInfo()
                {
                    DisplayName = "Race",
                    BuildIndex = 23,
                    IsWinable = true,
                    NumCheckpoints = 2,
                    MaxCars = 4
                },
            }
        },
    };
    #endregion

    /// <summary>
    /// Initializes the LevelCollection static field.
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Called by Unity")]
    private static void Initialize()
    {
        foreach (LevelCollection collection in LevelCollection.LevelCollections)
        {
            foreach (LevelInfo level in collection.Levels)
            {
                level.CollectionName = collection.ShortName;
            }
        }
    }
}
