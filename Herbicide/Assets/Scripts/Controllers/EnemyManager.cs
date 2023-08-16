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
    private int numEnemiesSpawned;

    /// <summary>
    /// All Enemies to spawn.
    /// </summary>
    private List<ObjectData> enemies;

    /// <summary>
    /// The height, in tiles, of all enemy layers in the map.
    /// </summary>
    private int mapHeight;


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
        Assert.IsNull(instance.enemies, "Already populated.");

        instance.mapHeight = mapHeight;
        List<ObjectData> enemyObjects = new List<ObjectData>();
        foreach (LayerData layer in enemyLayers)
        {
            Assert.IsTrue(layer.IsEnemyLayer(), "Not an enemy layer.");
            enemyObjects.AddRange(layer.GetEnemyObjectData());
        }
        instance.enemies = (enemyObjects.OrderBy(e => e.GetSpawnTime())).ToList();
    }

    /// <summary>
    /// Main update loop for the Enemy Manager. Checks the current game
    /// time and spawns an Enemy if it it's ready.
    /// </summary>
    /// <param name="dt">Current game time.</param>
    public static void UpdateEnemyManager(float dt)
    {
        //No more enemies to spawn
        List<ObjectData> enemies = instance.enemies;
        if (enemies == null || enemies.Count < 1) return;

        //Instantiate the Enemy to spawn
        ObjectData nextSpawn = enemies[0];
        if (dt < nextSpawn.GetSpawnTime()) return;
        Enemy spawnedEnemy = instance.GetEnemyPrefab(nextSpawn.GetEnemyName());
        Assert.IsNotNull(spawnedEnemy);

        //Determine spawn position
        Vector2Int spawnCoords = nextSpawn.GetSpawnCoordinates(instance.mapHeight);
        Debug.Log("Spawning at " + spawnCoords);
        float xWorldSpawn = TileGrid.CoordinateToPosition(spawnCoords.x);
        float yWorldSpawn = TileGrid.CoordinateToPosition(spawnCoords.y);
        Vector3 worldSpawnPos = new Vector3(xWorldSpawn, yWorldSpawn, 1);
        spawnedEnemy.gameObject.transform.position = worldSpawnPos;

        //Give the Enemy a controller and remove it from enemies list
        ControllerController.MakeEnemyController(spawnedEnemy);
        enemies.RemoveAt(0);
    }

    /// <summary>
    /// Returns an instantiated Enemy reference that represents an
    /// Enemy prefab with the type passed into this method.
    /// </summary>
    /// <param name="enemyType">the type of Enemy to clone</param>
    /// <returns>an instantiated enemy prefab</returns>
    private Enemy GetEnemyPrefab(string enemyType)
    {
        GameObject prefabToGet = null;

        if (enemyType.ToLower() == "kudzu") prefabToGet = enemyPrefabs[0];
        Assert.IsNotNull(prefabToGet, "Enemy type " + enemyType + " invalid.");

        Enemy enemyComponent = prefabToGet.GetComponent<Enemy>();
        Assert.IsNotNull(enemyComponent);

        return enemyComponent;
    }

    /// <summary>
    /// Returns the number of Enemies spawned in the level so far.
    /// </summary>
    /// <returns>the number of Enemies spawned in the level so far.</returns>
    public static int GetNumEnemiesSpawned()
    {
        return instance.numEnemiesSpawned;
    }


    /// <summary>
    /// Returns the number of Enemies that have yet to be spawned in this level. 
    /// </summary>
    /// <returns>the number of Enemies that have yet to be spawned 
    /// in this level. </returns>
    public static int EnemiesRemaining()
    {
        Assert.IsNotNull(instance.enemies);

        return instance.enemies.Count;
    }

    /// <summary>
    /// Returns true if there is at least one Enemy ready to be spawned.
    /// </summary>
    /// <returns>true if there is at least one Enemy ready to be spawned;
    /// otherwise, false.</returns>
    public static bool IsEnemyReady()
    {
        throw new System.NotImplementedException();
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
