using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BearController : DefenderController<BearController.BearState>
{
    /// <summary>
    /// Different states of a Bear Model.
    /// </summary>
    public enum BearState
    {
        SPAWN,
        IDLE,
        ATTACK,
        INVALID
    }

    /// <summary>
    /// Counts the number of seconds in the idle animation; resets
    /// on step.
    /// </summary>
    private float idleAnimationCounter;

    /// <summary>
    /// Counts the number of seconds in the attack animation; resets
    /// on step.
    /// </summary>
    private float attackAnimationCounter;

    /// <summary>
    /// Maximum number of targets a Bear can have.
    /// </summary>
    protected override int MAX_TARGETS => 1;

    /// <summary>
    /// Makes a new BearController.
    /// </summary>
    /// <param name="bear">The Bear Defender. </param>
    /// <returns>The created BearController.</returns>
    public BearController(Bear bear) : base(bear) { }

    /// <summary>
    /// Main update loop for the Squirrel.
    /// </summary>
    protected override void UpdateMob()
    {
        base.UpdateMob();
        if (!ValidModel()) return;

        ExecuteSpawnState();
        ExecuteIdleState();
        ExecuteAttackState();
    }

    /// <summary>
    /// Returns this BearController's Bear.
    /// </summary>
    /// <returns>this BearController's Bear.</returns>
    private Bear GetBear() { return GetMob() as Bear; }

    /// <summary>
    /// Handles all collisions between this controller's Bear
    /// model and some other collider.
    /// </summary>
    /// <param name="other">the other collider.</param>
    protected override void HandleCollision(Collider2D other) { throw new NotImplementedException(); }

    /// <summary>
    /// Returns the Bear's target if it has one. 
    /// </summary>
    /// <returns>the Bear's target; null if it doesn't have one.</returns>
    private Enemy GetTarget() { return NumTargets() == 1 ? GetTargets()[0] as Enemy : null; }

    /// <summary>
    /// Returns the Bear prefab to the BearFactory singleton.
    /// </summary>
    public override void DestroyModel() { BearFactory.ReturnBearPrefab(GetBear().gameObject); }

    //--------------------BEGIN STATE LOGIC----------------------//

    /// <summary>
    /// Returns true if two BearStates are equal.
    /// </summary>
    /// <param name="stateA">The first state.</param>
    /// <param name="stateB">The second state.</param>
    /// <returns>true if two BearStates are equal; otherwise, false.</returns>
    public override bool StateEquals(BearState stateA, BearState stateB)
    {
        return stateA == stateB;
    }

    /// <summary>
    /// Updates the state of this SquirrelController's Squirrel model.
    /// The transitions are: <br></br>
    /// 
    /// SPAWN --> IDLE : always <br></br>
    /// IDLE --> ATTACK : if target in range <br></br>
    /// ATTACK --> IDLE : if no target in range
    /// </summary>
    public override void UpdateStateFSM()
    {
        if (!ValidModel()) return;
        if (GetGameState() != GameState.ONGOING)
        {
            SetState(BearState.IDLE);
            return;
        }

        switch (GetState())
        {
            case BearState.SPAWN:
                if (SpawnStateDone()) SetState(BearState.IDLE);
                break;
            case BearState.IDLE:
                if (GetTarget() == null || !GetTarget().Targetable()) break;
                if (DistanceToTargetFromTree()
                    <= GetBear().GetAttackRange() &&
                    GetBear().GetAttackCooldown() <= 0) SetState(BearState.ATTACK);
                break;
            case BearState.ATTACK:
                if (GetTarget() == null || !GetTarget().Targetable()) SetState(BearState.IDLE);
                if (GetAnimationCounter() > 0) break;
                if (GetBear().GetAttackCooldown() > 0) SetState(BearState.IDLE);
                else if (DistanceToTargetFromTree()
                    > GetBear().GetAttackRange()) SetState(BearState.IDLE);
                break;
            case BearState.INVALID:
                throw new System.Exception("Invalid State.");
        }
    }

    /// <summary>
    /// Runs logic relevant to the Bear's spawn state.
    /// </summary>
    protected override void ExecuteSpawnState()
    {
        if (!ValidModel()) return;
        if (GetState() != BearState.SPAWN) return;

        GetBear().FaceDirection(Direction.SOUTH);
        base.ExecuteSpawnState();
    }

    /// <summary>
    /// Runs logic relevant to the Bear's idle state.
    /// </summary>
    protected virtual void ExecuteIdleState()
    {
        if (!ValidModel()) return;
        if (GetState() != BearState.IDLE) return;

        if (GetTarget() != null) FaceTarget();
        else GetBear().FaceDirection(Direction.SOUTH);

        SetAnimation(GetBear().IDLE_ANIMATION_DURATION, BearFactory.GetIdleTrack(GetBear().GetDirection()));
    }

    /// <summary>
    /// Runs logic relevant to the Squirrel's attacking state.
    /// </summary>
    protected virtual void ExecuteAttackState()
    {
        if (!ValidModel()) return;
        if (GetTarget() == null || !GetTarget().Targetable()) return;
        if (GetState() != BearState.ATTACK) return;

        // Animation Logic.

        FaceTarget();

        SetAnimation(GetBear().ATTACK_ANIMATION_DURATION, BearFactory.GetAttackTrack(GetBear().GetDirection()));

        if (!CanAttack()) return;

        // Swipe at the target tile.
        EmanationController chompEmanationController = new EmanationController(
            EmanationController.EmanationType.BEAR_CHOMP,
            1,
            GetTarget().GetAttackPosition());
        ControllerController.AddEmanationController(chompEmanationController);
        GetTarget().AdjustHealth(-GetBear().CHOMP_DAMAGE);

        // Reset attack animation.
        GetBear().RestartAttackCooldown();
    }

    /// <summary>
    /// Returns true if the Bear can swipe its target.
    /// </summary>
    /// <returns>true if the Bear can swipe its target; otherwise,
    /// false.</returns>
    public override bool CanAttack()
    {
        if (!base.CanAttack()) return false; //Cooldown
        if (GetState() != BearState.ATTACK) return false; //Not in the attack state.
        if (GetTarget() == null || !GetTarget().Targetable()) return false; //Invalid target.
        return true;
    }

    /// <summary>
    /// Adds one chunk of Time.deltaTime to the animation
    /// counter that tracks the current state.
    /// </summary>
    public override void AgeAnimationCounter()
    {
        BearState state = GetState();
        if (state == BearState.IDLE) idleAnimationCounter += Time.deltaTime;
        else if (state == BearState.ATTACK) attackAnimationCounter += Time.deltaTime;
    }

    /// <summary>
    /// Returns the animation counter for the current state.
    /// </summary>
    /// <returns>the animation counter for the current state.</returns>
    public override float GetAnimationCounter()
    {
        BearState state = GetState();
        if (state == BearState.IDLE) return idleAnimationCounter;
        else if (state == BearState.ATTACK) return attackAnimationCounter;
        else throw new System.Exception("State " + state + " has no counter.");
    }

    /// <summary>
    /// Sets the animation counter for the current state to 0.
    /// </summary>
    public override void ResetAnimationCounter()
    {
        BearState state = GetState();
        if (state == BearState.IDLE) idleAnimationCounter = 0;
        else if (state == BearState.ATTACK) attackAnimationCounter = 0;
    }

}
