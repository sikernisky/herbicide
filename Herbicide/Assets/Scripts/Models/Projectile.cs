using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Represents something that can be shot and collide with an
/// ITargetable.
/// </summary>
public abstract class Projectile : Model
{
    /// <summary>
    /// Type of this Projectile.
    /// </summary>
    public abstract ProjectileType TYPE { get; }

    //--------------------STATS---------------------//

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
    /// Current speed of this Projectile.
    /// </summary>
    private float speed;

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
    /// Current damage of this Projectile.
    /// </summary>
    private int damage;

    /// <summary>
    /// How many seconds this Projectile can last in the scene.
    /// </summary>
    public abstract float LIFESPAN { get; }

    /// <summary>
    /// How long this Projectile has been active
    /// </summary>
    private float age;

    //----------------------------------------------//

    /// <summary>
    /// How many seconds a Projectile's move animation lasts,
    /// from start to finish. 
    /// </summary>
    public abstract float MOVE_ANIMATION_DURATION { get; }

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
    /// The animation curve of this Projectile's lob.
    /// </summary>
    [SerializeField]
    private AnimationCurve lobCurve;

    /// <summary>
    /// The ITargetable that this Projectile hit; null if it hasn't hit
    /// something yet.
    /// </summary>
    private ITargetable victim;

    /// <summary>
    /// true if this Projectile traveled and reached its target destination;
    /// otherwise, false. Note: some projectiles do not have a "target" and
    /// move forever -- in that case, this is irrelevant. 
    /// </summary>
    private bool reachedTarget;

    /// <summary>
    /// Types of Projectiles.
    /// </summary>
    public enum ProjectileType
    {
        ACORN,
        BOMB
    }

    /// <summary>
    /// Informs this Projectile that it collided with some ITargetable.
    /// </summary>
    /// <param name="victim">The ITargetable this projectile collided with.
    /// </param>
    public void SetCollided(ITargetable victim)
    {
        Assert.IsNotNull(victim, "Victim cannot be null.");
        this.victim = victim;
    }

    /// <summary>
    /// Returns the ITargetable that this Projectile hit. This 
    /// returns null if it hasn't hit anything.  
    /// </summary>
    /// <returns>the ITargetable that this Projectile hit</returns>
    public ITargetable GetVictim() { return victim; }

    /// <summary>
    /// Informs this Projectile that it reached its positional target.
    /// </summary>
    public void SetReachedTarget() { reachedTarget = true; }

    /// <summary>
    /// Returns true if this Projectile reached its positional target.
    /// </summary>
    /// <returns>true if this Projectile reached its positional target;
    /// otherwise, false. </returns>
    public bool HasReachedTarget() { return reachedTarget; }

    /// <summary>
    /// Returns this Projectile's RigidBody2D component.
    /// </summary>
    /// <returns>this Projectile's RigidBody2D component.</returns>
    public Rigidbody2D GetBody() { return projectileBody; }

    /// <summary>
    /// Returns this Projectile's current speed.
    /// </summary>
    /// <returns>this Projectile's current speed.</returns>
    public float GetSpeed() { return speed; }

    /// <summary>
    /// Adds to this Projectile's current speed.
    /// </summary>
    /// <param name="amount">the amount of speed to add.</param>
    public void AdjustSpeed(float amount) { speed += amount; }

    /// <summary>
    /// Resets the Projectile's speed to its starting value.
    /// </summary>
    public void ResetSpeed() { speed = BASE_SPEED; }

    /// <summary>
    /// Returns this Projectile's current damage.
    /// </summary>
    /// <returns>this Projectile's current damage.</returns>
    public int GetDamage() { return damage; }

    /// <summary>
    /// Adds to this Projectile's current damage.
    /// </summary>
    /// <param name="amount">the amount of damage to add.</param>
    public void AdjustDamage(int amount) { damage += amount; }

    /// <summary>
    /// Resets the Projectile's damange to its starting value.
    /// </summary>
    public void ResetDamage() { damage = BASE_DAMAGE; }

    /// <summary>
    /// Returns true if this Projectile is active in the scene.
    /// </summary>
    /// <returns>true if this Projectile is active in the scene; otherwise,
    /// false.</returns>
    public bool IsActive() { return active; }

    /// <summary>
    /// Sets this Projectile as inactive.
    /// </summary>
    public void SetAsInactive() { active = false; }

    /// <summary>
    /// Adds to this Projectile's current age.
    /// </summary>
    /// <param name="time">the amount of time to add.</param>
    public void AddToLifespan(float time) { age = time <= 0 ? age : age + time; }

    /// <summary>
    /// Returns true if this Projectile has hit its lifespan.
    /// </summary>
    /// <returns>true if this Projectile has hit its lifespan;
    /// otherwise, false.</returns>
    public bool Expired() { return age >= LIFESPAN; }

    /// <summary>
    /// Returns this Projectile's animation curve component for
    /// a lob shot. Throws an exception if this projectile does
    /// not support lobbing.
    /// </summary>
    /// <returns>this Projectile's animation curve component for
    /// a lob shot.</returns>
    public AnimationCurve GetLobCurve()
    {
        if (lobCurve == null) throw new System.Exception("This projectile cannot lob.");
        return lobCurve;
    }

    /// <summary>
    /// Resets this Projectile's stats to their starting
    /// values.
    /// </summary>
    public override void ResetStats()
    {
        ResetSpeed();
        ResetDamage();
    }

    /// <summary>
    /// Sets this Projectile's 2D Collider's properties.
    /// </summary>
    public override void SetColliderProperties() { return; }

    /// <summary>
    /// Sets the position of the Projectile's shadow.
    /// </summary>
    /// <param name="position">The position to set the shadow to.</param>
    public void SetShadowPosition(Vector3 position) { shadow.transform.position = position; }

    /// <summary>
    /// Returns the Sprite component that represents this Projectile in
    /// the Inventory.
    /// </summary>
    /// <returns>the Sprite component that represents this Projectile in
    /// the Inventory.</returns>
    public override Sprite GetInventorySprite()
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Returns a Sprite that represents this Projectile when it is
    /// being placed.
    /// </summary>
    /// <returns> a Sprite that represents this Projectile when it is
    /// being placed.</returns>
    public override Sprite GetPlacementSprite()
    {
        throw new System.NotImplementedException();
    }
}
