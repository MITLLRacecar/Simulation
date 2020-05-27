using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraModule : MonoBehaviour
{
    #region Constants
    public const int ColorWidth = 640;
    public const int ColorHeight = 480;

    public const int DepthWidth = CameraModule.ColorWidth / 8;
    public const int DepthHeight = CameraModule.ColorHeight / 8;

    private static readonly float[] fieldOfView = { 69.4f, 42.5f };

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
                this.TakeColorImage();
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
                this.TakeDepthImage();
            }

            return this.depthImage;
        }
    }

    public void VisualizeDepth(Texture2D texture)
    {
        if (texture.width != CameraModule.DepthWidth || texture.height != CameraModule.DepthHeight)
        {
            throw new Exception("texture dimensions must match depth image dimensions");
        }

        Unity.Collections.NativeArray<Color32> rawData = texture.GetRawTextureData<Color32>();

        for (int i = 0; i < rawData.Length; i++)
        {
            rawData[i] = Hud.SensorBackgroundColor;
        }

        for (int r = 0; r < CameraModule.DepthHeight; r++)
        {
            for (int c = 0; c < CameraModule.DepthWidth; c++)
            {
                if (this.DepthImage[r][c] != CameraModule.minCode && this.DepthImage[r][c] != CameraModule.maxCode)
                {
                    rawData[r * texture.width + c] = Color.Lerp(Color.red, Color.blue, DepthImage[r][c] / 100 / CameraModule.maxRange);
                }
            }
        }

        texture.Apply();
    }

    private void Start()
    {
        this.GetComponent<Camera>().fieldOfView = CameraModule.fieldOfView[0];

        this.depthImage = new float[CameraModule.DepthHeight][];
        for (int r = 0; r < CameraModule.DepthHeight; r++)
        {
            this.depthImage[r] = new float[CameraModule.DepthWidth];
        }
    }

    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log(this.DepthImage);
        }

        this.isColorImageValid = false;
        this.isDepthImageValid = false;
    }

    private void TakeColorImage()
    {
        this.isColorImageValid = true;
    }

    private void TakeDepthImage()
    {
        float imageWidth = Mathf.Tan(CameraModule.fieldOfView[0] * Mathf.PI / 180);
        float imageHeight = Mathf.Tan(CameraModule.fieldOfView[1] * Mathf.PI / 180);

        for (int r = 0; r < CameraModule.DepthHeight; r++)
        {
            for (int c = 0; c < CameraModule.DepthWidth; c++)
            {
                Vector3 direction = this.transform.forward
                    + this.transform.up * imageHeight * ((float)r / CameraModule.DepthHeight - 0.5f)
                    + this.transform.right * imageWidth * ((float)c / CameraModule.DepthWidth - 0.5f);

                if (Physics.Raycast(this.transform.position, direction, out RaycastHit raycastHit, CameraModule.maxRange))
                {
                    this.depthImage[r][c] = raycastHit.distance > CameraModule.minRange ? raycastHit.distance * 100 : CameraModule.minCode;
                }
                else
                {
                    this.depthImage[r][c] = CameraModule.maxCode;
                }               
            }
        }

        this.isDepthImageValid = true;
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
        return CameraModule.ColorWidth;
    }

    public int get_height()
    {
        return CameraModule.ColorHeight;
    }
    #endregion
}
