using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.Collections.Generic;

/// <summary>
/// Controls a Projectile. <br></br>
/// 
/// The ProjectileController is responsible for manipulating its Projectile and bringing
/// it to life. This includes moving it, choosing targets, playing animations,
/// and more.
/// </summary>
/// <typeparam name="T">Enum to represent state of the Projectile.</typeparam>
public abstract class ProjectileController<T> : ModelController, IStateTracker<T> where T : Enum
{
    #region Fields

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
    /// The linear interpolation value for a LobShot.
    /// </summary>
    private float lobLerp;

    /// <summary>
    /// Counts the number of seconds in the mid air animation; resets
    /// on step.
    /// </summary>
    protected float midAirAnimationCounter;

    /// <summary>
    /// All Colliders2D that this Projectile has collided with.
    /// </summary>
    private HashSet<Collider2D> collidees;


    /// <summary>
    /// True if the Projectile should angle towards its target; otherwise,
    /// false.
    /// </summary>
    protected abstract bool angleTowardsTarget { get; }

    #endregion

    #region Methods

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
        linearDirection = (destination - start).normalized;
        if (angleTowardsTarget)
        {
            float angle = Mathf.Atan2(linearDirection.y, linearDirection.x) * Mathf.Rad2Deg;
            if(linearDirection.x < 0) angle += 180;
            projectile.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
        if (collidees == null) collidees = new HashSet<Collider2D>();
    }

    /// <summary>
    /// Main update loop for the ProjectileController. Here's where
    /// it manipulates its Projectile based off game state. 
    /// </summary>
    /// <param name="gameState">The most recent GameState.</param>
    public override void UpdateController(GameState gameState)
    {
        base.UpdateController(gameState);
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
    protected Projectile GetProjectile() => GetModel() as Projectile;

    /// <summary>
    /// Returns the Projectile prefab to the AcornFactory object pool.
    /// </summary>
    public override void ReturnModelToFactory() => ProjectileFactory.ReturnProjectilePrefab(GetProjectile().gameObject);

    /// <summary>
    /// Returns the position to where the Projectile should go.
    /// </summary>
    /// <returns>the position to where the Projectile should go.</returns>
    protected Vector3 GetDestination() => destination;

    /// <summary>
    /// Returns true if this controller's Projectile should be destoyed and
    /// set to null.
    /// </summary>
    /// <returns>true if this controller's Projectile should be destoyed and
    /// set to null; otherwise, false.</returns>
    public override bool ValidModel()
    {
        if (GetProjectile().Collided()) return false;
        if (GetProjectile().Expired()) return false;
        if (!GetProjectile().IsActive()) return false;

        return true;
    }

    /// <summary>
    /// Returns the distance between the Projectile's starting position
    /// to its target position.
    /// </summary>
    /// <returns>the distance between the Projectile's starting position
    /// to its target position.</returns>
    protected float GetInitialTargetDistance() => Vector3.Distance(start, destination);

    /// <summary>
    /// Returns the linear direction of the Projectile.
    /// </summary>
    /// <returns>the linear direction of the Projectile.</returns>
    protected Vector3 GetLinearDirection() => linearDirection;

    /// <summary>
    /// Handles a collision between the Projectile and some other Collider2D.
    /// </summary>
    /// <param name="other">Some other Collider2D.</param>
    protected override void HandleCollision(Collider2D other)
    {
        if (other == null) return;
        Model model = other.gameObject.GetComponent<Model>();
        if (model == null) return;
        if (lobLerp > 0 && lobLerp < .6f) return; // Keep the lob illusion
        if(collidees.Contains(other)) return;

        Debug.Log("Here");

        model.TriggerProjectileCollision(GetProjectile());
        DetonateProjectile(other);
        GetProjectile().SetCollided(true);
        AddColliderToIgnore(other);
    }

    /// <summary>
    /// Adds a Collider2D to the list of Colliders2D to ignore.
    /// The projectile will not detonate when colliding with this Collider2D.
    /// </summary>
    /// <param name="other">The collider to ignore. </param>
    public void AddColliderToIgnore(Collider2D other)
    {
        Assert.IsNotNull(other);
        if (collidees == null) collidees = new HashSet<Collider2D>();
        
        collidees.Add(other);
    }

    /// <summary>
    /// Processes events that occur when this Projectile detonates at
    /// a given position. <br></br>
    /// 
    /// We have this method because not all Projectiles collide with
    /// a Collider2D. Some Projectiles detonate at a position.
    /// </summary>
    /// <param name="detonationPosition">The position where the Projectile
    /// detonated.</param>
    protected virtual void DetonateProjectile(Vector3 detonationPosition) { return; }

    /// <summary>
    /// Processes events that occur when this projectile detonates at
    /// a given Collider2D.
    /// </summary>
    /// <param name="other">The Collider2D with which the Projectile collided. </param>
    protected virtual void DetonateProjectile(Collider2D other) { return; }

    #endregion

    #region State Logic

    /// <summary>
    /// Sets the State of this ProjectileController. This helps keep track of
    /// what its Projectile should do and what it is doing, and it is essential
    /// for the FSM logic. 
    /// </summary>
    /// <param name="state">The new state.</param>
    public void SetState(T state) => this.state = state;

    /// <summary>
    /// Returns the State of this ProjectileController. This helps keep track of
    /// what its Projectile should do and what it is doing, and it is essential
    /// for the FSM logic. 
    /// </summary>
    /// <returns>The State of this ProjectileController. </returns>
    public T GetState() => state;

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
    public void SetAnimationState(T animationState) => this.animationState = animationState;

    #endregion

    #region Movement Logic

    /// <summary>
    /// Called each frame: moves the Projectile towards its target in a linear
    /// fashion.
    /// </summary>
    protected virtual void LinearShot()
    {
        if (GetProjectile() == null) return;

        Vector3 currentPosition = GetProjectile().GetPosition();
        float step = GetProjectile().GetSpeed() * Time.deltaTime;
        Vector3 newPosition = currentPosition + linearDirection * step;
        GetProjectile().SetWorldPosition(newPosition);
    }

    /// <summary>
    /// Called each frame: moves the Projectile towards its target in a parabolic
    /// fashion.
    /// </summary>
    public void LobShot()
    {
        if (GetProjectile() == null) return;

        // The lob is done and nothing was hit: detonate the projectile.
        if (lobLerp >= 1)
        {
            DetonateProjectile(GetProjectile().GetPosition());
            GetProjectile().SetCollided(true);
            return;
        }

        Vector3 linearProgress = Vector3.Lerp(start, destination, lobLerp);
        float height = Mathf.Sin(lobLerp * Mathf.PI) * GetProjectile().LOB_HEIGHT;
        Vector3 newProjectilePos = new Vector3(linearProgress.x, linearProgress.y + height, linearProgress.z);
        GetProjectile().SetWorldPosition(newProjectilePos);

        // Adjust shadow position to appear lower as the projectile reaches its peak
        float shadowHeight = 1 - height / GetProjectile().LOB_HEIGHT; // Normalize shadow height
        Vector3 shadowPosition = new Vector3(linearProgress.x, linearProgress.y - shadowHeight, linearProgress.z);
        GetProjectile().SetShadowPosition(shadowPosition);

        lobLerp += GetProjectile().GetSpeed() * Time.deltaTime;
        if (Vector3.Distance(GetProjectile().GetPosition(), destination) < 0.05f) lobLerp = 1;
    }

    #endregion
}
