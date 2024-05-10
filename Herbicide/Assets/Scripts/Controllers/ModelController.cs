using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System.Linq;

public abstract class ModelController
{
    /// <summary>
    /// This Controller's Model.
    /// </summary>
    private Model model;

    /// <summary>
    /// Most recent GameState.
    /// </summary>
    private GameState gameState;

    /// <summary>
    /// The Controller's ID. 
    /// </summary>
    private int id;

    /// <summary>
    /// true if the player clicked on the Model since it was
    /// assigned to this ModelController; otherwise, false.
    /// </summary>
    private bool clicked;

    /// <summary>
    /// All active Models in the scene.
    /// </summary>
    private static List<Model> ALL_MODELS = new List<Model>();

    /// <summary>
    /// All ModelControllers this ModelController has not yet passed
    /// to the ControllerController.
    /// </summary>
    private List<ModelController> volatileModelControllers = new List<ModelController>();

    /// <summary>
    /// All EmanationControllers this ModelController has not yet passed
    /// to the ControllerController.
    /// </summary>
    private List<EmanationController> volatileEmanationControllers = new List<EmanationController>();

    /// <summary>
    /// Number of active Models of each ModelType.
    /// </summary>
    private ModelCounts counts;

    /// <summary>
    /// true if this Model dropped its death loot; otherwise, false.
    /// </summary>
    private bool droppedDeathLoot;

    /// <summary>
    /// true if the Model has been deposited back into its Factory's ObjectPool.
    /// </summary>
    private bool modelRemoved;


    /// <summary>
    /// Makes a new ModelController for a Model.
    /// </summary>
    /// <param name="model">The Model controlled by this ModelController.</param>
    public ModelController(Model model)
    {
        Assert.IsNotNull(model, "Model is null.");
        SetModel(model);
        GetModel().ResetModel();
        GetModel().SubscribeToCollision(HandleCollision);
        ApplySynergies();
        id = ALL_MODELS.Count;
        ALL_MODELS.Add(model);
    }

    /// <summary>
    /// Returns this ModelController's Model.
    /// </summary>
    /// <returns>this ModelController's Model.</returns>
    public Model GetModel() { return model; }

    /// <summary>
    /// Sets this Controller's Model.
    /// </summary>
    /// <param name="model">The new Model. </param>
    protected void SetModel(Model model)
    {
        Assert.IsNotNull(model, "Cannot set Model as null.");
        this.model = model;
    }

    /// <summary>
    /// Main update loop for this Controller's model. 
    /// </summary>
    public virtual void UpdateModel()
    {
        if (!ValidModel()) return;

        GetModel().UpdateEffects();
        UpdateTilePositions();
        ModelClickedUp();
        //UpdateSynergies();
        if (GetModel().PickedUp()) GetModel().SetWorldPosition(GetModel().GetHeldPosition());
        FixSortingOrder();
    }

    /// <summary>
    /// Finds the coordinates of the Tile(s) the Model is on.
    /// </summary>
    private void UpdateTilePositions()
    {
        if (!ValidModel()) return;

        // Placed tile position
        Vector2 worldPos = GetModel().GetPosition();
        int tileX = TileGrid.PositionToCoordinate(worldPos.x);
        int tileY = TileGrid.PositionToCoordinate(worldPos.y);
        GetModel().SetTileCoordinates(tileX, tileY);

        // Expanded tile position
        GetModel().WipeExpandedCoordinates();
        int placedX = GetModel().GetX();
        int placedY = GetModel().GetY();
        for (int x = placedX; x < placedX + GetModel().SIZE.x; x++)
        {
            for (int y = placedY; y < placedY + GetModel().SIZE.y; y++)
            {
                GetModel().AddExpandedTileCoordinate(x, y);
            }
        }
    }

    /// <summary>
    /// Queries the SynergyController to determine which Synergies are
    /// active. Performs logic based on the active Synergies.
    /// </summary>
    protected virtual void UpdateSynergies() { return; }

    /// <summary>
    /// Adds to the Model all Synergy effects that could affect it.
    /// </summary>
    protected virtual void ApplySynergies() { return; }

    /// <summary>
    /// Destroys and detatches the Model from this Controller.
    /// </summary>
    protected virtual void DestroyAndRemoveModel()
    {
        if (GetModel() == null || modelRemoved) return;

        DropDeathLoot();
        OnDestroyModel();

        ALL_MODELS.Remove(GetModel());
        DestroyModel();
        model = null;
        modelRemoved = true;
    }


    /// <summary>
    /// Drops loot from this Mob when it dies. 
    /// </summary>
    protected virtual void DropDeathLoot() { droppedDeathLoot = true; }

    /// <summary>
    /// Returns true if this Model dropped its death loot.
    /// </summary>
    /// <returns>true if this Model dropped its death loot; otherwise,
    /// false. </returns>
    protected bool DroppedDeathLoot() { return droppedDeathLoot; }

    /// <summary>
    /// Updates the Model's sorting order so that it appears behind Models
    /// before it and before Models behind it.
    /// </summary>
    protected virtual void FixSortingOrder()
    {
        if (!ValidModel()) return;

        GetModel().SetSortingOrder(-Mathf.FloorToInt(GetModel().GetPosition().y));
    }

    /// <summary>
    /// Returns true if this Controller's Model is "valid" -- what is valid
    /// is determined by inheriting controllers. 
    /// </summary>
    /// <returns>true if this Controller's Model is valid; otherwise,
    /// false.</returns>
    public abstract bool ValidModel();

