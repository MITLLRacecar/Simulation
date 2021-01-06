using UnityEngine;

/// <summary>
/// A distance cone which completes an autograder task when the car parks the correct distance away.
/// </summary>
public class DistanceConeObjective : DistanceCone
{
    #region Set In Unity Editor
    /// <summary>
    /// The distance which the car should park from the cone in cm. 
    /// </summary>
    [SerializeField]
    private float goalDistance = 30;

    /// <summary>
    /// The maximum amount the car can miss the goal distance in cm and still complete the objective.
    /// </summary>
    [SerializeField]
    private float allowableDistanceError = 2;
    #endregion

    protected override void Update()
    {
        base.Update();

        if (Mathf.Abs(this.Distance - this.goalDistance) < this.allowableDistanceError)
        {
            this.text.color = Color.green;
            if (LevelManager.GetCar().Physics.LinearVelocity.magnitude < Constants.MaxStopSeed)
            {
                // The autograder task must be stored as a separate script due to the inheritance structure
                AutograderManager.CompleteTask(this.GetComponent<AutograderTask>());
            }
        }
        else
        {
            this.text.color = Color.white;
        }
    }
}
