using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

/// <summary>
/// Controls an Owl. <br></br>
/// 
/// The OwlController is responsible for manipulating its Owl and bringing
/// it to life. This includes moving it, choosing targets, playing animations,
/// and more.
/// </summary>
/// <![CDATA[<param name="OwlState">]]>
public class OwlController : DefenderController<OwlController.OwlState>
{
    #region Fields

    /// <summary>
    /// States of a Owl.
    /// </summary>
    public enum OwlState
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
    /// Maximum number of targets a Owl can have at once.
    /// </summary>
    protected override int MAX_TARGETS => 1;

    /// <summary>
    /// How many seconds to wait between firing ice chunks.
    /// </summary>
    private const float delayBetweenIceChunks = 0.15f;

    #endregion

    #region Methods

    /// <summary>
    /// Creates a OwlController reference.
    /// </summary>
    /// <param name="Owl">The Owl defender.</param>
    /// <returns>a new OwlController reference.</returns>
    public OwlController(Owl owl) : base(owl) { }

    /// <summary>
    /// Main update loop for the Owl.
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
    /// Returns this OwlController's Owl.
    /// </summary>
    /// <returns>this OwlController's Owl.</returns>
    private Owl GetOwl() { return GetMob() as Owl; }

    /// <summary>
    /// Queues an ice chunk to be fired at the Owl's target.
    /// </summary>
    /// <returns>A reference to the coroutine.</returns>
    /// <param name="numIceChunks">The number of ice chunks to fire.</param>
    private IEnumerator FireIceChunks(int numIceChunks)
    {
        Assert.IsTrue(delayBetweenIceChunks >= 0, "Delay needs to be non-negative");

        for (int i = 0; i < numIceChunks; i++)
        {
            Enemy target = GetTarget() as Enemy;
            if (target == null || !target.Targetable()) yield break; // Invalid target.

            SetAnimation(GetOwl().ATTACK_ANIMATION_DURATION / numIceChunks,
                DefenderFactory.GetIdleTrack(
                    ModelType.OWL,
                    GetOwl().GetDirection(), GetOwl().GetTier()));

            GameObject iceChunkPrefab = ProjectileFactory.GetProjectilePrefab(ModelType.ICE_CHUNK);
            Assert.IsNotNull(iceChunkPrefab);
            IceChunk iceChunkComp = iceChunkPrefab.GetComponent<IceChunk>();
            Assert.IsNotNull(iceChunkComp);
            Vector3 targetPosition = GetTarget().GetAttackPosition();
            IceChunkController iceChunkController = new IceChunkController(iceChunkComp, GetOwl().GetPosition(), targetPosition);
            ControllerController.AddModelController(iceChunkController);

            if (i < numIceChunks - 1) // Wait for the delay between shots unless it's the last one
            {
                yield return new WaitForSeconds(delayBetweenIceChunks);
            }
        }
    }

    /// <summary>
    /// Returns true if the Owl can shoot an quill.
    /// </summary>
    /// <returns>true if the Owl can shoot an quill; otherwise,
    /// false.</returns>
    public override bool CanAttack()
    {
        if (!base.CanAttack()) return false; //Cooldown
        if (GetState() != OwlState.ATTACK) return false; //Not in the attack state.
        Enemy target = GetTarget() as Enemy;
        if (target == null || !target.Targetable()) return false; //Invalid target.
        return true;
    }

    #endregion

    #region State Logic

    /// <summary>
    /// Returns true if two OwlStates are equal.
    /// </summary>
    /// <param name="stateA">The first state.</param>
    /// <param name="stateB">The second state.</param>
    /// <returns>true if two OwlStates are equal; otherwise, false.</returns>
    public override bool StateEquals(OwlState stateA, OwlState stateB) => stateA == stateB;