    /// <summary>
    /// Returns true if this Controller should be removed. If so,
    /// destroys its Model. This happens when the Model is invalid.
    /// <br></br>
    /// 
    /// If the model is valid, does nothing.
    /// </summary>
    /// <returns>true if this Controller should be removed; otherwise,
    /// false. </returns>
    public bool TryRemoveController()
    {
        // Not ready to remove yet.
        if (ValidModel()) return false;

        // Model is invalid; remove it.
        DestroyAndRemoveModel();
        return true;
    }

    /// <summary>
    /// Returns a new list of ModelControllers that this ModelController has
    /// created but not yet passed to the ControllerController. Then, wipes
    /// the original list.
    /// </summary>
    /// <returns>a list of ModelControllers that this ModelController has
    /// created but not yet passed to the ControllerController.</returns>
    public List<ModelController> ExtricateModelControllers()
    {
        List<ModelController> copied =
            new List<ModelController>(volatileModelControllers);
        volatileModelControllers.Clear();
        return copied;
    }

    /// <summary>
    /// Returns a new list of EmanationContrllers that this ModelController has
    /// created but not yet passed to the ControllerController. Then, wipes
    /// the original list.
    /// </summary>
    /// <returns>a list of EmanationContrllers that this ModelController has
    /// created but not yet passed to the ControllerController.</returns>
    public List<EmanationController> ExtricateEmanationControllers()
    {
        List<EmanationController> copied =
            new List<EmanationController>(volatileEmanationControllers);
        volatileEmanationControllers.Clear();
        return copied;
    }

    /// <summary>
    /// Returns true if this ModelController has Controllers that the
    /// ControllerController should collect.
    /// </summary>
    /// <returns>true if this ModelController has Controllers that the
    /// ControllerController should collect; otherwise, false. </returns>
    public bool NeedsExtricating()
    {
        return volatileModelControllers.Count > 0 ||
        volatileEmanationControllers.Count > 0;
    }

    /// <summary>
    /// Adds a ModelController to the list of ModelControllers that
    /// need to be extricated by the ControllerController.
    /// </summary>
    /// <param name="modelController">The ModelController to add.</param>
    protected void AddModelControllerForExtrication(ModelController modelController)
    {
        Assert.IsNotNull(modelController, "ModelController is null.");
        volatileModelControllers.Add(modelController);
    }

    /// <summary>
    /// Adds an EmanationController to the list of EmanationControllers that
    /// need to be extricated by the ControllerController.
    /// </summary>
    /// <param name="modelController">The EmanationController to add.</param>
    protected void AddEmanationControllerForExtrication(EmanationController emanationController)
    {
        Assert.IsNotNull(emanationController, "EmanationController is null.");
        volatileEmanationControllers.Add(emanationController);
    }

    /// <summary>
    /// Handles a collision between this controller's model and
    /// some other Collider.
    /// </summary>
    /// <param name="other">the other collider.</param>
    protected abstract void HandleCollision(Collider2D other);

    /// <summary>
    /// Informs this ModelController of the number of active Models of
    /// each type.
    /// </summary>
    /// <param name="counts">The number of active Models of each ModelType.</param>
    public void InformOfModelCounts(ModelCounts counts) { this.counts = counts; }

    /// <summary>
    /// Returns the number of active Models of each type.
    /// </summary>
    /// <returns>The number of active Models of each type.</returns>
    protected ModelCounts GetModelCounts() { return counts; }

    /// <summary>
    /// Informs this ModelController of the most recent GameState so
    /// that it knows how to update its Model.
    /// </summary>
    /// <param name="state">The most recent GameState.</param>
    public void InformOfGameState(GameState state) { gameState = state; }

    /// <summary>
    /// Returns the most recent GameState recognized by this ModelController.
    /// </summary>
    /// <returns>the most recent GameState.</returns>
    protected GameState GetGameState() { return gameState; }

    /// <summary>
    /// Adds one chunk of Time.deltaTime to the animation
    /// counter that tracks the current state.
    /// </summary>
    public abstract void AgeAnimationCounter();

    /// <summary>
    /// Returns the animation counter for the current state.
    /// </summary>
    /// <returns>the animation counter for the current state.</returns>
    public abstract float GetAnimationCounter();

    /// <summary>
    /// Sets the animation counter for the current state to 0.
    /// </summary>
    public abstract void ResetAnimationCounter();

    /// <summary>
    /// Checks to see if the next frame in the animation needs to be
    /// displayed. If so, displays it.
    /// </summary>
    public virtual void StepAnimation()
    {
        AgeAnimationCounter();
        float stepTime = GetModel().CurrentAnimationDuration / GetModel().NumFrames();
        if (GetAnimationCounter() - stepTime > 0)
        {
            GetModel().NextFrame();
            ResetAnimationCounter();
        }
    }

    /// <summary>
    /// Returns a list of all active Models.
    /// </summary>
    /// <returns>a list of all Models in the scene.</returns>
    public static IReadOnlyList<Model> GetAllModels() { return ALL_MODELS.AsReadOnly(); }

    /// <summary>
    /// Checks if the player clicked on this Model.
    /// </summary>
    protected bool ModelClickedUp()
    {
        if (!ValidModel()) return false;
        bool clickedUp = InputController.ModelClickedUp(GetModel());
        clicked = false ? clickedUp : true;
        return clickedUp;
    }

    /// <summary>
    /// Returns true if the player has clicked on this Model since the
    /// scene began.
    /// </summary>
    /// <returns>true if the player has clicked on this Model since the
    /// scene began; otherwise, false.</returns>
    protected bool ClickedOnSinceSceneBegan() { return clicked; }

    /// <summary>
    /// Destroys the model. This should be done by a sub class, who gives
    /// the prefab back to the correct Factory.
    /// </summary>
    public abstract void DestroyModel();

    /// <summary>
    /// Performs logic right before this Model is destroyed.
    /// </summary>
    protected virtual void OnDestroyModel() { return; }
}
