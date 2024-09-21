/// <summary>
/// Controls a BasicTree. <br></br>
/// 
/// The BasicTreeController is responsible for manipulating its BasicTree and bringing
/// it to life. This includes moving it, choosing targets, playing animations,
/// and more.
/// </summary>
/// <![CDATA[<param name="BasicTreeState">]]>
public class BasicTreeController : TreeController<BasicTreeController.BasicTreeState>
{
    #region Fields 
    
    /// <summary>
    /// State of a BasicTree.
    /// </summary>
    public enum BasicTreeState
    {
        SPAWN,
        IDLE,
        INVALID
    }

    #endregion

    #region Methods

    /// <summary>
    /// Makes a new BasicTreeController.
    /// </summary>
    /// <param name="basicTree">The BasicTree model.</param>
    public BasicTreeController(BasicTree basicTree) : base(basicTree) { }

    /// <summary>
    /// Main update loop for the BasicTree.
    /// </summary>
    protected override void UpdateMob()
    {
        base.UpdateMob();
        if (!ValidModel()) return;

        ExecuteIdleState();
    }

    /// <summary>
    /// Returns the BasicTree model.
    /// </summary>
    /// <returns>the BasicTree model.</returns>
    protected BasicTree GetBasicTree() => GetTree() as BasicTree;

    /// <summary>
    /// Returns the BasicTree prefab to the BasicTreeFactory singleton.
    /// </summary>
    public override void ReturnModelToFactory() => TreeFactory.ReturnTreePrefab(GetBasicTree().gameObject);

    #endregion

    #region State Logic

    /// <summary>
    /// Returns true if two BasicTreeStates are equal.
    /// </summary>
    /// <param name="stateA">The first state.</param>
    /// <param name="stateB">The second state.</param>
    /// <returns>true if two BasicTreeStates are equal; otherwise, false.</returns>
    public override bool StateEquals(BasicTreeState stateA, BasicTreeState stateB) => stateA == stateB;

    /// <summary>
    /// Updates the state of this BasicTreeController's Tree model.
    /// The transitions are: <br></br>
    /// 
    /// SPAWN --> IDLE : always <br></br>
    /// </summary>
    public override void UpdateFSM()
    {
        switch (GetState())
        {
            case BasicTreeState.SPAWN:
                SetState(BasicTreeState.IDLE);
                break;
            case BasicTreeState.IDLE:
                break;
            case BasicTreeState.INVALID:
                throw new System.Exception("Invalid State.");
        }
    }

    /// <summary>
    /// Runs logic relevant to the BasicTree's IDLE state.
    /// </summary>
    protected virtual void ExecuteIdleState()
    {
        if (GetState() != BasicTreeState.IDLE) return;
        if (!ValidModel()) return;
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
