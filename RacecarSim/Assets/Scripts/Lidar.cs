using System;
using UnityEngine;

public class Lidar : MonoBehaviour
{
    #region Constants
    public const int NumSamples = 720;

    private const int motorFrequency = 6;
    private const int samplesPerSecond = Lidar.NumSamples * Lidar.motorFrequency;

    private const float minRange = 0.12f;
    private const float minCode = 0.0f;
    private const float maxRange = 100;
    private const float maxCode = 0.0f;

    private const float visualizationRange = 50;
    #endregion

    // samples are stored in cm
    public float[] Samples { get; private set; }

    private int curSample = 0;

    private Transform carTransform;

    private void Start()
    {
        this.Samples = new float[Lidar.NumSamples];
        this.carTransform = this.GetComponentInParent<Transform>();
    }

    void FixedUpdate()
    {
        int lastSample = (curSample + Mathf.RoundToInt(Lidar.samplesPerSecond * Time.deltaTime)) % NumSamples;

        while (curSample != lastSample)
        {
            this.transform.localRotation = Quaternion.Euler(0, curSample * 360.0f / Lidar.NumSamples, 0);
            this.Samples[curSample] = TakeSample();
            curSample = (curSample + 1) % NumSamples;
        }
    }

    private float TakeSample()
    {
        if(Physics.Raycast(this.transform.position, this.transform.forward, out RaycastHit raycastHit, Lidar.maxRange))
        {
            return raycastHit.distance > Lidar.minRange ? raycastHit.distance * 10 : Lidar.minCode;
        }

        return Lidar.maxCode;
    }

    public void VisualizeLidar(Texture2D texture)
    {
        Unity.Collections.NativeArray<Color32> rawData = texture.GetRawTextureData<Color32>();

        // Set background color
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

        // Render samples
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
}
