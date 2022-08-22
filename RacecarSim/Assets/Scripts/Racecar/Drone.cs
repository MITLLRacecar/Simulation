using System.Collections;
using System.Collections.Generic;
using System.Threading;
﻿using System;
using UnityEngine;

public class Drone : RacecarModule
{

    #region Constants
    /// <summary>
    /// The width (in pixels) of the color images captured by the camera.
    /// </summary>
    public const int ColorWidth = 640;

    /// <summary>
    /// The height (in pixels) of the color images captured by the camera.
    /// </summary>
    public const int ColorHeight = 480;

    /// <summary>
    /// The starting position of the drone.
    /// </summary>
    public static Vector3 startingPosition = new Vector3(0f, 0f, 0f);

    /// <summary>
    /// The target position of the drone - initially set to the starting position.
    /// </summary>
    public Vector3 targetPosition = startingPosition;

    /// <summary>
    /// The field of view (in degrees) of the camera.
    /// Based on the Intel RealSense D435i datasheet.
    /// </summary>
    private static readonly Vector2 fieldOfView = new Vector2(69.4f, 42.5f);

    /// <summary>
    /// The average relative error of distance measurements.
    /// Based on the Intel RealSense D435i datasheet.
    /// </summary>
    private const float averageErrorFactor = 0.02f;

    /// <summary>
    /// Time (in ms) to wait for the color or depth image to update during an async call.
    /// </summary>
    private const int asyncWaitTime = 200;

    /// <summary>
    /// Controls the rate at which the drone flies up and down.
    /// </summary>
    private const int droneSpeed = 20;
    #endregion

    #region Public Interface
    /// <summary>
    /// The camera on the drone.
    /// </summary>
    private Camera droneCamera;

    /// <summary>
    /// Helper object for the drone camera.
    /// </summary>
    public ImageCaptureHelper droneCameraHelper;

    /// <summary>
    /// The current position of the drone.
    /// </summary>
    public Vector3 CurrentPosition
    {
      get
      {
        return this.transform.position;
      }
    }

    /// <summary>
    /// The target height the drone should fly to.
    /// </summary>
    public float TargetHeight
    {
      private get
      {
        return this.targetPosition.y;
      }

      set
      {
        this.targetPosition = new Vector3(this.targetPosition.x, value, this.targetPosition.z);
      }
    }

    /// <summary>
    /// Has the drone descend to its starting position when called.
    /// </summary>
    /// <returns>Null.</returns>
    public void Land()
    {
      this.targetPosition = startingPosition;
    }
    #endregion

    protected override void Awake()
    {
        Camera[] cameras = this.GetComponentsInChildren<Camera>();
        this.droneCamera = cameras[0];
        droneCameraHelper = new ImageCaptureHelper(this.droneCamera);
        base.Awake();
    }

    private void Start()
    {
        this.droneCamera.fieldOfView = Drone.fieldOfView.y;
        startingPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
        this.targetPosition = startingPosition;
    }

    private void Update()
    {
        if (droneCameraHelper.mustUpdateRawImage)
        {
            droneCameraHelper.UpdateRawImage();
            droneCameraHelper.mustUpdateRawImage = false;
        }
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, (Time.deltaTime * droneSpeed) / Vector3.Distance(targetPosition, transform.localPosition));
    }

    private void LateUpdate()
    {
        droneCameraHelper.isRawImageValid = false;
    }
}
