using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Represents something that can be shot and collide with a
/// Model.
/// </summary>
public abstract class Projectile : Model
{
    #region Fields

    /// <summary>
    /// Current speed of this Projectile.
    /// </summary>
    private float speed;

    /// <summary>
    /// Current damage of this Projectile.
    /// </summary>
    private int damage;

    /// <summary>
    /// How long this Projectile has been active
    /// </summary>
    private float age;

    /// <summary>
    /// true if this Projectile is active in the scene.
    /// </summary>
    private bool active = true;

    /// <summary>
    /// Projectile's RigidBody component.
    /// </summary>
    [SerializeField]
    private Rigidbody2D projectileBody;

    /// <summary>
    /// The Projectile's Shadow.
    /// </summary>
    [SerializeField]
    private GameObject shadow;

    /// <summary>
    /// true if this Projectile hit something; otherwise, false. 
    /// </summary>
    private bool collided;

    /// <summary>
    /// true if this Projectile traveled and reached its target destination;
    /// otherwise, false. Note: some projectiles do not have a "target" and
    /// move forever -- in that case, this is irrelevant. 
    /// </summary>
    private bool reachedTarget;

    #endregion

    #region Stats

    /// <summary>
    /// Starting speed of this Projectile.
    /// </summary>
    public abstract float BASE_SPEED { get; }

    /// <summary>
    /// Maximum speed of this Projectile.
    /// </summary>
    public abstract float MAX_SPEED { get; }

    /// <summary>
    /// Minimum speed of this Projectile.
    /// </summary>
    public abstract float MIN_SPEED { get; }

    /// <summary>
    /// How high this Projectile will be lobbed.
    /// </summary>
    public virtual float LOB_HEIGHT => 2f;

    /// <summary>
    /// Starting damage of this Projectile.
    /// </summary>
    public abstract int BASE_DAMAGE { get; }

    /// <summary>
    /// Maximum damage of this Projectile.
    /// </summary>
    public abstract int MAX_DAMAGE { get; }

    /// <summary>
    /// Minimum damage of this Projectile.
    /// </summary>
    public abstract int MIN_DAMAGE { get; }

    /// <summary>
    /// How many seconds this Projectile can last in the scene.
    /// </summary>
    public abstract float LIFESPAN { get; }

    /// <summary>
    /// The duration of the animation that plays when this Projectile
    /// is mid-air.
    /// </summary>
    public virtual float MID_AIR_ANIMATION_DURATION => .2f;

    /// <summary>
    /// How many seconds a Projectile's move animation lasts,
    /// from start to finish. 
    /// </summary>
    public abstract float MOVE_ANIMATION_DURATION { get; }

    #endregion

    #region Methods

    /// <summary>
    /// Informs this Projectile that it collided with some Model.
    /// </summary>
    /// <param name="collided">true if this Projectile collided with
    /// some Model; otherwise, false.</param>
    public void SetCollided(bool collided) => this.collided = collided;

    /// <summary>
    /// Returns true if this Projectile has collided with something.
    /// </summary>
    /// <returns>true if this Projectile has collided with something;
    /// otherwise, false.</returns>
    public bool Collided() => collided;

    /// <summary>
    /// Informs this Projectile that it reached its positional target.
    /// </summary>
    public void SetReachedTarget() => reachedTarget = true;

    /// <summary>
    /// Returns true if this Projectile reached its positional target.
    /// </summary>
    /// <returns>true if this Projectile reached its positional target;
    /// otherwise, false. </returns>
    public bool HasReachedTarget() => reachedTarget;

    /// <summary>
    /// Returns this Projectile's RigidBody2D component.
    /// </summary>
    /// <returns>this Projectile's RigidBody2D component.</returns>
    public Rigidbody2D GetBody() => projectileBody;

    /// <summary>
    /// Returns this Projectile's current speed.
    /// </summary>
    /// <returns>this Projectile's current speed.</returns>
    public float GetSpeed() => speed;

    /// <summary>
    /// Adds to this Projectile's current speed.
    /// </summary>
    /// <param name="amount">the amount of speed to add.</param>
    public void AdjustSpeed(float amount) => speed += amount;

    /// <summary>
    /// Resets the Projectile's speed to its starting value.
    /// </summary>
    public void ResetSpeed() => speed = BASE_SPEED;

    /// <summary>
    /// Returns this Projectile's current damage.
    /// </summary>
    /// <returns>this Projectile's current damage.</returns>
    public int GetDamage() => damage;

    /// <summary>
    /// Adds to this Projectile's current damage.
    /// </summary>
    /// <param name="amount">the amount of damage to add.</param>
    public void AdjustDamage(int amount) => damage += amount;

    /// <summary>
    /// Resets the Projectile's damange to its starting value.
    /// </summary>
    public void ResetDamage() => damage = BASE_DAMAGE;

    /// <summary>
    /// Returns true if this Projectile is active in the scene.
    /// </summary>
    /// <returns>true if this Projectile is active in the scene; otherwise,
    /// false.</returns>
    public bool IsActive() => active;

    /// <summary>
    /// Sets this Projectile as inactive.
    /// </summary>
    public void SetAsInactive() => active = false;

    /// <summary>
    /// Sets this Projectile as active.
    /// </summary>
    private void SetActive() => active = true;

    /// <summary>
    /// Adds to this Projectile's current age.
    /// </summary>
    /// <param name="time">the amount of time to add.</param>
    public void AddToLifespan(float time) => age = time <= 0 ? age : age + time;

    /// <summary>
    /// Returns true if this Projectile has hit its lifespan.
    /// </summary>
    /// <returns>true if this Projectile has hit its lifespan;
    /// otherwise, false.</returns>
    public bool Expired() => age >= LIFESPAN;

    /// <summary>
    /// Resets this Projectile's age to zero.
    /// </summary>
    private void ResetAge() => age = 0;

    /// <summary>
    /// Returns the number of seconds that this Projectile has been active.
    /// </summary>
    /// <returns>the number of seconds that this Projectile has been active.</returns>
    public float GetAge() => age;

    /// <summary>
    /// Resets this Projectile's stats to their starting
    /// values.
    /// </summary>
    public override void ResetModel()
    {
        base.ResetModel();
        ResetSpeed();
        ResetDamage();
        SetCollided(false);
        SetActive();
        ResetAge();
    }

    /// <summary>
    /// Sets this Projectile's 2D Collider's properties.
    /// </summary>
    public override void SetColliderProperties() { }

    /// <summary>
    /// Sets the position of the Projectile's shadow.
    /// </summary>
    /// <param name="position">The position to set the shadow to.</param>
    public void SetShadowPosition(Vector3 position) => shadow.transform.position = position;

    /// <summary>
    /// Sets the active status of the shadow.
    /// </summary>
    /// <param name="active">true if the shadow should be active; otherwise, false. </param>
    public void SetShadowActive(bool active) => shadow.SetActive(active);

    /// <summary>
    /// Returns a fresh copy of this Projectile from the object pool. 
    /// </summary>
    /// <returns>a fresh copy of this Projectile from the object pool.  </returns>
    public override GameObject CreateNew() => ProjectileFactory.GetProjectilePrefab(TYPE);

    /// <summary>
    /// Returns a Sprite that represents this Projectile when it is
    /// being placed.
    /// </summary>
    /// <returns> a Sprite that represents this Projectile when it is
    /// being placed.</returns>
    public override Sprite[] GetPlacementTrack() => ProjectileFactory.GetPlacementTrack(TYPE);

    #endregion
}
