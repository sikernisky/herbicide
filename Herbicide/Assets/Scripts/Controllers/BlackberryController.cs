using System.Collections.Generic;
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

    /// <summary>
    /// Helper method to handle the detonation of the Blackberry at the specified explosion position.
    /// </summary>
    /// <param name="explosionPosition">The position of the explosion.</param>
    /// <param name="immuneObjects">Set of enemies immune to the explosion.</param>
    private void DetonateBlackberry(Vector3 explosionPosition, HashSet<Enemy> immuneObjects)
    {
        EmanationController piercingBlackberryEmanationController = new EmanationController(
            EmanationController.EmanationType.BLACKBERRY_EXPLOSION, 1,
            explosionPosition);
        ControllerController.AddEmanationController(piercingBlackberryEmanationController);
        int explosionX = TileGrid.PositionToCoordinate(explosionPosition.x);
        int explosionY = TileGrid.PositionToCoordinate(explosionPosition.y);
        Vector2 tileExplosionPos = new Vector2(explosionX, explosionY);
        ExplosionController.ExplodeOnEnemies(tileExplosionPos,
            GetBlackberry().EXPLOSION_RADIUS, GetBlackberry().BASE_DAMAGE, immuneObjects);

    }

    /// <summary>
    /// Processes events that occur when the Blackberry detonates at a given position.
    /// </summary>
    /// <param name="other">Collider2D the projectile collided with.</param>
    protected override void DetonateProjectile(Collider2D other)
    {
        Vector3 impactPoint = other.ClosestPoint(GetProjectile().transform.position);
        impactPoint = new Vector3(impactPoint.x, impactPoint.y, 1) - GetLinearDirection() * -.25f;
        HashSet<Enemy> immuneObjects = new HashSet<Enemy>();

        DetonateBlackberry(impactPoint, immuneObjects);
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

        DetonateBlackberry(explosionPosition, immuneObjects);
    }

    #endregion

    #region State Logic

    /// <summary>
    /// Updates the state of this BlackberryController's Blackberry model.
    /// The transitions are: <br></br>
    /// 
    /// SPAWN --> MOVING : when fired from source
    /// </summary>
    public override void UpdateFSM()
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
