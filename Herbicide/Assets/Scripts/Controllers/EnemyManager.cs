using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.Assertions;

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
    /// Number of Enemy objects spawned so far
    /// </summary>
    private int numEnemiesSpawned;

    /// <summary>
    /// The ObjectData of all Enemies to spawn.
    /// </summary>
    private List<ObjectData> enemyData;

    /// <summary>
    /// The height, in tiles, of all enemy layers in the map.
    /// </summary>
    private int mapHeight;

    /// <summary>
    /// true if the EnemyManager has been populated.
    /// </summary>
    private bool populated;


    /// <summary>
    /// Gives the EnemyManager instance all LayerData object layers that
    /// hold enemy objects. These Enemy objects will be spawned throughout
    /// the level.
    /// </summary>
    /// <param name="enemyLayers">All Enemy LayerData layers in the map.</param>
    /// <param name="mapHeight">The height, in tiles, of the map. </param>
    public static void PopulateWithEnemies(List<LayerData> enemyLayers, int mapHeight)
    {
        if (enemyLayers == null) return;
        Assert.IsFalse(Populated(), "Already populated.");

        instance.mapHeight = mapHeight;
        List<ObjectData> enemyObjects = new List<ObjectData>();
        foreach (LayerData layer in enemyLayers)
        {
            Assert.IsTrue(layer.IsEnemyLayer(), "Not an enemy layer.");
            enemyObjects.AddRange(layer.GetEnemyObjectData());
        }

        enemyObjects.ForEach(e => Assert.IsTrue(e.IsEnemy()));
        instance.enemyData = (enemyObjects.OrderBy(e => e.GetSpawnTime())).ToList();

        //Instantiate Enemy objects NOW so they're ready to go at runtime.
        foreach (ObjectData obToSpawn in instance.enemyData)
        {
            Enemy spawnedEnemy = EnemyFactory.MakeEnemy(obToSpawn.GetEnemyName());
            Assert.IsNotNull(spawnedEnemy);

            spawnedEnemy.gameObject.SetActive(false);
            Vector2 spawnWorldPos = new Vector2(
                TileGrid.CoordinateToPosition(obToSpawn.GetSpawnCoordinates(mapHeight).x),
                TileGrid.CoordinateToPosition(obToSpawn.GetSpawnCoordinates(mapHeight).y)
            );
            float spawnTime = obToSpawn.GetSpawnTime();
            ControllerController.MakeEnemyController(spawnedEnemy, spawnTime, spawnWorldPos);
        }

        instance.populated = true;
    }

    /// <summary>
    /// Returns the number of Enemies spawned in the level so far.
    /// </summary>
    /// <returns>the number of Enemies spawned in the level so far.</returns>
    /// <param name="dt">Current game time. </param>
    public static int GetNumEnemiesSpawned(float dt)
    {
        Assert.IsNotNull(instance.enemyData);

        int enemiesSpawned = 0;
        foreach (ObjectData enemyData in instance.enemyData)
        {
            Assert.IsTrue(enemyData.IsEnemy());
            if (enemyData.GetSpawnTime() <= dt) enemiesSpawned++;
        }
        return enemiesSpawned;
    }

    /// <summary>
    /// Returns the number of Enemies that have yet to be spawned in this level. 
    /// </summary>
    /// <returns>the number of Enemies that have yet to be spawned 
    /// in this level. </returns>
    /// <param name="dt">Current game time. </param>
    public static int EnemiesRemaining(float dt)
    {
        Assert.IsNotNull(instance.enemyData);

        int enemiesToGo = 0;
        foreach (ObjectData enemyData in instance.enemyData)
        {
            Assert.IsTrue(enemyData.IsEnemy());
            if (enemyData.GetSpawnTime() > dt) enemiesToGo++;
        }
        return enemiesToGo;
    }

    /// <summary>
    /// Returns true if the EnemyManager has been populated.
    /// </summary>
    /// <returns>true if the EnemyManager has been populated;
    /// otherwise, false. </returns>
    public static bool Populated()
    {
        return instance.populated;
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
