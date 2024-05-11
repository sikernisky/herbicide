using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

/// <summary>
/// The heart of a level. Responsible for <br></br>
/// 
/// (1) Setting up the level and singletons<br></br>
/// (2) Running the main update loop <br></br>
/// (3) Tracking game state <br></br>
/// (4) Closing a level and cleaning up
/// </summary>
public class LevelController : MonoBehaviour
{
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
    /// Sets up the level: <br></br>
    /// 
    /// (0) Sets Unity properties.<br></br>
    /// (1) Instantiates all factories and singletons.<br></br>
    /// (2) Parse all JSON data<br></br>
    /// (3) Spawns the TileGrid.<br></br>
    /// (4) Loads the Shop.<br></br>
    /// (5) Loads the Synergies.
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

        //(5) Load the Synergies (TODO: Remove this static HashSet with dynamic one)
        //HashSet<SynergyController.Synergy> synergies = new HashSet<SynergyController.Synergy>();
        //synergies.Add(SynergyController.Synergy.TRIPLE_THREAT);
        //SynergyController.LoadAllSynergies(synergies);


    }

    /// <summary>
    /// Main update loop: <br></br>
    /// 
    /// (1) Update and inform controllers of game state.<br></br>
    /// (2) Update Scene.<br></br>
    /// (3) Checks for input events.<br></br>
    /// (4) Update the SynergyController.<br></br>
    /// (5) Update Mob Controllers.<br></br>
    /// (6) Update Currencies.<br></br>
    /// (7) Update Balance.<br></br>
    /// (8) Update TileGrid.<br></br>
    /// (9) Update Canvas.<br></br>
    /// </summary>
    void Update()
    {
        //(1) Update and inform controllers of game state
        if (DetermineGameState() == GameState.INVALID) return;

        //(2) Update Scene.
        SceneController.UpdateScene();

        //(3) Check input events.
        CheckInputEvents();

        //(4) Update SynergyController.
        //SynergyController.UpdateSynergyController(ModelController.GetAllTargetableObjects());

        //(5) Update Controllers.
        ControllerController.UpdateAllControllers();

        //(6) Update the Economy.
        EconomyController.UpdateEconomy();

        //(7) Update Shop.
        ShopManager.UpdateShop();

        //(8) Update TileGrid.
        TileGrid.UpdateTiles();

        //(9) Update Canvas.
        CanvasController.UpdateCanvas(SceneController.GetFPS());
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
        ControllerController.SetSingleton(instance);
        EconomyController.SetSingleton(instance);
        CanvasController.SetSingleton(instance);
        ShopManager.SetSingleton(instance);
        SoundController.SetSingleton(instance);
        //SynergyController.SetSingleton(instance);
    }

    /// <summary>
    /// Sets up all Factories.
    private void MakeFactories()
    {
        AcornFactory.SetSingleton(instance);
        BasicTreeFactory.SetSingleton(instance);
        BasicTreeSeedFactory.SetSingleton(instance);
        BearFactory.SetSingleton(instance);
        BombFactory.SetSingleton(instance);
        BombSplatFactory.SetSingleton(instance);
        ButterflyFactory.SetSingleton(instance);
        DewFactory.SetSingleton(instance);
        EdgeFactory.SetSingleton(instance);
        EmanationFactory.SetSingleton(instance);
        FlooringFactory.SetSingleton(instance);
        HedgehogFactory.SetSingleton(instance);
        KudzuFactory.SetSingleton(instance);
        NexusFactory.SetSingleton(instance);
        NexusHoleFactory.SetSingleton(instance);
        ShopFactory.SetSingleton(instance);
        SquirrelFactory.SetSingleton(instance);
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
        int activeEnemies = ControllerController.NumActiveEnemies();
        bool nexusHolesFilled = ControllerController.AllHolesFilled();
        int enemiesRemaining = EnemyManager.EnemiesRemaining(SceneController.GetTimeElapsed());
        bool enemiesPresent = (enemiesRemaining > 0 || activeEnemies > 0);

        //Win condition: All enemies dead, at least one NexusHole not filled..
        if (!enemiesPresent && !nexusHolesFilled) currentGameState = GameState.WIN;

        //Lose condition: All holes filled.
        else if (nexusHolesFilled) currentGameState = GameState.LOSE;

        //Ongoing condition: At least one nexus hole not filled and at least one Enemy alive.
        else if (enemiesPresent && !nexusHolesFilled) currentGameState = GameState.ONGOING;

        //Something went wrong.
        else currentGameState = GameState.INVALID;

        //Inform Controllers.
        ControllerController.InformOfGameState(currentGameState);
        CanvasController.InformOfGameState(currentGameState);
        EconomyController.InformOfGameState(currentGameState);
        PlacementController.InformOfGameState(currentGameState);
        ShopManager.InformOfGameState(currentGameState);

        return currentGameState;
    }

    /// <summary>
    /// Checks for input events.
    /// </summary>
    private void CheckInputEvents()
    {
        //Always check for these input events
        PlacementController.CheckPlacementEvents(InputController.DidEscapeDown());

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
            EconomyController.CheckCurrencyPickup();
        }
    }


}
