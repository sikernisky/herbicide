using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Controls a Mob. The T generic Enum represents the Mob's State. 
/// These Enums are defined in inheriting controller classes. <br></br>
/// 
/// The MobController is responsible for manipulating its Mob and bringing
/// it to life. This includes moving it, choosing targets, playing animations,
/// and more.
/// </summary>
/// <typeparam name="T">Enum to represent state of the Mob.</typeparam>
public abstract class MobController<T> : ModelController, IStateTracker<T> where T : Enum
{
    #region Fields

    /// <summary>
    /// The list of current targets. Mobs will interperet a "target"
    /// in their own way.  
    /// </summary>
    private List<Model> targets;

    /// <summary>
    /// The list of Models the Mob is holding. 
    /// </summary>
    private List<Model> modelsHolding;

    /// <summary>
    /// The total time in seconds, from start to finish, of a damage flash
    /// animation.
    /// </summary>
    private const float FLASH_DURATION = .4f;

    /// <summary>
    /// The maximum number of targets the controller can have at once.
    /// </summary>
    protected abstract int MAX_TARGETS { get; }

    /// <summary>
    /// The number of targets this Enemy is allowed to pick up at once.
    /// </summary>
    protected virtual int HOLDING_LIMIT => 1;

    /// <summary>
    /// The Mob's state.
    /// </summary>
    private T state;

    /// <summary>
    /// Where this Mob is moving towards.
    /// </summary>
    private Vector3? movementDestinationPosition;

    /// <summary>
    /// true if the Mob controlled by this MobController
    /// needs to parse through all possible targets. This exists
    /// so that we don't need to go through targeting logic for Mobs
    /// that will never use them: StoneWall, for example.
    /// </summary>
    protected virtual bool FINDS_TARGETS => false;


    #endregion

    #region Methods

    /// <summary>
    /// Makes a new MobController for a Mob.
    /// </summary>
    /// <param name="mob">The Mob controlled by this MobController.</param>
    public MobController(Mob mob) : base(mob)
    {
        Assert.IsNotNull(mob, "Mob cannot be null.");
        targets = new List<Model>();
        modelsHolding = new List<Model>();
    }

    /// <summary>
    /// Main update loop for the MobController. Updates its model.
    /// </summary>
    /// <param name="gameState">The most recent GameState.</param>
    public override void UpdateController(GameState gameState)
    {
        base.UpdateController(gameState);
        UpdateDamageFlash();
        UpdateFSM();
        UpdateMob();
    }

    /// <summary>
    /// Main update loop for the MobController's Mob logic. 
    /// </summary>
    protected virtual void UpdateMob()
    {
        if (!ValidModel()) return;
        if(GetMob().ReadyToSpawn() && !GetMob().Spawned()) SpawnMob();
        if(!GetMob().Spawned()) return;

        if (FINDS_TARGETS) UpdateTargets(GetAllModels());
        GetMob().StepMainActionCooldownByDeltaTime();
    }

    /// <summary>
    /// Returns this MobController's Mob model. Inheriting controller
    /// classes use this method to access their Mob; then, they cast
    /// it to its respective type.
    /// </summary>
    /// <returns>this MobController's Mob model.</returns>
    protected Mob GetMob() => GetModel() as Mob;

    /// <summary>
    /// Animates this Controller's model's damage flash effect if it is
    /// playing.
    /// </summary>
    private void UpdateDamageFlash()
    {
        if (GetMob() == null || System.Object.Equals(null, GetMob())) return;

        float remainingFlashTime = GetMob().TimeRemainingInFlashAnimation();
        if (remainingFlashTime <= 0)
        {
            //GetModel().SetColor(Color.white);
            return;
        }

        float newDamageFlashingTime = Mathf.Clamp(remainingFlashTime - Time.deltaTime, 0, FLASH_DURATION);
        GetMob().SetRemainingFlashAnimationTime(newDamageFlashingTime);

        if (remainingFlashTime == FLASH_DURATION)
        {
            GetModel().SetColor(new Color32(255, 0, 0, 255));
            return; // Skip lerping for the first frame
        }

        float lerpFactor = 1 - Mathf.Cos((Mathf.PI * remainingFlashTime) / FLASH_DURATION);
        lerpFactor = Mathf.Clamp(lerpFactor, 0, 1);
        Color32 hitColor = new Color32(255, 0, 0, 255);
        Color32 color = Color32.Lerp(Color.white, hitColor, lerpFactor);
        GetModel().SetColor(color);
    }

