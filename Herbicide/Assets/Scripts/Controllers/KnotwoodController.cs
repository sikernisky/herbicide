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
    /// Returns the spawn position of the Knotwood when in a SpawnHole.
    /// </summary>
    /// <param name="originalSpawnPos">The position of the SpawnHole it is
    /// spawning from.</param>
    /// <returns> the spawn position of the Knotwood when in a SpawnHole.</returns>
    protected override Vector3 SpawnHoleSpawnPos(Vector3 originalSpawnPos)
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

        SpawnHole spawnHoleTarget = target as SpawnHole;

        if (spawnHoleTarget == null) return false;
        if (!spawnHoleTarget.Targetable()) return false;
        if (!TileGrid.CanReach(GetKnotwood().GetWorldPosition(), spawnHoleTarget.GetWorldPosition())) return false;

        return true;
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
                //if (PoppedOutOfHole()) SetState(KnotwoodState.IDLE);
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
                                  GetKnotwood().Direction, GetKnotwood().GetHealthState()));

        PopOutOfMovePos(SpawnHoleSpawnPos(GetKnotwood().GetSpawnWorldPosition()));
        GetKnotwood().Direction = Direction.SOUTH;

        if (!ReachedMovementTarget()) return;

        // We have fully popped out of the hole.
        //SetPoppedOutOfHole();
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
            GetKnotwood().Direction, GetKnotwood().GetHealthState()));

        SetNextMovePos(GetKnotwood().GetWorldPosition());
        MoveLinearlyTowardsMovePos();

        GetKnotwood().Direction = Direction.SOUTH;
    }

    /// <summary>
    /// Runs logic for the Knotwood model's chase state. 
    /// </summary>
    protected void ExecuteChaseState()
    {
        if (GetState() != KnotwoodState.CHASE) return;

        SetNextAnimation(GetKnotwood().GetMovementAnimationDuration(), EnemyFactory.GetMovementTrack(
            GetKnotwood().TYPE,
            GetKnotwood().Direction, GetKnotwood().GetHealthState()));

        // Move to target.
        MoveLinearlyTowardsMovePos();

        // We reached our move target, so we need a new one.
        if (!ReachedMovementTarget()) return;
        Mob target = GetTarget() as Mob;
        if (target == null || !target.Targetable()) return;

        if (DistanceToTarget() <= GetKnotwood().GetMainActionRange()) return;

        Vector3 nextMove = TileGrid.NextTilePosTowardsGoalUsingAStar(GetKnotwood().GetWorldPosition(), GetTarget().GetWorldPosition());
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
                       GetKnotwood().Direction, GetKnotwood().GetHealthState()));

        // Move to target.
        MoveLinearlyTowardsMovePos();

        // We reached our move target, so we need a new one.
        if (!ReachedMovementTarget()) return;
        if (GetNearestCarrier() == null) return;

        Vector3 nextMove = TileGrid.NextTilePosTowardsGoalUsingAStar(GetKnotwood().GetWorldPosition(), GetNearestCarrier().GetWorldPosition());
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
                       GetKnotwood().Direction, GetKnotwood().GetHealthState()));

        //Attack Logic : Only if target is valid.
        Mob target = GetTarget() as Mob;
        bool validTarget = GetTarget() != null && target.Targetable();
        if (!CanPerformMainAction()) return;
        if (!validTarget) return;

        FaceTarget();
        target.AdjustHealth(-GetKnotwood().KICK_DAMAGE); // Kick.
        GetHeldTargets().ForEach(target => target.SetMaskInteraction(SpriteMaskInteraction.None));
        GetKnotwood().RestartMainActionCooldown();
    }

    /// <summary>
    /// Runs logic for the Knotwood model's escaping state. 
    /// </summary>
    protected void ExecuteEscapeState()
    {

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
