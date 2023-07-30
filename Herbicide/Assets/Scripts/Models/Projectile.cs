using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Represents something that can be shot and collide with an
/// IAttackable.
/// </summary>
public class Projectile : MonoBehaviour
{
    /// <summary>
    /// Type of a Projectile.
    /// </summary>
    public enum ProjectileType
    {
        ACORN
    }

    /// <summary>
    /// Speed of this Projectile.
    /// </summary>
    [SerializeField]
    private float speed;

    /// <summary>
    /// Type of this Projectile.
    /// </summary>
    [SerializeField]
    private ProjectileType type;

    /// <summary>
    /// How long this Projectile lasts, in seconds.
    /// </summary>
    [SerializeField]
    private float lifespan;

    /// <summary>
    /// How much damage this Projectile does per hit.
    /// </summary>
    [SerializeField]
    private int damage;

    /// <summary>
    /// true if this Projectile is active in the scene.
    /// </summary>
    private bool active = true;

    /// <summary>
    /// How long this Projectile has been active
    /// </summary>
    private float age;

    /// <summary>
    /// Projectile's RigidBody component.
    /// </summary>
    [SerializeField]
    private Rigidbody2D projectileBody;


    /// <summary>
    /// Returns the speed of this Projectile.
    /// </summary>
    /// <returns>the speed of this Projectile.</returns>
    public float GetSpeed()
    {
        return speed;
    }

    /// <summary>
    /// Returns this Projectile's RigidBody2D component.
    /// </summary>
    /// <returns>this Projectile's RigidBody2D component.</returns>
    public Rigidbody2D GetBody()
    {
        return projectileBody;
    }

    /// <summary>
    /// Returns true if this Projectile is active in the scene.
    /// </summary>
    /// <returns>true if this Projectile is active in the scene; otherwise,
    /// false.</returns>
    public bool IsActive()
    {
        return active;
    }

    /// <summary>
    /// Sets this Projectile as inactive.
    /// </summary>
    public void SetAsInactive()
    {
        active = false;
    }

    /// <summary>
    /// Returns the damage dealt by this Projectile.
    /// </summary>
    /// <returns>the damage dealt by this Projectile.</returns>
    public int GetDamage()
    {
        return damage;
    }

    /// <summary>
    /// Adds to this Projectile's current age.
    /// </summary>
    /// <param name="time">the amount of time to add.</param>
    public void AddToLifespan(float time)
    {
        if (time <= 0) return;
        age += time;
    }

    /// <summary>
    /// Returns true if this Projectile has hit its lifespan.
    /// </summary>
    /// <returns>true if this Projectile has hit its lifespan;
    /// otherwise, false.</returns>
    public bool Expired()
    {
        return age >= lifespan;
    }
}
