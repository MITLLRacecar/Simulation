using UnityEngine;

/// <summary>
/// A distance cone which can be moved and resized by user input.
/// </summary>
public class InteractableCone : DistanceCone
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
    /// The maximum distance the cone can be placed from the camera (in dm).
    /// </summary>
    private const int maxDistance = 200;

    /// <summary>
    /// The rate at which the cone resizes when the mouse scroll wheel is turned.
    /// </summary>
    private const float scrollSpeed = 0.1f;
    #endregion

    protected override void Update()
    {
        base.Update();

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
            this.Scale += Input.mouseScrollDelta[1] * InteractableCone.scrollSpeed;
        }
    }
}
