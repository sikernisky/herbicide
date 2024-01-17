using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Abstract class to represent controllers of Mobs. The T generic Enum
/// represents the Controller's Mob's State. These Enums are defined in
/// inheriting classes. <br></br>
/// 
/// The MobController is responsible for manipulating its Mob and bringing
/// it to life. This includes moving it, choosing targets, playing animations,
/// and more.
/// </summary>
/// <typeparam name="T">Enum to represent state of the Mob.</typeparam>
public abstract class MobController<T> : ModelController, IStateTracker<T> where T : Enum
{
    /// <summary>
    /// The list of current targets. Mobs will interperet a "target"
    /// in their own way.  
    /// </summary>
    private List<Model> targets;

    /// <summary>
    /// The list of targets the Mob is holding.
    /// </summary>
    private List<Model> targetsHolding;

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
    /// The State that triggered the Mob's most recent animation.
    /// </summary>
    private T animationState;

    /// <summary>
    /// The number of Mobs assigned to MobControllers since
    /// this scene began.
    /// </summary>
    private static int NUM_MOBS;

    /// <summary>
    /// Where this Mob moves next.
    /// </summary>
    private Vector3? nextMovePos;

    /// <summary>
    /// The Mob's pathfinding cache.
    /// </summary>
    private PathfindingCache pathfindingCache;

    /// <summary>
    /// The number of occupying Models during the previous frame.
    /// </summary>
    private int blockingObjectsLastFrame;

    /// <summary>
    /// Where the Mob is parabolically moving towards.
    /// </summary>
    public Vector3 parabolaTarget;

    /// <summary>
    /// The height of the Mob's parabolic movement.
    /// </summary>
    public float arcHeight = 0.5f;

    /// <summary>
    /// The starting position of the parabolic move.
    /// </summary>
    Vector3 parabolaStartPos;

    /// <summary>
    /// The choppiness of the paraoblic movement.
    /// </summary>
    float parabolaScale;

    /// <summary>
    /// How far along the current parabolic move is.
    /// </summary>
    float parabolaProgress;


    /// <summary>
    /// Makes a new MobController for a Mob.
    /// </summary>
    /// <param name="mob">The Mob controlled by this MobController.</param>
    public MobController(Mob mob) : base(mob)
    {
        Assert.IsNotNull(mob, "Mob cannot be null.");
        targets = new List<Model>();
        targetsHolding = new List<Model>();
        SpawnMob();
        NUM_MOBS++;
    }

    /// <summary>
    /// Main update loop for the MobController. Updates its model.
    /// </summary>
    public override void UpdateModel()
    {
        base.UpdateModel();
        UpdateDamageFlash();
        UpdateStateFSM();
        UpdateMob();
    }

    /// <summary>
    /// Main update loop for the MobController's Mob logic. 
    /// </summary>
    protected virtual void UpdateMob()
    {
        if (!ValidModel()) return;
        UpdateTargets(GetAllModels(), TileGrid.GetAllTiles());
        GetMob().AdjustAttackCooldown(-Time.deltaTime);
        if (GetPathfindingCache() == null) SetPathfindingCache();
        ValidatePathfindingCache();
    }

    /// <summary>
    /// Queries the SynergyController to determine which Synergies are
    /// active. Performs logic based on the active Synergies.
    /// </summary>
    protected override void UpdateSynergies() { return; }

    /// <summary>
    /// Adds to the Mob all Synergy effects that could affect it.
    /// </summary>
    protected override void ApplySynergies() { return; }

    /// <summary>
    /// Returns this MobController's Mob model. Inheriting controller
    /// classes use this method to access their Mob; then, they cast
    /// it to its respective type.
    /// </summary>
    /// <returns>this MobController's Mob model.</returns>
    protected Mob GetMob() { return GetModel() as Mob; }

