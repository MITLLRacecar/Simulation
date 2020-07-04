using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the best times pane of the main menu.
/// </summary>
public class BestTimesUI : MonoBehaviour
{
    #region Public Interface
    public void UpdateTexts()
    {
        this.texts[Texts.Names.GetHashCode()].text = BestTimes.GetFormattedNames();
        this.texts[Texts.Times.GetHashCode()].text = BestTimes.GetFormattedTimes();
    }
    #endregion

    private enum Texts
    {
        Names = 1,
        Times = 2
    }

    private Text[] texts;

    private void Awake()
    {
        this.texts = this.GetComponentsInChildren<Text>();
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            this.gameObject.SetActive(false);
        }
    }
}
