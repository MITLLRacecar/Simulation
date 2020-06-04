using System;
using UnityEngine;

public class Racecar : MonoBehaviour
{
    public Camera ThirdPersonCamera;

    #region Constants
    private static readonly Vector3 cameraOffset = new Vector3(0, 4.0f, -8.0f);
    private const float cameraSpeed = 4;
    #endregion

    #region Public Interface
    public CameraModule Camera { get; private set; }
    public Controller Controller { get; private set; }
    public Drive Drive { get; private set; }
    public Lidar Lidar { get; private set; }
    public PhysicsModule Physics { get; private set; }

    public void EnterDefaultDrive()
    {
        Debug.Log(">> Entering default drive mode");
        this.isDefaultDrive = true;
        this.DefaultDriveStart();
    }

    private void EnterUserProgram()
    {
        Debug.Log(">> Entering user program mode");
        PythonInterface.Instance.PythonStart();
        this.isDefaultDrive = false;
    }

    private void HandleExit()
    {
        Debug.Log(">> Goodbye!");
        PythonInterface.Instance.HandleExit();
        Application.Quit();
    }
    #endregion

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

    private void DefaultDriveStart()
    {
        this.Drive.Stop();
    }

    private void DefaultDriveUpdate()
    {
        float forwardSpeed = this.Controller.get_trigger(Controller.Trigger.RIGHT);
        float backSpeed = this.Controller.get_trigger(Controller.Trigger.LEFT);
        this.Drive.Angle = this.Controller.get_joystick(Controller.Joystick.LEFT).x;

        this.Drive.Speed = forwardSpeed - backSpeed;

        if (this.Controller.was_pressed(Controller.Button.A))
        {
            Debug.Log("Kachow!");
        }
    }
}