    /// <summary>
    /// Returns a copy of the list of this MobController's current targets. Mobs interperet
    /// targets differently; for example, a Squirrel will attack its target,
    /// but some other Mob might heal its target.
    /// </summary>
    /// <returns>this MobController's target.</returns>
    protected List<Model> GetTargets() { return new List<Model>(targets); }

    /// <summary>
    /// Clears the Mob's list of targets.
    /// </summary>
    private void ClearTargets() { targets.Clear(); }

    /// <summary>
    /// Adds a Model to this Mob's list of targets.
    /// </summary>
    /// <param name="targetable">The target to add.</param>
    private void AddTarget(Model targetable)
    {
        Assert.IsNotNull(targetable, "Target cannot be null.");
        Assert.IsFalse(NumTargets() >= MAX_TARGETS, GetMob() +
            " already has " + NumTargets() + " targets.");
        Assert.IsTrue(CanTarget(targetable), "Not a valid target.");

        targets.Add(targetable);
    }

    /// <summary>
    /// Runs through all targetable Models and adds each one
    /// this Mob can target to its targets list.
    /// </summary>
    /// <param name="nonTiles">All targetable Models that are not Tiles..</param>
    /// <param name="tiles">All targetable Models that are Tiles.</param>
    private void UpdateTargets(IReadOnlyList<Model> nonTiles, IReadOnlyList<Model> tiles)
    {
        Assert.IsNotNull(nonTiles, "List of targets is null.");

        ClearTargets();
        foreach (Model targetable in nonTiles)
        {
            if (CanTarget(targetable) && NumTargets() < MAX_TARGETS)
            {
                AddTarget(targetable);
            }
        }

        foreach (Model targetable in tiles)
        {
            if (CanTarget(targetable) && NumTargets() < MAX_TARGETS)
            {
                AddTarget(targetable);
            }
        }
    }

    /// <summary>
    /// Returns true if the Mob is allowed to target a Model passed
    /// into this method.
    /// </summary>
    /// <param name="target">A potential target.</param>
    /// <returns>true if the Mob is allowed to target a Model; otherwise,
    /// false.</returns>
    protected abstract bool CanTarget(Model target);

    /// <summary>
    /// Returns the current number of targets the Mob has
    /// elected. 
    /// </summary>
    /// <returns>the current number of targets the Mob has
    /// elected.</returns>
    protected int NumTargets() { return targets.Count; }

    /// <summary>
    /// Animates this Controller's model's damage flash effect if it is
    /// playing.
    /// </summary>
    private void UpdateDamageFlash()
    {
        if (GetMob() == null || System.Object.Equals(null, GetMob())) return;

        float remainingFlashTime = GetMob().TimeRemaningInFlashAnimation();
        if (remainingFlashTime <= 0)
        {
            GetModel().SetColor(Color.white); // Explicitly set to white when the flash time ends
            return;
        }
        float newDamageFlashingTime = Mathf.Clamp(remainingFlashTime - Time.deltaTime, 0, FLASH_DURATION);
        GetMob().SetRemainingFlashAnimationTime(newDamageFlashingTime);

        // Simplified lerp target calculation
        float lerpFactor = Mathf.Cos((Mathf.PI * remainingFlashTime) / FLASH_DURATION);
        lerpFactor = Mathf.Clamp(lerpFactor, 0, 1);

        float score = Mathf.Lerp(FLASH_INTENSITY, 1f, lerpFactor);
        byte greenBlueComponent = (byte)(score * 255);
        Color32 color = new Color32(255, greenBlueComponent, greenBlueComponent, 255);
        GetModel().SetColor(color);
    }

