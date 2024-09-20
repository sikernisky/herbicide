/// <summary>
/// Controls a SpeedTree. <br></br>
/// 
/// The SpeedTreeController is responsible for manipulating its SpeedTree and bringing
/// it to life. This includes moving it, choosing targets, playing animations,
/// and more.
/// </summary>
/// <![CDATA[<param name="SpeedTreeState">]]>
public class SpeedTreeController : TreeController<SpeedTreeController.SpeedTreeState>
{
    #region Fields 

    /// <summary>
    /// State of a SpeedTree.
    /// </summary>
    public enum SpeedTreeState
    {
        SPAWN,
        IDLE,
        INVALID
    }

    #endregion

    #region Methods

    /// <summary>
    /// Makes a new SpeedTreeController.
    /// </summary>
    /// <param name="speedTree">The SpeedTree model.</param>
    public SpeedTreeController(SpeedTree speedTree) : base(speedTree) { }

    /// <summary>
    /// Main update loop for the SpeedTree.
    /// </summary>
    protected override void UpdateMob()
    {
        base.UpdateMob();
        if (!ValidModel()) return;

        ExecuteIdleState();
    }

    /// <summary>
    /// Returns the SpeedTree model.
    /// </summary>
    /// <returns>the SpeedTree model.</returns>
    protected SpeedTree GetSpeedTree() => GetTree() as SpeedTree;

    /// <summary>
    /// Returns the SpeedTree prefab to the SpeedTreeFactory singleton.
    /// </summary>
    public override void ReturnModelToFactory() => TreeFactory.ReturnTreePrefab(GetSpeedTree().gameObject);

    #endregion

    #region State Logic

    /// <summary>
    /// Returns true if two SpeedTreeStates are equal.
    /// </summary>
    /// <param name="stateA">The first state.</param>
    /// <param name="stateB">The second state.</param>
    /// <returns>true if two SpeedTreeStates are equal; otherwise, false.</returns>
    public override bool StateEquals(SpeedTreeState stateA, SpeedTreeState stateB) => stateA == stateB;

    /// <summary>
    /// Updates the state of this SpeedTreeController's Tree model.
    /// The transitions are: <br></br>
    /// 
    /// SPAWN --> IDLE : always <br></br>
    /// </summary>
    public override void UpdateFSM()
    {
        switch (GetState())
        {
            case SpeedTreeState.SPAWN:
                SetState(SpeedTreeState.IDLE);
                break;
            case SpeedTreeState.IDLE:
                break;
            case SpeedTreeState.INVALID:
                throw new System.Exception("Invalid State.");
        }
    }

    /// <summary>
    /// Runs logic relevant to the SpeedTree's IDLE state.
    /// </summary>
    protected virtual void ExecuteIdleState()
    {
        if (GetState() != SpeedTreeState.IDLE) return;
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
