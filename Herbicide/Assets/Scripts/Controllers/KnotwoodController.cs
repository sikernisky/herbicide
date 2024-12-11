using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using static KudzuController;

/// <summary>
/// Controls a Knotwood. <br></br>
/// 
/// The KnotwoodController is responsible for manipulating its Knotwood and bringing
/// it to life. This includes moving it, choosing targets, playing animations,
/// and more.
/// </summary>
/// <![CDATA[<param name="KnotwoodState">]]>
public class KnotwoodController : EnemyController<KnotwoodController.KnotwoodState>
{
    #region Fields

    /// <summary>
    /// All possible states of the Knotwood.
    /// </summary>
    public enum KnotwoodState
    {
        INACTIVE, // Not spawned yet.
        ENTERING, // Entering map.
        IDLE, // Static, nothing to do.
        CHASE, // Pursuing target.
        ATTACK, // Attacking target.
        ESCAPE, // Running back to start.
        PROTECT, // Protecting other escaping enemies.
        DEAD, // Dead.
        INVALID // Something went wrong.
    }

    /// <summary>
    /// The maximum number of targets a Knotwood can select at once.
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

    #endregion

    #region Methods

    /// <summary>
    /// Makes a new KnotwoodController.
    /// </summary>
    /// <param name="knotwood">The Knotwood Enemy. </param>
    /// <returns>The created KnotwoodController.</returns>
    public KnotwoodController(Knotwood knotwood) : base(knotwood) { }

