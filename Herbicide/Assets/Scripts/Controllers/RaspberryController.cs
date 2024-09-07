using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls a Raspberry. <br></br>
/// 
/// The RaspberryController is responsible for manipulating its Raspberry and bringing
/// it to life. This includes moving it, choosing targets, playing animations,
/// and more.
/// </summary>
/// <![CDATA[<param name="RaspberryState">]]>
public class RaspberryController : ProjectileController<RaspberryController.RaspberryState> 
{
    #region Fields

    /// <summary>
    /// Possible states of an Raspberry over its lifetime.
    /// </summary>
    public enum RaspberryState
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
    /// Gives an Raspberry an RaspberryController.
    /// </summary>
    /// <param name="raspberry">The Raspberry which will get an RaspberryController.</param>
    /// <param name="destination">Where the Raspberry started.</param>
    /// <param name="destination">Where the Raspberry should go.</param>
    public RaspberryController(Raspberry raspberry, Vector3 start, Vector3 destination) :
        base(raspberry, start, destination)
    { }

    /// <summary>
    /// Returns the Raspberry model.
    /// </summary>
    /// <returns>the Raspberry model.</returns>
    protected Raspberry GetRaspberry() => GetProjectile() as Raspberry;

    /// <summary>
    /// Helper method to handle the detonation of the Raspberry at the specified explosion position.
    /// </summary>
    /// <param name="explosionPosition">The position of the explosion.</param>
    /// <param name="immuneObjects">Set of enemies immune to the explosion.</param>
    private void DetonateRaspberry(Vector3 explosionPosition, HashSet<Enemy> immuneObjects)
    {
        EmanationController piercingRaspberryEmanationController = new EmanationController(
            EmanationController.EmanationType.RASPBERRY_EXPLOSION, 1,
            explosionPosition);
        ControllerController.AddEmanationController(piercingRaspberryEmanationController);
        int explosionX = TileGrid.PositionToCoordinate(explosionPosition.x);
        int explosionY = TileGrid.PositionToCoordinate(explosionPosition.y);
        Vector2 tileExplosionPos = new Vector2(explosionX, explosionY);
        ExplosionController.ExplodeOnEnemies(tileExplosionPos,
            GetRaspberry().EXPLOSION_RADIUS, GetRaspberry().BASE_DAMAGE, immuneObjects);

    }

    /// <summary>
    /// Processes events that occur when the Raspberry detonates at a given position.
    /// </summary>
    /// <param name="other">Collider2D the projectile collided with.</param>
    protected override void DetonateProjectile(Collider2D other)
    {
        Vector3 impactPoint = other.ClosestPoint(GetProjectile().transform.position);
        impactPoint = new Vector3(impactPoint.x, impactPoint.y, 1) - GetLinearDirection() * -.25f;
        HashSet<Enemy> immuneObjects = new HashSet<Enemy>();

        DetonateRaspberry(impactPoint, immuneObjects);
    }

    /// <summary>
    /// Processes events that occur when this Projectile detonates at a given position.
    /// This method is used because not all Projectiles collide with a Collider2D.
    /// Some Projectiles detonate at a position.
    /// </summary>
    /// <param name="detonationPosition">The position where the Projectile detonated.</param>
    protected override void DetonateProjectile(Vector3 detonationPosition)
    {
        Vector3 explosionPosition = detonationPosition - GetLinearDirection() * -.25f;
        explosionPosition = new Vector3(explosionPosition.x, explosionPosition.y, 1);

        HashSet<Enemy> immuneObjects = new HashSet<Enemy>();

        DetonateRaspberry(explosionPosition, immuneObjects);
    }

    #endregion

    #region State Logic

    /// <summary>
    /// Updates the state of this RaspberryController's Raspberry model.
    /// The transitions are: <br></br>
    /// 
    /// SPAWN --> MOVING : when fired from source
    /// </summary>
    public override void UpdateStateFSM()
    {
        if (!ValidModel()) return;

        switch (GetState())
        {
            case RaspberryState.SPAWN:
                SetState(RaspberryState.MOVING);
                break;
            case RaspberryState.MOVING:
                break;
        }
    }

    /// <summary>
    /// Returns true if two RaspberryStates are equal.
    /// </summary>
    /// <param name="stateA">The first state.</param>
    /// <param name="stateB">The second state.</param>
    /// <returns>true if two RaspberryStates are equal; otherwise, false.</returns>
    public override bool StateEquals(RaspberryState stateA, RaspberryState stateB) => stateA == stateB;

    /// <summary>
    /// Runs logic relevant to the Raspberry's MOVING state.
    /// </summary>
    public override void ExecuteMovingState()
    {
        if (!ValidModel()) return;
        if (GetState() != RaspberryState.MOVING) return;

        SetAnimation(GetRaspberry().MID_AIR_ANIMATION_DURATION, ProjectileFactory.GetMidAirAnimationTrack(GetRaspberry()));
        LobShot();
    }

    #endregion

    #region Animation Logic

    /// <summary>
    /// Adds one chunk of Time.deltaTime to the animation
    /// counter that tracks the current state.
    /// </summary>
    public override void AgeAnimationCounter()
    {
        RaspberryState state = GetState();
        if (state == RaspberryState.MOVING) midAirAnimationCounter += Time.deltaTime;
    }

    /// <summary>
    /// Returns the animation counter for the current state.
    /// </summary>
    /// <returns>the animation counter for the current state.</returns>
    public override float GetAnimationCounter()
    {
        RaspberryState state = GetState();
        if (state == RaspberryState.MOVING) return midAirAnimationCounter;
        return 0;
    }

    /// <summary>
    /// Sets the animation counter for the current state to 0.
    /// </summary>
    public override void ResetAnimationCounter()
    {
        RaspberryState state = GetState();
        if (state == RaspberryState.MOVING) midAirAnimationCounter = 0;
    }

    #endregion
}
