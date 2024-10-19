using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Controls a Bunny. <br></br>
/// 
/// The BunnyController is responsible for manipulating its Bunny and bringing
/// it to life. This includes moving it, choosing targets, playing animations,
/// and more.
/// </summary>
/// <![CDATA[<param name="OwlState">]]>
public class BunnyController : DefenderController<BunnyController.BunnyState>
{
    #region Fields

    /// <summary>
    /// States of a Bunny.
    /// </summary>
    public enum BunnyState
    {
        SPAWN,
        IDLE,
        GENERATE,
        INVALID
    }

    /// <summary>
    /// Counts the number of seconds in the idle animation; resets
    /// on step.
    /// </summary>
    private float idleAnimationCounter;

    /// <summary>
    /// Counts the number of seconds in the generate animation; resets
    /// on step.
    /// </summary>
    private float generateAnimationCounter;

    /// <summary>
    /// Maximum number of targets a Bunny can have at once.
    /// </summary>
    protected override int MAX_TARGETS => 1;

    #endregion

    #region Methods

    /// <summary>
    /// Creates a BunnyController reference.
    /// </summary>
    /// <param name="Bunny">The Bunny defender.</param>
    /// <returns>a new BunnyController reference.</returns>
    public BunnyController(Bunny Bunny) : base(Bunny) { }

    /// <summary>
    /// Main update loop for the Bunny.
    /// </summary>
    protected override void UpdateMob()
    {
        base.UpdateMob();
        if (!ValidModel()) return;
        if(!GetBunny().IsPlaced()) return;

        ExecuteSpawnState();
        ExecuteIdleState();
        ExecuteGenerateState();
    }

    /// <summary>
    /// Returns this BunnyController's Bunny.
    /// </summary>
    /// <returns>this BunnyController's Bunny.</returns>
    private Bunny GetBunny() { return GetMob() as Bunny; }

    #endregion

    #region State Logic

    /// <summary>
    /// Returns true if two BunnyStates are equal.
    /// </summary>
    /// <param name="stateA">The first state.</param>
    /// <param name="stateB">The second state.</param>
    /// <returns>true if two BunnyStates are equal; otherwise, false.</returns>
    public override bool StateEquals(BunnyState stateA, BunnyState stateB) => stateA == stateB;

    /// <summary>
    /// Updates the state of this BunnyController's Bunny model.
    /// The transitions are: <br></br>
    /// 
    /// SPAWN --> IDLE : always <br></br>
    /// </summary>
    public override void UpdateFSM()
    {
        if (!ValidModel()) return;
        if (GetGameState() != GameState.ONGOING)
        {
            SetState(BunnyState.IDLE);
            return;
        }

        switch (GetState())
        {
            case BunnyState.SPAWN:
                if (SpawnStateDone()) SetState(BunnyState.IDLE);
                break;
            case BunnyState.IDLE:
                if (GetBunny().GetMainActionCooldownRemaining() <= 0) SetState(BunnyState.GENERATE);
                break;
            case BunnyState.GENERATE:
                if (GetAnimationCounter() > 0) break;
                if (GetBunny().GetMainActionCooldownRemaining() > 0) SetState(BunnyState.IDLE);
                break;
            case BunnyState.INVALID:
                throw new System.Exception("Invalid State.");
        }
    }

    /// <summary>
    /// Runs logic relevant to the Bunny's spawn state.
    /// </summary>
    protected override void ExecuteSpawnState()
    {
        if (!ValidModel()) return;
        if (GetState() != BunnyState.SPAWN) return;

        GetBunny().FaceDirection(Direction.SOUTH);
        base.ExecuteSpawnState();
    }

    /// <summary>
    /// Runs logic relevant to the Bunny's idle state.
    /// </summary>
    protected virtual void ExecuteIdleState()
    {
        if (!ValidModel()) return;
        if (GetState() != BunnyState.IDLE) return;

        GetBunny().FaceDirection(Direction.SOUTH);
        SetNextAnimation(GetBunny().IDLE_ANIMATION_DURATION,
            DefenderFactory.GetIdleTrack(
                ModelType.BUNNY,
                GetBunny().GetDirection(), GetBunny().GetTier()));
    }

    /// <summary>
    /// Runs logic relevant to the Bunny's attacking state.
    /// </summary>
    protected virtual void ExecuteGenerateState()
    {
        if (!ValidModel()) return;
        if (GetState() != BunnyState.GENERATE) return;

        GetBunny().FaceDirection(Direction.SOUTH);
        if (!CanPerformMainAction()) return;


        // Generate
        GameObject dewPrefab = CollectableFactory.GetCollectablePrefab(ModelType.DEW);
        Assert.IsNotNull(dewPrefab, "Dew prefab is null.");
        Dew dewComp = dewPrefab.GetComponent<Dew>();
        Assert.IsNotNull(dewComp, "Dew component is null.");

        float spawnRadius = GetBunny().GetMainActionRange(); 
        float angle = Random.Range(0, 2 * Mathf.PI);
        float randomRadius = Random.Range(0f, 1f) * spawnRadius;
        Vector2 randomPositionWithinCircle = new Vector2(
            Mathf.Cos(angle) * randomRadius,
            Mathf.Sin(angle) * randomRadius
        );

        int dewValue;
        if(GetBunny().GetTier() == 1) dewValue = GetBunny().DEW_VALUE_TIER_ONE;
        else if(GetBunny().GetTier() == 2) dewValue = GetBunny().DEW_VALUE_TIER_TWO;
        else dewValue = GetBunny().DEW_VALUE_TIER_THREE;

        Vector3 spawnPosition = GetBunny().GetTreePosition() + new Vector3(randomPositionWithinCircle.x, randomPositionWithinCircle.y, 0);
        DewController dewController = new DewController(dewComp, spawnPosition, dewValue);
        ControllerManager.AddModelController(dewController);

        SetNextAnimation(GetBunny().GENERATION_ANIMATION_DURATION,
            DefenderFactory.GetIdleTrack(
            ModelType.BUNNY, GetBunny().GetDirection(), GetBunny().GetTier()));

        // Reset attack animation.
        GetBunny().RestartMainActionCooldown();
    }

    #endregion

    #region Animation Logic

    /// <summary>
    /// Adds one chunk of Time.deltaTime to the animation
    /// counter that tracks the current state.
    /// </summary>
    public override void AgeAnimationCounter()
    {
        BunnyState state = GetState();
        if (state == BunnyState.IDLE) idleAnimationCounter += Time.deltaTime;
        else if (state == BunnyState.GENERATE) generateAnimationCounter += Time.deltaTime;
    }

    /// <summary>
    /// Returns the animation counter for the current state.
    /// </summary>
    /// <returns>the animation counter for the current state.</returns>
    public override float GetAnimationCounter()
    {
        BunnyState state = GetState();
        if (state == BunnyState.IDLE) return idleAnimationCounter;
        else if (state == BunnyState.GENERATE) return generateAnimationCounter;
        else return 0;
    }

    /// <summary>
    /// Sets the animation counter for the current state to 0.
    /// </summary>
    public override void ResetAnimationCounter()
    {
        BunnyState state = GetState();
        if (state == BunnyState.IDLE) idleAnimationCounter = 0;
        else if (state == BunnyState.GENERATE) generateAnimationCounter = 0;
    }

    #endregion
}
