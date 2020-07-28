using UnityEngine;

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
