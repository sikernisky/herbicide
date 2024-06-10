using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;
using System.Linq;
using System.Net.Http.Headers;

/// <summary>
/// Controller for a Kudzu Enemy.
/// </summary>
public class KudzuController : EnemyController<KudzuController.KudzuState>
{
    /// <summary>
    /// State of a Kudzu.
    /// </summary>
    public enum KudzuState
    {
        INACTIVE, // Not spawned yet.
        SPAWNING, // Spawning in.
        IDLE, // Static, nothing to do.
        CHASE, // Pursuing target.
        ATTACK, // Attacking target.
        ESCAPE, // Running back to start.
        PROTECT, // Protecting other escaping enemies.
        EXITING, // Leaving map.
        DEAD, // Dead.
        INVALID // Something went wrong.
    }

    /// <summary>
    /// The maximum number of targets a Kudzu can select at once.
    /// </summary>
    protected override int MAX_TARGETS => 1;

    /// <summary>
    /// Counts the number of seconds in the spawn animation; resets
    /// on step.
    /// </summary>
    private float spawnAnimationCounter;

    /// <summary>
    /// Counts the number of seconds in the idle animation; resets
    /// on step.
    /// </summary>
    private float idleAnimationCounter;

    /// <summary>
    /// Counts the number of seconds in the chase animation; resets
    /// on step.
    /// </summary>
    private float chaseAnimationCounter;

    /// <summary>
    /// Counts the number of seconds in the attack animation; resets
    /// on step.
    /// </summary>
    private float attackAnimationCounter;

    /// <summary>
    /// Counts the number of seconds in the protect animation; resets
    /// on step.
    /// </summary>
    private float protectAnimationCounter;

    /// <summary>
    /// Counts the number of seconds in the escape animation; resets
    /// on step.
    /// </summary>
    private float escapeAnimationCounter;

    /// <summary>
    /// Counts the number of seconds in the exiting animation; resets
    /// on step.
    /// </summary>
    private float exitingAnimationCounter;

    /// <summary>
    /// Counts the number of seconds until the Kudzu can hop again.
    /// </summary>
    private float hopCooldownCounter;



    /// <summary>
    /// Makes a new KudzuController.
    /// </summary>
    /// <param name="kudzu">The Kudzu Enemy. </param>
    /// <returns>The created KudzuController.</returns>
    public KudzuController(Kudzu kudzu) : base(kudzu) { }

    /// <summary>
    /// Updates the Kudzu controlled by this KudzuController.
    /// </summary>
    protected override void UpdateMob()
    {
        base.UpdateMob();

        if (!ValidModel()) return;
        ExecuteInactiveState();
        ExecuteSpawnState();
        ExecuteIdleState();
        ExecuteChaseState();
        ExecuteAttackState();
        ExecuteProtectState();
        ExecuteEscapeState();
        ExecuteExitState();
        // Debug.Log(GetState());
    }

    /// <summary>
    /// Returns this KudzuController's Kudzu model.
    /// </summary>
    /// <returns>this KudzuController's Kudzu model.</returns>
    private Kudzu GetKudzu() { return GetModel() as Kudzu; }

    /// <summary>
    /// Drops a resource at the Kudzu's resource drop rate.
    /// </summary>
    protected override void DropDeathLoot()
    {
        if (DroppedDeathLoot()) return;
        if (GetKudzu().Exited()) return;

        Dew dew = DewFactory.GetDewPrefab().GetComponent<Dew>();
        Vector3 lootPos = GetKudzu().Exited() ? GetKudzu().GetExitPos() : GetKudzu().GetPosition();
        int value = GetKudzu().CURRENCY_VALUE_ON_DEATH;
        DewController dewController = new DewController(dew, lootPos, value);
        ControllerController.AddModelController(dewController);
        base.DropDeathLoot();
    }

    /// <summary>
    /// Returns the spawn position of the Kudzu when in a NexusHole.
    /// </summary>
    /// <param name="originalSpawnPos">The position of the NexusHole it is
    /// spawning from.</param>
    /// <returns> the spawn position of the Kudzu when in a NexusHole.</returns>
    protected override Vector3 NexusHoleSpawnPos(Vector3 originalSpawnPos)
    {
        originalSpawnPos.y -= 2;
        return originalSpawnPos;
    }

