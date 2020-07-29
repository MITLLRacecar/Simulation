using UnityEngine;

/// <summary>
/// Fails the player if the pass on the incorrect side of a slalom cone.
/// </summary>
public class SlalomCone : MonoBehaviour
{
    #region Set in Unity Editor
    /// <summary>
    /// True if the cone is red.
    /// </summary>
    [SerializeField]
    private bool isRed;

    /// <summary>
    /// True if the player should incur a time penalty if they pass the cone on the wrong side.
    /// </summary>
    [SerializeField]
    private bool isTimePenalty;

    /// <summary>
    /// True if the player should be disqualified if they pass the cone on the wrong side.
    /// </summary>
    [SerializeField]
    private bool isDisqualify;
    #endregion

    /// <summary>
    /// The failure message to show if the player crosses the incorrect side of a red cone.
    /// </summary>
    private const string redMessage = "You must pass on the right side of red slalom cones.";

    /// <summary>
    /// The failure message to show if the player crosses the incorrect side of a blue cone.
    /// </summary>
    private const string blueMessage = "You must pass on the left side of blue slalom cones.";

    private const float timePenalty = 5;

    private void OnTriggerEnter(Collider other)
    {
        // If the collider is a player, cause the player to fail
        Racecar player = other.GetComponentInParent<Racecar>();
        if (player != null)
        {
            if (this.isTimePenalty)
            {
                VariableManager.AddPenalty(SlalomCone.timePenalty);
                Destroy(this.gameObject);
            }
            else if (this.isDisqualify)
            {
                player.Hud.ShowFailureMessage(isRed ? SlalomCone.redMessage : SlalomCone.blueMessage);
            }
        }
    }
}
