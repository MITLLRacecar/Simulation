using UnityEngine;

/// <summary>
/// Controls an AR tag which can be modified by the user.
/// </summary>
public class ArTagToggle : MonoBehaviour
{
    #region Set in Unity Editor
    /// <summary>
    /// The AR tags which can be displayed on this object.
    /// </summary>
    [SerializeField]
    private Material[] tags;

    /// <summary>
    /// The index of the AR tag to initially render.
    /// </summary>
    [SerializeField]
    private int tagIndex = 0;
    #endregion

    #region Constants
    /// <summary>
    /// The possible colors of the AR tag.
    /// </summary>
    private static readonly Color[] colors = new Color[] { Color.white, Color.red, new Color(0, 0.5f, 1), Color.green };

    /// <summary>
    /// The scale applied to the mouse scroll speed when rotating the AR tag.
    /// </summary>
    private const float scrollScale = 10;

    /// <summary>
    /// The color of the padding of a selected AR tag.
    /// </summary>
    private static readonly Color selectedColor = new Color(0.75f, 0.75f, 0.75f);
    #endregion

    private enum Renderers
    {
        Outline,
        Tag,
        Padding
    }

    /// <summary>
    /// The renders of the objects in the AR tag.
    /// </summary>
    private Renderer[] renderers;

    /// <summary>
    /// The index of the current color shown.
    /// </summary>
    private int colorIndex = 0;

    /// <summary>
    /// True if the AR tag is currently selected by the user.
    /// </summary>
    private bool selected = false;

    private void Awake()
    {
        this.renderers = this.GetComponentsInChildren<Renderer>();
    }

    private void Start()
    {
        this.renderers[(int)Renderers.Tag].material = tags[tagIndex];
        this.renderers[(int)Renderers.Padding].material.SetColor("_Color", Color.white);
    }

    private void Update()
    {
        bool leftMouse = Input.GetMouseButtonDown(0);
        bool rightMouse = Input.GetMouseButtonDown(1);
        bool tagClicked = (leftMouse || rightMouse) &&
            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit) &&
            hit.collider.gameObject == this.gameObject;

        if (this.selected)
        {
            if (tagClicked)
            {
                if (leftMouse)
                {
                    // Toggle the displayed tag when left-clicked
                    this.tagIndex += tags.Length + 1;
                    this.tagIndex %= tags.Length;
                    this.renderers[(int)Renderers.Tag].material = tags[tagIndex];
                }
                if (rightMouse)
                {
                    // Toggle the color when right-clicked
                    this.colorIndex++;
                    this.renderers[(int)Renderers.Outline].material.SetColor("_Color", colors[colorIndex % colors.Length]);
                }
            }
            else if (leftMouse || rightMouse)
            {
                // Deselect the tag if we click off it
                this.selected = false;
                this.renderers[(int)Renderers.Padding].material.SetColor("_Color", Color.white);
            }

            // Rotate the tag based on scroll amount
            this.transform.Rotate(0, 0, ArTagToggle.scrollScale * Input.mouseScrollDelta[1]);
        }
        else if (tagClicked)
        {
            // Select the tag when left-clicked
            this.selected = true;
            this.renderers[(int)Renderers.Padding].material.SetColor("_Color", ArTagToggle.selectedColor);
        }
    }
}
