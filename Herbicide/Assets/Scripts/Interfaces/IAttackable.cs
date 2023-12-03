using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents something that can attack an ITargetable
/// and take damage.
/// </summary>
public interface IAttackable : ITargetable
{
    /// <summary>
    /// Amount of attack range this IAttackable starts with.
    /// </summary>
    float BASE_ATTACK_RANGE { get; }

    /// <summary>
    /// Most amount of attack range this IAttackable can have.
    /// </summary>
    float MAX_ATTACK_RANGE { get; }

    /// <summary>
    /// Least amount of attack range this IAttackable can have.
    /// </summary>
    float MIN_ATTACK_RANGE { get; }

    /// <summary>
    /// Amount of attack cooldown this IAttackable starts with.
    /// </summary>
    float BASE_ATTACK_COOLDOWN { get; }

    /// <summary>
    /// Most amount of attack cooldown this IAttackable can have.
    /// </summary>
    float MAX_ATTACK_COOLDOWN { get; }

    /// <summary>
    /// Least amount of attack cooldown this IAttackable can have.
    /// </summary>
    float MIN_ATTACK_COOLDOWN { get; }

    /// <summary>
    /// Directs this IAttackable to face its target. 
    /// </summary>
    void FaceTarget(ITargetable t);

    /// <summary>
    /// Resets this IAttackable's attack range.
    /// </summary>
    void ResetAttackRange();

    /// <summary>
    /// Resets this IAttackable's attack cooldown.
    /// </summary>
    void ResetAttackCooldown();

    /// <summary>
    /// Returns true if this IAttackable is allowed to attack some
    /// ITargetable.
    /// </summary>
    /// <param name="target">the target to try to attack.</param>
    /// <returns>true if this IAttackable is allowed to attack the
    /// given ITargetable.
    bool CanAttack(ITargetable target);

    /// <summary>
    /// Returns this IAttackable's current attack range.
    /// </summary>
    /// <returns>this IAttackable's current attack range.</returns>
    float GetAttackRange();

    /// <summary>
    /// Sets this IAttackable's positive attack range.
    /// </summary>
    /// <param name="amount">The new attack range.</param>
    void SetAttackRange(float amount);

    /// <summary>
    /// Returns this IAttackable's current attack cooldown.
    /// </summary>
    /// <returns>this IAttackable's current attack cooldown.</returns>
    float GetAttackCooldown();

    /// <summary>
    /// Sets this IAttackable's positive attack cooldown.
    /// </summary>
    /// <param name="amount">The new attack cooldown.</param>
    void SetAttackCooldown(float amount);
}
