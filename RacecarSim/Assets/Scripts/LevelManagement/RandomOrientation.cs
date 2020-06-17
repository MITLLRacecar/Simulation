using UnityEngine;

/// <summary>
/// Randomly adjusts the position and rotation of an object on Start.
/// </summary>
public class RandomOrientation : MonoBehaviour
{
    #region Set in Unity Editor
    /// <summary>
    /// The maximum amount the object can translate along each axis (in dm).
    /// </summary>
    [SerializeField]
    private Vector3 MaxTranslation = Vector3.zero;

    /// <summary>
    /// The maximum amount the object can rotate about each axis (in degrees).
    /// </summary>
    [SerializeField]
    private Vector3 MaxRotation = Vector3.zero;
    #endregion

    void Start()
    {
        this.transform.position += new Vector3(
            Random.Range(-this.MaxTranslation.x, this.MaxTranslation.x),
            Random.Range(-this.MaxTranslation.y, this.MaxTranslation.y),
            Random.Range(-this.MaxTranslation.z, this.MaxTranslation.z));

        this.transform.Rotate(
            Random.Range(-this.MaxRotation.x, this.MaxRotation.x),
            Random.Range(-this.MaxRotation.y, this.MaxRotation.y),
            Random.Range(-this.MaxRotation.z, this.MaxRotation.z));
    }
}
