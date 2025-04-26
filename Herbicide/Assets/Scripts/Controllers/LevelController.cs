using System.Data.Common;
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
        Vector3 cameraPos = TileGrid.SpawnGrid(instance, tiledData);
        CameraController.MoveCamera(cameraPos);
        
        //(5) Enable the correct LevelBehaviourController
        SetLevelBehaviourControllerSingleton();

        //(6) Initialize the Ticket Pool
        TicketManager.InitializeTickets();
    }

    /// <summary>
    /// Main update loop for singletons.
    /// </summary>
    void Update()
    {
        // Update and inform controllers of game state
        GameState gameState = DetermineGameState();
        if (gameState == GameState.INVALID) return;

        // Update Scene.
        SceneController.UpdateScene();

        // Update InputManager.
        InputManager.UpdateInputManager();

        // Update placement events.
        PlacementManager.UpdatePlacementManager(gameState);

        // Update ModelControllers.
        ControllerManager.UpdateModelControllers(gameState);

        // Update the Economy.
        EconomyController.UpdateEconomy(gameState);

        // Update Health.
        HealthController.UpdateHealthController(gameState);

        // Update Shop.
        ShopManager.UpdateShopManager(gameState);

        // Update TicketManager.
        TicketManager.UpdateTicketManager(gameState);

        // Update the InsightManager.
        InsightManager.UpdateInsightManager(gameState);

        // Update Canvas.
        CanvasController.UpdateCanvas(gameState);

        // Update LevelCompletionController.   
        LevelCompletionController.UpdateLevelCompletionController(gameState);

        // Update SettingsController.
        SettingsController.UpdateSettingsMenu();

        // Update the LevelBehaviourController.
        LevelBehaviourController.UpdateLevelBehaviourController(gameState);

        // Update the Time Scale.
        UpdateTimeScale();

        // Update the LightManager.
        LightManager.UpdateLightManager();

        // Update the CollectionManager.
        CollectionManager.UpdateCollectionManager();
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
        InputManager.SetSingleton(instance);
        PlacementManager.SetSingleton(instance);    
        EnemyManager.SetSingleton(instance);
        ShopManager.SetSingleton(instance);
        ControllerManager.SetSingleton(instance);
        EconomyController.SetSingleton(instance);
        HealthController.SetSingleton(instance);
        CanvasController.SetSingleton(instance);
        SettingsController.SetSingleton(instance);
        SoundController.SetSingleton(instance);
        CollectionManager.SetSingleton(instance); 
        LevelCompletionController.SetSingleton(instance);
        LightManager.SetSingleton(instance);
        ExplosionController.SetSingleton(instance);
        InsightManager.SetSingleton(instance);
        TicketManager.SetSingleton(instance);
        InventoryManager.SetSingleton(instance);
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
        HoleFactory.SetSingleton(instance);
        ProjectileFactory.SetSingleton(instance);
        ShopFactory.SetSingleton(instance);
        TileFactory.SetSingleton(instance);
        WallFactory.SetSingleton(instance);
        TicketFactory.SetSingleton(instance);
        InventoryFactory.SetSingleton(instance);
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
        EconomyController.SubscribeToSaveLoadEvents(instance);
        HealthController.SubscribeToSaveLoadEvents(instance);
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
        int activeEnemies = ControllerManager.NumEnemiesRemainingInclusive();
        int enemiesRemaining = EnemyManager.NumEnemiesThatRemainToBeSpawned(SceneController.GetTimeElapsed());
        bool enemiesPresent = enemiesRemaining > 0 || activeEnemies > 0;

        if (HealthController.LivesRemaining() == 0) currentGameState = GameState.LOSE;
        else if (!enemiesPresent && enemiesRemaining == 0) currentGameState = GameState.WIN;
        else currentGameState = GameState.ONGOING;
        return currentGameState;

    }
    #endregion
}
