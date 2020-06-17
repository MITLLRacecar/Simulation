using UnityEngine;

/// <summary>
/// Manages the parking cube used in Lab 3C.
/// </summary>
public class ParkingCube : MonoBehaviour
{
    #region Set in Unity Editor
    /// <summary>
    /// The maximum amount the object can translate along each axis (in dm).
    /// </summary>
    [SerializeField]
    private Vector3 MaxTranslation = Vector3.zero;

    /// <summary>
    /// The maximum amount the object can rotate about each axis (in degrees).
    /// </summary>
    [SerializeField]
    private Vector3 MaxRotation = Vector3.zero;
    #endregion

    #region Constants
    /// <summary>
    /// Distance to offset the angle/distance display from the wall closest point.
    /// </summary>
    private static readonly Vector3 canvasOffset = new Vector3(0, 2f, 0.01f);

    /// <summary>
    /// Layer mask used to ignore UI objects.
    /// </summary>
    private const int noUiMask = ~(1 << 5);
    #endregion

    /// <summary>
    /// The racecar from which to measure distance and angle.
    /// </summary>
    private Racecar player;

    private void Awake()
    {
        this.player = GameObject.FindGameObjectWithTag("Player").GetComponent<Racecar>();
    }

    private void Start()
    {
        this.transform.position += new Vector3(
            Random.Range(-this.MaxTranslation.x, this.MaxTranslation.x),
            Random.Range(-this.MaxTranslation.y, this.MaxTranslation.y),
            Random.Range(-this.MaxTranslation.z, this.MaxTranslation.z));

        this.transform.Rotate(
            Random.Range(-this.MaxRotation.x, this.MaxRotation.x),
            Random.Range(-this.MaxRotation.y, this.MaxRotation.y),
            Random.Range(-this.MaxRotation.z, this.MaxRotation.z));
    }

    private void Update()
    {
        float angle = Mathf.Abs(this.transform.rotation.eulerAngles.y - this.player.transform.rotation.eulerAngles.y);
        if (angle > 180)
        {
            angle = 360 - angle;
        }

        // Cast a ray from the car to the wall to find the closest point on the wall
        float distance = -1;
        if (Physics.Raycast(this.player.transform.position, this.transform.forward, out RaycastHit secondHit, 1000, ParkingCube.noUiMask))
        {
            distance = (secondHit.distance - Racecar.radius) * 10;
        }

        this.player.Hud.SetMessage($"Angle: {angle:F1} degrees\nDistance: {distance:F1} cm");
    }
}
