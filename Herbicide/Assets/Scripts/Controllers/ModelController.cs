using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Controls a Model. <br></br>
/// 
/// The ModelController is responsible for manipulating its Model and bringing
/// it to life. This includes moving it, choosing targets, playing animations,
/// and more.
/// </summary>
public abstract class ModelController
{
    #region Fields

    /// <summary>
    /// This Controller's Model.
    /// </summary>
    private Model model;

    /// <summary>
    /// Most recent GameState.
    /// </summary>
    private GameState gameState;

    /// <summary>
    /// All active Models in the scene.
    /// </summary>
    private static List<Model> ALL_MODELS = new List<Model>();

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
    /// Where the Model is parabolically moving towards.
    /// </summary>
    private Vector3 parabolaTarget;

    /// <summary>
    /// How far along the current parabolic move is.
    /// </summary>
    private float parabolaProgress;

    /// <summary>
    /// The choppiness of the paraoblic movement.
    /// </summary>
    private float parabolaScale;

    /// <summary>
    /// The height of the Model's parabolic movement.
    /// </summary>
    private float arcHeight = 0.5f;

    /// <summary>
    /// The starting position of the parabolic move.
    /// </summary>
    Vector3 parabolaStartPos;

    #endregion

    #region Methods

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
        GetModel().SubscribeToProjectileCollisionEvent(HandleProjectileCollision);

