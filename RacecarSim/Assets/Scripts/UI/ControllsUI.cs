using UnityEngine;

/// <summary>
/// Manages the controls pane of the Main Menu.
/// </summary>
public class ControllsUI : MonoBehaviour
{
    private void Update()
    {
        if (Input.anyKeyDown)
        {
            this.gameObject.SetActive(false);
        }
    }
}
