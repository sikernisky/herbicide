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
    /// Adds some amount (can be negative) of health to this ITargetable.
    /// </summary>
    /// <param name="amount">The amount of health to adjust.</param>
    void AdjustHealth(int amount);

    /// <summary>
    /// Returns the health of this ITargetable.
    /// </summary>
    /// <returns>the health of this ITargetable.</returns>
    int GetHealth();

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
    /// Returns this ITargetable's Transform component.
    /// </summary>
    /// <returns>this ITargetable's Transform component.</returns>
    Transform GetTransform();

    /// <summary>
    /// Returns the position where entities attacking this ITargetable
    /// should aim.
    /// </summary>
    /// <returns>the attack position of this ITargetable.</returns>
    Vector3 GetAttackPosition();

    /// <summary>
    /// Starts the Damage Flash animation by resetting the amount of time
    /// left in the animation to the total amount of time it takes to
    /// complete one animation cycle/// 
    /// </summary>
    void FlashDamage();

    /// <summary>
    /// Returns the amount of time that remains in this ITargetable's
    /// flash animation. 
    /// </summary>
    /// <returns>the amount of time that remains in this ITargetable's
    /// flash animation</returns>
    float TimeRemaningInFlashAnimation();

    /// <summary>
    /// Sets the amount of time  this ITargetable's has left 
    /// in its flash animation.
    /// </summary>
    /// <param name="value">The new amount of time that this ITargetable
    /// has left in its flash animation. .</param>
    void SetRemainingFlashAnimationTime(float value);

    /// <summary>
    /// Called when this ITargetable dies.
    /// </summary>
    void OnDie();

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

    /// <summary>
    /// Returns the Euclidian distance from this ITargetable to another ITargetable.
    /// </summary>
    /// <param name="target">The ITargetable from which to calculate distance.</param>
    float DistanceToTarget(ITargetable target);

    /// <summary>
    /// Resets this ITargetable's stats to their default values.
    /// </summary>
    void ResetStats();

    /// <summary>
    /// Returns true if something can target this ITargetable
    /// </summary>
    /// <returns>true if something can target this ITargetable; otherwise, false.
    /// /// </returns>
    bool Targetable();
}
