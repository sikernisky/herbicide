using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Abstract class to represent a reactive entity in the game. Mobs
/// implement attack functionality, but they don't need to represent
/// things that can attack. 
/// </summary>
public abstract class Mob : PlaceableObject, IAttackable
{
    //--------------------BEGIN STATS----------------------//

    /// <summary>
    /// Amount of attack range this Mob starts with.
    /// </summary>
    public abstract float BASE_ATTACK_RANGE { get; }

    /// <summary>
    /// Most amount of attack range this Mob can have.
    /// </summary>
    public abstract float MAX_ATTACK_RANGE { get; }

    /// <summary>
    /// Least amount of attack range this Mob can have.
    /// </summary>
    public abstract float MIN_ATTACK_RANGE { get; }

    /// <summary>
    /// This Mob's current attack range. 
    /// </summary>
    private float attackRange;

    /// <summary>
    /// Amount of attack speed this Mob starts with.
    /// </summary>
    public abstract float BASE_ATTACK_SPEED { get; }

    /// <summary>
    /// Most amount of attack speed this Mob can have.
    /// </summary>
    public abstract float MAX_ATTACK_SPEED { get; }

    /// <summary>
    /// Least amount of attack speed this Mob can have.
    /// </summary>
    public abstract float MIN_ATTACK_SPEED { get; }

    /// <summary>
    /// This Mob's current attack speed. 
    /// </summary>
    private float attackSpeed;

    //---------------------END STATS-----------------------//

    /// <summary>
    /// true if this Mob is spawned in the scene.
    /// </summary>
    private bool spawned;


    /// <summary>
    /// Called when this Mob activates in the scene.
    /// </summary>
    public virtual void OnSpawn()
    {
        spawned = true;
        ResetStats();
    }

    /// <summary>
    /// Returns true if this Mob is spawned in the scene.
    /// </summary>
    /// <returns>true if this Mob is spawned in the scene.</returns>
    public bool Spawned() { return spawned; }

    /// <summary>
    /// Returns true if this Mob can attack a target.
    /// </summary>
    /// <param name="target">The ITargetable that this Mob is trying
    /// to attack. </param>
    /// <returns>true if this Mob can attack a target;
    /// otherwise, false.</returns>
    public virtual bool CanAttack(ITargetable target) { return target != null && GetAttackSpeed() > 0; }

    /// <summary>
    /// Returns this Mob's current attack range.
    /// </summary>
    /// <returns>this Mob's current attack range.</returns>
    public float GetAttackRange() { return attackRange; }

    /// <summary>
    /// Sets this IAttackable's attack range.
    /// </summary>
    /// <param name="amount">The new attack range.</param>
    public void SetAttackRange(float amount) { attackRange = Mathf.Clamp(amount, MIN_ATTACK_RANGE, MAX_ATTACK_RANGE); }

    /// <summary>
    /// Resets this Mob's attack range to its starting value.
    /// </summary>
    public void ResetAttackRange() { attackRange = BASE_ATTACK_RANGE; }

    /// <summary>
    /// Returns this Mob's current attack speed.
    /// </summary>
    /// <returns>this Mob's current attack speed.</returns>
    public float GetAttackSpeed() { return attackSpeed; }

    /// <summary>
    /// Sets this IAttackable's attack speed.
    /// </summary>
    /// <param name="amount">The new attack speed.</param>
    public void SetAttackSpeed(float amount) { attackSpeed = Mathf.Clamp(amount, MIN_ATTACK_SPEED, MAX_ATTACK_SPEED); }

    /// <summary>
    /// Resets this Mob's attack speed to its starting value.
    /// </summary>
    public void ResetAttackSpeed() { attackSpeed = BASE_ATTACK_SPEED; }

    /// <summary>
    /// Resets this Mob's stats to their default values.
    /// </summary>
    public override void ResetStats()
    {
        base.ResetStats();
        ResetAttackRange();
        ResetAttackSpeed();
    }

    /// <summary>
    /// Changes this Mob's direction such that it faces some ITargetable.
    /// </summary>
    /// <param name="t">The ITargetable to face.</param>
    public void FaceTarget(ITargetable t)
    {
        Assert.IsNotNull(t, "Cannot face a null target.");

        float xDistance = GetPosition().x - t.GetPosition().x;
        float yDistance = GetPosition().y - t.GetPosition().y;
        bool xGreater = Mathf.Abs(xDistance) > Mathf.Abs(yDistance)
            ? true : false;

        if (xGreater && xDistance <= 0) FaceDirection(Direction.EAST);
        if (xGreater && xDistance > 0) FaceDirection(Direction.WEST);
        if (!xGreater && yDistance <= 0) FaceDirection(Direction.NORTH);
        if (!xGreater && yDistance > 0) FaceDirection(Direction.SOUTH);
    }
}