    /// <summary>
    /// Returns the closest Tile coordinate position from the Mob to some PlaceableObject.
    /// </summary>
    /// <param name="target">The PlaceableObject to find the closest Tile coordinate position to.</param>
    /// <returns>The closest Tile coordinate position from the Mob to some PlaceableObject.</returns>
    protected Vector3 ClosestTileCoordinatePositionToTarget(PlaceableObject target) => ClosestTileCoordinatePositionToTarget(target.transform.position);

    /// <summary>
    /// Returns the closest Tile coordinate position from the Mob to a given position.
    /// </summary>
    /// <param name="targetPosition">The position to find the closest Tile coordinate position to.</param>
    /// <returns>The closest Tile coordinate position from the Mob to the given position.</returns>
    protected Vector3 ClosestTileCoordinatePositionToTarget(Vector3 targetPosition)
    {
        Vector2Int mobPosition = new Vector2Int(Mathf.FloorToInt(GetMob().transform.position.x), Mathf.FloorToInt(GetMob().transform.position.y));
        Vector2Int targetTilePosition = new Vector2Int(Mathf.FloorToInt(targetPosition.x), Mathf.FloorToInt(targetPosition.y));
        Vector3 convertedMobPosition = new Vector3(mobPosition.x, mobPosition.y, 1);
        Vector3 convertedTargetPosition = new Vector3(targetTilePosition.x, targetTilePosition.y, 1);

        if (TileGrid.CanReach(convertedMobPosition, convertedTargetPosition)) return convertedTargetPosition;

        return convertedMobPosition;
    }

    /// <summary>
    /// Brings this Controller's Mob into life by calling its OnSpawn()
    /// method. 
    /// </summary>
    protected virtual void SpawnMob()
    {
        Assert.IsFalse(GetMob().Spawned(), "Mob is already spawned.");
        GetMob().RefreshRenderer();
        GetMob().OnSpawn();
        GetMob().gameObject.SetActive(true);
    }

    /// <summary>
    /// Returns true if the Mob can perform its main action this frame.
    /// </summary>
    /// <returns>true if the Mob can perform its main action this frame; 
    /// otherwise, false. </returns>
    public virtual bool CanPerformMainAction() { return GetMob().GetMainActionCooldownRemaining() <= 0; }

    /// <summary>
    /// Modifies Collectable behavior based on equipped items.
    /// </summary>
    /// <param name="collectable">the Collectable to apply effects to.</param>
    /// <param name="spawnCenter">the center of the circle to spawn within.</param>
    /// <param name="spawnRadius">the radius of the circle to spawn within.</param>
    protected override void HandleCollectableEquipmentEffects(Collectable collectable, Vector2 spawnCenter, float spawnRadius)
    {
        base.HandleCollectableEquipmentEffects(collectable, spawnCenter, spawnRadius);
        if (GetModel().IsEquipped(ModelType.INVENTORY_ITEM_BUNADRYL))
        {
            if (!DirectionConstants.AllDirections.Any(dir => TileGrid.ClassTypeExistsOnSurfaceAt<Defender>(TileGrid.GetCoordinatesOfNeighborInDirection(GetMob().Coordinates, dir))))
                ProduceCollectableFromModel(collectable.TYPE, spawnCenter, spawnRadius, true);
        }
    }

    #endregion

    #region State Logic

    /// <summary>
    /// Sets the State of this MobController. This helps keep track of
    /// what its Mob should do and what it is doing, and it is essential
    /// for the FSM logic. 
    /// </summary>
    /// <param name="state">The State to set.</param>
    public void SetState(T state) => this.state = state;

    /// <summary>
    /// Returns the State of this MobController. This helps keep track of
    /// what its Mob should do and what it is doing, and it is essential
    /// for the FSM logic. 
    /// </summary>
    /// <returns>The State of this MobController. </returns>
    public T GetState() => state;

    /// <summary>
    /// Processes this MobController's state FSM to determine the
    /// correct state. Takes the current state and chooses whether
    /// or not to switch to another based on game conditions. /// 
    /// </summary>
    public abstract void UpdateFSM();

