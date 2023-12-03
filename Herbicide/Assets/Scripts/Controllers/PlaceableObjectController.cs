using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Abstract class to represent controllers of PlaceableObjects.<br></br>
/// 
/// The PlaceableObjectController is responsible for manipulating its PlaceableObject
/// and bringing it to life.
/// /// </summary>
public abstract class PlaceableObjectController
{
    /// <summary>
    /// This Controller's PlaceableObject model.
    /// </summary>
    private PlaceableObject model;

    /// <summary>
    /// Unique ID of this Controller.
    /// </summary>
    private int id;

    /// <summary>
    /// Most recent GameState.
    /// </summary>
    private GameState gameState;

    /// <summary>
    /// All instantiated PlaceableObjects.
    /// </summary>
    private static List<PlaceableObject> ALL_PLACEABLES = new List<PlaceableObject>();

    /// <summary>
    /// The color strength, from 0-1, of the damage flash animation.
    /// </summary>
    private const float FLASH_INTENSITY = .4f;

    /// <summary>
    /// The total time in seconds, from start to finish, of a damage flash
    /// animation.
    /// </summary>
    private const float FLASH_DURATION = .4f;


    /// <summary>
    /// Makes a new PlaceableObjectController for a PlaceableObject.
    /// </summary>
    /// <param name="placeableObject">The PlaceableObject controlled by
    ///  this PlaceableObjectController.</param>
    /// coroutine
    public PlaceableObjectController(PlaceableObject placeableObject)
    {
        Assert.IsNotNull(placeableObject, "PlaceableObject is null.");
        SetModel(placeableObject);
        GetModel().ResetStats();
        GetModel().SubscribeToCollision(HandleCollision);
        id = ALL_PLACEABLES.Count;
        ALL_PLACEABLES.Add(placeableObject);
    }

    /// <summary>
    /// Main update loop for this Controller's model. 
    /// </summary>
    /// <param name="targets">A complete list of ITargetables in the scene.</param>
    public virtual void UpdateModel() { UpdateDamageFlash(); }

    /// <summary>
    /// Informs this MobController of the most recent GameState so
    /// that it knows how to update its Mob.
    /// </summary>
    /// <param name="state">The most recent GameState.</param>
    public void InformOfGameState(GameState state) { gameState = state; }

    /// <summary>
    /// Returns the most recent GameState recognized by this MobController.
    /// </summary>
    /// <returns>the most recent GameState.</returns>
    protected GameState GetGameState() { return gameState; }

    /// <summary>
    /// Checks if this Controllers's model should be removed from
    /// the scene. If so, removes it.
    /// </summary>
    protected abstract void TryRemoveModel();

    /// <summary>
    /// Returns true if this Controller's model is "valid" -- what is valid
    /// is determined by inheriting controllers. 
    /// </summary>
    /// <returns>true if this Controller's model is valid; otherwise,
    /// false.</returns>
    public abstract bool ValidModel();

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
    /// Returns this Controller's PlaceableObject model. Inheriting controller
    /// classes use this method to access their PlaceableObject; then, they cast
    /// it to its respective type.
    /// </summary>
    /// <returns>this PlaceableObjectControllers's PlaceableObject model.</returns>
    public PlaceableObject GetModel() { return model; }

    /// <summary>
    /// Detatches the PlaceableObject model from this Controller.
    /// </summary>
    protected void RemoveModel() { model = null; }

    /// <summary>
    /// Sets this Controller's placeable object model.
    /// </summary>
    /// <param name="model">The new PlaceableObject model. </param>
    protected void SetModel(PlaceableObject model)
    {
        Assert.IsNotNull(model, "Cannot set PlaceableObject as null.");
        this.model = model;
    }

    /// <summary>
    /// Animates this Controller's model's damage flash effect if it is
    /// playing.
    /// </summary>
    private void UpdateDamageFlash()
    {
        float remainingFlashTime = GetModel().TimeRemaningInFlashAnimation();
        // if (GetModel().NAME == "Squirrel") Debug.Log(remainingFlashTime);
        if (remainingFlashTime <= 0) return;
        float newDamageFlashingTime = Mathf.Clamp(remainingFlashTime - Time.deltaTime, 0, FLASH_DURATION);
        GetModel().SetRemainingFlashAnimationTime(newDamageFlashingTime);
        float lerpTarget = Mathf.Abs(remainingFlashTime - FLASH_DURATION / 2f) * (FLASH_INTENSITY * 10f);
        float score = Mathf.Lerp(FLASH_INTENSITY, 1f, lerpTarget);
        byte greenBlueComponent = (byte)(score * 255);
        Color32 color = new Color32(255, greenBlueComponent, greenBlueComponent, 255);
        GetModel().SetColor(color);
    }

    /// <summary>
    /// Returns a list of all PlaceableObjects that are also ITargetables
    /// (all of them). 
    /// </summary>
    /// <returns>a list of all ITargetables in the scene.</returns>
    protected static List<ITargetable> GetAllTargetableObjects()
    {
        List<ITargetable> allTargetables = new List<ITargetable>();
        allTargetables.AddRange(ALL_PLACEABLES.Where(tar => tar as ITargetable != null));
        return allTargetables;
    }

    /// <summary>
    /// Moves this Controller's model to a target position over some number
    /// of seconds. Optionally, this movement can be supported by an AnimationCurve
    /// to make it smooth. 
    /// </summary>
    /// <param name="targetPosition">The position to which the model should move.</param>
    /// <param name="duration">How many seconds this movement should take.</param>
    /// <param name="lerp">Optional parameter for a custom lerp animation. </param>
    public virtual void MoveModel(Vector3 targetPosition, float duration, AnimationCurve lerp = null)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Handles a collision between this controller's model and
    /// some other Collider.
    /// </summary>
    /// <param name="other">the other collider.</param>
    protected abstract void HandleCollision(Collider2D other);
}
