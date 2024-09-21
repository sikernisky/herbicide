using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.Assertions;

/// <summary>
/// Manages saving and loading PlayerData to and from a file.
/// </summary>
public class SaveLoadManager : MonoBehaviour
{
    #region Fields 

    /// <summary>
    /// Reference to the SaveLoadManager singleton.
    /// </summary>
    private static SaveLoadManager instance;

    /// <summary>
    /// The PlayerData object that we are currently using.
    /// </summary>
    private PlayerData currentLoad;

    /// <summary>
    /// The path to the file where we save the player data.
    /// </summary>
    private string SAVE_PATH => Application.persistentDataPath + "/playerData.sav";

    #endregion

    #region Methods

    /// <summary>
    /// Finds and sets the SaveLoadManager singleton for a level. Loads
    /// the PlayerData from the computer.
    /// </summary>
    /// <param name="levelController">The LevelController singleton.</param>
    public static void SetSingleton(LevelController levelController)
    {
        if (levelController == null) return;

        SaveLoadManager[] saveLoadManagers = FindObjectsOfType<SaveLoadManager>();
        Assert.IsNotNull(saveLoadManagers, "Array of SettingsControllers is null.");
        Assert.AreEqual(1, saveLoadManagers.Length);
        instance = saveLoadManagers[0];
        instance.Load();
    }

    /// <summary>
    /// Saves the current PlayerData to the save path.
    /// </summary>
    public static void Save()
    {
        if(instance.currentLoad == null) return;
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(instance.SAVE_PATH, FileMode.Create);
        formatter.Serialize(stream, instance.currentLoad);
        stream.Close();
    }

    /// <summary>
    /// Loads the PlayerData from the given path. If not found, creates a new PlayerData.
    /// </summary>
    /// <returns>the loaded PlayerData object, or a new PlayerData object if
    /// there is none on the computer. </returns>
    private void Load()
    {
        if (File.Exists(SAVE_PATH))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(SAVE_PATH, FileMode.Open);
            PlayerData data = formatter.Deserialize(stream) as PlayerData;
            stream.Close();
            currentLoad = data;
        }
        else currentLoad = new PlayerData();
        SetLevel(0); //temp
    }

    /// <summary>
    /// Sets the level to the given level. This is 0-indexed, so the first level is level 0.
    /// </summary>
    /// <param name="level">the level to set to.</param>
    public static void SetLevel(int level)
    {
        if(level < 0) return;
        instance.currentLoad.furthestLevel = level;
    }

    /// <summary>
    /// Returns the level the player is currently on. This is
    /// 0-indexed, so the first level is level 0.
    /// </summary>
    /// <returns>the level the player is currently on.</returns>
    public static int GetLevel() => instance.currentLoad.furthestLevel;

    #endregion
}
