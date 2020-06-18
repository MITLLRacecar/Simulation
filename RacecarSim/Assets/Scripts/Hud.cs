using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the heads-up display shown to the user.
/// </summary>
public class Hud : MonoBehaviour
{
    #region Set in Unity Editor
    /// <summary>
    /// The textbox shown when the user fails an objective.
    /// </summary>
    [SerializeField]
    private GameObject FailureMessage;

    /// <summary>
    /// The textbox shown when the user successfully completes a lab.
    /// </summary>
    [SerializeField]
    private GameObject SuccessMessage;
    #endregion

    #region Constants
    /// <summary>
    /// The color used for the background of sensor visualizations.
    /// </summary>
    public static readonly Color SensorBackgroundColor = new Color(0.2f, 0.2f, 0.2f);

    /// <summary>
    /// The number of times the LIDAR map is smaller than the color and depth visualizations.
    /// </summary>
    private const int lidarMapScale = 4;

    /// <summary>
    /// The background color of the mode label when the car is in default drive mode.
    /// </summary>
    private static readonly Color defaultDriveColor = new Color(0, 0.5f, 1);

    /// <summary>
    /// The background color of the mode label when the car is in user program mode.
    /// </summary>
    private static readonly Color userProgramColor = new Color(0.75f, 0, 0.25f);

    /// <summary>
    /// The factor that time slows down during time warp.
    /// </summary>
    private const float timeWarpScale = 0.1f;
    #endregion

    #region Public Interface
    /// <summary>
    /// Sets the message text at the bottom of the screen.
    /// </summary>
    /// <param name="message">The text to show.</param>
    public void SetMessage(string message, float persistTime = -1, float fadeTime = 1.0f)
    {
        this.texts[Texts.Message.GetHashCode()].text = message;
        this.messageCounter = 0;
        this.messagePersistTime = persistTime;
        this.messageFadeTime = fadeTime;
    }

    /// <summary>
    /// Updates the icon showing the current driving mode.
    /// </summary>
    /// <param name="isDefaultDrive">Whether the car is currently in default drive mode.</param>
    public void UpdateMode(bool isDefaultDrive, bool isValid)
    {
        if (isDefaultDrive)
        {
            this.texts[Texts.Mode.GetHashCode()].text = "Default Drive";
            this.images[Images.ModeBackground.GetHashCode()].color = Hud.defaultDriveColor;
        }
        else
        {
            this.texts[Texts.Mode.GetHashCode()].text = "User Program";
            this.images[Images.ModeBackground.GetHashCode()].color = Hud.userProgramColor;
        }
        this.images[Images.Star.GetHashCode()].enabled = isValid;
        print(isValid);
    }

    /// <summary>
    /// Show the failure textbox with the provided message.
    /// </summary>
    /// <param name="text">The reason the user failed.</param>
    public void ShowFailureMessage(string text)
    {
        this.FailureMessage.SetActive(true);
        this.texts[Texts.Failure.GetHashCode()].text = text;
    }

    /// <summary>
    /// Show the success textbox with the provided completion time.
    /// </summary>
    /// <param name="time">The time (in seconds) the player took to complete the lab.</param>
    public void ShowSuccessMessage(float time)
    {
        this.SuccessMessage.SetActive(true);
        this.texts[Texts.SuccessTime.GetHashCode()].text = $"Time: {time:F2} seconds";
    }
    #endregion

    /// <summary>
    /// The mutable text fields of the HUD, with values corresponding to the index in texts.
    /// </summary>
    private enum Texts
    {
        TrueSpeed = 4,
        LinearAcceleration = 8,
        AngularVelocity = 11,
        Mode = 13,
        Message = 14,
        Failure = 16,
        SuccessTime = 19
    }

    /// <summary>
    /// The mutable images of the HUD, with values corresponding to the index in images.
    /// </summary>
    private enum Images
    {
        TimeWarp = 0,
        ColorFeed = 3,
        DepthFeed = 5,
        LidarMap = 7,
        ModeBackground = 9,
        Star = 10,
        ControllerFirstButton = 12
    }

    /// <summary>
    /// All text fields contained in the HUD.
    /// </summary>
    private Text[] texts;

    /// <summary>
    /// All images contained in the HUD.
    /// </summary>
    private RawImage[] images;

    /// <summary>
    /// The RACECAR for which the HUD displays information.
    /// </summary>
    private Racecar racecar;

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
    /// The current factor at which time progresses.
    /// </summary>
    private float curTimeScale;

