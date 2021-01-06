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

    /// <summary>
    /// The autograder task attached to the ParkingCube, if any.
    /// </summary>
    private AutograderTask autograderTask;

    /// <summary>
    /// The shortest distance between the car and the wall in cm calculated this frame.
    /// </summary>
    private float? distance;

    /// <summary>
    /// The angle between the wall and the car in degrees calculated this frame.
    /// </summary>
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

    /// <summary>
    /// True if the car is within the desired distance and angle threshold.
    /// </summary>
    private bool IsSuccess
    {
        get
        {
            return Mathf.Abs(ParkingCube.goalAngle - this.Angle) < ParkingCube.angleThreshold 
                && Mathf.Abs(ParkingCube.goalDistance - this.Distance) < ParkingCube.distanceThreshold;
        }
    }

    private void Awake()
    {
        this.autograderTask = this.GetComponent<AutograderTask>();
    }

    private void Start()
    {
        // In exploration mode, randomize the position and rotation within the specified boundaries
        if (this.autograderTask == null)
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
    }

    private void Update()
    {
        // Reset angle and distance so they are recalculated this frame
        this.angle = null;
        this.distance = null;

        if (this.autograderTask == null)
        {
            LevelManager.ShowMessage($"Angle: {this.Angle:F1} degrees\nDistance: {this.Distance:F1} cm", this.IsSuccess ? Color.green : Color.white, -1);
        }
        else if (this.IsSuccess && LevelManager.GetCar().Physics.LinearVelocity.magnitude < Constants.MaxStopSeed)
        {
            // TODO: Find a way to display Angle and Distance
            AutograderManager.CompleteTask(this.autograderTask);
        }
    }
}
