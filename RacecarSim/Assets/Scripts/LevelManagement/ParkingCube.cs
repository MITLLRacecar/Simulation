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
    /// The angle that the car should attempt to reach with the wall
    /// </summary>
    private const float goalAngle = 0;

    /// <summary>
    /// The threshold around goalAngle we will consider an acceptable angle.
    /// </summary>
    private const float angleThreshold = 5;

    /// <summary>
    /// The distance that the car should attempt to reach from the wall.
    /// </summary>
    private const float goalDistance = 20;

    /// <summary>
    /// The threshold around goalDistance we will consider an acceptable distance.
    /// </summary>
    private const float distanceThreshold = 5;

    /// <summary>
    /// The distance (in dm) above the ground at which we cast rays to measure the car's distance from the wall.
    /// </summary>
    private const float rayCastHeight = 2;
    #endregion

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
        Racecar player = LevelManager.GetCar();
        float angle = Mathf.Abs(this.transform.rotation.eulerAngles.y - player.transform.rotation.eulerAngles.y);
        if (angle > 180)
        {
            angle = 360 - angle;
        }

        // Cast a ray from the car to the wall to find the closest point on the wall
        float distance = -1;
        if (Physics.Raycast(player.transform.position + Vector3.up * ParkingCube.rayCastHeight, this.transform.forward, out RaycastHit hit, 1000))
        {
            distance = (hit.distance - Racecar.radius) * 10;
        }

        LevelManager.ShowMessage($"Angle: {angle:F1} degrees\nDistance: {distance:F1} cm", Color.white, -1);

        if (Mathf.Abs(ParkingCube.goalAngle - angle) < ParkingCube.angleThreshold 
            && Mathf.Abs(ParkingCube.goalDistance - distance) < ParkingCube.distanceThreshold
            && player.Physics.LinearVelocity.magnitude < 0.01f)
        {
            LevelManager.HandleFinish(player.Index);
        }
    }
}
