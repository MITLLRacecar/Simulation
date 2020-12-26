using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Manages an entry displaying the best time for a single lab.
/// </summary>
public class BestTimeUIEntry : MonoBehaviour
{
    #region Public Interface
    /// <summary>
    /// Begins the level corresponding to the best time entry in evaluation mode.
    /// </summary>
    public void HandleBegin()
    {
        LevelManager.LevelManagerMode = LevelManagerMode.Race;
        LevelManager.NumPlayers = 1;

        LevelManager.LevelInfo = this.level;
        SceneManager.LoadScene(this.level.BuildIndex, LoadSceneMode.Single);
    }

    /// <summary>
    /// Initializes the entry with best time and level information.
    /// </summary>
    /// <param name="levelInfo">The level to which the entry corresponds.</param>
    /// <param name="bestTimeInfo">The best times with which the player completed the level.</param>
    public void SetInfo(LevelInfo levelInfo, BestTimeInfo bestTimeInfo)
    {
        this.texts[(int)Texts.LevelName].text = levelInfo.FullName;
        this.texts[(int)Texts.OverallTime].text = BestTimeUIEntry.FormatTime(bestTimeInfo.OverallTime);

        string checkpointTimes = string.Empty;
        if (bestTimeInfo.CheckpointTimes != null && bestTimeInfo.CheckpointTimes.Length > 1)
        {
            checkpointTimes = $"1) {BestTimeUIEntry.FormatTime(bestTimeInfo.CheckpointTimes[0])}";
            for (int i = 1; i < bestTimeInfo.CheckpointTimes.Length; i++)
            {
                checkpointTimes += $"    {i + 1}) {BestTimeUIEntry.FormatTime(bestTimeInfo.CheckpointTimes[i])}";
            }
        }
        this.texts[(int)Texts.Checkpoints].text = checkpointTimes;

        this.level = levelInfo;
    }
    #endregion

    /// <summary>
    /// The mutable text fields of the best time entry, with values corresponding to the index in texts.
    /// </summary>
    private enum Texts
    {
        LevelName = 0,
        OverallTime = 1,
        Checkpoints = 3
    }

    /// <summary>
    /// The mutable text fields in the best time entry.
    /// </summary>
    private Text[] texts;

    /// <summary>
    /// The level to which the entry corresponds.
    /// </summary>
    private LevelInfo level;

    private void Awake()
    {
        this.texts = this.GetComponentsInChildren<Text>();
    }

    /// <summary>
    /// Formats a time to be displayed.
    /// </summary>
    /// <param name="time">The time, in seconds</param>
    /// <returns>The time, rounded and formatted as a string.</returns>
    private static string FormatTime(float time)
    {
        return time == float.MaxValue ? "--" : time.ToString("F3");
    }
}
