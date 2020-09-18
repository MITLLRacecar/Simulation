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
        this.texts[this.messageTextIndex].text = message;
        this.texts[this.messageTextIndex].color = color;

        this.messageColor = color;
        this.messageCounter = 0;
        this.messagePersistTime = persistTime;
        this.messageFadeTime = fadeTime;
    }

    /// <summary>
    /// Updates the element showing the current time elapsed in the race.
    /// </summary>
    /// <param name="mainTime">The overall time (in seconds) that the current level has been running.</param>
    public void UpdateTime(float mainTime)
    {
        this.texts[this.mainTimeTextIndex].text = mainTime.ToString("F3");
    }

    #region Abstract
    /// <summary>
    /// Update the element(s) indicating that the race is won.
    /// </summary>
    /// <param name="times">The finish time of each car, indexed by car.</param>
    public abstract void HandleWin(float[] times);

    /// <summary>
    /// Update the element(s) indicating that a car failed a critical objective.
    /// </summary>
    /// <param name="carIndex">The index of the car which failed the objective.</param>
    /// <param name="reason">A description of the failure.</param>
    public abstract void HandleFailure(int carIndex, string reason);

    /// <summary>
    /// Update the element(s) indicating how many Python programs are connected to RacecarSim.
    /// </summary>
    /// <param name="numConnectedPrograms"></param>
    public abstract void UpdateConnectedPrograms(int numConnectedPrograms);

    /// <summary>
    /// Updates the element(s) showing the current simulation mode.
    /// </summary>
    /// <param name="mode">The current mode of the simulation.</param>
    public abstract void UpdateMode(SimulationMode mode);

    /// <summary>
    /// Update the element(s) showing the time it took each car to reach each checkpoint.
    /// </summary>
    /// <param name="checkpointTimes">The time at which each car reached each checkpoint, indexed by car, then checkpoint.</param>
    public abstract void UpdateCheckpointTimes(float[,] checkpointTimes);

    /// <summary>
    /// Updates the element(s) showing the current rate at which time progresses.
    /// </summary>
    /// <param name="timeScale">The current rate at which time progresses. 1 is full speed, 0.5 is half speed, and 0 is paused.</param>
    public abstract void UpdateTimeScale(float timeScale);
    #endregion
    #endregion

    /// <summary>
    /// All text fields contained in the screen manager.
    /// </summary>
    protected Text[] texts;

    /// <summary>
    /// All images contained in the screen manager.
    /// </summary>
    protected RawImage[] images;

    /// <summary>
    /// The index of the message text in texts.
    /// </summary>
    protected int messageTextIndex = 0;

    /// <summary>
    /// The index of the main time text in texts.
    /// </summary>
    protected int mainTimeTextIndex = 1;

    protected virtual void Awake()
    {
        this.texts = GetComponentsInChildren<Text>();
        this.images = this.GetComponentsInChildren<RawImage>();

        this.messagePersistTime = -1;

        this.texts[this.messageTextIndex].text = string.Empty;
        this.texts[this.mainTimeTextIndex].text = string.Empty;
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
