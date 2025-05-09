using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Controls a Spurge. <br></br>
/// 
/// The SpurgeController is responsible for manipulating its Spurge and bringing
/// it to life. This includes moving it, choosing targets, playing animations,
/// and more.
/// </summary>
/// <![CDATA[<param name="SpurgeState">]]>
public class SpurgeController : EnemyController<SpurgeController.SpurgeState>
{
    #region Fields

    /// <summary>
    /// All possible states of the Spurge.
    /// </summary>
    public enum SpurgeState
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
    /// The maximum number of targets a Spurge can select at once.
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
    /// Makes a new SpurgeController.
    /// </summary>
    /// <param name="Spurge">The Spurge Enemy. </param>
    /// <returns>The created SpurgeController.</returns>
    public SpurgeController(Spurge spurge) : base(spurge) { }

    /// <summary>
    /// Returns this SpurgeController's Spurge model.
    /// </summary>
    /// <returns>this SpurgeController's Spurge model.</returns>
    private Spurge GetSpurge() => GetEnemy() as Spurge;

    /// <summary>
    /// Updates the Spurge model controlled by this SpurgeController.
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
    /// Returns the spawn position of the Spurge when in a SpawnHole.
    /// </summary>
    /// <param name="originalSpawnPos">The position of the SpawnHole it is
    /// spawning from.</param>
    /// <returns> the spawn position of the Spurge when in a SpawnHole.</returns>
    protected override Vector3 SpawnHoleSpawnPos(Vector3 originalSpawnPos)
    {
        originalSpawnPos.y -= 2;
        return originalSpawnPos;
    }

    /// <summary>
    /// Returns true if the current SpurgeState renders the Spurge
    /// immune to damage.
    /// </summary>
    /// <returns>true if the current SpurgeState renders the Spurge
    /// immune to damage; otherwise, false. </returns>
    protected override bool IsCurrentStateImmune()
    {
        SpurgeState state = GetState();
        return state == SpurgeState.INACTIVE ||
               state == SpurgeState.ENTERING ||
               state == SpurgeState.INVALID;
    }

    /// <summary>
    /// Returns true if the Spurge can target the Model passed
    /// into this method.
    /// </summary>
    /// <param name="target">The Model to check for targetability.</param>
    /// <returns>true if the Spurge can target the Model passed
    /// into this method; otherwise, false. </returns>
    protected override bool CanTargetOtherModel(Model target)
    {
        if (!GetSpurge().Spawned()) return false;
        if (GetState() == SpurgeState.ENTERING ||
            GetState() == SpurgeState.INACTIVE) return false;


        SpawnHole nexusHoleTarget = target as SpawnHole;

        // If escaping, only target SpawnHoles.
        if (GetState() == SpurgeState.ESCAPE)
        {
            if (nexusHoleTarget == null) return false;
            if (!nexusHoleTarget.Targetable()) return false;
            if (!GetSpurge().IsExiting() && !TileGrid.CanReach(GetSpurge().GetWorldPosition(), nexusHoleTarget.GetWorldPosition())) return false;
            if (!IsClosestTargetableModelAlongPath(nexusHoleTarget)) return false;

            return true;
        }

        throw new System.Exception("Something wrong happened.");
    }

    /// <summary>
    /// Performs logic right before the Enemy is destroyed.
    /// </summary>
    protected override void OnDestroyModel()
    {
        // Kill any alive SpurgeMinions that belong to this Spurge.
        GetSpurge().KillMinions();
        base.OnDestroyModel();
    }

    #endregion

    #region State Logic

