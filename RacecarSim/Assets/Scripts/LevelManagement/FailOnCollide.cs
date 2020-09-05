using UnityEngine;

/// <summary>
/// Causes the player to fail if they collide with this object.
/// </summary>
public class FailOnCollide : MonoBehaviour
{
    /// <summary>
    /// The failure message to show the player on collision.
    /// </summary>
    private const string failureMessage = "You hit an object you were not supposed to hit";

    private void OnCollisionEnter(Collision collision)
    {
        // If the collider is a player, cause the player to fail
        Racecar player = collision.collider.GetComponentInParent<Racecar>();
        if (player != null)
        {
            LevelManager.HandleFailure(player.Index, FailOnCollide.failureMessage);
        }
    }
}
