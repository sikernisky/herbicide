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
/// /// </summary>
public abstract class MobController<T> : PlaceableObjectController where T : Enum
{
    /// <summary>
    /// The current target. Mobs will interperet a "target"
    /// in their own way.  
    /// </summary>
    private ITargetable target;

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
        SpawnMob();
        NUM_MOBS++;
    }

    /// <summary>
    /// Main update loop for the MobController. Updates its model.
    /// </summary>
    public override void UpdateModel()
    {
        base.UpdateModel();
        UpdateStateFSM();
        UpdateMob();
    }

    /// <summary>
    /// Main update loop for the MobController's Mob logic. 
    /// </summary>
    protected virtual void UpdateMob()
    {
        float oldCooldown = GetMob().GetAttackCooldown();
        oldCooldown -= Time.deltaTime;
        GetMob().SetAttackCooldown(oldCooldown);
    }

    /// <summary>
    /// Returns this MobController's Mob model. Inheriting controller
    /// classes use this method to access their Mob; then, they cast
    /// it to its respective type.
    /// </summary>
    /// <returns>this MobController's Mob model.</returns>
    protected Mob GetMob() { return GetModel() as Mob; }

    /// <summary>
    /// Returns this MobController's target. Mobs interperet targets
    /// differently; for example, a Squirrel will attack its target,
    /// but some other Mob might heal its target.
    /// </summary>
    /// <returns>this MobController's target.</returns>
    protected ITargetable GetTarget() { return target; }

    /// <summary>
    /// Sets this MobController's target. If the target is null,
    /// it means that the Mob has no target. Make sure to use
    /// ElectTarget() so that the right target for this Mob
    /// is selected.
    /// </summary>
    /// <param name="targetable">The target to set.</param>
    protected void SetTarget(ITargetable targetable) { target = targetable; }

    /// <summary>
    /// Removes this MobController's target, setting it to null. 
    /// </summary>
    protected void WipeTarget() { target = null; }

    /// <summary>
    /// Sets this MobController's target from a filtered list of ITargetables. By
    /// default, this is a random target out of all filtered options. 
    /// </summary>
    /// <param name="filteredTargetables">a list of ITargables that this EnemyController
    /// is allowed to set as its target. /// </param>
    protected virtual void ElectTarget(List<ITargetable> filteredTargetables)
    {
        Assert.IsNotNull(filteredTargetables, "List of targets is null.");
        if (!ValidModel()) return;

        //If the current target is feasible, return
        if (GetTarget() != null && GetTarget().Targetable()) return;

        //ResetAnimationCounter();
        int random = UnityEngine.Random.Range(0, filteredTargetables.Count);
        if (filteredTargetables.Count == 0) SetTarget(null);
        else SetTarget(filteredTargetables[random]);
    }

    /// <summary>
    /// Parses the list of all ITargetables in the scene such that it
    /// only contains ITargetables that this MobController's Mob is allowed
    /// to target. /// 
    /// </summary>
    /// <param name="targetables">the list of all ITargetables in the scene</param>
    /// <returns>a list containing ITargetables that this MobController's Mob is allowed
    /// to target</returns>
    protected abstract List<ITargetable> FilterTargets(List<ITargetable> targetables);

    //--------------------BEGIN STATE LOGIC----------------------//

    /// <summary>
    /// Sets the State of this MobController. This helps keep track of
    /// what its Mob should do and what it is doing, and it is essential
    /// for the FSM logic. 
    /// </summary>
    /// <param name="state"></param>
    protected void SetState(T state) { this.state = state; }

    /// <summary>
    /// Returns the State of this MobController. This helps keep track of
    /// what its Mob should do and what it is doing, and it is essential
    /// for the FSM logic. 
    /// </summary>
    /// <returns>The State of this MobController. </returns>
    protected T GetState() { return state; }

    /// <summary>
    /// Processes this MobController's state FSM to determine the
    /// correct state. Takes the current state and chooses whether
    /// or not to switch to another based on game conditions. /// 
    /// </summary>
    protected abstract void UpdateStateFSM();

    /// <summary>
    /// Logic to execute when this MobController's Mob is idle.
    /// The MobController manipulates the Mob model by calling
    /// its methods.
    /// </summary>
    protected abstract void ExecuteIdleState();

    /// <summary>
    /// Logic to execute when this MobController's Mob is attacking.
    /// The MobController manipulates the Mob model by calling
    /// its methods.
    /// </summary>
    protected abstract void ExecuteAttackState();

    /// <summary>
    /// Returns true if the Mob can attack this frame.
    /// </summary>
    /// <returns>true if the Mob can attack this frame; 
    /// otherwise, false. </returns>
    protected virtual bool CanAttack() { return GetMob().GetAttackCooldown() <= 0; }

    /// <summary>
    /// Logic to execute when this MobController's Mob is chasing.
    /// The MobController manipulates the Mob model by calling
    /// its methods.
    /// </summary>
    protected abstract void ExecuteChaseState();

    /// <summary>
    /// Returns true if two states are equal.
    /// </summary>
    /// <param name="stateA">The first state.</param>
    /// <param name="stateB">The second state</param>
    /// <returns>true if two states are equal; otherwise, false. </returns>
    protected abstract bool StateEquals(T stateA, T stateB);

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
    /// Returns true if this MobController's model is not null.
    /// </summary>
    /// <returns>true if this Controller's model is not null; otherwise,
    /// false.</returns>
    public override bool ValidModel() { return GetMob() != null; }

    /// <summary>
    /// Brings this Controller's Mob into life by calling its OnSpawn()
    /// method. 
    /// </summary>
    protected virtual void SpawnMob()
    {
        GetMob().RefreshRenderer();
        GetMob().OnSpawn();
    }

    //---------------------ANIMATION LOGIC----------------------//

    /// <summary>
    /// Returns the State that triggered the Mob's most recent
    /// animation.
    /// </summary>
    /// <returns>the State that triggered the Mob's most recent
    /// animation.</returns>
    protected T GetAnimationState() { return animationState; }

    /// <summary>
    /// Sets the State that triggered the Mob's most recent
    /// animation.
    /// </summary>
    /// <param name="animationState">the animation state to set.</param>
    protected void SetAnimationState(T animationState) { this.animationState = animationState; }
}
