using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Controller for a Kudzu Enemy.
/// </summary>
public class KudzuController : EnemyController
{

    /// <summary>
    /// The maximum number of targets a Kudzu can select at once.
    /// </summary>
    protected override int MAX_TARGETS => 1;

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
    /// Returns this KudzuController's Kudzu model.
    /// </summary>
    /// <returns>this KudzuController's Kudzu model.</returns>
    private Kudzu GetKudzu() { return GetModel() as Kudzu; }


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

        HashSet<EnemyState> immuneStates = new HashSet<EnemyState>()
        {
            EnemyState.INACTIVE,
            EnemyState.SPAWNING,
            EnemyState.EXITING,
            EnemyState.INVALID
        };

        bool isImmune = immuneStates.Contains(GetState());
        if (!isImmune && !TileGrid.OnWalkableTile(GetEnemy().GetPosition())) return false;
        else if (GetKudzu().Dead()) return false;
        else if (GetKudzu().Exited()) return false;

        // Debug.Log("Valid Kudzu");

        return true;
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
            SetState(EnemyState.IDLE);
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
            case EnemyState.INACTIVE:
                if (GetKudzu().Spawned()) SetState(EnemyState.SPAWNING);
                break;

            case EnemyState.SPAWNING:
                if (PoppedOutOfHole()) SetState(EnemyState.IDLE);
                break;

            case EnemyState.IDLE:
                if (!ReachedMovementTarget()) break;
                if (idleAnimationCounter > 0) break;

                if (!targetExists && carrierToProtect) SetState(EnemyState.PROTECT);
                else if (targetExists && withinChaseRange) SetState(EnemyState.CHASE);
                else if (holdingTarget) SetState(EnemyState.ESCAPE);
                break;

            case EnemyState.CHASE:
                if (!ReachedMovementTarget()) break;
                if (chaseAnimationCounter > 0) break;

                if (!targetExists) SetState(carrierToProtect ? EnemyState.PROTECT : EnemyState.IDLE);
                else if (withinAttackRange) SetState(EnemyState.ATTACK);
                else if (!withinChaseRange) SetState(EnemyState.IDLE);
                else if (holdingTarget) SetState(EnemyState.ESCAPE);
                break;

            case EnemyState.ATTACK:
                if (!ReachedMovementTarget()) break;
                if (attackAnimationCounter > 0) break;


                if (holdingTarget) SetState(EnemyState.ESCAPE);
                else if (!targetExists) SetState(carrierToProtect ? EnemyState.PROTECT : EnemyState.IDLE);
                else if (!withinAttackRange) SetState(withinChaseRange ? EnemyState.CHASE : EnemyState.IDLE);

                break;
            case EnemyState.ESCAPE:
                if (!ReachedMovementTarget()) break;
                if (escapeAnimationCounter > 0) break;

                if (!targetExists && !holdingTarget) SetState(carrierToProtect ? EnemyState.PROTECT : EnemyState.IDLE);
                else if (!holdingTarget) SetState(EnemyState.IDLE);
                else if (Vector2.Distance(GetKudzu().GetPosition(), GetTarget().GetPosition()) < 0.1f)
                {
                    SetState(EnemyState.EXITING);
                }
                break;

            case EnemyState.PROTECT:
                if (!ReachedMovementTarget()) break;
                if (protectAnimationCounter > 0) break;

                if (!carrierToProtect) SetState(EnemyState.IDLE);
                else if (targetExists)
                {
                    if (withinAttackRange) SetState(EnemyState.ATTACK);
                    else if (withinChaseRange) SetState(EnemyState.CHASE);
                }
                break;

            case EnemyState.EXITING:
            case EnemyState.INVALID:
                break;
            default:
                throw new System.Exception("Invalid state.");
        }

    }

    /// <summary>
    /// Runs logic for the Kudzu model's Inactive state.
    /// </summary>
    protected override void ExecuteInactiveState()
    {
        if (GetState() != EnemyState.INACTIVE) return;

        SetNextMovePos(GetKudzu().GetSpawnPos());
    }

    /// <summary>
    /// Runs logic for the Kudzu model's Spawn state.
    /// </summary>
    protected override void ExecuteSpawnState()
    {
        if (GetState() != EnemyState.SPAWNING) return;

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
    protected override void ExecuteIdleState()
    {
        if (GetState() != EnemyState.IDLE) return;

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
    protected override void ExecuteChaseState()
    {
        if (GetState() != EnemyState.CHASE) return;

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
    protected override void ExecuteProtectState()
    {
        if (GetState() != EnemyState.PROTECT) return;

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
    protected override void ExecuteAttackState()
    {
        if (GetState() != EnemyState.ATTACK) return;

        SetAnimation(GetKudzu().ATTACK_ANIMATION_DURATION, EnemyFactory.GetAttackTrack(
            GetKudzu().TYPE,
                       GetKudzu().GetDirection(), GetKudzu().GetHealthState()));

        //Attack Logic : Only if target is valid.
        Mob target = GetTarget() as Mob;
        bool validTarget = GetTarget() != null && target.Targetable();
        if (!CanAttack()) return;
        if (!validTarget) return;

        FaceTarget();
        if (CanHoldTarget(target as Nexus)) HoldTarget(target as Nexus); // Hold.
        else target.AdjustHealth(-GetKudzu().BONK_DAMAGE); // Bonk.
        GetHeldTargets().ForEach(target => target.SetMaskInteraction(SpriteMaskInteraction.None));
        GetKudzu().RestartAttackCooldown();
    }

    /// <summary>
    /// Runs logic for the Kudzu model's escaping state. 
    /// </summary>
    protected override void ExecuteEscapeState()
    {
        if (GetState() != EnemyState.ESCAPE) return;
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
    protected override void ExecuteExitState()
    {
        if (GetState() != EnemyState.EXITING) return;
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
        if (GetState() == EnemyState.SPAWNING) return false;

        Nexus nexusTarget = target as Nexus;
        NexusHole nexusHoleTarget = target as NexusHole;

        // If escaping, only target NexusHoles.
        if (GetState() == EnemyState.ESCAPE || GetState() == EnemyState.EXITING)
        {
            if (nexusHoleTarget == null) return false;
            if (!nexusHoleTarget.Targetable()) return false;
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
}