    /// <summary>
    /// Returns the closest position from the Mob to some PlaceableObject.<br></br>
    /// 
    /// This is needed because some Models are larger than 1x1. So traditional distance
    /// calculations don't work anymore because they always used bottom left corner.
    /// For example: <br></br>
    /// 
    /// [][]        [][]
    /// [][x]       [x][]
    /// [][] 
    /// <br></br>
    ///</summary>
    /// <param name="target"></param>
    /// <returns></returns>
    protected Vector3 ClosestPositionToTarget(PlaceableObject target)
    {
        float minDistance = float.MaxValue;
        Vector2Int closestPosition = new Vector2Int();
        foreach (var thisCoord in GetMob().GetExpandedTileCoordinates())
        {
            foreach (var targetCoord in target.GetExpandedTileCoordinates())
            {
                float distance = Vector2Int.Distance(thisCoord, targetCoord);
                Vector3 convertedThis = new Vector3(thisCoord.x, thisCoord.y, 1);
                if (distance < minDistance && GetPathfindingCache().IsReachable(target))
                {
                    minDistance = distance;
                    closestPosition = targetCoord;
                }
            }
        }

        return new Vector3(closestPosition.x, closestPosition.y, 1);
    }

    /// <summary>
    /// Returns true if the Mob can hold some PlaceableObject.
    /// </summary>
    /// <returns>true if the Mob can hold the PlaceableObject; otherwise,
    /// false. </returns>
    /// <param name="target">The target to check. </param> 
    protected virtual bool CanHoldTarget(PlaceableObject target)
    {
        Assert.IsNotNull(target, "Target is null.");

        Nexus nexusTarget = target as Nexus;

        if (!target.HOLDABLE) return false;
        if (!GetTargets().Contains(target)) return false; // Need to target before holding
        if (NumTargetsHolding() >= HOLDING_LIMIT) return false;
        if (target.PickedUp()) return false;
        if (nexusTarget.CashedIn()) return false;

        return true;
    }

    /// <summary>
    /// Adds a target to the list of targets the Mob is holding.
    /// </summary>
    /// <param name="target">The target to hold. </param> 
    protected void HoldTarget(PlaceableObject target)
    {
        Assert.IsNotNull(targetsHolding, "List of holding targets is null.");
        Assert.IsTrue(CanHoldTarget(target), "Need to check holding validity.");

        targetsHolding.Add(target);
        target.PickUp(GetMob().GetTransform(), GetMob().HOLDER_OFFSET);
    }

    /// <summary>
    /// Returns the number of targets the Mob is currently holding.
    /// </summary>
    /// <returns>the number of targets the Mob is currently holding.</returns>
    protected int NumTargetsHolding()
    {
        Assert.IsNotNull(targetsHolding, "List of holding targets is null.");

        return targetsHolding.Count;
    }

    /// <summary>
    /// Returns a copy of the list of targets the Mob is currently holding.
    /// </summary>
    /// <returns>a copy of the list of targets the Mob is currently holding.</returns>
    protected List<Model> GetHeldTargets() { return new List<Model>(targetsHolding); }

    /// <summary>
    /// Returns true if the Mob is holding some target.
    /// </summary>
    /// <returns>true if the Mob is holding the target; otherwise, false.</returns>
    protected bool IsHoldingTarget(PlaceableObject target)
    {
        return targetsHolding.Contains(target);
    }

    /// <summary>
    /// Returns the MobController's PathfindingCache reference.
    /// </summary>
    /// <returns>the MobController's PathfindingCache reference.</returns>
    protected PathfindingCache GetPathfindingCache() { return pathfindingCache; }

    /// <summary>
    /// Sets this MobController's pathfinding cache.
    /// </summary>
    protected virtual void SetPathfindingCache() { pathfindingCache = new PathfindingCache(GetMob()); }

    /// <summary>
    /// Checks if the terrain has changed since the last frame. If so, wipes the
    /// Mob's pathfinding cache.
    /// </summary>
    private void ValidatePathfindingCache()
    {
        if (GetPathfindingCache() == null) return;
        int numBlockers = 0;
        foreach (Model model in GetAllModels())
        {
            PlaceableObject placeableObjectModel = model as PlaceableObject;
            if (placeableObjectModel != null && placeableObjectModel.OCCUPIER) numBlockers++;
        }
        if (numBlockers != blockingObjectsLastFrame)
        {
            GetPathfindingCache().ClearCache();
            blockingObjectsLastFrame = numBlockers;
        }
    }

