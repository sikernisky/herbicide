using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using static SpurgeController;

/// <summary>
/// Controls a SpurgeMinion. <br></br>
/// 
/// The SpurgeMinionController is responsible for manipulating its SpurgeMinion and bringing
/// it to life. This includes moving it, choosing targets, playing animations,
/// and more.
/// </summary>
/// <![CDATA[<param name="SpurgeMinionState">]]>
public class SpurgeMinionController : EnemyController<SpurgeMinionController.SpurgeMinionState>
{
    #region Fields

    /// <summary>
    /// All possible states of the SpurgeMinion.
    /// </summary>
    public enum SpurgeMinionState
    {
        INACTIVE, // Not spawned yet.
        ENTERING, // Entering map.
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
    /// The maximum number of targets a SpurgeMinion can select at once.
    /// </summary>
    protected override int MAX_TARGETS => 1;

    /// <summary>
    /// Counts the number of seconds in the spawn animation; resets
    /// on step.
    /// </summary>
    protected float spawnAnimationCounter;

    /// <summary>
    /// Counts the number of seconds in the idle animation; resets
    /// on step.
    /// </summary>
    protected float idleAnimationCounter;

    /// <summary>
    /// Counts the number of seconds in the chase animation; resets
    /// on step.
    /// </summary>
    protected float chaseAnimationCounter;

    /// <summary>
    /// Counts the number of seconds in the attack animation; resets
    /// on step.
    /// </summary>
    protected float attackAnimationCounter;

    /// <summary>
    /// Counts the number of seconds in the protect animation; resets
    /// on step.
    /// </summary>
    protected float protectAnimationCounter;

    /// <summary>
    /// Counts the number of seconds in the escape animation; resets
    /// on step.
    /// </summary>
    protected float escapeAnimationCounter;

    /// <summary>
    /// Counts the number of seconds in the exiting animation; resets
    /// on step.
    /// </summary>
    protected float exitingAnimationCounter;

    #endregion

    #region Methods

    /// <summary>
    /// Makes a new SpurgeMinionController.
    /// </summary>
    /// <param name="spurgeMinion">The SpurgeMinion Enemy. </param>
    /// <returns>The created SpurgeMinionController.</returns>
    public SpurgeMinionController(SpurgeMinion spurgeMinion) : base(spurgeMinion) { }

    /// <summary>
    /// Returns this SpurgeMinionController's SpurgeMinion model.
    /// </summary>
    /// <returns>this SpurgeMinionController's SpurgeMinion model.</returns>
    private SpurgeMinion GetSpurge() => GetEnemy() as SpurgeMinion;

    /// <summary>
    /// Updates the SpurgeMinion model controlled by this SpurgeMinionController.
    /// </summary>
    protected override void UpdateMob()
    {
        base.UpdateMob();
        ExecuteInactiveState();
        ExecuteEnteringState();
        ExecuteIdleState();
        ExecuteChaseState();
        ExecuteAttackState();
        ExecuteProtectState();
        ExecuteEscapeState();
        ExecuteExitState();
    }

    /// <summary>
    /// Returns the spawn position of the SpurgeMinion when in a NexusHole.
    /// </summary>
    /// <param name="originalSpawnPos">The position of the NexusHole it is
    /// spawning from.</param>
    /// <returns> the spawn position of the SpurgeMinion when in a NexusHole.</returns>
    protected override Vector3 NexusHoleSpawnPos(Vector3 originalSpawnPos)
    {
        originalSpawnPos.y -= 2;
        return originalSpawnPos;
    }

    /// <summary>
    /// Returns true if the current SpurgeMinionState renders the SpurgeMinion
    /// immune to damage.
    /// </summary>
    /// <returns>true if the current SpurgeMinionState renders the SpurgeMinion
    /// immune to damage; otherwise, false. </returns>
    protected override bool IsCurrentStateImmune()
    {
        SpurgeMinionState state = GetState();
        return state == SpurgeMinionState.INACTIVE ||
               state == SpurgeMinionState.ENTERING ||
               state == SpurgeMinionState.EXITING ||
               state == SpurgeMinionState.INVALID;
    }

