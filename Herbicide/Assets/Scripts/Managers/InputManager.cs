using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Detects and provides information about player input.
/// </summary>
public class InputManager : MonoBehaviour
{
    /// <summary>
    /// Reference to the InputManager singleton.
    /// </summary>
    private static InputManager Instance { get; set; }

    /// <summary>
    /// The last hovered Model.
    /// </summary>
    public static Model PreviousHoveredWorldModel { get; private set; }

    /// <summary>
    /// The currently hovered Model.
    /// </summary>
    public static Model CurrentHoveredWorldModel { get; private set; }

    /// <summary>
    /// The last hovered UI GameObject.
    /// </summary>
    public static UIModel PreviousHoveredUIModel { get; private set; }

    /// <summary>
    /// The currently hovered UI GameObject.
    /// </summary>
    public static UIModel CurrentHoveredUIModel { get; private set; }

    /// <summary>
    /// The player's mouse position
    /// </summary>
    public static Vector3 ScreenMousePosition { get; private set; }

    /// <summary>
    /// The player's mouse position in world coordinates
    /// </summary>
    public static Vector3 WorldMousePosition { get; private set; }

    /// <summary>
    /// Storage for RaycastHit2D results.
    /// </summary>
    private RaycastHit2D[] HitTemp { get; set; }

    /// <summary>
    /// Storage for UI Raycast results.
    /// </summary>
    private List<RaycastResult> RaycastResults { get; set; }

    /// <summary>
    /// Backing field for the GraphicRaycaster property.
    /// </summary>
    [SerializeField]
    private GraphicRaycaster _graphicRaycaster;

    /// <summary>
    /// The GraphicRaycaster object in the scene.
    /// </summary>
    private GraphicRaycaster GraphicRaycaster { get { return _graphicRaycaster; } }

    /// <summary>
    /// Backing field for the PointerEventData property.
    /// </summary>
    [SerializeField]
    private EventSystem _eventSystem;

    /// <summary>
    /// Component to manage and route UI input events to objects in the scene.
    /// </summary>
    private EventSystem EventSystem { get { return _eventSystem; } }

    /// <summary>
    /// Contains all information about the pointer when an event is triggered.
    /// </summary>
    private PointerEventData PointerEventData { get; set; }


    /// <summary>
    /// Finds and sets the InputController singleton for a level. Also instantiates the
    /// InputController's temporary objects.
    /// </summary>
    /// <param name="levelController">The LevelController singleton.</param>
    public static void SetSingleton(LevelController levelController)
    {
        if (levelController == null) return;
        if (Instance != null) return;

        InputManager[] inputManagers = FindObjectsOfType<InputManager>();
        Assert.IsNotNull(inputManagers, "Array of InputManagers is null.");
        Assert.AreEqual(1, inputManagers.Length);
        Instance = inputManagers[0];
        Instance.MakeTempObjects();
    }

    /// <summary>
    /// Initializes all objects when this InputController controller is instantiated
    /// to save memory.
    /// </summary>
    private void MakeTempObjects()
    {
        Assert.IsNotNull(GraphicRaycaster, "GraphicRaycaster is null.");
        Assert.IsNotNull(EventSystem, "EventSystem is null.");
        ScreenMousePosition = new Vector3();
        WorldMousePosition = new Vector3();
        RaycastResults = new List<RaycastResult>();
        PointerEventData = new PointerEventData(EventSystem);
        HitTemp = new RaycastHit2D[PhysicsConstants.MaxRaycastHits];
    }

    /// <summary>
    /// Main loop for the InputManager. Detects and provides information about player input.
    /// </summary>
    /// <param name="levelController">The LevelController singleton.</param>
    public static void UpdateInputManager()
    {
        Instance.UpdateMousePositions();
        Instance.DetectHoveredModel();
        Instance.DetectHoveredUIElement();
    }

    /// <summary>
    /// Updates screen and world mouse positions.
    /// </summary>
    private void UpdateMousePositions()
    {
        ScreenMousePosition = Input.mousePosition;
        WorldMousePosition = Camera.main.ScreenToWorldPoint(ScreenMousePosition);
    }

    /// <summary>
    /// Detects the model that is currently hovered by the mouse.
    /// </summary>
    private void DetectHoveredModel()
    {
        PreviousHoveredWorldModel = CurrentHoveredWorldModel;
        CurrentHoveredWorldModel = GetHoveredModel();
    }

    /// <summary>
    /// Detects the UI element that is currently hovered by the mouse.
    /// </summary>
    private void DetectHoveredUIElement()
    {
        PreviousHoveredUIModel = CurrentHoveredUIModel;
        CurrentHoveredUIModel = GetHoveredUIElement();
    }

    /// <summary>
    /// Performs a 2D raycast and returns the topmost hovered UI element.
    /// </summary>
    /// <returns>the topmost hovered UI element.</returns>
    private UIModel GetHoveredUIElement()
    {
        PointerEventData.position = ScreenMousePosition;
        RaycastResults.Clear();
        GraphicRaycaster.Raycast(PointerEventData, RaycastResults);

        return RaycastResults.Count > 0 ? RaycastResults[0].gameObject.GetComponent<UIModel>() : null;
    }

