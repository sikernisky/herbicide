using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// The heart of view mode where the Player unlocks and chooses
/// Defenders. Responsible for <br></br>
/// 
/// (1) Setting up the collection menu and singletons<br></br>
/// (2) Running the main update loop <br></br>
/// (3) Detecting input and switching to main game (with
/// help of scene controller)
/// </summary>
public class CollectionMenuController : MonoBehaviour
{
    /// <summary>
    /// Reference to the CollectionMenuController singleton.
    /// </summary>
    private CollectionMenuController instance;

    /// <summary>
    /// The game state.
    /// </summary>
    private GameState currentGameState;

    /// <summary>
    /// The SkillSlot controllers to update.
    /// </summary>
    private List<SkillSlotController> skillSlotControllers;

    /// <summary>
    /// TEMPORARY field for unlocked skills while we wait for development of
    /// save data. 
    /// </summary>
    private List<SkillSlotScriptable.SkillSlotType> unlockedSkills;



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

        //(2) Load Skill Slots
        skillSlotControllers = SkillTreeManager.LoadSkillSlots();
        unlockedSkills = new List<SkillSlotScriptable.SkillSlotType>();
        unlockedSkills.Add(SkillSlotScriptable.SkillSlotType.SQUIRREL);
    }

    /// <summary>
    /// Main update loop: <br></br>
    /// 
    /// (1) Updates Game State. <br></br>
    /// (2) Checks for input events.<br></br>
    /// (3) Update Canvas.<br></br>
    /// (4) Update SkillSlotControllers.
    /// </summary>
    void Update()
    {
        //(1) Updates Game State.
        if (DetermineGameState() == GameState.INVALID) return;

        //(2) Check input events.
        CheckInputEvents();

        //(3) Update Canvas.
        CanvasController.UpdateCanvas(SceneController.GetFPS());

        //(4) Update SkillSlotControllers.
        instance.skillSlotControllers.ForEach(ssc => ssc.UpdateSlot(instance.unlockedSkills));
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
        SkillTreeManager.SetSingleton(instance);
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
