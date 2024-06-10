using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Controller for a Knotwood Enemy.
/// </summary>
public class KnotwoodController : EnemyController<KnotwoodController.KnotwoodState>
{
    /// <summary>
    /// The maximum number of targets a Knotwood can select at once.
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
    /// Different states a Knotwood can be in.
    /// </summary>
    public enum KnotwoodState
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
    /// Makes a new KnotwoodController.
    /// </summary>
    /// <param name="knotwood">The Knotwood Enemy. </param>
    /// <returns>The created KnotwoodController.</returns>
    public KnotwoodController(Knotwood knotwood) : base(knotwood) { }

    /// <summary>
    /// Returns the Knotwood model of this KnotwoodController.
    /// </summary>
    /// <returns>the Knotwood model of this KnotwoodController.</returns>
    protected Knotwood GetKnotwood() { return GetModel() as Knotwood; }

    /// <summary>
    /// Updates the Knotwood controlled by this KnotwoodController.
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
    }


    /// <summary>
    /// Updates the state of this KnotwoodController's Knotwood model.
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
            SetState(KnotwoodState.IDLE);
            return;
        }

        Mob target = GetTarget() as Mob;
        bool targetExists = target != null && target.Targetable();
        bool carrierToProtect = GetNearestCarrier() != null;
        bool withinChaseRange = targetExists && DistanceToTarget() <= GetKnotwood().GetChaseRange();
        bool withinAttackRange = targetExists && DistanceToTarget() <= GetKnotwood().GetAttackRange();
        bool holdingTarget = NumTargetsHolding() > 0;

        switch (GetState())
        {
            case KnotwoodState.INACTIVE:
                if (GetKnotwood().Spawned()) SetState(KnotwoodState.SPAWNING);
                break;

            case KnotwoodState.SPAWNING:
                if (PoppedOutOfHole()) SetState(KnotwoodState.IDLE);
                break;

            case KnotwoodState.IDLE:
                if (!ReachedMovementTarget()) break;
                if (idleAnimationCounter > 0) break;

                if (!targetExists && carrierToProtect) SetState(KnotwoodState.PROTECT);
                else if (targetExists && withinChaseRange) SetState(KnotwoodState.CHASE);
                else if (holdingTarget) SetState(KnotwoodState.ESCAPE);
                break;

            case KnotwoodState.CHASE:
                if (!ReachedMovementTarget()) break;
                if (chaseAnimationCounter > 0) break;

                if (!targetExists) SetState(carrierToProtect ? KnotwoodState.PROTECT : KnotwoodState.IDLE);
                else if (withinAttackRange) SetState(KnotwoodState.ATTACK);
                else if (!withinChaseRange) SetState(KnotwoodState.IDLE);
                else if (holdingTarget) SetState(KnotwoodState.ESCAPE);
                break;

            case KnotwoodState.ATTACK:
                if (!ReachedMovementTarget()) break;
                if (attackAnimationCounter > 0) break;


                if (holdingTarget) SetState(KnotwoodState.ESCAPE);
                else if (!targetExists) SetState(carrierToProtect ? KnotwoodState.PROTECT : KnotwoodState.IDLE);
                else if (!withinAttackRange) SetState(withinChaseRange ? KnotwoodState.CHASE : KnotwoodState.IDLE);

                break;
            case KnotwoodState.ESCAPE:
                if (!ReachedMovementTarget()) break;
                if (escapeAnimationCounter > 0) break;

                if (!targetExists && !holdingTarget) SetState(carrierToProtect ? KnotwoodState.PROTECT : KnotwoodState.IDLE);
                else if (!holdingTarget) SetState(KnotwoodState.IDLE);
                else if (Vector2.Distance(GetKnotwood().GetPosition(), GetTarget().GetPosition()) < 0.1f)
                {
                    SetState(KnotwoodState.EXITING);
                }
                break;

            case KnotwoodState.PROTECT:
                if (!ReachedMovementTarget()) break;
                if (protectAnimationCounter > 0) break;

                if (!carrierToProtect) SetState(KnotwoodState.IDLE);
                else if (targetExists)
                {
                    if (withinAttackRange) SetState(KnotwoodState.ATTACK);
                    else if (withinChaseRange) SetState(KnotwoodState.CHASE);
                }
                break;

            case KnotwoodState.EXITING:
            case KnotwoodState.INVALID:
                break;
            default:
                throw new System.Exception("Invalid state.");
        }
    }

    /// <summary>
    /// Runs logic for the Knotwood model's Inactive state.
    /// </summary>
    protected virtual void ExecuteInactiveState()
    {
        if (GetState() != KnotwoodState.INACTIVE) return;

        SetNextMovePos(GetKnotwood().GetSpawnPos());
    }

    /// <summary>
    /// Runs logic for the Kudzu model's Spawn state.
    /// </summary>
    protected virtual void ExecuteSpawnState()
    {
        if (GetState() != KnotwoodState.SPAWNING) return;

        GetKnotwood().SetSpawning(true);
        SetAnimation(GetKnotwood().MOVE_ANIMATION_DURATION, EnemyFactory.GetSpawnTrack(
            GetKnotwood().TYPE,
                                  GetKnotwood().GetDirection(), GetKnotwood().GetHealthState()));

        PopOutOfMovePos(NexusHoleSpawnPos(GetKnotwood().GetSpawnPos()));
        GetKnotwood().FaceDirection(Direction.SOUTH);

        if (!ReachedMovementTarget()) return;

        // We have fully popped out of the hole.
        SetPoppedOutOfHole();
    }

    /// <summary>
    /// Runs logic for the Kudzu model's idle state.
    /// </summary>
    protected virtual void ExecuteIdleState()
    {
        if (GetState() != KnotwoodState.IDLE) return;

        SetAnimation(GetKnotwood().IDLE_ANIMATION_DURATION, EnemyFactory.GetIdleTrack(
            GetKnotwood().TYPE,
            GetKnotwood().GetDirection(), GetKnotwood().GetHealthState()));

        SetNextMovePos(GetKnotwood().GetPosition());
        MoveLinearlyTowardsMovePos();

        GetKnotwood().FaceDirection(Direction.SOUTH);
    }

    /// <summary>
    /// Runs logic for the Kudzu model's chase state. 
    /// </summary>
    protected virtual void ExecuteChaseState()
    {
        if (GetState() != KnotwoodState.CHASE) return;

        SetAnimation(GetKnotwood().MOVE_ANIMATION_DURATION, EnemyFactory.GetMovementTrack(
            GetKnotwood().TYPE,
            GetKnotwood().GetDirection(), GetKnotwood().GetHealthState()));

        // Move to target.
        MoveLinearlyTowardsMovePos();

        // We reached our move target, so we need a new one.
        if (!ReachedMovementTarget()) return;
        Mob target = GetTarget() as Mob;
        if (target == null || !target.Targetable()) return;

        if (DistanceToTarget() <= GetKnotwood().GetAttackRange()) return;

        Vector3 closest = ClosestPositionToTarget(target);
        Vector3 nextMove = TileGrid.NextTilePosTowardsGoal(GetKnotwood().GetPosition(), closest);
        SetNextMovePos(nextMove);
    }

    /// <summary>
    /// Runs logic for the Kudzu model's protect state.
    /// </summary>
    protected virtual void ExecuteProtectState()
    {
        if (GetState() != KnotwoodState.PROTECT) return;

        SetAnimation(GetKnotwood().MOVE_ANIMATION_DURATION, EnemyFactory.GetMovementTrack(
            GetKnotwood().TYPE,
                       GetKnotwood().GetDirection(), GetKnotwood().GetHealthState()));

        // Move to target.
        MoveLinearlyTowardsMovePos();

        // We reached our move target, so we need a new one.
        if (!ReachedMovementTarget()) return;
        if (GetNearestCarrier() == null) return;

        Vector3 closest = GetNearestCarrier().GetPosition();
        Vector3 nextMove = TileGrid.NextTilePosTowardsGoal(GetKnotwood().GetPosition(), closest);
        SetNextMovePos(nextMove);
    }

    /// <summary>
    /// Runs logic for the Kudzu model's attack state. 
    /// </summary>
    protected virtual void ExecuteAttackState()
    {
        if (GetState() != KnotwoodState.ATTACK) return;

        SetAnimation(GetKnotwood().ATTACK_ANIMATION_DURATION, EnemyFactory.GetAttackTrack(
                GetKnotwood().TYPE,
                       GetKnotwood().GetDirection(), GetKnotwood().GetHealthState()));

        //Attack Logic : Only if target is valid.
        Mob target = GetTarget() as Mob;
        bool validTarget = GetTarget() != null && target.Targetable();
        if (!CanAttack()) return;
        if (!validTarget) return;

        FaceTarget();
        if (CanHoldTarget(target)) HoldTarget(target); // Hold.
        else target.AdjustHealth(-GetKnotwood().KICK_DAMAGE); // Kick.
        GetHeldTargets().ForEach(target => target.SetMaskInteraction(SpriteMaskInteraction.None));
        GetKnotwood().RestartAttackCooldown();
    }

    /// <summary>
    /// Runs logic for the Kudzu model's escaping state. 
    /// </summary>
    protected virtual void ExecuteEscapeState()
    {
        if (GetState() != KnotwoodState.ESCAPE) return;
        if (!ValidModel()) return;
        if (GetTarget() == null) return;

        SetAnimation(GetKnotwood().MOVE_ANIMATION_DURATION, EnemyFactory.GetMovementTrack(
            GetKnotwood().TYPE,
                                  GetKnotwood().GetDirection(), GetKnotwood().GetHealthState()));

        // Move to target.
        if (TileGrid.IsNexusHole(GetNextMovePos())) MoveParabolicallyTowardsMovePos();
        else MoveLinearlyTowardsMovePos();

        // We reached our move target, so we need a new one.
        if (!ReachedMovementTarget()) return;
        SetNextMovePos(TileGrid.NextTilePosTowardsGoal(GetKnotwood().GetPosition(), GetTarget().GetPosition()));
    }

    /// <summary>
    /// Runs logic for the Kudzu's Exit state.
    /// </summary>
    protected virtual void ExecuteExitState()
    {
        if (GetState() != KnotwoodState.EXITING) return;
        if (GetKnotwood().Exited()) return;
        NexusHole nexusHoleTarget = GetTarget() as NexusHole;
        if (nexusHoleTarget == null) return;

        Assert.IsTrue(GetTarget() as NexusHole != null, "exit target needs to be a NH.");
        Vector3 nexusHolePosition = GetTarget().GetPosition();
        Vector3 jumpPosition = nexusHolePosition;
        jumpPosition.y -= 2;

        if (!GetKnotwood().IsExiting() && !GetKnotwood().Exited()) GetKnotwood().SetExiting(nexusHolePosition);

        SetAnimation(GetKnotwood().MOVE_ANIMATION_DURATION, EnemyFactory.GetMovementTrack(
                GetKnotwood().TYPE,
                       GetKnotwood().GetDirection(), GetKnotwood().GetHealthState()));

        // Move to target.
        GetKnotwood().SetMaskInteraction(SpriteMaskInteraction.VisibleOutsideMask);
        GetHeldTargets().ForEach(target => target.SetMaskInteraction(SpriteMaskInteraction.VisibleOutsideMask));
        SetNextMovePos(jumpPosition);
        FallIntoMovePos(3f);

        if (!ReachedMovementTarget()) return;
        if (GetKnotwood().IsExiting() && GetKnotwood().GetPosition() == jumpPosition)
            GetKnotwood().SetExited();
        Assert.IsTrue(NumTargetsHolding() > 0, "You need to hold targets to exit.");
    }

    /// <summary>
    /// Returns the Knotwood prefab to the KnotwoodFactory singleton.
    /// </summary>
    public override void DestroyModel()
    {
        EnemyFactory.ReturnEnemyPrefab(GetKnotwood().gameObject);
    }

    /// <summary>
    /// Returns true if two KnotwoodStates are equal.
    /// </summary>
    /// <param name="stateA">the first KnotwoodState</param>
    /// <param name="stateB">the second KnotwoodState</param>
    /// <returns> true if two KnotwoodStates are equal; otherwise,
    /// false. </returns>
    public override bool StateEquals(KnotwoodState stateA, KnotwoodState stateB)
    {
        return stateA == stateB;
    }

    /// <summary>
    /// Returns the spawn position of the Knotwood when in a NexusHole.
    /// </summary>
    /// <param name="originalSpawnPos">The position of the NexusHole it is
    /// spawning from.</param>
    /// <returns> the spawn position of the Knotwood when in a NexusHole.</returns>
    protected override Vector3 NexusHoleSpawnPos(Vector3 originalSpawnPos)
    {
        originalSpawnPos.y -= 2;
        return originalSpawnPos;
    }

    /// <summary>
    /// Returns true if the Knotwood can target the Model passed
    /// into this method.
    /// </summary>
    /// <param name="target">The Model to check for targetability.</param>
    /// <returns>true if the Knotwood can target the Model passed
    /// into this method; otherwise, false. </returns>
    protected override bool CanTarget(Model target)
    {
        if (!GetKnotwood().Spawned()) return false;
        if (GetState() == KnotwoodState.SPAWNING) return false;

        Nexus nexusTarget = target as Nexus;
        NexusHole nexusHoleTarget = target as NexusHole;

        // If escaping, only target NexusHoles.
        if (GetState() == KnotwoodState.ESCAPE || GetState() == KnotwoodState.EXITING)
        {
            if (nexusHoleTarget == null) return false;
            if (!nexusHoleTarget.Targetable()) return false;
            if (nexusHoleTarget.PickedUp()) return false;
            if (!GetKnotwood().IsExiting() && !TileGrid.CanReach(GetKnotwood().GetPosition(), nexusHoleTarget.GetPosition())) return false;
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
            if (!TileGrid.CanReach(GetKnotwood().GetPosition(), nexusTarget.GetPosition())) return false;
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
        KnotwoodState state = GetState();
        if (state == KnotwoodState.SPAWNING) spawnAnimationCounter += Time.deltaTime;
        else if (state == KnotwoodState.IDLE) idleAnimationCounter += Time.deltaTime;
        else if (state == KnotwoodState.CHASE) chaseAnimationCounter += Time.deltaTime;
        else if (state == KnotwoodState.ATTACK) attackAnimationCounter += Time.deltaTime;
        else if (state == KnotwoodState.PROTECT) protectAnimationCounter += Time.deltaTime;
        else if (state == KnotwoodState.ESCAPE) escapeAnimationCounter += Time.deltaTime;
        else if (state == KnotwoodState.EXITING) exitingAnimationCounter += Time.deltaTime;
    }

    /// <summary>
    /// Returns the animation counter for the current state.
    /// </summary>
    /// <returns>the animation counter for the current state.</returns>
    public override float GetAnimationCounter()
    {
        KnotwoodState state = GetState();
        if (state == KnotwoodState.SPAWNING) return spawnAnimationCounter;
        else if (state == KnotwoodState.IDLE) return idleAnimationCounter;
        else if (state == KnotwoodState.CHASE) return chaseAnimationCounter;
        else if (state == KnotwoodState.ATTACK) return attackAnimationCounter;
        else if (state == KnotwoodState.PROTECT) return protectAnimationCounter;
        else if (state == KnotwoodState.ESCAPE) return escapeAnimationCounter;
        else if (state == KnotwoodState.EXITING) return exitingAnimationCounter;
        else return 0;
    }

    /// <summary>
    /// Sets the animation counter for the current state to 0.
    /// </summary>
    public override void ResetAnimationCounter()
    {
        KnotwoodState state = GetState();
        if (state == KnotwoodState.SPAWNING) spawnAnimationCounter = 0;
        else if (state == KnotwoodState.IDLE) idleAnimationCounter = 0;
        else if (state == KnotwoodState.CHASE) chaseAnimationCounter = 0;
        else if (state == KnotwoodState.ATTACK) attackAnimationCounter = 0;
        else if (state == KnotwoodState.PROTECT) protectAnimationCounter = 0;
        else if (state == KnotwoodState.ESCAPE) escapeAnimationCounter = 0;
        else if (state == KnotwoodState.EXITING) exitingAnimationCounter = 0;
    }
}
