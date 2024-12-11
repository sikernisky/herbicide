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
    protected override bool angleTowardsTarget => false;

    #endregion

    #region Methods

    /// <summary>
    /// Gives an Acorn an AcornController.
    /// </summary>
    /// <param name="acorn">The acorn which will get an AcornController.</param>
    /// <param name="start">Where the acorn started.</param>
    /// <param name="destination">Where the acorn should go.</param>
    /// <param name="numSplits">How many times the acorn will split.</param>
    public AcornController(Acorn acorn, Vector3 start, Vector3 destination, int numSplits) :
        base(acorn, start, destination) 
    {
        GetAcorn().SetNumSplits(numSplits);
    }

    /// <summary>
    /// Returns the Acorn model.
    /// </summary>
    /// <returns>the Acorn model.</returns>
    protected Acorn GetAcorn() => GetProjectile() as Acorn;

    /// <summary>
    /// Processes events that occur when the Acorn detonates at a given position.
    /// </summary>
    /// <param name="other">Collider2D the projectile collided with.</param>
    protected override void DetonateProjectile(Collider2D other)
    {
        Vector3 impactPoint = other.ClosestPoint(GetProjectile().transform.position);
        impactPoint = new Vector3(impactPoint.x, impactPoint.y, 1) - GetLinearDirection() * -.25f;

        if (GetAcorn().GetNumSplits() <= 0) return;

        Vector2 originalDirection = GetLinearDirection();
        Vector2 perpDirection1 = new Vector2(-originalDirection.y, originalDirection.x);
        Vector2 perpDirection2 = new Vector2(originalDirection.y, -originalDirection.x);
        float distanceMultiplier = 1000f;

        GameObject acornPrefab = ProjectileFactory.GetProjectilePrefab(ModelType.ACORN);
        Assert.IsNotNull(acornPrefab);
        Acorn acornComp = acornPrefab.GetComponent<Acorn>();
        Assert.IsNotNull(acornComp);
        Vector3 targetPosition1 = impactPoint + new Vector3(perpDirection1.x, perpDirection1.y, 1) * distanceMultiplier;
        int numSplits = GetAcorn().GetNumSplits() - 1;
        AcornController acornController = new AcornController(acornComp, impactPoint, targetPosition1, numSplits);
        acornController.AddColliderToIgnore(other);
        ControllerManager.AddModelController(acornController);

        acornPrefab = ProjectileFactory.GetProjectilePrefab(ModelType.ACORN);
        Assert.IsNotNull(acornPrefab);
        acornComp = acornPrefab.GetComponent<Acorn>();
        Assert.IsNotNull(acornComp);
        Vector3 targetPosition2 = impactPoint + new Vector3(perpDirection2.x, perpDirection2.y, 1) * distanceMultiplier;
        acornController = new AcornController(acornComp, impactPoint, targetPosition2, numSplits);
        acornController.AddColliderToIgnore(other);
        ControllerManager.AddModelController(acornController);
    }

    #endregion

    #region State Logic

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

        SetNextAnimation(GetAcorn().MID_AIR_ANIMATION_DURATION, ProjectileFactory.GetMidAirAnimationTrack(GetAcorn()));
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
