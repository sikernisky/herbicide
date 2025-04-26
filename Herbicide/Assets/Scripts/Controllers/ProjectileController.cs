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
    private T State { get; set; }

    /// <summary>
    /// The State that triggered the Projectile's most recent animation.
    /// </summary>
    private T AnimationState { get; set; }

    /// <summary>
    /// Where the projectile started.
    /// </summary>
    private Vector3 StartPosition { get; set; }

    /// <summary>
    /// Where the projectile should go.
    /// </summary>
    private Vector3 TargetPosition { get; set; }

    /// <summary>
    /// The direction of the projectile during a LinearShot.
    /// </summary>
    private Vector3 LinearDirection { get; set; }

    /// <summary>
    /// The linear interpolation value for a LobShot.
    /// </summary>
    private float LobShotLinearInterpretation { get; set; }

    /// <summary>
    /// Counts the number of seconds in the mid air animation; resets
    /// on step.
    /// </summary>
    protected float MidAirAnimationCounter { get; set; }

    /// <summary>
    /// true if the Projectile should angle towards its target; otherwise,
    /// false.
    /// </summary>
    protected abstract bool ShouldAngleTowardsTarget { get; }

    #endregion

    #region Methods

    /// <summary>
    /// Makes a new ProjectileController for this Projectile.
    /// </summary>
    /// <param name="projectile">the Projectile that needs a controller.</param>
    /// <param name="start">where the Projectile started.</param>
    /// <param name="destination">where the Projectile should go.</param>
    public ProjectileController(Projectile projectile, Vector3 start, Vector3 destination) : base(projectile)
    {
        Assert.IsNotNull(projectile, "Projectile cannot be null.");
        StartPosition = start;
        GetProjectile().transform.position = start;
        TargetPosition = destination;
        LinearDirection = (destination - start).normalized;
        RotateProjectileUsingLinearDirection();
    }

    /// <summary>
    /// Rotates the Projectile to face its target based on its linear direction if
    /// it should angle towards its target.
    /// </summary>
    private void RotateProjectileUsingLinearDirection()
    {
        if(!ShouldAngleTowardsTarget) return;
        float angle = Mathf.Atan2(LinearDirection.y, LinearDirection.x) * Mathf.Rad2Deg;
        if (LinearDirection.x < 0) angle += 180;
        GetProjectile().transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
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
        UpdateFSM();
        ExecuteMovingState();
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
    protected Vector3 GetDestination() => TargetPosition;

    /// <summary>
    /// Returns true if this controller's Projectile should be destoyed and
    /// set to null.
    /// </summary>
    /// <returns>true if this controller's Projectile should be destoyed and
    /// set to null; otherwise, false.</returns>
    public override bool ValidModel()
    {
        if (GetProjectile().HasCollided) return false;
        if (GetProjectile().Expired()) return false;
        if (!GetProjectile().IsActive) return false;

        return true;
    }

    /// <summary>
    /// Returns the distance between the Projectile's starting position
    /// to its target position.
    /// </summary>
    /// <returns>the distance between the Projectile's starting position
    /// to its target position.</returns>
    protected float GetInitialTargetDistance() => Vector3.Distance(StartPosition, TargetPosition);

    /// <summary>
    /// Returns the linear direction of the Projectile.
    /// </summary>
    /// <returns>the linear direction of the Projectile.</returns>
    protected Vector3 GetLinearDirection() => LinearDirection;

    /// <summary>
    /// Handles a collision between the Projectile and some other Collider2D.
    /// </summary>
    /// <param name="other">Some other Collider2D.</param>
    protected override void HandleCollision(Collider2D other)
    {
        if (other == null) return;
        Model model = other.gameObject.GetComponent<Model>();
        if(!CanCollide(other, model)) return;
        model.TriggerProjectileCollision(GetProjectile());
        GetProjectile().SetCollided(true);
        DetonateProjectile(other);
        GetProjectile().AddColliderToIgnore(other);
    }

    /// <summary>
    /// Returns true if the Projectile can collide with a given Collider2D.
    /// </summary>
    /// <param name="other">the Collider2D to check for collision.</param>
    /// <param name="model">the Model of the Collider2D to check for collision.</param>
    /// <returns></returns>
    private bool CanCollide(Collider2D other, Model model)
    {
        if (GetProjectile().HasCollided) return false;
        if (GetProjectile().IgnoresCollider(other)) return false;
        if (model == null) return false;
        if(model.IgnoresCollider(GetProjectile().GetComponent<Collider2D>())) return false;
        if (LobShotLinearInterpretation > 0 && LobShotLinearInterpretation < .75f) return false; // Keep the lob illusion
        return true;
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
    protected virtual void DetonateProjectile(Collider2D other) 
    {
        FireSplitProjectiles(other.gameObject.GetComponent<Model>());
    }

    /// <summary>
    /// Handles the splitting of a Projectile. This method is called when a
    /// splitting projectile detonates.
    /// </summary>
    /// <param name="target">the Model on which the Projectile split.</param>
    protected virtual void FireSplitProjectiles(Model target)
    {
        if(GetProjectile().NumSplits <= 0) return;
        (Projectile splitLeft, Projectile splitRight) = MakePerpendicularProjectiles();
        (Vector2 leftTarget, Vector2 rightTarget) = GetPerpendicularTargetPositions();
        if(target != null) IgnorePerpendicularColliders(target, splitLeft, splitRight);
        FireProjectileFromModel(splitLeft, leftTarget, false);
        FireProjectileFromModel(splitRight, rightTarget, false);
    }

    /// <summary>
    /// Returns and creates two perpendicular Projectiles to the original Projectile.
    /// Reduces the number of splits by one for each new Projectile.
    /// </summary>
    /// <returns>two perpendicular Projectiles to the original Projectile.</returns>
    private (Projectile, Projectile) MakePerpendicularProjectiles()
    {
        Projectile splitLeft = GetProjectile().CreateNew().GetComponent<Projectile>();
        Projectile splitRight = GetProjectile().CreateNew().GetComponent<Projectile>();
        splitLeft.CloneFrom(GetProjectile());
        splitRight.CloneFrom(GetProjectile());
        splitLeft.SetNumSplits(GetProjectile().NumSplits-1);
        splitRight.SetNumSplits(GetProjectile().NumSplits-1);
        return (splitLeft, splitRight);
    }

    /// <summary>
    /// Returns two perpendicular vectors to the original target.
    /// </summary>
    /// <returns>two perpendicular vectors to the original target.</returns>
    private (Vector3 perp1, Vector3 perp2) GetPerpendicularTargetPositions()
    {
        Vector3 direction = (TargetPosition - StartPosition).normalized;
        Vector3 perpendicularLeft = new Vector3(-direction.y, direction.x, 0);
        Vector3 perpendicularRight = new Vector3(direction.y, -direction.x, 0);
        Vector3 leftTarget = TargetPosition + perpendicularLeft * PhysicsConstants.ProjectileSplitDistance;
        Vector3 rightTarget = TargetPosition + perpendicularRight * PhysicsConstants.ProjectileSplitDistance;
        return (leftTarget, rightTarget);
    }

    /// <summary>
    /// Adds the left and right split Projectiles to the list of Colliders2D to ignore.
    /// </summary>
    /// <param name="target">the Model on which the Projectile split.</param>
    /// <param name="splitLeft">the left split Projectile.</param>
    /// <param name="splitRight">the right split Projectile.</param>
    private void IgnorePerpendicularColliders(Model target, Projectile splitLeft, Projectile splitRight)
    {
        Assert.IsNotNull(target);
        Assert.IsNotNull(splitLeft);
        Assert.IsNotNull(splitRight);
        target.AddColliderToIgnore(splitLeft.GetComponent<Collider2D>());
        target.AddColliderToIgnore(splitRight.GetComponent<Collider2D>());
    }

    #endregion

    #region State Logic

    /// <summary>
    /// Sets the State of this ProjectileController. This helps keep track of
    /// what its Projectile should do and what it is doing, and it is essential
    /// for the FSM logic. 
    /// </summary>
    /// <param name="state">The new state.</param>
    public void SetState(T state) => this.State = state;

    /// <summary>
    /// Returns the State of this ProjectileController. This helps keep track of
    /// what its Projectile should do and what it is doing, and it is essential
    /// for the FSM logic. 
    /// </summary>
    /// <returns>The State of this ProjectileController. </returns>
    public T GetState() => State;

    /// <summary>
    /// Processes this ProjectileController's state FSM to determine the
    /// correct state. Takes the current state and chooses whether
    /// or not to switch to another based on game conditions.
    /// </summary>
    public abstract void UpdateFSM();

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
    /// Returns the State that triggered the Projectile's most recent
    /// animation.
    /// </summary>
    /// <returns>the State that triggered the Projectile's most recent
    /// animation.</returns>
    public T GetAnimationState() { return AnimationState; }

    /// <summary>
    /// Sets the State that triggered the Projectile's most recent
    /// animation.
    /// </summary>
    /// <param name="animationState">the animation state to set.</param>
    public void SetAnimationState(T animationState) => this.AnimationState = animationState;

    #endregion

    #region Movement Logic

    /// <summary>
    /// Called each frame: moves the Projectile towards its target in a linear
    /// fashion.
    /// </summary>
    protected virtual void LinearShot()
    {
        Projectile projectile = GetProjectile();
        if (projectile == null) return;

        Vector3 currentPosition = projectile.GetWorldPosition();
        float tileScale = BoardConstants.TileSize;
        float adjustedSpeed = projectile.Speed * tileScale; // Adjust speed based on tile scale
        float step = adjustedSpeed * Time.deltaTime;
        Vector3 newPosition = currentPosition + LinearDirection.normalized * step;
        projectile.SetWorldPosition(newPosition);
    }

    /// <summary>
    /// Called each frame: moves the Projectile towards its target in a parabolic
    /// fashion.
    /// </summary>
    public void LobShot()
    {
        if (GetProjectile() == null) return;

        // The lob is done and nothing was hit: detonate the projectile.
        if (LobShotLinearInterpretation >= 1)
        {
            DetonateProjectile(GetProjectile().GetWorldPosition());
            GetProjectile().SetCollided(true);
            return;
        }

        Vector3 linearProgress = Vector3.Lerp(StartPosition, TargetPosition, LobShotLinearInterpretation);
        float tileScale = BoardConstants.TileSize;
        float height = Mathf.Sin(LobShotLinearInterpretation * Mathf.PI) * GetProjectile().LobHeight;
        Vector3 newProjectilePos = new Vector3(linearProgress.x, linearProgress.y + height, linearProgress.z);
        GetProjectile().SetWorldPosition(newProjectilePos);

        // Adjust shadow position to appear lower as the projectile reaches its peak
        float shadowHeight = 1 - height / GetProjectile().LobHeight; // Normalize shadow height
        Vector3 shadowPosition = new Vector3(linearProgress.x, linearProgress.y - shadowHeight, linearProgress.z);
        GetProjectile().SetShadowPosition(shadowPosition);

        // Adjust lobLerp to scale with the tile size, affecting the increment speed
        LobShotLinearInterpretation += GetProjectile().Speed * tileScale * Time.deltaTime;
        if (Vector3.Distance(GetProjectile().GetWorldPosition(), TargetPosition) < 0.05f) LobShotLinearInterpretation = 1;
    }

    #endregion
}