    //--------------------BEGIN STATE LOGIC----------------------//

    /// <summary>
    /// Sets the State of this MobController. This helps keep track of
    /// what its Mob should do and what it is doing, and it is essential
    /// for the FSM logic. 
    /// </summary>
    /// <param name="state">The State to set.</param>
    public void SetState(T state) { this.state = state; }

    /// <summary>
    /// Returns the State of this MobController. This helps keep track of
    /// what its Mob should do and what it is doing, and it is essential
    /// for the FSM logic. 
    /// </summary>
    /// <returns>The State of this MobController. </returns>
    public T GetState() { return state; }

    /// <summary>
    /// Processes this MobController's state FSM to determine the
    /// correct state. Takes the current state and chooses whether
    /// or not to switch to another based on game conditions. /// 
    /// </summary>
    public abstract void UpdateStateFSM();

    /// <summary>
    /// Returns true if the Mob can attack this frame.
    /// </summary>
    /// <returns>true if the Mob can attack this frame; 
    /// otherwise, false. </returns>
    public virtual bool CanAttack() { return GetMob().GetAttackCooldown() <= 0; }

    /// <summary>
    /// Returns true if two states are equal.
    /// </summary>
    /// <param name="stateA">The first state.</param>
    /// <param name="stateB">The second state</param>
    /// <returns>true if two states are equal; otherwise, false. </returns>
    public abstract bool StateEquals(T stateA, T stateB);

    /// <summary>
    /// Returns the State that triggered the Mob's most recent
    /// animation.
    /// </summary>
    /// <returns>the State that triggered the Mob's most recent
    /// animation.</returns>
    public T GetAnimationState() { return animationState; }

    /// <summary>
    /// Sets the State that triggered the Mob's most recent
    /// animation.
    /// </summary>
    /// <param name="animationState">the animation state to set.</param>
    public void SetAnimationState(T animationState) { this.animationState = animationState; }

    //----------------------END STATE LOGIC----------------------//

    /// <summary>
    /// Returns where the Mob should move next.
    /// </summary>
    /// <returns>where the Mob should move next.</returns>
    protected Vector3? GetNextMovePos() { return nextMovePos; }

    /// <summary>
    /// Moves the Mob towards the next movement position in a linear manner.
    /// </summary>
    /// <param name="speed">How fast to move towards the movement target.</param>
    protected void MoveLinearlyTowardsMovePos(float speed)
    {
        if (GetNextMovePos() != null)
        {
            Vector3 adjusted = new Vector3(GetNextMovePos().Value.x, GetNextMovePos().Value.y, 1);
            float step = speed * Time.deltaTime;
            step = Mathf.Clamp(step, 0f, step);
            Vector3 newPosition = Vector3.MoveTowards(GetMob().GetPosition(), adjusted, step);
            if (GetMob().GetPosition() != adjusted) GetMob().FaceTarget(adjusted);
            GetMob().SetWorldPosition(newPosition);
        }
    }

    /// <summary>
    /// Moves the Mob towards the next movement position in manner such that it
    /// looks like it is falling into a hole.
    /// </summary>
    /// <param name="speed">How fast to move towards the movement target.</param>
    /// <param name="acceleration">How fast to accelerate towards the movement target.</param>

    protected void FallIntoMovePos(float speed, float acceleration)
    {
        if (GetNextMovePos() != null)
        {
            Vector3 adjusted = new Vector3(nextMovePos.Value.x, nextMovePos.Value.y, 1);
            float distance = Vector3.Distance(GetMob().GetPosition(), adjusted);

            float fallSpeed = speed + acceleration * distance;
            float step = fallSpeed * Time.deltaTime;
            step = Mathf.Clamp(step, 0f, step);

            Vector3 newPosition = Vector3.MoveTowards(GetMob().GetPosition(), adjusted, step);
            if (GetMob().GetPosition() != adjusted) GetMob().FaceTarget(adjusted);
            GetMob().SetWorldPosition(newPosition);

            float scaleStep = 3.5f * Time.deltaTime;
            Vector3 newScale = Vector3.Lerp(GetMob().transform.localScale, Vector3.zero, scaleStep);
            GetMob().transform.localScale = newScale;
        }
    }

