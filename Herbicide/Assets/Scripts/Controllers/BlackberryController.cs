using UnityEngine;

/// <summary>
/// Controls a Blackberry. <br></br>
/// 
/// The BlackberyController is responsible for manipulating its Blackberry and bringing
/// it to life. This includes moving it, choosing targets, playing animations,
/// and more.
/// </summary>
/// <![CDATA[<param name="BlackberryState">]]>
public class BlackberryController : ProjectileController<BlackberryController.BlackberryState>
{
    #region Fields

    /// <summary>
    /// Possible states of an Blackberry over its lifetime.
    /// </summary>
    public enum BlackberryState
    {
        SPAWN,
        MOVING
    }

    /// <summary>
    /// Blackberries do not angle towards their target.
    /// </summary>
    protected override bool angleTowardsTarget => false;

    #endregion

    #region Methods

    /// <summary>
    /// Gives an Blackberry an BlackberryController.
    /// </summary>
    /// <param name="blackberry">The blackberry which will get an BlackberryController.</param>
    /// <param name="destination">Where the blackberry started.</param>
    /// <param name="destination">Where the blackberry should go.</param>
    public BlackberryController(Blackberry blackberry, Vector3 start, Vector3 destination) :
        base(blackberry, start, destination)
    { }

    /// <summary>
    /// Returns the Blackberry model.
    /// </summary>
    /// <returns>the Blackberry model.</returns>
    protected Blackberry GetBlackberry() => GetProjectile() as Blackberry;

    #endregion

    #region State Logic

    /// <summary>
    /// Updates the state of this BlackberryController's Blackberry model.
    /// The transitions are: <br></br>
    /// 
    /// SPAWN --> MOVING : when fired from source
    /// </summary>
    public override void UpdateStateFSM()
    {
        if (!ValidModel()) return;

        switch (GetState())
        {
            case BlackberryState.SPAWN:
                SetState(BlackberryState.MOVING);
                break;
            case BlackberryState.MOVING:
                break;
        }
    }

    /// <summary>
    /// Returns true if two BlackberryStates are equal.
    /// </summary>
    /// <param name="stateA">The first state.</param>
    /// <param name="stateB">The second state.</param>
    /// <returns>true if two BlackberryStates are equal; otherwise, false.</returns>
    public override bool StateEquals(BlackberryState stateA, BlackberryState stateB) => stateA == stateB;

    /// <summary>
    /// Runs logic relevant to the Blackberry's MOVING state.
    /// </summary>
    public override void ExecuteMovingState()
    {
        if (!ValidModel()) return;
        if (GetState() != BlackberryState.MOVING) return;

        SetAnimation(GetBlackberry().MID_AIR_ANIMATION_DURATION, ProjectileFactory.GetMidAirAnimationTrack(GetBlackberry()));
        LinearShot();
    }

    /// <summary>
    /// Runs logic relevant to the Blackberry's COLLIDING state.
    /// </summary>
    public override void ExecuteCollidingState() { }

    /// <summary>
    /// Runs logic relevant to the Blackberry's DEAD state.
    /// </summary>
    public override void ExecuteDeadState() { }

    #endregion

    #region Animation Logic

    /// <summary>
    /// Adds one chunk of Time.deltaTime to the animation
    /// counter that tracks the current state.
    /// </summary>
    public override void AgeAnimationCounter()
    {
        BlackberryState state = GetState();
        if (state == BlackberryState.MOVING) midAirAnimationCounter += Time.deltaTime;
    }

    /// <summary>
    /// Returns the animation counter for the current state.
    /// </summary>
    /// <returns>the animation counter for the current state.</returns>
    public override float GetAnimationCounter()
    {
        BlackberryState state = GetState();
        if (state == BlackberryState.MOVING) return midAirAnimationCounter;
        return 0;
    }

    /// <summary>
    /// Sets the animation counter for the current state to 0.
    /// </summary>
    public override void ResetAnimationCounter()
    {
        BlackberryState state = GetState();
        if (state == BlackberryState.MOVING) midAirAnimationCounter = 0;
    }

    #endregion
}
