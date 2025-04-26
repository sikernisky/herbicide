using UnityEngine;

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
    public float Speed { get; private set; }

    /// <summary>
    /// Current damage of this Projectile.
    /// </summary>
    public int Damage { get; private set; }

    /// <summary>
    /// How long this Projectile has been active
    /// </summary>
    public float Age { get; private set; }

    /// <summary>
    /// true if this Projectile is active in the scene.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// true if this Projectile has hit something; otherwise, false. 
    /// </summary>
    public bool HasCollided { get; private set; }

    /// <summary>
    /// The number of times this Projectile will split upon impact.
    /// </summary>
    public int NumSplits { get; private set; }

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
    /// Starting speed of this Projectile.
    /// </summary>
    public abstract float BaseSpeed { get; }

    /// <summary>
    /// Maximum speed of this Projectile.
    /// </summary>
    public abstract float MaxSpeed { get; }

    /// <summary>
    /// Minimum speed of this Projectile.
    /// </summary>
    public abstract float MinSpeed { get; }

    /// <summary>
    /// Starting damage of this Projectile.
    /// </summary>
    public abstract int BaseDamage { get; }

    /// <summary>
    /// Maximum damage of this Projectile.
    /// </summary>
    public abstract int MaxDamage { get; }

    /// <summary>
    /// Minimum damage of this Projectile.
    /// </summary>
    public abstract int MinDamage { get; }

    /// <summary>
    /// How many seconds this Projectile can last in the scene.
    /// </summary>
    public abstract float Lifespan { get; }

    /// <summary>
    /// How many seconds a Projectile's move animation lasts,
    /// from start to finish. 
    /// </summary>
    public abstract float MovementAnimationDuration { get; }

    /// <summary>
    /// How high this Projectile will be lobbed.
    /// </summary>
    public virtual float LobHeight => ModelStatConstants.ProjectileLobHeight;

    /// <summary>
    /// The duration of the animation that plays when this Projectile
    /// is mid-air.
    /// </summary>
    public virtual float MidAirAnimationDuration => AnimationConstants.ProjectileMidAirAnimationDuration;

    #endregion

    #region Methods

    /// <summary>
    /// Informs this Projectile that it collided with some Model.
    /// </summary>
    /// <param name="collided">true if this Projectile collided with
    /// some Model; otherwise, false.</param>
    public void SetCollided(bool collided)
    {
        HasCollided = collided;
        ModelCollider.enabled = !collided;
    }

    /// <summary>
    /// Resets the Projectile's speed to its starting value.
    /// </summary>
    public void ResetSpeed() => Speed = BaseSpeed;

    /// <summary>
    /// Adds to this Projectile's current age.
    /// </summary>
    /// <param name="time">the amount of time to add.</param>
    public void AddToLifespan(float time) => Age = time <= 0 ? Age : Age + time;

    /// <summary>
    /// Returns true if this Projectile has hit its lifespan.
    /// </summary>
    /// <returns>true if this Projectile has hit its lifespan;
    /// otherwise, false.</returns>
    public bool Expired() => Age >= Lifespan;

    /// <summary>
    /// Resets this Projectile's age to zero.
    /// </summary>
    private void ResetAge() => Age = 0;

    /// <summary>
    /// Resets this Projectile's stats to their starting
    /// values.
    /// </summary>
    public override void ResetModel()
    {
        base.ResetModel();
        ResetSpeed();
        Damage = BaseDamage;
        SetCollided(false);
        SetNumSplits(0);
        IsActive = true;
        ResetAge();
    }

    /// <summary>
    /// Sets the number of times this Projectile will split upon impact.
    /// </summary>
    /// <param name="newNumSplits">the number of times this Projectile will split upon impact.</param>
    public void SetNumSplits(int newNumSplits) => NumSplits = Mathf.Clamp(newNumSplits, 0, int.MaxValue);

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
    /// Clones this Projectile from another Projectile.
    /// </summary>
    /// <param name="m">The Model to clone from.</param>
    public override void CloneFrom(Model m)
    {
        base.CloneFrom(m);
        if(m is not Projectile p) return;
        Speed = p.Speed;
        Damage = p.Damage;
        NumSplits = p.NumSplits;
    }

    /// <summary>
    /// Returns a Sprite that represents this Projectile when it is
    /// being placed.
    /// </summary>
    /// <returns> a Sprite that represents this Projectile when it is
    /// being placed.</returns>
    public override Sprite[] GetPlacementTrack() => ProjectileFactory.GetPlacementTrack(this);

    #endregion
}
