using System;
using UnityEngine;

/// <summary>
/// A color representation which can be serialized to binary.
/// </summary>
[Serializable]
public class SerializableColor
{
    /// <summary>
    /// Red component of the color, on the range [0, 1].
    /// </summary>
    public float r;

    /// <summary>
    /// Green component of the color, on the range [0, 1].
    /// </summary>
    public float g;

    /// <summary>
    /// Blue component of the color, on the range [0, 1].
    /// </summary>
    public float b;

    /// <summary>
    /// The color represented as the Unity Color class.
    /// </summary>
    public Color Color
    {
        get
        {
            return new Color(r, g, b);
        }
    }

    /// <summary>
    /// Creates a serializable color with the same RGB values as a Unity-style color.
    /// </summary>
    /// <param name="color">The Unity-style color to copy.</param>
    public SerializableColor(Color color)
    {
        this.r = color.r;
        this.g = color.g;
        this.b = color.b;
    }

    /// <summary>
    /// Creates a serializable color with the specified RGB values.
    /// </summary>
    /// <param name="r">The red component of the color, on the range [0, 1]</param>
    /// <param name="g">The green component of the color, on the range [0, 1]</param>
    /// <param name="b">The blue component of the color, on the range [0, 1]</param>
    public SerializableColor(float r, float g, float b)
    {
        this.r = r;
        this.g = g;
        this.b = b;
    }
}
