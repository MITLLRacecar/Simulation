using System;
using UnityEngine;

public class Racecar : MonoBehaviour
{
    public Camera ThirdPersonCamera;

    #region Constants
    private static readonly Vector3 cameraOffset = new Vector3(0, 5, -6);
    private const float cameraSpeed = 2;

    #endregion

    private Action curUpdate;
    private Action curUpdateSlow;

    private float updateSlowTime = 1;
    private float updateSlowCounter = 0;

    public CameraModule Camera { get; private set; }
    public Controller Controller { get; private set; }
    public Drive Drive { get; private set; }
    public Lidar Lidar { get; private set; }
    public PhysicsModule Physics { get; private set; }

    private void Start()
    {
        // Find submodules
        this.Camera = this.GetComponentInChildren<CameraModule>();
        this.Controller = this.GetComponent<Controller>();
        this.Drive = this.GetComponent<Drive>();
        this.Lidar = this.GetComponentInChildren<Lidar>();
        this.Physics = this.GetComponent<PhysicsModule>();

        this.Lidar.SetCarTransform(this.transform);
        this.GetComponent<Rigidbody>().centerOfMass = new Vector3(0, -1f, 0);

        this.EnterDefaultDrive();
    }

    private void Update()
    {
        this.curUpdate();

        if (this.curUpdateSlow != null)
        {
            this.updateSlowCounter -= Time.deltaTime;
            if (this.updateSlowCounter <= 0)
            {
                this.updateSlowCounter = this.updateSlowTime;
                this.curUpdateSlow();
            }
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

    private void defaultStart()
    {
        this.Drive.stop();
    }

    private void defaultUpdate()
    {
        float forwardSpeed = this.Controller.get_trigger(Controller.Trigger.RIGHT);
        float backSpeed = this.Controller.get_trigger(Controller.Trigger.LEFT);
        float angle = this.Controller.get_joystick(Controller.Joystick.LEFT).Item1;

        this.Drive.set_speed_angle(forwardSpeed - backSpeed, angle);

        if (this.Controller.was_pressed(Controller.Button.A))
        {
            Debug.Log("Kachow!");
        }
    }

    private void EnterDefaultDrive()
    {
        Debug.Log(">> Entering default drive mode");
        this.defaultStart();
        this.curUpdate = this.defaultUpdate;
        this.curUpdateSlow = null;
    }

    private void EnterUserProgram()
    {
        Debug.Log(">> Entering user program mode");
        this.UserStart();
        this.curUpdate = this.UserUpdate;
        this.curUpdateSlow = null;
    }

    private void HandleExit()
    {
        Debug.Log(">> Goodbye!");
        PythonInterface.Instance.HandleExit();
        Application.Quit();
    }

    private void UserStart()
    {

    }

    private void UserUpdate()
    {

    }

    #region Python Interface
    void set_start_update(Action start, Action update, Action update_slow = null)
    {
        // What to do about this method?
    }

    double get_delta_time()
    {
        return Time.deltaTime;
    }

    void set_update_slow_time(float time)
    {
        this.updateSlowTime = time;
    }
    #endregion
}
