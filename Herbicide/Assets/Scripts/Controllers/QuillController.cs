using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls a Quill. <br></br>
/// 
/// The QuillController is responsible for manipulating its Quill and bringing
/// it to life. This includes moving it, choosing targets, playing animations,
/// and more.
/// </summary>
/// <![CDATA[<param name="QuillState">]]>
public class QuillController : ProjectileController<QuillController.QuillState>
{
    #region Fields

    /// <summary>
    /// Possible states of a Quill over its lifetime.
    /// </summary>
    public enum QuillState
    {
        SPAWN,
        MOVING
    }

    /// <summary>
    /// Quills angle towards their target.
    /// </summary>
    protected override bool angleTowardsTarget => true;

    #endregion

    #region Methods

    /// <summary>
    /// Gives an Quill an QuillController.
    /// </summary>
    /// <param name="quill">The quill which will get an QuillController.</param>
    /// <param name="destination">Where the quill started.</param>
    /// <param name="destination">Where the quill should go.</param>
    public QuillController(Quill quill, Vector3 start, Vector3 destination) :
        base(quill, start, destination)
    { }

    /// <summary>
    /// Returns the Quill model.
    /// </summary>
    /// <returns>the Quill model.</returns>
    protected Quill GetQuill() => GetProjectile() as Quill;

    /// <summary>
    /// Handles a collision between the Quill and some other Collider2D.
    /// </summary>
    /// <param name="other">Some other Collider2D.</param>
    protected override void HandleCollision(Collider2D other) => base.HandleCollision(other);

    /// <summary>
    /// Helper method to handle the detonation of the quill at the specified explosion position.
    /// </summary>
    /// <param name="explosionPosition">The position of the explosion.</param>
    /// <param name="immuneObjects">Set of enemies immune to the explosion.</param>
    private void DetonateQuill(Vector3 explosionPosition, HashSet<Enemy> immuneObjects)
    {
        // We need the piercing quill to define the custom explosion area
        GameObject piercingQuill = GetQuill().GetPiercingQuill();
        piercingQuill.transform.position = explosionPosition;
        piercingQuill.transform.rotation = GetProjectile().transform.rotation;
        if (GetLinearDirection().x >= 0)
            piercingQuill.transform.Rotate(0, 0, 180);

        EmanationController piercingQuillEmanationController = new EmanationController(
            EmanationController.EmanationType.QUILL_PIERCE, 1,
            piercingQuill.transform.position,
            piercingQuill.transform.rotation);
        ControllerController.AddEmanationController(piercingQuillEmanationController);
        ExplosionController.ExplodeOnEnemies(piercingQuill, GetQuill().PIERCING_DAMAGE, immuneObjects);
    }

    /// <summary>
    /// Processes events that occur when the Quill detonates at a given position.
    /// </summary>
    /// <param name="other">Collider2D the projectile collided with.</param>
    protected override void DetonateProjectile(Collider2D other)
    {
        Vector3 impactPoint = other.ClosestPoint(GetProjectile().transform.position);
        Vector3 explosionPosition = impactPoint - GetLinearDirection() * -1f;
        explosionPosition = new Vector3(explosionPosition.x, explosionPosition.y, 1);
        Enemy initialTarget = other.gameObject.GetComponent<Model>() as Enemy;
        HashSet<Enemy> immuneObjects = new HashSet<Enemy>() { initialTarget };
        DetonateQuill(explosionPosition, immuneObjects);
    }

    /// <summary>
    /// Processes events that occur when this Projectile detonates at a given position.
    /// This method is used because not all Projectiles collide with a Collider2D.
    /// Some Projectiles detonate at a position.
    /// </summary>
    /// <param name="detonationPosition">The position where the Projectile detonated.</param>
    protected override void DetonateProjectile(Vector3 detonationPosition)
    {
        Vector3 explosionPosition = detonationPosition - GetLinearDirection() * -1f;
        explosionPosition = new Vector3(explosionPosition.x, explosionPosition.y, 1);
        HashSet<Enemy> immuneObjects = new HashSet<Enemy>();
        DetonateQuill(explosionPosition, immuneObjects);
    }

    #endregion

    #region State Logic

    /// <summary>
    /// Updates the state of this QuillController's Quill model.
    /// The transitions are: <br></br>
    /// 
    /// SPAWN --> MOVING : when fired from source
    /// </summary>
    public override void UpdateStateFSM()
    {
        if (!ValidModel()) return;

        switch (GetState())
        {
            case QuillState.SPAWN:
                SetState(QuillState.MOVING);
                break;
            case QuillState.MOVING:
                break;
        }
    }

    /// <summary>
    /// Returns true if two QuillStates are equal.
    /// </summary>
    /// <param name="stateA">The first state.</param>
    /// <param name="stateB">The second state.</param>
    /// <returns>true if two QuillStates are equal; otherwise, false.</returns>
    public override bool StateEquals(QuillState stateA, QuillState stateB) => stateA == stateB;

    /// <summary>
    /// Runs logic relevant to the Quill's MOVING state.
    /// </summary>
    public override void ExecuteMovingState()
    {
        if (!ValidModel()) return;
        if (GetState() != QuillState.MOVING) return;

        SetAnimation(GetQuill().MID_AIR_ANIMATION_DURATION, ProjectileFactory.GetMidAirAnimationTrack(GetQuill()));
        LinearShot();
    }

    /// <summary>
    /// Runs logic relevant to the Quill's COLLIDING state.
    /// </summary>
    public override void ExecuteCollidingState() { return; }

    /// <summary>
    /// Runs logic relevant to the Quill's DEAD state.
    /// </summary>
    public override void ExecuteDeadState() { return; }

    #endregion

    #region Animation Logic

    /// <summary>
    /// Adds one chunk of Time.deltaTime to the animation
    /// counter that tracks the current state.
    /// </summary>
    public override void AgeAnimationCounter()
    {
        QuillState state = GetState();
        if (state == QuillState.MOVING) midAirAnimationCounter += Time.deltaTime;
    }

    /// <summary>
    /// Returns the animation counter for the current state.
    /// </summary>
    /// <returns>the animation counter for the current state.</returns>
    public override float GetAnimationCounter()
    {
        QuillState state = GetState();
        if (state == QuillState.MOVING) return midAirAnimationCounter;
        return 0;
    }

    /// <summary>
    /// Sets the animation counter for the current state to 0.
    /// </summary>
    public override void ResetAnimationCounter()
    {
        QuillState state = GetState();
        if (state == QuillState.MOVING) midAirAnimationCounter = 0;
    }

    #endregion
}
