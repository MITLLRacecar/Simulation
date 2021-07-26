using System;
using System.Threading;
using UnityEngine;

/// <summary>
/// Simulates the color and depth channels of the RealSense camera.
/// </summary>
public class CameraModule : RacecarModule
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
    /// The field of view (in degrees) of the camera.
    /// Based on the Intel RealSense D435i datasheet.
    /// </summary>
    private static readonly Vector2 fieldOfView = new Vector2(69.4f, 42.5f);

    /// <summary>
    /// The minimum distance (in dm) that can be detected by the depth channel.
    /// Based on the Intel RealSense D435i datasheet.
    /// </summary>
    private static float minRange = 1.05f;

    /// <summary>
    /// The value recorded for a depth sample less than minRange.
    /// </summary>
    private static float minCode = 0.0f;

    /// <summary>
    /// The maximum distance (in dm) that can be detected by the depth channel.
    /// Based on the Intel RealSense D435i datasheet.
    /// </summary>
    private static float maxRange = 100f;

    /// <summary>
    /// The value recorded for a depth sample greater than maxRange.
    /// </summary>
    private static float maxCode = 0.0f;

    /// <summary>
    /// The average relative error of distance measurements.
    /// Based on the Intel RealSense D435i datasheet.
    /// </summary>
    private const float averageErrorFactor = 0.02f;

    /// <summary>
    /// Time (in ms) to wait for the color or depth image to update during an async call.
    /// </summary>
    private const int asyncWaitTime = 200;
    #endregion

    #region Public Interface
    /// <summary>
    /// The width (in pixels) of the depth images captured by the camera.
    /// </summary>
    public static int DepthWidth { get { return CameraModule.ColorWidth / Settings.DepthDivideFactor; } }

    /// <summary>
    /// The height (in pixels) of the depth images captured by the camera.
    /// </summary>
    public static int DepthHeight { get { return CameraModule.ColorHeight / Settings.DepthDivideFactor; } }

    /// <summary>
    /// The GPU-side texture to which the color camera renders.
    /// </summary>
    public RenderTexture ColorImage
    {
        get
        {
            return this.colorCamera.targetTexture;
        }
    }

    /// <summary>
    /// The raw bytes of the color image captured by the color camera this frame.
    /// Each pixel is stored in the ARGB 32-bit format, from top left to bottom right.
    /// </summary>
    public byte[] ColorImageRaw
    {
        get
        {
            if (!isColorImageRawValid)
            {
                this.UpdateColorImageRaw();
            }
            return this.colorImageRaw;
        }
    }

    /// <summary>
    /// The depth values (in cm) captured by the depth camera this frame, from top left to bottom right.
    /// </summary>
    public float[][] DepthImage
    {
        get
        {
            if (!isDepthImageValid)
            {
                this.UpdateDepthImage();
            }

            return this.depthImage;
        }
    }

    /// <summary>
    /// The raw bytes of the depth values (in cm) captured by the depth camera this frame.
    /// Each value is a 32-bit IEEE float, indexed from top left to bottom right.
    /// </summary>
    public byte[] DepthImageRaw
    {
        get
        {
            if (!this.isDepthImageRawValid)
            {
                this.UpdateDepthImageRaw();
            }

            return this.depthImageRaw;
        }
    }

    /// <summary>
    /// Creates a visualization of the current depth image.
    /// </summary>
    /// <param name="texture">The texture to which the visualization is rendered (must be DepthWidth by DepthHeight).</param>
    public void VisualizeDepth(Texture2D texture)
    {
        if (texture.width != CameraModule.DepthWidth || texture.height != CameraModule.DepthHeight)
        {
            throw new Exception("Texture dimensions must match depth image dimensions.");
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
                    rawData[(CameraModule.DepthHeight - r) * texture.width + c] = CameraModule.InterpolateDepthColor(DepthImage[r][c]);
                }
            }
        }

        texture.Apply();
    }

    /// <summary>
    /// Asynchronously updates and returns the color image captured by the camera.
    /// Warning: This method blocks for asyncWaitTime ms to wait for the new image to load.
    /// </summary>
    /// <returns>The color image captured by the camera.</returns>
    public byte[] GetColorImageRawAsync()
    {
        this.mustUpdateColorImageRaw = true;
        Thread.Sleep(CameraModule.asyncWaitTime);
        return this.colorImageRaw;
    }

    /// <summary>
    /// Asynchronously updates and returns the depth image captured by the camera.
    /// Warning: This method blocks for asyncWaitTime ms to wait for the new image to load.
    /// </summary>
    /// <returns>The depth image captured by the camera.</returns>
    public byte[] GetDepthImageRawAsync()
    {
        this.mustUpdateDepthImageRaw = true;
        Thread.Sleep(CameraModule.asyncWaitTime);
        return this.depthImageRaw;
    }
    #endregion

    /// <summary>
    /// Private member for the ColorImageRaw accessor.
    /// </summary>
    private byte[] colorImageRaw;

    /// <summary>
    /// True if colorImageRaw is up to date with the color image rendered for the current frame.
    /// </summary>
    private bool isColorImageRawValid = false;

    /// <summary>
    /// Private member for the DepthImage accessor.
    /// </summary>
    private float[][] depthImage;

    /// <summary>
    /// True if depthImage is up to date with the depth image captured for the current frame.
    /// </summary>
    private bool isDepthImageValid = false;

    /// <summary>
    /// Private member for the DepthImageRaw accessor.
    /// </summary>
    private byte[] depthImageRaw;

    /// <summary>
    /// True if depthImageRaw is up to date with the depth image captured for the current frame.
    /// </summary>
    private bool isDepthImageRawValid = false;

    /// <summary>
    /// The color camera on the car.
    /// </summary>
    private Camera colorCamera;

    /// <summary>
    /// The depth camera on the car.
    /// This is currently unused, but a future goal is to use this instead of raycasts.
    /// </summary>
    private Camera depthCamera;

    /// <summary>
    /// If true, colorImageRaw is updated next frame.
    /// </summary>
    private bool mustUpdateColorImageRaw;

    /// <summary>
    /// If true, depthImageRaw is updated next frame.
    /// </summary>
    private bool mustUpdateDepthImageRaw;

    protected override void Awake()
    {
        Camera[] cameras = this.GetComponentsInChildren<Camera>();
        this.colorCamera = cameras[0];
        this.depthCamera = cameras[1];

        this.depthImage = new float[CameraModule.DepthHeight][];
        for (int r = 0; r < CameraModule.DepthHeight; r++)
        {
            this.depthImage[r] = new float[CameraModule.DepthWidth];
        }

        this.depthImageRaw = new byte[sizeof(float) * CameraModule.DepthHeight * CameraModule.DepthWidth];
        this.colorImageRaw = new byte[sizeof(float) * CameraModule.ColorWidth * CameraModule.ColorHeight];

        if (Settings.HideCarsInColorCamera)
        {
            this.colorCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("Player"));
        }

        base.Awake();
    }

    private void Start()
    {
        this.colorCamera.fieldOfView = CameraModule.fieldOfView.y;
        this.depthCamera.fieldOfView = CameraModule.fieldOfView.y;
    }

    private void Update()
    {
        if (this.mustUpdateColorImageRaw)
        {
            this.UpdateColorImageRaw();
            this.mustUpdateColorImageRaw = false;
        }

        if (this.mustUpdateDepthImageRaw)
        {
            this.UpdateDepthImageRaw();
            this.mustUpdateDepthImageRaw = false;
        }

        if (this.racecar.Hud != null)
        {
            this.VisualizeDepth(this.racecar.Hud.DepthVisualization);
        }
    }

    private void LateUpdate()
    {
        this.isColorImageRawValid = false;
        this.isDepthImageValid = false;
        this.isDepthImageRawValid = false;
    }

    /// <summary>
    /// Interpolate a depth sample to a white-yellow-red-blue-gray range.
    /// </summary>
    /// <param name="depth">The depth of a particular sample (in cm).</param>
    /// <returns>The color representing the supplied depth.</returns>
    private static Color InterpolateDepthColor(float depth)
    {
        // Convert depth to a [0, 1] range
        depth /= 10 * CameraModule.maxRange;

        // Select correct color range and interpolate
        if (depth < 0.05f)
        {
            return Color.Lerp(Color.white, Color.yellow, depth / 0.05f);
        }
        else if (depth < 0.2f)
        {
            return Color.Lerp(Color.yellow, Color.red, (depth - 0.05f) / 0.15f);
        }
        else if (depth < 0.6f)
        {
            return Color.Lerp(Color.red, Color.blue, (depth - 0.2f) / 0.4f);
        }
        else
        {
            return Color.Lerp(Color.blue, Hud.SensorBackgroundColor, (depth - 0.6f) / 0.4f);
        }
    }

    /// <summary>
    /// Update colorImageRaw by rendering the color camera on the GPU and copying to the CPU.
    /// Warning: this operation is very expensive.
    /// </summary>
    private void UpdateColorImageRaw()
    {
        RenderTexture activeRenderTexture = RenderTexture.active;

        // Tell GPU to render the image captured by the color camera
        RenderTexture.active = this.ColorImage;
        this.colorCamera.Render();

        // Copy this image from the GPU to a Texture2D on the CPU
        Texture2D image = new Texture2D(this.ColorImage.width, this.ColorImage.height);
        image.ReadPixels(new Rect(0, 0, this.ColorImage.width, this.ColorImage.height), 0, 0);
        image.Apply();

        // Restore the previous GPU render target
        RenderTexture.active = activeRenderTexture;

        // Copy the bytes from the Texture2D to this.colorImageRaw, reversing row order
        // (Unity orders bottom-to-top, we want top-to-bottom)
        byte[] bytes = image.GetRawTextureData();
        int bytesPerRow = CameraModule.ColorWidth * 4;
        for (int r = 0; r < CameraModule.ColorHeight; r++)
        {
            Buffer.BlockCopy(bytes, (CameraModule.ColorHeight - r - 1) * bytesPerRow, this.colorImageRaw, r * bytesPerRow, bytesPerRow);
        }

        Destroy(image);
        this.isColorImageRawValid = true;
    }

    /// <summary>
    /// Update depthImage by performing a ray cast for each depth pixel.
    /// Warning: this operation is very expensive.
    /// </summary>
    private void UpdateDepthImage()
    {
        for (int r = 0; r < CameraModule.DepthHeight; r++)
        {
            for (int c = 0; c < CameraModule.DepthWidth; c++)
            {
                Ray ray = this.depthCamera.ViewportPointToRay(new Vector3(
                    (float)c / (CameraModule.DepthWidth - 1),
                    (CameraModule.DepthHeight - r - 1.0f) / (CameraModule.DepthHeight - 1),
                    0));

                if (Physics.Raycast(ray, out RaycastHit raycastHit, CameraModule.maxRange, Constants.IgnoreUIMask))
                {
                    float distance = Settings.IsRealism 
                        ? raycastHit.distance * NormalDist.Random(1, CameraModule.averageErrorFactor) 
                        : raycastHit.distance;
                    this.depthImage[r][c] = distance > CameraModule.minRange ? distance * 10 : CameraModule.minCode;
                }
                else
                {
                    this.depthImage[r][c] = CameraModule.maxCode;
                }
            }
        }

        this.isDepthImageValid = true;
    }

    /// <summary>
    /// Update depthImageRaw from DepthImage
    /// </summary>
    private void UpdateDepthImageRaw()
    {
        for (int r = 0; r < CameraModule.DepthHeight; r++)
        {
            Buffer.BlockCopy(this.DepthImage[r], 0,
                            this.depthImageRaw, r * CameraModule.DepthWidth * sizeof(float),
                            CameraModule.DepthWidth * sizeof(float));
        }

        this.isDepthImageRawValid = true;
    }
}
