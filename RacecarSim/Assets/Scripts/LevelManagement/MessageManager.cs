using UnityEngine;

public class MessageManager : MonoBehaviour
{
    #region Set in Unity Editor
    public string Text = string.Empty;
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
        this.hud.SetMessage(this.Text);
    }

    // Update is called once per frame
    void Update()
    {
        if (this.isActive)
        {
            if (Input.GetMouseButtonDown(0))
            {
                this.hud.SetMessage(this.Text, 0.5f);
                this.isActive = false;
            }
        }
    }
}
