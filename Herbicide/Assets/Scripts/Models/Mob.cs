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
    /// Amount of attack cooldown this Mob starts with.
    /// </summary>
    public abstract float BASE_ATTACK_COOLDOWN { get; }

    /// <summary>
    /// Most amount of attack cooldown this Mob can have.
    /// </summary>
    public abstract float MAX_ATTACK_COOLDOWN { get; }

    /// <summary>
    /// Least amount of attack cooldown this Mob can have.
    /// </summary>
    public float MIN_ATTACK_COOLDOWN => 0f;

    /// <summary>
    /// This Mob's current attack cooldown.
    /// </summary>
    private float attackCooldown;

    /// <summary>
    /// Amount of chase range this Mob starts with.
    /// </summary>
    public abstract float BASE_CHASE_RANGE { get; }

    /// <summary>
    /// Most amount of chase range this Mob can have.
    /// </summary>
    public abstract float MAX_CHASE_RANGE { get; }

    /// <summary>
    /// Least amount of chase range this Mob can have.
    /// </summary>
    public abstract float MIN_CHASE_RANGE { get; }

    /// <summary>
    /// This Mob's current chase range. 
    /// </summary>
    private float chaseRange;

    /// <summary>
    /// This Mob's current attack speed. 
    /// </summary>
    private float attackSpeed;

    /// <summary>
    /// Amount of movement speed this Mob starts with.
    /// </summary>
    public abstract float BASE_MOVEMENT_SPEED { get; }

    /// <summary>
    /// Max amount of movement speed this Mob starts with.
    /// </summary>
    public abstract float MAX_MOVEMENT_SPEED { get; }

    /// <summary>
    /// Min amount of movement speed this Mob starts with.
    /// </summary>
    public abstract float MIN_MOVEMENT_SPEED { get; }

    /// <summary>
    /// This Mob's current movement speed.
    /// </summary>
    private float movementSpeed;


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
    /// Returns true if this Mob is dead. This is when the Mob
    /// has a non-positive health value and has already been
    /// spawned.
    /// </summary>
    /// <returns>true if this Mob is dead; otherwise, false.</returns>
    public override bool Dead() { return base.Dead() && spawned; }

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
    /// Sets this Mob's attack range.
    /// </summary>
    /// <param name="amount">The new attack range.</param>
    public void SetAttackRange(float amount)
    {
        attackRange = Mathf.Clamp(amount, MIN_ATTACK_RANGE, MAX_ATTACK_RANGE);
    }

    /// <summary>
    /// Resets this Mob's attack range to its starting value.
    /// </summary>
    public void ResetAttackRange() { attackRange = BASE_ATTACK_RANGE; }

    /// <summary>
    /// Returns this Mob's current attack cooldown.
    /// </summary>
    /// <returns>this Mob's current attack cooldown.</returns>
    public float GetAttackCooldown() { return attackCooldown; }

    /// <summary>
    /// Sets this Mob's attack cooldown.
    /// </summary>
    /// <param name="amount">The new attack cooldown.</param>
    public void SetAttackCooldown(float amount)
    {
        attackCooldown = Mathf.Clamp(amount, MIN_ATTACK_COOLDOWN, MAX_ATTACK_COOLDOWN);
    }

    /// <summary>
    /// Resets this Mob's attack cooldown to its starting value.
    /// </summary>
    public void ResetAttackCooldown() { attackCooldown = BASE_ATTACK_COOLDOWN; }

    /// <summary>
    /// Returns this Mob's current chase range.
    /// </summary>
    /// <returns>this Mob's current chase range.</returns>
    public virtual float GetChaseRange() { return chaseRange; }

    /// <summary>
    /// Sets this Mob's chase range.
    /// </summary>
    /// <param name="amount">The new chase range.</param>
    public void SetChaseRange(float amount)
    {
        chaseRange = Mathf.Clamp(amount, MIN_CHASE_RANGE, MAX_CHASE_RANGE);
    }

    /// <summary>
    /// Resets this Mob's chase range to its starting value.
    /// </summary>
    public void ResetChaseRange() { chaseRange = BASE_CHASE_RANGE; }

    /// <summary>
    /// Returns this Mob's current attack speed.
    /// </summary>
    /// <returns>this Mob's current attack speed.</returns>
    public float GetAttackSpeed() { return attackSpeed; }

    /// <summary>
    /// Returns this Mob's current movement speed.
    /// </summary>
    /// <returns>this Mob's current movement speed.</returns>
    public float GetMovementSpeed() { return movementSpeed; }

    /// <summary>
    /// Sets this IAttackable's movement speed.
    /// </summary>
    /// <param name="amount">The new movement speed.</param>
    public void SetMovementSpeed(float amount)
    {
        movementSpeed = Mathf.Clamp(amount, MIN_MOVEMENT_SPEED, MAX_MOVEMENT_SPEED);
    }

    /// <summary>
    /// Resets this Mob's movement speed to its starting value.
    /// </summary>
    public void ResetMovementSpeed() { movementSpeed = BASE_MOVEMENT_SPEED; }

    /// <summary>
    /// Resets this Mob's stats to their default values.
    /// </summary>
    public override void ResetStats()
    {
        base.ResetStats();
        ResetAttackRange();
        ResetChaseRange();
        ResetMovementSpeed();
        SetAttackCooldown(0);
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

    /// <summary>
    /// Changes this Mob's direction such that it faces some position.
    /// </summary>
    /// <param name="t">The position to face.</param>
    public void FaceTarget(Vector3 t)
    {
        float xDistance = GetPosition().x - t.x;
        float yDistance = GetPosition().y - t.y;
        bool xGreater = Mathf.Abs(xDistance) > Mathf.Abs(yDistance)
            ? true : false;

        if (xGreater && xDistance <= 0) FaceDirection(Direction.EAST);
        if (xGreater && xDistance > 0) FaceDirection(Direction.WEST);
        if (!xGreater && yDistance <= 0) FaceDirection(Direction.NORTH);
        if (!xGreater && yDistance > 0) FaceDirection(Direction.SOUTH);
    }
}