    /// <summary>
    /// Performs a 2D raycast and returns the topmost hovered Model.
    /// </summary>
    /// <returns>The hovered Model, or null if none is detected.</returns>
    private Model GetHoveredModel()
    {
        ContactFilter2D filter = new ContactFilter2D();
        filter.useTriggers = true;
        int hitCount = Physics2D.Raycast(WorldMousePosition, Vector2.zero, filter, HitTemp);
        Model topModel = null;
        float closestZ = float.MaxValue;

        for (int i = 0; i < hitCount; i++)
        {
            Model model = HitTemp[i].collider.GetComponent<Model>();
            if (model == null) continue;

            float modelZ = model.transform.position.z;
            if (modelZ < closestZ)
            {
                topModel = model;
                closestZ = modelZ;
            }
        }
        return topModel;
    }

    /// <summary>
    /// Returns true if the currently hovered world model has changed and attempts to retrieve it as the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the world model to check for.</typeparam>
    /// <param name="hoveredModel">The newly hovered model of type <typeparamref name="T"/> if the condition is met; otherwise, null.</param>
    /// <returns>True if the hovered world model has changed and is of type <typeparamref name="T"/>, otherwise false.</returns>
    public static bool TryGetNewlyHoveredWorldModel<T>(out T hoveredModel) where T : class
    {
        if (CurrentHoveredWorldModel != null && CurrentHoveredWorldModel != PreviousHoveredWorldModel && CurrentHoveredWorldModel is T model)
        {
            hoveredModel = model;
            return true;
        }
        hoveredModel = null;
        return false;
    }

    /// <summary>
    /// Checks if the previously hovered world model has changed and attempts to retrieve it as the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the world model to check for.</typeparam>
    /// <param name="exitedModel">The previously hovered model of type <typeparamref name="T"/> if the condition is met; otherwise, null.</param>
    /// <returns>True if the previously hovered world model has changed and is of type <typeparamref name="T"/>, otherwise false.</returns>
    public static bool TryGetPreviouslyHoveredWorldModel<T>(out T exitedModel) where T : class
    {
        if (PreviousHoveredWorldModel != null && PreviousHoveredWorldModel != CurrentHoveredWorldModel && PreviousHoveredWorldModel is T model)
        {
            exitedModel = model;
            return true;
        }
        exitedModel = null;
        return false;
    }

    /// <summary>
    /// Returns true if the currently hovered UI model has changed and attempts to retrieve it as the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the UI model to check for.</typeparam>
    /// <param name="hoveredModel">The newly hovered model of type <typeparamref name="T"/> if the condition is met; otherwise, null.</param>
    /// <returns>True if the hovered UI model has changed and is of type <typeparamref name="T"/>, otherwise false.</returns>
    public static bool TryGetNewlyHoveredUIModel<T>(out T hoveredModel) where T : class
    {
        if (CurrentHoveredUIModel != null && CurrentHoveredUIModel != PreviousHoveredUIModel && CurrentHoveredUIModel is T model)
        {
            hoveredModel = model;
            return true;
        }
        hoveredModel = null;
        return false;
    }

    /// <summary>
    /// Checks if the previously hovered UI model has changed and attempts to retrieve it as the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the UI model to check for.</typeparam>
    /// <param name="exitedModel">The previously hovered model of type <typeparamref name="T"/> if the condition is met; otherwise, null.</param>
    /// <returns>True if the previously hovered UI model has changed and is of type <typeparamref name="T"/>, otherwise false.</returns>
    public static bool TryGetPreviouslyHoveredUIModel<T>(out T exitedModel) where T : class
    {
        if (PreviousHoveredUIModel != null && PreviousHoveredUIModel != CurrentHoveredUIModel && PreviousHoveredUIModel is T model)
        {
            exitedModel = model;
            return true;
        }
        exitedModel = null;
        return false;
    }

    /// <summary>
    /// Returns true if the player has clicked their primary button up (PC: mouse) this
    /// frame
    /// </summary>
    /// <returns>true if the player clicked their primary up; otherwise, false. </returns>
    public static bool DidPrimaryUp() => Input.GetMouseButtonUp(0);

    /// <summary>
    /// Returns true if the player has clicked their primary button down (PC: mouse) this\
    /// frame.
    /// </summary>
    /// <returns>true if the player clicked their primary down; otherwise, false. </returns>
    public static bool DidPrimaryDown() => Input.GetMouseButtonDown(0);

    /// <summary>
    /// Returns true if the player pressed some keycode down this frame.
    /// </summary>
    /// <returns>true if the player pressed some keycode down this frame; otherwise, false.</returns>
    public static bool DidKeycodeDown(KeyCode keyCode) => Input.GetKeyDown(keyCode);
}
