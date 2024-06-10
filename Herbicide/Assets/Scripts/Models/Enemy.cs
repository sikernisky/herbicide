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
    /// Reference to the Enemy's health bar background SpriteRenderer component.
    /// </summary>
    [SerializeField]
    private SpriteRenderer healthBarBackground;

    /// <summary>
    /// Reference to the Enemy's health bar SpriteRenderer component.
    /// </summary> 
    [SerializeField]
    private SpriteRenderer healthBarFill;

    /// <summary>
    /// The time at which this Enemy spawns.
    /// </summary>
    private float spawnTime;

    /// <summary>
    /// Current health state of this Enemy.
    /// </summary>
    private EnemyHealthState healthState;

    /// <summary>
    /// true if the Enemy exited; otherwise, false.
    /// </summary>
    private bool exited;

    /// <summary>
    /// true if the Enemy is exiting; otherwise, false.
    /// </summary>
    private bool exiting;

    /// <summary>
    /// The position of the structure the Enemy is using to exit.
    /// </summary>
    private Vector3 exitPos;

    /// <summary>
    /// How much a currency collectable dropped by this Enemy on death
    /// is worth.
    /// </summary>
    public virtual int CURRENCY_VALUE_ON_DEATH => 25;

    /// <summary>
    /// true if this Enemy is currently spawning; otherwise, false.
    /// </summary>
    private bool spawning;

    /// <summary>
    /// Set of active DamageOverTime effects on this Enemy.
    /// </summary>
    private HashSet<DamageOverTime> activeDOTs;

    /// <summary>
    /// The DamageOverTime effects applied to this Enemy this cycle.
    /// </summary>
    private HashSet<DamageOverTime.DOTType> appliedDOTsThisCycle;

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
    /// Called when this Enemy appears on the TileGrid and is assigned
    /// an EnemyController (or a subclass of EnemyController).
    /// </summary>
    public override void OnSpawn()
    {
        Assert.IsTrue(ReadyToSpawn(), "Not Ready.");
        base.OnSpawn();

        activeDOTs = new HashSet<DamageOverTime>();
        appliedDOTsThisCycle = new HashSet<DamageOverTime.DOTType>();
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
    /// Returns true if this Enemy exited.
    /// </summary>
    /// <returns>true if this Enemy exited; otherwise, false. </returns>
    public bool Exited() { return exited; }

    /// <summary>
    /// Sets this Enemy as exited.
    /// </summary>
    public void SetExited()
    {
        exiting = false;
        exited = true;
    }

    /// <summary>
    /// Sets the Enemy as exiting, but not yet exited.
    /// </summary>
    /// <param name="exitPos">The position of the structure the
    /// Enemy is using to exit. /// </param>
    public void SetExiting(Vector3 exitPos)
    {
        exited = false;
        exiting = true;
        this.exitPos = exitPos;
    }

    /// <summary>
    /// Returns true if this Enemy is exiting.
    /// </summary>
    /// <returns>true if this Enemy is exiting; otherwise, false. </returns>
    public bool IsExiting() { return exiting; }

    /// <summary>
    /// Returns the position of the structure the Enemy is
    /// using to exit.
    /// </summary>
    /// <returns>the position of the structure the Enemy is
    /// using to exit.</returns>
    public Vector3 GetExitPos() { return exitPos; }

    /// <summary>
    /// Sets the Enemy as spawning.
    /// </summary>
    public void SetSpawning(bool spawning) { this.spawning = spawning; }

    /// <summary>
    /// Returns true if this Enemy is currently spawning.
    /// </summary>
    /// <returns>true if this Enemy is currently spawning; otherwise, false. </returns>
    public bool IsSpawning() { return spawning; }

    /// <summary>
    /// Returns true if this Enemy is targetable.
    /// </summary>
    /// <returns>true if this Enemy is targetable; otherwise, false. </returns>
    public override bool Targetable()
    {
        return base.Targetable() && !IsSpawning();
    }

    /// <summary>
    /// Adjusts this Mob's health by some amount. Updates the health bar.
    /// </summary>
    /// <param name="amount">The amount to adjust by. </param>
    public override void AdjustHealth(float amount)
    {
        base.AdjustHealth(amount);
        float healthPercentage = (float)GetHealth() / BASE_HEALTH;
        healthBarFill.transform.localScale = new Vector3(healthPercentage, 1, 1);
    }

    /// <summary>
    /// Adjusts this Mob's health by some amount over time. Updates the health bar.
    /// </summary>
    /// <param name="amount">The amount to adjust by. </param>
    public void AdjustHealth(DamageOverTime dot)
    {
        Assert.IsNotNull(dot, "DamageOverTime is null.");

        activeDOTs.Add(dot);
    }

    /// <summary>
    /// Updates the DamageOverTime effects on this Enemy.
    /// </summary>
    public void UpdateDamageOverTimeEffects()
    {
        if (activeDOTs == null) activeDOTs = new HashSet<DamageOverTime>();
        if (appliedDOTsThisCycle == null) appliedDOTsThisCycle = new HashSet<DamageOverTime.DOTType>();

        foreach (DamageOverTime dot in activeDOTs)
        {
            bool shouldAdjustHealth = dot.UpdateDamageOverTime();

            if (!dot.DoesStack())
            {
                if (appliedDOTsThisCycle.Contains(dot.GetDOTType())) continue;
                appliedDOTsThisCycle.Add(dot.GetDOTType());
            }

            if (shouldAdjustHealth) AdjustHealth(-dot.GetDamage());
        }

        activeDOTs.RemoveWhere(dot => dot.IsFinished());
        appliedDOTsThisCycle.Clear();
    }

    /// <summary>
    /// Toggle the visibility of the Enemy's health bar.
    /// </summary>
    public void ToggleHealthBar(bool showHealthBar)
    {
        Assert.IsNotNull(healthBarBackground, "HealthBarBackground is null.");
        Assert.IsNotNull(healthBarFill, "HealthBarFill is null.");

        if (showHealthBar)
        {
            healthBarBackground.enabled = true;
            healthBarFill.enabled = true;
        }
        else
        {
            healthBarBackground.enabled = false;
            healthBarFill.enabled = false;
        }
    }
}

