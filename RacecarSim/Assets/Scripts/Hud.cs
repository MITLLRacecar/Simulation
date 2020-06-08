using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the heads-up display shown to the user.
/// </summary>
public class Hud : MonoBehaviour
{
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
    #endregion

    #region Public Interface
    public void UpdateMode(bool isDefaultDrive)
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
        Mode = 13
    }

    /// <summary>
    /// The mutable images of the HUD, with values corresponding to the index in images.
    /// </summary>
    private enum Images
    {
        ColorFeed = 2,
        DepthFeed = 4,
        LidarMap = 6,
        ModeBackground = 8,
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

    private void Start()
    {
        // Find components
        this.texts = GetComponentsInChildren<Text>();
        this.images = this.GetComponentsInChildren<RawImage>();

        this.images[Images.LidarMap.GetHashCode()].texture = new Texture2D(CameraModule.ColorWidth / Hud.lidarMapScale, CameraModule.ColorHeight / Hud.lidarMapScale);
        this.images[Images.DepthFeed.GetHashCode()].texture = new Texture2D(CameraModule.DepthWidth, CameraModule.DepthHeight);

        this.racecar = this.transform.parent.GetComponentInChildren<Racecar>();
    }

    private void Update()
    {
        // Update mutable texts and images
        this.texts[Texts.TrueSpeed.GetHashCode()].text = this.racecar.Physics.LinearVelocity.magnitude.ToString("F2");
        this.texts[Texts.LinearAcceleration.GetHashCode()].text = FormatVector3(this.racecar.Physics.LinearAccceleration);
        this.texts[Texts.AngularVelocity.GetHashCode()].text = FormatVector3(this.racecar.Physics.AngularVelocity);

        this.racecar.Lidar.VisualizeLidar((Texture2D)this.images[Images.LidarMap.GetHashCode()].texture);
        this.racecar.Camera.VisualizeDepth((Texture2D)this.images[Images.DepthFeed.GetHashCode()].texture);
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
