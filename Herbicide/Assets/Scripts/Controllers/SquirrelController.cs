using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Controls a Squirrel. <br></br>
/// 
/// The SquirrelController is responsible for manipulating its Squirrel and bringing
/// it to life. This includes moving it, choosing targets, playing animations,
/// and more.
/// </summary>
/// <![CDATA[<param name="SquirrelState">]]>
public class SquirrelController : DefenderController<SquirrelController.SquirrelState>
{
    #region Fields

    /// <summary>
    /// State of a Squirrel. 
    /// </summary>
    public enum SquirrelState
    {
        SPAWN,
        IDLE,
        ATTACK,
        INVALID
    }

    /// <summary>
    /// The maximum number of targets a Squirrel can select at once.
    /// </summary>
    protected override int MAX_TARGETS => 1;

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
    /// How many seconds to wait between firing acorns.
    /// </summary>
    private const float delayBetweenAcorns = 0.1f;

    #endregion

    #region Methods

    /// <summary>
    /// Makes a new SquirrelController.
    /// </summary>
    /// <param name="defender">The Squirrel Defender. </param>
    /// <param name="tier">The tier of the Squirrel.</param>
    /// <returns>The created SquirrelController.</returns>
    public SquirrelController(Squirrel squirrel, int tier) : base(squirrel, tier) { }

    /// <summary>
    /// Main update loop for the Squirrel.
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
    /// Returns this SquirrelController's Squirrel.
    /// </summary>
    /// <returns>this SquirrelController's Squirrel.</returns>
    private Squirrel GetSquirrel() => GetMob() as Squirrel;

    /// <summary>
    /// Returns true if the Squirrel can shoot an acorn.
    /// </summary>
    /// <returns>true if the Squirrel can shoot an acorn; otherwise,
    /// false.</returns>
    public override bool CanPerformMainAction()
    {
        if (!base.CanPerformMainAction()) return false; //Cooldown
        if (GetState() != SquirrelState.ATTACK) return false; //Not in the attack state.
        Enemy target = GetTarget() as Enemy;
        if (target == null || !target.Targetable()) return false; //Invalid target.
        return true;
    }

    #endregion

    #region State Logic

    /// <summary>
    /// Returns true if two SquirrelStates are equal.
    /// </summary>
    /// <param name="stateA">The first state.</param>
    /// <param name="stateB">The second state.</param>
    /// <returns>true if two SquirrelStates are equal; otherwise, false.</returns>
    public override bool StateEquals(SquirrelState stateA, SquirrelState stateB) => stateA == stateB;

    /// <summary>
    /// Updates the state of this SquirrelController's Squirrel model.
    /// The transitions are: <br></br>
    /// 
    /// SPAWN --> IDLE : always <br></br>
    /// IDLE --> ATTACK : if target in range <br></br>
    /// ATTACK --> IDLE : if no target in range
    /// </summary>
    public override void UpdateFSM()
    {
        if (!ValidModel()) return;
        if (GetGameState() != GameState.ONGOING)
        {
            SetState(SquirrelState.IDLE);
            return;
        }

        Enemy target = GetTarget() as Enemy;
        switch (GetState())
        {
            case SquirrelState.SPAWN:
                if (SpawnStateDone()) SetState(SquirrelState.IDLE);
                break;
            case SquirrelState.IDLE:
                if (target == null || !target.Targetable()) break;
                if (GetTarget() != null &&
                    GetSquirrel().GetMainActionCooldownRemaining() <= 0) SetState(SquirrelState.ATTACK);
                break;
            case SquirrelState.ATTACK:
                if (target == null || !target.Targetable()) SetState(SquirrelState.IDLE);
                if (GetAnimationCounter() > 0) break;
                if (GetSquirrel().GetMainActionCooldownRemaining() > 0) SetState(SquirrelState.IDLE);
                else if (GetTarget() == null) SetState(SquirrelState.IDLE);
                break;
            case SquirrelState.INVALID:
                throw new System.Exception("Invalid State.");
        }
    }

    /// <summary>
    /// Runs logic relevant to the Squirrel's spawn state.
    /// </summary>
    protected override void ExecuteSpawnState()
    {
        if (!ValidModel()) return;
        if (GetState() != SquirrelState.SPAWN) return;

        GetSquirrel().Direction = Direction.SOUTH;
        base.ExecuteSpawnState();
    }

    /// <summary>
    /// Runs logic relevant to the Squirrel's idle state.
    /// </summary>
    protected virtual void ExecuteIdleState()
    {
        if (!ValidModel()) return;
        if (GetState() != SquirrelState.IDLE) return;
        Enemy target = GetTarget() as Enemy;
        if (target != null) FaceTarget();
        else GetSquirrel().Direction = Direction.SOUTH;

        SetNextAnimation(GetSquirrel().IDLE_ANIMATION_DURATION,
            DefenderFactory.GetIdleTrack(
                ModelType.SQUIRREL,
                GetSquirrel().Direction, GetSquirrel().GetTier()));
    }

    /// <summary>
    /// Runs logic relevant to the Squirrel's attacking state.
    /// </summary>
    protected virtual void ExecuteAttackState()
    {
        if (!ValidModel()) return;
        Enemy target = GetTarget() as Enemy;
        if (target == null || !target.Targetable()) return;
        if (GetState() != SquirrelState.ATTACK) return;

        FaceTarget();
        if (!CanPerformMainAction()) return;

        FireProjectileFromModel(ModelType.ACORN, GetTarget().GetAttackPosition(), false);

        // Reset attack animation.
        GetSquirrel().RestartMainActionCooldown();
    }

    #endregion

    #region Animation Logic

    /// <summary>
    /// Adds one chunk of Time.deltaTime to the animation
    /// counter that tracks the current state.
    /// </summary>
    public override void AgeAnimationCounter()
    {
        SquirrelState state = GetState();
        if (state == SquirrelState.IDLE) idleAnimationCounter += Time.deltaTime;
        else if (state == SquirrelState.ATTACK) attackAnimationCounter += Time.deltaTime;
    }

    /// <summary>
    /// Returns the animation counter for the current state.
    /// </summary>
    /// <returns>the animation counter for the current state.</returns>
    public override float GetAnimationCounter()
    {
        SquirrelState state = GetState();
        if (state == SquirrelState.IDLE) return idleAnimationCounter;
        else if (state == SquirrelState.ATTACK) return attackAnimationCounter;
        else return 0;
    }

    /// <summary>
    /// Sets the animation counter for the current state to 0.
    /// </summary>
    public override void ResetAnimationCounter()
    {
        SquirrelState state = GetState();
        if (state == SquirrelState.IDLE) idleAnimationCounter = 0;
        else if (state == SquirrelState.ATTACK) attackAnimationCounter = 0;
    }

    #endregion
}
