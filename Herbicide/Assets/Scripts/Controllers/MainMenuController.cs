using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// The heart of the main menu. Responsible for <br></br>
/// 
/// (1) Setting up the main menu and singletons<br></br>
/// (2) Running the main update loop <br></br>
/// (3) Detecting input and switching to main game (with
/// help of scene controller)
/// </summary>
public class MainMenuController : MonoBehaviour
{
    /// <summary>
    /// Reference to the MainMenuController singleton.
    /// </summary>
    private MainMenuController instance;

    /// <summary>
    /// The game state.
    /// </summary>
    private GameState currentGameState;


    /// <summary>
    /// Sets up the Main Menu: <br></br>
    /// 
    /// (0) Sets Unity properties.<br></br>
    /// (1) Instantiates all factories and singletons.<br></br>
    /// </summary>
    void Start()
    {
        //(0) Set Unity Properties
        SceneController.SetUnityProperties();

        //(1) Instantiate all factories and singletons
        SetSingleton();
        MakeSingletons();
    }

    /// <summary>
    /// Main update loop: <br></br>
    /// 
    /// (1) Updates Game State. <br></br>
    /// (2) Checks for input events.<br></br>
    /// (3) Update Canvas.<br></br>
    /// </summary>
    void Update()
    {
        //(1) Updates Game State.
        if (DetermineGameState() == GameState.INVALID) return;

        //(2) Check input events.
        CheckInputEvents();

        //(3) Update Canvas.
        CanvasController.UpdateCanvas();
    }


    /// <summary>
    /// Instantiates the necessary Singletons for the Main Menu.
    /// </summary>
    private void MakeSingletons()
    {
        Assert.IsNotNull(instance, "MainMenuController singleton not set: call " +
            " SetSingleton()");

        CameraController.SetSingleton(instance);
        CanvasController.SetSingleton(instance);
        InputController.SetSingleton(instance);
        SceneController.SetSingleton(instance);
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
    /// Checks for input events.
    /// </summary>
    private void CheckInputEvents()
    {
        //UI Specific: does it matter if we're hovering over a UI element?
        if (InputController.HoveringOverUIElement())
        {
            //Put events here to trigger if we're hovering over a Canvas / UI element.
        }
        else
        {
            //Put events here to trigger if we're NOT hovering over a Canvas / UI element.
        }
    }

    /// <summary>
    /// Returns the current state of the game. Since this is the Main Menu,
    /// always returns MENU. Also informs controllers of this game state.
    /// </summary>
    /// <returns>the current GameState.</returns>
    private GameState DetermineGameState()
    {
        currentGameState = GameState.MENU;
        CanvasController.InformOfGameState(currentGameState);
        return GameState.MENU;
    }
}