    /// <summary>
    /// Returns true if this controller's Kudzu should be destoyed and
    /// set to null.
    /// </summary>
    /// <returns>true if this controller's Kudzu should be destoyed and
    /// set to null; otherwise, false.</returns>
    public override bool ValidModel()
    {
        if (!GetEnemy().Spawned()) return true;

        HashSet<KudzuState> immuneStates = new HashSet<KudzuState>()
        {
            KudzuState.INACTIVE,
            KudzuState.SPAWNING,
            KudzuState.EXITING,
            KudzuState.INVALID
        };

        bool isImmune = immuneStates.Contains(GetState());
        if (!isImmune && !TileGrid.OnWalkableTile(GetEnemy().GetPosition())) return false;
        else if (GetKudzu().Dead()) return false;
        else if (GetKudzu().Exited()) return false;

        // Debug.Log("Valid Kudzu");

        return true;
    }

    /// <summary>
    /// Returns the Kudzu prefab to the KudzuFactory singleton.
    /// </summary>
    public override void DestroyModel()
    {
        EnemyFactory.ReturnEnemyPrefab(GetKudzu().gameObject);
    }

    //--------------------BEGIN STATE LOGIC----------------------//

    /// <summary>
    /// Returns true if two KudzuStates are equal.
    /// </summary>
    /// <param name="stateA">The first state.</param>
    /// <param name="stateB">The second state.</param>
    /// <returns>true if two KudzuStates are equal; otherwise, false.</returns>
    public override bool StateEquals(KudzuState stateA, KudzuState stateB)
    {
        return stateA == stateB;
    }

    /// <summary>
    /// Updates the state of this KudzuController's Kudzu model.
    /// The transitions are: <br></br>
    /// 
    /// INACTIVE --> SPAWN : if ready to spawn <br></br>
    /// SPAWN --> IDLE : always <br></br>
    /// IDLE --> CHASE : if target in chase range <br></br>
    /// CHASE --> ATTACK : if target in attack range and attack off cooldown <br></br>
    /// ATTACK --> CHASE : if target not in attack range, or attack on cooldown <br></br>
    /// ATTACK --> ESCAPE : if grabbed a target and running away with it <br></br>
    /// ESCAPE --> CHASE : if dropped target <br></br>
    /// ESCAPE --> EXIT : if made it back to start pos with target<br></br>
    /// CHASE --> IDLE : if target not in chase range <br></br>
    /// </summary>
    public override void UpdateStateFSM()
    {
        if (!ValidModel()) return;
        if (GetGameState() != GameState.ONGOING)
        {
            SetState(KudzuState.IDLE);
            return;
        }

        Mob target = GetTarget() as Mob;
        bool targetExists = target != null && target.Targetable();
        bool carrierToProtect = GetNearestCarrier() != null;
        bool withinChaseRange = targetExists && DistanceToTarget() <= GetKudzu().GetChaseRange();
        bool withinAttackRange = targetExists && DistanceToTarget() <= GetKudzu().GetAttackRange();
        bool holdingTarget = NumTargetsHolding() > 0;

        //Debug.Log(GetState());



        switch (GetState())
        {
            case KudzuState.INACTIVE:
                if (GetKudzu().Spawned()) SetState(KudzuState.SPAWNING);
                break;

            case KudzuState.SPAWNING:
                if (PoppedOutOfHole()) SetState(KudzuState.IDLE);
                break;

            case KudzuState.IDLE:
                if (!ReachedMovementTarget()) break;
                if (idleAnimationCounter > 0) break;

                if (!targetExists && carrierToProtect) SetState(KudzuState.PROTECT);
                else if (targetExists && withinChaseRange) SetState(KudzuState.CHASE);
                else if (holdingTarget) SetState(KudzuState.ESCAPE);
                break;

            case KudzuState.CHASE:
                if (!ReachedMovementTarget()) break;
                if (chaseAnimationCounter > 0) break;

                if (!targetExists) SetState(carrierToProtect ? KudzuState.PROTECT : KudzuState.IDLE);
                else if (withinAttackRange) SetState(KudzuState.ATTACK);
                else if (!withinChaseRange) SetState(KudzuState.IDLE);
                else if (holdingTarget) SetState(KudzuState.ESCAPE);
                break;

            case KudzuState.ATTACK:
                if (!ReachedMovementTarget()) break;
                if (attackAnimationCounter > 0) break;


                if (holdingTarget) SetState(KudzuState.ESCAPE);
                else if (!targetExists) SetState(carrierToProtect ? KudzuState.PROTECT : KudzuState.IDLE);
                else if (!withinAttackRange) SetState(withinChaseRange ? KudzuState.CHASE : KudzuState.IDLE);

                break;
            case KudzuState.ESCAPE:
                if (!ReachedMovementTarget()) break;
                if (escapeAnimationCounter > 0) break;

                if (!targetExists && !holdingTarget) SetState(carrierToProtect ? KudzuState.PROTECT : KudzuState.IDLE);
                else if (!holdingTarget) SetState(KudzuState.IDLE);
                else if (Vector2.Distance(GetKudzu().GetPosition(), GetTarget().GetPosition()) < 0.1f)
                {
                    SetState(KudzuState.EXITING);
                }
                break;

            case KudzuState.PROTECT:
                if (!ReachedMovementTarget()) break;
                if (protectAnimationCounter > 0) break;

                if (!carrierToProtect) SetState(KudzuState.IDLE);
                else if (targetExists)
                {
                    if (withinAttackRange) SetState(KudzuState.ATTACK);
                    else if (withinChaseRange) SetState(KudzuState.CHASE);
                }
                break;

            case KudzuState.EXITING:
            case KudzuState.INVALID:
                break;
            default:
                throw new System.Exception("Invalid state.");
        }

    }


