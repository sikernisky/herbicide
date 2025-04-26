using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Controls the Collection Menu scene.
/// </summary>
public class CollectionMenuController : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// Reference to the CollectionMenuController singleton.
    /// </summary>
    private CollectionMenuController instance;

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
    }

    /// <summary>
    /// Main update loop: <br></br>
    /// 
    /// (1) Updates Game State. <br></br>
    /// (2) Updates C nvas.<br></br>
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
    /// Instantiates the necessary Singletons for the Main Menu.
    /// </summary>
    private void MakeSingletons()
    {
        Assert.IsNotNull(instance, "MainMenuController singleton not set: call " +
            " SetSingleton()");
        
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
        CollectionMenuController[] collectionMenuControllers =
            FindObjectsOfType<CollectionMenuController>();
        Assert.IsNotNull(collectionMenuControllers, "Array of found main menu controllers " +
            "is null.");
        Assert.IsTrue(collectionMenuControllers.Length == 1, "not enough / too many " +
            "levelcontrollers in the scene (" + collectionMenuControllers.Length + ").");
        instance = collectionMenuControllers[0];
    }

    /// <summary>
    /// Returns the current state of the game. Since this is the Main Menu,
    /// always returns MENU. Also informs controllers of this game state.
    /// </summary>
    /// <returns>the current GameState.</returns>
    private GameState DetermineGameState() => GameState.MENU;

    #endregion
}
