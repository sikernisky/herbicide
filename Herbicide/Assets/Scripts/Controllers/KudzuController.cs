using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Controls a Kudzu. <br></br>
/// 
/// The KnotwoodController is responsible for manipulating its Knotwood and bringing
/// it to life. This includes moving it, choosing targets, playing animations,
/// and more.
/// </summary>
/// <![CDATA[<param name="KudzuState">]]>
public class KudzuController : EnemyController<KudzuController.KudzuState>
{
    #region Fields

    /// <summary>
    /// All possible states of the Kudzu.
    /// </summary>
    public enum KudzuState
    {
        INACTIVE, // Not spawned yet.
        ENTERING, // Entering map.
        IDLE, // Static, nothing to do.
        ESCAPE, // Running back to start.
        EXITING // Leaving map.
    }

    /// <summary>
    /// The maximum number of targets a Kudzu can select at once.
    /// </summary>
    protected override int MAX_TARGETS => 1;

    /// <summary>
    /// Counts the number of seconds until the Kudzu can hop again.
    /// </summary>
    private float hopCooldownCounter;

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
    /// Makes a new KudzuController.
    /// </summary>
    /// <param name="kudzu">The Kudzu Enemy. </param>
    /// <returns>The created KudzuController.</returns>
    public KudzuController(Kudzu kudzu) : base(kudzu) { }

    /// <summary>
    /// Returns this KudzuController's Kudzu model.
    /// </summary>
    /// <returns>this KudzuController's Kudzu model.</returns>
    private Kudzu GetKudzu() => GetEnemy() as Kudzu;

    /// <summary>
    /// Updates the Kudzu model controlled by this KudzuController.
    /// </summary>
    protected override void UpdateMob()
    {
        base.UpdateMob();
        ExecuteInactiveState();
        ExecuteEnteringState();
        ExecuteIdleState();
        ExecuteEscapeState();
        ExecuteExitState();
    }

    /// <summary>
    /// Returns the spawn position of the Kudzu when in a SpawnHole.
    /// </summary>
    /// <param name="originalSpawnPos">The position of the SpawnHole it is
    /// spawning from.</param>
    /// <returns> the spawn position of the Kudzu when in a SpawnHole.</returns>
    protected override Vector3 SpawnHoleSpawnPos(Vector3 originalSpawnPos)
    {
        originalSpawnPos.y -= 2;
        return originalSpawnPos;
    }

    /// <summary>
    /// Returns true if the current KudzuState renders the Kudzu
    /// immune to damage.
    /// </summary>
    /// <returns>true if the current KudzuState renders the Kudzu
    /// immune to damage; otherwise, false. </returns>
    protected override bool IsCurrentStateImmune()
    {
        KudzuState state = GetState();
        return state == KudzuState.INACTIVE ||
               state == KudzuState.ENTERING ||
               state == KudzuState.EXITING;
    }

    /// <summary>
    /// Returns true if the Kudzu can target the Model passed
    /// into this method.
    /// </summary>
    /// <param name="target">The Model to check for targetability.</param>
    /// <returns>true if the Kudzu can target the Model passed
    /// into this method; otherwise, false. </returns>
    protected override bool CanTargetOtherModel(Model target)
    {
        if (!GetKudzu().Spawned()) return false;
        if (GetState() == KudzuState.INACTIVE) return false;

        GoalHole goalHoleTarget = target as GoalHole;
        if (goalHoleTarget == null) return false;
        if (!goalHoleTarget.Targetable()) return false;
        if (!TileGrid.CanReach(GetKudzu().GetWorldPosition(), goalHoleTarget.GetWorldPosition())) return false;

        return true;
    }

    #endregion

    #region State Logic

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
    public override void UpdateFSM()
    {
        if (!ValidModel()) return;
        if (GetGameState() != GameState.ONGOING)
        {
            SetState(KudzuState.IDLE);
            return;
        }

        switch (GetState())
        {
            case KudzuState.INACTIVE:
                if (GetKudzu().Spawned()) SetState(KudzuState.ENTERING);
                break;

            case KudzuState.ENTERING:
                if (GetKudzu().IsEntered()) SetState(KudzuState.ESCAPE);
                break;

            case KudzuState.IDLE:
                break;

            case KudzuState.ESCAPE:
                if (!ReachedMovementTarget()) break;
                if (escapeAnimationCounter > 0) break;
                if (Vector2.Distance(GetKudzu().GetWorldPosition(), GetTarget().GetWorldPosition()) < 0.1f) SetState(KudzuState.EXITING);
                break;

            case KudzuState.EXITING:
                break;

            default:
                throw new System.Exception("Invalid state.");
        }
    }

    /// <summary>
    /// Returns true if two KudzuStates are equal.
    /// </summary>
    /// <param name="stateA">The first KudzuState</param>
    /// <param name="stateB">The second KudzuState</param>
    /// <returns>true if two KudzuStates are equal; otherwise, false. </returns>
    public override bool StateEquals(KudzuState stateA, KudzuState stateB) => stateA == stateB;