    /// <summary>
    /// Runs logic for the Kudzu model's Inactive state.
    /// </summary>
    protected virtual void ExecuteInactiveState()
    {
        if (GetState() != KudzuState.INACTIVE) return;

        SetNextMovePos(GetKudzu().GetSpawnPos());
    }

    /// <summary>
    /// Runs logic for the Kudzu model's Spawn state.
    /// </summary>
    protected virtual void ExecuteSpawnState()
    {
        if (GetState() != KudzuState.SPAWNING) return;

        GetKudzu().SetSpawning(true);
        SetAnimation(GetKudzu().MOVE_ANIMATION_DURATION, EnemyFactory.GetSpawnTrack(
            GetKudzu().TYPE,
                                  GetKudzu().GetDirection(), GetKudzu().GetHealthState()));

        PopOutOfMovePos(NexusHoleSpawnPos(GetKudzu().GetSpawnPos()));
        GetKudzu().FaceDirection(Direction.SOUTH);

        if (!ReachedMovementTarget()) return;

        // We have fully popped out of the hole.
        SetPoppedOutOfHole();
    }

    /// <summary>
    /// Runs logic for the Kudzu model's idle state.
    /// </summary>
    protected virtual void ExecuteIdleState()
    {
        if (GetState() != KudzuState.IDLE) return;

        SetAnimation(GetKudzu().IDLE_ANIMATION_DURATION, EnemyFactory.GetIdleTrack(
            GetKudzu().TYPE,
            GetKudzu().GetDirection(), GetKudzu().GetHealthState()));

        SetNextMovePos(GetKudzu().GetPosition());
        MoveLinearlyTowardsMovePos();

        GetKudzu().FaceDirection(Direction.SOUTH);
    }

    /// <summary>
    /// Runs logic for the Kudzu model's chase state. 
    /// </summary>
    protected virtual void ExecuteChaseState()
    {
        if (GetState() != KudzuState.CHASE) return;

        SetAnimation(GetKudzu().MOVE_ANIMATION_DURATION, EnemyFactory.GetMovementTrack(
            GetKudzu().TYPE,
            GetKudzu().GetDirection(), GetKudzu().GetHealthState()));

        // Move to target.
        MoveLinearlyTowardsMovePos();

        // Decrement hop counter.
        hopCooldownCounter -= Time.deltaTime;
        if (hopCooldownCounter > 0) return;

        // We reached our move target, so we need a new one.
        if (!ReachedMovementTarget()) return;
        Mob target = GetTarget() as Mob;
        if (target == null || !target.Targetable()) return;

        if (DistanceToTarget() <= GetKudzu().GetAttackRange()) return;

        Vector3 closest = ClosestPositionToTarget(target);
        Vector3 nextMove = TileGrid.NextTilePosTowardsGoal(GetKudzu().GetPosition(), closest);
        SetNextMovePos(nextMove);
        hopCooldownCounter = GetKudzu().HOP_COOLDOWN;
    }

