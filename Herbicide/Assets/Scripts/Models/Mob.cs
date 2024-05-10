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
public abstract class Mob : PlaceableObject
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
    /// This Mob's current attack cooldown cap.
    /// </summary>
    private float attackCooldownCap;

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
    /// By default, Mobs do not occupy Tiles.
    /// </summary>
    public override bool OCCUPIER => false;

    /// <summary>
    /// true if this Mob is spawned in the scene.
    /// </summary>
    private bool spawned;

    /// <summary>
    /// Where this mob spawned.
    /// </summary>
    private Vector3 spawnPos;


    /// <summary>
    /// Called when this Mob activates in the scene.
    /// </summary>
    public virtual void OnSpawn()
    {
        spawned = true;
        ResetModel();
    }

    /// <summary>
    /// Returns this Mob's spawn position.
    /// </summary>
    /// <returns>the position where this Mob spawned.</returns>
    public Vector3 GetSpawnPos() { return spawnPos; }

    /// <summary>
    /// Sets where this Mob spawns.
    /// </summary>
    /// <param name="spawnPos">Where the mob spawns. </param>
    public void SetSpawnPos(Vector3 spawnPos) { this.spawnPos = spawnPos; }

    /// <summary>
    /// Returns true if this Mob is dead. This is when the Mob
    /// has a non-positive health value and has already been
    /// spawned.
    /// </summary>
    /// <returns>true if this Mob is dead; otherwise, false.</returns>
    public override bool Dead() { return GetHealth() <= 0 && spawned; }

    /// <summary>
    /// Returns true if this Mob is spawned in the scene.
    /// </summary>
    /// <returns>true if this Mob is spawned in the scene.</returns>
    public bool Spawned() { return spawned; }

    /// <summary>
    /// Returns true if this Mob can attack a target.
    /// </summary>
    /// <param name="target">The Mob that this Mob is trying
    /// to attack. </param>
    /// <returns>true if this Mob can attack a target;
    /// otherwise, false.</returns>
    public virtual bool CanAttack(Mob target) { return target != null; }

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
    /// Adds some amount to this Mob's attack cooldown.
    /// </summary>
    /// <param name="amount">The new attack cooldown.</param>
    public void AdjustAttackCooldown(float amount)
    {
        attackCooldown = Mathf.Clamp(GetAttackCooldown() + amount,
            MIN_ATTACK_COOLDOWN, attackCooldownCap);
    }

    /// <summary>
    /// Adds some amount to this Mob's attack cooldown cap
    /// </summary>
    /// <param name="amount">The new attack cooldown cap.</param>
    public void AdjustAttackCooldownCap(float amount)
    {
        attackCooldownCap = Mathf.Clamp(attackCooldownCap + amount,
            MIN_ATTACK_COOLDOWN, MAX_ATTACK_COOLDOWN);
    }

    /// <summary>
    /// Returns this Mob's attack cooldown cap.
    /// </summary>
    /// <returns>this Mob's attack cooldown cap.</returns>
    public float GetAttackCooldownCap() { return attackCooldownCap; }

    /// <summary>
    /// Resets this Mob's attack cooldown to its currently capped value.
    /// </summary>
    public void ResetAttackCooldown() { attackCooldown = attackCooldownCap; }

    /// <summary>
    /// Resets this Mob's attack cooldown to its starting value.
    /// </summary>
    public void ResetAttackCooldownToBase() { attackCooldownCap = BASE_ATTACK_COOLDOWN; }

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
    /// Returns this Mob's current movement speed.
    /// </summary>
    /// <returns>this Mob's current movement speed.</returns>
    public float GetMovementSpeed() { return movementSpeed; }

    /// <summary>
    /// Adds some amount to this Mob's movement speed.
    /// </summary>
    /// <param name="amount">The amount to add.</param>
    public virtual void AdjustMovementSpeed(float amount)
    {
        movementSpeed = Mathf.Clamp(movementSpeed + amount,
            MIN_MOVEMENT_SPEED, MAX_MOVEMENT_SPEED);
    }

    /// <summary>
    /// Resets this Mob's movement speed to its starting value.
    /// </summary>
    public void ResetMovementSpeed() { movementSpeed = BASE_MOVEMENT_SPEED; }

    /// <summary>
    /// Resets this Mob's stats to their default values.
    /// </summary>
    public override void ResetModel()
    {
        base.ResetModel();
        ResetAttackRange();
        ResetChaseRange();
        ResetMovementSpeed();
        ResetAttackCooldownToBase();
        AdjustAttackCooldown(0);
    }



    /// <summary>
    /// Changes this Mob's direction such that it faces some Model.
    /// </summary>
    /// <param name="target">The Model to face.</param>
    public void FaceTarget(Model target)
    {
        Assert.IsNotNull(target, "Cannot face a null target.");

        float xDistance = GetPosition().x - target.GetPosition().x;
        float yDistance = GetPosition().y - target.GetPosition().y;
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
