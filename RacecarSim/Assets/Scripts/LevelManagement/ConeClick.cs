using UnityEngine;

/// <summary>
/// Moves the gameobject to the point clicked on screen.
/// </summary>
public class ConeClick : MonoBehaviour
{
    #region Set in Unity Editor
    /// <summary>
    /// If true, the cone can be resized by scrolling with the mouse.
    /// </summary>
    public bool Resizable = false;
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
    
    private void Update()
    {
        // Place the cone at the position clicked on screen
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit raycastHit, ConeClick.maxDistance))
            {
                this.transform.position = raycastHit.point;
            }
        }

        // Resize the cone with the scroll wheel
        if (this.Resizable && Input.mouseScrollDelta[1] != 0)
        {
            this.curScale = Mathf.Clamp(this.curScale + Input.mouseScrollDelta[1] * ConeClick.scrollSpeed, ConeClick.minScale, ConeClick.maxScale);
            this.transform.localScale = Vector3.one * curScale;
        }
    }
}