    /// <summary>
    /// Runs logic for the Kudzu model's protect state.
    /// </summary>
    protected virtual void ExecuteProtectState()
    {
        if (GetState() != KudzuState.PROTECT) return;

        SetAnimation(GetKudzu().MOVE_ANIMATION_DURATION, EnemyFactory.GetMovementTrack(
            GetKudzu().TYPE,
                       GetKudzu().GetDirection(), GetKudzu().GetHealthState()));

        // Move to target.
        MoveLinearlyTowardsMovePos();

        // Decrement hop counter.
        hopCooldownCounter -= Time.deltaTime;
        if (hopCooldownCounter > 0) return;

        // We reached our move target, so we need a new one.
        if (!ReachedMovementTarget()) return;
        if (GetNearestCarrier() == null) return;

        Vector3 closest = GetNearestCarrier().GetPosition();
        Vector3 nextMove = TileGrid.NextTilePosTowardsGoal(GetKudzu().GetPosition(), closest);
        SetNextMovePos(nextMove);
        hopCooldownCounter = GetKudzu().HOP_COOLDOWN;
    }

    /// <summary>
    /// Runs logic for the Kudzu model's attack state. 
    /// </summary>
    protected virtual void ExecuteAttackState()
    {
        if (GetState() != KudzuState.ATTACK) return;

        SetAnimation(GetKudzu().ATTACK_ANIMATION_DURATION, EnemyFactory.GetAttackTrack(
            GetKudzu().TYPE,
                       GetKudzu().GetDirection(), GetKudzu().GetHealthState()));

        //Attack Logic : Only if target is valid.
        Mob target = GetTarget() as Mob;
        bool validTarget = GetTarget() != null && target.Targetable();
        if (!CanAttack()) return;
        if (!validTarget) return;

        FaceTarget();
        if (CanHoldTarget(target)) HoldTarget(target); // Hold.
        else target.AdjustHealth(-GetKudzu().BONK_DAMAGE); // Bonk.
        GetHeldTargets().ForEach(target => target.SetMaskInteraction(SpriteMaskInteraction.None));
        GetKudzu().RestartAttackCooldown();
    }

    /// <summary>
    /// Runs logic for the Kudzu model's escaping state. 
    /// </summary>
    protected virtual void ExecuteEscapeState()
    {
        if (GetState() != KudzuState.ESCAPE) return;
        if (!ValidModel()) return;
        if (GetTarget() == null) return;

        SetAnimation(GetKudzu().MOVE_ANIMATION_DURATION, EnemyFactory.GetMovementTrack(
            GetEnemy().TYPE,
                                  GetKudzu().GetDirection(), GetKudzu().GetHealthState()));

        // Move to target.
        if (TileGrid.IsNexusHole(GetNextMovePos())) MoveParabolicallyTowardsMovePos();
        else MoveLinearlyTowardsMovePos();

        // Decrement hop counter.
        hopCooldownCounter -= Time.deltaTime;
        if (hopCooldownCounter > 0) return;

        // We reached our move target, so we need a new one.
        if (!ReachedMovementTarget()) return;
        SetNextMovePos(TileGrid.NextTilePosTowardsGoal(GetKudzu().GetPosition(), GetTarget().GetPosition()));
        hopCooldownCounter = GetKudzu().HOP_COOLDOWN;
    }

    /// <summary>
    /// Runs logic for the Kudzu's Exit state.
    /// </summary>
    protected virtual void ExecuteExitState()
    {
        if (GetState() != KudzuState.EXITING) return;
        if (GetKudzu().Exited()) return;
        NexusHole nexusHoleTarget = GetTarget() as NexusHole;
        if (nexusHoleTarget == null) return;

        Assert.IsTrue(GetTarget() as NexusHole != null, "exit target needs to be a NH.");
        Vector3 nexusHolePosition = GetTarget().GetPosition();
        Vector3 jumpPosition = nexusHolePosition;
        jumpPosition.y -= 2;

        if (!GetKudzu().IsExiting() && !GetKudzu().Exited()) GetKudzu().SetExiting(nexusHolePosition);

        SetAnimation(GetKudzu().MOVE_ANIMATION_DURATION, EnemyFactory.GetMovementTrack(
            GetKudzu().TYPE,
                       GetKudzu().GetDirection(), GetKudzu().GetHealthState()));

        // Move to target.
        GetKudzu().SetMaskInteraction(SpriteMaskInteraction.VisibleOutsideMask);
        GetHeldTargets().ForEach(target => target.SetMaskInteraction(SpriteMaskInteraction.VisibleOutsideMask));
        SetNextMovePos(jumpPosition);
        FallIntoMovePos(3f);

        if (!ReachedMovementTarget()) return;
        if (GetKudzu().IsExiting() && GetKudzu().GetPosition() == jumpPosition)
            GetKudzu().SetExited();
        Assert.IsTrue(NumTargetsHolding() > 0, "You need to hold targets to exit.");
    }



