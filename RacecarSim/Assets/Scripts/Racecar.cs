using System;
using UnityEngine;

public class Racecar : MonoBehaviour
{
    public Camera ThirdPersonCamera;

    #region Constants
    private static readonly Vector3 cameraOffset = new Vector3(0, 5, -6);
    private const float cameraSpeed = 2;

    #endregion

    private Action userStart;
    private Action userUpdate;
    private Action userUpdateSlow;

    private Action curUpdate;
    private Action curUpdateSlow;

    private float updateSlowTime = 1;
    private float updateSlowCounter = 0;

    public Drive Drive { get; private set; }
    public Controller Controller { get; private set; }

    private void Start()
    {
        // Find submodules
        this.Drive = this.gameObject.GetComponent<Drive>();
        this.Controller = this.gameObject.GetComponent<Controller>();

        this.GetComponent<Rigidbody>().centerOfMass = new Vector3(0, -1f, 0);

        this.enterDefaultDrive();
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
            this.handleExit();
        }
        else if (Input.GetButtonDown("Start"))
        {
            this.enterUserProgram();
        }
        else if (Input.GetButtonDown("Back"))
        {
            this.enterDefaultDrive();
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

    private void enterDefaultDrive()
    {
        Debug.Log(">> Entering default drive mode");
        this.defaultStart();
        this.curUpdate = this.defaultUpdate;
        this.curUpdateSlow = null;
    }

    private void enterUserProgram()
    {
        Debug.Log(">> Entering user program mode");
        this.userStart();
        this.curUpdate = this.userUpdate;
        this.curUpdateSlow = this.userUpdateSlow;
    }

    private void handleExit()
    {
        Debug.Log(">> Goodbye!");
        Application.Quit();
    }

    #region Python Interface
    void set_start_update(Action start, Action update, Action update_slow = null)
    {
        this.userStart = start;
        this.userUpdate = update;
        this.userUpdateSlow = update_slow;
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