    /// <summary>
    /// Returns true if two states are equal.
    /// </summary>
    /// <param name="stateA">The first state.</param>
    /// <param name="stateB">The second state</param>
    /// <returns>true if two states are equal; otherwise, false. </returns>
    public abstract bool StateEquals(T stateA, T stateB);

    #endregion

    #region Targeting Logic

    /// <summary>
    /// Returns a copy of the list of this MobController's current targets. Mobs interperet
    /// targets differently; for example, a Squirrel will attack its target,
    /// but some other Mob might heal its target.
    /// 
    /// TODO: Add priority logic, put the most prio targets at the front of the list.
    /// This works with mobs who have multiple targets. 
    /// 
    /// </summary>
    /// <returns>this MobController's target.</returns>
    protected List<Model> GetTargets() => new List<Model>(targets);

    /// <summary>
    /// Returns the Mob's most pressing target.
    /// </summary>
    /// <returns>the Mob's most pressing target.</returns>
    protected virtual Model GetTarget() => targets.Count == 0 ? null : targets[0];

    /// <summary>
    /// Clears the Mob's list of targets after assigning them to a list of
    /// targets that existed in the previous frame.
    /// </summary>
    private void ClearTargets() => targets.Clear();

    /// <summary>
    /// Adds a Model to this Mob's list of targets.
    /// </summary>
    /// <param name="targetable">The target to add.</param>
    private void AddTarget(Model targetable)
    {
        Assert.IsNotNull(targetable, "Target cannot be null.");
        targets.Add(targetable);
    }

    /// <summary>
    /// Runs through all targetable Models and adds each one
    /// this Mob can target to its targets list.
    /// </summary>
    /// <param name="nonTiles">All targetable Models that are not Tiles..</param>
    private void UpdateTargets(IReadOnlyList<Model> nonTiles)
    {
        Assert.IsNotNull(nonTiles, "List of targets is null.");

        ClearTargets();

        foreach (Model targetable in nonTiles)
        {
            if (CanTargetOtherModel(targetable)) AddTarget(targetable);
        }

        SortTargets(targets);
    }

    /// <summary>
    /// Returns true if the distance between the Mob and a target position is less
    /// than or equal to the Mob's current main action range.
    /// </summary>
    /// <param name="targetPosition">The position of the target.</param>
    /// <returns>true if in range; otherwise, false.</returns>
    protected virtual bool IsMobInRangeOfPosition(Vector2 targetPosition)
    {
        return Vector2.Distance(GetMob().GetWorldPosition(), targetPosition) <= GetMob().GetMainActionRange();
    }

    /// <summary>
    /// Returns true if the Model is within the leniency range of the Mob.
    /// </summary>
    /// <param name="targetPosition">The position of the target.</param>
    /// <returns>true if within leniency range; otherwise, false.</returns>
    protected virtual bool IsMobInLeniencyRangeOfPosition(Vector2 targetPosition)
    {
        return Vector2.Distance(GetMob().GetWorldPosition(), targetPosition) <= GetMob().GetMainActionRange() * GetMob().LIENENCY_RANGE_MULTIPLIER;
    }

    /// <summary>
    /// Sorts the list of targets this Mob has elected to target by
    /// priority. This method is called after UpdateTargets() and
    /// before the Mob acts on its targets.
    /// </summary>
    /// <param name="targets">The list of targets to sort.</param>
    protected virtual void SortTargets(List<Model> targets) { }

    /// <summary>
    /// Returns true if the Mob is allowed to target a Model passed
    /// into this method.
    /// </summary>
    /// <param name="target">A potential target.</param>
    /// <returns>true if the Mob is allowed to target a Model; otherwise,
    /// false.</returns>
    protected abstract bool CanTargetOtherModel(Model target);

    /// <summary>
    /// Returns the current number of targets the Mob has
    /// elected. 
    /// </summary>
    /// <returns>the current number of targets the Mob has
    /// elected.</returns>
    protected int NumTargets() => targets.Count;

    /// <summary>
    /// Returns the number of targets the Mob is currently holding.
    /// </summary>
    /// <returns>the number of targets the Mob is currently holding.</returns>
    protected int NumTargetsHolding()
    {
        Assert.IsNotNull(modelsHolding, "List of holding targets is null.");

        return modelsHolding.Count;
    }