    /// <summary>
    /// Returns true if the SpurgeMinion can target the Model passed
    /// into this method.
    /// </summary>
    /// <param name="target">The Model to check for targetability.</param>
    /// <returns>true if the SpurgeMinion can target the Model passed
    /// into this method; otherwise, false. </returns>
    protected override bool CanTargetOtherModel(Model target)
    {
        if (!GetSpurge().Spawned()) return false;
        if (GetState() == SpurgeMinionState.ENTERING ||
            GetState() == SpurgeMinionState.INACTIVE) return false;


        Nexus nexusTarget = target as Nexus;
        NexusHole nexusHoleTarget = target as NexusHole;

        // If escaping, only target NexusHoles.
        if (GetState() == SpurgeMinionState.ESCAPE || GetState() == SpurgeMinionState.EXITING)
        {
            if (nexusHoleTarget == null) return false;
            if (!nexusHoleTarget.Targetable()) return false;
            if (!GetSpurge().IsExiting() && !TileGrid.CanReach(GetSpurge().GetPosition(), nexusHoleTarget.GetPosition())) return false;
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
            if (!TileGrid.CanReach(GetSpurge().GetPosition(), nexusTarget.GetPosition())) return false;

            return true;
        }

        throw new System.Exception("Something wrong happened.");
    }



    #endregion

    #region State Logic

    /// <summary>
    /// Updates the state of this SpurgeMinionController's SpurgeMinion model.
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
    public override void UpdateFSM()
    {
        if (!ValidModel()) return;
        if (GetGameState() != GameState.ONGOING)
        {
            SetState(SpurgeMinionState.IDLE);
            return;
        }

        Mob target = GetTarget() as Mob;
        bool targetExists = target != null && target.Targetable();
        bool carrierToProtect = GetNearestCarrier() != null;
        bool withinChaseRange = targetExists && DistanceToTarget() <= GetSpurge().GetChaseRange();
        bool withinAttackRange = targetExists && DistanceToTarget() <= GetSpurge().GetAttackRange();
        bool holdingTarget = NumTargetsHolding() > 0;

        switch (GetState())
        {
            case SpurgeMinionState.INACTIVE:
                if (GetSpurge().Spawned()) SetState(SpurgeMinionState.ENTERING);
                break;

            case SpurgeMinionState.ENTERING:
                if (PoppedOutOfHole()) SetState(SpurgeMinionState.IDLE);
                break;

            case SpurgeMinionState.IDLE:
                if (!ReachedMovementTarget()) break;

                if (!targetExists && carrierToProtect) SetState(SpurgeMinionState.PROTECT);
                else if (targetExists && withinChaseRange) SetState(SpurgeMinionState.CHASE);
                else if (holdingTarget) SetState(SpurgeMinionState.ESCAPE);
                break;

            case SpurgeMinionState.CHASE:
                if (!ReachedMovementTarget()) break;

                if (!targetExists) SetState(carrierToProtect ? SpurgeMinionState.PROTECT : SpurgeMinionState.IDLE);
                else if (withinAttackRange) SetState(SpurgeMinionState.ATTACK);
                else if (!withinChaseRange) SetState(SpurgeMinionState.IDLE);
                else if (holdingTarget) SetState(SpurgeMinionState.ESCAPE);
                break;

            case SpurgeMinionState.ATTACK:
                if (!ReachedMovementTarget()) break;

                if (holdingTarget) SetState(SpurgeMinionState.ESCAPE);
                else if (!targetExists) SetState(carrierToProtect ? SpurgeMinionState.PROTECT : SpurgeMinionState.IDLE);
                else if (!withinAttackRange) SetState(withinChaseRange ? SpurgeMinionState.CHASE : SpurgeMinionState.IDLE);

                break;
            case SpurgeMinionState.ESCAPE:
                if (!ReachedMovementTarget()) break;

                if (!targetExists && !holdingTarget) SetState(carrierToProtect ? SpurgeMinionState.PROTECT : SpurgeMinionState.IDLE);
                else if (!holdingTarget) SetState(SpurgeMinionState.IDLE);
                else if (Vector2.Distance(GetSpurge().GetPosition(), GetTarget().GetPosition()) < 0.05f)
                {
                    SetState(SpurgeMinionState.EXITING);
                }
                break;

            case SpurgeMinionState.PROTECT:
                if (!ReachedMovementTarget()) break;

                if (!carrierToProtect) SetState(SpurgeMinionState.IDLE);
                else if (targetExists)
                {
                    if (withinAttackRange) SetState(SpurgeMinionState.ATTACK);
                    else if (withinChaseRange) SetState(SpurgeMinionState.CHASE);
                }
                break;

            case SpurgeMinionState.EXITING:
            case SpurgeMinionState.INVALID:
                break;
            default:
                throw new System.Exception("Invalid state.");
        }
    }

