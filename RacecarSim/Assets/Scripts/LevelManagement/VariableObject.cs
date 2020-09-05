using UnityEngine;

/// <summary>
/// An object which randomly varies to alter a level.
/// </summary>
public class VariableObject : MonoBehaviour
{
    /// <summary>
    /// The variable color set of the race which determines this object's color.
    /// </summary>
    [SerializeField]
    private int colorSetIndex = -1;

    /// <summary>
    /// The index of this object within its color set.
    /// </summary>
    [SerializeField]
    private int colorIndex = -1;

    /// <summary>
    /// The index of the variable turn in the race which determines this object's behavior.
    /// </summary>
    [SerializeField]
    private int turnIndex = -1;

    /// <summary>
    /// The turn direction which this object blocks.
    /// </summary>
    [SerializeField]
    private VariableManager.RaceTurn turnDirection = VariableManager.RaceTurn.None;

    /// <summary>
    /// Set the color of this object to the variable color chosen for the specified set and index.
    /// </summary>
    [SerializeField]
    private bool setColor;

    /// <summary>
    /// Set the color of this object's children to the variable color chosen for the specified set and index.
    /// </summary>
    [SerializeField]
    private bool setColorInChild;

    /// <summary>
    /// Rotate the object 90 degrees to the left or right depending on the variable turn chosen for the specified turn index.
    /// </summary>
    [SerializeField]
    private bool setRotation;

    /// <summary>
    /// Remove the object if the variable turn at the specified index turns in the direction of this object.
    /// </summary>
    [SerializeField]
    private bool remove;

    void Start()
    {
        if (this.setColor)
        {
            this.GetComponent<Renderer>().material.color = VariableManager.GetVariableColor(this.colorSetIndex, this.colorIndex);
        }

        if (this.setColorInChild)
        {
            this.GetComponentInChildren<Renderer>().material.color = VariableManager.GetVariableColor(this.colorSetIndex, this.colorIndex);
        }

        if (this.setRotation)
        {
            VariableManager.RaceTurn turnDirection = VariableManager.GetVariableTurn(this.turnIndex);
            switch (turnDirection)
            {
                case VariableManager.RaceTurn.Left:
                    this.transform.Rotate(0, 0, 90);
                    break;
                case VariableManager.RaceTurn.Right:
                    this.transform.Rotate(0, 0, -90);
                    break;
            }
        }

        if (this.remove)
        {
            if (VariableManager.GetVariableTurn(this.turnIndex) == this.turnDirection)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
