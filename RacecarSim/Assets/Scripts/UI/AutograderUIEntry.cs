using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages an entry displaying the performance on a single autograder level.
/// </summary>
public class AutograderUIEntry : MonoBehaviour
{
    #region Constants
    /// <summary>
    /// The color shown for a score which is between 0 and full credit, not inclusive.
    /// </summary>
    private static readonly Color partialCreditColor = new Color(1, 0.5f, 0);

    /// <summary>
    /// The color shown for a score which is greater than full credit.
    /// </summary>
    private static readonly Color extraCreditColor = new Color(0.75f, 0, 1);
    #endregion

    #region Public Interface
    /// <summary>
    /// Initializes the entry with score and time information.
    /// </summary>
    /// <param name="levelInfo">Information about the level.</param>
    /// <param name="bestTimeInfo">Information about the user's performance in the level.</param>
    public void SetInfo(AutograderLevelInfo levelInfo, AutograderLevelScore levelScore)
    {
        this.texts[(int)Texts.Name].text = levelInfo.IsRequired ? $"*{levelInfo.Title}" : levelInfo.Title;

        if (levelScore != null)
        {
            this.texts[(int)Texts.Score].text = $"{levelScore.Score:F2}/{levelInfo.MaxPoints:F2}";
            this.texts[(int)Texts.Time].text = levelScore.Time.ToString("F2");

            if (levelInfo.MaxPoints > 0 && levelScore.Score == 0)
            {
                // No credit
                this.texts[(int)Texts.Score].color = Color.red;
            }
            else if (levelScore.Score > levelInfo.MaxPoints)
            {
                // Extra credit
                this.texts[(int)Texts.Score].color = AutograderUIEntry.extraCreditColor;
            }
            else if (levelScore.Score != levelInfo.MaxPoints)
            {
                // Partial credit
                this.texts[(int)Texts.Score].color = AutograderUIEntry.partialCreditColor;
            }
        }
        else
        {
            this.texts[(int)Texts.Score].text = $"--/{levelInfo.MaxPoints:F2}";
            this.texts[(int)Texts.Score].color = Color.red;
            this.texts[(int)Texts.Time].text = "--";
        }
    }
    #endregion

    /// <summary>
    /// The mutable text fields of the UI entry, with values corresponding to the index in texts.
    /// </summary>
    private enum Texts
    {
        Name = 0,
        Score = 1,
        Time = 2
    }

    /// <summary>
    /// The mutable text fields in the UI entry.
    /// </summary>
    private Text[] texts;

    private void Awake()
    {
        this.texts = this.GetComponentsInChildren<Text>();
    }
}