    /// <summary>
    /// Returns true if two SpurgeStates are equal.
    /// </summary>
    /// <param name="stateA">The first SpurgeState</param>
    /// <param name="stateB">The second SpurgeState</param>
    /// <returns>true if two SpurgeStates are equal; otherwise, false. </returns>
    public override bool StateEquals(SpurgeMinionState stateA, SpurgeMinionState stateB) => stateA == stateB;

    /// <summary>
    /// Runs logic for the SpurgeMinion model's Inactive state.
    /// </summary>
    protected void ExecuteInactiveState()
    {
        if (GetState() != SpurgeMinionState.INACTIVE) return;

        SetNextMovePos(GetSpurge().GetSpawnPos());
    }

    /// <summary>
    /// Runs logic for the SpurgeMinion model's Spawn state.
    /// </summary>
    protected void ExecuteEnteringState()
    {
        if (GetState() != SpurgeMinionState.ENTERING) return;

        GetSpurge().SetEntering(GetSpurge().GetSpawnPos());
        SetAnimation(GetSpurge().MOVE_ANIMATION_DURATION, EnemyFactory.GetSpawnTrack(
            GetSpurge().TYPE, GetSpurge().GetDirection(), GetSpurge().GetHealthState()));

        PopOutOfMovePos(NexusHoleSpawnPos(GetSpurge().GetSpawnPos()));
        GetSpurge().FaceDirection(Direction.SOUTH);

        if (!ReachedMovementTarget()) return;

        // We have fully popped out of the hole.
        SetPoppedOutOfHole();
    }

    /// <summary>
    /// Runs logic for the SpurgeMinion model's idle state.
    /// </summary>
    protected void ExecuteIdleState()
    {
        if (GetState() != SpurgeMinionState.IDLE) return;

        GetSpurge().SetEntered();
        SetAnimation(GetSpurge().IDLE_ANIMATION_DURATION, EnemyFactory.GetIdleTrack(
            GetSpurge().TYPE,
            GetSpurge().GetDirection(), GetSpurge().GetHealthState()));

        SetNextMovePos(GetSpurge().GetPosition());
        MoveLinearlyTowardsMovePos();

        GetSpurge().FaceDirection(Direction.SOUTH);
    }

    /// <summary>
    /// Runs logic for the SpurgeMinion model's chase state. 
    /// </summary>
    protected void ExecuteChaseState()
    {
        if (GetState() != SpurgeMinionState.CHASE) return;

        SetAnimation(GetSpurge().MOVE_ANIMATION_DURATION, EnemyFactory.GetMovementTrack(
            GetSpurge().TYPE,
            GetSpurge().GetDirection(), GetSpurge().GetHealthState()));

        // Move to target.
        MoveLinearlyTowardsMovePos();

        // We reached our move target, so we need a new one.
        if (!ReachedMovementTarget()) return;
        Mob target = GetTarget() as Mob;
        if (target == null || !target.Targetable()) return;

        if (DistanceToTarget() <= GetSpurge().GetAttackRange()) return;

        Vector3 closest = ClosestTileCoordinatePositionToTarget(target);
        Vector3 nextMove = TileGrid.NextTilePosTowardsGoal(GetSpurge().GetPosition(), closest);
        SetNextMovePos(nextMove);
    }