        ALL_MODELS.Add(model);
    }

    /// <summary>
    /// Returns this ModelController's Model.
    /// </summary>
    /// <returns>this ModelController's Model.</returns>
    public Model GetModel() => model;

    /// <summary>
    /// Sets this Controller's Model.
    /// </summary>
    /// <param name="model">The new Model. </param>
    protected void SetModel(Model model)
    {
        Assert.IsNotNull(model, "Cannot set Model as null.");
        this.model = model;
        this.model.gameObject.SetActive(true);
    }

    /// <summary>
    /// Main update loop for this Controller's model. 
    /// </summary>
    /// <param name="gameState">The most recent GameState.</param>
    public virtual void UpdateController(GameState gameState)
    {
        if (!ValidModel()) return;
        this.gameState = gameState;

        FixSortingOrder();
        StepAnimation();
        GetModel().ProcessEffects();
    }

    /// <summary>
    /// Destroys and detatches the Model from this Controller.
    /// </summary>
    private void DestroyAndRemoveModel()
    {
        if (GetModel() == null || modelRemoved) return;

        // Subclasses can apply effects to game before destroying & removing the Model.
        OnDestroyModel();

        ALL_MODELS.Remove(GetModel());
        ReturnModelToFactory();
        model = null;
        modelRemoved = true;
    }


    /// <summary>
    /// Drops loot from this Mob when it dies. 
    /// </summary>
    protected virtual void DropDeathLoot() => droppedDeathLoot = true;

    /// <summary>
    /// Returns true if this Model dropped its death loot.
    /// </summary>
    /// <returns>true if this Model dropped its death loot; otherwise,
    /// false. </returns>
    protected bool DroppedDeathLoot() => droppedDeathLoot;

    /// <summary>
    /// Updates the Model's sorting order so that it appears behind Models
    /// before it and before Models behind it.
    /// </summary>
    protected virtual void FixSortingOrder()
    {
        if (!ValidModel()) return;

        GetModel().SetSortingOrder(-Mathf.FloorToInt(GetModel().GetWorldPosition().y));
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
    public bool ShouldRemoveController()
    {
        // Not ready to remove yet.
        if (ValidModel()) return false;

        // Ready.
        return true;
    }

    /// <summary>
    /// Called when the ControllerManager removes this ModelController from
    /// the scene. Destroys this controller's Model.
    /// </summary>
    public void OnRemoveController() => DestroyAndRemoveModel();

    /// <summary>
    /// Handles a collision between this controller's model and
    /// some other Collider.
    /// </summary>
    /// <param name="other">the other collider.</param>
    protected virtual void HandleCollision(Collider2D other) { }

    /// <summary>
    /// Handles a collision between this controller's model and
    /// a Projectile. <br></br>
    /// 
    /// This is manually called by the ProjectileController when its
    /// Projectile hits this controller's Model.
    /// </summary>
    /// <param name="projectile">The Projectile that collided with
    /// this controller's Model. </param>
    protected virtual void HandleProjectileCollision(Projectile projectile) { }

    /// <summary>
    /// Informs this ModelController of the number of active Models of
    /// each type.
    /// </summary>
    /// <param name="counts">The number of active Models of each ModelType.</param>
    public void InformOfModelCounts(ModelCounts counts) => this.counts = counts;

    /// <summary>
    /// Returns the number of active Models of each type.
    /// </summary>
    /// <returns>The number of active Models of each type.</returns>
    protected ModelCounts GetModelCounts() => counts;

    /// <summary>
    /// Returns the most recent GameState recognized by this ModelController.
    /// </summary>
    /// <returns>the most recent GameState.</returns>
    protected GameState GetGameState() => gameState;

    /// <summary>
    /// Returns a list of all active Models.
    /// </summary>
    /// <returns>a list of all Models in the scene.</returns>
    public static IReadOnlyList<Model> GetAllModels() => ALL_MODELS.AsReadOnly();

    /// <summary>
    /// Returns this ModelController's Model to its Factory.
    /// </summary>
    public abstract void ReturnModelToFactory();

    /// <summary>
    /// Performs logic right before this Model is destroyed.
    /// </summary>
    protected virtual void OnDestroyModel() => DropDeathLoot();

    /// <summary>
    /// Takes the stats of a Model that is being combined into this Model
    /// and applies them to this Model.
    /// </summary>
    /// <param name="combiningModel">a Model of the same type as this ModelController's
    /// Model being combined with other Models into the Model controlled by this ModelController.</param>
    public virtual void AquireStatsOfCombiningModels(List<Model> combiningModels)
    {
        Assert.IsNotNull(combiningModels, "Combining Models is null.");
        combiningModels.ForEach(m => Assert.IsNotNull(m, "Combining Model is null."));
        combiningModels.ForEach(m => Assert.IsTrue(GetModel().TYPE == m.TYPE, "Model types do not match."));
    }

    /// <summary>
    /// Returns true if a given Model is the closest Model of its type to 
    /// the Model controlled by this ModelController.
    /// </summary>
    /// <param name="targetModel">The Model to check.</param>
    /// <returns>true if a given Model is the closest Model of its type to 
    /// the Model controlled by this ModelController; otherwise, false. </returns>
    protected bool IsClosestTargetableModelAlongPath(Model targetModel)
    {
        Assert.IsNotNull(targetModel);

        Model closestModel = null;
        float minDistance = float.MaxValue;

        foreach (Model otherModel in GetAllModels())
        {
            if (otherModel == null) continue;
            if (otherModel.TYPE != targetModel.TYPE) continue;
            if (otherModel == GetModel()) continue;

            float distance = TileGrid.GetPathfindingTileDistance(GetModel().GetWorldPosition(), otherModel.GetWorldPosition());
            if (distance < minDistance)
            {
                minDistance = distance;
                closestModel = otherModel;
            }
        }
        return closestModel == targetModel;
    }

    /// <summary>
    /// Configures and produces a Collectable from the Model's position.
    /// </summary>
    /// <param name="resourceType">the type of Collectable to produce.</param>
    /// <param name="spawnCenter">the center of the circle to spawn within.</param>
    /// <param name="spawnRadius">the radius of the circle to spawn within.</param>
    /// <param name="ignoreEquipment">true if the Collectable should ignore equipment effects;
    /// otherwise, false.</param>
    public virtual void ProduceCollectableFromModel(ModelType resourceType, Vector2 spawnCenter, float spawnRadius, bool ignoreEquipment)
    {
        GameObject resourcePrefab = CollectableFactory.GetCollectablePrefab(resourceType);
        Currency currencyComp = resourcePrefab.GetComponent<Currency>();
        if (!ignoreEquipment) HandleCollectableEquipmentEffects(currencyComp, spawnCenter, spawnRadius);
        Vector2 spawnPosition = GetRandomCurrencySpawnPositionFromModelPosition(spawnRadius) + spawnCenter;
        ControllerManager.CreateCollectableController(resourceType, currencyComp, spawnPosition);
    }

    /// <summary>
    /// Returns a position that is a random spot within a circle of a given radius
    /// from the Model's position.
    /// </summary>
    /// <param name="spawnRadius">the radius of the circle to spawn within.</param>
    /// <returns>a position that is a random spot within a circle of a given radius
    /// from the Model's position.</returns>
    private Vector2 GetRandomCurrencySpawnPositionFromModelPosition(float spawnRadius)
    {
        float angle = UnityEngine.Random.Range(0, 2 * Mathf.PI);
        float randomRadius = UnityEngine.Random.Range(0f, 1f) * spawnRadius;
        return new Vector2(Mathf.Cos(angle) * randomRadius, Mathf.Sin(angle) * randomRadius);
    }

    /// <summary>
    /// Modifies Collectable behavior based on equipped items.
    /// </summary>
    /// <param name="collectable">the Collectable to apply effects to.</param>
    /// <param name="spawnCenter">the center of the circle to spawn within.</param>
    /// <param name="spawnRadius">the radius of the circle to spawn within.</param>
    protected virtual void HandleCollectableEquipmentEffects(Collectable collectable, Vector2 spawnCenter, float spawnRadius)
    {
        Assert.IsNotNull(collectable, "Collectable cannot be null.");
    }

    /// <summary>
    /// Configures and shoots a projectile from the Model's world position towards a target position.
    /// </summary>
    /// <param name="projectileType">the type of projectile to configure and fire.</param>
    /// <param name="targetPosition">the position to fire the projectile towards.</param>
    /// <param name="ignoreEquipment">true if the projectile should ignore equipment effects;
    public virtual void FireProjectileFromModel(ModelType projectileType, Vector3 targetPosition, bool ignoreEquipment)
    {
        Assert.IsTrue(ModelTypeHelper.IsProjectile(projectileType));
        var projectile = ProjectileFactory.GetProjectilePrefab(projectileType).GetComponent<Projectile>();
        ControllerManager.CreateProjectileController(projectileType, projectile, GetModel().GetWorldPosition(), targetPosition);
        if (!ignoreEquipment) HandleProjectileEquipmentEffects(projectile);
    }

    /// <summary>
    /// Configures and shoots a pre-made projectile from the Model's world position towards a target position.
    /// </summary>
    /// <param name="projectile">the type of projectile to configure and fire.</param>
    /// <param name="targetPosition">the position to fire the projectile towards.</param>
    /// <param name="ignoreEquipment">true if the projectile should ignore equipment effects;
    public virtual void FireProjectileFromModel(Projectile projectile, Vector3 targetPosition, bool ignoreEquipment)
    {
        Assert.IsNotNull(projectile, "Projectile cannot be null.");
        ControllerManager.CreateProjectileController(projectile.TYPE, projectile, GetModel().GetWorldPosition(), targetPosition);
        if (!ignoreEquipment) HandleProjectileEquipmentEffects(projectile);
    }

    /// <summary>
    /// Modifies projectile behavior based on equipped items.
    /// </summary>
    /// <param name="projectile">The type projectile to modify.</param>
    protected virtual void HandleProjectileEquipmentEffects(Projectile projectile)
    {
        Assert.IsNotNull(projectile, "Projectile cannot be null.");
        if (GetModel().IsEquipped(ModelType.INVENTORY_ITEM_ACORNOL)) projectile.SetNumSplits(AbilityItemConstants.AcornolSplits);
    }

    #endregion

    #region Animation Logic

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
    /// Sets the Model's animation to the given track after the current
    /// one finishes or immediately if the Model has changed directions
    /// since the previous animation change. Starts from the beginning
    /// of the track unless the given animation is already playing.
    /// </summary>
    /// <param name="cycleDuration">The duration of the animation cycle.</param>
    /// <param name="track">The track to animate.</param>
    protected void SetNextAnimation(float cycleDuration, Sprite[] track)
    {
        if (GetModel().HasAnimationTrack() && GetModel().GetNumCyclesOfCurrentAnimationCompleted() == 0
              && GetModel().GetDirectionOfMostRecentAnimation() == GetModel().Direction) return;
        else SetCurrentAnimation(cycleDuration, track);
    }

    /// <summary>
    /// Sets the Model's animation to the given track. Overrides any 
    /// current animation. Starts from the beginning of the track unless
    /// the given animation is already playing.
    /// </summary>
    /// <param name="cycleDuration">The duration of the animation cycle.</param>
    /// <param name="track">The track to animate.</param>
    protected void SetCurrentAnimation(float cycleDuration, Sprite[] track)
    {
        GetModel().SetAnimationDuration(cycleDuration);
        if (track != GetModel().CurrentAnimationTrack) GetModel().SetAnimationTrack(track);
        else GetModel().SetAnimationTrack(track, GetModel().CurrentFrame, false);
    }

    /// <summary>
    /// Checks to see if the next frame in the animation needs to be
    /// displayed. If so, displays it.
    /// </summary>
    private void StepAnimation()
    {
        if (!GetModel().HasAnimationTrack()) return;

        AgeAnimationCounter();
        if (ShouldGoNextFrame())
        {
            GetModel().NextFrame();
            ResetAnimationCounter();

            if (GetModel().CurrentFrame == 0) GetModel().IncrementNumCyclesOfCurrentAnimationCompleted();
        }
        GetModel().SetSprite(GetModel().GetSpriteAtCurrentFrame());
    }

    /// <summary>
    /// Returns true if the current animation cycle is finished.
    /// </summary>
    /// <returns>true if the current animation cycle is finished; otherwise,
    /// false. </returns>
    private bool ShouldGoNextFrame()
    {
        float stepTime = GetModel().CurrentAnimationDuration / GetModel().NumFrames();
        return GetAnimationCounter() >= stepTime;

    }

    #endregion

    #region Movement Logic

    /// <summary>
    /// Moves the Model towards a target position in a linear manner.
    /// </summary>
    /// <param name="targetPosition">The target position to move towards.</param>
    /// <param name="speed">How fast to move towards the target position.</param>
    protected void MoveLinearlyTowards(Vector3 targetPosition, float speed)
    {
        Vector3 adjusted = new Vector3(targetPosition.x, targetPosition.y, 1);
        float step = speed * Time.deltaTime * BoardConstants.TileSize;
        step = Mathf.Clamp(step, 0f, step);
        Vector3 newPosition = Vector3.MoveTowards(GetModel().GetWorldPosition(), adjusted, step);
        if (GetModel().GetWorldPosition() != adjusted) GetModel().FaceDirection(adjusted);
        GetModel().SetWorldPosition(newPosition);
    }

    /// <summary>
    /// Moves the Model towards a target position in a parabolic manner.
    /// </summary>
    /// <param name="targetPosition">The target position to move towards.</param>
    /// <param name="speed">How fast to move towards the target position.</param>
    protected void MoveParabolicallyTowards(Vector3 targetPosition, float speed)
    {
        parabolaTarget = new Vector3(targetPosition.x, targetPosition.y, 1);
        parabolaProgress = Mathf.Min(parabolaProgress + Time.deltaTime * parabolaScale, 1.0f);
        float parabola = 1.0f - 4.0f * (parabolaProgress - 0.5f) * (parabolaProgress - 0.5f);
        Vector3 nextPos = Vector3.Lerp(parabolaStartPos, parabolaTarget, parabolaProgress);
        nextPos.y += parabola * arcHeight;
        if (GetModel().GetWorldPosition() != nextPos) GetModel().FaceDirection(nextPos);
        GetModel().SetWorldPosition(nextPos);
    }

    /// <summary>
    /// Sets the fields related to parabolic movement to their default values.
    /// </summary>
    protected void ResetParabolicFields(float scale, Vector3 targetPosition)
    {
        parabolaProgress = 0f;
        parabolaScale = scale;
        parabolaStartPos = GetModel().GetWorldPosition();
        parabolaTarget = targetPosition;
    }

    /// <summary>
    /// Moves the Model towards a target position such that it looks like it is falling
    /// into it. 
    /// </summary>
    /// <param name="targetPosition">The target position to move towards.</param>
    /// <param name="speed">How fast to move towards the target position.</param>
    /// <param name="acceleration">How fast to accelerate towards the target position.</param>
    protected void FallTowards(Vector3 targetPosition, float speed, float acceleration)
    {
        Vector3 adjusted = new Vector3(targetPosition.x, targetPosition.y, 1);
        float distance = Vector3.Distance(GetModel().GetWorldPosition(), adjusted);
        float fallSpeed = speed + acceleration * distance;
        float step = fallSpeed * Time.deltaTime;
        step = Mathf.Clamp(step, 0f, step);

        Vector3 newPosition = Vector3.MoveTowards(GetModel().GetWorldPosition(), adjusted, step);
        if (GetModel().GetWorldPosition() != adjusted) GetModel().FaceDirection(adjusted);
        GetModel().SetWorldPosition(newPosition);

        float scaleStep = 3.5f * Time.deltaTime;
        Vector3 newScale = Vector3.Lerp(GetModel().transform.localScale, Vector3.zero, scaleStep);
        GetModel().transform.localScale = newScale;
    }

    /// <summary>
    /// Moves the Model from a starting position to a target position in a popping, parabolic
    /// manner.
    /// </summary>
    /// <param name="popStartPosition">Where the mob is popping from.</param>
    /// <param name="targetPosition">The target position to move towards.</param>
    /// <param name="speed">How fast to move towards the target position.</param>
    protected void PopFrom(Vector3 popStartPosition, Vector3 targetPosition, float speed)
    {
        Vector3 adjusted = new Vector3(targetPosition.x, targetPosition.y, 1);
        float step = speed * Time.deltaTime;
        step = Mathf.Clamp(step, 0f, step);

        Vector3 newPosition = Vector3.MoveTowards(GetModel().GetWorldPosition(), adjusted, step);
        if (GetModel().GetWorldPosition() != adjusted) GetModel().FaceDirection(adjusted);
        GetModel().SetWorldPosition(newPosition);

        // Calculate the scaling factor based on the distance to the target position
        float distanceToTarget = Vector3.Distance(GetModel().GetWorldPosition(), adjusted);
        float totalDistance = Vector3.Distance(popStartPosition, adjusted);
        float scaleFraction = 1 - distanceToTarget / totalDistance;

        // Ensure scaleFraction is within the bounds of 0 and 1
        scaleFraction = Mathf.Clamp01(scaleFraction);

        // Calculate the new scale
        Vector3 endScale = new Vector3(BoardConstants.TileSize, BoardConstants.TileSize, 1);
        Vector3 newScale = Vector3.Lerp(new Vector3(0.1f, 0.1f, 0.1f), endScale, scaleFraction);
        GetModel().transform.localScale = newScale;
    }

    #endregion
}
