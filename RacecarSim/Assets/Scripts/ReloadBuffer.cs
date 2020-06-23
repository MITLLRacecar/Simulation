using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages an intermediate scene used to avoid errors when reloading a level.
/// </summary>
public class ReloadBuffer : MonoBehaviour
{
    /// <summary>
    /// The build index of the scene to reload.
    /// </summary>
    public static int BuildIndexToReload = 0;

    /// <summary>
    /// The build index of the ReloadBuffer scene.
    /// </summary>
    public const int BuildIndex = 1;

    void Start()
    {
        SceneManager.LoadScene(ReloadBuffer.BuildIndexToReload, LoadSceneMode.Single);
    }
}