    /// <summary>
    /// Updates the state of this SpurgeController's Spurge model.
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
            SetState(SpurgeState.IDLE);
            return;
        }

        Mob target = GetTarget() as Mob;
        bool targetExists = target != null && target.Targetable();
        bool carrierToProtect = GetNearestCarrier() != null;
        bool withinChaseRange = targetExists && DistanceToTarget() <= GetSpurge().GetChaseRange();
        bool withinAttackRange = targetExists && DistanceToTarget() <= GetSpurge().GetMainActionRange();
        bool holdingTarget = NumTargetsHolding() > 0;

        switch (GetState())
        {
            case SpurgeState.INACTIVE:
                if (GetSpurge().Spawned()) SetState(SpurgeState.ENTERING);
                break;

            case SpurgeState.ENTERING:
                //if (PoppedOutOfHole()) SetState(SpurgeState.IDLE);
                break;

            case SpurgeState.IDLE:
                if (!ReachedMovementTarget()) break;

                if (!targetExists && carrierToProtect) SetState(SpurgeState.PROTECT);
                else if (targetExists && withinChaseRange) SetState(SpurgeState.CHASE);
                else if (holdingTarget) SetState(SpurgeState.ESCAPE);
                break;

            case SpurgeState.CHASE:
                if (!ReachedMovementTarget()) break;

                if (!targetExists) SetState(carrierToProtect ? SpurgeState.PROTECT : SpurgeState.IDLE);
                else if (withinAttackRange) SetState(SpurgeState.ATTACK);
                else if (!withinChaseRange) SetState(SpurgeState.IDLE);
                else if (holdingTarget) SetState(SpurgeState.ESCAPE);
                break;

            case SpurgeState.ATTACK:
                if (!ReachedMovementTarget()) break;

                if (holdingTarget) SetState(SpurgeState.ESCAPE);
                else if (!targetExists) SetState(carrierToProtect ? SpurgeState.PROTECT : SpurgeState.IDLE);
                else if (!withinAttackRange) SetState(withinChaseRange ? SpurgeState.CHASE : SpurgeState.IDLE);

                break;
            case SpurgeState.ESCAPE:
                if (!ReachedMovementTarget()) break;

                if (!targetExists && !holdingTarget) SetState(carrierToProtect ? SpurgeState.PROTECT : SpurgeState.IDLE);
                else if (!holdingTarget) SetState(SpurgeState.IDLE);

                break;

            case SpurgeState.PROTECT:
                if (!ReachedMovementTarget()) break;

                if (!carrierToProtect) SetState(SpurgeState.IDLE);
                else if (targetExists)
                {
                    if (withinAttackRange) SetState(SpurgeState.ATTACK);
                    else if (withinChaseRange) SetState(SpurgeState.CHASE);
                }
                break;

            case SpurgeState.INVALID:
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
    public override bool StateEquals(SpurgeState stateA, SpurgeState stateB) => stateA == stateB;

    /// <summary>
    /// Runs logic for the Spurge model's Inactive state.
    /// </summary>
    protected void ExecuteInactiveState()
    {
        if (GetState() != SpurgeState.INACTIVE) return;

        SetNextMovePos(GetSpurge().GetSpawnWorldPosition());
    }

    /// <summary>
    /// Runs logic for the Spurge model's Spawn state.
    /// </summary>
    protected void ExecuteEnteringState()
    {
        if (GetState() != SpurgeState.ENTERING) return;

        GetSpurge().SetEntering(GetSpurge().GetSpawnWorldPosition());
        SetNextAnimation(GetSpurge().GetMovementAnimationDuration(), EnemyFactory.GetSpawnTrack(
            GetSpurge().TYPE, GetSpurge().Direction, GetSpurge().GetHealthState()));

        PopOutOfMovePos(SpawnHoleSpawnPos(GetSpurge().GetSpawnWorldPosition()));
        GetSpurge().Direction = Direction.SOUTH;

        if (!ReachedMovementTarget()) return;

        // We have fully popped out of the hole.
        // SetPoppedOutOfHole();
    }

    /// <summary>
    /// Runs logic for the Spurge model's idle state.
    /// </summary>
    protected void ExecuteIdleState()
    {
        if (GetState() != SpurgeState.IDLE) return;

        GetSpurge().SetEntered();
        SetNextAnimation(GetSpurge().IDLE_ANIMATION_DURATION, EnemyFactory.GetIdleTrack(
            GetSpurge().TYPE,
            GetSpurge().Direction, GetSpurge().GetHealthState()));

        SetNextMovePos(GetSpurge().GetWorldPosition());
        MoveLinearlyTowardsMovePos();

        GetSpurge().Direction = Direction.SOUTH;
    }

    /// <summary>
    /// Runs logic for the Spurge model's chase state. 
    /// </summary>
    protected void ExecuteChaseState()
    {
        if (GetState() != SpurgeState.CHASE) return;

        SetNextAnimation(GetSpurge().GetMovementAnimationDuration(), EnemyFactory.GetMovementTrack(
            GetSpurge().TYPE,
            GetSpurge().Direction, GetSpurge().GetHealthState()));

        // Move to target.
        MoveLinearlyTowardsMovePos();

        // We reached our move target, so we need a new one.
        if (!ReachedMovementTarget()) return;
        Mob target = GetTarget() as Mob;
        if (target == null || !target.Targetable()) return;

        if (DistanceToTarget() <= GetSpurge().GetMainActionRange()) return;

        Vector3 nextMove = TileGrid.NextTilePosTowardsGoalUsingAStar(GetSpurge().GetWorldPosition(), GetTarget().GetWorldPosition());
        SetNextMovePos(nextMove);
    }

    /// <summary>
    /// Runs logic for the Spurge model's protect state.
    /// </summary>
    protected void ExecuteProtectState()
    {
        if (GetState() != SpurgeState.PROTECT) return;

        SetNextAnimation(GetSpurge().GetMovementAnimationDuration(), EnemyFactory.GetMovementTrack(
            GetSpurge().TYPE,
                       GetSpurge().Direction, GetSpurge().GetHealthState()));

        // Move to target.
        MoveLinearlyTowardsMovePos();

        // We reached our move target, so we need a new one.
        if (!ReachedMovementTarget()) return;
        if (GetNearestCarrier() == null) return;

        Vector3 nextMove = TileGrid.NextTilePosTowardsGoalUsingAStar(GetSpurge().GetWorldPosition(), GetNearestCarrier().GetWorldPosition());
        SetNextMovePos(nextMove);
    }

    /// <summary>
    /// Runs logic for the Spurge model's attack state. 
    /// </summary>
    protected void ExecuteAttackState()
    {
        if (GetState() != SpurgeState.ATTACK) return;

        SetNextAnimation(GetSpurge().GetMainActionAnimationDuration(), EnemyFactory.GetAttackTrack(
                GetSpurge().TYPE,
                       GetSpurge().Direction, GetSpurge().GetHealthState()));

        //Attack Logic : Only if target is valid.
        Mob target = GetTarget() as Mob;
        bool validTarget = GetTarget() != null && target.Targetable();
        if (!CanPerformMainAction()) return;
        if (!validTarget) return;

        FaceTarget();
        GetHeldTargets().ForEach(target => target.SetMaskInteraction(SpriteMaskInteraction.None));
        GetSpurge().RestartMainActionCooldown();
    }

    /// <summary>
    /// Runs logic for the Spurge model's escaping state. 
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
        SpurgeState state = GetState();
        if (state == SpurgeState.ENTERING) spawnAnimationCounter += Time.deltaTime;
        else if (state == SpurgeState.IDLE) idleAnimationCounter += Time.deltaTime;
        else if (state == SpurgeState.CHASE) chaseAnimationCounter += Time.deltaTime;
        else if (state == SpurgeState.ATTACK) attackAnimationCounter += Time.deltaTime;
        else if (state == SpurgeState.PROTECT) protectAnimationCounter += Time.deltaTime;
        else if (state == SpurgeState.ESCAPE) escapeAnimationCounter += Time.deltaTime;
    }

    /// <summary>
    /// Returns the animation counter for the current state.
    /// </summary>
    /// <returns>the animation counter for the current state.</returns>
    public override float GetAnimationCounter()
    {
        SpurgeState state = GetState();
        if (state == SpurgeState.ENTERING) return spawnAnimationCounter;
        else if (state == SpurgeState.IDLE) return idleAnimationCounter;
        else if (state == SpurgeState.CHASE) return chaseAnimationCounter;
        else if (state == SpurgeState.ATTACK) return attackAnimationCounter;
        else if (state == SpurgeState.PROTECT) return protectAnimationCounter;
        else if (state == SpurgeState.ESCAPE) return escapeAnimationCounter;
        else return 0;
    }

    /// <summary>
    /// Sets the animation counter for the current state to 0.
    /// </summary>
    public override void ResetAnimationCounter()
    {
        SpurgeState state = GetState();
        if (state == SpurgeState.ENTERING) spawnAnimationCounter = 0;
        else if (state == SpurgeState.IDLE) idleAnimationCounter = 0;
        else if (state == SpurgeState.CHASE) chaseAnimationCounter = 0;
        else if (state == SpurgeState.ATTACK) attackAnimationCounter = 0;
        else if (state == SpurgeState.PROTECT) protectAnimationCounter = 0;
        else if (state == SpurgeState.ESCAPE) escapeAnimationCounter = 0;
    }

    #endregion
}
