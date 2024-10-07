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
    /// The LevelBehaviourController for the tutorial.
    /// </summary>
    [SerializeField]
    private GameObject tutorialController;

    #endregion

    #region Methods

    /// <summary>
    /// Sets up the level: <br></br>
    /// 
    /// (1) Instantiates all factories and singletons.<br></br>
    /// (2) Loads SaveManager data. <br></br>
    /// (3) Parse all JSON data<br></br>
    /// (4) Spawns the TileGrid.<br></br>
    /// (5) Enables the correct LevelBehaviourController.
    /// </summary>
    void Start()
    {
        //(1) Instantiate all factories and singletons
        SetSingleton();
        MakeFactories();
        MakeSingletons();

        //(2) Load SaveManager data
        SubscribeControllersToSaveLoadEvents();
        SaveLoadManager.Load();

        //(3) Parse JSON
        JSONController.ParseTiledData();

        //(4) Spawn the TileGrid and set the Camera
        TiledData tiledData = JSONController.GetTiledData();
        Vector2 cameraPos = TileGrid.SpawnGrid(instance, tiledData);
        CameraController.MoveCamera(cameraPos);
        
        //(5) Enable the correct LevelBehaviourController
        SetLevelBehaviourControllerSingleton();
    }

    /// <summary>
    /// Main update loop: <br></br>
    /// 
    /// (1) Update and inform controllers of game state.<br></br>
    /// (2) Update Scene.<br></br>
    /// (3) Updates placement and input events.<br></br>
    /// (4) Update Model Controllers.<br></br>
    /// (5) Update Currencies.<br></br>
    /// (6) Update Balance.<br></br>
    /// (7) Update TileGrid.<br></br>
    /// (8) Update Canvas.<br></br>
    /// (9) Update StageController. <br></br>
    /// (10) Update LevelCompletionController. <br></br>
    /// (11) Update SettingsController. <br></br>
    /// (12) Update the LevelBehaviourController. <br></br>
    /// (13) Update the Time Scale.
    /// </summary>
    void Update()
    {
        //(1) Update and inform controllers of game state
        GameState gameState = DetermineGameState();
        if (gameState == GameState.INVALID) return;

        //(2) Update Scene.
        SceneController.UpdateScene();

        //(3) Update input and placement events.
        PlacementController.UpdatePlacementEvents(gameState, InputController.DidEscapeDown());
        PlacementController.UpdateCombinationEvents();
        UpdateInputEvents();

        //(4) Update ModelControllers.
        ControllerController.UpdateModelControllers(gameState);

        //(5) Update the Economy.
        EconomyController.UpdateEconomy(gameState);

        //(6) Update Shop.
        ShopManager.UpdateShopManager(gameState);

        //(7) Update TileGrid.
        TileGrid.UpdateTiles();

        //(8) Update Canvas.
        CanvasController.UpdateCanvas(gameState);

        //(9) Update StageController.
        StageController.UpdateStageController(gameState);

        //(10) Update LevelCompletionController.   
        LevelCompletionController.UpdateLevelCompletionController(gameState);

        //(11) Update SettingsController.
        SettingsController.UpdateSettingsMenu();

        //(12) Update the LevelBehaviourController.
        LevelBehaviourController.UpdateLevelBehaviourController();

        //(13) Update the Time Scale.
        UpdateTimeScale();
    }

    /// <summary>
    /// Called when the application is quitting. Sets the player stats
    /// back to the beginning.
    /// </summary>
    void OnApplicationQuit()
    {
        SaveLoadManager.OnQuit();
        LevelBehaviourController.OnQuit();
    }

    /// <summary>
    /// Instantiates the necessary Singletons for the LevelController.
    /// </summary>
    private void MakeSingletons()
    {
        Assert.IsNotNull(instance, "method SetSingleton() should set the " +
            "levelcontroller singleton (currently null.)");

        SaveLoadManager.SetSingleton(instance);
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
        CollectionManager.SetSingleton(instance); 
        LevelCompletionController.SetSingleton(instance);
        StageController.SetSingleton(instance);
        ExplosionController.SetSingleton(instance);
    }

    /// <summary>
    /// Sets up all Factories.
    private void MakeFactories()
    {
        TreeFactory.SetSingleton(instance);
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
    /// Enables the correct LevelBehaviourController based on the current level.
    /// Then, sets the LevelBehaviourController singleton.
    /// </summary>
    private void SetLevelBehaviourControllerSingleton()
    {
        int level = SaveLoadManager.GetLoadedGameLevel();
        switch(level)
        {
            case 0:
                tutorialController.SetActive(true);
                break;
            default:
                tutorialController.SetActive(false);
                break;
        }
        LevelBehaviourController.SetSingleton(instance);
    }

    /// <summary>
    /// Subscribes controllers to Save and Load events.
    /// </summary>
    private void SubscribeControllersToSaveLoadEvents()
    {
        CameraController.SubscribeToSaveLoadEvents(instance);
        CollectionManager.SubscribeToSaveLoadEvents(instance);
        ShopManager.SubscribeToSaveLoadEvents(instance);
    }

    /// <summary>
    /// Updates the TimeScale based on the current state of the game.
    /// </summary>
    private void UpdateTimeScale()
    {
        bool paused = SettingsController.SettingsMenuOpen() || LevelBehaviourController.IsPaused();
        if(paused) Time.timeScale = 0;
        else Time.timeScale = 1;
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
        int enemiesRemaining = EnemyManager.NumEnemiesThatRemainToBeSpawned(SceneController.GetTimeElapsed());
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
