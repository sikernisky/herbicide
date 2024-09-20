using UnityEngine;

/// <summary>
/// Controls a SpeedTreeSeed. <br></br>
/// 
/// The SpeedTreeSeedController is responsible for manipulating its SpeedTreeSeed and bringing
/// it to life. This includes moving it playing animations, and more.
/// </summary>
/// <![CDATA[<param name="SpeedTreeSeedState">]]>
public class SpeedTreeSeedController : CollectableController<SpeedTreeSeedController.SpeedTreeSeedState>
{
    #region Fields

    /// <summary>
    /// State of a SpeedTreeSeed.
    /// </summary>
    public enum SpeedTreeSeedState
    {
        SPAWN,
        BOBBING,
        COLLECTING
    }

    #endregion

    #region Methods

    /// <summary>
    /// Assigns a SpeedTreeSeed to a SpeedTreeSeedController.
    /// </summary>
    /// <param name="speedTreeSeed">The SpeedTreeSeed to assign.</param>
    /// <param name="dropPos">Where the SpeedTreeSeed first dropped.</param>
    public SpeedTreeSeedController(SpeedTreeSeed speedTreeSeed, Vector3 dropPos) : base(speedTreeSeed, dropPos)
    {
        GetSpeedTreeSeed().SetWorldPosition(dropPos);
        GetSpeedTreeSeed().AdjustValue(1);
    }

    /// <summary>
    /// Main update loop for the SpeedTreeSeed.
    /// </summary>
    /// <param name="gameState">The most recent GameState.</param>
    public override void UpdateController(GameState gameState)
    {
        base.UpdateController(gameState);
        ExecuteBobbingState();
        ExecuteCollectingState();
    }

    /// <summary>
    /// Returns this controller's SpeedTreeSeed model.
    /// </summary>
    /// <returns>this controller's SpeedTreeSeed model.</returns>
    private SpeedTreeSeed GetSpeedTreeSeed() => GetCollectable() as SpeedTreeSeed;

    #endregion

    #region State Logic

    /// <summary>
    /// Returns true if two SpeedTreeSeedStates are equal.
    /// </summary>
    /// <param name="stateA">The first state.</param>
    /// <param name="stateB">The second state.</param>
    /// <returns>true if the two SpeedTreeSeedStates are equal.</returns>
    public override bool StateEquals(SpeedTreeSeedState stateA, SpeedTreeSeedState stateB) => stateA == stateB;

    /// <summary>
    /// Updates the state of this SpeedTreeSeedController's SpeedTreeSeed model.
    /// The transitions are: <br></br>
    /// 
    /// SPAWN --> BOBBING : when dropped from source <br></br>
    /// BOBBING --> COLLECTING : when being collected <br></br>
    /// COLLECTING --> DEAD : when collected. <br></br>
    /// </summary>
    public override void UpdateFSM()
    {
        switch (GetState())
        {
            case SpeedTreeSeedState.SPAWN:
                SetState(SpeedTreeSeedState.BOBBING);
                break;
            case SpeedTreeSeedState.BOBBING:
                if (InHomingRange()) SetState(SpeedTreeSeedState.COLLECTING);
                break;
            case SpeedTreeSeedState.COLLECTING:
                break;
        }
    }

    /// <summary>
    /// Runs logic for the SpeedTreeSeed's bobbing state.
    /// </summary>
    protected virtual void ExecuteBobbingState()
    {
        if (!ValidModel()) return;
        if (GetState() != SpeedTreeSeedState.BOBBING) return;

        BobUpAndDown();
    }

    /// <summary>
    /// Runs logic for the SpeedTreeSeed's collecting state.
    /// </summary>
    protected virtual void ExecuteCollectingState()
    {
        if (!ValidModel()) return;
        if (GetState() != SpeedTreeSeedState.COLLECTING) return;

        if (InCollectionRange())
        {
            EconomyController.CashIn(GetSpeedTreeSeed());
            GetSpeedTreeSeed().OnCollect();
        }
        else MoveTowardsCursor();
    }

    #endregion

    #region Animation Logic

    /// <summary>
    /// Adds one chunk of Time.deltaTime to the animation
    /// counter that tracks the current state.
    /// </summary>
    public override void AgeAnimationCounter() => throw new System.NotImplementedException();

    /// <summary>
    /// Returns the animation counter for the current state.
    /// </summary>
    /// <returns>the animation counter for the current state.</returns>
    public override float GetAnimationCounter() => throw new System.NotImplementedException();
    /// <summary>
    /// Sets the animation counter for the current state to 0.
    /// </summary>
    public override void ResetAnimationCounter() => throw new System.NotImplementedException();

    #endregion
}
