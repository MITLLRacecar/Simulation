using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages random aspects of a level.
/// </summary>
public class VariableManager : MonoBehaviour
{
    #region Set in Unity Editor
    /// <summary>
    /// The number of turns which are randomly set in the race.
    /// </summary>
    [SerializeField]
    private int numVariableTurns = 0;

    // The Unity editor does not support jagged arrays, so we have hard coded 3 sets of variable colors,
    // allowing the level designer to use up to 3 distinct sets of variable colors throughout the race

    /// <summary>
    /// The first set of colors which are randomized in the race.
    /// </summary>
    [SerializeField]
    private RaceColor[] variableColor1 = new RaceColor[0];

    /// <summary>
    /// The second set of colors which are randomized in the race.
    /// </summary>
    [SerializeField]
    private RaceColor[] variableColor2 = new RaceColor[0];

    /// <summary>
    /// The third set of colors which are randomized in the race.
    /// </summary>
    [SerializeField]
    private RaceColor[] variableColor3 = new RaceColor[0];
    #endregion

    #region Public Interface
    /// <summary>
    /// The preset colors to be used in races.
    /// </summary>
    public enum RaceColor
    {
        White,
        Black,
        Red,
        Green,
        Blue,
        Pink,
        Yellow,
        Orange,
        Purple
    };

    /// <summary>
    /// The potential directions of a variable turn in a race.
    /// </summary>
    public enum RaceTurn
    {
        None,
        Left,
        Right
    }

    /// <summary>
    /// Returns the turn direction for a particular variable turn in the race.
    /// </summary>
    /// <param name="turnIndex">The index of the variable turn as it appears in the race.</param>
    /// <returns>The direction of the turn at turnIndex.</returns>
    public static RaceTurn GetVariableTurn(int turnIndex)
    {
        return VariableManager.raceTurns[turnIndex];
    }

    /// <summary>
    /// Returns the color of an object based on the variable color set to which it belongs.
    /// </summary>
    /// <param name="setIndex">The index of the color set as it appears in the race.</param>
    /// <param name="colorIndex">The index of the object in that color set.</param>
    /// <returns>The color of the object.</returns>
    public static Color GetVariableColor(int setIndex, int colorIndex)
    {
        return VariableManager.raceColors[setIndex][colorIndex];
    }
    #endregion

    /// <summary>
    /// The Color value corresponding to each value of RaceColor
    /// </summary>
    private static Color[] raceColorValues =
    {
        Color.white,
        Color.black,
        Color.red,
        Color.green,
        new Color(0, 0.5f, 1), // Blue
        new Color(1, 0, 1), // Pink
        Color.yellow,
        new Color(1, 0.5f, 0), // Orange
        new Color(0.5f, 0, 1) // Purple
    };

    /// <summary>
    /// The direction of each variable turn in the race, in order.
    /// </summary>
    private static RaceTurn[] raceTurns;

    /// <summary>
    /// The color assignments for each variable color set in the race, in order.
    /// </summary>
    private static Color[][] raceColors;

    private void Awake()
    {
        // Randomly set each race turn to Left or Right with equal probability
        VariableManager.raceTurns = new RaceTurn[this.numVariableTurns];
        for (int i = 0; i < VariableManager.raceTurns.Length; i++)
        {
            VariableManager.raceTurns[i] = Random.value < 0.5f ? RaceTurn.Left : RaceTurn.Right;
        }

        // Randomize colors within each variable color set
        VariableManager.raceColors = new Color[3][];
        RaceColor[][] allSets = { this.variableColor1, this.variableColor2, this.variableColor3 };
        for (int i = 0; i < VariableManager.raceColors.Length; i++)
        {
            List<RaceColor> options = new List<RaceColor>(allSets[i]);
            raceColors[i] = new Color[options.Count];

            for (int j = 0; options.Count > 0; ++j)
            {
                int selection = Random.Range(0, options.Count);
                raceColors[i][j] = VariableManager.raceColorValues[(int)options[selection]];
                options.RemoveAt(selection);
            }
        }
    }
}
