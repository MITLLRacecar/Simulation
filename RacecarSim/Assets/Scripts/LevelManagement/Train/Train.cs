using UnityEngine;

/// <summary>
/// A single "chunk" of a train.
/// </summary>
public class Train : MonoBehaviour
{
    /// <summary>
    /// The length of a single train chunk.
    /// </summary>
    public const float Length = 4;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<TrainExit>() != null)
        {
            Destroy(this.gameObject);
        }
    }
}
