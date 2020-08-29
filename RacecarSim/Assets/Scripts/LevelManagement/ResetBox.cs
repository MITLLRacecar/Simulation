using UnityEngine;

/// <summary>
/// An object which resets a racecar back to the previous checkpoint on collision.
/// </summary>
public class ResetBox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Racecar racecar = other.GetComponentInParent<Racecar>();
        if (racecar != null)
        {
            racecar.ResetToCheckpoint();
        }
    }
}
