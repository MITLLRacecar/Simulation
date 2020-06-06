using UnityEngine;

/// <summary>
/// Encapsulates a RACECAR-MN.
/// </summary>
public class Racecar : MonoBehaviour
{
    #region Set in the Unity Editor
    /// <summary>
    /// The third person camera through which the user observes the car.
    /// </summary>
    public Camera ThirdPersonCamera;
    #endregion

    #region Constants
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
    /// Causes the car to enter default drive mode, allowing the user to drive around with the controller.
    /// </summary>
    public void EnterDefaultDrive()
    {
        Debug.Log(">> Entering default drive mode");
        this.isDefaultDrive = true;
        this.DefaultDriveStart();
    }

    /// <summary>
    /// Causes the car to enter user program mode, executing the user program.
    /// </summary>
    public void EnterUserProgram()
    {
        Debug.Log(">> Entering user program mode");
        PythonInterface.Instance.PythonStart();
        this.isDefaultDrive = false;
    }

    /// <summary>
    /// Safely exits the application.
    /// </summary>
    public void HandleExit()
    {
        Debug.Log(">> Goodbye!");
        PythonInterface.Instance.HandleExit();
        Application.Quit();
    }
    #endregion

    /// <summary>
    /// True if the car is currently in default drive mode.
    /// </summary>
    private bool isDefaultDrive = true;

    private void Start()
    {
        // Find submodules
        this.Camera = this.GetComponent<CameraModule>();
        this.Controller = this.GetComponent<Controller>();
        this.Drive = this.GetComponent<Drive>();
        this.Lidar = this.GetComponentInChildren<Lidar>();
        this.Physics = this.GetComponent<PhysicsModule>();

        this.EnterDefaultDrive();
    }

    private void Update()
    {
        if (isDefaultDrive)
        {
            this.DefaultDriveUpdate();
        }
        else
        {
            PythonInterface.Instance.PythonUpdate();
        }

        if (Input.GetButton("Start") && Input.GetButton("Back"))
        {
            this.HandleExit();
        }
        else if (Input.GetButtonDown("Start"))
        {
            this.EnterUserProgram();
        }
        else if (Input.GetButtonDown("Back"))
        {
            this.EnterDefaultDrive();
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
