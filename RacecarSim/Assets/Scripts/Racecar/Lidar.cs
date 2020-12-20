using System;
using UnityEngine;

/// <summary>
/// Simulates the LIDAR sensor.
/// </summary>
public class Lidar : RacecarModule
{
    #region Constants
    /// <summary>
    /// A bitmask which ignores the UI Layer.
    /// </summary>
    public const int IgnoreUIMask = ~(1 << 5);

    /// <summary>
    /// The number of samples captured in a single rotation.
    /// Based on the YDLIDAR X4 datasheet.
    /// </summary>
    public const int NumSamples = 720;

    /// <summary>
    /// The frequency of the LIDAR motor in hz.
    /// </summary>
    private const int motorFrequency = 6;

    /// <summary>
    /// The number of sample taken per second.
    /// </summary>
    private const int samplesPerSecond = Lidar.NumSamples * Lidar.motorFrequency;

    /// <summary>
    /// The minimum distance that can be detected (in dm).
    /// Based on the YDLIDAR X4 datasheet.
    /// </summary>
    private const float minRange = 1.2f;

    /// <summary>
    /// The value recorded for a sample less than minRange.
    /// </summary>
    private const float minCode = 0.0f;

    /// <summary>
    /// The maximum distance that can be detected (in dm).
    /// Based on the YDLIDAR X4 datasheet.
    /// </summary>
    private const float maxRange = 100;

    /// <summary>
    /// The value recorded for a sample greater than maxRange.
    /// </summary>
    private const float maxCode = 0.0f;

    /// <summary>
    /// The average relative error of distance measurements.
    /// Based on the YDLIDAR X4 datasheet.
    /// </summary>
    private const float averageErrorFactor = 0.02f;

    /// <summary>
    /// The maximum range displayed in the LIDAR visualization (in dm).
    /// </summary>
    private const float visualizationRange = 50;

    /// <summary>
    /// The failure message to show when something hits the LIDAR.
    /// </summary>
    private const string collisionFailureMessage = "The LIDAR is expensive and fragile, please do not hit it!";
    #endregion

    #region Public Interface
    /// <summary>
    /// The distance (in cm) of each angle sample.
    /// </summary>
    public float[] Samples { get; private set; }

    /// <summary>
    /// Creates a visualization of the current LIDAR samples.
    /// </summary>
    /// <param name="texture">The texture to which the LIDAR visualization is rendered.</param>
    public void VisualizeLidar(Texture2D texture)
    {
        Unity.Collections.NativeArray<Color32> rawData = texture.GetRawTextureData<Color32>();

        // Create background: gray for in range and black for out of range
        int circleBoundary = Math.Min(texture.width, texture.height) * Math.Min(texture.width, texture.height) / 4;
        for (int r = 0; r < texture.height; r++)
        {
            for (int c = 0; c < texture.width; c++)
            {
                float x = r - texture.height / 2;
                float y = c - texture.width / 2;
                rawData[r * texture.width + c] = x * x + y * y < circleBoundary ? Hud.SensorBackgroundColor : Color.black;
            }
        }

        // Render each sample as a red pixel
        Vector2 center = new Vector2(texture.width / 2, texture.height / 2);
        float length = Mathf.Min(texture.width / 2.0f, texture.height / 2.0f);
        for (int i = 0; i < this.Samples.Length; i++)
        {
            if (this.Samples[i] != Lidar.minCode && this.Samples[i] != Lidar.maxCode && this.Samples[i] < Lidar.visualizationRange * 10)
            {
                float angle = 2 * Mathf.PI * i / Lidar.NumSamples;
                Vector2 point = center + this.Samples[i] / 10 / Lidar.visualizationRange * length * new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
                rawData[(int)point.y * texture.width + (int)point.x] = Color.red;
            }
        }

        texture.Apply();
    }
    #endregion

    /// <summary>
    /// The index of the most recently captured sample.
    /// </summary>
    private int curSample = 0;

    protected override void FindParent()
    {
        this.racecar = this.GetComponentInParent<Racecar>();
    }

    private void Start()
    {
        this.Samples = new float[Lidar.NumSamples];
    }

    private void Update()
    {
        if (this.racecar.Hud != null)
        {
            this.VisualizeLidar(this.racecar.Hud.LidarVisualization);
        }
    }

    private void FixedUpdate()
    {
        int lastSample = (curSample + Mathf.RoundToInt(Lidar.samplesPerSecond * Time.deltaTime)) % NumSamples;

        // Take samples for the current frame by physically rotating the LIDAR
        while (curSample != lastSample)
        {
            this.transform.localRotation = Quaternion.Euler(0, curSample * 360.0f / Lidar.NumSamples, 0);
            this.Samples[curSample] = TakeSample();
            curSample = (curSample + 1) % NumSamples;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger && this.racecar.Hud != null)
        {
            LevelManager.HandleFailure(this.racecar.Index, Lidar.collisionFailureMessage);
        }
    }

    /// <summary>
    /// Take a sample at the current orientation.
    /// </summary>
    /// <returns>The distance (in cm) of the object directly in view of the LIDAR.</returns>
    private float TakeSample()
    {
        if (Physics.Raycast(this.transform.position, this.transform.forward, out RaycastHit raycastHit, Lidar.maxRange, Lidar.IgnoreUIMask))
        {
            float distance = Settings.IsRealism 
                ? raycastHit.distance * NormalDist.Random(1, Lidar.averageErrorFactor)
                : raycastHit.distance;
            return distance > Lidar.minRange ? distance * 10 : Lidar.minCode;
        }

        return Lidar.maxCode;
    }
}
