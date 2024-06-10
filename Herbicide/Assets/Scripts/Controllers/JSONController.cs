using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Newtonsoft.Json;
using UnityEditor.VersionControl;

/// <summary>
/// Parses JSON files into data.
/// </summary>
public class JSONController : MonoBehaviour
{
    /// <summary>
    /// Reference to the JSONController singleton.
    /// </summary>
    private static JSONController instance;

    /// <summary>
    /// JSON File that stores this level's Tiled data. This includes:<br></br>
    /// 
    /// (1) TileGrid information<br></br>
    /// (2) Enemy spawn points
    /// </summary>
    [SerializeField]
    private TextAsset tiledJSON;

    /// <summary>
    /// JSON file that stores this level's Tiled Data, parsed into a TiledData object.
    /// </summary>
    private static TiledData translatedTiledJson;


    /// <summary>
    /// Finds and sets the JSONController singleton for a level.
    /// </summary>
    /// <param name="levelController">The LevelController singleton.</param>
    public static void SetSingleton(LevelController levelController)
    {
        if (levelController == null) return;

        JSONController[] jsonControllers = FindObjectsOfType<JSONController>();
        Assert.IsNotNull(jsonControllers, "Array of InputControllers is null.");
        Assert.AreEqual(1, jsonControllers.Length);
        instance = jsonControllers[0];
        Assert.IsNotNull(instance.tiledJSON);
    }

    /// <summary>
    /// Finds and sets the JSONController singleton for the Main Menu.
    /// </summary>
    /// <param name="levelController">The LevelController singleton.</param>
    public static void SetSingleton(MainMenuController mainMenuController)
    {
        if (mainMenuController == null) return;

        JSONController[] jsonControllers = FindObjectsOfType<JSONController>();
        Assert.IsNotNull(jsonControllers, "Array of InputControllers is null.");
        Assert.AreEqual(1, jsonControllers.Length);
        instance = jsonControllers[0];
        Assert.IsNotNull(instance.tiledJSON);
    }

    /// <summary>
    /// Parses JSON file that contains the current level's Tiled data into a string.
    /// Stores that string.
    /// </summary>
    public static void ParseTiledData()
    {
        // print the extension of the file.
        string json = instance.tiledJSON.text;
        TiledData tiledData = Newtonsoft.Json.JsonConvert.DeserializeObject<TiledData>(json);
        Assert.IsNotNull(tiledData, "Parsed TileData object is null.");
        instance.SetTranslatedJson(tiledData);
    }

    /// <summary>
    /// Sets the TiledData object that stores the current level's parsed JSON.
    /// Can only do this once, 
    /// </summary>
    /// <param name="translatedJson">The non-null parsed JSON.</param>
    private void SetTranslatedJson(TiledData translatedJson)
    {
        if (translatedJson == null) return;
        if (JSONController.translatedTiledJson != null) return;

        JSONController.translatedTiledJson = translatedJson;
    }

    /// <summary>
    /// Returns the TiledData object that represents the current level's parsed JSON
    /// file. This object should not be null when calling this method.
    /// </summary>
    /// <returns>the TiledData object that represents the current level's parsed JSON
    /// file. </returns>
    public static TiledData GetTiledData()
    {
        Assert.IsNotNull(JSONController.translatedTiledJson);

        return JSONController.translatedTiledJson;
    }
}
