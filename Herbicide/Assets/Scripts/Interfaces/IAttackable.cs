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
    /// Amount of damage done by this IAttackable each attack.
    /// </summary>
    int DAMAGE_PER_ATTACK { get; }

    /// <summary>
    /// Attacks another IAttackable.
    /// </summary>
    /// <param name="target">the IAttackable to attack.</param>
    void Attack(ITargetable target);

    /// <summary>
    /// Resets this IAttackable's attack range.
    /// </summary>
    void ResetAttackRange();

    /// <summary>
    /// Resets this IAttackable's attack speed.
    /// </summary>
    void ResetAttackSpeed();

    /// <summary>
    /// Returns true if this IAttackable can attack another IAttackable at
    /// the current frame.
    /// </summary>
    /// <param name="target">the target to try to attack.</param>
    /// <returns>true if this IAttackable can attack another IAttackable at
    /// the current frame.</returns>
    bool CanAttackNow(ITargetable target);

    /// <summary>
    /// Returns true if this IAttackable can ever attack another IAttackable.
    /// </summary>
    /// <param name="target">the target to try to attack.</param>
    /// <returns>true if this IAttackable can ever attack another IAttackable.
    bool CanAttackEver(ITargetable target);

    /// <summary>
    /// Returns the Euclidian distance from this IAttackable to another IAttackable.
    /// </summary>
    /// <param name="target">The other IAttackable.</param>
    float DistanceToTarget(ITargetable target);

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
