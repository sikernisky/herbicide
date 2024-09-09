using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using static UnityEngine.GraphicsBuffer;

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
        IDLE, // Static, nothing to do.
        FOLLOW, // Following its spurge 
        INVALID, // Invalid state.
    }

    /// <summary>
    /// The maximum number of targets a SpurgeMinion can select at once.
    /// </summary>
    protected override int MAX_TARGETS => 1;

    /// <summary>
    /// Counts the number of seconds in the idle animation; resets
    /// on step.
    /// </summary>
    protected float idleAnimationCounter;

    /// <summary>
    /// Counts the number of seconds in the follow animation; resets
    /// on step.
    /// </summary>
    protected float followAnimationCounter;

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
    private SpurgeMinion GetSpurgeMinion() => GetEnemy() as SpurgeMinion;

    /// <summary>
    /// Updates the SpurgeMinion model controlled by this SpurgeMinionController.
    /// </summary>
    protected override void UpdateMob()
    {
        base.UpdateMob();
        ExecuteInactiveState();
        ExecuteIdleState();
        ExecuteFollowState();
    }

    /// <summary>
    /// Spawns a SpurgeMinion. Overrides the SpawnMob() method in EnemyController
    /// because that method sets the position to the spawn position of the enemy
    /// plus some offset of a NexusHole. But SpurgeMinions are directly spawned
    /// at some position.
    /// </summary>
    protected override void SpawnMob()
    {
        if (!GetEnemy().ReadyToSpawn()) return;
        GetSpurgeMinion().SetWorldPosition(GetSpurgeMinion().GetSpawnPos());
        GetSpurgeMinion().RefreshRenderer();
        GetSpurgeMinion().OnSpawn();
        GetSpurgeMinion().gameObject.SetActive(true);
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
    /// Returns true if this controller's SpurgeMinion should be destoyed and
    /// set to null.
    /// </summary>
    /// <returns>true if this controller's SpurgeMinion should be destoyed and
    /// set to null; otherwise, false.</returns>
    public override bool ValidModel()
    {
        if (!GetEnemy().Spawned()) return true;

        HashSet<SpurgeMinionState> immuneStates = new HashSet<SpurgeMinionState>()
        {
            SpurgeMinionState.INACTIVE,
            SpurgeMinionState.INVALID
        };

        bool isImmune = immuneStates.Contains(GetState());
        if (isImmune) return true;
        if (!TileGrid.OnWalkableTile(GetEnemy().GetPosition())) return false;
        else if (GetSpurgeMinion().Dead()) return false;

        return true;
    }

    /// <summary>
    /// Returns true if the SpurgeMinion can target the Model passed
    /// into this method.
    /// </summary>
    /// <param name="target">The Model to check for targetability.</param>
    /// <returns>true if the SpurgeMinion can target the Model passed
    /// into this method; otherwise, false. </returns>
    protected override bool CanTargetOtherModel(Model target) => false;

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

        switch (GetState())
        {
            case SpurgeMinionState.INACTIVE:
                if (GetSpurgeMinion().Spawned()) SetState(SpurgeMinionState.IDLE);
                break;

            case SpurgeMinionState.IDLE:
                
                if(GetGameState() == GameState.ONGOING) SetState(SpurgeMinionState.FOLLOW);
                break;

            case SpurgeMinionState.FOLLOW:
                break;

            case SpurgeMinionState.INVALID:
                break;
            default:
                throw new System.Exception("Invalid state.");
        }
    }

    /// <summary>
    /// Returns true if two SpurgeStates are equal.
    /// </summary>
    /// <param name="stateA">The first SpurgeMinionState</param>
    /// <param name="stateB">The second SpurgeMinionState</param>
    /// <returns>true if two SpurgeStates are equal; otherwise, false. </returns>
    public override bool StateEquals(SpurgeMinionState stateA, SpurgeMinionState stateB) => stateA == stateB;

    /// <summary>
    /// Runs logic for the SpurgeMinion model's Inactive state.
    /// </summary>
    protected void ExecuteInactiveState() { }

    /// <summary>
    /// Runs logic for the SpurgeMinion model's idle state.
    /// </summary>
    protected void ExecuteIdleState()
    {
        if (GetState() != SpurgeMinionState.IDLE) return;

        SetAnimation(GetSpurgeMinion().IDLE_ANIMATION_DURATION, EnemyFactory.GetIdleTrack(
            GetSpurgeMinion().TYPE,
            GetSpurgeMinion().GetDirection(), GetSpurgeMinion().GetHealthState()));

        SetNextMovePos(GetSpurgeMinion().GetPosition());
        MoveLinearlyTowardsMovePos();

        GetSpurgeMinion().FaceDirection(Direction.SOUTH);
    }

    /// <summary>
    /// Runs logic for the SpurgeMinion model's follow state.
    /// </summary>
    protected void ExecuteFollowState()
    {
        if (GetState() != SpurgeMinionState.FOLLOW) return;

        SetAnimation(GetSpurgeMinion().IDLE_ANIMATION_DURATION, EnemyFactory.GetIdleTrack(
            GetSpurgeMinion().TYPE,
            GetSpurgeMinion().GetDirection(), GetSpurgeMinion().GetHealthState()));

/*        MoveLinearlyTowardsMovePos();
        SetNextMovePos(GetSpurgeMinion().GetPositionOfSpurgeFollowing());*/
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
        if (state == SpurgeMinionState.IDLE) idleAnimationCounter += Time.deltaTime;
        else if (state == SpurgeMinionState.FOLLOW) followAnimationCounter += Time.deltaTime;
    }

    /// <summary>
    /// Returns the animation counter for the current state.
    /// </summary>
    /// <returns>the animation counter for the current state.</returns>
    public override float GetAnimationCounter()
    {
        SpurgeMinionState state = GetState();
        if (state == SpurgeMinionState.IDLE) return idleAnimationCounter;
        else if (state == SpurgeMinionState.FOLLOW) return followAnimationCounter;
        return 0;
    }

    /// <summary>
    /// Sets the animation counter for the current state to 0.
    /// </summary>
    public override void ResetAnimationCounter()
    {
        SpurgeMinionState state = GetState();
        if (state == SpurgeMinionState.IDLE) idleAnimationCounter = 0;
        else if (state == SpurgeMinionState.FOLLOW) followAnimationCounter = 0;
    }

    #endregion
}
