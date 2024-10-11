using UnityEngine;

/// <summary>
/// Controls a BasicTreeSeed. <br></br>
/// 
/// The BasicTreeSeedController is responsible for manipulating its BasicTreeSeed and bringing
/// it to life. This includes moving it playing animations, and more.
/// </summary>
/// <![CDATA[<param name="BasicTreeSeedState">]]>
public class BasicTreeSeedController : CollectableController<BasicTreeSeedController.BasicTreeSeedState>    
{
    #region Fields

    /// <summary>
    /// State of a BasicTreeSeed.
    /// </summary>
    public enum BasicTreeSeedState
    {
        SPAWN,
        BOBBING,
        COLLECTING
    }

    #endregion

    #region Methods

    /// <summary>
    /// Assigns a BasicTreeSeed to a BasicTreeSeedController.
    /// </summary>
    /// <param name="basicTreeSeed">The BasicTreeSeed to assign.</param>
    /// <param name="dropPos">Where the BasicTreeSeed first dropped.</param>
    public BasicTreeSeedController(BasicTreeSeed basicTreeSeed, Vector3 dropPos) : base(basicTreeSeed, dropPos)
    {
        GetBasicTreeSeed().SetWorldPosition(dropPos);
        GetBasicTreeSeed().SetValue(1);
    }

    /// <summary>
    /// Main update loop for the BasicTreeSeed.
    /// </summary>
    /// <param name="gameState">The most recent GameState.</param>
    public override void UpdateController(GameState gameState)
    {
        base.UpdateController(gameState);
        ExecuteBobbingState();
        ExecuteCollectingState();
    }

    /// <summary>
    /// Returns this controller's BasicTreeSeed model.
    /// </summary>
    /// <returns>this controller's BasicTreeSeed model.</returns>
    private BasicTreeSeed GetBasicTreeSeed() => GetCollectable() as BasicTreeSeed;

    #endregion

    #region State Logic

    /// <summary>
    /// Returns true if two BasicTreeSeedStates are equal.
    /// </summary>
    /// <param name="stateA">The first state.</param>
    /// <param name="stateB">The second state.</param>
    /// <returns>true if the two BasicTreeSeedStates are equal.</returns>
    public override bool StateEquals(BasicTreeSeedState stateA, BasicTreeSeedState stateB) => stateA == stateB;

    /// <summary>
    /// Updates the state of this BasicTreeSeedController's BasicTreeSeed model.
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
            case BasicTreeSeedState.SPAWN:
                SetState(BasicTreeSeedState.BOBBING);
                break;
            case BasicTreeSeedState.BOBBING:
                if (InHomingRange()) SetState(BasicTreeSeedState.COLLECTING);
                break;
            case BasicTreeSeedState.COLLECTING:
                break;
        }
    }

    /// <summary>
    /// Runs logic for the BasicTreeSeed's bobbing state.
    /// </summary>
    protected virtual void ExecuteBobbingState()
    {
        if (!ValidModel()) return;
        if (GetState() != BasicTreeSeedState.BOBBING) return;

        BobUpAndDown();
    }

    /// <summary>
    /// Runs logic for the BasicTreeSeed's collecting state.
    /// </summary>
    protected virtual void ExecuteCollectingState()
    {
        if (!ValidModel()) return;
        if (GetState() != BasicTreeSeedState.COLLECTING) return;

        if (InCollectionRange())
        {
            EconomyController.CashIn(GetBasicTreeSeed());
            GetBasicTreeSeed().OnCollect();
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
