using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System;

/// <summary>
/// Abstract class to represent controllers of Projectiles.
/// 
/// The ProjectileController is responsible for manipulating its Projectile and bringing
/// it to life. This includes moving it, firing in a specific way, playing animations,
/// and more.
/// </summary>
/// <typeparam name="T">Enum to represent state of the Projectile.</typeparam>
public abstract class ProjectileController<T> : ModelController, IStateTracker<T> where T : Enum
{
    /// <summary>
    /// The number of Projectiles assigned to ProjectileControllers since
    /// this scene began.
    /// </summary>
    private static int NUM_PROJECTILES;

    /// <summary>
    /// The Projectile's state.
    /// </summary>
    private T state;

    /// <summary>
    /// The State that triggered the Projectile's most recent animation.
    /// </summary>
    private T animationState;

    /// <summary>
    /// Where the projectile started.
    /// </summary>
    private Vector3 start;

    /// <summary>
    /// Where the projectile should go.
    /// </summary>
    private Vector3 destination;

    /// <summary>
    /// The direction of the projectile during a LinearShot.
    /// </summary>
    private Vector3 linearDirection;

    /// <summary>
    /// Progress metric in our parabolic shot.
    /// </summary>
    private float parabolicProgress;

    /// <summary>
    /// Step metric in our parabolic shot.
    /// </summary>
    private float parabolicStep;

    /// <summary>
    /// true if the Projectile hit its target and deployed its effect;
    /// otherwise, false. If the projectile does not apply an effect,
    /// this is irrelevant.
    /// </summary>
    private bool effectApplied;


    /// <summary>
    /// Makes a new ProjectileController for this Projectile.
    /// </summary>
    /// <param name="projectile">the Projectile that needs a controller.</param>
    /// <param name="start">where the Projectile started.</param>
    /// <param name="destination">where the Projectile should go.</param>
    public ProjectileController(Projectile projectile, Vector3 start, Vector3 destination)
     : base(projectile)
    {
        Assert.IsNotNull(projectile, "Projectile cannot be null.");
        this.start = start;
        this.destination = destination;
        projectile.transform.position = start;
        linearDirection = (destination - GetProjectile().GetPosition()).normalized;
        NUM_PROJECTILES++;
    }

    /// <summary>
    /// Main update loop for the ProjectileController. Here's where
    /// it manipulates its Projectile based off game state. 
    /// </summary>
    public override void UpdateModel()
    {
        base.UpdateModel();
        if (!ValidModel()) return;
        GetProjectile().AddToLifespan(Time.deltaTime);
        UpdateStateFSM();
        ExecuteMovingState();
        ExecuteCollidingState();
        ExecuteDeadState();
    }

    /// <summary>
    /// Returns this ProjectileController's Projectile model. Inheriting controller
    /// classes use this method to access their Projectile; then, they cast
    /// it to its respective type.
    /// </summary>
    /// <returns>this ProjectileController's Projectile model.</returns>
    protected Projectile GetProjectile() { return GetModel() as Projectile; }

    /// <summary>
    /// Handles a collision between the Projectile and some other Collider2D.
    /// </summary>
    /// <param name="other">Some other Collider2D.</param>
    protected override void HandleCollision(Collider2D other) { return; }

    /// <summary>
    /// Returns the position to where the Projectile should go.
    /// </summary>
    /// <returns>the position to where the Projectile should go.</returns>
    protected Vector3 GetDestination() { return destination; }

    /// <summary>
    /// Returns true if the projectile applied its effect.
    /// </summary>
    /// <returns>true if the projectile applied its effect; otherwise,
    /// false.</returns>
    protected virtual bool AppliedEffect() { return effectApplied; }

    /// <summary>
    /// Informs the ProjectileController that it applied its effect, if
    /// it has one.
    /// </summary>
    protected virtual void ApplyEffect() { effectApplied = true; }

    /// <summary>
    /// Returns true if this controller's Projectile should be destoyed and
    /// set to null.
    /// </summary>
    /// <returns>true if this controller's Projectile should be destoyed and
    /// set to null; otherwise, false.</returns>
    protected override bool ShouldRemoveModel()
    {
        if (GetProjectile().GetVictim() != null) return true;
        if (GetProjectile().Expired()) return true;
        if (!GetProjectile().IsActive()) return true;

        return false;
    }

    /// <summary>
    /// Returns the distance between the Projectile's starting position
    /// to its target position.
    /// </summary>
    /// <returns>the distance between the Projectile's starting position
    /// to its target position.</returns>
    protected float GetInitialTargetDistance() { return Vector3.Distance(start, destination); }

