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
    /// The angle that the car should attempt to reach with the wall in degrees.
    /// </summary>
    private const float goalAngle = 0;

    /// <summary>
    /// The threshold around goalAngle in degrees we will consider an acceptable angle.
    /// </summary>
    private const float angleThreshold = 5;

    /// <summary>
    /// The distance that the car should attempt to reach from the wall in cm.
    /// </summary>
    private const float goalDistance = 20;

    /// <summary>
    /// The threshold around goalDistance in cm we will consider an acceptable distance.
    /// </summary>
    private const float distanceThreshold = 2;
    #endregion

    private float? distance;

    private float? angle;

    /// <summary>
    /// The shortest distance between the car and the wall in cm.
    /// </summary>
    private float Distance
    {
        get
        {
            if (!distance.HasValue)
            {
                RaycastHit raycastHit = new RaycastHit
                {
                    point = this.transform.position,
                    distance = float.NaN
                };

                for (int i = 0; i < 3; i++)
                {
                    // Perform a raycast from the cube to the car to find the closest point on the car
                    if (Physics.Raycast(raycastHit.point, LevelManager.GetCar().Center - raycastHit.point, out RaycastHit carPoint, Constants.RaycastMaxDistance, Constants.IgnoreUIMask))
                    {
                        if (carPoint.collider.GetComponentInParent<Racecar>() == null)
                        {
                            break;
                        }

                        // Perform a second raycast directly back from the car to the wall to find the closest point on the wall
                        if (Physics.Raycast(carPoint.point, this.transform.forward, out raycastHit, Constants.RaycastMaxDistance, Constants.IgnoreUIAndPlayerMask))
                        {
                            if (raycastHit.collider.gameObject != this.gameObject)
                            {
                                break;
                            }

                            print($"{i}: {carPoint.point}; {raycastHit.distance}");
                        }
                    }
                }

                this.distance = raycastHit.distance * 10;
            }

            return this.distance.Value;
        }
    }

    /// <summary>
    /// The angle between the wall and the car in degrees.
    /// </summary>
    private float Angle
    {
        get
        {
            if (!this.angle.HasValue)
            {
                this.angle = Mathf.Abs(this.transform.rotation.eulerAngles.y - LevelManager.GetCar().transform.rotation.eulerAngles.y);
                if (this.angle > 180)
                {
                    this.angle = 360 - this.angle;
                }
            }

            return this.angle.Value;
        }
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
        // Reset angle and distance so they are recalculated this frame
        this.angle = null;
        this.distance = null;

        Color messageColor = Color.white;

        if (Mathf.Abs(ParkingCube.goalAngle - this.Angle) < ParkingCube.angleThreshold
            && Mathf.Abs(ParkingCube.goalDistance - this.Distance) < ParkingCube.distanceThreshold)
        {
            messageColor = Color.green;
            if (LevelManager.GetCar().Physics.LinearVelocity.magnitude < Constants.MaxStopSeed)
            {
                AutograderTask autograderTask = this.GetComponent<AutograderTask>();
                if (autograderTask != null)
                {
                    AutograderManager.CompleteTask(autograderTask);
                }
            }
        }

        LevelManager.ShowMessage($"Angle: {this.Angle:F1} degrees\nDistance: {this.Distance:F1} cm", messageColor, -1);
    }
}
