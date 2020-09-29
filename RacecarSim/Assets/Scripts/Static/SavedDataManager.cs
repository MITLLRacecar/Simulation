using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

/// <summary>
/// Manages loading, saving, and accessing saved game data.
/// </summary>
public static class SavedDataManager
{
    #region Constants
    /// <summary>
    /// The path of the file containing the saved game data.
    /// </summary>
    private static readonly string saveFilePath = Application.persistentDataPath + "/GameData.dat";
    #endregion

    /// <summary>
    /// The current game data loaded in memory.
    /// </summary>
    public static SavedData Data { get; private set; } = null;

    /// <summary>
    /// Saves the current game data to disk.
    /// </summary>
    public static void Save()
    {
        if (SavedDataManager.Data != null)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using (FileStream file = File.Create(SavedDataManager.saveFilePath))
            {
                binaryFormatter.Serialize(file, SavedDataManager.Data);
            }
        }
        else
        {
            Debug.LogError("Attempted to save data before any data was loaded.");
        }
    }

    /// <summary>
    /// Loads the saved game data on disk into memory, or creates default data if no saved data is found.
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Called by Unity")]
    private static void Load()
    {
        if (File.Exists(SavedDataManager.saveFilePath))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using (FileStream file = File.OpenRead(SavedDataManager.saveFilePath))
            {
                try
                {
                    SavedDataManager.Data = (SavedData)binaryFormatter.Deserialize(file);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Unable to load saved data, so using default data instead. Exception: [{e}]");
                    SavedDataManager.Data = SavedData.Default;
                }
            }
        }
        else
        {
            SavedDataManager.Data = SavedData.Default;
        }
    }
}
