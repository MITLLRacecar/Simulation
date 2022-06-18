using System.Collections;
using System.Collections.Generic;
using System.Threading;
﻿using System;
using UnityEngine;

public class Drone : RacecarModule
{

    #region Constants
    /// <summary>
    /// The starting position of the drone.
    /// </summary>
    public static Vector3 startingPosition = new Vector3(0f, 0f, 0f);

    /// <summary>
    /// The target position of the drone - initially set to the starting position.
    /// </summary>
    public Vector3 targetPosition = startingPosition;

    /// <summary>
    /// The width (in pixels) of the color images captured by the camera.
    /// </summary>
    public const int ColorWidth = 640;

    /// <summary>
    /// The height (in pixels) of the color images captured by the camera.
    /// </summary>
    public const int ColorHeight = 480;

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
    /// Time (in ms) to wait for the color image to update during an async call.
    /// </summary>
    private const int asyncWaitTime = 200;

    /// <summary>
    /// Controls the rate at which the drone flies up and down.
    /// </summary>
    private const int droneSpeed = 20;
    #endregion

    #region Public Interface
    /// <summary>
    /// The GPU-side texture to which the color camera renders.
    /// </summary>
    public RenderTexture DroneImg
    {
        get
        {
            return this.droneCamera.targetTexture;
        }
    }

    /// <summary>
    /// The raw bytes of the color image captured by the drone's camera this frame.
    /// Each pixel is stored in the ARGB 32-bit format, from top left to bottom right.
    /// </summary>
    public byte[] DroneImageRaw
    {
        get
        {
            if (!isDroneImageRawValid)
            {
                this.UpdateDroneImageRaw();
            }
            return this.droneImageRaw;
        }
    }

    /// <summary>
    /// Asynchronously updates and returns the color image captured by the drone's camera.
    /// Warning: This method blocks for asyncWaitTime ms to wait for the new image to load.
    /// </summary>
    /// <returns>The color image captured by the drone's camera.</returns>
    public byte[] GetDroneImageRawAsync()
    {
        this.mustUpdateDroneImageRaw = true;
        Thread.Sleep(Drone.asyncWaitTime);
        return this.droneImageRaw;
    }

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

    /// <summary>
    /// Private member for the DroneImageRaw accessor.
    /// </summary>
    private byte[] droneImageRaw;

    /// <summary>
    /// True if droneImageRaw is up to date with the color image rendered for the current frame.
    /// </summary>
    private bool isDroneImageRawValid = false;

    /// <summary>
    /// The color camera on the drone.
    /// </summary>
    private Camera droneCamera;

    /// <summary>
    /// If true, droneImageRaw is updated next frame.
    /// </summary>
    private bool mustUpdateDroneImageRaw;

    private void UpdateDroneImageRaw()
    {
        RenderTexture activeRenderTexture = RenderTexture.active;

        // Tell GPU to render the image captured by the drone camera
        RenderTexture.active = this.DroneImg;
        this.droneCamera.Render();

        // Copy this image from the GPU to a Texture2D on the CPU
        Texture2D image = new Texture2D(this.DroneImg.width, this.DroneImg.height);
        image.ReadPixels(new Rect(0, 0, this.DroneImg.width, this.DroneImg.height), 0, 0);
        image.Apply();

        // Restore the previous GPU render target
        RenderTexture.active = activeRenderTexture;

        // Copy the bytes from the Texture2D to this.colorImageRaw, reversing row order
        // (Unity orders bottom-to-top, we want top-to-bottom)
        byte[] bytes = image.GetRawTextureData();
        int bytesPerRow = Drone.ColorWidth * 4;
        for (int r = 0; r < Drone.ColorHeight; r++)
        {
            Buffer.BlockCopy(bytes, (Drone.ColorHeight - r - 1) * bytesPerRow, this.droneImageRaw, r * bytesPerRow, bytesPerRow);
        }

        Destroy(image);
        this.isDroneImageRawValid = true;
    }

    protected override void Awake()
    {
        Camera[] cameras = this.GetComponentsInChildren<Camera>();
        this.droneCamera = cameras[0];
        this.droneImageRaw = new byte[sizeof(float) * Drone.ColorWidth * Drone.ColorHeight];

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
        if (this.mustUpdateDroneImageRaw)
        {
            this.UpdateDroneImageRaw();
            this.mustUpdateDroneImageRaw = false;
        }
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, (Time.deltaTime * droneSpeed) / Vector3.Distance(targetPosition, transform.localPosition));
    }

    private void LateUpdate()
    {
        this.isDroneImageRawValid = false;
    }
}
