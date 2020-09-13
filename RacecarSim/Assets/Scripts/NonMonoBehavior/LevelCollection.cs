using System.Collections.Generic;

/// <summary>
/// Information about a collection of related levels.
/// </summary>
public class LevelCollection
{
    /// <summary>
    /// The name of the collection displayed to users.
    /// </summary>
    public string DisplayName;

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
    public static LevelCollection[] LevelCollections =
    {
        new LevelCollection()
        {
            DisplayName = "Miscellaneous",
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
            Levels = new LevelInfo[]
            {
                new LevelInfo()
                {
                    DisplayName = "Time Trial",
                    BuildIndex = 17,
                    IsWinable = true
                },
                new LevelInfo()
                {
                    DisplayName = "Grand Prix",
                    BuildIndex = 18,
                    IsWinable = true,
                    MaxCars = 4
                },
            }
        },
        new LevelCollection()
        {
            DisplayName = "Bonus 1: IMU",
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
                    IsWinable = true
                },
                new LevelInfo()
                {
                    DisplayName = "Race",
                    BuildIndex = 23,
                    IsWinable = true,
                    MaxCars = 4
                },
            }
        },
    };
    #endregion
}
