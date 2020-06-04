using UnityEngine;

public class Lidar : MonoBehaviour
{
    #region Constants
    public const int NumSamples = 720;

    private const int motorFrequency = 6;
    private const int samplesPerSecond = Lidar.NumSamples * Lidar.motorFrequency;

    private const float minRange = 0.12f;
    private const float minCode = 0.0f;
    private const float maxRange = 10;
    private const float maxCode = 0.0f;
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
            this.transform.localRotation = Quaternion.Euler(0, (float)curSample / Lidar.NumSamples, 0);
            this.Samples[curSample] = TakeSample();
            curSample = (curSample + 1) % NumSamples;
        }
    }

    private float TakeSample()
    {
        if(Physics.Raycast(this.transform.position, this.transform.forward, out RaycastHit raycastHit, Lidar.maxRange))
        {
            return raycastHit.distance > Lidar.minRange ? raycastHit.distance * 100 : Lidar.minCode;
        }

        return Lidar.maxCode;
    }

    public void VisualizeLidar(Texture2D texture)
    {
        Unity.Collections.NativeArray<Color32> rawData = texture.GetRawTextureData<Color32>();

        // Set background color
        for (int i = 0; i < rawData.Length; i++)
        {
            rawData[i] = Hud.SensorBackgroundColor;
        }

        // Render samples
        Vector2 center = new Vector2(texture.width / 2, texture.height / 2);
        float length = Mathf.Min(texture.width / 2.0f, texture.height / 2.0f);
        for (int i = 0; i < this.Samples.Length; i++)
        {
            if (this.Samples[i] != Lidar.minCode && this.Samples[i] != Lidar.maxCode)
            {
                float angle = 2 * Mathf.PI * i / Lidar.NumSamples;
                Vector2 point = center + this.Samples[i] / 100 / Lidar.maxRange * length * new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
                rawData[(int)point.y * texture.width + (int)point.x] = Color.red;
            }
        }

        texture.Apply();
    }
}
