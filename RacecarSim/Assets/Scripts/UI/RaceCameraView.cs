using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages a view from a single camera on a race screen.
/// </summary>
public class RaceCameraView : MonoBehaviour
{
    /// <summary>
    /// The image displaying the camera view.
    /// </summary>
    public RawImage Image { get; private set; }

    /// <summary>
    /// The text overlaying the camera view.
    /// </summary>
    public Text Text { get; private set; }

    private void Awake()
    {
        this.Image = this.GetComponentInChildren<RawImage>();
        this.Text = this.GetComponentInChildren<Text>();
    }
}
