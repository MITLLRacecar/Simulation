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
                    AutograderBuildIndex = 26,
                    AutograderLevelCode = "lab1",
                    AutograderLevels = new AutograderLevelInfo[]
                    {
                        new AutograderLevelInfo()
                        {
                            Title = "Straight",
                            Description = "Drive straight when RT is down.",
                            MaxPoints = 0.5f
                        },
                        new AutograderLevelInfo()
                        {
                            Title = "Reverse",
                            Description = "Drive backward when LT is down.",
                            MaxPoints = 0.5f,
                            DefaultCameraIndex = 2
                        },
                        new AutograderLevelInfo()
                        {
                            Title = "Left",
                            Description = "Turn left when the left joystick is pushed to the left.",
                            MaxPoints = 0.5f
                        },
                        new AutograderLevelInfo()
                        {
                            Title = "Right",
                            Description = "Turn right when the left joystick is pushed to the right.",
                            MaxPoints = 0.5f
                        },
                        new AutograderLevelInfo()
                        {
                            Title = "Circle",
                            Description = "Drive in a clockwise circle when the A button is pressed.",
                            MaxPoints = 2,
                            TimeLimit = 15
                        },
                        new AutograderLevelInfo()
                        {
                            Title = "Square",
                            Description = "Drive in a clockwise square when the B button is pressed.",
                            MaxPoints = 2.5f,
                            TimeLimit = 25
                        },
                        new AutograderLevelInfo()
                        {
                            Title = "Figure 8",
                            Description = "Drive in a figure 8 when the X button is pressed.",
                            MaxPoints = 2.5f,
                            TimeLimit = 35
                        },
                        new AutograderLevelInfo()
                        {
                            Title = "Chosen Shape",
                            Description = "Drive in a shape of your choice when the Y button is pressed.",
                            MaxPoints = 1,
                            TimeLimit = 15,
                            DoNotProceedUntilStopped = true
                        }
                    }
                }
            }
        },
        new LevelCollection()
        {
            DisplayName = "Lab 2: Color Camera",
            ShortName = "Lab 2",
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
                    AutograderBuildIndex = 34,
                    AutograderLevelCode = "lab2a",
                    AutograderLevels = new AutograderLevelInfo[]
                    {
                        new AutograderLevelInfo()
                        {
                            Title = "Straight",
                            Description = "Drive forward at full speed when RT is fully pressed.",
                            MaxPoints = 1,
                            IsRequired = true
                        },
                        new AutograderLevelInfo()
                        {
                            Title = "Gentle Turn",
                            Description = "Follow the red line for a 30 degree turn.",
                            MaxPoints = 1
                        },
                        new AutograderLevelInfo()
                        {
                            Title = "Sharp Turn",
                            Description = "Follow the blue line for a 90 degree turn.",
                            MaxPoints = 1
                        },
                        new AutograderLevelInfo()
                        {
                            Title = "Curved path",
                            Description = "Follow the green line for a winding path.",
                            MaxPoints = 1,
                            TimeLimit = 12
                        },
                        new AutograderLevelInfo()
                        {
                            Title = "Color Priority 1",
                            Description = "Follow the color priority red > green > blue.",
                            MaxPoints = 3
                        },
                        new AutograderLevelInfo()
                        {
                            Title = "Color Priority 2",
                            Description = "Follow the color priority red > green > blue for a more complex path.",
                            MaxPoints = 3,
                            TimeLimit = 20
                        },
                        new AutograderLevelInfo()
                        {
                            Title = "Extra: Perpendicular",
                            Description = "Identify and turn onto a line which is directly perpendicular to the car.",
                            MaxPoints = 0
                        },
                    }
                },
                new LevelInfo()
                {
                    DisplayName = "B: Cone Parking",
                    BuildIndex = 6,
                    HelpMessage = "Left-click on the screen to move the cone",
                    AutograderBuildIndex = 41,
                    AutograderLevelCode = "lab2b",
                    AutograderLevels = new AutograderLevelInfo[]
                    {
                        new AutograderLevelInfo()
                        {
                            Title = "Far",
                            Description = "Park 30 cm away from the cone.",
                            MaxPoints = 2
                        },
                        new AutograderLevelInfo()
                        {
                            Title = "Close",
                            Description = "Park 30 cm away from the cone.",
                            MaxPoints = 1
                        },
                        new AutograderLevelInfo()
                        {
                            Title = "Very Far",
                            Description = "Park 30 cm away from the cone.",
                            MaxPoints = 1,
                            TimeLimit = 15
                        },
                        new AutograderLevelInfo()
                        {
                            Title = "Slight left",
                            Description = "Park 30 cm away from the cone.",
                            MaxPoints = 2,
                            TimeLimit = 15
                        },
                        new AutograderLevelInfo()
                        {
                            Title = "Far right",
                            Description = "Park 30 cm away from the cone.",
                            MaxPoints = 2,
                            TimeLimit = 15
                        },
                        new AutograderLevelInfo()
                        {
                            Title = "Near and left",
                            Description = "Park 30 cm away from the cone.",
                            MaxPoints = 2,
                            TimeLimit = 15
                        }
                    }
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
                    AutograderBuildIndex = 47,
                    AutograderLevelCode = "lab3a",
                    AutograderLevels = new AutograderLevelInfo[]
                    {
                        new AutograderLevelInfo()
                        {
                            Title = "Full Speed",
                            Description = "Travel at full speed when there are no obstacles.",
                            MaxPoints = 1,
                            IsRequired = true
                        },
                        new AutograderLevelInfo()
                        {
                            Title = "Wall",
                            Description = "Avoid hitting the wall.",
                            MaxPoints = 2
                        },
                        new AutograderLevelInfo()
                        {
                            Title = "Blocks",
                            Description = "Avoid hitting the blocks.",
                            MaxPoints = 1
                        },
                        new AutograderLevelInfo()
                        {
                            Title = "Cone",
                            Description = "Avoid hitting the cone.",
                            MaxPoints = 1
                        },
                        new AutograderLevelInfo()
                        {
                            Title = "Off-centered Cone",
                            Description = "Avoid hitting the cone.",
                            MaxPoints = 2
                        },
                        new AutograderLevelInfo()
                        {
                            Title = "Gap",
                            Description = "Drive through the gap.",
                            MaxPoints = 1
                        },
                        new AutograderLevelInfo()
                        {
                            Title = "Ramp",
                            Description = "Drive up the ramp.",
                            MaxPoints = 2
                        },
                        new AutograderLevelInfo()
                        {
                            Title = "Extra: Cliff",
                            Description = "Avoid driving off the cliff.",
                            MaxPoints = 0
                        },
                    }
                },
                new LevelInfo()
                {
                    DisplayName = "B: Cone Parking (Revisited)",
                    BuildIndex = 9,
                    HelpMessage = "Left-click on the screen to move the cone and scroll to resize the cone",
                    AutograderBuildIndex = 55,
                    AutograderLevelCode = "lab3b",
                    AutograderLevels = new AutograderLevelInfo[]
                    {
                        new AutograderLevelInfo()
                        {
                            Title = "Normal Cone",
                            Description = "Park 30 cm away from the cone.",
                            MaxPoints = 1
                        },
                        new AutograderLevelInfo()
                        {
                            Title = "Large Cone",
                            Description = "Park 30 cm away from the cone.",
                            MaxPoints = 1
                        },
                        new AutograderLevelInfo()
                        {
                            Title = "Small Cone",
                            Description = "Park 30 cm away from the cone.",
                            MaxPoints = 1
                        },
                        new AutograderLevelInfo()
                        {
                            Title = "Slight left",
                            Description = "Park 30 cm away from the cone.",
                            MaxPoints = 1,
                            TimeLimit = 15
                        },
                        new AutograderLevelInfo()
                        {
                            Title = "Far right",
                            Description = "Park 30 cm away from the cone.",
                            MaxPoints = 1,
                            TimeLimit = 15
                        }
                    }
                },
                new LevelInfo()
                {
                    DisplayName = "C: Wall Parking",
                    BuildIndex = 10,
                    AutograderBuildIndex = 60,
                    AutograderLevelCode = "lab3c",
                    AutograderLevels = new AutograderLevelInfo[]
                    {
                        new AutograderLevelInfo()
                        {
                            Title = "Straight",
                            Description = "Align with and park 20 cm away from the wall.",
                            MaxPoints = 2.5f
                        },
                        new AutograderLevelInfo()
                        {
                            Title = "Slight Angle",
                            Description = "Align with and park 20 cm away from the wall.",
                            MaxPoints = 2.5f
                        },
                        new AutograderLevelInfo()
                        {
                            Title = "Moderate Angle",
                            Description = "Align with and park 20 cm away from the wall.",
                            MaxPoints = 2.5f
                        },
                        new AutograderLevelInfo()
                        {
                            Title = "Major Angle",
                            Description = "Align with and park 20 cm away from the wall.",
                            MaxPoints = 2.5f,
                            TimeLimit = 15
                        }
                    }
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
                    IsRaceable = true,
                    AutograderBuildIndex = 64,
                    AutograderLevelCode = "p1",
                    AutograderLevels = new AutograderLevelInfo[]
                    {
                        new AutograderLevelInfo()
                        {
                            Title = "Blue Cone",
                            Description = "Pass the left side of the blue cone.",
                            MaxPoints = 1
                        },
                        new AutograderLevelInfo()
                        {
                            Title = "Red Cone",
                            Description = "Pass the right side of the red cone.",
                            MaxPoints = 1
                        },
                        new AutograderLevelInfo()
                        {
                            Title = "Straight Course",
                            Description = "Complete the slalom course.",
                            MaxPoints = 3,
                            TimeLimit = 20
                        },
                        new AutograderLevelInfo()
                        {
                            Title = "Curved Course",
                            Description = "Complete the slalom course.",
                            MaxPoints = 3,
                            TimeLimit = 25
                        },
                        new AutograderLevelInfo()
                        {
                            Title = "Gates",
                            Description = "Drive through each cone gate.",
                            MaxPoints = 2,
                            TimeLimit = 15
                        },
                        new AutograderLevelInfo()
                        {
                            Title = "Full Course",
                            Description = "Complete the slalom course.",
                            MaxPoints = 5,
                            TimeLimit = 90,
                            TimeBonuses = new Vector2[]{ new Vector2(20, 1), new Vector2(30, 0.5f), new Vector2(45, 0), new Vector2(60, -1), new Vector2(float.PositiveInfinity, -2) }
                        }
                    }
                },
                new LevelInfo()
                {
                    DisplayName = "Cone Slalom: Hard",
                    BuildIndex = 12,
                    IsRaceable = true,
                    NumCheckpoints = 2,
                    AutograderBuildIndex = 70,
                    AutograderLevelCode = "p1hard",
                    AutograderLevels = new AutograderLevelInfo[]
                    {
                        new AutograderLevelInfo()
                        {
                            Title = "Extra: Close",
                            Description = "Complete the slalom course.",
                            MaxPoints = 0,
                            TimeLimit = 30
                        },
                        new AutograderLevelInfo()
                        {
                            Title = "Extra: Hill",
                            Description = "Complete the slalom course.",
                            MaxPoints = 0,
                            TimeLimit = 30
                        },
                        new AutograderLevelInfo()
                        {
                            Title = "Full Course",
                            Description = "Complete the slalom course.",
                            MaxPoints = 0,
                            TimeLimit = 120,
                            TimeBonuses = new Vector2[]{ new Vector2(60, 1), new Vector2(75, 0.5f), new Vector2(90, 0), new Vector2(float.PositiveInfinity, -0.5f) }
                        }
                    }
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
                    HelpMessage = "You can switch the 3rd person camera view by pressing the space bar"
                },
                new LevelInfo()
                {
                    DisplayName = "A: Safety Stop (Revisited)",
                    BuildIndex = 8,
                    AutograderBuildIndex = 73,
                    AutograderLevelCode = "lab4a",
                    AutograderLevels = new AutograderLevelInfo[]
                    {
                        new AutograderLevelInfo()
                        {
                            Title = "Full Speed",
                            Description = "Travel at full speed when there are no obstacles.",
                            MaxPoints = 1,
                            IsRequired = true,
                            DefaultCameraIndex = 2
                        },
                        new AutograderLevelInfo()
                        {
                            Title = "Wall",
                            Description = "Avoid hitting the wall.",
                            MaxPoints = 1,
                            DefaultCameraIndex = 2
                        },
                        new AutograderLevelInfo()
                        {
                            Title = "Blocks",
                            Description = "Avoid hitting the blocks.",
                            MaxPoints = 1,
                            DefaultCameraIndex = 2
                        },
                        new AutograderLevelInfo()
                        {
                            Title = "Cone",
                            Description = "Avoid hitting the cone.",
                            MaxPoints = 1,
                            DefaultCameraIndex = 2
                        },
                        new AutograderLevelInfo()
                        {
                            Title = "Gap",
                            Description = "Drive through the gap.",
                            MaxPoints = 1,
                            DefaultCameraIndex = 2
                        }
                    }
                },
                new LevelInfo()
                {
                    DisplayName = "B: Wall Following",
                    BuildIndex = 14,
                    IsRaceable = true,
                    NumCheckpoints = 2,
                    AutograderBuildIndex = 78,
                    AutograderLevelCode = "lab4b",
                    AutograderLevels = new AutograderLevelInfo[]
                    {
                        new AutograderLevelInfo()
                        {
                            Title = "45 Degree",
                            Description = "Navigate the course without touching any walls.",
                            MaxPoints = 2,
                            TimeLimit = 12,
                        },
                        new AutograderLevelInfo()
                        {
                            Title = "90 Degree",
                            Description = "Navigate the course without touching any walls.",
                            MaxPoints = 3,
                            TimeLimit = 20
                        },
                        new AutograderLevelInfo()
                        {
                            Title = "Tight",
                            Description = "Navigate the course without touching any walls.",
                            MaxPoints = 3,
                            TimeLimit = 30
                        },
                        new AutograderLevelInfo()
                        {
                            Title = "Posts",
                            Description = "Navigate the course without touching any posts.",
                            MaxPoints = 2,
                            TimeLimit = 30
                        },
                        new AutograderLevelInfo()
                        {
                            Title = "Full Course",
                            Description = "Navigate the course without touching any walls.",
                            MaxPoints = 5,
                            TimeLimit = 90,
                            TimeBonuses = new Vector2[]{ new Vector2(20, 1), new Vector2(30, 0.5f), new Vector2(45, 0), new Vector2(60, -1), new Vector2(float.PositiveInfinity, -2) }
                        },
                        new AutograderLevelInfo()
                        {
                            Title = "Extra: Organic",
                            Description = "Navigate the course.",
                            MaxPoints = 0,
                            TimeLimit = 90,
                            TimeBonuses = new Vector2[]{ new Vector2(45, 0.5f), new Vector2(60, 0), new Vector2(float.PositiveInfinity, -0.5f) }
                        }
                    }
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
                    HelpMessage = "Click a block to select it, then: left-click it to change it's tag, right-click it to change it's color, or scroll to rotate"
                },
                new LevelInfo()
                {
                    DisplayName = "AR Tag Decisions",
                    BuildIndex = 16,
                    IsRaceable = true,
                    NumCheckpoints = 2,
                    AutograderBuildIndex = 84,
                    AutograderLevelCode = "lab5",
                    AutograderLevels = new AutograderLevelInfo[]
                    {
                        new AutograderLevelInfo()
                        {
                            Title = "ID - Left",
                            Description = "Turn left upon seeing a marker with ID 0.",
                            MaxPoints = 1.5f
                        },
                        new AutograderLevelInfo()
                        {
                            Title = "ID - Right",
                            Description = "Turn right upon seeing a marker with ID 1.",
                            MaxPoints = 1.5f
                        },
                        new AutograderLevelInfo()
                        {
                            Title = "Orientation - Left",
                            Description = "Turn left if a marker with ID 199 is oriented left.",
                            MaxPoints = 1.5f
                        },
                        new AutograderLevelInfo()
                        {
                            Title = "Orientation - Right",
                            Description = "Turn right if a marker with ID 199 is oriented right.",
                            MaxPoints = 1.5f
                        },
                        new AutograderLevelInfo()
                        {
                            Title = "Color - Red",
                            Description = "Upon seeing a marker with ID 2, follow the color of the marker.",
                            MaxPoints = 2
                        },
                        new AutograderLevelInfo()
                        {
                            Title = "Color - Blue",
                            Description = "Upon seeing a marker with ID 2, follow the color of the marker.",
                            MaxPoints = 2
                        },
                        new AutograderLevelInfo()
                        {
                            Title = "Full Course",
                            Description = "Complete the randomly generated course.",
                            MaxPoints = 5,
                            TimeLimit = 75,
                            TimeBonuses = new Vector2[]{ new Vector2(15, 1), new Vector2(25, 0.5f), new Vector2(40, 0), new Vector2(60, -1), new Vector2(float.PositiveInfinity, -2) }
                        }
                    }
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
                    IsRaceable = true,
                    NumCheckpoints = 3,
                    AutograderBuildIndex = 91,
                    AutograderLevelCode = "final",
                    AutograderLevels = new AutograderLevelInfo[]
                    {
                        new AutograderLevelInfo()
                        {
                            Title = "Time Trial",
                            Description = "Navigate through the course.",
                            MaxPoints = 25,
                            TimeLimit = 300,
                            TimeBonuses = new Vector2[]{ new Vector2(75, 3), new Vector2(90, 2), new Vector2(120, 1), new Vector2(150, 0), new Vector2(180, -1), new Vector2(240, -3), new Vector2(float.PositiveInfinity, -5) }
                        }
                    }
                },
                new LevelInfo()
                {
                    DisplayName = "Grand Prix 2020",
                    BuildIndex = 18,
                    IsRaceable = true,
                    NumCheckpoints = 5,
                    MaxCars = 4,
                    AutograderBuildIndex = 92,
                    AutograderLevelCode = "final",
                    AutograderLevels = new AutograderLevelInfo[]
                    {
                        new AutograderLevelInfo()
                        {
                            Title = "Grad Prix 2020",
                            Description = "Navigate through the course.",
                            MaxPoints = 25,
                            TimeLimit = 360,
                            TimeBonuses = new Vector2[]{ new Vector2(105, 3), new Vector2(120, 2), new Vector2(150, 1), new Vector2(180, 0), new Vector2(240, -1), new Vector2(300, -3), new Vector2(float.PositiveInfinity, -5) }
                        }
                    }
                },
                new LevelInfo()
                {
                    DisplayName = "Grand Prix 2021",
                    BuildIndex = 95,
                    IsRaceable = true,
                    NumCheckpoints = 8,
                    MaxCars = 4,
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
                    HelpMessage = "The car's center of mass has been artificially raised dramatically"
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
                    IsRaceable = true
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
                    IsRaceable = true,
                    AutograderBuildIndex = 93,
                    AutograderLevelCode = "test",
                    AutograderLevels = new AutograderLevelInfo[]
                    {
                        new AutograderLevelInfo()
                        {
                            Title = "Test: Basic Finish",
                            Description = "This level exists for debugging purposes only.",
                            MaxPoints = 5,
                            TimeBonuses = new Vector2[]{ new Vector2(2, 0.5f), new Vector2(3, 0), new Vector2(5, -1), new Vector2(float.PositiveInfinity, -2) }
                        }
                    }
                },
                new LevelInfo()
                {
                    DisplayName = "Checkpoints and Finish",
                    BuildIndex = 22,
                    IsRaceable = true,
                    NumCheckpoints = 2,
                    AutograderBuildIndex = 94,
                    AutograderLevelCode = "test",
                    AutograderLevels = new AutograderLevelInfo[]
                    {
                        new AutograderLevelInfo()
                        {
                            Title = "Test: Checkpoints and Finish",
                            Description = "This level exists for debugging purposes only.",
                            MaxPoints = 5,
                            TimeBonuses = new Vector2[]{ new Vector2(2, 0.5f), new Vector2(3, 0), new Vector2(5, -1), new Vector2(float.PositiveInfinity, -2) }
                        }
                    }
                },
                new LevelInfo()
                {
                    DisplayName = "Race",
                    BuildIndex = 23,
                    IsRaceable = true,
                    NumCheckpoints = 2,
                    MaxCars = 4
                },
                new LevelInfo()
                {
                    DisplayName = "Objectives",
                    BuildIndex = 24,
                    IsRaceable = true,
                    NumCheckpoints = 2,
                    HelpMessage = "Pass red cones on the right and blue cones on the left"
                },
            }
        },
    };
    #endregion

    /// <summary>
    /// Initializes static level-related fields.
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
                if (level.IsRaceable)
                {
                    level.WinableIndex = LevelInfo.WinableLevels.Count;
                    LevelInfo.WinableLevels.Add(level);
                }
            }
        }
    }
}
