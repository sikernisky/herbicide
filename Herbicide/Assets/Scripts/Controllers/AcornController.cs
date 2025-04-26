using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Controls an Acorn. <br></br>
/// 
/// The AcornController is responsible for manipulating its Acorn and bringing
/// it to life. This includes moving it, choosing targets, playing animations,
/// and more.
/// </summary>
/// <![CDATA[<param name="AcornState">]]>
public class AcornController : ProjectileController<AcornController.AcornState>
{
    #region Fields

    /// <summary>
    /// Possible states of an Acorn over its lifetime.
    /// </summary>
    public enum AcornState
    {
        SPAWN,
        MOVING
    }

    /// <summary>
    /// Acorns do not angle towards their target.
    /// </summary>
    protected override bool ShouldAngleTowardsTarget => false;

    #endregion

    #region Methods

    /// <summary>
    /// Gives an Acorn an AcornController.
    /// </summary>
    /// <param name="acorn">The acorn which will get an AcornController.</param>
    /// <param name="start">Where the acorn started.</param>
    /// <param name="destination">Where the acorn should go.</param>
    public AcornController(Acorn acorn, Vector3 start, Vector3 destination) : base(acorn, start, destination) { }

    /// <summary>
    /// Updates the state of this AcornController's Acorn model.
    /// The transitions are: <br></br>
    /// 
    /// SPAWN --> MOVING : when fired from source
    /// </summary>
    public override void UpdateFSM()
    {
        if (!ValidModel()) return;

        switch (GetState())
        {
            case AcornState.SPAWN:
                SetState(AcornState.MOVING);
                break;
            case AcornState.MOVING:
                break;
        }
    }

    /// <summary>
    /// Returns true if two AcornStates are equal.
    /// </summary>
    /// <param name="stateA">The first state.</param>
    /// <param name="stateB">The second state.</param>
    /// <returns>true if two AcornStates are equal; otherwise, false.</returns>
    public override bool StateEquals(AcornState stateA, AcornState stateB) => stateA == stateB;

    /// <summary>
    /// Runs logic relevant to the Acorn's MOVING state. Called each
    /// frame the Acorn is in the MOVING state.
    /// </summary>
    public override void ExecuteMovingState()
    {
        if (!ValidModel()) return;
        if (GetState() != AcornState.MOVING) return;
        SetNextAnimation(GetProjectile().MidAirAnimationDuration, ProjectileFactory.GetMidAirAnimationTrack(GetProjectile().TYPE));
        LinearShot();
    }

    #endregion

    #region Animation Logic

    /// <summary>
    /// Adds one chunk of Time.deltaTime to the animation
    /// counter that tracks the current state.
    /// </summary>
    public override void AgeAnimationCounter()
    {
        AcornState state = GetState();
        if (state == AcornState.MOVING) MidAirAnimationCounter += Time.deltaTime;
    }

    /// <summary>
    /// Returns the animation counter for the current state.
    /// </summary>
    /// <returns>the animation counter for the current state.</returns>
    public override float GetAnimationCounter()
    {
        AcornState state = GetState();
        if (state == AcornState.MOVING) return MidAirAnimationCounter;
        return 0;
    }

    /// <summary>
    /// Sets the animation counter for the current state to 0.
    /// </summary>
    public override void ResetAnimationCounter()
    {
        AcornState state = GetState();
        if (state == AcornState.MOVING) MidAirAnimationCounter = 0;
    }

    #endregion
}