    /// <summary>
    /// Updates the Knotwood model controlled by this KnotwoodController.
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
    }

    /// <summary>
    /// Returns the Knotwood model of this KnotwoodController.
    /// </summary>
    /// <returns>the Knotwood model of this KnotwoodController.</returns>
    protected Knotwood GetKnotwood() => GetEnemy() as Knotwood;

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
    protected override bool CanTargetOtherModel(Model target)
    {
        if (!GetKnotwood().Spawned()) return false;
        if (GetState() == KnotwoodState.ENTERING) return false;

        Nexus nexusTarget = target as Nexus;
        NexusHole nexusHoleTarget = target as NexusHole;

        // If escaping, only target NexusHoles.
        if (GetState() == KnotwoodState.ESCAPE)
        {
            if (nexusHoleTarget == null) return false;
            if (!nexusHoleTarget.Targetable()) return false;
            if (!GetKnotwood().IsExiting() && !TileGrid.CanReach(GetKnotwood().GetPosition(), nexusHoleTarget.GetPosition())) return false;
            if (!IsClosestTargetableModelAlongPath(nexusHoleTarget)) return false;

            return true;
        }

        // If not escaping, only target Nexii.
        else
        {
            if (nexusTarget == null) return false;
            if (!nexusTarget.Targetable()) return false;
            if (nexusTarget.PickedUp()) return false;
            if (nexusTarget.CashedIn()) return false;
            if (!IsClosestTargetableNexusAlongPath(nexusTarget)) return false;
            if (!TileGrid.CanReach(GetKnotwood().GetPosition(), nexusTarget.GetPosition())) return false;

            return true;
        }

        throw new System.Exception("Something wrong happened.");
    }

    /// <summary>
    /// Returns true if the current KnotwoodState renders the Knotwood
    /// immune to damage.
    /// </summary>
    /// <returns>true if the current KnotwoodState renders the Knotwood
    /// immune to damage; otherwise, false. </returns>
    protected override bool IsCurrentStateImmune()
    {
        KnotwoodState state = GetState();
        return state == KnotwoodState.INACTIVE ||
               state == KnotwoodState.ENTERING ||
               state == KnotwoodState.INVALID;
    }

    #endregion

    #region State Logic

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
    public override void UpdateFSM()
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
        bool withinAttackRange = targetExists && DistanceToTarget() <= GetKnotwood().GetMainActionRange();
        bool holdingTarget = NumTargetsHolding() > 0;

        switch (GetState())
        {
            case KnotwoodState.INACTIVE:
                if (GetKnotwood().Spawned()) SetState(KnotwoodState.ENTERING);
                break;

            case KnotwoodState.ENTERING:
                if (PoppedOutOfHole()) SetState(KnotwoodState.IDLE);
                break;

            case KnotwoodState.IDLE:
                if (!ReachedMovementTarget()) break;

                if (!targetExists && carrierToProtect) SetState(KnotwoodState.PROTECT);
                else if (targetExists && withinChaseRange) SetState(KnotwoodState.CHASE);
                else if (holdingTarget) SetState(KnotwoodState.ESCAPE);
                break;

            case KnotwoodState.CHASE:
                if (!ReachedMovementTarget()) break;

                if (!targetExists) SetState(carrierToProtect ? KnotwoodState.PROTECT : KnotwoodState.IDLE);
                else if (withinAttackRange) SetState(KnotwoodState.ATTACK);
                else if (!withinChaseRange) SetState(KnotwoodState.IDLE);
                else if (holdingTarget) SetState(KnotwoodState.ESCAPE);
                break;

            case KnotwoodState.ATTACK:
                if (!ReachedMovementTarget()) break;

                if (holdingTarget) SetState(KnotwoodState.ESCAPE);
                else if (!targetExists) SetState(carrierToProtect ? KnotwoodState.PROTECT : KnotwoodState.IDLE);
                else if (!withinAttackRange) SetState(withinChaseRange ? KnotwoodState.CHASE : KnotwoodState.IDLE);

                break;
            case KnotwoodState.ESCAPE:
                if (!ReachedMovementTarget()) break;

                if (!targetExists && !holdingTarget) SetState(carrierToProtect ? KnotwoodState.PROTECT : KnotwoodState.IDLE);
                else if (!holdingTarget) SetState(KnotwoodState.IDLE);
                break;

            case KnotwoodState.PROTECT:
                if (!ReachedMovementTarget()) break;

                if (!carrierToProtect) SetState(KnotwoodState.IDLE);
                else if (targetExists)
                {
                    if (withinAttackRange) SetState(KnotwoodState.ATTACK);
                    else if (withinChaseRange) SetState(KnotwoodState.CHASE);
                }
                break;

            case KnotwoodState.INVALID:
                break;
            default:
                throw new System.Exception("Invalid state.");
        }
    }

    /// <summary>
    /// Returns true if two KnotwoodStates are equal.
    /// </summary>
    /// <param name="stateA">The first KnotwoodState</param>
    /// <param name="stateB">The second KnotwoodState</param>
    /// <returns>true if two KnotwoodStates are equal; otherwise, false. </returns>
    public override bool StateEquals(KnotwoodState stateA, KnotwoodState stateB) => stateA == stateB;

    /// <summary>
    /// Runs logic for the Knotwood model's Inactive state.
    /// </summary>
    protected void ExecuteInactiveState()
    {
        if (GetState() != KnotwoodState.INACTIVE) return;

        SetNextMovePos(GetKnotwood().GetSpawnWorldPosition());
    }

    /// <summary>
    /// Runs logic for the Knotwood model's Spawn state.
    /// </summary>
    protected void ExecuteEnteringState()
    {
        if (GetState() != KnotwoodState.ENTERING) return;

        GetKnotwood().SetEntering(GetKnotwood().GetSpawnWorldPosition());
        SetNextAnimation(GetKnotwood().GetMovementAnimationDuration(), EnemyFactory.GetSpawnTrack(
            GetKnotwood().TYPE,
                                  GetKnotwood().GetDirection(), GetKnotwood().GetHealthState()));

        PopOutOfMovePos(NexusHoleSpawnPos(GetKnotwood().GetSpawnWorldPosition()));
        GetKnotwood().FaceDirection(Direction.SOUTH);

        if (!ReachedMovementTarget()) return;

        // We have fully popped out of the hole.
        SetPoppedOutOfHole();
    }

    /// <summary>
    /// Runs logic for the Knotwood model's idle state.
    /// </summary>
    protected void ExecuteIdleState()
    {
        if (GetState() != KnotwoodState.IDLE) return;

        GetKnotwood().SetEntered();
        SetNextAnimation(GetKnotwood().IDLE_ANIMATION_DURATION, EnemyFactory.GetIdleTrack(
            GetKnotwood().TYPE,
            GetKnotwood().GetDirection(), GetKnotwood().GetHealthState()));

        SetNextMovePos(GetKnotwood().GetPosition());
        MoveLinearlyTowardsMovePos();

        GetKnotwood().FaceDirection(Direction.SOUTH);
    }

    /// <summary>
    /// Runs logic for the Knotwood model's chase state. 
    /// </summary>
    protected void ExecuteChaseState()
    {
        if (GetState() != KnotwoodState.CHASE) return;

        SetNextAnimation(GetKnotwood().GetMovementAnimationDuration(), EnemyFactory.GetMovementTrack(
            GetKnotwood().TYPE,
            GetKnotwood().GetDirection(), GetKnotwood().GetHealthState()));

        // Move to target.
        MoveLinearlyTowardsMovePos();

        // We reached our move target, so we need a new one.
        if (!ReachedMovementTarget()) return;
        Mob target = GetTarget() as Mob;
        if (target == null || !target.Targetable()) return;

        if (DistanceToTarget() <= GetKnotwood().GetMainActionRange()) return;

        Vector3 nextMove = TileGrid.NextTilePosTowardsGoal(GetKnotwood().GetPosition(), GetTarget().GetPosition());
        SetNextMovePos(nextMove);
    }

    /// <summary>
    /// Runs logic for the Knotwood model's protect state.
    /// </summary>
    protected void ExecuteProtectState()
    {
        if (GetState() != KnotwoodState.PROTECT) return;

        SetNextAnimation(GetKnotwood().GetMovementAnimationDuration(), EnemyFactory.GetMovementTrack(
            GetKnotwood().TYPE,
                       GetKnotwood().GetDirection(), GetKnotwood().GetHealthState()));

        // Move to target.
        MoveLinearlyTowardsMovePos();

        // We reached our move target, so we need a new one.
        if (!ReachedMovementTarget()) return;
        if (GetNearestCarrier() == null) return;

        Vector3 nextMove = TileGrid.NextTilePosTowardsGoal(GetKnotwood().GetPosition(), GetNearestCarrier().GetPosition());
        SetNextMovePos(nextMove);
    }

    /// <summary>
    /// Runs logic for the Knotwood model's attack state. 
    /// </summary>
    protected void ExecuteAttackState()
    {
        if (GetState() != KnotwoodState.ATTACK) return;

        SetNextAnimation(GetKnotwood().GetMainActionAnimationDuration(), EnemyFactory.GetAttackTrack(
                GetKnotwood().TYPE,
                       GetKnotwood().GetDirection(), GetKnotwood().GetHealthState()));

        //Attack Logic : Only if target is valid.
        Mob target = GetTarget() as Mob;
        bool validTarget = GetTarget() != null && target.Targetable();
        if (!CanPerformMainAction()) return;
        if (!validTarget) return;

        FaceTarget();
        if (CanHoldTarget(target as Nexus)) HoldTarget(target as Nexus); // Hold.
        else target.AdjustHealth(-GetKnotwood().KICK_DAMAGE); // Kick.
        GetHeldTargets().ForEach(target => target.SetMaskInteraction(SpriteMaskInteraction.None));
        GetKnotwood().RestartMainActionCooldown();
    }

    /// <summary>
    /// Runs logic for the Knotwood model's escaping state. 
    /// </summary>
    protected void ExecuteEscapeState()
    {
        if (GetState() != KnotwoodState.ESCAPE) return;
        if (!ValidModel()) return;
        if (GetTarget() == null) return;

        SetNextAnimation(GetKnotwood().GetMovementAnimationDuration(), EnemyFactory.GetMovementTrack(
            GetKnotwood().TYPE,
                                  GetKnotwood().GetDirection(), GetKnotwood().GetHealthState()));

        // Move to target.
        MoveLinearlyTowardsMovePos();

        if(Vector2.Distance(GetKnotwood().GetPosition(), GetTarget().GetPosition()) < 0.05f)
        {
            GetKnotwood().SetExited();
            foreach (Model target in GetHeldTargets())
            {
                Nexus nexusTarget = target as Nexus;
                if (nexusTarget != null) nexusTarget.CashIn();
            }
        }

        // We reached our move target, so we need a new one.
        if (!ReachedMovementTarget()) return;
        SetNextMovePos(TileGrid.NextTilePosTowardsGoal(GetKnotwood().GetPosition(), GetTarget().GetPosition()));
    }

    #endregion

    #region Animation Logic

    /// <summary>
    /// Adds one chunk of Time.deltaTime to the animation
    /// counter that tracks the current state.
    /// </summary>
    public override void AgeAnimationCounter()
    {
        KnotwoodState state = GetState();
        if (state == KnotwoodState.ENTERING) spawnAnimationCounter += Time.deltaTime;
        else if (state == KnotwoodState.IDLE) idleAnimationCounter += Time.deltaTime;
        else if (state == KnotwoodState.CHASE) chaseAnimationCounter += Time.deltaTime;
        else if (state == KnotwoodState.ATTACK) attackAnimationCounter += Time.deltaTime;
        else if (state == KnotwoodState.PROTECT) protectAnimationCounter += Time.deltaTime;
        else if (state == KnotwoodState.ESCAPE) escapeAnimationCounter += Time.deltaTime;
    }

    /// <summary>
    /// Returns the animation counter for the current state.
    /// </summary>
    /// <returns>the animation counter for the current state.</returns>
    public override float GetAnimationCounter()
    {
        KnotwoodState state = GetState();
        if (state == KnotwoodState.ENTERING) return spawnAnimationCounter;
        else if (state == KnotwoodState.IDLE) return idleAnimationCounter;
        else if (state == KnotwoodState.CHASE) return chaseAnimationCounter;
        else if (state == KnotwoodState.ATTACK) return attackAnimationCounter;
        else if (state == KnotwoodState.PROTECT) return protectAnimationCounter;
        else if (state == KnotwoodState.ESCAPE) return escapeAnimationCounter;
        else return 0;
    }

    /// <summary>
    /// Sets the animation counter for the current state to 0.
    /// </summary>
    public override void ResetAnimationCounter()
    {
        KnotwoodState state = GetState();
        if (state == KnotwoodState.ENTERING) spawnAnimationCounter = 0;
        else if (state == KnotwoodState.IDLE) idleAnimationCounter = 0;
        else if (state == KnotwoodState.CHASE) chaseAnimationCounter = 0;
        else if (state == KnotwoodState.ATTACK) attackAnimationCounter = 0;
        else if (state == KnotwoodState.PROTECT) protectAnimationCounter = 0;
        else if (state == KnotwoodState.ESCAPE) escapeAnimationCounter = 0;
    }

    #endregion
}
