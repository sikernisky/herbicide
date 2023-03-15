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

        //(2) Spawn the TileGrid
        TileGrid.SpawnGrid(instance);

        //(3) Load the Inventory
        InventoryController.LoadEntireInventory(items);
    }

    /// <summary>
    /// Main update loop: <br></br>
    /// 
    /// (1) Checks for input events.
    /// (2) Checks for placement events.
    /// </summary>
    void Update()
    {
        //(1) Check input events.
        CheckInputEvents();

        //(2) Check placement events.
        PlacementController.CheckPlacementEvents(instance);

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
    }

    /// <summary>
    /// Instantiates all Factories.
    private void MakeFactories()
    {
        FlooringFactory.SetSingleton(instance);
        TreeFactory.SetSingleton(instance);
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
    /// Checks for input events. First, checks for UI events. If none, checks
    /// for non-UI events.
    /// </summary>
    private void CheckInputEvents()
    {
        //Always check for these input events
        InventoryController.CheckInventoryInputEvents(instance);

        //UI Specific: does it matter if we're hovering over a UI element?
        if (InputController.HoveringOverUIElement())
        {
        }
        else
        {
            TileGrid.CheckTileInputEvents(instance);
        }
    }
}