    /// <summary>
    /// Runs logic for the SpurgeMinion model's protect state.
    /// </summary>
    protected void ExecuteProtectState()
    {
        if (GetState() != SpurgeMinionState.PROTECT) return;

        SetAnimation(GetSpurge().MOVE_ANIMATION_DURATION, EnemyFactory.GetMovementTrack(
            GetSpurge().TYPE,
                       GetSpurge().GetDirection(), GetSpurge().GetHealthState()));

        // Move to target.
        MoveLinearlyTowardsMovePos();

        // We reached our move target, so we need a new one.
        if (!ReachedMovementTarget()) return;
        if (GetNearestCarrier() == null) return;

        Vector3 closest = GetNearestCarrier().GetPosition();
        Vector3 nextMove = TileGrid.NextTilePosTowardsGoal(GetSpurge().GetPosition(), closest);
        SetNextMovePos(nextMove);
    }

    /// <summary>
    /// Runs logic for the SpurgeMinion model's attack state. 
    /// </summary>
    protected void ExecuteAttackState()
    {
        if (GetState() != SpurgeMinionState.ATTACK) return;

        SetAnimation(GetSpurge().ATTACK_ANIMATION_DURATION, EnemyFactory.GetAttackTrack(
                GetSpurge().TYPE,
                       GetSpurge().GetDirection(), GetSpurge().GetHealthState()));

        //Attack Logic : Only if target is valid.
        Mob target = GetTarget() as Mob;
        bool validTarget = GetTarget() != null && target.Targetable();
        if (!CanAttack()) return;
        if (!validTarget) return;

        FaceTarget();
        if (CanHoldTarget(target as Nexus)) HoldTarget(target as Nexus); // Hold.
        GetHeldTargets().ForEach(target => target.SetMaskInteraction(SpriteMaskInteraction.None));
        GetSpurge().RestartAttackCooldown();
    }

    /// <summary>
    /// Runs logic for the SpurgeMinion model's escaping state. 
    /// </summary>
    protected void ExecuteEscapeState()
    {
        if (GetState() != SpurgeMinionState.ESCAPE) return;
        if (!ValidModel()) return;
        if (GetTarget() == null) return;

        SetAnimation(GetSpurge().MOVE_ANIMATION_DURATION, EnemyFactory.GetMovementTrack(
            GetSpurge().TYPE,
                                  GetSpurge().GetDirection(), GetSpurge().GetHealthState()));

        // Move to target.
        MoveLinearlyTowardsMovePos();

        // We reached our move target, so we need a new one.
        if (!ReachedMovementTarget()) return;
        SetNextMovePos(TileGrid.NextTilePosTowardsGoal(GetSpurge().GetPosition(), GetTarget().GetPosition()));
    }