    /// <summary>
    /// Runs logic for the Kudzu model's Inactive state.
    /// </summary>
    protected void ExecuteInactiveState()
    {
        if (GetState() != KudzuState.INACTIVE) return;

        SetNextMovePos(GetKudzu().GetSpawnWorldPosition());
    }

    /// <summary>
    /// Runs logic for the Kudzu model's Entering state.
    /// </summary>
    protected void ExecuteEnteringState()
    {
        if (GetState() != KudzuState.ENTERING) return;

        GetKudzu().SetEntering(GetKudzu().GetSpawnWorldPosition());

        SetNextAnimation(GetKudzu().GetMovementAnimationDuration(), EnemyFactory.GetSpawnTrack(
            GetKudzu().TYPE, GetKudzu().Direction, GetKudzu().GetHealthState()));

        GetKudzu().Direction = Direction.SOUTH;

        if (!ReachedMovementTarget()) PopOutOfMovePos(SpawnHoleSpawnPos(GetKudzu().GetSpawnWorldPosition()));
        else GetKudzu().SetEntered();
    }

    /// <summary>
    /// Runs logic for the Kudzu model's idle state.
    /// </summary>
    protected void ExecuteIdleState()
    {
        if (GetState() != KudzuState.IDLE) return;

        GetKudzu().SetEntered();

        SetNextAnimation(GetKudzu().IDLE_ANIMATION_DURATION, EnemyFactory.GetIdleTrack(
            GetKudzu().TYPE,
            GetKudzu().Direction, GetKudzu().GetHealthState()));

        SetNextMovePos(GetKudzu().GetWorldPosition());
        MoveLinearlyTowardsMovePos();

        GetKudzu().Direction = Direction.SOUTH;
    }

    /// <summary>
    /// Runs logic for the Kudzu model's escaping state. 
    /// </summary>
    protected void ExecuteEscapeState()
    {
        if (GetState() != KudzuState.ESCAPE) return;
        if (!ValidModel()) return;
        if (GetTarget() == null) return;

        SetNextAnimation(GetKudzu().GetMovementAnimationDuration(), EnemyFactory.GetMovementTrack(
            GetEnemy().TYPE,
                                  GetKudzu().Direction, GetKudzu().GetHealthState()));

        // Move to target.
        if (TileGrid.IsGoalHole(GetNextMovePos())) MoveParabolicallyTowardsMovePos();
        else MoveLinearlyTowardsMovePos();

        // Decrement hop counter.
        hopCooldownCounter -= Time.deltaTime;
        if (hopCooldownCounter > 0) return;

        // We reached our move target, so we need a new one.
        if (!ReachedMovementTarget()) return;
        Vector3 nextWaypointPos = TileGrid.GetNextWaypointWorldPosition(GetPathId(), GetCurrentWaypointIndex());
        SetNextMovePos(TileGrid.NextTilePosTowardsGoalUsingPathfindingDistance(GetKudzu().GetWorldPosition(), nextWaypointPos, GetKudzu().Direction));
        hopCooldownCounter = GetKudzu().GetHopCooldown();
    }

    /// <summary>
    /// Runs logic for the Kudzu's Exit state.
    /// </summary>
    protected void ExecuteExitState()
    {
        if (GetState() != KudzuState.EXITING) return;
        if (GetKudzu().IsEscaped()) return;
        Assert.IsTrue(GetTarget() as GoalHole != null, "exit target needs to be a GoalHole.");

        Vector3 jumpDestination = GetTarget().GetWorldPosition();
        jumpDestination.y -= 2;

        if (!GetKudzu().IsExiting() && !GetKudzu().IsEscaped()) GetKudzu().SetExiting(jumpDestination);

        SetNextAnimation(GetKudzu().GetMovementAnimationDuration(), EnemyFactory.GetMovementTrack(
            GetKudzu().TYPE,
                       GetKudzu().Direction, GetKudzu().GetHealthState()));

        // Move to target.
        SetNextMovePos(jumpDestination);
        FallIntoMovePos(3f);
        if (Vector2.Distance(GetKudzu().GetWorldPosition(), jumpDestination) < 0.1f) GetKudzu().SetEscaped();
    }

    #endregion

    #region Animation Logic

    /// <summary>
    /// Adds one chunk of Time.deltaTime to the animation
    /// counter that tracks the current state.
    /// </summary>
    public override void AgeAnimationCounter()
    {
        KudzuState state = GetState();
        if (state == KudzuState.ENTERING) spawnAnimationCounter += Time.deltaTime;
        else if (state == KudzuState.IDLE) idleAnimationCounter += Time.deltaTime;
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
        if (state == KudzuState.ENTERING) return spawnAnimationCounter;
        else if (state == KudzuState.IDLE) return idleAnimationCounter;
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
        if (state == KudzuState.ENTERING) spawnAnimationCounter = 0;
        else if (state == KudzuState.IDLE) idleAnimationCounter = 0;
        else if (state == KudzuState.ESCAPE) escapeAnimationCounter = 0;
        else if (state == KudzuState.EXITING) exitingAnimationCounter = 0;
    }

    #endregion
}
