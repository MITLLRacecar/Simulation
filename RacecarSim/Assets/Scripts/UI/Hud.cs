using System;
using UnityEngine;

/// <summary>
/// Controls the heads-up display shown to the user during races with a single car.
/// </summary>
public class Hud : ScreenManager
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
    /// The background color of the mode label when the simulation is in default drive mode.
    /// </summary>
    private static readonly Color defaultDriveColor = new Color(0, 0.75f, 0.25f);

    /// <summary>
    /// The background color of the mode label when the simulation is in user program mode.
    /// </summary>
    private static readonly Color userProgramColor = new Color(0.75f, 0, 0.25f);

    /// <summary>
    /// The background color of the mode label when the simulation is in wait mode.
    /// </summary>
    private static readonly Color waitColor = new Color(1f, 0.5f, 0);

    /// <summary>
    /// The background color of the mode label when the simulation is paused.
    /// </summary>
    private static readonly Color pausedColor = new Color(0, 0.5f, 1f);

    /// <summary>
    /// The number of times the LIDAR map is smaller than the color and depth visualizations.
    /// </summary>
    private const int lidarMapScale = 4;
    #endregion

    #region Public Interface
    #region Overrides
    public override void HandleWin(float[] times)
    {
        this.SuccessMessage.SetActive(true);
        this.texts[(int)Texts.SuccessTime].text = $"Time: {times[0]:F3} seconds";
    }

    public override void HandleFailure(int carIndex, string reason)
    {
        this.FailureMessage.SetActive(true);
        this.texts[(int)Texts.Failure].text = reason;
    }

    public override void UpdateConnectedPrograms(int numConnectedPrograms)
    {
        this.images[(int)Images.ConnectedProgram].color = numConnectedPrograms > 0 ? new Color(1, 1, 1, 1) : new Color(1, 1, 1, 0.25f);
    }

    public override void UpdateMode(SimulationMode mode)
    {
        string modeName;
        Color iconColor = Color.white;

        switch (mode)
        {
            case SimulationMode.DefaultDrive:
                modeName = "Default Drive";
                iconColor = Hud.defaultDriveColor;
                break;
            case SimulationMode.UserProgram:
                modeName = "User Program";
                iconColor = Hud.userProgramColor;
                break;
            case SimulationMode.Wait:
                modeName = "Wait";
                iconColor = Hud.waitColor;
                break;
            case SimulationMode.Paused:
                modeName = "Paused";
                iconColor = Hud.pausedColor;
                break;
            default:
                modeName = "Unrecognized";
                break;
        }

        this.texts[(int)Texts.Mode].text = modeName;
        this.images[(int)Images.ModeBackground].color = iconColor;
    }

    public override void UpdateTimeScale(float timeScale)
    {
        this.images[(int)Images.TimeWarp].color = new Color(1, 1, 1, Mathf.Max(0, 1 - Mathf.Sqrt(timeScale)));
        this.texts[(int)Texts.TimeScale].text = timeScale >= 1 ? string.Empty : $"{Mathf.Round(1 / timeScale)}x Slow Motion";
    }

    public override void UpdateTime(float mainTime)
    {
        this.texts[(int)Texts.MainTime].text = mainTime.ToString("F3");
    }

    public override void UpdateCheckpointTimes(float[,] checkpointTimes)
    {
        string checkpointsFormatted = string.Empty;
        if (checkpointTimes != null && checkpointTimes.Length > 0)
        {
            checkpointsFormatted = checkpointTimes[0, 0].ToString("F3");
            for (int i = 1; i < checkpointTimes.Length; i++)
            {
                checkpointsFormatted += checkpointTimes[0, i].ToString("F3");
            }
        }
        this.texts[(int)Texts.LapTime].text = checkpointsFormatted;
    }
    #endregion

    /// <summary>
    /// The texture containing the LIDAR visualization.
    /// </summary>
    public Texture2D LidarVisualization
    {
        get
        {
            return (Texture2D)this.images[(int)Images.LidarMap].texture;
        }
    }

    /// <summary>
    /// The texture containing the depth camera visualization.
    /// </summary>
    public Texture2D DepthVisualization
    {
        get
        {
            return (Texture2D)this.images[(int)Images.DepthFeed].texture;
        }
    }

    /// <summary>
    /// Update the physics statistics shown on the HUD.
    /// </summary>
    /// <param name="speed">The magnitude of the car's linear velocity in m/s.</param>
    /// <param name="linearAcceleration">The car's linear acceleration in m/s^2.</param>
    /// <param name="angularVelocity">The car's angular velocity in rad/s.</param>
    public void UpdatePhysics(float speed, Vector3 linearAcceleration, Vector3 angularVelocity)
    {
        this.texts[(int)Texts.TrueSpeed].text = speed.ToString("F2");
        this.texts[(int)Texts.LinearAcceleration].text = FormatVector3(linearAcceleration);
        this.texts[(int)Texts.AngularVelocity].text = FormatVector3(angularVelocity);
    }
    #endregion

    /// <summary>
    /// The mutable text fields of the HUD, with values corresponding to the index in texts.
    /// </summary>
    private enum Texts
    {
        Message = 0,
        TimeScale = 1,
        TrueSpeed = 6,
        LinearAcceleration = 10,
        AngularVelocity = 13,
        Mode = 15,
        Failure = 17,
        SuccessTime = 20,
        MainTime = 22,
        LapTime = 23
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
        ConnectedProgram = 10,
        ControllerFirstButton = 12
    }

    protected override void Awake()
    {
        base.Awake();

        this.images[(int)Images.LidarMap].texture = new Texture2D(CameraModule.ColorWidth / Hud.lidarMapScale, CameraModule.ColorHeight / Hud.lidarMapScale);
        this.images[(int)Images.DepthFeed].texture = new Texture2D(CameraModule.DepthWidth, CameraModule.DepthHeight);

        this.UpdateConnectedPrograms(0);
    }

    private void Start()
    {
        this.FailureMessage.SetActive(false);
        this.SuccessMessage.SetActive(false);

        this.texts[(int)Texts.MainTime].text = string.Empty;
        this.texts[(int)Texts.LapTime].text = string.Empty;

        this.UpdateTimeScale(1.0f);
    }

    protected override void Update()
    {
        this.UpdateController();
        base.Update();
    }

    /// <summary>
    /// Update the controller icon to show the current buttons, triggers, and joysticks being pressed.
    /// </summary>
    private void UpdateController()
    {
        Array buttons = Enum.GetValues(typeof(Controller.Button));
        Array triggers = Enum.GetValues(typeof(Controller.Trigger));
        Array joysticks = Enum.GetValues(typeof(Controller.Joystick));

        int index = (int)Images.ControllerFirstButton;

        foreach (Controller.Button button in buttons)
        {
            this.images[index].enabled = Controller.IsDown(button);
            index++;
        }

        foreach (Controller.Trigger trigger in triggers)
        {
            this.images[index].enabled = Controller.GetTrigger(trigger) > 0;
            index++;
        }

        foreach (Controller.Joystick joystick in joysticks)
        {
            Vector2 joystickAxes = Controller.GetJoystick(joystick);
            this.images[index].enabled = joystickAxes.x != 0 || joystickAxes.y != 0;
            index++;
        }
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
