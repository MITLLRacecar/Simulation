using System;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField]
    private VariableManager.KeyTime index;

    private void Start()
    {
        VariableManager.SetCheckpoint(this.index.GetHashCode() - 2, new Tuple<Vector3, Quaternion>(this.transform.position, this.transform.rotation));
    }

    private void OnTriggerEnter(Collider other)
    {
        Racecar player = other.attachedRigidbody.GetComponent<Racecar>();
        if (player != null)
        {
            VariableManager.SetKeyTime(index, Time.time);
        }
    }
}
