using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Parses JSON files into data.
/// </summary>
public class JSONController : MonoBehaviour
{
    /// <summary>
    /// Reference to the JSONControlelr singleton.
    /// </summary>
    private static JSONController instance;

    /// <summary>
    /// JSON File that stores this level's data. 
    /// </summary>
    [SerializeField]
    private TextAsset levelJSON;

    /// <summary>
    /// The levelJSON file parsed into a LevelData object.
    /// </summary>
    private LevelData data;

    /// <summary>
    /// Finds and sets the JSONController singleton.
    /// </summary>
    /// <param name="levelController">The LevelController singleton.</param>
    public static void SetSingleton(LevelController levelController)
    {
        if (levelController == null) return;

        JSONController[] jsonControllers = FindObjectsOfType<JSONController>();
        Assert.IsNotNull(jsonControllers, "Array of InputControllers is null.");
        Assert.AreEqual(1, jsonControllers.Length);
        instance = jsonControllers[0];
        Assert.IsNotNull(instance.levelJSON);
        instance.ParseLevelData();
    }

    /// <summary>
    /// Parses the level JSON file into a LevelData object. Stores that
    /// object.
    /// </summary>
    private void ParseLevelData()
    {
        string json = levelJSON.text;
        instance.data = JsonUtility.FromJson<LevelData>(json);
    }

    /// <summary>
    /// Returns a list of EnemyData representing all Enemies that will exist
    /// in the current level.
    /// </summary>
    /// <returns>A list of EnemyData for this level.</returns>
    public static List<EnemyData> GetEnemyData()
    {
        string json = instance.levelJSON.text;
        LevelData data = JsonUtility.FromJson<LevelData>(json);
        return data.GetEnemyData();
    }

}
