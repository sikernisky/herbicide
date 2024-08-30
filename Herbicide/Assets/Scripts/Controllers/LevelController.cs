using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Controls the game loop and updates
/// all other controllers. The heart of a level. 
/// </summary>
public class LevelController : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// Reference to the LevelController singleton.
    /// </summary>
    private LevelController instance;

    /// <summary>
    /// The most recent game state.
    /// </summary>
    private GameState currentGameState;

    /// <summary>
    /// true if Debug mode is on.
    /// </summary>
    private bool debugOn;

    /// <summary>
    /// true if the game is paused; otherwise, false.
    /// </summary>
    private bool isPaused;

    #endregion

    #region Methods

    /// <summary>
    /// Sets up the level: <br></br>
    /// 
    /// (0) Sets Unity properties.<br></br>
    /// (1) Instantiates all factories and singletons.<br></br>
    /// (2) Parse all JSON data<br></br>
    /// (3) Spawns the TileGrid.<br></br>
    /// (4) Loads the Shop.<br></br>
    /// </summary>
    void Start()
    {
        //(0) Set Unity properties
        SceneController.SetUnityProperties();

        //(1) Instantiate all factories and singletons
        SetSingleton();
        MakeFactories();
        MakeSingletons();

        //(2) Parse JSON
        JSONController.ParseTiledData();

        //(3) Spawn the TileGrid and set the Camera
        TiledData tiledData = JSONController.GetTiledData();
        Vector2 cameraPos = TileGrid.SpawnGrid(instance, tiledData);
        CameraController.MoveCamera(cameraPos);

        //(4) Load the Shop
        ShopManager.LoadShop();
    }

    /// <summary>
    /// Main update loop: <br></br>
    /// 
    /// (1) Update and inform controllers of game state.<br></br>
    /// (2) Update Scene.<br></br>
    /// (3) Updates placement and input events.<br></br>
    /// (4) Update Mob Controllers.<br></br>
    /// (5) Update Currencies.<br></br>
    /// (6) Update Balance.<br></br>
    /// (7) Update TileGrid.<br></br>
    /// (8) Update Canvas.<br></br>
    /// (9) Update StageController. <br></br>
    /// (10) Update LevelCompletionController. 
    /// </summary>
    void Update()
    {
        // Pause logic
        if (SettingsController.SettingsMenuOpen()) isPaused = true;
        else isPaused = false;

        // Update Settings (immune to pausing)
        SettingsController.UpdateSettingsMenu();
        if (isPaused) return;

        //(1) Update and inform controllers of game state
        GameState gameState = DetermineGameState();
        if (gameState == GameState.INVALID) return;

        //(2) Update Scene.
        SceneController.UpdateScene();

        //(3) Update input and placement events.
        PlacementController.UpdatePlacementEvents(gameState, InputController.DidEscapeDown());
        PlacementController.UpdateCombinationEvents();
        UpdateInputEvents();

        //(4) Update Controllers.
        ControllerController.UpdateModelControllers(gameState);

        //(5) Update the Economy.
        EconomyController.UpdateEconomy(gameState);

        //(6) Update Shop.
        ShopManager.UpdateShop(gameState);

        //(7) Update TileGrid.
        TileGrid.UpdateTiles();

        //(8) Update Canvas.
        CanvasController.UpdateCanvas(gameState);

        //(9) Update StageController.
        StageController.UpdateStageController(gameState);

        //(10) Update LevelCompletionController.   
        LevelCompletionController.UpdateLevelCompletionController(gameState);
    }

    /// <summary>
    /// Instantiates the necessary Singletons for the LevelController.
    /// </summary>
    private void MakeSingletons()
    {
        Assert.IsNotNull(instance, "method SetSingleton() should set the " +
            "levelcontroller singleton (currently null.)");
        SceneController.SetSingleton(instance);
        JSONController.SetSingleton(instance);
        TileGrid.SetSingleton(instance);
        CameraController.SetSingleton(instance);
        InputController.SetSingleton(instance);
        PlacementController.SetSingleton(instance);
        EnemyManager.SetSingleton(instance);
        ShopManager.SetSingleton(instance);
        ControllerController.SetSingleton(instance);
        EconomyController.SetSingleton(instance);
        CanvasController.SetSingleton(instance);
        SettingsController.SetSingleton(instance);
        SoundController.SetSingleton(instance);
        LevelCompletionController.SetSingleton(instance);
        StageController.SetSingleton(instance);
        ExplosionController.SetSingleton(instance);
    }

    /// <summary>
    /// Sets up all Factories.
    private void MakeFactories()
    {
        BasicTreeFactory.SetSingleton(instance);
        CollectableFactory.SetSingleton(instance);
        DefenderFactory.SetSingleton(instance);
        EdgeFactory.SetSingleton(instance);
        EmanationFactory.SetSingleton(instance);
        EnemyFactory.SetSingleton(instance);
        FlooringFactory.SetSingleton(instance);
        NexusFactory.SetSingleton(instance);
        NexusHoleFactory.SetSingleton(instance);
        ProjectileFactory.SetSingleton(instance);
        ShopFactory.SetSingleton(instance);
        TileFactory.SetSingleton(instance);
        WallFactory.SetSingleton(instance);
    }

    /// <summary>
    /// Finds and assigns the LevelController instance.
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
    /// Returns the correct state of the level. For
    /// example, if the player has no more Trees, the game is LOSE. If all
    /// Enemies are dead, the game is WIN. Informs sub-controllers of this
    /// new state.
    /// </summary>
    /// <return>The most updated GameState.</return>
    private GameState DetermineGameState()
    {

        int activeEnemies = ControllerController.NumEnemiesRemainingInclusive();
        int enemiesRemaining = EnemyManager.EnemiesRemaining(SceneController.GetTimeElapsed());
        bool enemiesPresent = (enemiesRemaining > 0 || activeEnemies > 0);
        bool nexusPresent = ControllerController.NumActiveNexii() > 0;

        // Win condition: All enemies dead, at least one nexus remaning. 
        if (!enemiesPresent && nexusPresent) currentGameState = GameState.WIN;

        // Lose condition: No nexii remaining.
        else if (!nexusPresent) currentGameState = GameState.LOSE;

        // Ongoing condition: At least one nexus present and at least one Enemy alive.
        else if (enemiesPresent && nexusPresent) currentGameState = GameState.ONGOING;

        // Something went wrong.
        else currentGameState = GameState.INVALID;

        return currentGameState;
    }

    /// <summary>
    /// Checks for input events.
    /// </summary>
    private void UpdateInputEvents()
    {
        //Debug Toggle
        debugOn = InputController.DidDebugDown() ? !debugOn : debugOn;
        TileGrid.SetDebug(instance, debugOn);
        CanvasController.SetDebug(instance, debugOn);

        //UI Specific: does it matter if we're hovering over a UI element?
        if (InputController.HoveringOverUIElement())
        {
            //Put events here to trigger if we're hovering over a Canvas / UI element.
        }
        else
        {
            //Put events here to trigger if we're NOT hovering over a Canvas / UI element.
            TileGrid.CheckTileInputEvents();
        }
    }

    #endregion
}
