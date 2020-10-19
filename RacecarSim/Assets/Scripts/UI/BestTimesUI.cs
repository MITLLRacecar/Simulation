using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the best times pane of the main menu.
/// </summary>
public class BestTimesUI : MonoBehaviour
{
    #region Set in Unity Editor
    /// <summary>
    /// The UI template which displays the best time information for a single level.
    /// </summary>
    [SerializeField]
    private GameObject bestTimeEntry;

    /// <summary>
    /// The Canvas object which contains all best time entries.
    /// </summary>
    [SerializeField]
    private GameObject bestTimesContainer;
    #endregion

    #region Constants
    /// <summary>
    /// The width of a best time entry divided by the space between two best time entries.
    /// </summary>
    private const int entryWidthToBufferRatio = 4;

    /// <summary>
    /// The fraction of the container that a single best time entry should take up.
    /// </summary>
    private const float entryHeight = 0.12f;

    /// <summary>
    /// The fraction of the container that a best time entry should leave unoccupied on the left and right.
    /// </summary>
    private const float entryXBuffer = 0.02f;
    #endregion

    #region Public Interface
    /// <summary>
    /// Update objects displaying best times.
    /// </summary>
    public void UpdateEntries()
    {
        if (this.bestTimeEntries.Length != SavedDataManager.Data.BestTimes.Length)
        {
            Debug.LogError("The existing best time UI entries do not align with the best times in the saved data. Deleting the existing UI entries.");
            foreach(BestTimeUIEntry uiEntry in this.bestTimeEntries)
            {
                GameObject.Destroy(uiEntry.gameObject);
            }
            this.CreateBlankEntries();
        }

        for(int i = 0; i < this.bestTimeEntries.Length; i++)
        {
            this.bestTimeEntries[i].SetInfo(LevelInfo.WinableLevels[i], SavedDataManager.Data.BestTimes[i]);
        }
    }

    /// <summary>
    /// Return to the main menu.
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
        SavedDataManager.Data.ClearBestTimes();
        SavedDataManager.Save();
        this.UpdateEntries();
    }
    #endregion

    /// <summary>
    /// The UI elements for each best time entry.
    /// </summary>
    private BestTimeUIEntry[] bestTimeEntries;

    private void Start()
    {
        // Create best time entries on the canvas
        this.CreateBlankEntries();
        this.UpdateEntries();
    }

    /// <summary>
    /// Creates a UI element for each best time entry.
    /// </summary>
    private void CreateBlankEntries()
    {
        this.bestTimeEntries = new BestTimeUIEntry[LevelInfo.WinableLevels.Count];

        // Set anchor points of container
        RectTransform container = (RectTransform)this.bestTimesContainer.transform;
        container.anchorMax = new Vector2(1, 1);
        container.anchorMin = new Vector2(0, 1 - BestTimesUI.entryHeight * this.bestTimeEntries.Length);
        container.anchoredPosition = new Vector2(0, 0);
        container.sizeDelta = new Vector2(0, 0);

        float entryYBuffer = 1.0f / (this.bestTimeEntries.Length * (BestTimesUI.entryWidthToBufferRatio + 1) + 2);
        float entryHeight = entryYBuffer * BestTimesUI.entryWidthToBufferRatio;
        float anchorY = 1 - entryYBuffer;

        // TODO: Handle float rounding errors
        for (int i = 0; i < this.bestTimeEntries.Length; i++)
        {
            GameObject uiEntry = GameObject.Instantiate(this.bestTimeEntry, Vector3.zero, Quaternion.identity);

            // Set uiEntry's anchor points inside of the container
            uiEntry.transform.SetParent(this.bestTimesContainer.transform);
            RectTransform rect = uiEntry.GetComponent<RectTransform>();
            rect.anchorMax = new Vector2(1 - BestTimesUI.entryXBuffer, anchorY);
            anchorY -= entryHeight;
            rect.anchorMin = new Vector2(BestTimesUI.entryXBuffer, anchorY);
            anchorY -= entryYBuffer;

            // Size exactly to the anchor points
            rect.anchoredPosition = new Vector2(0, 0);
            rect.sizeDelta = new Vector2(0, 0);

            this.bestTimeEntries[i] = uiEntry.GetComponent<BestTimeUIEntry>();
        }
    }
}
