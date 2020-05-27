using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraModule : MonoBehaviour
{
    #region Constants
    private static readonly int[] dimensions = { 640, 480 };
    private static readonly float[] fieldOfView = { 69.4f, 42.5f };
    private static readonly int depthSampleFactor = 4;

    private static float minRange = 0.105f;
    private static float minCode = 0.0f;
    private static float maxRange = 10f;
    private static float maxCode = 0.0f;
    #endregion

    private Tuple<int, int, int>[][] curColorImage;
    private float[][] curDepthImage;

    private void Start()
    {
        this.GetComponent<Camera>().fieldOfView = CameraModule.fieldOfView[0];
    }

    private void LateUpdate()
    {
        this.curColorImage = null;
        this.curDepthImage = null;
    }

    private void takeColorImage()
    {

    }

    private void takeDepthImage()
    {
        this.curDepthImage = new float[CameraModule.dimensions[1]][];

        float imageWidth = Mathf.Tan(CameraModule.fieldOfView[0] * Mathf.PI / 180);
        float imageHeight = Mathf.Tan(CameraModule.fieldOfView[1] * Mathf.PI / 180);
        for (int r = 0; r < CameraModule.dimensions[1]; r++)
        {
            this.curDepthImage[r] = new float[CameraModule.dimensions[0]];

            for (int c = 0; c < CameraModule.dimensions[0]; c++)
            {
                Vector3 direction = this.transform.forward
                    + this.transform.up * imageHeight * -(r / CameraModule.dimensions[1] - 0.5f)
                    + this.transform.right * imageWidth * (c / CameraModule.dimensions[0] - 0.5f);

                if (Physics.Raycast(this.transform.position, direction, out RaycastHit raycastHit, CameraModule.maxRange))
                {
                    this.curDepthImage[r][c] = raycastHit.distance > CameraModule.minRange ? raycastHit.distance * 100 : CameraModule.minCode;
                }

                this.curDepthImage[r][c] = CameraModule.maxCode;
            }
        }
    }

    #region Python Interface
    public Tuple<int, int, int>[][] get_image()
    {
        if (this.curColorImage == null)
        {

        }

        return this.curColorImage;
    }

    public float[][] get_depth_image()
    {
        if (this.curColorImage == null)
        {
            this.takeDepthImage();
        }
        
        return this.curDepthImage;
    }

    public int get_width()
    {
        return CameraModule.dimensions[0];
    }

    public int get_height()
    {
        return CameraModule.dimensions[1];
    }
    #endregion
}
