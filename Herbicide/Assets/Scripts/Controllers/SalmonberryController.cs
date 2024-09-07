using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls a Salmonberry. <br></br>
/// 
/// The SalmonberryController is responsible for manipulating its Salmonberry and bringing
/// it to life. This includes moving it, choosing targets, playing animations,
/// and more.
/// </summary>
/// <![CDATA[<param name="SalmonberryState">]]>
public class SalmonberryController : ProjectileController<SalmonberryController.SalmonberryState>
{
    #region Fields

    /// <summary>
    /// Possible states of an Salmonberry over its lifetime.
    /// </summary>
    public enum SalmonberryState
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
    /// Gives an Salmonberry an SalmonberryController.
    /// </summary>
    /// <param name="salmonberry">The Salmonberry which will get an SalmonberryController.</param>
    /// <param name="destination">Where the Salmonberry started.</param>
    /// <param name="destination">Where the Salmonberry should go.</param>
    public SalmonberryController(Salmonberry salmonberry, Vector3 start, Vector3 destination) :
        base(salmonberry, start, destination)
    { }

    /// <summary>
    /// Returns the Salmonberry model.
    /// </summary>
    /// <returns>the Salmonberry model.</returns>
    protected Salmonberry GetSalmonberry() => GetProjectile() as Salmonberry;

    /// <summary>
    /// Helper method to handle the detonation of the Salmonberry at the specified explosion position.
    /// </summary>
    /// <param name="explosionPosition">The position of the explosion.</param>
    /// <param name="immuneObjects">Set of enemies immune to the explosion.</param>
    private void DetonateSalmonberry(Vector3 explosionPosition, HashSet<Enemy> immuneObjects)
    {
        EmanationController piercingSalmonberryEmanationController = new EmanationController(
            EmanationController.EmanationType.SALMONBERRY_EXPLOSION, 1,
            explosionPosition);
        ControllerController.AddEmanationController(piercingSalmonberryEmanationController);
        int explosionX = TileGrid.PositionToCoordinate(explosionPosition.x);
        int explosionY = TileGrid.PositionToCoordinate(explosionPosition.y);
        Vector2 tileExplosionPos = new Vector2(explosionX, explosionY);
        ExplosionController.ExplodeOnEnemies(tileExplosionPos,
            GetSalmonberry().EXPLOSION_RADIUS, GetSalmonberry().BASE_DAMAGE, immuneObjects);

    }

    /// <summary>
    /// Processes events that occur when the Salmonberry detonates at a given position.
    /// </summary>
    /// <param name="other">Collider2D the projectile collided with.</param>
    protected override void DetonateProjectile(Collider2D other)
    {
        Vector3 impactPoint = other.ClosestPoint(GetProjectile().transform.position);
        impactPoint = new Vector3(impactPoint.x, impactPoint.y, 1) - GetLinearDirection() * -.25f;
        HashSet<Enemy> immuneObjects = new HashSet<Enemy>();

        DetonateSalmonberry(impactPoint, immuneObjects);
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

        DetonateSalmonberry(explosionPosition, immuneObjects);
    }

    #endregion

    #region State Logic

    /// <summary>
    /// Updates the state of this SalmonberryController's Salmonberry model.
    /// The transitions are: <br></br>
    /// 
    /// SPAWN --> MOVING : when fired from source
    /// </summary>
    public override void UpdateStateFSM()
    {
        if (!ValidModel()) return;

        switch (GetState())
        {
            case SalmonberryState.SPAWN:
                SetState(SalmonberryState.MOVING);
                break;
            case SalmonberryState.MOVING:
                break;
        }
    }

    /// <summary>
    /// Returns true if two SalmonberryStates are equal.
    /// </summary>
    /// <param name="stateA">The first state.</param>
    /// <param name="stateB">The second state.</param>
    /// <returns>true if two SalmonberryStates are equal; otherwise, false.</returns>
    public override bool StateEquals(SalmonberryState stateA, SalmonberryState stateB) => stateA == stateB;

    /// <summary>
    /// Runs logic relevant to the Salmonberry's MOVING state.
    /// </summary>
    public override void ExecuteMovingState()
    {
        if (!ValidModel()) return;
        if (GetState() != SalmonberryState.MOVING) return;

        SetAnimation(GetSalmonberry().MID_AIR_ANIMATION_DURATION, ProjectileFactory.GetMidAirAnimationTrack(GetSalmonberry()));
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
        SalmonberryState state = GetState();
        if (state == SalmonberryState.MOVING) midAirAnimationCounter += Time.deltaTime;
    }

    /// <summary>
    /// Returns the animation counter for the current state.
    /// </summary>
    /// <returns>the animation counter for the current state.</returns>
    public override float GetAnimationCounter()
    {
        SalmonberryState state = GetState();
        if (state == SalmonberryState.MOVING) return midAirAnimationCounter;
        return 0;
    }

    /// <summary>
    /// Sets the animation counter for the current state to 0.
    /// </summary>
    public override void ResetAnimationCounter()
    {
        SalmonberryState state = GetState();
        if (state == SalmonberryState.MOVING) midAirAnimationCounter = 0;
    }

    #endregion
}
