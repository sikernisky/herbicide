using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

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
    /// All controllers this ModelController has not yet passed
    /// to the ControllerController.
    /// </summary>
    private List<ModelController> volatileControllers = new List<ModelController>();

    /// <summary>
    /// true if this Model will be destroyed at the end of the frame;
    /// otherwise, false.
    /// </summary>
    private bool scheduledForDestruction;



    /// <summary>
    /// Makes a new ModelController for a Model.
    /// </summary>
    /// <param name="model">The Model controlled by
    ///  this ModelController.</param>
    /// coroutine
    public ModelController(Model model)
    {
        Assert.IsNotNull(model, "Model is null.");
        SetModel(model);
        GetModel().ResetStats();
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
        GetModel().UpdateEffects();
        if (ShouldRemoveModel()) DestroyAndRemoveModel();
        UpdateTilePosition();
        CheckModelClickUp();
        UpdateSynergies();
    }

    /// <summary>
    /// Finds the coordinates of the Tile(s) the Model is on.
    /// </summary>
    private void UpdateTilePosition()
    {
        if (!ValidModel()) return;
        Vector2 worldPos = GetModel().GetPosition();
        int tileX = TileGrid.PositionToCoordinate(worldPos.x);
        int tileY = TileGrid.PositionToCoordinate(worldPos.y);
        GetModel().SetTileCoordinates(tileX, tileY);
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
        if (GetModel() == null || System.Object.Equals(null, GetModel())) return;

        ALL_MODELS.Remove(GetModel());
        DestroyModel();
        model = null;
    }

    /// <summary>
    /// Returns true if this Controller's Model is "valid" -- what is valid
    /// is determined by inheriting controllers. 
    /// </summary>
    /// <returns>true if this Controller's Model is valid; otherwise,
    /// false.</returns>
    public virtual bool ValidModel()
    {
        return !scheduledForDestruction && GetModel() != null &&
            !System.Object.Equals(GetModel(), null);
    }

    /// <summary>
    /// Returns true if the model is no longer needed and
    /// should be removed by the ControllerController.<br></br>
    /// 
    /// By default, this returns true if the model controlled by this
    /// Controller is NULL.
    /// </summary>
    /// <returns>true if the model is no longer needed and
    /// should be removed by the ControllerController; otherwise,
    /// returns false.</returns>
    public virtual bool ShouldRemoveController() { return !ValidModel(); }

    /// <summary>
    /// Returns true if this Controller's model should be destroyed
    /// and set to null.
    /// </summary>
    /// <returns>true if this Controller's model should be destroyed
    /// and set to null; otherwise, false. </returns>
    protected abstract bool ShouldRemoveModel();

    /// <summary>
    /// Returns a new list of ModelControllers that this ModelController has
    /// created but not yet passed to the ControllerController. Then, wipes
    /// the original list.
    /// </summary>
    /// <returns>a list of ModelControllers that this ModelController has
    /// created but not yet passed to the ControllerController.</returns>
    public List<ModelController> ExtricateControllers()
    {
        List<ModelController> copied =
            new List<ModelController>(volatileControllers);
        volatileControllers.Clear();
        return copied;
    }

    /// <summary>
    /// Adds a ModelController to the list of ModelControllers that
    /// need to be extricated by the ControllerController.
    /// </summary>
    /// <param name="modelController">The ModelController to add.</param>
    protected void AddController(ModelController modelController)
    {
        Assert.IsNotNull(modelController, "ProjectileController is null.");
        volatileControllers.Add(modelController);
    }

    /// <summary>
    /// Handles a collision between this controller's model and
    /// some other Collider.
    /// </summary>
    /// <param name="other">the other collider.</param>
    protected abstract void HandleCollision(Collider2D other);

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
    /// Returns a list of all Models that are also ITargetables
    /// (all of them). 
    /// </summary>
    /// <returns>a list of all ITargetables in the scene.</returns>
    public static List<ITargetable> GetAllTargetableObjects()
    {
        List<ITargetable> allTargetables = new List<ITargetable>();
        foreach (Model model in ALL_MODELS)
        {
            ITargetable targetable = model as ITargetable;
            if (targetable != null) allTargetables.Add(targetable);
        }
        return allTargetables;
    }

    /// <summary>
    /// Checks if the player clicked on this Model.
    /// </summary>
    private void CheckModelClickUp()
    {
        if (clicked) return;
        if (!ValidModel()) return;
        if (InputController.ModelClickedUp(GetModel())) clicked = true;
    }

    /// <summary>
    /// Returns true if the player has clicked on this Model since the
    /// scene began.
    /// </summary>
    /// <returns>true if the player has clicked on this Model since the
    /// scene began; otherwise, false.</returns>
    protected bool ClickedOn() { return clicked; }

    /// <summary>
    /// Schedules this Model for destruction and destroys it at the
    /// end of the current frame.
    /// </summary>
    public void DestroyModel()
    {
        if (scheduledForDestruction) return;
        scheduledForDestruction = true;
        GameObject.Destroy(GetModel().gameObject);
    }
}
