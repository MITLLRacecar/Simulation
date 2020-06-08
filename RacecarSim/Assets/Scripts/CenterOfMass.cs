using UnityEngine;

/// <summary>
/// Sets the center of mass of an object.
/// </summary>
public class CenterOfMass : MonoBehaviour
{
    /// <summary>
    /// Center of mass, set in Unity editor.
    /// </summary>
    public Vector3 Com;

    void Start()
    {
        this.GetComponent<Rigidbody>().centerOfMass = this.Com;
    }
}
