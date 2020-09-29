using System;
using UnityEngine;

/// <summary>
/// Encapsulates all of the appearance customization options for a car.
/// </summary>
[Serializable]
public class CarCustomization
{
    /// <summary>
    /// The color of the front half of the color.
    /// </summary>
    public SerializableColor FrontColor;

    /// <summary>
    /// True if the front half of the car should be metallic.
    /// </summary>
    public bool IsFrontShiny;

    /// <summary>
    /// The color of the back half of the car.
    /// </summary>
    public SerializableColor BackColor;

    /// <summary>
    /// True if the back half of the car should be metallic.
    /// </summary>
    public bool IsBackShiny;

    /// <summary>
    /// Creates a customization containing a single matte color.
    /// </summary>
    /// <param name="mainColor">The color applied to the front and back of the car.</param>
    public CarCustomization(Color mainColor)
    {
        this.FrontColor = new SerializableColor(mainColor);
        this.IsFrontShiny = false;
        this.BackColor = new SerializableColor(mainColor);
        this.IsBackShiny = false;
    }
}
