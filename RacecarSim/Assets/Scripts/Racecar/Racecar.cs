using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Encapsulates a RACECAR-MN.
/// </summary>
public class Racecar : MonoBehaviour
{
    #region Set in Unity Editor
    /// <summary>
    /// The third person camera through which the user observes the car.
    /// </summary>
    [SerializeField]
    private Camera ThirdPersonCamera;

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
    /// The distance from which the camera follows the car.
    /// </summary>
    private static readonly Vector3 cameraOffset = new Vector3(0, 4.0f, -8.0f);

    /// <summary>
    /// The speed at which the camera follows the car.
    /// </summary>
    private const float cameraSpeed = 6;
    #endregion

    #region Public Interface
    /// <summary>
    /// The settings applied to this car/user.
    /// </summary>
    public Settings Settings { get; private set; }

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
        Debug.Log(">> Entering default drive mode");
        this.isDefaultDrive = true;
        this.DefaultDriveStart();
        this.Hud.UpdateMode(true);
    }

    /// <summary>
    /// Causes the car to enter user program mode, executing the user program.
    /// </summary>
    public void EnterUserProgram()
    {
        Debug.Log(">> Entering user program mode");
        this.pythonInterface.PythonStart();
        this.isDefaultDrive = false;
        this.Hud.UpdateMode(false);
    }

    /// <summary>
    /// Safely exits the application.
    /// </summary>
    public void HandleExit()
    {
        Debug.Log(">> Goodbye!");
        this.pythonInterface.HandleExit();

        // Reload current level with the ReloadBuffer
        ReloadBuffer.BuildIndexToReload = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadSceneAsync(ReloadBuffer.BuildIndex, LoadSceneMode.Single);
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

    private void Awake()
    {
        this.Settings = new Settings();
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
        this.EnterDefaultDrive();
    }

    private void Update()
    {
        // Call correct update function based on mode
        if (isDefaultDrive)
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
        Vector3 followPoint = this.transform.forward * Racecar.cameraOffset.z;
        Vector3 targetCameraPosition = this.transform.position + new Vector3(followPoint.x, Racecar.cameraOffset.y, followPoint.z);
        this.ThirdPersonCamera.transform.position = Vector3.Lerp(this.ThirdPersonCamera.transform.position, targetCameraPosition, Racecar.cameraSpeed * Time.deltaTime);

        this.ThirdPersonCamera.transform.LookAt(this.transform.position);
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
    }
}
