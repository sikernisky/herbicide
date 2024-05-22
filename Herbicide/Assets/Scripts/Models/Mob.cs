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
    /// Starting number of attacks this Mob can make per second.
    /// </summary>
    public abstract float BASE_ATTACK_SPEED { get; }

    /// <summary>
    /// Max number of attacks this Mob can make per second.
    /// </summary>
    public abstract float MAX_ATTACK_SPEED { get; }

    /// <summary>
    /// Min number of attacks this Mob can make per second.
    /// </summary>
    public float MIN_ATTACK_SPEED => 0f;

    /// <summary>
    /// Current number of attacks this Mob can make per second.
    /// </summary>
    private float attackSpeed;

    /// <summary>
    /// How many seconds this Mob has to wait before attacking again.
    /// </summary>
    private float attackCooldownTimer;

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
    public virtual void OnSpawn() { spawned = true; }

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
    /// Resets this Mob's attack range to its starting value.
    /// </summary>
    public void ResetAttackRange() { attackRange = BASE_ATTACK_RANGE; }

    /// <summary>
    /// Returns the number of seconds this Mob has to wait before
    /// attacking again.
    /// </summary>
    /// <returns>the number of seconds this Mob has to wait before
    /// attacking again.</returns>
    public float GetAttackCooldown() { return attackCooldownTimer; }

    /// <summary>
    /// Reduces this Mob's attack cooldown by Time.deltaTime.
    /// </summary>
    public void StepAttackCooldown()
    {
        attackCooldownTimer = Mathf.Clamp(GetAttackCooldown() - Time.deltaTime,
            MIN_ATTACK_SPEED, 1f / attackSpeed);
    }

    /// <summary>
    /// Sets the number of attacks this Mob can make per second.
    /// </summary>
    public void SetAttackSpeed(float attackSpeed) { this.attackSpeed = attackSpeed; }  

    /// <summary>
    /// Resets this Mob's attack cooldown to its currently capped value.
    /// </summary>
    public void RestartAttackCooldown() { attackCooldownTimer = 1f / attackSpeed; }

    /// <summary>
    /// Resets this Mob's attack speed to its starting value.
    /// </summary>
    public void ResetAttackSpeed() { attackSpeed = BASE_ATTACK_SPEED; }

    /// <summary>
    /// Returns this Mob's current chase range.
    /// </summary>
    /// <returns>this Mob's current chase range.</returns>
    public virtual float GetChaseRange() { return chaseRange; }

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
        ResetAttackSpeed();
    }
}
