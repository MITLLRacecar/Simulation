using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLine : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Racecar player = other.attachedRigidbody.GetComponent<Racecar>();
        if (player != null)
        {
            player.HandleFinish();
        }
    }
}
