using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Controls the life of a level: <br></br>
/// 
/// (1) Setting up the level and singletons<br></br>
/// (2) Running the main update loop <br></br>
/// (3) Closing a level and cleaning up
/// </summary>
public class LevelController : MonoBehaviour
{
    /// <summary>
    /// Reference to the LevelController singleton.
    /// </summary>
    private LevelController instance;

    /// <summary>
    /// Items to load the InventoryController with. This is a temporary
    /// field and should be replaced when SelectMode is implemented.
    /// </summary>
    [SerializeField]
    private PlaceableObject[] items;

    /// <summary>
    /// All Enemy controllers active in the level.
    /// </summary>
    private List<EnemyController> enemyControllers;

    /// <summary>
    /// Sets up the level: <br></br>
    /// 
    /// (1) Instantiates all factories and singletons.<br></br>
    /// (2) Spawns the TileGrid.<br></br>
    /// (3) Loads the Inventory.<br></br>
    /// </summary>
    void Start()
    {
        //(1) Instantiate all factories and singletons
        SetSingleton();
        MakeFactories();
        MakeSingletons();

        //(2) Spawn the TileGrid and set the Camera
        CameraController.MoveCamera(TileGrid.SpawnGrid(instance));

        //(3) Load the Inventory
        InventoryController.LoadEntireInventory(items);
    }

    /// <summary>
    /// Main update loop: <br></br>
    /// 
    /// (1) Checks for input events.
    /// (2) Spawn an Enemy if possible.
    /// (3) Update EnemyControllers.
    /// </summary>
    void Update()
    {
        //(1) Check input events.
        CheckInputEvents();

        //(2) Spawn an enemy if possible.
        TrySpawnEnemy();

        //(3) Update EnemyControllers.
        UpdateEnemyControllers();
    }

    /// <summary>
    /// Instantiates all Singletons.
    /// </summary>
    private void MakeSingletons()
    {
        Assert.IsNotNull(instance, "method SetSingleton() should set the " +
            "levelcontroller singleton (currently null.)");
        TileGrid.SetSingleton(instance);
        CameraController.SetSingleton(instance);
        InputController.SetSingleton(instance);
        PlacementController.SetSingleton(instance);
        InventoryController.SetSingleton(instance);
        EnemyManager.SetSingleton(instance);
    }

    /// <summary>
    /// Instantiates all Factories.
    private void MakeFactories()
    {
        FlooringFactory.SetSingleton(instance);
        TreeFactory.SetSingleton(instance);
    }

    /// <summary>
    /// Finds and assigns the LevelController instance. Also instantiates
    /// the list of EnemyControllers.
    /// </summary>
    private void SetSingleton()
    {
        LevelController[] levelControllers = FindObjectsOfType<LevelController>();
        Assert.IsNotNull(levelControllers, "Array of found level controllers " +
            "is null.");
        Assert.IsTrue(levelControllers.Length == 1, "not enough / too many " +
            "levelcontrollers in the scene (" + levelControllers.Length + ").");
        instance = levelControllers[0];
        enemyControllers = new List<EnemyController>();
    }

    /// <summary>
    /// Attempts to spawn an Enemy.
    /// </summary>
    private void TrySpawnEnemy()
    {
        //Disqualifiers for spawning an enemy
        if (EnemyManager.EnemiesRemaining() <= 0) return;

        //Spawn the enemy, give it a controller.
        (Enemy, Vector2Int) tuple = EnemyManager.GetNextEnemy();
        Enemy nextEnemy = tuple.Item1;
        Vector2Int nextSpawnPos = tuple.Item2;
        float xWorldSpawnPos = TileGrid.CoordinateToPosition(nextSpawnPos.x);
        float yWorldSpawnPos = TileGrid.CoordinateToPosition(nextSpawnPos.y);
        Vector3 spawnWorldPos = new Vector3(xWorldSpawnPos, yWorldSpawnPos, 1);
        MovingEnemy movingEnemy = nextEnemy as MovingEnemy;
        int controllerId = EnemyManager.GetNumEnemiesSpawned() - 1; //start at 0

        if (movingEnemy != null)
        {
            TileGrid.PaintTile(nextSpawnPos.x, nextSpawnPos.y, TileGrid.PATHFINDING_RED);
            MovingEnemyController mec =
                new MovingEnemyController(movingEnemy, spawnWorldPos, controllerId);
            AddEnemyController(mec);
        }
    }

    /// <summary>
    /// Updates all EnemyControllers in the level.
    /// </summary>
    private void UpdateEnemyControllers()
    {
        if (enemyControllers == null) return;
        foreach (EnemyController controller in enemyControllers)
        {
            controller.UpdateEnemy();
        }
    }

    /// <summary>
    /// Adds an EnemyController to the LevelController's list of EnemyControllers.
    /// </summary>
    /// <param name="enemyController">the EnemyController to add.</param>
    private void AddEnemyController(EnemyController enemyController)
    {
        if (enemyController == null) return;

        enemyControllers.Add(enemyController);
    }

    /// <summary>
    /// Checks for input events. First, checks for UI events. If none, checks
    /// for non-UI events.
    /// </summary>
    private void CheckInputEvents()
    {
        //Always check for these input events
        InventoryController.CheckInventoryInputEvents(instance, InputController.DidEscapeDown());

        //UI Specific: does it matter if we're hovering over a UI element?
        if (InputController.HoveringOverUIElement())
        {
        }
        else
        {
            TileGrid.CheckTileInputEvents(instance);
        }
        TileGrid.CheckGridPlacementEvents(instance);
    }
}