    /// <summary>
    /// The default value of Time.fixedDeltaTime.
    /// </summary>
    private float defaultFixedDeltaTime;

    private void Awake()
    {
        // Find components
        this.texts = GetComponentsInChildren<Text>();
        this.images = this.GetComponentsInChildren<RawImage>();

        this.images[Images.LidarMap.GetHashCode()].texture = new Texture2D(CameraModule.ColorWidth / Hud.lidarMapScale, CameraModule.ColorHeight / Hud.lidarMapScale);
        this.images[Images.DepthFeed.GetHashCode()].texture = new Texture2D(CameraModule.DepthWidth, CameraModule.DepthHeight);

        this.racecar = this.transform.parent.GetComponentInChildren<Racecar>();
    }

    private void Start()
    {
        this.messagePersistTime = -1;
        this.curTimeScale = 1.0f;
        this.defaultFixedDeltaTime = Time.fixedDeltaTime;
        this.images[Images.TimeWarp.GetHashCode()].enabled = false;
        this.FailureMessage.SetActive(false);
        this.SuccessMessage.SetActive(false);
    }

    private void Update()
    {
        // Update mutable texts and images
        this.texts[Texts.TrueSpeed.GetHashCode()].text = this.racecar.Physics.LinearVelocity.magnitude.ToString("F2");
        this.texts[Texts.LinearAcceleration.GetHashCode()].text = FormatVector3(this.racecar.Physics.LinearAccceleration);
        this.texts[Texts.AngularVelocity.GetHashCode()].text = FormatVector3(this.racecar.Physics.AngularVelocity);

        this.racecar.Lidar.VisualizeLidar((Texture2D)this.images[Images.LidarMap.GetHashCode()].texture);
        this.racecar.Camera.VisualizeDepth((Texture2D)this.images[Images.DepthFeed.GetHashCode()].texture);

        this.UpdateController();

        // Toggle time warp when the backspace alt key is pressed
        if (Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt))
        {
            this.ToggleTimeWarp();
        }

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
            this.texts[Texts.Message.GetHashCode()].color = Color.Lerp(Color.white, Color.clear, this.messageCounter / this.messageFadeTime);
        }
    }

    /// <summary>
    /// Update the controller icon to show the current buttons, triggers, and joysticks being pressed.
    /// </summary>
    private void UpdateController()
    {
        // Update buttons
        int index = Images.ControllerFirstButton.GetHashCode();
        foreach (Controller.Button button in Enum.GetValues(typeof(Controller.Button)))
        {
            this.images[index].enabled = this.racecar.Controller.IsDown(button);
            index++;
        }

        // Update triggers
        foreach (Controller.Trigger trigger in Enum.GetValues(typeof(Controller.Trigger)))
        {
            this.images[index].enabled = this.racecar.Controller.GetTrigger(trigger) > 0;
            index++;
        }

        // Update joysticks
        foreach (Controller.Joystick joystick in Enum.GetValues(typeof(Controller.Joystick)))
        {
            Vector2 values = this.racecar.Controller.GetJoystick(joystick);
            this.images[index].enabled = values.x * values.x + values.y * values.y > 0;
            index++;
        }
    }

    /// <summary>
    /// Turns time warp on or off and updates the HUD accordingly.
    /// </summary>
    private void ToggleTimeWarp()
    {
        this.images[Images.TimeWarp.GetHashCode()].enabled = !this.images[Images.TimeWarp.GetHashCode()].enabled;

        this.curTimeScale = this.curTimeScale < 1.0f ? 1.0f : Hud.timeWarpScale;
        Time.timeScale = this.curTimeScale;
        Time.fixedDeltaTime = this.defaultFixedDeltaTime * this.curTimeScale;
    }

    /// <summary>
    /// Formats a vector with a constant as a string with a constant width.
    /// </summary>
    /// <param name="vector">The vector to format.</param>
    /// <returns>The vector formatted as a string with exactly 19 characters.</returns>
    private string FormatVector3(Vector3 vector)
    {
        return $"({FormatFloat(vector.x)},{FormatFloat(vector.y)},{FormatFloat(vector.z)})";       
    }

    /// <summary>
    /// Rounds and formats a float as a string with a constant width.
    /// </summary>
    /// <param name="value">A value less than 10.</param>
    /// <returns>The provided value formatted as a string with exactly five characters.</returns>
    private string FormatFloat(float value)
    {
        string str = value.ToString("F2");

        // Add a leading space if there is no negative sign
        if (str[0] != '-')
        {
            return $" {str}";
        }

        return str;
    }
}