    /// <summary>
    /// Moves the Mob towards the next movement position in manner such that it
    /// looks like it popping out of a hole.
    /// </summary>
    /// <param name="speed">How fast to move towards the movement target.</param>
    /// <param name="startPosition">Where the mob is popping from.</param>
    protected void PopOutOfMovePos(float speed, Vector3 startPosition)
    {
        if (GetNextMovePos() != null)
        {
            Vector3 adjusted = new Vector3(GetNextMovePos().Value.x, GetNextMovePos().Value.y, 1);
            float step = speed * Time.deltaTime;
            step = Mathf.Clamp(step, 0f, step);

            Vector3 newPosition = Vector3.MoveTowards(GetMob().GetPosition(), adjusted, step);
            if (GetMob().GetPosition() != adjusted) GetMob().FaceTarget(adjusted);
            GetMob().SetWorldPosition(newPosition);

            // Calculate the scaling factor based on the distance to the target position
            float distanceToTarget = Vector3.Distance(GetMob().GetPosition(), adjusted);
            float totalDistance = Vector3.Distance(startPosition, adjusted);
            float scaleFraction = 1 - distanceToTarget / totalDistance;

            // Ensure scaleFraction is within the bounds of 0 and 1
            scaleFraction = Mathf.Clamp01(scaleFraction);

            // Calculate the new scale
            Vector3 newScale = Vector3.Lerp(new Vector3(0.1f, 0.1f, 0.1f), Vector3.one, scaleFraction);
            GetMob().transform.localScale = newScale;
        }
    }

    /// <summary>
    /// Moves the Mob towards the next movement position in a parabolic manner.
    /// </summary>
    protected void MoveParabolicallyTowardsMovePos()
    {
        parabolaTarget = new Vector3(GetNextMovePos().Value.x, GetNextMovePos().Value.y, 1);
        if (GetMob().GetPosition() != parabolaTarget)
        {
            //reset

        }

        parabolaProgress = Mathf.Min(parabolaProgress + Time.deltaTime * parabolaScale, 1.0f);
        float parabola = 1.0f - 4.0f * (parabolaProgress - 0.5f) * (parabolaProgress - 0.5f);
        Vector3 nextPos = Vector3.Lerp(parabolaStartPos, parabolaTarget, parabolaProgress);
        nextPos.y += parabola * arcHeight;
        if (GetMob().GetPosition() != nextPos) GetMob().FaceTarget(nextPos);
        GetMob().SetWorldPosition(nextPos);
    }

    /// <summary>
    /// Sets where the Mob should move next.
    /// </summary>
    /// <param name="nextPos">where the Mob should move next.</param>
    protected virtual void SetNextMovePos(Vector3? nextPos)
    {
        nextMovePos = nextPos;

        // Reset parabolic stats.
        parabolaProgress = 0;
        parabolaTarget = new Vector3(nextMovePos.Value.x, nextMovePos.Value.y, 1);
        parabolaStartPos = GetMob().GetPosition(); // TEMP
        parabolaScale = GetMob().GetMovementSpeed() /
            Vector3.Distance(GetMob().GetPosition(), parabolaTarget); // TEMP
    }

    /// <summary>
    /// Brings this Controller's Mob into life by calling its OnSpawn()
    /// method. 
    /// </summary>
    protected virtual void SpawnMob()
    {
        GetMob().RefreshRenderer();
        GetMob().OnSpawn();
        GetMob().gameObject.SetActive(true);
        GetMob().SubscribeToCollision(HandleCollision);
    }

}
