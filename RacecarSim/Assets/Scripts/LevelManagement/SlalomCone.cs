using UnityEngine;

/// <summary>
/// Fails the player if the pass on the incorrect side of a slalom cone.
/// </summary>
public class SlalomCone : MonoBehaviour
{
    #region Set in Unity Editor
    /// <summary>
    /// True if the cone is red, meaning that it must be passed on the right side.
    /// </summary>
    [SerializeField]
    private bool isRed;

    /// <summary>
    /// The penalty which a player receive if they pass the cone on the incorrect side.
    /// </summary>
    [SerializeField]
    private SlalomConePenalty penalty = SlalomConePenalty.Disqualify;

    /// <summary>
    /// The time penalty (in seconds) which a player receives for passing the cone on the incorrect side, if penalty is set to TimePenalty.
    /// </summary>
    [SerializeField]
    private float timePenalty = 5;
    #endregion

    /// <summary>
    /// The types of penalties which players can receive when they pass a slalom cone on the incorrect side.
    /// </summary>
    private enum SlalomConePenalty
    {
        Disqualify,
        Reset,
        TimePenalty
    }

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
            switch (this.penalty)
            {
                case SlalomConePenalty.Disqualify:
                    LevelManager.HandleFailure(player.Index, isRed ? SlalomCone.redMessage : SlalomCone.blueMessage);
                    break;
                case SlalomConePenalty.Reset:
                    LevelManager.ResetCar(player.Index);
                    break;
                case SlalomConePenalty.TimePenalty:
                    LevelManager.AddTimePenalty(player.Index, this.timePenalty);
                    Destroy(this.gameObject);
                    break;
            }
        }
    }
}
