using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages a view from a single camera on a race screen.
/// </summary>
public class RaceCameraView : MonoBehaviour
{
    public RawImage Image { get; private set; }

    public Text Text { get; private set; }

    private void Awake()
    {
        this.Image = this.GetComponentInChildren<RawImage>();
        this.Text = this.GetComponentInChildren<Text>();
    }
}
