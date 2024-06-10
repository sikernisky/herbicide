using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.Assertions;
using System.Text.RegularExpressions;


/// <summary>
/// Manages what and how many Enemies spawn in a level. This
/// also acts as an "EnemyFactory" in that it spawns Enemy
/// GameObjects from an integer. 
/// </summary>
public class EnemyManager : MonoBehaviour
{
    /// <summary>
    /// Reference to the EnemyManager singleton
    /// </summary>
    private static EnemyManager instance;

    /// <summary>
    /// Sorted spawn times of all Enemies this level.
    /// </summary>
    private List<float> spawnTimes;

    /// <summary>
    /// true if the EnemyManager has been populated.
    /// </summary>
    private bool populated;


    /// <summary>
    /// Gives the EnemyManager a dictionary of Enemy spawn marker locations
    /// and their unparsed spawn data. This method decodes the data and instantiates
    /// Enemies to its specification.
    /// </summary>
    /// <param name="enemyLayers">All Enemy LayerData layers in the map.</param>
    /// <param name="mapHeight">The height, in tiles, of the map. </param>
    public static void PopulateWithEnemies(Dictionary<Vector2Int, string> markers, int mapHeight)
    {
        if (markers == null) return;
        Assert.IsFalse(Populated(), "Already populated.");

        // Parse the data.
        foreach (KeyValuePair<Vector2Int, string> marker in markers)
        {
            Vector2Int spawnLocation = marker.Key;
            string unparsedData = marker.Value;
            List<(string, float)> parsedData = instance.ParseMarkerData(unparsedData);

            foreach (var enemyData in parsedData)
            {
                string enemyName = enemyData.Item1;
                GameObject enemy = instance.GetEnemyPrefabFromString(enemyName);
                Assert.IsNotNull(enemy);
                Enemy enemyComp = enemy.GetComponent<Enemy>();
                enemy.gameObject.SetActive(false);
                float spawnX = TileGrid.CoordinateToPosition(marker.Key.x);
                float spawnY = TileGrid.CoordinateToPosition(marker.Key.y);
                Vector3 spawnWorldPos = new Vector3(spawnX, spawnY, 1);
                float spawnTime = enemyData.Item2;
                instance.spawnTimes.Add(spawnTime);
                enemyComp.SetSpawnTime(spawnTime);
                enemyComp.SetSpawnPos(spawnWorldPos);
                ControllerController.MakeModelController(enemyComp);
            }
        }

        instance.spawnTimes.Sort();
        instance.populated = true;
    }

    /// <summary>
    /// Returns a list of tuples of Enemy names to their spawn times.
    /// </summary>
    /// <param name="input">A string of comma separated names followed
    /// by their spawn times. </param>
    /// <returns>a list of tuples of Enemy names to their spawn times.</returns>
    private List<(string, float)> ParseMarkerData(string input)
    {
        Assert.IsNotNull(input);

        string[] sliced = input.Split(',');
        List<(string, float)> result = new List<(string, float)>();
        string pattern = @"([a-zA-Z]+)(\d+(\.\d+)?)";

        foreach (string item in sliced)
        {
            Match match = Regex.Match(item, pattern);
            if (!match.Success) continue;
            string letters = match.Groups[1].Value;
            string numbers = match.Groups[2].Value;
            result.Add((letters, float.Parse(numbers)));
        }

        return result;
    }

    /// <summary>
    /// Returns the number of Enemies that have yet to be spawned in this level. 
    /// </summary>
    /// <returns>the number of Enemies that have yet to be spawned 
    /// in this level. </returns>
    /// <param name="dt">Current game time. </param>
    public static int EnemiesRemaining(float dt)
    {
        Assert.IsNotNull(instance.spawnTimes);

        int enemiesToGo = 0;
        foreach (float spawnTime in instance.spawnTimes)
        {
            if (spawnTime > dt) enemiesToGo++;
        }
        return enemiesToGo;
    }

    /// <summary>
    /// Returns true if the EnemyManager has been populated.
    /// </summary>
    /// <returns>true if the EnemyManager has been populated;
    /// otherwise, false. </returns>
    public static bool Populated() { return instance.populated; }

    /// <summary>
    /// Finds and sets the EnemyManager singleton.
    /// </summary>
    /// <param name="levelController"></param>
    public static void SetSingleton(LevelController levelController)
    {
        if (levelController == null) return;
        if (instance != null) return;

        EnemyManager[] enemyManagers = FindObjectsOfType<EnemyManager>();
        Assert.IsNotNull(enemyManagers, "Array of InputControllers is null.");
        Assert.AreEqual(1, enemyManagers.Length);
        instance = enemyManagers[0];
        instance.spawnTimes = new List<float>();
    }

    /// <summary>
    /// Returns a GameObject with an Enemy component attached that matches
    /// the string passed into this method. If the string does not match
    /// an Enemy type, throws an Exception.
    /// </summary>
    /// <param name="enemyName">The name of the Enemy.</param>
    /// <returns>A GameObject with an Enemy component that matches the string
    /// passed into this method.</returns>
    private GameObject GetEnemyPrefabFromString(string enemyName)
    {
        Assert.IsNotNull(enemyName);

        switch (enemyName.ToLower())
        {
            case "knotwood":
                return EnemyFactory.GetEnemyPrefab(ModelType.KNOTWOOD);
            case "kudzu":
                return EnemyFactory.GetEnemyPrefab(ModelType.KUDZU);
            default:
                break;
        }

        throw new System.NotSupportedException(enemyName + " not supported.");
    }
}
