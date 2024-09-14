using UnityEngine;

/// <summary>
/// Controls an IceChunk. <br></br>
/// 
/// The IceChunkController is responsible for manipulating its IceChunk and bringing
/// it to life. This includes moving it, choosing targets, playing animations,
/// and more.
/// </summary>
/// <![CDATA[<param name="IceChunkState">]]>
public class IceChunkController : ProjectileController<IceChunkController.IceChunkState>
{
    #region Fields

    /// <summary>
    /// Possible states of an IceChunk over its lifetime.
    /// </summary>
    public enum IceChunkState
    {
        SPAWN,
        MOVING
    }

    /// <summary>
    /// IceChunks do not angle towards their target.
    /// </summary>
    protected override bool angleTowardsTarget => false;

    #endregion

    #region Methods

    /// <summary>
    /// Gives an IceChunk an IceChunkController.
    /// </summary>
    /// <param name="iceChunk">The IceChunk which will get an IceChunkController.</param>
    /// <param name="start">Where the IceChunk started.</param>
    /// <param name="destination">Where the IceChunk should go.</param>
    /// <param name="numSplits">How many times the IceChunk will split.</param>
    public IceChunkController(IceChunk iceChunk, Vector3 start, Vector3 destination) :
        base(iceChunk, start, destination) { }

    /// <summary>
    /// Returns the IceChunk model.
    /// </summary>
    /// <returns>the IceChunk model.</returns>
    protected IceChunk GetIceChunk() => GetProjectile() as IceChunk;

    /// <summary>
    /// Processes events that occur when the IceChunk detonates at a given position.
    /// </summary>
    /// <param name="other">Collider2D the projectile collided with.</param>
    protected override void DetonateProjectile(Collider2D other)
    {
        base.DetonateProjectile(other);
    }

    #endregion

    #region State Logic

    /// <summary>
    /// Updates the state of this IceChunkController's IceChunk model.
    /// The transitions are: <br></br>
    /// 
    /// SPAWN --> MOVING : when fired from source
    /// </summary>
    public override void UpdateFSM()
    {
        if (!ValidModel()) return;

        switch (GetState())
        {
            case IceChunkState.SPAWN:
                SetState(IceChunkState.MOVING);
                break;
            case IceChunkState.MOVING:
                break;
        }
    }

    /// <summary>
    /// Returns true if two IceChunkStates are equal.
    /// </summary>
    /// <param name="stateA">The first state.</param>
    /// <param name="stateB">The second state.</param>
    /// <returns>true if two IceChunkStates are equal; otherwise, false.</returns>
    public override bool StateEquals(IceChunkState stateA, IceChunkState stateB) => stateA == stateB;

    /// <summary>
    /// Runs logic relevant to the IceChunk's MOVING state. Called each
    /// frame the IceChunk is in the MOVING state.
    /// </summary>
    public override void ExecuteMovingState()
    {
        if (!ValidModel()) return;
        if (GetState() != IceChunkState.MOVING) return;

        SetAnimation(GetIceChunk().MID_AIR_ANIMATION_DURATION, ProjectileFactory.GetMidAirAnimationTrack(GetIceChunk()));
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
        IceChunkState state = GetState();
        if (state == IceChunkState.MOVING) midAirAnimationCounter += Time.deltaTime;
    }

    /// <summary>
    /// Returns the animation counter for the current state.
    /// </summary>
    /// <returns>the animation counter for the current state.</returns>
    public override float GetAnimationCounter()
    {
        IceChunkState state = GetState();
        if (state == IceChunkState.MOVING) return midAirAnimationCounter;
        return 0;
    }

    /// <summary>
    /// Sets the animation counter for the current state to 0.
    /// </summary>
    public override void ResetAnimationCounter()
    {
        IceChunkState state = GetState();
        if (state == IceChunkState.MOVING) midAirAnimationCounter = 0;
    }

    #endregion
}
