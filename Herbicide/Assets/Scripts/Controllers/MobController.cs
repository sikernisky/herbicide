using System;
using System.Collections;
using System.Collections.Generic;
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
    private List<PlaceableObject> targets;

    /// <summary>
    /// The list of targets the Mob is holding.
    /// </summary>
    private List<PlaceableObject> targetsHolding;

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
    /// Makes a new MobController for a Mob.
    /// </summary>
    /// <param name="mob">The Mob controlled by this MobController.</param>
    public MobController(Mob mob) : base(mob)
    {
        Assert.IsNotNull(mob, "Mob cannot be null.");
        targets = new List<PlaceableObject>();
        targetsHolding = new List<PlaceableObject>();
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
        UpdateTargets(GetAllTargetableObjects());
        GetMob().AdjustAttackCooldown(-Time.deltaTime);
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
    protected List<PlaceableObject> GetTargets() { return new List<PlaceableObject>(targets); }

    /// <summary>
    /// Clears the Mob's list of targets.
    /// </summary>
    private void ClearTargets() { targets.Clear(); }

    /// <summary>
    /// Adds a PlaceableObject to this Mob's list of targets.
    /// </summary>
    /// <param name="targetable">The target to add.</param>
    private void AddTarget(PlaceableObject targetable)
    {
        Assert.IsNotNull(targetable, "Target cannot be null.");
        Assert.IsFalse(NumTargets() >= MAX_TARGETS, GetMob() +
            " already has " + NumTargets() + " targets.");
        Assert.IsTrue(CanTarget(targetable), "Not a valid target.");

        targets.Add(targetable);
    }

    /// <summary>
    /// Runs through all targetable Placeable Objects and adds each one
    /// this Mob can target to its targets list.
    /// </summary>
    /// <param name="allTargets">All targetable Models.</param>
    private void UpdateTargets(List<PlaceableObject> allTargets)
    {
        Assert.IsNotNull(allTargets, "List of targets is null.");

        ClearTargets();
        foreach (PlaceableObject targetable in allTargets)
        {
            if (CanTarget(targetable) && NumTargets() < MAX_TARGETS)
            {
                AddTarget(targetable);
            }
        }
    }

    /// <summary>
    /// Returns true if the Mob is allowed to target a PlaceableObject passed
    /// into this method.
    /// </summary>
    /// <param name="target">A potential target.</param>
    /// <returns>true if the Mob is allowed to target a PlaceableObject; otherwise,
    /// false.</returns>
    protected abstract bool CanTarget(PlaceableObject target);

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
                Vector3 convertedTarg = new Vector3(targetCoord.x, targetCoord.y, 1);
                if (distance < minDistance && TileGrid.CanReach(convertedThis, convertedTarg))
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

        if (!GetTargets().Contains(target)) return false; // Need to target before holding
        if (NumTargetsHolding() >= HOLDING_LIMIT) return false;
        if (target.PickedUp()) return false;

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
    protected List<PlaceableObject> GetHeldTargets()
    {
        return new List<PlaceableObject>(targetsHolding);
    }

    /// <summary>
    /// Returns true if the Mob is holding some target.
    /// </summary>
    /// <returns>true if the Mob is holding the target; otherwise, false.</returns>
    protected bool IsHoldingTarget(PlaceableObject target)
    {
        return targetsHolding.Contains(target);
    }

    //--------------------BEGIN STATE LOGIC----------------------//

    /// <summary>
    /// Sets the State of this MobController. This helps keep track of
    /// what its Mob should do and what it is doing, and it is essential
    /// for the FSM logic. 
    /// </summary>
    /// <param name="state"></param>
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
    /// Sets where the Mob should move next.
    /// </summary>
    /// <param name="nextPos">where the Mob should move next.</param>
    protected virtual void SetNextMovePos(Vector3? nextPos) { nextMovePos = nextPos; }

    /// <summary>
    /// Brings this Controller's Mob into life by calling its OnSpawn()
    /// method. 
    /// </summary>
    protected virtual void SpawnMob()
    {
        GetMob().RefreshRenderer();
        GetMob().OnSpawn();
    }
}
