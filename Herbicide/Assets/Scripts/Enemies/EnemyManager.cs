using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Manages what and how many Enemies spawn in a level.
/// </summary>
public class EnemyManager : MonoBehaviour
{
    /// <summary>
    /// The enemies to spawn, in order.
    /// </summary>
    [SerializeField]
    private List<Enemy> enemies;

    /// <summary>
    /// The spawn positions -- tile coordinates -- of the enemies, in order.
    /// </summary>
    [SerializeField]
    private List<Vector2Int> spawnPositions;

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
    public static (Enemy enemy, Vector2Int spawnPosition) GetNextEnemy()
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
    /// <returns></returns>
    public static int EnemiesRemaining()
    {
        if (instance.enemies == null) return 0;

        return instance.enemies.Count;
    }

    /// <summary>
    /// Returns the next Enemy to spawn and its spawn position. Removes the Enemy and
    /// spawn position from their respective queues. 
    /// </summary>
    /// <returns>A double containing the next Enemy to spawn and its spawn position.</returns>
    private (Enemy, Vector2Int) Dequeue()
    {
        //sSafety checks
        if (enemies == null || spawnPositions == null) return default;
        if (enemies.Count == 0 || spawnPositions.Count == 0) return default;
        Assert.AreEqual(enemies.Count, spawnPositions.Count);

        Enemy nextEnemy = enemies[0];
        Vector2Int nextSpawnPosition = spawnPositions[0];

        enemies.RemoveAt(0);
        spawnPositions.RemoveAt(0);

        return (nextEnemy, nextSpawnPosition);
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
    }
}
