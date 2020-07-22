using System.Runtime.CompilerServices;
using UnityEngine;

public class VariableManager : MonoBehaviour
{
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

    public static VariableTurn[] TurnChoices { get; private set; } = new VariableTurn[2];

    public static Color GetColor(VariableColor color)
    {
        return VariableManager.colors[colorAssignments[color.GetHashCode()]];
    }

    private static int[] colorAssignments;

    void Awake()
    {
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
    }
}
