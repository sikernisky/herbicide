using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents something that can be targeted and attacked.
/// </summary>
public interface ITargetable
{
    /// <summary>
    /// Amount of health this ITargetable starts with.
    /// </summary>
    int BASE_HEALTH { get; }

    /// <summary>
    /// Most amount of health this ITargetable can have.
    /// </summary>
    int MAX_HEALTH { get; }

    /// <summary>
    /// Least amount of health this ITargetable can have.
    /// </summary>
    int MIN_HEALTH { get; }

    /// <summary>
    /// How long to flash when this ITargetable takes damage.
    /// </summary>
    float DAMAGE_FLASH_TIME { get; }

    /// <summary>
    /// Takes some positive damage amount.
    /// </summary>
    /// <param name="amount">The amount of damage to take.</param>
    void TakeDamage(int amount);

    /// <summary>
    /// Returns the health of this ITargetable.
    /// </summary>
    /// <returns>the health of this ITargetable.</returns>
    int GetHealth();

    /// <summary>
    /// Heals some positive health amount.
    /// </summary>
    /// <param name="amount">The amount of health to heal.</param>
    void Heal(int amount);

    /// <summary>
    /// Resets this IAttackable's health to its starting health amount.
    /// </summary>
    void ResetHealth();

    /// <summary>
    /// Returns the position of this ITargetable.
    /// </summary>
    /// <returns>the position of this ITargetable.</returns>
    Vector3 GetPosition();

    /// <summary>
    /// Returns the position where entities attacking this ITargetable
    /// should aim.
    /// </summary>
    /// <returns>the attack position of this ITargetable.</returns>
    Vector3 GetAttackPosition();

    /// <summary>
    /// Spawns this ITargetable, resetting its values.
    /// </summary>
    void ResetStats();

    /// <summary>
    /// Flashes this ITargetable to signal it has taken some damage.
    /// </summary>
    void FlashDamage();

    /// <summary>
    /// Called when this ITargetable dies.
    /// </summary>
    void Die();

    /// <summary>
    /// Returns true if this ITargetable is no longer alive.
    /// </summary>
    /// <returns>true if this ITargetable is dead; otherwise, false.</returns>
    bool Dead();

    /// <summary>
    /// Returns the Collider2D component used by this ITargetable.
    /// </summary>
    /// <returns>the Collider2D component used by this ITargetable.</returns>
    Collider2D GetColllider();
}
