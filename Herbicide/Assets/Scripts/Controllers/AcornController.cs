using UnityEngine;

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
        MOVING,
        COLLIDING,
        DEAD
    }

    /// <summary>
    /// Acorns do not angle towards their target.
    /// </summary>
    protected override bool angleTowardsTarget => false;

    #endregion

    #region Methods

    /// <summary>
    /// Gives an Acorn an AcornController.
    /// </summary>
    /// <param name="acorn">The acorn which will get an AcornController.</param>
    /// <param name="destination">Where the acorn started.</param>
    /// <param name="destination">Where the acorn should go.</param>
    public AcornController(Acorn acorn, Vector3 start, Vector3 destination) :
        base(acorn, start, destination) { }

    /// <summary>
    /// Returns the Acorn model.
    /// </summary>
    /// <returns>the Acorn model.</returns>
    protected Acorn GetAcorn() => GetProjectile() as Acorn;

    /// <summary>
    /// Returns the Acorn prefab to the AcornFactory singleton.
    /// </summary>
    public override void DestroyModel() => ProjectileFactory.ReturnProjectilePrefab(GetAcorn().gameObject);

    /// <summary>
    /// Handles a collision between the Projectile and some other Collider2D.
    /// </summary>
    /// <param name="other">Some other Collider2D.</param>
    protected override void HandleCollision(Collider2D other)
    {
        if (other == null) return;
        Model model = other.gameObject.GetComponent<Model>();
        if (model == null) return;
        model.TriggerProjectileCollision(GetProjectile());
        GetProjectile().SetCollided(model);
    }

    #endregion

    #region State Logic

    /// <summary>
    /// Updates the state of this AcornController's Acorn model.
    /// The transitions are: <br></br>
    /// 
    /// SPAWN --> MOVING : when fired from source <br></br>
    /// MOVING --> COLLIDING : when hits valid target <br></br>
    /// COLLIDING --> DEAD : when all effects have been applied to valid target <br></br>
    /// </summary>
    public override void UpdateStateFSM()
    {
        if (!ValidModel()) return;

        switch (GetState())
        {
            case AcornState.SPAWN:
                SetState(AcornState.MOVING);
                break;
            case AcornState.MOVING:
                break;
            case AcornState.COLLIDING:
                break;
            case AcornState.DEAD:
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
    /// Runs logic relevant to the Acorn's MOVING state.
    /// </summary>
    public override void ExecuteMovingState()
    {
        if (!ValidModel()) return;
        if (GetState() != AcornState.MOVING) return;

        SetAnimation(GetAcorn().MID_AIR_ANIMATION_DURATION, ProjectileFactory.GetMidAirAnimationTrack(GetAcorn()));
        LinearShot();
    }

    /// <summary>
    /// Runs logic relevant to the Acorn's COLLIDING state.
    /// </summary>
    public override void ExecuteCollidingState() { }

    /// <summary>
    /// Runs logic relevant to the Acorn's DEAD state.
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
        AcornState state = GetState();
        if (state == AcornState.MOVING) midAirAnimationCounter += Time.deltaTime;
    }

    /// <summary>
    /// Returns the animation counter for the current state.
    /// </summary>
    /// <returns>the animation counter for the current state.</returns>
    public override float GetAnimationCounter()
    {
        AcornState state = GetState();
        if (state == AcornState.MOVING) return midAirAnimationCounter;
        return 0;
    }

    /// <summary>
    /// Sets the animation counter for the current state to 0.
    /// </summary>
    public override void ResetAnimationCounter()
    {
        AcornState state = GetState();
        if (state == AcornState.MOVING) midAirAnimationCounter = 0;
    }

    #endregion
}
