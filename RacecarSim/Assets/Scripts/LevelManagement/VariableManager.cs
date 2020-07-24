using System.Data;
using UnityEngine;

public class VariableManager : MonoBehaviour
{
    public class TimeInfo
    {
        public float startTime;
        public float finishTime;
        public float[] checkpointTimes = new float[3];

        public override string ToString()
        {
            string output = $"Start Time: {startTime}\nFinish Time: {finishTime}";
            for (int i = 0; i < checkpointTimes.Length; i++)
            {
                output += $"\nCheckpoint {i} time: {checkpointTimes[i]}";
            }

            return output;
        }
    }

    private static readonly Color[] colors =
    {
        Color.white,
        Color.red,
        new Color(0, 0.5f, 1),
        Color.green, 
        new Color(1, 0.5f, 0),
        new Color(0.5f, 0, 1)
    };

    public enum VariableColor
    {
        None,
        Left,
        Center,
        Right,
        Fast,
        Slow
    }

    public enum VariableTurn
    {
        None,
        Left,
        Right
    }

    public enum KeyTime
    {
        Start,
        Finish,
        Checkpoint1,
        Checkpoint2,
        Checkpoint3
    }

    public static VariableTurn[] TurnChoices { get; private set; } = new VariableTurn[2];

    private System.Tuple<Vector3, Quaternion>[] checkpoints = new System.Tuple<Vector3, Quaternion>[3];

    public static Color GetColor(VariableColor color)
    {
        return VariableManager.colors[colorAssignments[color.GetHashCode()]];
    }

    public void SetCheckpoint(int index, System.Tuple<Vector3, Quaternion> info)
    {
        this.checkpoints[index] = info;
    }

    public System.Tuple<Vector3, Quaternion> GetCheckpoint(int index)
    {
        return this.checkpoints[index];
    }

    public void SetKeyTime(KeyTime keyTime, float time)
    {
        switch (keyTime)
        {
            case KeyTime.Start:
                this.timeInfo.startTime = time;
                break;

            case KeyTime.Finish:
                this.timeInfo.finishTime = time;
                break;

            default:
                this.timeInfo.checkpointTimes[keyTime.GetHashCode() - KeyTime.Checkpoint1.GetHashCode()] = time;
                break;
        }
    }

    private static int[] colorAssignments;

    private TimeInfo timeInfo = new TimeInfo();

    private Hud hud;

    private GameObject racecar;

    private void Awake()
    {
        this.hud = FindObjectOfType<Hud>();

        VariableManager.TurnChoices[0] = Random.value < 0.5f ? VariableTurn.Left : VariableTurn.Right;
        VariableManager.TurnChoices[1] = Random.value < 0.5f ? VariableTurn.Left : VariableTurn.Right;

        VariableManager.colorAssignments = new int[]{ 0, 1, 2, 3, 4, 5 };
        if (Random.value < 0.5f)
        {
            colorAssignments[4] = 5;
            colorAssignments[5] = 4;
        }

        for (int i = 1; i < 4; i++)
        {
            int swapIndex = Random.Range(1, 4);

            int temp = colorAssignments[i];
            colorAssignments[i] = colorAssignments[swapIndex];
            colorAssignments[swapIndex] = temp;
        }

        this.racecar = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        this.hud.UpdateTimes(this.timeInfo);

        for (int i = 0; i < 3; ++i)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                this.racecar.transform.position = this.checkpoints[i].Item1;
                this.racecar.transform.rotation = this.checkpoints[i].Item2;
            }
        }
    }
}
