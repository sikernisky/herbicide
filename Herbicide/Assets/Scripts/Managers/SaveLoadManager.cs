using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.Assertions;
using System.Collections.Generic;
using System;
using System.Data.Common;

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
    private GameSaveData currentLoad;

    /// <summary>
    /// The path to the file where we save the player data.
    /// </summary>
    private string SAVE_PATH => Application.persistentDataPath + "/playerData.sav";

    /// <summary>
    /// The Action that is invoked when a save is requested.
    /// </summary>
    public event Action OnSaveRequested;

    /// <summary>
    /// The Action that is invoked when a load is requested.
    /// </summary>
    public event Action OnLoadRequested;

    /// <summary>
    /// true if the PlayerData has been loaded from the computer.
    /// </summary>
    private bool loaded;


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
    }

    /// <summary>
    /// Finds and sets the MainMenuController singleton for a level. Loads
    /// the PlayerData from the computer.
    /// </summary>
    /// <param name="mainMenuController">The MainMenuController singleton.</param>
    public static void SetSingleton(MainMenuController mainMenuController)
    {
        if (mainMenuController == null) return;

        SaveLoadManager[] saveLoadManagers = FindObjectsOfType<SaveLoadManager>();
        Assert.IsNotNull(saveLoadManagers, "Array of SettingsControllers is null.");
        Assert.AreEqual(1, saveLoadManagers.Length);
        instance = saveLoadManagers[0];
    }

    /// <summary>
    /// Invokes other classes' save methods so that they save to the
    /// current PlayerData. Then, saves the current PlayerData to the save path.
    /// </summary>
    public static void Save()
    {
        if(instance.currentLoad == null) return;
        instance.OnSaveRequested?.Invoke();
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(instance.SAVE_PATH, FileMode.Create);
        formatter.Serialize(stream, instance.currentLoad);
        stream.Close();
    }

    /// <summary>
    /// Loads the PlayerData from the given path. If not found, creates a new PlayerData.
    /// From this PlayerData, invokes the OnLoadRequested event so that other classes
    /// load their data too.
    /// </summary>
    /// <returns>the loaded PlayerData object, or a new PlayerData object if
    /// there is none on the computer. </returns>
    public static void Load()
    {
        if(instance.loaded) return;
        instance.currentLoad = null;

        if (File.Exists(instance.SAVE_PATH))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(instance.SAVE_PATH, FileMode.Open);
            try
            {
                GameSaveData data = formatter.Deserialize(stream) as GameSaveData;
                instance.currentLoad = data;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error loading data: {e.Message}");
                instance.currentLoad = new GameSaveData();
            }
            finally { stream.Close(); }
        }
        else instance.currentLoad = new GameSaveData();
        instance.OnLoadRequested?.Invoke();
        instance.loaded = true;
    }

    /// <summary>
    /// Wipes the current save data.
    /// </summary>
    public static void WipeCurrentSave()
    {
        instance.currentLoad = new GameSaveData(); // Clear in-memory data
        if (File.Exists(instance.SAVE_PATH)) File.Delete(instance.SAVE_PATH);
        else Debug.Log("Save file not found to delete.");
    }

    /// <summary>
    /// Subscribes a method to the OnSaveRequested event.
    /// </summary>
    /// <param name="saveMethod">the method to subscribe.</param>
    public static void SubscribeToToSaveEvent(Action saveMethod)
    {
        Assert.IsNotNull(saveMethod, "saveMethod is null.");

        instance.OnSaveRequested += saveMethod;
    }

    /// <summary>
    /// Subscribes a method to the OnLoadRequested event.
    /// </summary>
    /// <param name="loadMethod">the method to subscribe.</param>
    public static void SubscribeToToLoadEvent(Action loadMethod)
    {
        Assert.IsNotNull(loadMethod, "loadMethod is null.");

        instance.OnLoadRequested += loadMethod;
    }

    /// <summary>
    /// Sets the level to the given level. This is 0-indexed, so the first level is level 0.
    /// </summary>
    /// <param name="level">the level to set to.</param>
    public static void SaveGameLevel(int level)
    {
        Assert.IsNotNull(instance.currentLoad, "currentLoad is null.");
        if (level < 0) return;
        instance.currentLoad.furthestLevel = level;
    }

    /// <summary>
    /// Returns the level the player is currently on. This is
    /// 0-indexed, so the first level is level 0.
    /// </summary>
    /// <returns>the level the player is currently on.</returns>
    public static int GetLoadedGameLevel() 
    {
        Assert.IsNotNull(instance.currentLoad, "currentLoad is null.");
        return instance.currentLoad.furthestLevel; 
    }

    /// <summary>
    /// Saves the CollectionSaveData to the PlayerData.
    /// </summary>
    /// <param name="collectionSaveData">the saved Collection data.</param>
    public static void SaveCollection(CollectionManager collectionManager, CollectionSaveData collectionSaveData)
    {
        Assert.IsNotNull(instance.currentLoad, "currentLoad is null.");
        Assert.IsNotNull(collectionManager, "collectionManager is null.");
        Assert.IsNotNull(collectionSaveData, "modelSaveData is null.");

        instance.currentLoad.collectionSaveData = collectionSaveData;
    }

    /// <summary>
    /// Returns the CollectionSaveData from the PlayerData.
    /// </summary>
    /// <returns>the CollectionSaveData from the PlayerData.</returns>
    public static CollectionSaveData LoadCollection(CollectionManager collectionManager)
    {
        Assert.IsNotNull(instance.currentLoad, "currentLoad is null.");
        Assert.IsNotNull(collectionManager, "collectionManager is null.");

        return instance.currentLoad.collectionSaveData;
    }

    /// <summary>
    /// Saves the list of ShopSaveData to the PlayerData.
    /// </summary>
    /// <param name="shopSaveData">the saved Shop data.</param>
    public static void SaveShop(ShopManager shopManager, ShopSaveData shopSaveData)
    {
        Assert.IsNotNull(instance.currentLoad, "currentLoad is null.");
        Assert.IsNotNull(shopManager, "shopManager is null.");
        Assert.IsNotNull(shopSaveData, "modelSaveData is null.");

        instance.currentLoad.shopSaveData = shopSaveData;
    }

    /// <summary>
    /// Returns the ShopSaveData from the PlayerData.
    /// </summary>
    /// <returns>the ShopSaveData from the PlayerData.</returns>
    public static ShopSaveData LoadShop(ShopManager shopManager)
    {
        Assert.IsNotNull(instance.currentLoad, "currentLoad is null.");
        Assert.IsNotNull(shopManager, "shopManager is null.");

        return instance.currentLoad.shopSaveData;
    }

    /// <summary>
    /// Called when the application is quitting.
    /// </summary>
    public static void OnQuit()
    {
        SaveGameLevel(0);
        Save();
        WipeCurrentSave(); // temporary
    }

    #endregion
}
