using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A cone which measures and displays the distance to the car.
/// </summary>
public class DistanceCone : MonoBehaviour
{
    #region Constants
    /// <summary>
    /// The smallest the cone can become.
    /// </summary>
    private const float minScale = 0.25f;

    /// <summary>
    /// The largest the cone can become.
    /// </summary>
    private const float maxScale = 4;
    #endregion
    /// <summary>
    /// The canvas used to display the cone distance.
    /// </summary>
    private Canvas canvas;

    /// <summary>
    /// The text displaying the cone distance.
    /// </summary>
    protected Text text;

    /// <summary>
    /// The current size of the cone.
    /// </summary>
    protected float Scale
    {
        get
        {
            // The scale will always be the same along all three axes
            return this.transform.localScale.x;
        }
        set
        {
            float scale = Mathf.Clamp(value, DistanceCone.minScale, DistanceCone.maxScale);
            this.transform.localScale = Vector3.one * scale;
        }
    }

    /// <summary>
    /// The radius of the cone at the center in dm.
    /// </summary>
    private float Radius
    {
        get
        {
            return 0.35f * this.Scale;
        }
    }

    /// <summary>
    /// The center of the cone (ie half way up the cone body).
    /// </summary>
    private Vector3 Center
    {
        get
        {
            return this.transform.position + this.transform.up * this.Scale * 0.6f;
        }
    }

    /// <summary>
    /// The distance between the cone and the car in cm, or NaN if the cone cannot see the car.
    /// </summary>
    protected float Distance
    {
        get
        {
            if (Physics.Raycast(this.Center, LevelManager.GetCar().Center - this.Center, out RaycastHit raycastHit, Constants.RaycastMaxDistance, Constants.IgnoreUIMask))
            {
                if (raycastHit.collider.GetComponentInParent<Racecar>() != null)
                {
                    return 10 * (raycastHit.distance - this.Radius);
                }
            }

            return float.NaN;
        }
    }

    protected virtual void Awake()
    {
        this.canvas = this.GetComponentInChildren<Canvas>();
        this.text = this.GetComponentInChildren<Text>();
    }

    protected virtual void Start()
    {
        this.Scale = this.transform.localScale.x;
    }

    protected virtual void Update()
    {
        this.text.text = $"{this.Distance:F1} cm";
        this.canvas.transform.LookAt(Camera.main.transform.position);
    }
}
