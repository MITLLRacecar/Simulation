using UnityEngine;

/// <summary>
/// An object which resets a racecar back to the previous checkpoint on collision.
/// </summary>
public class ResetOnCollide : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Racecar racecar = other.GetComponentInParent<Racecar>();
        if (racecar != null)
        {
            LevelManager.ResetCar(racecar.Index);
        }
    }
}
