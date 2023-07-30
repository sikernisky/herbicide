using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

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
    /// Items to load the InventoryController with. This is a temporary
    /// field and should be replaced when SelectMode is implemented.
    /// </summary>
    [SerializeField]
    private PlaceableObject[] items;

    /// <summary>
    /// How long the level waits before spawning enemies.
    /// </summary>
    [SerializeField]
    private float enemySpawnDelay;

    /// <summary>
    /// Reference to the LevelController singleton.
    /// </summary>
    private LevelController instance;

    /// <summary>
    /// Stores FPS timing.
    /// </summary>
    private float deltaTime;

    /// <summary>
    /// How much time has elapsed since the level began.
    /// </summary>
    private float levelTime;

    /// <summary>
    /// Maximum frame rate.
    /// </summary>
    private const int MAX_FRAME_RATE = 60;

    /// <summary>
    /// The most recent game state.
    /// </summary>
    private GameState currentGameState;



    /// <summary>
    /// Sets up the level: <br></br>
    /// 
    /// (0) Sets Unity properties.<br></br>
    /// (1) Instantiates all factories and singletons.<br></br>
    /// (2) Spawns the TileGrid.<br></br>
    /// (3) Loads the Inventory.<br></br>
    /// (4) Parse JSON data for Enemy objects <br></br>
    /// (5) Initialize GAME_STATE state
    /// </summary>
    void Start()
    {
        //(0) Set Unity properties
        SetUnityProperties();

        //(1) Instantiate all factories and singletons
        SetSingleton();
        MakeFactories();
        MakeSingletons();

        //(2) Spawn the TileGrid and set the Camera
        CameraController.MoveCamera(TileGrid.SpawnGrid(instance));

        //(3) Load the Inventory
        InventoryController.LoadEntireInventory(items);

        //(4) Parse JSON data for Enemy objects
        JSONController.GetEnemyData();

        //(5) Initialize GAME_STATE state
    }

    /// <summary>
    /// Main update loop: <br></br>
    /// 
    /// (1) Update and inform controllers of game state.<br></br>
    /// (2) Update time elapsed.<br></br>
    /// (3) Checks for input events.<br></br>
    /// (4) Spawn an Enemy if possible.<br></br>
    /// (5) Update Controllers.<br></br>
    /// (6) Update Projectiles.<br></br>
    /// (7) Update Currencies.<br></br>
    /// (8) Update Balance.<br></br>
    /// (9) Update TileGrid.<br></br>
    /// (10) Update Canvas.<br></br>
    /// 
    /// These actions are called if the game state is ONGOING. Otherwise,
    /// separate update methods are called.
    /// </summary>
    void Update()
    {
        //DebugFPS();

        //(1) Update and inform controllers of game state
        if (DetermineGameState() == GameState.INVALID) return;

        //(2) Increment time.
        levelTime += Time.deltaTime;

        //(3) Check input events.
        CheckInputEvents();

        //(4) Spawn an enemy if possible.
        TrySpawnEnemy();

        //(5) Update Controllers.
        ControllerController.UpdateAllControllers(TileGrid.GetAllTargetableObjects());

        //(6) Update Projectiles.
        ProjectileController.CheckProjectiles();

        //(7) Update the Economy.
        EconomyController.UpdateEconomy();

        //(8) Update Inventory.
        InventoryController.UpdateSlots(EconomyController.GetMoney());

        //(9) Update TileGrid.
        TileGrid.TrackEnemyTilePositions(ControllerController.GetAllActiveEnemies());

        //(10) Update Canvas.
        CanvasController.UpdateCanvas();
    }

    /// <summary>
    /// Instantiates all Singletons.
    /// </summary>
    private void MakeSingletons()
    {
        Assert.IsNotNull(instance, "method SetSingleton() should set the " +
            "levelcontroller singleton (currently null.)");
        JSONController.SetSingleton(instance);
        TileGrid.SetSingleton(instance);
        CameraController.SetSingleton(instance);
        InputController.SetSingleton(instance);
        PlacementController.SetSingleton(instance);
        InventoryController.SetSingleton(instance);
        EnemyManager.SetSingleton(instance);
        ProjectileController.SetSingleton(instance);
        ControllerController.SetSingleton(instance);
        EconomyController.SetSingleton(instance);
        CanvasController.SetSingleton(instance);
        SoundController.SetSingleton(instance);
    }

    /// <summary>
    /// Instantiates all Factories.
    private void MakeFactories()
    {
        FlooringFactory.SetSingleton(instance);
        TreeFactory.SetSingleton(instance);
        DefenderFactory.SetSingleton(instance);
        EnemyFactory.SetSingleton(instance);
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
    }

    /// <summary>
    /// Attempts to spawn an Enemy.
    /// </summary>
    private void TrySpawnEnemy()
    {
        if (levelTime < enemySpawnDelay) return;

        EnemyManager.RefreshAvailableEnemies(levelTime);
        if (!EnemyManager.IsEnemyReady()) return;

        //Spawn the enemy.
        (GameObject, Vector2Int) tuple = EnemyManager.GetNextEnemy();
        Enemy nextEnemy = tuple.Item1.GetComponent<Enemy>();
        Assert.IsNotNull(nextEnemy);

        //Fix Enemies spawning at invalid positions.
        Vector2Int nextSpawnPos = tuple.Item2;
        if (!TileGrid.TileExistsAtPosition(tuple.Item2.x, tuple.Item2.y))
        {
            Vector2Int brokenPos = tuple.Item2;
            Vector2Int fixedPos = TileGrid.NearestEdgeCoordinate(brokenPos.x, brokenPos.y);
            nextSpawnPos = fixedPos;
        }

        float xWorldSpawnPos = TileGrid.CoordinateToPosition(nextSpawnPos.x);
        float yWorldSpawnPos = TileGrid.CoordinateToPosition(nextSpawnPos.y);
        Vector3 spawnWorldPos = new Vector3(xWorldSpawnPos, yWorldSpawnPos, 1);
        MovingEnemy movingEnemy = nextEnemy as MovingEnemy;
        int controllerId = EnemyManager.GetNumEnemiesSpawned() - 1; //start at 0
        nextEnemy.transform.position = spawnWorldPos;
        ControllerController.MakeEnemyController(nextEnemy);
    }

    /// <summary>
    /// Returns the correct state of the level. For
    /// example, if the player has no more Trees, the game is LOSE. If all
    /// Enemies are dead, the game is WIN. Informs sub-controllers of this
    /// new state.
    /// </summary>
    /// <return>The most updated GameState.</return>
    private GameState DetermineGameState()
    {
        HashSet<Tree> activeTrees = ControllerController.GetAllActiveTrees();
        HashSet<Enemy> activeEnemies = ControllerController.GetAllActiveEnemies();
        int enemiesRemaining = EnemyManager.EnemiesRemaining();
        bool enemiesPresent = (enemiesRemaining > 0 || activeEnemies.Count > 0);

        //Win condition: All enemies dead, at least one Tree alive.
        if (!enemiesPresent && activeTrees.Count > 0) currentGameState = GameState.WIN;

        //Lose condition: All trees dead, at least one Enemy alive.
        else if (enemiesPresent && activeTrees.Count == 0) currentGameState = GameState.LOSE;

        //Tie condition: All trees and enemies dead. 
        else if (!enemiesPresent && activeTrees.Count == 0) currentGameState = GameState.TIE;

        //Ongoing condition: At least one tree and enemy alive.
        else if (enemiesPresent && activeTrees.Count > 0) currentGameState = GameState.ONGOING;

        //Something went wrong.
        else currentGameState = GameState.INVALID;

        //Inform Controllers.
        InventoryController.InformOfGameState(currentGameState);
        ControllerController.InformOfGameState(currentGameState);
        CanvasController.InformOfGameState(currentGameState);
        PlacementController.InformOfGameState(currentGameState);

        return currentGameState;
    }

    /// <summary>
    /// Checks for input events. First, checks for UI events. If none, checks
    /// for non-UI events.
    /// </summary>
    private void CheckInputEvents()
    {
        //Always check for these input events
        PlacementController.CheckPlacementEvents(InputController.DidEscapeDown());

        //UI Specific: does it matter if we're hovering over a UI element?
        if (InputController.HoveringOverUIElement())
        {
            //Put events here to trigger if we're hovering over a Canvas / UI element.
        }
        else
        {
            //Put events here to trigger if we're NOT hovering over a Canvas / UI element.
            TileGrid.CheckTileInputEvents();
            EconomyController.CheckCurrencyPickup();
        }
    }

    /// <summary>
    /// Sets properties of the engine outside of gameplay.
    /// </summary>
    private void SetUnityProperties()
    {
        Application.targetFrameRate = MAX_FRAME_RATE;
        Screen.SetResolution(1920, 1080, false);
    }

    /// <summary>
    /// Prints out the current FPS.
    /// </summary>
    private void DebugFPS()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        Debug.Log($"FPS: {Mathf.RoundToInt(fps)}");
    }
}
