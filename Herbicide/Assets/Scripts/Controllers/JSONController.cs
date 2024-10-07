using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Controls JSON events. Parses JSON files from Tiled into game data.
/// </summary>
public class JSONController : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// Reference to the JSONController singleton.
    /// </summary>
    private static JSONController instance;

    /// <summary>
    /// JSON Files that store each level's Tiled data. The index
    /// of the list corresponds to the level number. For example,
    /// the first element in the list is the JSON file for level 1.
    /// </summary>
    [SerializeField]
    private List<TextAsset> tiledJSONLevels;

    /// <summary>
    /// JSON file that stores this level's Tiled Data, parsed into a TiledData object.
    /// </summary>
    private static TiledData translatedTiledJson;

    #endregion

    #region Methods

    /// <summary>
    /// Finds and sets the JSONController singleton for a level.
    /// </summary>
    /// <param name="levelController">The LevelController singleton.</param>
    public static void SetSingleton(LevelController levelController)
    {
        if (levelController == null) return;

        JSONController[] jsonControllers = FindObjectsOfType<JSONController>();
        Assert.IsNotNull(jsonControllers, "Array of JSONControllers is null.");
        Assert.AreEqual(1, jsonControllers.Length);
        instance = jsonControllers[0];
        Assert.IsNotNull(instance.tiledJSONLevels);
        Assert.IsTrue(instance.tiledJSONLevels.Count > 0, "No levels assigned to JSONController.");
    }

    /// <summary>
    /// Parses JSON file that contains the current level's Tiled data into a string.
    /// Stores that string.
    /// </summary>
    public static void ParseTiledData()
    {
        int levelToLoad = SaveLoadManager.GetLoadedGameLevel();
        levelToLoad = Mathf.Clamp(levelToLoad, 0, instance.tiledJSONLevels.Count - 1); // clamp the level to the number of levels we have.
        TextAsset jsonTextAsset = instance.tiledJSONLevels[levelToLoad];
        string json = jsonTextAsset.text;
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

        translatedTiledJson = translatedJson;
    }

    /// <summary>
    /// Returns the TiledData object that represents the current level's parsed JSON
    /// file. This object should not be null when calling this method.
    /// </summary>
    /// <returns>the TiledData object that represents the current level's parsed JSON
    /// file. </returns>
    public static TiledData GetTiledData()
    {
        Assert.IsNotNull(translatedTiledJson);
        return translatedTiledJson;
    }

    /// <summary>
    /// Returns the maximum level index.
    /// </summary>
    /// <returns>the maximum level index.</returns>
    public static int GetMaxLevelIndex()
    {
        Assert.IsNotNull(instance.tiledJSONLevels); 
        return instance.tiledJSONLevels.Count - 1;
    }

    #endregion
}
