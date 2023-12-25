using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.Linq;

/// <summary>
/// Represents some entity whose goal is to make the player
/// lose the current level.
/// </summary>
public abstract class Enemy : Mob
{

    /// <summary>
    /// The chance, between 0-1, that this Enemy drops
    /// its loot upon death. 
    /// </summary>
    public abstract float LOOT_DROP_CHANCE { get; }

    /// <summary>
    /// The world position where this Enemy spawns.
    /// </summary>
    private Vector3 spawnPosition;

    /// <summary>
    /// The time at which this Enemy spawns.
    /// </summary>
    private float spawnTime;

    /// <summary>
    /// Current health state of this Enemy.
    /// </summary>
    private EnemyHealthState healthState;

    /// <summary>
    /// true if the Enemy escaped; otherwise, false.
    /// </summary>
    private bool escaped;

    /// <summary>
    /// Type of this Enemy.
    /// </summary>
    public enum EnemyType
    {
        KUDZU
    }

    /// <summary>
    /// State of an Enemy based on its health.
    /// </summary>
    public enum EnemyHealthState
    {
        HEALTHY,
        DAMAGED,
        CRITICAL
    }

    /// <summary>
    /// Resets this Enemy's position to be its spawn position.
    /// </summary>
    private void ResetPosition() { transform.position = spawnPosition; }

    /// <summary>
    /// Called when this Enemy appears on the TileGrid and is assigned
    /// an EnemyController (or a subclass of EnemyController).
    /// </summary>
    public override void OnSpawn()
    {
        Assert.IsTrue(ReadyToSpawn(), "Not Ready.");
        base.OnSpawn();
    }

    /// <summary>
    /// Returns true if this Enemy is due to spawn and it has not yet spawned.
    /// </summary>
    /// <returns>true if this Enemy is due to spawn and it has not yet spawned;
    /// otherwise, returns false. /// </returns>
    public bool ReadyToSpawn() { return !Spawned() && SceneController.GetTimeElapsed() >= GetSpawnTime(); }

    /// <summary>
    /// Updates this Enemy's HealthState.
    /// </summary>
    public void UpdateHealthState()
    {
        float currentPercent = (float)GetHealth() / (float)MAX_HEALTH;
        if (currentPercent >= .66f) healthState = EnemyHealthState.HEALTHY;
        else if (currentPercent >= .33f) healthState = EnemyHealthState.DAMAGED;
        else healthState = EnemyHealthState.CRITICAL;
    }

    /// <summary>
    /// Returns the current HealthState of this Enemy.
    /// </summary>
    /// <returns>the current HealthState of this Enemy.</returns>
    public EnemyHealthState GetHealthState() { return healthState; }

    /// <summary>
    /// Sets the SpawnTime of this Enemy.
    /// </summary>
    /// <param name="time">the time at which this Enemy spawns.</param>
    public void SetSpawnTime(float time) { spawnTime = Mathf.Clamp(time, 0, float.MaxValue); }

    /// <summary>
    /// Returns this Enemy's SpawnTime.
    /// </summary>
    /// <returns>this Enemy's SpawnTime.</returns>
    public float GetSpawnTime() { return spawnTime; }

    /// <summary>
    /// Sets the (X, Y) world coordinates where this Enemy spawns in the level.
    /// </summary>
    /// <param name="coords">the (X, Y) coordinates.</param>
    public void SetSpawnWorldPosition(Vector3 position) { spawnPosition = position; }

    /// <summary>
    /// Returns the (X, Y) world position where this Enemy spawns in the level.
    /// </summary>
    /// <returns>the (X, Y) world position where this Enemy spawns.</returns>
    public Vector3 GetSpawnWorldPosition() { return spawnPosition; }

    /// <summary>
    /// Returns true if this Enemy escaped.
    /// </summary>
    /// <returns>true if this Enemy escaped; otherwise, false. </returns>
    public bool Escaped() { return escaped; }

    /// <summary>
    /// Sets this Enemy as escaped.
    /// </summary>
    public void SetEscaped() { escaped = true; }
}
