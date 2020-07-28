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
    /// Exposes the RealSense D435i color and depth channels.
    /// </summary>
    public CameraModule Camera { get; private set; }

    /// <summary>
    /// Exposes the Xbox controller.
    /// </summary>
    public Controller Controller { get; private set; }

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
    /// The heads-up display associated with this car.
    /// </summary>
    public Hud Hud { get; private set; }

    /// <summary>
    /// Causes the car to enter default drive mode, allowing the user to drive around with the controller.
    /// </summary>
    public void EnterDefaultDrive()
    {
        print(">> Entering default drive mode");
        this.isDefaultDrive = true;
        this.DefaultDriveStart();
        this.Hud.UpdateMode(this.isDefaultDrive, this.isValidRun);
    }

    /// <summary>
    /// Causes the car to enter user program mode, executing the user program.
    /// </summary>
    public void EnterUserProgram()
    {
        print(">> Entering user program mode");
        this.pythonInterface.PythonStart();
        this.isDefaultDrive = false;
        this.Hud.UpdateMode(this.isDefaultDrive, this.isValidRun);

        this.startTime = Time.time;
        VariableManager.SetKeyTime(VariableManager.KeyTime.Start, this.startTime);
    }

    /// <summary>
    /// Safely exits the application.
    /// </summary>
    public void HandleExit()
    {
        print(">> Goodbye!");
        this.pythonInterface.HandleExit();

        // Reload current level with the ReloadBuffer
        ReloadBuffer.BuildIndexToReload = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadSceneAsync(ReloadBuffer.BuildIndex, LoadSceneMode.Single);
    }

    /// <summary>
    /// Handles crossing the finish line.
    /// </summary>
    /// <param name="level">The level entry to update in BestTimes (if time is recorded for this level).</param>
    public void HandleFinish(BestTimes.Level level = BestTimes.Level.None)
    {
        if (this.isValidRun)
        {
            float time = Time.time - this.startTime;
            this.Hud.ShowSuccessMessage(time);
            this.isValidRun = false;

            if (level != BestTimes.Level.None)
            {
                BestTimes.UpdateBestTime(level, time);
            }
        }
    }

    /// <summary>
    /// Tells the car that the current level has a winnable objective.
    /// </summary>
    public void SetIsWinable()
    {
        this.isValidRun = true;
        this.Hud.UpdateMode(this.isDefaultDrive, this.isValidRun);
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
    #endregion

    /// <summary>
    /// The UDP interface to the Python script controlling this car.
    /// </summary>
    private PythonInterface pythonInterface;

    /// <summary>
    /// True if the car is currently in default drive mode.
    /// </summary>
    private bool isDefaultDrive = true;

    /// <summary>
    /// True if the current run has only been controlled by the user's program.
    /// </summary>
    private bool isValidRun;

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
        this.isValidRun = false;
        this.curCamera = 0;

        this.pythonInterface = new PythonInterface(this);
        this.Hud = this.transform.parent.GetComponentInChildren<Hud>();

        // Find submodules
        this.Camera = this.GetComponent<CameraModule>();
        this.Controller = this.GetComponent<Controller>();
        this.Drive = this.GetComponent<Drive>();
        this.Lidar = this.GetComponentInChildren<Lidar>();
        this.Physics = this.GetComponent<PhysicsModule>();
    }

    private void Start()
    {
        // Begin with main player camera (0th camera)
        this.PlayerCameras[0].enabled = true;
        for (int i = 1; i < this.PlayerCameras.Length; i++)
        {
            this.PlayerCameras[i].enabled = false;
        }

        this.EnterDefaultDrive();
    }

    private void Update()
    {
        // Call correct update function based on mode
        if (this.isDefaultDrive)
        {
            this.DefaultDriveUpdate();
        }
        else
        {
            this.pythonInterface.PythonUpdate();
        }

        // Handle START and BACK buttons
        if (this.Controller.IsDown(Controller.Button.START) && this.Controller.IsDown(Controller.Button.BACK))
        {
            this.HandleExit();
        }
        else if (this.Controller.WasPressed(Controller.Button.START))
        {
            this.EnterUserProgram();
        }
        else if (this.Controller.WasPressed(Controller.Button.BACK))
        {
            this.EnterDefaultDrive();
            if (this.isValidRun)
            {
                this.isValidRun = false;
                this.Hud.UpdateMode(this.isDefaultDrive, this.isValidRun);
            }
        }

        // Toggle camera when the space bar is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            this.PlayerCameras[this.curCamera].enabled = false;
            this.curCamera = (this.curCamera + 1) % this.PlayerCameras.Length;
            this.PlayerCameras[this.curCamera].enabled = true;
        }

        // Return to main menu on escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            this.pythonInterface.HandleExit();
            SceneManager.LoadScene(0);
        }

        // Check if we have exceeded FailureSpeed
        if (this.FailureSpeed > 0 && this.Physics.LinearVelocity.magnitude > this.FailureSpeed)
        {
            this.Hud.ShowFailureMessage($"The car exceeded {this.FailureSpeed} m/s");
        }
    }

    private void LateUpdate()
    {
        for (int i = 0; i < this.PlayerCameras.Length; ++i)
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Checkpoint>() != null)
        {
            this.checkPoint = other.gameObject;
        }
    }

    /// <summary>
    /// Called on the first frame when the car enters default drive mode.
    /// </summary>
    private void DefaultDriveStart()
    {
        this.Drive.MaxSpeed = Drive.DefaultMaxSpeed;
        this.Drive.Stop();
    }

    /// <summary>
    /// Called each frame that the car is in default drive mode.
    /// </summary>
    private void DefaultDriveUpdate()
    {
        this.Drive.Speed = this.Controller.GetTrigger(Controller.Trigger.RIGHT) - this.Controller.GetTrigger(Controller.Trigger.LEFT);
        this.Drive.Angle = this.Controller.GetJoystick(Controller.Joystick.LEFT).x;

        if (this.Controller.WasPressed(Controller.Button.A))
        {
            print("Kachow!");
        }

        if (this.Controller.WasPressed(Controller.Button.B))
        {
            this.ResetToCheckpoint();
        }

        // Use the bumpers to adjust max speed
        if (this.Controller.WasPressed(Controller.Button.RB))
        {
            this.Drive.MaxSpeed = Mathf.Min(this.Drive.MaxSpeed + 0.1f, 1);
        }
        if (this.Controller.WasPressed(Controller.Button.LB))
        {
            this.Drive.MaxSpeed = Mathf.Max(this.Drive.MaxSpeed - 0.1f, 0);
        }

        // If the user moves in default drive mode, it is no longer a valid run
        if (this.isValidRun && this.Drive.Speed != 0)
        {
            this.isValidRun = false;
            this.Hud.UpdateMode(this.isDefaultDrive, this.isValidRun);
        }
    }
}
