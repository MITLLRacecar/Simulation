using UnityEngine;

public class Lidar : MonoBehaviour
{
    #region Constants
    private const int numSamples = 720;
    private const int motorFrequency = 6;
    private const int samplesPerSecond = Lidar.numSamples * Lidar.motorFrequency;

    private const float minRange = 0.12f;
    private const float minCode = 0.0f;
    private const float maxRange = 10;
    private const float maxCode = 0.0f;
    #endregion

    // samples are stored in cm
    private float[] samples = new float[Lidar.numSamples];
    private int curSample = 0;

    private Transform carTransform;

    void Start()
    {
        
    }

    void FixedUpdate()
    {
        int lastSample = (curSample + Mathf.RoundToInt(Lidar.samplesPerSecond * Time.deltaTime)) % numSamples;

        while (curSample != lastSample)
        {
            this.transform.rotation = Quaternion.Euler(0, this.carTransform.rotation.eulerAngles.y + 360 * curSample / Lidar.numSamples, 0);
            this.samples[curSample] = TakeSample();
            curSample = (curSample + 1) % numSamples;
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

    public void UpdateMap(Texture2D map)
    {
        Unity.Collections.NativeArray<Color32> rawData = map.GetRawTextureData<Color32>();

        // Set background color
        for (int i = 0; i < rawData.Length; i++)
        {
            rawData[i] = Hud.SensorBackgroundColor;
        }

        // Render samples
        Vector2 center = new Vector2(map.width / 2, map.height / 2);
        float length = Mathf.Min(map.width / 2.0f, map.height / 2.0f);
        for (int i = 0; i < this.samples.Length; i++)
        {
            if (this.samples[i] != Lidar.minCode && this.samples[i] != Lidar.maxCode)
            {
                float angle = 2 * Mathf.PI * i / Lidar.numSamples;
                Vector2 point = center + samples[i] / 100 / Lidar.maxRange * length * new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
                rawData[(int)point.y * map.width + (int)point.x] = Color.red;
            }
        }

        map.Apply();
    }

    public void SetCarTransform(Transform transform)
    {
        this.carTransform = transform;
    }

    #region Python Interface
    public int get_length(float? timeout=null)
    {
        return Lidar.numSamples;
    }

    public float[] get_ranges(float? timeout=null)
    {
        return this.samples;
    }
    #endregion
}
