using UnityEngine;

/// <summary>
/// The base class from which all racecar modules inherit.
/// 
/// Each module handles a particular aspect of the car's hardware, such as the camera, controller, etc.
/// </summary>
public abstract class RacecarModule : MonoBehaviour
{
    /// <summary>
    /// The parent racecar to which this module belongs.
    /// </summary>
    protected Racecar racecar;

    protected virtual void Awake()
    {
        this.FindParent();
    }

    /// <summary>
    /// Set the pointer to the parent Racecar object.
    /// </summary>
    protected virtual void FindParent()
    {
        this.racecar = this.GetComponent<Racecar>();
    }
}
