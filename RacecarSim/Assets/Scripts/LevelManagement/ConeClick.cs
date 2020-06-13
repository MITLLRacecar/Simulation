using UnityEngine;

/// <summary>
/// Moves the gameobject to the point clicked on screen.
/// </summary>
public class ConeClick : MonoBehaviour
{
    /// <summary>
    /// The maximum click distance from the camera (in dm).
    /// </summary>
    private const int maxDistance = 200;
    
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit raycastHit, ConeClick.maxDistance))
            {
                this.transform.position = raycastHit.point;
            }
        }
    }
}
