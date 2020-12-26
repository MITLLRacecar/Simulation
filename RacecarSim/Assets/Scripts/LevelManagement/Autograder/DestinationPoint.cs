using UnityEngine;

/// <summary>
/// An autograder task in which the user must drive to a specific point.
/// </summary>
public class DestinationPoint : AutograderTask
{
    private void OnTriggerEnter(Collider other)
    {
        Racecar racecar = other.GetComponentInParent<Racecar>();
        if (racecar != null)
        {
            AutograderManager.CompleteTask(this);
        }
    }
}