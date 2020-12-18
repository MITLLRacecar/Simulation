using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A cone which can be moved and resized by user input.
/// </summary>
public class InteractableCone : MonoBehaviour
{
    #region Set in Unity Editor
    /// <summary>
    /// If true, the cone can be resized by scrolling with the mouse.
    /// </summary>
    [SerializeField]
    private bool Resizable = false;

    /// <summary>
    /// If true, clicking on the screen will place the cone at that location.
    /// </summary>
    [SerializeField]
    private bool ClickToPlace = true;
    #endregion

    #region Constants
    /// <summary>
    /// The maximum click distance the cone can be placed from the camera (in dm).
    /// </summary>
    private const int maxDistance = 200;

    /// <summary>
    /// The smallest the cone can become.
    /// </summary>
    private const float minScale = 0.25f;

    /// <summary>
    /// The largest the cone can become.
    /// </summary>
    private const float maxScale = 4;

    /// <summary>
    /// The rate at which the cone resizes when the mouse scroll wheel is turned.
    /// </summary>
    private const float scrollSpeed = 0.1f;
    #endregion

    /// <summary>
    /// The current size of the cone.
    /// </summary>
    private float curScale = 1.0f;

    /// <summary>
    /// The canvas used to display the cone distance.
    /// </summary>
    private Canvas canvas;

    /// <summary>
    /// The text displaying the cone distance.
    /// </summary>
    private Text text;

    /// <summary>
    /// The radius of the cone (in dm)
    /// </summary>
    private float Radius
    {
        get
        {
            return 0.7f * this.curScale - 0.15f;
        }
    }

    /// <summary>
    /// The distance between the cone and the car in cm.
    /// </summary>
    private float Distance
    {
        get
        {
            return Mathf.Max(0, 10 * (Vector3.Distance(this.transform.position, LevelManager.GetCar().transform.position) - Racecar.radius - this.Radius));
        }
    }

    private void Awake()
    {
        this.canvas = this.GetComponentInChildren<Canvas>();
        this.text = this.GetComponentInChildren<Text>();
    }

    private void Update()
    {
        // Place the cone at the position clicked on screen
        if (this.ClickToPlace && Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit raycastHit, InteractableCone.maxDistance))
            {
                this.transform.position = raycastHit.point;
                this.transform.rotation = Quaternion.identity;
            }
        }

        // Resize the cone with the scroll wheel
        if (this.Resizable && Input.mouseScrollDelta[1] != 0)
        {
            this.curScale = Mathf.Clamp(this.curScale + Input.mouseScrollDelta[1] * InteractableCone.scrollSpeed, InteractableCone.minScale, InteractableCone.maxScale);
            this.transform.localScale = Vector3.one * curScale;
        }

        // Update UI
        this.text.text = $"{this.Distance:F1} cm";
        this.canvas.transform.LookAt(Camera.main.transform.position);
    }
}
