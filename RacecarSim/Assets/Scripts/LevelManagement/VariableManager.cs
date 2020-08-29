using System.Collections.Generic;
using UnityEngine;

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

    public enum RaceTurn
    {
        None,
        Left,
        Right
    }

    public static RaceTurn GetVariableTurn(int turnIndex)
    {
        return VariableManager.raceTurns[turnIndex];
    }

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

    private static RaceTurn[] raceTurns;

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

















    //public class TimeInfo
    //{
    //    public float startTime;
    //    public float finishTime;
    //    public float[] checkpointTimes = new float[3];
    //    public float penalty = 0;

    //    public override string ToString()
    //    {
    //        string output = $"Start Time: {startTime}\nFinish Time: {finishTime}";
    //        for (int i = 0; i < checkpointTimes.Length; i++)
    //        {
    //            output += $"\nCheckpoint {i} time: {checkpointTimes[i]}";
    //        }

    //        return output;
    //    }
    //}

    //private static readonly Color[] colors =
    //{
    //    Color.white,
    //    Color.red,
    //    new Color(0, 0.5f, 1),
    //    Color.green, 
    //    new Color(1, 0.5f, 0),
    //    new Color(0.5f, 0, 1)
    //};

    //public enum VariableColor
    //{
    //    None,
    //    Left,
    //    Center,
    //    Right,
    //    Fast,
    //    Slow
    //}

    //public enum VariableTurn
    //{
    //    None,
    //    Left,
    //    Right
    //}

    //public enum KeyTime
    //{
    //    Start,
    //    Finish,
    //    Checkpoint1,
    //    Checkpoint2,
    //    Checkpoint3
    //}

    //public static VariableTurn[] TurnChoices { get; private set; } = new VariableTurn[2];

    //private static System.Tuple<Vector3, Quaternion>[] checkpoints = new System.Tuple<Vector3, Quaternion>[3];

    //public static Color GetColor(VariableColor color)
    //{
    //    return VariableManager.colors[colorAssignments[color.GetHashCode()]];
    //}

    //public static void SetCheckpoint(int index, System.Tuple<Vector3, Quaternion> info)
    //{
    //    checkpoints[index] = info;
    //}

    //public static System.Tuple<Vector3, Quaternion> GetCheckpoint(int index)
    //{
    //    return checkpoints[index];
    //}

    //public static void SetKeyTime(KeyTime keyTime, float time)
    //{
    //    time += timeInfo.penalty;
    //    switch (keyTime)
    //    {
    //        case KeyTime.Start:
    //            timeInfo.startTime = time;
    //            break;

    //        case KeyTime.Finish:
    //            timeInfo.finishTime = time;
    //            break;

    //        default:
    //            timeInfo.checkpointTimes[keyTime.GetHashCode() - KeyTime.Checkpoint1.GetHashCode()] = time;
    //            break;
    //    }
    //}

    //public static void AddPenalty(float time)
    //{
    //    timeInfo.penalty += time;
    //}

    //private static int[] colorAssignments;

    //private static TimeInfo timeInfo;

    //private static Hud hud;

    //private static GameObject racecar;

    //private void Awake()
    //{
    //    timeInfo = new TimeInfo();
    //    hud = FindObjectOfType<Hud>();

    //    VariableManager.TurnChoices[0] = Random.value < 0.5f ? VariableTurn.Left : VariableTurn.Right;
    //    VariableManager.TurnChoices[1] = Random.value < 0.5f ? VariableTurn.Left : VariableTurn.Right;

    //    VariableManager.colorAssignments = new int[]{ 0, 1, 2, 3, 4, 5 };
    //    if (Random.value < 0.5f)
    //    {
    //        colorAssignments[4] = 5;
    //        colorAssignments[5] = 4;
    //    }

    //    for (int i = 1; i < 4; i++)
    //    {
    //        int swapIndex = Random.Range(1, 4);

    //        int temp = colorAssignments[i];
    //        colorAssignments[i] = colorAssignments[swapIndex];
    //        colorAssignments[swapIndex] = temp;
    //    }
    //}

    //private void Start()
    //{
    //    GameObject[] racecars = GameObject.FindGameObjectsWithTag("Player");
    //    foreach (GameObject car in racecars)
    //    {
    //        if (car.GetComponent<Racecar>().Hud != null)
    //        {
    //            VariableManager.racecar = car;
    //            break;
    //        }
    //    }
    //}

    //private void Update()
    //{
    //    if (hud != null)
    //    {
    //        hud.UpdateTimes(timeInfo);
    //    }

    //    for (int i = 0; i < 3; i++)
    //    {
    //        if (Input.GetKeyDown(KeyCode.Alpha1 + i))
    //        {
    //            racecar.transform.position = checkpoints[i].Item1;
    //            racecar.transform.rotation = checkpoints[i].Item2;
    //            racecar.transform.position += Vector3.up;
    //            break;
    //        }
    //    }
    //}
}