    //---------------------END STATE LOGIC-----------------------//

    /// <summary>
    /// Returns true if the Kudzu can target the Model passed
    /// into this method.
    /// </summary>
    /// <param name="target">The Model to check for targetability.</param>
    /// <returns>true if the Kudzu can target the Model passed
    /// into this method; otherwise, false. </returns>
    protected override bool CanTarget(Model target)
    {
        if (!GetKudzu().Spawned()) return false;
        if (GetState() == KudzuState.SPAWNING) return false;

        Nexus nexusTarget = target as Nexus;
        NexusHole nexusHoleTarget = target as NexusHole;

        // If escaping, only target NexusHoles.
        if (GetState() == KudzuState.ESCAPE || GetState() == KudzuState.EXITING)
        {
            if (nexusHoleTarget == null) return false;
            if (!nexusHoleTarget.Targetable()) return false;
            if (nexusHoleTarget.PickedUp()) return false;
            if (!GetKudzu().IsExiting() && !TileGrid.CanReach(GetKudzu().GetPosition(), nexusHoleTarget.GetPosition())) return false;
            if (!IsClosestNexusHole(nexusHoleTarget)) return false;

            return true;
        }

        // If not escaping, only target Nexii.
        else
        {
            if (nexusTarget == null) return false;
            if (!nexusTarget.Targetable()) return false;
            if (nexusTarget.PickedUp()) return false;
            if (nexusTarget.CashedIn()) return false;
            if (!TileGrid.CanReach(GetKudzu().GetPosition(), nexusTarget.GetPosition())) return false;
            if (!IsClosestDroppedNexus(nexusTarget)) return false;

            return true;
        }

        throw new System.Exception("Something wrong happened.");
    }


    /// <summary>
    /// Adds one chunk of Time.deltaTime to the animation
    /// counter that tracks the current state.
    /// </summary>
    public override void AgeAnimationCounter()
    {
        KudzuState state = GetState();
        if (state == KudzuState.SPAWNING) spawnAnimationCounter += Time.deltaTime;
        else if (state == KudzuState.IDLE) idleAnimationCounter += Time.deltaTime;
        else if (state == KudzuState.CHASE) chaseAnimationCounter += Time.deltaTime;
        else if (state == KudzuState.ATTACK) attackAnimationCounter += Time.deltaTime;
        else if (state == KudzuState.PROTECT) protectAnimationCounter += Time.deltaTime;
        else if (state == KudzuState.ESCAPE) escapeAnimationCounter += Time.deltaTime;
        else if (state == KudzuState.EXITING) exitingAnimationCounter += Time.deltaTime;
    }

    /// <summary>
    /// Returns the animation counter for the current state.
    /// </summary>
    /// <returns>the animation counter for the current state.</returns>
    public override float GetAnimationCounter()
    {
        KudzuState state = GetState();
        if (state == KudzuState.SPAWNING) return spawnAnimationCounter;
        else if (state == KudzuState.IDLE) return idleAnimationCounter;
        else if (state == KudzuState.CHASE) return chaseAnimationCounter;
        else if (state == KudzuState.ATTACK) return attackAnimationCounter;
        else if (state == KudzuState.PROTECT) return protectAnimationCounter;
        else if (state == KudzuState.ESCAPE) return escapeAnimationCounter;
        else if (state == KudzuState.EXITING) return exitingAnimationCounter;
        else return 0;
    }

    /// <summary>
    /// Sets the animation counter for the current state to 0.
    /// </summary>
    public override void ResetAnimationCounter()
    {
        KudzuState state = GetState();
        if (state == KudzuState.SPAWNING) spawnAnimationCounter = 0;
        else if (state == KudzuState.IDLE) idleAnimationCounter = 0;
        else if (state == KudzuState.CHASE) chaseAnimationCounter = 0;
        else if (state == KudzuState.ATTACK) attackAnimationCounter = 0;
        else if (state == KudzuState.PROTECT) protectAnimationCounter = 0;
        else if (state == KudzuState.ESCAPE) escapeAnimationCounter = 0;
        else if (state == KudzuState.EXITING) exitingAnimationCounter = 0;
    }
}
