using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

/// <summary>
/// Manages all placement events in the game. This will replace the
/// PlacementController, which is a scrappy script that was created
/// as a proof of concept. This script will be more robust.
/// </summary>
public class PlacementManager : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// Reference to the PlacementManager singleton.
    /// </summary>
    private static PlacementManager Instance { get; set; }

    /// <summary>
    /// The number of IUseable objects that the player has equipped.
    /// </summary>
    public static int NumUseablesEquipped { get; private set; }

    /// <summary>
    /// true if the player is currently placing an object, false otherwise.
    /// </summary>
    public static bool IsPlacing { get; private set; }

    /// <summary>
    /// true if the player has registered a mouse down event, false otherwise.
    /// </summary>
    private bool MouseRegisteredDown { get; set; }

    /// <summary>
    /// true if the player has completed a buffer between the start of placement
    /// and the first frame of placement, false otherwise.
    /// </summary>
    private bool PlacementStartedBuffer { get; set; }

    /// <summary>
    /// The IUseable object that the player is currently placing.
    /// </summary>
    private IUseable CurrentPlaceable { get; set; }

    /// <summary>   
    /// Backing field for the dummy image component.
    /// </summary>
    [SerializeField]
    private Image _dummyImage;

    /// <summary>   
    /// The image component creating the placement preview.
    /// </summary>
    private Image DummyImage { get { return _dummyImage; } }

    /// <summary>
    /// Defines a delegate for completing a placement.
    /// </summary>
    public delegate void FinishPlacingDelegate();

    /// <summary>
    /// Action triggered when the player finishes placing an object.
    /// </summary>
    public static event Action<bool> OnPlacementFinished;


    #endregion

    #region Methods

    /// <summary>
    /// Finds and sets the PlacementManager singleton.
    /// </summary>
    /// <param name="levelController">the LevelController singleton.</param>
    public static void SetSingleton(LevelController levelController)
    {
        if (levelController == null) return;
        if (Instance != null) return;

        PlacementManager[] placementManagers = FindObjectsOfType<PlacementManager>();
        Assert.IsNotNull(placementManagers, "Array of PlacementManagers is null.");
        Assert.AreEqual(1, placementManagers.Length);
        Instance = placementManagers[0];
    }

    /// <summary>
    /// Updates the PlacementManager based on the current GameState.
    /// </summary>
    /// <param name="gameState">the current GameState.</param>
    public static void UpdatePlacementManager(GameState gameState)
    {
        if (gameState != GameState.ONGOING) Instance.StopPlacing(false);
        if (Instance.DidActionToStopPlacing()) Instance.StopPlacing(false);
        if (!IsPlacing) return;
        
        Instance.GluePlacementDummyToMouse();
        Instance.CheckAndHandleMouseExitDuringPlacement();
        Instance.CheckAndHandleMouseEnterDuringPlacement();
        Instance.CheckAndHandleMouseDownDuringPlacement();
        Instance.CheckAndHandleMouseUpDuringPlacement();
        Instance.PlacementStartedBuffer = true;
    }

    /// <summary>
    /// Glues the placement dummy to the mouse cursor. This is called every frame
    /// when the player is placing an object.
    /// </summary>
    private void GluePlacementDummyToMouse()
    {
        Assert.IsTrue(IsPlacing, "Player is not placing an object.");
        DummyImage.transform.position = InputManager.ScreenMousePosition;
    }

    /// <summary>
    /// Starts a placement event for the given IUseable object.
    /// </summary>
    /// <param name="useable">the given IUseable object.</param>
    public static void StartPlacing(IUseable useable)
    {
        if (Instance == null || useable == null) return;
        Instance.CurrentPlaceable = useable;
        Instance.SetupPlacementDummy(useable);
        IsPlacing = true;
    }

    /// <summary>
    /// Configures the placement dummy for the given IUseable object.
    /// </summary>
    /// <param name="useable">the object we are placing. </param>
    private void SetupPlacementDummy(IUseable useable)
    {
        Assert.IsNotNull(useable, "IUseable is null.");
        DummyImage.gameObject.SetActive(true);
        DummyImage.sprite = useable.GetPlacementTrack()[0]; // TODO: Animation
        DummyImage.rectTransform.sizeDelta = useable.GetPlacementSize();
        ShowPlacementDummy();
    }

    /// <summary>
    /// Shows the placement dummy image by setting its alpha to 1.
    /// </summary>
    private void ShowPlacementDummy()
    {
        Instance.DummyImage.color = new Color(
            Instance.DummyImage.color.r,
            Instance.DummyImage.color.g,
            Instance.DummyImage.color.b,
            1);
    }

    /// <summary>
    /// Stops placing the current IUseable object. Resets all placement variables
    /// to their default values.
    /// </summary>
    /// <param name="wasSuccessful">true if the placement was successful; otherwise, false.</param>
    private void StopPlacing(bool wasSuccessful)
    {
        OnPlacementFinished?.Invoke(wasSuccessful);
        IsPlacing = false;
        MouseRegisteredDown = false;
        DummyImage.gameObject.SetActive(false);
        DummyImage.sprite = null;
        CurrentPlaceable = null;
    }

    /// <summary>
    /// Returns true if the player did an action to stop placing an object.
    /// </summary>
    /// <returns>true if the player did an action to stop placing an object;
    /// otherwise, false.</returns>
    private bool DidActionToStopPlacing() => InputManager.DidKeycodeDown(KeyCode.Escape);

    /// <summary>
    /// Checks for a "mouse enter object" event during placement. If the player's
    /// mouse enters an object during placement, triggers the appropriate event.
    /// </summary>
    private void CheckAndHandleMouseEnterDuringPlacement()
    {
        if(!IsPlacing || !PlacementStartedBuffer) return;

        if (InputManager.TryGetNewlyHoveredWorldModel<ISurface>(out var surfaceEntered) && IsSurfaceSpecific()) OnMouseEnterSurfaceDuringPlacement(surfaceEntered);
        else if (InputManager.TryGetNewlyHoveredWorldModel<Model>(out var modelEntered)) OnMouseEnterModelDuringPlacement(modelEntered);
    }

    /// <summary>
    /// Triggers the appropriate event when the player's mouse enters a Surface
    /// during placement.
    /// </summary>
    /// <param name="surfaceEntered">The Surface that the player moused over.</param>
    private void OnMouseEnterSurfaceDuringPlacement(ISurface surfaceEntered)
    {
        Assert.IsTrue(IsSurfaceSpecific(), "IUseable is not Surface specific.");
        if (surfaceEntered == null) return;
        if (IsSurfacePlaceable()) OnMouseEnterSurfaceDuringPlacementAsSurfacePlaceable(surfaceEntered, CurrentPlaceable as ISurfacePlaceable);
        else if(IsSurfaceUseable()) OnMouseEnterSurfaceDuringPlacementAsSurfaceUseable(surfaceEntered, CurrentPlaceable as ISurfaceUseable);
    }

    /// <summary>
    /// Triggers the appropriate event when the player's mouse enters a Surface
    /// during placement as a surface placeable object.
    /// </summary>
    /// <param name="surfaceEntered">The Surface that the player is hovering over.</param>
    /// <param name="surfacePlaceable">The object that the player is placing.</param>
    private void OnMouseEnterSurfaceDuringPlacementAsSurfacePlaceable(ISurface surfaceEntered, ISurfacePlaceable surfacePlaceable)
    {
        Assert.IsNotNull(surfaceEntered, "Surface is null.");
        Assert.IsNotNull(surfacePlaceable, "ISurfacePlaceable is null.");
        Assert.AreEqual(CurrentPlaceable, surfacePlaceable, "ISurfacePlaceable is not current.");
        Assert.IsTrue(IsSurfacePlaceable(), "Placing object is not ISurfacePlaceable.");
        if(surfaceEntered.GhostPlace(surfacePlaceable)) HidePlacementDummy();
    }

    /// <summary>
    /// Hides the placement dummy image by setting its alpha to 0.
    /// </summary>
    private void HidePlacementDummy()
    {
        Instance.DummyImage.color = new Color(
            Instance.DummyImage.color.r,
            Instance.DummyImage.color.g,
            Instance.DummyImage.color.b,
            0);
    }

    /// <summary>
    /// Triggers the appropriate event when the player's mouse enters a Surface
    /// during placement as a Surface useable object.
    /// </summary>
    /// <param name="surfaceEntered">The Surface that the player is hovering over.</param>
    /// <param name="surfaceUseable">The object that the player is placing.</param>
    private void OnMouseEnterSurfaceDuringPlacementAsSurfaceUseable(ISurface surfaceEntered, ISurfaceUseable surfaceUseable)
    {
        Assert.IsNotNull(surfaceEntered, "Surface is null.");
        Assert.IsNotNull(surfaceUseable, "ISurfaceUsable is null.");
        Assert.AreEqual(CurrentPlaceable, surfaceUseable, "ISurfaceUsable is not current.");
        Assert.IsTrue(IsSurfaceUseable(), "Placing object is not ISurfaceUseable.");
        throw new System.NotImplementedException(); 
    }

    /// <summary>
    /// Triggers the appropriate event when the player's mouse enters a Model
    /// during placement.
    /// </summary>
    /// <param name="modelEntered">the Model that the player is hovering over.</param>
    private void OnMouseEnterModelDuringPlacement(Model modelEntered)
    {
        if (modelEntered == null) return;
        if(IsEquipable()) OnMouseEnterModelDuringPlacementAsEquipable(modelEntered, CurrentPlaceable as IEquipable);
    }

    /// <summary>
    /// Triggers the appropriate event when the player's mouse enters a Model
    /// during placement as an IEquipable object.
    /// </summary>
    /// <param name="modelEntered">the Model that the player is hovering over.</param>
    /// <param name="equipable">the IEquipable object that the player is placing.</param>
    private void OnMouseEnterModelDuringPlacementAsEquipable(Model modelEntered, IEquipable equipable)
    {
        Assert.IsNotNull(modelEntered, "Model is null.");
        Assert.IsNotNull(equipable, "IEquipable is null.");
        Assert.AreEqual(CurrentPlaceable, equipable, "IEquipable is not current.");
        Assert.IsTrue(IsEquipable(), "Placing object is not IEquipable.");
        if(modelEntered.CanEquip(equipable.EquipmentType)) modelEntered.SetColor(Color.red);
    }

    /// <summary>
    /// Checks for a "mouse exit object" event during placement. If the player's
    /// mouse exits an object during placement, triggers the appropriate event.
    /// </summary>
    private void CheckAndHandleMouseExitDuringPlacement()
    {
        if (!IsPlacing || !PlacementStartedBuffer) return;

        if (InputManager.TryGetPreviouslyHoveredWorldModel<ISurface>(out var surfaceExited) && IsSurfaceSpecific()) OnMouseExitSurfaceDuringPlacement(surfaceExited);
        else if (InputManager.TryGetPreviouslyHoveredWorldModel<Model>(out var modelExited)) OnMouseExitModelDuringPlacement(modelExited);
    }

    /// <summary>
    /// Triggers the appropriate event when the player's mouse exits a Surface
    /// during placement.
    /// </summary>
    /// <param name="surfaceExited">The Surface that the player moused off.</param>
    private void OnMouseExitSurfaceDuringPlacement(ISurface surfaceExited)
    {
        Assert.IsTrue(IsSurfaceSpecific(), "IUseable is not Surface specific.");
        if (surfaceExited == null) return;
        if (IsSurfacePlaceable()) OnMouseExitSurfaceDuringPlacementAsSurfacePlaceable(surfaceExited, CurrentPlaceable as ISurfacePlaceable);
        else if (IsSurfaceUseable()) OnMouseExitSurfaceDuringPlacementAsSurfaceUseable(surfaceExited, CurrentPlaceable as ISurfaceUseable);
    }

    /// <summary>
    /// Triggers the appropriate event when the player's mouse exits a Surface
    /// during placement as a Surface placeable object.
    /// </summary>
    /// <param name="surfaceHoveredOff">The Surface that the player is hovering off.</param>
    /// <param name="surfacePlaceable">The object that the player is placing.</param>
    private void OnMouseExitSurfaceDuringPlacementAsSurfacePlaceable(ISurface surfaceHoveredOff, ISurfacePlaceable surfacePlaceable)
    {
        Assert.IsNotNull(surfaceHoveredOff, "Surface is null.");
        Assert.IsNotNull(surfacePlaceable, "ISurfacePlaceable is null.");
        Assert.AreEqual(CurrentPlaceable, surfacePlaceable, "ISurfacePlaceable is not current.");
        Assert.IsTrue(IsSurfacePlaceable(), "Placing object is not ISurfacePlaceable.");
        surfaceHoveredOff.GhostRemove();
        ShowPlacementDummy();
    }

    /// <summary>
    /// Triggers the appropriate event when the player's mouse exits a Surface
    /// during placement as a Surface useable object.
    /// </summary>
    /// <param name="surfaceExited">The Surface that the player is hovering off.</param>
    /// <param name="surfaceUseable">The object that the player is placing.</param>
    private void OnMouseExitSurfaceDuringPlacementAsSurfaceUseable(ISurface surfaceExited, ISurfaceUseable surfaceUseable)
    {
        Assert.IsNotNull(surfaceExited, "Surface is null.");
        Assert.IsNotNull(surfaceUseable, "ISurfaceUsable is null.");
        Assert.AreEqual(CurrentPlaceable, surfaceUseable, "ISurfaceUsable is not current.");
        Assert.IsTrue(IsSurfaceUseable(), "Placing object is not ISurfaceUsable.");
    }

    /// <summary>
    /// Triggers the appropriate event when the player's mouse exits a Model
    /// during placement
    /// </summary>
    /// <param name="modelExited">the Model that the player is hovering off.</param>
    private void OnMouseExitModelDuringPlacement(Model modelExited)
    {
        if (modelExited == null) return;
        if (IsEquipable()) OnMouseExitModelDuringPlacementAsEquipable(modelExited, CurrentPlaceable as IEquipable);
    }

    /// <summary>
    /// Triggers the appropriate event when the player's mouse exits a Model
    /// during placement as an IEquipable object.
    /// </summary>
    /// <param name="modelExited">the Model that the player is hovering off.</param>
    /// <param name="equipable">the IEquipable object that the player is placing.</param>
    private void OnMouseExitModelDuringPlacementAsEquipable(Model modelExited, IEquipable equipable)
    {
        Assert.IsNotNull(modelExited, "Model is null.");
        Assert.IsNotNull(equipable, "IEquipable is null.");
        Assert.AreEqual(CurrentPlaceable, equipable, "IEquipable is not current.");
        Assert.IsTrue(IsEquipable(), "Placing object is not IEquipable.");
        modelExited.SetColor(Color.white);
    }

    /// <summary>
    /// Checks for a mouse down event during placement. If the player clicked down
    /// during placement, this method will register the mouse down event.
    /// </summary>
    private void CheckAndHandleMouseDownDuringPlacement()
    {
        if (!InputManager.DidPrimaryDown() || !IsPlacing) return;
        MouseRegisteredDown = true;
    }

    /// <summary>
    /// Checks for a mouse click up event during placement. If the player clicked
    /// up, this method will attempt to place the object. If the placement is successful,
    /// stops placing the object.
    /// </summary>
    private void CheckAndHandleMouseUpDuringPlacement()
    {
        if(!InputManager.DidPrimaryUp() || !IsPlacing || !PlacementStartedBuffer || !MouseRegisteredDown) return;
        bool successfulPlacement;
        if(IsSurfaceSpecific()) successfulPlacement = AttemptPlacementOnSurface(InputManager.CurrentHoveredWorldModel as ISurface);
        else successfulPlacement = AttemptPlacementOnModel(InputManager.CurrentHoveredWorldModel);
        if (successfulPlacement && IsSurfaceSpecific()) OnSuccessfulSurfacePlacement(InputManager.CurrentHoveredWorldModel as ISurface);    
        else if(successfulPlacement && !IsSurfaceSpecific()) OnSuccessfulModelPlacement(InputManager.CurrentHoveredWorldModel);
        if (successfulPlacement) OnSuccessfulPlacement();
    }

    /// <summary>
    /// Returns true if a placement attempt on the surface clicked up was successful.
    /// </summary>
    /// <returns>true if a placement attempt on the surface clicked up was successful;
    /// otherwise, false. </returns>
    /// <param name="surfaceClickedUp">the Surface that the player is placing the object on.</param>
    private bool AttemptPlacementOnSurface(ISurface surfaceClickedUp)
    {
        if (IsSurfacePlaceable()) return AttemptToPlaceOnSurface(surfaceClickedUp, CurrentPlaceable as ISurfacePlaceable);
        else if (IsSurfaceUseable()) return AttemptToUseOnSurface(surfaceClickedUp, CurrentPlaceable as ISurfaceUseable);
        return false;
    }

    /// <summary>
    /// Returns true if an attempt to place the current ISurfacePlaceable object 
    /// was successful.
    /// </summary>
    /// <param name="surfaceClickedUp">the Surface that the player is placing the object on.</param>
    /// <param name="surfacePlaceable">the ISurfacePlaceable object that the player is placing.</param>
    /// <returns>true if an attempt to place the current ISurfacePlaceable object 
    /// was successful; otherwise, false.</returns>
    private bool AttemptToPlaceOnSurface(ISurface surfaceClickedUp, ISurfacePlaceable surfacePlaceable)
    {
        if(surfaceClickedUp == null) return false;
        Assert.IsNotNull(surfacePlaceable, "surfacePlaceable is null.");
        Assert.AreEqual(CurrentPlaceable, surfacePlaceable, "surfacePlaceable is not current.");
        Assert.IsTrue(IsSurfacePlaceable(), "IUseable is not tile specific.");
        return surfaceClickedUp.Place(surfacePlaceable);
    }

    /// <summary>
    /// Returns true if an attempt to use the current ISurfaceUseable object 
    /// was successful.
    /// </summary>
    /// <param name="surfaceClickedUp">the Surface that the player is using the object on.</param>
    /// <param name="surfaceUseable">the ISurfaceUseable object that the player is placing.</param>
    /// <returns>true if an attempt to use the current ISurfaceUseable object 
    /// was successful; otherwise, false.</returns>
    private bool AttemptToUseOnSurface(ISurface surfaceClickedUp, ISurfaceUseable surfaceUseable)
    {
        if (surfaceClickedUp == null) return false;
        Assert.IsNotNull(surfaceUseable, "surfacePlaceable is null.");
        Assert.AreEqual(CurrentPlaceable, surfaceUseable, "surfacePlaceable is not current.");
        Assert.IsTrue(IsSurfaceUseable(), "IUseable is not SurfaceUseable.");
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Returns true if a placement attempt on the model clicked up was successful.
    /// </summary>
    /// <param name="modelClickedUp">the Model that the player is placing the object on.</param>
    /// <returns>true if a placement attempt on the model clicked up was successful;
    /// otherwise, false.</returns>
    private bool AttemptPlacementOnModel(Model modelClickedUp)
    {
        if (IsEquipable()) return AttemptToEquipOnModel(modelClickedUp, CurrentPlaceable as IEquipable);
        else return true;
    }

    /// <summary>
    /// Returns true if an attempt to equip the current IEquipable object
    /// was successful.
    /// </summary>
    /// <param name="modelClickedUp">the Model that the player is equipping the object on.</param>
    /// <param name="equipable">the IEquipable object that the player is placing.</param>
    /// <returns>true if an attempt to equip the current IEquipable object
    /// was successful; otherwise, false.</returns>
    private bool AttemptToEquipOnModel(Model modelClickedUp, IEquipable equipable)
    {
        if (modelClickedUp == null) return false;
        Assert.IsNotNull(equipable, "equipable is null.");
        Assert.AreEqual(CurrentPlaceable, equipable, "equipable is not current.");
        Assert.IsTrue(IsEquipable(), "IUseable is not tile equipable.");
        if(!modelClickedUp.CanEquip(equipable.EquipmentType)) return false;
        modelClickedUp.Equip(equipable.EquipmentType);
        NumUseablesEquipped++;
        return true;
    }

    /// <summary>
    /// Called when the player successfully places an object. Calls general
    /// end-of-placement events.
    /// </summary>
    private void OnSuccessfulPlacement()
    {
        CurrentPlaceable.OnPlace(InputManager.WorldMousePosition);
        StopPlacing(true);
    }

    /// <summary>
    /// Called when a ISurfacePlaceable object is successfully placed.
    /// </summary>
    /// <param name="surfacePlacedOn">the Surface that the player placed the object on.</param>
    private void OnSuccessfulSurfacePlacement(ISurface surfacePlacedOn)
    {
        Assert.IsNotNull(surfacePlacedOn, "surfacePlacedOn is null.");
        Assert.IsTrue(IsSurfaceSpecific(), "IUseable is not tile specific.");
        surfacePlacedOn.GhostRemove();
    }

    /// <summary>
    /// Called when a Model is successfully placed on.
    /// </summary>
    /// <param name="modelPlacedOn">the Model that the player placed the object on.</param>
    private void OnSuccessfulModelPlacement(Model modelPlacedOn)
    {
        Assert.IsNotNull(modelPlacedOn, "modelPlacedOn is null.");
        if(IsEquipable()) modelPlacedOn.SetColor(Color.white);
    }

    /// <summary>
    /// Returns true if the current IUseable object is a surface placement object.
    /// </summary>
    /// <returns>true if the current IUseable object is a surface placement object;
    /// otherwise, false. </returns>
    private bool IsSurfaceSpecific() => CurrentPlaceable is ISurfacePlaceable ||
                                        CurrentPlaceable is ISurfaceUseable;

    /// <summary>
    /// Returns true if the current IUseable object is a surface placement object.
    /// </summary>
    /// <returns>true if the current IUseable object is a surface placement object;
    /// otherwise, false.</returns>
    private bool IsSurfacePlaceable() => CurrentPlaceable is ISurfacePlaceable;

    /// <summary>
    /// Returns true if the current IUseable object is a surface useable object.
    /// </summary>
    /// <returns>true if the current IUseable object is a surface useable object;
    /// otherwise, false. </returns>
    private bool IsSurfaceUseable() => CurrentPlaceable is ISurfaceUseable;

    /// <summary>
    /// Returns true if the current IUseable object is a surface equipable object.
    /// </summary>
    /// <returns>true if the current IUseable object is a surface equipable object;
    /// otherwise, false. </returns>
    private bool IsEquipable() => CurrentPlaceable is IEquipable;

    #endregion
}