    /// <summary>
    /// Runs logic for the SpurgeMinion's Exit state.
    /// </summary>
    protected void ExecuteExitState()
    {
        if (GetState() != SpurgeMinionState.EXITING) return;
        if (GetSpurge().Exited()) return;
        NexusHole nexusHoleTarget = GetTarget() as NexusHole;
        if (nexusHoleTarget == null) return;

        Assert.IsTrue(GetTarget() as NexusHole != null, "exit target needs to be a NH.");
        Vector3 nexusHolePosition = GetTarget().GetPosition();
        Vector3 jumpPosition = nexusHolePosition;
        jumpPosition.y -= .5f;

        if (!GetSpurge().IsExiting() && !GetSpurge().Exited()) GetSpurge().SetExiting(nexusHolePosition);

        SetAnimation(GetSpurge().MOVE_ANIMATION_DURATION, EnemyFactory.GetMovementTrack(
                GetSpurge().TYPE,
                       GetSpurge().GetDirection(), GetSpurge().GetHealthState()));

        // Move to target.
        GetSpurge().SetMaskInteraction(SpriteMaskInteraction.VisibleOutsideMask);
        GetHeldTargets().ForEach(target => target.SetMaskInteraction(SpriteMaskInteraction.VisibleOutsideMask));
        SetNextMovePos(jumpPosition);
        FallIntoMovePos(3f);

        if (!ReachedMovementTarget()) return;
        if (GetSpurge().IsExiting() && GetSpurge().GetPosition() == jumpPosition)
        {
            GetSpurge().SetExited();
            foreach (Model target in GetHeldTargets())
            {
                Nexus nexusTarget = target as Nexus;
                if (nexusTarget != null) nexusTarget.CashIn();
            }
        }

        Assert.IsTrue(NumTargetsHolding() > 0, "You need to hold targets to exit.");
    }

    #endregion

    #region Animation Logic

    /// <summary>
    /// Adds one chunk of Time.deltaTime to the animation
    /// counter that tracks the current state.
    /// </summary>
    public override void AgeAnimationCounter()
    {
        SpurgeMinionState state = GetState();
        if (state == SpurgeMinionState.ENTERING) spawnAnimationCounter += Time.deltaTime;
        else if (state == SpurgeMinionState.IDLE) idleAnimationCounter += Time.deltaTime;
        else if (state == SpurgeMinionState.CHASE) chaseAnimationCounter += Time.deltaTime;
        else if (state == SpurgeMinionState.ATTACK) attackAnimationCounter += Time.deltaTime;
        else if (state == SpurgeMinionState.PROTECT) protectAnimationCounter += Time.deltaTime;
        else if (state == SpurgeMinionState.ESCAPE) escapeAnimationCounter += Time.deltaTime;
        else if (state == SpurgeMinionState.EXITING) exitingAnimationCounter += Time.deltaTime;
    }

    /// <summary>
    /// Returns the animation counter for the current state.
    /// </summary>
    /// <returns>the animation counter for the current state.</returns>
    public override float GetAnimationCounter()
    {
        SpurgeMinionState state = GetState();
        if (state == SpurgeMinionState.ENTERING) return spawnAnimationCounter;
        else if (state == SpurgeMinionState.IDLE) return idleAnimationCounter;
        else if (state == SpurgeMinionState.CHASE) return chaseAnimationCounter;
        else if (state == SpurgeMinionState.ATTACK) return attackAnimationCounter;
        else if (state == SpurgeMinionState.PROTECT) return protectAnimationCounter;
        else if (state == SpurgeMinionState.ESCAPE) return escapeAnimationCounter;
        else if (state == SpurgeMinionState.EXITING) return exitingAnimationCounter;
        else return 0;
    }

    /// <summary>
    /// Sets the animation counter for the current state to 0.
    /// </summary>
    public override void ResetAnimationCounter()
    {
        SpurgeMinionState state = GetState();
        if (state == SpurgeMinionState.ENTERING) spawnAnimationCounter = 0;
        else if (state == SpurgeMinionState.IDLE) idleAnimationCounter = 0;
        else if (state == SpurgeMinionState.CHASE) chaseAnimationCounter = 0;
        else if (state == SpurgeMinionState.ATTACK) attackAnimationCounter = 0;
        else if (state == SpurgeMinionState.PROTECT) protectAnimationCounter = 0;
        else if (state == SpurgeMinionState.ESCAPE) escapeAnimationCounter = 0;
        else if (state == SpurgeMinionState.EXITING) exitingAnimationCounter = 0;
    }

    #endregion
}
