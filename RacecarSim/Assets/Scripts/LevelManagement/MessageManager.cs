using UnityEngine;

/// <summary>
/// Manages the message shown on the HUD
/// </summary>
public class MessageManager : MonoBehaviour
{
    #region Set in Unity Editor
    [SerializeField]
    private string Text = string.Empty;
    #endregion

    /// <summary>
    /// True if the message is currently being shown.
    /// </summary>
    private bool isActive;

    /// <summary>
    /// The Hud on which to display the message.
    /// </summary>
    private Hud hud;

    private void Awake()
    {
        this.hud = this.GetComponent<Hud>();
    }

    private void Start()
    {
        this.isActive = this.Text != string.Empty;
        this.hud.SetMessage(this.Text, Color.white);
    }

    // Update is called once per frame
    void Update()
    {
        if (this.isActive)
        {
            if (Input.GetMouseButtonDown(0))
            {
                this.hud.SetMessage(this.Text, Color.white, 0.5f);
                this.isActive = false;
            }
        }
    }
}
