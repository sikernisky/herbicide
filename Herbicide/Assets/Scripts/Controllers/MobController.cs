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
        List<PlaceableObject> filteredTargets = FilterTargets(GetAllTargetableObjects());
        CleanTargets(filteredTargets);
        ElectTarget(filteredTargets);
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
    /// Returns a list of this MobController's current targets. Mobs interperet
    /// targets differently; for example, a Squirrel will attack its target,
    /// but some other Mob might heal its target.
    /// </summary>
    /// <returns>this MobController's target.</returns>
    protected List<PlaceableObject> GetTargets() { return targets; }

    /// <summary>
    /// Sets this MobController's target. Make sure to use
    /// ElectTarget() so that the right target for this Mob
    /// is selected.
    /// </summary>
    /// <param name="targetable">The target to add.</param>
    protected void AddTarget(PlaceableObject targetable)
    {
        Assert.IsNotNull(targetable, "Target cannot be null.");
        Assert.IsFalse(NumTargets() >= MAX_TARGETS, GetMob() +
            " already has " + NumTargets() + " targets.");
        targets.Add(targetable);
    }

    /// <summary>
    /// Removes one of this MobController's targets.
    /// </summary>
    /// <param name="targetable">The target to remove.</param>
    protected void RemoveTarget(PlaceableObject targetable)
    {
        Assert.IsNotNull(targetable, "Target cannot be null.");
        Assert.IsTrue(targets.Contains(targetable), "Does not contain target " + targetable);
        targets.Remove(targetable);
    }

    /// <summary>
    /// Sets this MobController's targets to be a filtered list of PlaceableObjects.
    /// </summary>
    /// <param name="filteredTargetables">a list of PlaceableObjects that this EnemyController
    /// is allowed to set as its target. /// </param>
    protected virtual void ElectTarget(List<PlaceableObject> filteredTargetables)
    {
        if (filteredTargetables == null) return;
        if (!ValidModel()) return;

        //Debug.Log(NumTargets());

        //Add as many targets as possible.
        foreach (PlaceableObject filteredTarget in filteredTargetables)
        {
            if (NumTargets() < MAX_TARGETS)
            {
                AddTarget(filteredTarget);
            }
        }
    }

    /// <summary>
    /// Runs through the list of the Mob's current targets and removes
    /// those that no longer meet the standards of being a target.
    /// </summary>
    /// <param name="filteredTargets">The most up to date list of
    /// valid targets in the scene.</param>
    protected virtual void CleanTargets(List<PlaceableObject> filteredTargets)
    {
        if (filteredTargets == null) return;

        List<PlaceableObject> targetsToRemove = new List<PlaceableObject>();
        GetTargets().RemoveAll(t => t == null || System.Object.Equals(t, null));
        GetTargets().RemoveAll(t => !filteredTargets.Contains(t));
    }

    /// <summary>
    /// Returns the current number of targets the Mob has
    /// elected. 
    /// </summary>
    /// <returns>the current number of targets the Mob has
    /// elected.</returns>
    protected int NumTargets() { return targets.Count; }

    /// <summary>
    /// Parses the list of all PlaceableObjects in the scene such that it
    /// only contains PlaceableObjects that this MobController's Mob is allowed
    /// to target. /// 
    /// </summary>
    /// <param name="targetables">the list of all PlaceableObjects in the scene</param>
    /// <returns>a list containing PlaceableObjects that this MobController's Mob is allowed
    /// to target</returns>
    protected abstract List<PlaceableObject> FilterTargets(List<PlaceableObject> targetables);

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
