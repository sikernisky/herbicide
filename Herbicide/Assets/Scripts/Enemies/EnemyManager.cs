using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Assertions;

/// <summary>
/// Manages what and how many Enemies spawn in a level. This
/// also acts as an "EnemyFactory" in that it spawns Enemy
/// GameObjects from an integer. 
/// </summary>
public class EnemyManager : MonoBehaviour
{

    /// <summary>
    /// Enemies to spawn this level. 
    /// </summary>
    private List<EnemyData> enemies;

    /// <summary>
    /// Enemies that can spawn right now.
    /// </summary>
    private List<EnemyData> readyEnemies;

    /// <summary>
    /// All Enemy Prefabs, indexed by type:<br></br>
    /// 
    /// 0 --> Kudzu
    /// </summary>
    [SerializeField]
    private List<GameObject> enemyPrefabs;

    /// <summary>
    /// Reference to the EnemyManager singleton
    /// </summary>
    private static EnemyManager instance;

    /// <summary>
    /// Number of Enemy objects spawned so far
    /// </summary>
    private static int numEnemiesSpawned;

    /// <summary>
    /// Returns the next Enemy to spawn with its spawn position.
    /// </summary>
    /// <returns>A double with its first element the Enemy to spawn and its second element
    /// the position at which to spawn it.</returns>
    public static (GameObject enemy, Vector2Int spawnPosition) GetNextEnemy()
    {
        numEnemiesSpawned++;
        return instance.Dequeue();
    }

    /// <summary>
    /// Returns the number of Enemies spawned in the level so far.
    /// </summary>
    /// <returns>the number of Enemies spawned in the level so far.</returns>
    public static int GetNumEnemiesSpawned()
    {
        return numEnemiesSpawned;
    }


    /// <summary>
    /// Returns the number of Enemies that have yet to be spawned in this level. 
    /// </summary>
    /// <returns>the number of Enemies that have yet to be spawned 
    /// in this level. </returns>
    public static int EnemiesRemaining()
    {
        if (instance.enemies == null) return 0;

        return instance.enemies.Count;
    }

    /// <summary>
    /// Returns true if there is at least one Enemy ready to be spawned.
    /// </summary>
    /// <returns>true if there is at least one Enemy ready to be spawned;
    /// otherwise, false.</returns>
    public static bool IsEnemyReady()
    {
        if (instance.readyEnemies == null) return false;
        return instance.readyEnemies.Count > 0;
    }

    /// <summary>
    /// Retrieves this level's enemy data and parses it.
    /// </summary>
    private void ParseEnemyData()
    {
        enemies = JSONController.GetEnemyData();
        readyEnemies = new List<EnemyData>();
        RefreshAvailableEnemies(0);
        Assert.IsNotNull(enemies);
    }

    /// <summary>
    /// Returns the next Enemy to spawn and its spawn position. Removes the Enemy and
    /// spawn position from their respective queues. 
    /// </summary>
    /// <returns>A double containing the next Enemy to spawn and its spawn position.</returns>
    private (GameObject, Vector2Int) Dequeue()
    {
        //Safety checks
        if (readyEnemies == null) return default;
        if (readyEnemies.Count == 0) return default;

        EnemyData nextEnemyData = readyEnemies[0];
        Vector2Int spawnPos = nextEnemyData.GetSpawnPos();
        Enemy.EnemyType spawnType;
        Enum.TryParse(nextEnemyData.GetEnemyType(), true, out spawnType);
        GameObject enemyPrefab = enemyPrefabs[(int)spawnType];
        readyEnemies.RemoveAt(0);

        return (enemyPrefab, spawnPos);
    }

    /// <summary>
    /// Updates this EnemyManager's list of enemies that it is ready to spawn
    /// at any given time.
    /// </summary>
    /// <param name="levelTime">How much time has elapsed in the level.</param>
    public static void RefreshAvailableEnemies(float levelTime)
    {
        for (int i = instance.enemies.Count - 1; i >= 0; i--)
        {
            EnemyData eData = instance.enemies[i];
            if (levelTime >= eData.GetTime())
            {
                instance.enemies.RemoveAt(i);
                instance.readyEnemies.Add(eData);
            }
        }
    }

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
        instance.ParseEnemyData();
    }
}
