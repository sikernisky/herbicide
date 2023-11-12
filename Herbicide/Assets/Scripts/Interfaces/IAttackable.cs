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
    /// Amount of attack speed this IAttackable starts with.
    /// </summary>
    float BASE_ATTACK_SPEED { get; }

    /// <summary>
    /// Most amount of attack speed this IAttackable can have.
    /// </summary>
    float MAX_ATTACK_SPEED { get; }

    /// <summary>
    /// Least amount of attack speed this IAttackable can have.
    /// </summary>
    float MIN_ATTACK_SPEED { get; }

    /// <summary>
    /// Directs this IAttackable to face its target. 
    /// </summary>
    void FaceTarget(ITargetable t);

    /// <summary>
    /// Resets this IAttackable's attack range.
    /// </summary>
    void ResetAttackRange();

    /// <summary>
    /// Resets this IAttackable's attack speed.
    /// </summary>
    void ResetAttackSpeed();

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
    /// Returns this IAttackable's current attack range.
    /// </summary>
    /// <returns>this IAttackable's current attack range.</returns>
    float GetAttackSpeed();

    /// <summary>
    /// Sets this IAttackable's positive attack range.
    /// </summary>
    /// <param name="amount">The new attack range.</param>
    void SetAttackSpeed(float amount);
}