    /// <summary>
    /// Updates the state of this OwlController's Owl model.
    /// The transitions are: <br></br>
    /// 
    /// SPAWN --> IDLE : always <br></br>
    /// </summary>
    public override void UpdateFSM()
    {
        if (!ValidModel()) return;
        if (GetGameState() != GameState.ONGOING)
        {
            SetState(OwlState.IDLE);
            return;
        }

        Enemy target = GetTarget() as Enemy;
        switch (GetState())
        {
            case OwlState.SPAWN:
                if (SpawnStateDone()) SetState(OwlState.IDLE);
                break;
            case OwlState.IDLE:
                if (target == null || !target.Targetable()) break;
                if (DistanceToTargetFromTree() <= GetOwl().GetAttackRange() &&
                    GetOwl().GetAttackCooldown() <= 0) SetState(OwlState.ATTACK);
                break;
            case OwlState.ATTACK:
                if (target == null || !target.Targetable()) SetState(OwlState.IDLE);
                if (GetAnimationCounter() > 0) break;
                if (GetOwl().GetAttackCooldown() > 0) SetState(OwlState.IDLE);
                else if (DistanceToTarget() > GetOwl().GetAttackRange()) SetState(OwlState.IDLE);
                break;
            case OwlState.INVALID:
                throw new System.Exception("Invalid State.");
        }
    }

    /// <summary>
    /// Runs logic relevant to the Owl's spawn state.
    /// </summary>
    protected override void ExecuteSpawnState()
    {
        if (!ValidModel()) return;
        if (GetState() != OwlState.SPAWN) return;

        GetOwl().FaceDirection(Direction.SOUTH);
        base.ExecuteSpawnState();
    }

    /// <summary>
    /// Runs logic relevant to the Owl's idle state.
    /// </summary>
    protected virtual void ExecuteIdleState()
    {
        if (!ValidModel()) return;
        if (GetState() != OwlState.IDLE) return;
        Enemy target = GetTarget() as Enemy;
        if (target != null && DistanceToTargetFromTree() <= GetOwl().GetAttackRange())
            FaceTarget();
        else GetOwl().FaceDirection(Direction.SOUTH);

        SetAnimation(GetOwl().IDLE_ANIMATION_DURATION,
            DefenderFactory.GetIdleTrack(
                ModelType.OWL,
                GetOwl().GetDirection(), GetOwl().GetTier()));
    }

    /// <summary>
    /// Runs logic relevant to the Owl's attacking state.
    /// </summary>
    protected virtual void ExecuteAttackState()
    {
        if (!ValidModel()) return;
        Enemy target = GetTarget() as Enemy;
        if (target == null || !target.Targetable()) return;
        if (GetState() != OwlState.ATTACK) return;

        FaceTarget();
        if (!CanAttack()) return;

        // Calculate the number of quills to fire based on the Owl's tier.
        int tier = GetOwl().GetTier();
        int numQuillsToFire = 1;
        if (tier == 2) numQuillsToFire = 2;
        else if (tier >= 3) numQuillsToFire = 4;
        GetOwl().StartCoroutine(FireIceChunks(numQuillsToFire));

        // Reset attack animation.
        GetOwl().RestartAttackCooldown();
    }

    #endregion

    #region Animation Logic

    /// <summary>
    /// Adds one chunk of Time.deltaTime to the animation
    /// counter that tracks the current state.
    /// </summary>
    public override void AgeAnimationCounter()
    {
        OwlState state = GetState();
        if (state == OwlState.IDLE) idleAnimationCounter += Time.deltaTime;
        else if (state == OwlState.ATTACK) attackAnimationCounter += Time.deltaTime;
    }

    /// <summary>
    /// Returns the animation counter for the current state.
    /// </summary>
    /// <returns>the animation counter for the current state.</returns>
    public override float GetAnimationCounter()
    {
        OwlState state = GetState();
        if (state == OwlState.IDLE) return idleAnimationCounter;
        else if (state == OwlState.ATTACK) return attackAnimationCounter;
        else return 0;
    }

    /// <summary>
    /// Sets the animation counter for the current state to 0.
    /// </summary>
    public override void ResetAnimationCounter()
    {
        OwlState state = GetState();
        if (state == OwlState.IDLE) idleAnimationCounter = 0;
        else if (state == OwlState.ATTACK) attackAnimationCounter = 0;
    }

    #endregion
}
