using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Controls the life of a level: <br></br>
/// 
/// (1) Setting up the level and singletons<br></br>
/// (2) Running the main update loop <br></br>
/// (3) Closing a level and cleaning up<br></br>
/// </summary>
public class LevelController : MonoBehaviour
{
    /// <summary>
    /// Reference to the LevelController singleton.
    /// </summary>
    private LevelController instance;

    /// <summary>
    /// Sets up the level: <br></br>
    /// 
    /// (1) Instantiates all singletons.<br></br>
    /// (2) Spawns the TileGrid.
    /// </summary>
    void Start()
    {
        //(1) Instantiate all singletons
        MakeSingletons();

        //(2) Spawn the TileGrid
        TileGrid.SpawnGrid(instance);
    }

    /// <summary>
    /// Main update loop.
    /// </summary>
    void Update()
    {
        TileGrid.CheckTileInputEvents(instance);
    }

    /// <summary>
    /// Instantiates all Singletons, including self.
    /// </summary>
    private void MakeSingletons()
    {
        SetSingleton();
        Assert.IsNotNull(instance, "method SetSingleton() should set the " +
            "levelcontroller singleton (currently null.)");
        TileGrid.SetSingleton(instance);
        CameraController.SetSingleton(instance);
        InputController.SetSingleton(instance);
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
}
