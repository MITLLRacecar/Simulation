using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraModule : MonoBehaviour
{
    #region Constants
    public const int Width = 640;
    public const int Height = 480;

    private static readonly float[] fieldOfView = { 69.4f, 42.5f };
    private static readonly int depthSampleFactor = 8;

    private static float minRange = 0.105f;
    private static float minCode = 0.0f;
    private static float maxRange = 10f;
    private static float maxCode = 0.0f;
    #endregion

    private Tuple<int, int, int>[][] colorImage;
    private bool isColorImageValid = false;
    private float[][] depthImage;
    private bool isDepthImageValid = false;

    public Tuple<int, int, int>[][] ColorImage
    {
        get
        {
            if (!this.isColorImageValid)
            {
                this.takeColorImage();
            }

            return this.colorImage;
        }
    }

    public float[][] DepthImage
    {
        get
        {
            if (!isDepthImageValid)
            {
                this.takeDepthImage();
            }

            return this.depthImage;
        }
    }

    public void VisualizeDepth(Texture2D texture)
    {
        //if (texture.width != CameraModule.Width || texture.height != CameraModule.Height)
        //{
        //    throw new Exception("texture dimensions must match depth image dimensions");
        //}

        //Unity.Collections.NativeArray<Color32> rawData = texture.GetRawTextureData<Color32>();

        //for (int i = 0; i < rawData.Length; i++)
        //{
        //    rawData[i] = Color.black;
        //}

        //for (int r = 0; r < this.DepthImage.Length; r++)
        //{
        //    for (int c = 0; c < this.DepthImage[r].Length; c++)
        //    {
        //        if (this.DepthImage[r][c] != CameraModule.minCode && this.DepthImage[r][c] != CameraModule.maxCode)
        //        {
        //            rawData[r * texture.width + c] = Color.Lerp(Color.red, Color.blue, DepthImage[r][c] / CameraModule.maxRange);
        //        }
        //    }
        //}

        //texture.Apply();
    }

    private void Start()
    {
        this.GetComponent<Camera>().fieldOfView = CameraModule.fieldOfView[0];

        this.depthImage = new float[CameraModule.Height][];
        for (int r = 0; r < CameraModule.Height; r++)
        {
            this.depthImage[r] = new float[CameraModule.Width];
        }
    }

    private void LateUpdate()
    {
        this.isColorImageValid = false;
        this.isDepthImageValid = false;
    }

    private void takeColorImage()
    {

    }

    private void takeDepthImage()
    {
        float imageWidth = Mathf.Tan(CameraModule.fieldOfView[0] * Mathf.PI / 180);
        float imageHeight = Mathf.Tan(CameraModule.fieldOfView[1] * Mathf.PI / 180);
        for (int r = 0; r < CameraModule.Height; r++)
        {
            for (int c = 0; c < CameraModule.Width; c++)
            {
                if (r % depthSampleFactor == 0 && c % depthSampleFactor == 0)
                {
                    Vector3 direction = this.transform.forward
                        + this.transform.up * imageHeight * -(r / CameraModule.Height - 0.5f)
                        + this.transform.right * imageWidth * (c / CameraModule.Width - 0.5f);

                    if (Physics.Raycast(this.transform.position, direction, out RaycastHit raycastHit, CameraModule.maxRange))
                    {
                        this.depthImage[r][c] = raycastHit.distance > CameraModule.minRange ? raycastHit.distance * 100 : CameraModule.minCode;
                    }

                    this.depthImage[r][c] = CameraModule.maxCode;
                }
            }
        }
    }

    #region Python Interface
    public Tuple<int, int, int>[][] get_image()
    {
        return this.ColorImage;
    }

    public float[][] get_depth_image()
    {
        return this.DepthImage;
    }

    public int get_width()
    {
        return CameraModule.Width;
    }

    public int get_height()
    {
        return CameraModule.Height;
    }
    #endregion
}
