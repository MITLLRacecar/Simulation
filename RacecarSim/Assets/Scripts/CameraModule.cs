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

    private byte[] colorImageRaw;
    private bool isColorImageRawValid = false;
    private float[][] depthImage;
    private bool isDepthImageValid = false;
    private byte[] depthImageRaw;
    private bool isDepthImageRawValid = false;

    public RenderTexture ColorImage
    {
        get
        {
            return this.GetComponent<Camera>().targetTexture;
        }
    }

    public byte[] ColorImageRaw
    {
        get
        {
            if (!isColorImageRawValid)
            {
                Texture2D dest = new Texture2D(this.ColorImage.width, this.ColorImage.height, TextureFormat.BGRA32, false);
                Graphics.CopyTexture(this.ColorImage, dest);
                this.colorImageRaw = dest.GetRawTextureData();
                this.isColorImageRawValid = true;
            }
            return this.colorImageRaw;
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

    public byte[] DepthImageRaw
    {
        get
        {
            if (!this.isDepthImageRawValid)
            {
                for (int r = 0; r < CameraModule.DepthHeight; r++)
                {
                    Buffer.BlockCopy(this.DepthImage[r], 0,
                                    this.depthImageRaw, r * CameraModule.DepthWidth * sizeof(float),
                                    CameraModule.DepthWidth * sizeof(float));
                }
                this.isDepthImageRawValid = true;
            }

            return this.depthImageRaw;
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
        this.GetComponent<Camera>().aspect = (float)CameraModule.ColorWidth / CameraModule.ColorHeight;

        this.depthImage = new float[CameraModule.DepthHeight][];
        for (int r = 0; r < CameraModule.DepthHeight; r++)
        {
            this.depthImage[r] = new float[CameraModule.DepthWidth];
        }

        this.depthImageRaw = new byte[sizeof(float) * CameraModule.DepthHeight * CameraModule.DepthWidth];
    }

    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log(this.DepthImage);
        }

        this.isColorImageRawValid = false;
        this.isDepthImageValid = false;
        this.isDepthImageRawValid = false;
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
}
