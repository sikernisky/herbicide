using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Controls the Main Menu scene.
/// </summary>
public class MainMenuController : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// Reference to the MainMenuController singleton.
    /// </summary>
    private MainMenuController instance;

    #endregion

    #region Methods

    /// <summary>
    /// Sets up the Main Menu: <br></br>
    /// 
    /// (1) Instantiates all factories and singletons.<br></br>
    /// </summary>
    void Start()
    {
        //(1) Instantiate all factories and singletons
        SetSingleton();
        MakeSingletons();

        // temp
        SaveLoadManager.WipeCurrentSave();
    }

    /// <summary>
    /// Main update loop: <br></br>
    /// 
    /// (1) Updates Game State. <br></br>
    /// (2) Update Canvas.<br></br>
    /// </summary>
    void Update()
    {
        //(1) Updates Game State.
        GameState gameState = DetermineGameState();
        if (gameState == GameState.INVALID) return;

        //(2) Update Canvas.
        CanvasController.UpdateCanvas(gameState);
    }

    /// <summary>
    /// Called when the application is quitting. Sets the player stats
    /// back to the beginning.
    /// </summary>
    void OnApplicationQuit()
    {
        // temp
        SaveLoadManager.WipeCurrentSave();
    }

    /// <summary>
    /// Instantiates the necessary Singletons for the Main Menu.
    /// </summary>
    private void MakeSingletons()
    {
        Assert.IsNotNull(instance, "MainMenuController singleton not set: call " +
            " SetSingleton()");

        SaveLoadManager.SetSingleton(instance);
        SceneController.SetSingleton(instance);
        CameraController.SetSingleton(instance);
        CanvasController.SetSingleton(instance);
        SoundController.SetSingleton(instance);
    }

    /// <summary>
    /// Finds and assigns the MainMenuController instance.
    /// </summary>
    private void SetSingleton()
    {
        MainMenuController[] mainMenuControllers = FindObjectsOfType<MainMenuController>();
        Assert.IsNotNull(mainMenuControllers, "Array of found main menu controllers " +
            "is null.");
        Assert.IsTrue(mainMenuControllers.Length == 1, "not enough / too many " +
            "levelcontrollers in the scene (" + mainMenuControllers.Length + ").");
        instance = mainMenuControllers[0];
    }

    /// <summary>
    /// Returns the current state of the game. Since this is the Main Menu,
    /// always returns MENU. Also informs controllers of this game state.
    /// </summary>
    /// <returns>the current GameState.</returns>
    private GameState DetermineGameState() => GameState.MENU;

    #endregion
}
