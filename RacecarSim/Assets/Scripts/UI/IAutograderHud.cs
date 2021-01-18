/// <summary>
/// The HUD operations needed to show real-time information about the autograder.
/// </summary>
public interface IAutograderHud
{
    /// <summary>
    /// Shows information about the autograder level currently being run.
    /// </summary>
    /// <param name="levelIndex">The 0-based index of the autograder level for the lab.</param>
    /// <param name="title">The title of the autograder level.</param>
    /// <param name="description">The description of what is being tested in this autograder level.</param>
    void SetLevelInfo(int levelIndex, string title, string description);

    /// <summary>
    /// Shows the score for the current autograder level.
    /// </summary>
    /// <param name="score">The total points which the user has earned so far.</param>
    /// <param name="maxScore">The maximum number of points which the user can earn in this level.</param>
    void UpdateScore(float score, float maxScore);

    /// <summary>
    /// Shows the time spent on the current autograder level.
    /// </summary>
    /// <param name="time">The number of seconds for which the user's program has been running in the current level.</param>
    /// <param name="timeLimit">The number of seconds which the user is allowed to spend on the current level.</param>
    void UpdateTime(float time, float timeLimit);

    /// <summary>
    /// Shows the maximum time allowed for the autograder level.
    /// </summary>
    /// <param name="maxTime">The maximum time in seconds which the user can spend on the current level.</param>
    void SetMaxTime(float maxTime);

    /// <summary>
    /// Shows the current completion time score bonus.
    /// </summary>
    /// <param name="maxTime">The bonus points the user would receive if they completed the level right now.</param>
    /// <param name="bonus">The maximum time in seconds which the user can spend and still receive the current bonus.</param>
    /// <param name="isLastBracket">True if this is the last time bracket.</param>
    void SetTimeBonus(float maxTime, float bonus, bool isLastBracket);
}
