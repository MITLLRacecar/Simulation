using UnityEngine;

/// <summary>
/// Fails the player if the pass on the incorrect side of a slalom cone.
/// </summary>
public class SlalomCone : MonoBehaviour
{
    /// <summary>
    /// True if the cone is red.
    /// </summary>
    [SerializeField]
    private bool isRed;

    /// <summary>
    /// The failure message to show if the player crosses the incorrect side of a red cone.
    /// </summary>
    private const string redMessage = "You must pass on the right side of red slalom cones.";

    /// <summary>
    /// The failure message to show if the player crosses the incorrect side of a blue cone.
    /// </summary>
    private const string blueMessage = "You must pass on the left side of blue slalom cones.";

    private void OnTriggerEnter(Collider other)
    {
        // If the collider is a player, cause the player to fail
        Racecar player = other.GetComponentInParent<Racecar>();
        if (player != null)
        {
            player.Hud.ShowFailureMessage(isRed ? SlalomCone.redMessage : SlalomCone.blueMessage);
        }
    }
}
