using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages an intermediate scene used to avoid errors when reloading a level.
/// </summary>
public class ReloadBuffer : MonoBehaviour
{
    #region Constants
    /// <summary>
    /// The build index of the ReloadBuffer scene.
    /// </summary>
    public const int BuildIndex = 1;

    /// <summary>
    /// The time (in seconds) to wait in the reload buffer.
    /// </summary>
    private const float waitTime = 0.5f;
    #endregion

    /// <summary>
    /// The build index of the scene to reload.
    /// </summary>
    public static int BuildIndexToReload = 0;

    /// <summary>
    /// The counter used to track time spent in the reload buffer.
    /// </summary>
    private float counter;

    private void Start()
    {
        this.counter = ReloadBuffer.waitTime;
    }

    private void Update()
    {
        this.counter -= Time.deltaTime;
        if (this.counter <= 0)
        {
            SceneManager.LoadScene(ReloadBuffer.BuildIndexToReload, LoadSceneMode.Single);
        }
    }
}
