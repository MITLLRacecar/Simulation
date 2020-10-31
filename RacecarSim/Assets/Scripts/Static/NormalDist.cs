using System;
using UnityEngine;

public static class NormalDist
{
    /// <summary>
    /// Takes a random sample from a normal distribution.
    /// </summary>
    /// <param name="mean">The mean of the distribution.</param>
    /// <param name="sdev">The standard deviation of the distribution.</param>
    /// <returns>A random number sampled from the specified distribution.</returns>
    public static float Random(float mean = 0, float sdev = 1)
    {
        float x = UnityEngine.Random.value;

        if (x >= 0.5)
        {
            return mean + sdev * Mathf.Sqrt(-1.57079632679f * Mathf.Log(1 - Mathf.Pow(2 * x - 1, 2), Mathf.Exp(1)));
        }
        else
        {
            return mean - sdev * Mathf.Sqrt(-1.57079632679f * Mathf.Log(1 - Mathf.Pow(2 * x - 1, 2), Mathf.Exp(1)));
        }
    }

    static NormalDist()
    {
        UnityEngine.Random.InitState(DateTime.Now.Millisecond);
    }
}
