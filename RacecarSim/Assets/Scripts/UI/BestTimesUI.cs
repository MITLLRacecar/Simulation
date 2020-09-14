using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the best times pane of the main menu.
/// </summary>
public class BestTimesUI : MonoBehaviour
{
    #region Public Interface
    /// <summary>
    /// Update text objects displaying best times.
    /// </summary>
    public void UpdateTexts()
    {
        this.texts[(int)Texts.Names].text = BestTimes.GetFormattedNames();
        this.texts[(int)Texts.Times].text = BestTimes.GetFormattedTimes();
    }

    /// <summary>
    /// Return to the main menu
    /// </summary>
    public void Return()
    {
        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// Clear all recorded best times.
    /// </summary>
    public void Clear()
    {
        BestTimes.Clear();
        this.UpdateTexts();
    }
    #endregion

    /// <summary>
    /// The text objects in the best times pane.
    /// </summary>
    private enum Texts
    {
        Names = 1,
        Times = 2
    }

    /// <summary>
    /// The text objects in the best times pane.
    /// </summary>
    private Text[] texts;

    private void Awake()
    {
        this.texts = this.GetComponentsInChildren<Text>();
    }
}
