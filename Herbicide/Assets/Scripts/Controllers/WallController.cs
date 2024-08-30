using UnityEngine;

/// <summary>
/// Controls a Wall.
/// 
/// The WallController is responsible for manipulating its Wall and bringing
/// it to life. This includes moving it, choosing targets, playing animations,
/// and more.
/// </summary>
/// <![CDATA[<param name="WallState">]]>]]>
public class WallController : MobController<WallController.WallState>
{
    #region Fields

    /// <summary>
    /// Different states of a Wall.
    /// /// </summary>
    public enum WallState
    {
        SPAWN,
        IDLE
    }

    /// <summary>
    /// Maximum number of targets a Wall is allowed to have.
    /// </summary>
    protected override int MAX_TARGETS => 0;

    #endregion

    #region Methods

    /// <summary>
    /// Assigns a Wall to this WallController.
    /// </summary>
    /// <param name="wall"></param>The Wall to assign. <summary>
    public WallController(Wall wall) : base(wall) { }

    /// <summary>
    /// Returns this WallController's Wall.
    /// </summary>
    /// <returns>this WallController's Wall.</returns>
    protected Wall GetWall() => GetMob() as Wall;

    /// <summary>
    /// Returns true if the Wall should be removed.
    /// </summary>
    /// <returns>true if the Wall should be removed; otherwise, false.</returns>
    public override bool ValidModel() => true;

    /// <summary>
    /// Returns true if the Wall can target the Model passed into this method.
    /// </summary>
    /// <param name="target">The Model to check. </param>
    /// <returns>true if the Wall can target the Model passed into this method;
    /// otherwise, false. </returns>
    protected override bool CanTargetModel(Model target) => false;

    /// <summary>
    /// Returns the Wall prefab to the WallFactory singleton.
    /// </summary>
    public override void DestroyModel() => WallFactory.ReturnWallPrefab(GetWall().gameObject);

    #endregion

    #region State Logic

    /// <summary>
    /// Returns true if two WallStates are equal.
    /// </summary>
    /// <param name="stateA">The first WallState.</param>
    /// <param name="stateB">The second WallState.</param>
    /// <returns>true if the two WallStates are equal.</returns>
    public override bool StateEquals(WallState stateA, WallState stateB) => stateA == stateB;

    /// <summary>
    /// Updates the state of the Wall. The transitions are: <br></br>
    /// 
    /// SPAWN --> IDLE : always
    /// </summary>
    public override void UpdateStateFSM()
    {
        if (!ValidModel()) return;

        switch (GetState())
        {
            case WallState.SPAWN:
                SetState(WallState.IDLE);
                break;
            case WallState.IDLE:
                break;
        }
    }

    #endregion

    #region Animation Logic

    /// <summary>
    /// Adds one chunk of Time.deltaTime to the animation
    /// counter that tracks the current state.
    /// </summary>
    public override void AgeAnimationCounter() { throw new System.NotImplementedException(); }

    /// <summary>
    /// Returns the animation counter for the current state.
    /// </summary>
    /// <returns>the animation counter for the current state.</returns>
    public override float GetAnimationCounter() { throw new System.NotImplementedException(); }

    /// <summary>
    /// Sets the animation counter for the current state to 0.
    /// </summary>
    public override void ResetAnimationCounter() { throw new System.NotImplementedException(); }

    #endregion
}