    /// <summary>
    /// Returns a copy of the list of targets the Mob is currently holding.
    /// </summary>
    /// <returns>a copy of the list of targets the Mob is currently holding.</returns>
    protected List<Model> GetHeldTargets() => new List<Model>(modelsHolding);

    /// <summary>
    /// Returns the distance from this Controller's Mob to its first target.
    /// </summary>
    /// <returns>the distance from this Controller's Mob to its first target. </returns>
    protected float DistanceToTarget()
    {
        Assert.IsNotNull(GetTargets(), "Targets list is null.");
        if (GetTargets().Count == 0) return float.MaxValue;

        Model target = GetTargets()[0];
        Assert.IsNotNull(target, "Target is null.");
        Vector2 mobPosition = GetModel().GetWorldPosition();  
        Vector2 targetPosition = target.GetWorldPosition();
        float tileScale = BoardConstants.TileSize;
        float distance = Vector2.Distance(mobPosition, targetPosition);
        return distance / tileScale; // Normalize the distance by the tile scale
    }

    /// <summary>
    /// Makes the Mob face its first target.
    /// </summary>
    protected void FaceTarget()
    {
        if (GetTargets().Count == 0) return;
        Model target = GetTarget();
        if (target != null) GetMob().FaceDirection(target.GetWorldPosition());
    }

    #endregion

    #region Movement Logic

    /// <summary>
    /// Moves the Mob towards the next movement position in a linear manner.
    /// </summary>
    protected void MoveLinearlyTowardsMovePos()
    {
        if (GetNextMovePos() == null) return;
        MoveLinearlyTowards(GetNextMovePos().Value, GetMob().GetMovementSpeed());
    }

    /// <summary>
    /// Moves the Mob towards the next movement position in manner such that it
    /// looks like it is falling into a hole.
    /// </summary>
    /// <param name="acceleration">How fast to accelerate towards the movement target.</param>

    protected void FallIntoMovePos(float acceleration)
    {
        if (GetNextMovePos() == null) return;
        FallTowards(GetNextMovePos().Value, GetMob().GetMovementSpeed(), acceleration);
    }

    /// <summary>
    /// Moves the Mob towards the next movement position in manner such that it
    /// looks like it popping out of a hole.
    /// </summary>
    /// <param name="startPosition">Where the mob is popping from.</param>
    protected void PopOutOfMovePos(Vector3 startPosition)
    {
        if (GetNextMovePos() == null) return;
        PopFrom(startPosition, GetNextMovePos().Value, GetMob().ENTERING_MOVEMENT_SPEED);
    }

    /// <summary>
    /// Moves the Mob towards the next movement position in a parabolic manner.
    /// </summary>
    protected void MoveParabolicallyTowardsMovePos()
    {
        if (GetNextMovePos() == null) return;
        MoveParabolicallyTowards(GetNextMovePos().Value, GetMob().GetMovementSpeed());
    }

    /// <summary>
    /// Sets where the Mob should move next.
    /// </summary>
    /// <param name="nextPos">where the Mob should move next.</param>
    protected virtual void SetNextMovePos(Vector3? nextPos)
    {
        movementDestinationPosition = nextPos;
        Vector3 newParabolicTarget = new Vector3(movementDestinationPosition.Value.x, movementDestinationPosition.Value.y, 1);
        float newParabolicScale = GetMob().GetMovementSpeed() /
            Vector3.Distance(GetMob().GetWorldPosition(), newParabolicTarget);

        ResetParabolicFields(newParabolicScale, newParabolicTarget);
    }

    /// <summary>
    /// Returns where the Mob is moving towards. 
    /// </summary>
    /// <returns>where the Mob is moving towards. .</returns>
    protected Vector3? GetNextMovePos() { return movementDestinationPosition; }

    /// <summary>
    /// Returns true if the Mob's position is at the spot it
    /// is trying to move towards.
    /// </summary>
    /// <returns>true if the Mob made it to its movement destination;
    /// otherwise, false. </returns>
    protected bool ReachedMovementTarget()
    {
        if (GetNextMovePos() == null) return true;

        Vector3 nextMovePos = new Vector3(
            GetNextMovePos().Value.x,
            GetNextMovePos().Value.y,
            1
        );

        return Vector3.Distance(GetMob().GetWorldPosition(), nextMovePos) < 0.01f;
    }

    #endregion
}
