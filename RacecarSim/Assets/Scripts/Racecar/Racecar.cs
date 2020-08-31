using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Encapsulates a RACECAR-MN.
/// </summary>
public class Racecar : MonoBehaviour
{
    #region Set in Unity Editor
    /// <summary>
    /// The cameras through which the user can observe the car.
    /// </summary>
    [SerializeField]
    private Camera[] PlayerCameras;

    /// <summary>
    /// Cause the player to fail if they exceed this speed (in m/s).
    /// </summary>
    [SerializeField]
    private float FailureSpeed = -1;
    #endregion

    #region Constants
    /// <summary>
    /// The radius of the car along the Z axis (in dm)
    /// </summary>
    public const float radius = 1.8f;

    /// <summary>
    /// The distance from which each player camera follows the car.
    /// </summary>
    private static readonly Vector3[] cameraOffsets =
    {
        new Vector3(0, 4, -8),
        new Vector3(0, 20, -2),
        new Vector3(0, 4, 8)
    };

    /// <summary>
    /// The speed at which the camera follows the car.
    /// </summary>
    private const float cameraSpeed = 6;
    #endregion

    #region Public Interface
    /// <summary>
    /// The index of the racecar.
    /// </summary>
    public int Index;

    /// <summary>
    /// Exposes the RealSense D435i color and depth channels.
    /// </summary>
    public CameraModule Camera { get; private set; }

    /// <summary>
    /// Exposes the car motors.
    /// </summary>
    public Drive Drive { get; private set; }

    /// <summary>
    /// Exposes the YDLIDAR X4 sensor.
    /// </summary>
    public Lidar Lidar { get; private set; }

    /// <summary>
    /// Exposes the RealSense D435i IMU.
    /// </summary>
    public PhysicsModule Physics { get; private set; }

    /// <summary>
    /// The heads-up display controlled by this car, if any.
    /// </summary>
    public Hud Hud { get; set; }

    /// <summary>
    /// Handles crossing the finish line.
    /// </summary>
    /// <param name="level">The level entry to update in BestTimes (if time is recorded for this level).</param>
    public void HandleFinish(BestTimes.Level level = BestTimes.Level.None)
    {
        //if (this.isValidRun)
        //{
        //    float time = Time.time - this.startTime;
        //    if (this.Hud != null)
        //    {
        //        this.Hud.ShowSuccessMessage(time);
        //    }
        //    this.isValidRun = false;

        //    if (level != BestTimes.Level.None)
        //    {
        //        BestTimes.UpdateBestTime(level, time);
        //    }
        //}
    }

    /// <summary>
    /// Moves the car to the most recent checkpoint.
    /// </summary>
    public void ResetToCheckpoint()
    {
        if (this.checkPoint != null)
        {
            this.transform.position = this.checkPoint.transform.position + Vector3.up;
            this.transform.rotation = this.checkPoint.transform.rotation;

            Rigidbody rbody = this.GetComponent<Rigidbody>();
            rbody.velocity = Vector3.zero;
            rbody.angularVelocity = Vector3.zero;
        }
    }

    /// <summary>
    /// Called on the first frame when the car enters default drive mode.
    /// </summary>
    public void DefaultDriveStart()
    {
        this.Drive.MaxSpeed = Drive.DefaultMaxSpeed;
        this.Drive.Stop();
    }

    /// <summary>
    /// Called each frame that the car is in default drive mode.
    /// </summary>
    public void DefaultDriveUpdate()
    {
        this.Drive.Speed = Controller.GetTrigger(Controller.Trigger.RIGHT) - Controller.GetTrigger(Controller.Trigger.LEFT);
        this.Drive.Angle = Controller.GetJoystick(Controller.Joystick.LEFT).x;

        if (Controller.WasPressed(Controller.Button.A))
        {
            print("Kachow!");
        }

        if (Controller.WasPressed(Controller.Button.Y))
        {
            this.ResetToCheckpoint();
        }

        // Use the bumpers to adjust max speed
        if (Controller.WasPressed(Controller.Button.RB))
        {
            this.Drive.MaxSpeed = Mathf.Min(this.Drive.MaxSpeed + 0.1f, 1);
        }
        if (Controller.WasPressed(Controller.Button.LB))
        {
            this.Drive.MaxSpeed = Mathf.Max(this.Drive.MaxSpeed - 0.1f, 0);
        }
    }
    #endregion

    /// <summary>
    /// The time at which the car entered user program mode.
    /// </summary>
    private float startTime;

    /// <summary>
    /// The index in PlayerCameras of the current active camera.
    /// </summary>
    private int curCamera;

    /// <summary>
    /// The most recent checkpoint which the RACECAR touched.
    /// </summary>
    private GameObject checkPoint;

    private void Awake()
    {
        this.curCamera = 0;

        // Find submodules
        this.Camera = this.GetComponent<CameraModule>();
        this.Drive = this.GetComponent<Drive>();
        this.Lidar = this.GetComponentInChildren<Lidar>();
        this.Physics = this.GetComponent<PhysicsModule>();
    }

    private void Start()
    {
        // Begin with main player camera (0th camera)
        if (this.PlayerCameras.Length > 0)
        {
            this.PlayerCameras[0].enabled = true;
            for (int i = 1; i < this.PlayerCameras.Length; i++)
            {
                this.PlayerCameras[i].enabled = false;
            }
        }
    }

    private void Update()
    {
        // Toggle camera when the space bar is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            this.PlayerCameras[this.curCamera].enabled = false;
            this.curCamera = (this.curCamera + 1) % this.PlayerCameras.Length;
            this.PlayerCameras[this.curCamera].enabled = true;
        }

        // Check if we have exceeded FailureSpeed
        if (this.FailureSpeed > 0 && this.Physics.LinearVelocity.magnitude > this.FailureSpeed)
        {
            if (this.Hud != null)
            {
                this.Hud.ShowFailureMessage($"The car exceeded {this.FailureSpeed} m/s");
            }
        }
    }

    private void LateUpdate()
    {
        for (int i = 0; i < this.PlayerCameras.Length; i++)
        {
            Vector3 followPoint = this.transform.forward * Racecar.cameraOffsets[i].z;
            Vector3 targetCameraPosition = this.transform.position + new Vector3(followPoint.x, Racecar.cameraOffsets[i].y, followPoint.z);
            this.PlayerCameras[i].transform.position = Vector3.Lerp(
                this.PlayerCameras[i].transform.position,
                targetCameraPosition,
                Racecar.cameraSpeed * Time.deltaTime);

            this.PlayerCameras[i].transform.LookAt(this.transform.position);
        }
    }
}
