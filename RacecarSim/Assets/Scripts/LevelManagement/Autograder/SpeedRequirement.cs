using UnityEngine;

/// <summary>
/// An autograder task in which the user must reach a particular speed.
/// </summary>
public class SpeedRequirement : AutograderTask
{
    #region Set in Unity Editor
    /// <summary>
    /// The speed in m/s which the player must reach to complete this task.
    /// </summary>
    [SerializeField]
    private float speed;
    #endregion

    private void Update()
    {
        if (LevelManager.GetCar().Physics.LinearVelocity.magnitude > this.speed)
        {
            AutograderManager.CompleteTask(this);
        }
    }
}
