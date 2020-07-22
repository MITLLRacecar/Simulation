using UnityEngine;

public class VariableObject : MonoBehaviour
{
    [SerializeField]
    private VariableManager.VariableColor color;

    [SerializeField]
    private VariableManager.VariableTurn turnDirection;

    [SerializeField]
    private int turnIndex;

    [SerializeField]
    private bool setColor;

    [SerializeField]
    private bool setColorInChild;

    [SerializeField]
    private bool setRotation;

    [SerializeField]
    private bool remove;

    void Start()
    {
        if (this.color != VariableManager.VariableColor.None)
        {
            Color rgbColor = VariableManager.GetColor(this.color);
            if (this.setColor)
            {
                this.GetComponent<Renderer>().material.color = rgbColor;
            }

            if (this.setColorInChild)
            {
                this.GetComponentInChildren<Renderer>().material.color = rgbColor;
            }
        }

        if (this.setRotation)
        {
            if (VariableManager.TurnChoices[this.turnIndex] == VariableManager.VariableTurn.Left)
            {
                this.transform.Rotate(0, 0, 90);
            }
            else
            {
                this.transform.Rotate(0, 0, -90);
            }
        }

        if (this.remove)
        {
            if (VariableManager.TurnChoices[this.turnIndex] == this.turnDirection)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
