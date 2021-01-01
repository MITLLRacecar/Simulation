using UnityEngine;

/// <summary>
/// Controls an AR marker which can be modified by the user.
/// </summary>
public class ArMarkerToggle : MonoBehaviour
{
    #region Set in Unity Editor
    /// <summary>
    /// The AR marker patterns which can be displayed on this object.
    /// </summary>
    [SerializeField]
    private Material[] patterns;

    /// <summary>
    /// The index of the pattern to initially render.
    /// </summary>
    [SerializeField]
    private int initialPattern = 0;

    /// <summary>
    /// The index of the color to initially render.
    /// </summary>
    [SerializeField]
    private int initialColor = 0;
    #endregion

    #region Constants
    /// <summary>
    /// The possible colors of the AR marker.
    /// </summary>
    private static readonly Color[] colors = new Color[] { Color.white, Color.red, new Color(0, 0.5f, 1), Color.green };

    /// <summary>
    /// The scale applied to the mouse scroll speed when rotating the AR marker.
    /// </summary>
    private const float scrollScale = 10;

    /// <summary>
    /// The color of the padding of a selected AR marker.
    /// </summary>
    private static readonly Color selectedColor = new Color(0.75f, 0.75f, 0.75f);
    #endregion

    /// <summary>
    /// The renderers in the ARMarker object, with values corresponding to the index in renderers.
    /// </summary>
    private enum Renderers
    {
        Outline,
        Pattern,
        Padding
    }

    /// <summary>
    /// The renders of the objects in the AR tag.
    /// </summary>
    private Renderer[] renderers;

    /// <summary>
    /// The index of the current pattern.
    /// </summary>
    /// <remarks>Do not modify directly, use the PatternIndex accessor instead.</remarks>
    private int patternIndex;

    /// <summary>
    /// The index of the current color.
    /// </summary>
    /// <remarks>Do not modify directly, use the ColorIndex accessor instead.</remarks>
    private int colorIndex;

    /// <summary>
    /// True if the AR marker is currently selected by the user.
    /// </summary>
    /// <remarks>Do not modify directly, use the Selected accessor instead.</remarks>
    private bool selected = false;

    /// <summary>
    /// The index of the current pattern.
    /// </summary>
    private int PatternIndex
    {
        get 
        { 
            return this.patternIndex; 
        }
        set
        {
            this.patternIndex = value;
            this.renderers[(int)Renderers.Pattern].material = patterns[this.patternIndex];
        }
    }

    /// <summary>
    /// The index of the current color.
    /// </summary>
    private int ColorIndex
    {
        get
        {
            return this.colorIndex;
        }
        set
        {
            this.colorIndex = value;
            this.renderers[(int)Renderers.Outline].material.SetColor("_Color", colors[this.colorIndex]);
        }
    }
    
    /// <summary>
    /// True if the AR marker is currently selected by the user.
    /// </summary>
    private bool Selected
    {
        get
        {
            return this.selected;
        }
        set
        {
            this.selected = value;
            this.renderers[(int)Renderers.Padding].material.SetColor("_Color", this.selected ? ArMarkerToggle.selectedColor : Color.white);
        }
    }

    private void Awake()
    {
        this.renderers = this.GetComponentsInChildren<Renderer>();
    }

    private void Start()
    {
        this.PatternIndex = this.initialPattern;
        this.ColorIndex = this.initialColor;
        this.Selected = false;
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
                    // Toggle the displayed pattern when left-clicked
                    this.PatternIndex = (this.PatternIndex + 1) % this.patterns.Length;
                }
                if (rightMouse)
                {
                    // Toggle the color when right-clicked
                    this.ColorIndex = (this.ColorIndex + 1) % ArMarkerToggle.colors.Length;
                }
            }
            else if (leftMouse || rightMouse)
            {
                // Deselect the tag if we click off it
                this.Selected = false;
            }

            // Rotate the tag based on scroll amount
            this.transform.Rotate(0, 0, ArMarkerToggle.scrollScale * Input.mouseScrollDelta[1]);
        }
        else if (tagClicked)
        {
            // Select the tag when left-clicked
            this.Selected = true;
        }
    }
}
