using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Moves the gameobject to the point clicked on screen.
/// </summary>
public class ConeClick : MonoBehaviour
{
    #region Set in Unity Editor
    /// <summary>
    /// If true, the cone can be resized by scrolling with the mouse.
    /// </summary>
    [SerializeField]
    private bool Resizable = false;
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
    /// The racecar from which the cone measures distance.
    /// </summary>
    private GameObject player;

    /// <summary>
    /// The distance between the cone and the car in cm.
    /// </summary>
    private float Distance
    {
        get
        {
            return Vector3.Distance(this.transform.position, this.player.transform.position) * 10 
                - 16                  // Account for car radius
                - 7 * this.curScale;  // Account for cone radius
        }
    }

    private void Awake()
    {
        this.canvas = this.GetComponentInChildren<Canvas>();
        this.text = this.GetComponentInChildren<Text>();
        this.player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        // Place the cone at the position clicked on screen
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit raycastHit, ConeClick.maxDistance))
            {
                this.transform.position = raycastHit.point;
                this.transform.rotation = Quaternion.identity;
            }
        }

        // Resize the cone with the scroll wheel
        if (this.Resizable && Input.mouseScrollDelta[1] != 0)
        {
            this.curScale = Mathf.Clamp(this.curScale + Input.mouseScrollDelta[1] * ConeClick.scrollSpeed, ConeClick.minScale, ConeClick.maxScale);
            this.transform.localScale = Vector3.one * curScale;
        }

        // Update UI
        this.text.text = $"{this.Distance:F1} cm";
        this.canvas.transform.LookAt(Camera.main.transform.position);
    }
}
