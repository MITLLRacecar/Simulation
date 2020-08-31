using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The base class from which all in-simulation HUDs and screen managers inherit.
/// </summary>
public abstract class ScreenManager : MonoBehaviour
{
    #region Public Interface
    /// <summary>
    /// Show a text message to the user.
    /// </summary>
    /// <param name="message">The text to show.</param>
    /// <param name="color">The color of the text.</param>
    /// <param name="persistTime">The time in seconds the text is shown.</param>
    /// <param name="fadeTime">In the time in seconds it takes for the text to fade out after persistTime has passed.</param>
    public void ShowMessage(string message, Color color, float persistTime = -1, float fadeTime = 1.0f)
    {
        this.texts[0].text = message;
        this.texts[0].color = color;

        this.messageColor = color;
        this.messageCounter = 0;
        this.messagePersistTime = persistTime;
        this.messageFadeTime = fadeTime;
    }

    /// <summary>
    /// Updates the element showing the current simulation mode.
    /// </summary>
    /// <param name="mode">The current mode of the simulation.</param>
    public abstract void UpdateMode(SimulationMode mode);

    public abstract void UpdateTimeScale(float timeScale);

    public abstract void UpdateTimes(float[][] times);

    public abstract void HandleWin(float[][] times);

    public abstract void HandleFailure(int carIndex, string reason);
    #endregion

    /// <summary>
    /// All text fields contained in the HUD.
    /// </summary>
    protected Text[] texts;

    /// <summary>
    /// All images contained in the HUD.
    /// </summary>
    protected RawImage[] images;

    protected virtual void Awake()
    {
        this.texts = GetComponentsInChildren<Text>();
        this.images = this.GetComponentsInChildren<RawImage>();

        this.messagePersistTime = -1;
    }

    protected virtual void Update()
    {
        // Handle message persistence and fadeout
        if (this.messagePersistTime > 0)
        {
            this.messageCounter += Time.deltaTime;
            if (this.messageCounter > this.messagePersistTime)
            {
                this.messagePersistTime = 0;
                this.messageCounter = 0;
            }
        }
        else if (this.messagePersistTime == 0 && this.messageCounter < this.messageFadeTime)
        {
            this.messageCounter += Time.deltaTime;
            this.texts[0].color = Color.Lerp(this.messageColor, Color.clear, this.messageCounter / this.messageFadeTime);
        }
    }

    /// <summary>
    /// A counter used to track message persistence and fade out.
    /// </summary>
    private float messageCounter;

    /// <summary>
    /// The time is seconds that the current message will persist.  If -1, the current message will persist indefinitely.
    /// </summary>
    private float messagePersistTime;

    /// <summary>
    /// The time in seconds that the current message will take to fade out.
    /// </summary>
    private float messageFadeTime;

    /// <summary>
    /// The color of the current message.
    /// </summary>
    private Color messageColor;
}
