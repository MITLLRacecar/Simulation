using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Manages the dialog shown when the user has not set their username.
/// </summary>
public class NoUsernameUI : MonoBehaviour
{
    #region PublicInterface
    public void Initialize(int buildIndex)
    {
        this.buildIndex = buildIndex;
        this.input.text = Settings.Username;
        this.saveButton.interactable = false;
    }

    /// <summary>
    /// Handles when the keep default button was pressed.
    /// </summary>
    public void KeepDefault()
    {
        SceneManager.LoadScene(this.buildIndex, LoadSceneMode.Single);
    }

    /// <summary>
    /// Handles when the Save button was pressed.
    /// </summary>
    public void Save()
    {
        Settings.Username = this.input.text;
        Settings.SaveSettings();
        this.KeepDefault();
    }

    /// <summary>
    /// Handles when the input field was changed.
    /// </summary>
    public void InputChanged()
    {
        this.saveButton.interactable = this.input.text != Settings.DefaultUsername;
    }
    #endregion

    /// <summary>
    /// The build index of the level to load after the user closes this dialog.
    /// </summary>
    public int buildIndex { private get; set; }

    /// <summary>
    /// The input field in which the user can enter their username.
    /// </summary>
    private InputField input;

    /// <summary>
    /// The button which allows the user to save their new username.
    /// </summary>
    private Button saveButton;

    private void Awake()
    {
        this.input = this.GetComponentInChildren<InputField>();
        this.saveButton = this.GetComponentsInChildren<Button>()[1];
    }
}
