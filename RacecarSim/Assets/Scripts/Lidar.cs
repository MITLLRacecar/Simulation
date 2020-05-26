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

    void Start()
    {
        
    }

    void FixedUpdate()
    {
        int lastSample = (curSample + Mathf.RoundToInt(Lidar.samplesPerSecond * Time.deltaTime)) % numSamples;

        while (curSample != lastSample)
        {
            this.transform.rotation = Quaternion.Euler(0, 360 * curSample / Lidar.numSamples, 0);
            this.samples[curSample] = TakeSample();
            curSample = (curSample + 1) % numSamples;
        }
    }

    private float TakeSample()
    {
        RaycastHit raycastHit;
        if(Physics.Raycast(this.transform.position, this.transform.forward, out raycastHit, Lidar.maxRange))
        {
            return raycastHit.distance > Lidar.minRange ? raycastHit.distance * 100 : Lidar.minCode;
        }

        return Lidar.maxCode;
    }

    public Texture2D CreateMap()
    {
        
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
