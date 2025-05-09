using UnityEngine;

/// <summary>
/// Controls a Dew. <br></br>
/// 
/// The DewController is responsible for manipulating its Dew and bringing
/// it to life. This includes moving it playing animations, and more.
/// </summary>
/// <![CDATA[<param name="DewState">]]>
public class DewController : CollectableController<DewController.DewState>
{
    #region Fields

    /// <summary>
    /// State of a Dew.
    /// </summary>
    public enum DewState
    {
        SPAWN,
        BOBBING,
        COLLECTING
    }

    #endregion

    #region Methods

    /// <summary>
    /// Assigns a Dew to a DewController.
    /// </summary>
    /// <param name="dew">The Dew to assign.</param>
    /// <param name="worldSpawnPosition">Where the Dew first dropped.</param>
    public DewController(Dew dew, Vector3 worldSpawnPosition) : base(dew, worldSpawnPosition)
    {
        GetDew().SetWorldPosition(worldSpawnPosition);
    }

    /// <summary>
    /// Main update loop for the Dew.
    /// </summary>
    /// <param name="gameState">The most recent GameState.</param>
    public override void UpdateController(GameState gameState)
    {
        base.UpdateController(gameState);
        ExecuteBobbingState();
        ExecuteCollectingState();
    }

    /// <summary>
    /// Returns this controller's Dew model.
    /// </summary>
    /// <returns>this controller's Dew model.</returns>
    private Dew GetDew() => GetCollectable() as Dew;

    #endregion

    #region State Logic

    /// <summary>
    /// Returns true if two DewStates are equal.
    /// </summary>
    /// <param name="stateA">The first state.</param>
    /// <param name="stateB">The second state.</param>
    /// <returns>true if the two DewStates are equal.</returns>
    public override bool StateEquals(DewState stateA, DewState stateB) => stateA == stateB;

    /// <summary>
    /// Updates the state of this DewController's Dew model.
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
            case DewState.SPAWN:
                SetState(DewState.BOBBING);
                break;
            case DewState.BOBBING:
                if (InHomingRange()) SetState(DewState.COLLECTING);
                break;
            case DewState.COLLECTING:
                break;
        }
    }

    /// <summary>
    /// Runs logic for the Dew's bobbing state.
    /// </summary>
    protected virtual void ExecuteBobbingState()
    {
        if (!ValidModel()) return;
        if (GetState() != DewState.BOBBING) return;

        BobUpAndDown();
    }

    /// <summary>
    /// Runs logic for the Dew's collecting state.
    /// </summary>
    protected virtual void ExecuteCollectingState()
    {
        if (!ValidModel()) return;
        if (GetState() != DewState.COLLECTING) return;

        if (InCollectionRange())
        {
            EconomyController.CashIn(GetDew());
            GetDew().OnCollect();
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