    //-----------------------STATE LOGIC------------------------//

    /// <summary>
    /// Sets the State of this ProjectileController. This helps keep track of
    /// what its Projectile should do and what it is doing, and it is essential
    /// for the FSM logic. 
    /// </summary>
    /// <param name="state">The new state.</param>
    public void SetState(T state) { this.state = state; }

    /// <summary>
    /// Returns the State of this ProjectileController. This helps keep track of
    /// what its Projectile should do and what it is doing, and it is essential
    /// for the FSM logic. 
    /// </summary>
    /// <returns>The State of this ProjectileController. </returns>
    public T GetState() { return state; }

    /// <summary>
    /// Processes this ProjectileController's state FSM to determine the
    /// correct state. Takes the current state and chooses whether
    /// or not to switch to another based on game conditions. /// 
    /// </summary>
    public abstract void UpdateStateFSM();

    /// <summary>
    /// Returns true if two states are equal.
    /// </summary>
    /// <param name="stateA">The first state.</param>
    /// <param name="stateB">The second state</param>
    /// <returns>true if two states are equal; otherwise, false. </returns>
    public abstract bool StateEquals(T stateA, T stateB);

    /// <summary>
    /// Logic to execute when this ProjectileController's Projectile is moving.
    /// The ProjectileController manipulates the Projectile model by calling
    /// its methods.
    /// </summary>
    public abstract void ExecuteMovingState();

    /// <summary>
    /// Logic to execute when this ProjectileController's Projectile is moving.
    /// The ProjectileController manipulates the Projectile model by calling
    /// its methods.
    /// </summary>
    public abstract void ExecuteCollidingState();

    /// <summary>
    /// Logic to execute when this ProjectileController's Projectile is dead.
    /// The ProjectileController manipulates the Projectile model by calling
    /// its methods.
    /// </summary>
    public abstract void ExecuteDeadState();

    /// <summary>
    /// Returns the State that triggered the Projectile's most recent
    /// animation.
    /// </summary>
    /// <returns>the State that triggered the Projectile's most recent
    /// animation.</returns>
    public T GetAnimationState() { return animationState; }

    /// <summary>
    /// Sets the State that triggered the Projectile's most recent
    /// animation.
    /// </summary>
    /// <param name="animationState">the animation state to set.</param>
    public void SetAnimationState(T animationState) { this.animationState = animationState; }


    //------------------------SHOT/MOVEMENT TYPES------------------------//

    /// <summary>
    /// Called each frame: moves the Projectile towards its target in a lobbing
    /// fashion.
    /// </summary>
    protected virtual void ParabolicShot(float height, float maxSize)
    {
        if (GetProjectile() == null) return;

        parabolicStep = Time.deltaTime / .5f;
        parabolicProgress = Mathf.Min(parabolicProgress + parabolicStep, 1f);

        float parabola = 1.0f - 4.0f * (parabolicProgress - 0.5f) * (parabolicProgress - 0.5f);
        Vector3 nextPos = Vector3.Lerp(start, destination, parabolicProgress);
        nextPos.y += parabola * height;

        Vector3 nextShadowPos = nextPos;
        nextShadowPos.y -= parabola * height;
        nextShadowPos.x += (parabola * height) / 2f;

        float newSize = parabola * maxSize;
        newSize = Mathf.Clamp(newSize, 1f, maxSize);
        GetProjectile().transform.localScale = new Vector3(newSize, newSize, 1);
        GetProjectile().SetWorldPosition(nextPos);
        GetProjectile().SetShadowPosition(nextShadowPos);

        if (parabolicProgress == 1)
        {
            GetProjectile().SetWorldPosition(GetDestination());
            GetProjectile().SetReachedTarget();
        }
    }

    /// <summary>
    /// Called each frame: moves the Projectile towards its target in a linear
    /// fashion.
    /// </summary>
    protected virtual void LinearShot()
    {
        if (GetProjectile() == null) return;

        Vector3 currentPosition = GetProjectile().GetPosition();
        Vector3 targetPosition = GetDestination();

        // Calculate the step size
        float step = GetProjectile().GetSpeed() * Time.deltaTime;

        // Calculate the new position by moving along the direction vector
        Vector3 newPosition = currentPosition + linearDirection * step;
        GetProjectile().SetWorldPosition(newPosition);

        // Check if the projectile has reached the destination
        if (currentPosition == targetPosition) { }
    }
}
