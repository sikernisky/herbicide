using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a Hazard that slows Enemies that walk
/// over it.
/// </summary>
public abstract class SlowZone : Hazard
{
    /// <summary>
    /// Starting attack range of a SlowZone.
    /// </summary>
    public override float BASE_ATTACK_RANGE => 0.5f;

    /// <summary>
    /// Maximum attack range of a SlowZone.
    /// </summary>
    public override float MAX_ATTACK_RANGE => float.MaxValue;

    /// <summary>
    /// Minimum attack range of a SlowZone.
    /// </summary>
    public override float MIN_ATTACK_RANGE => 0f;

    /// <summary>
    /// Starting attack cooldown of a SlowZone.
    /// </summary>
    public override float BASE_ATTACK_COOLDOWN => 0.01f;

    /// <summary>
    /// Maxmimum attack cooldown of a SlowZone.
    /// </summary>
    public override float MAX_ATTACK_COOLDOWN => 0.01f;

    /// <summary>
    /// Base chase range of a SlowZone.
    /// </summary>
    public override float BASE_CHASE_RANGE => 0.5f;

    /// <summary>
    /// Base chase range of a SlowZone.
    /// </summary>
    public override float MAX_CHASE_RANGE => BASE_ATTACK_RANGE;

    /// <summary>
    /// Minimum chase range of a SlowZone.
    /// </summary>
    public override float MIN_CHASE_RANGE => MIN_ATTACK_RANGE;

    /// <summary>
    /// Base movement speed of a SlowZone.
    /// </summary>
    public override float BASE_MOVEMENT_SPEED => 0f;

    /// <summary>
    /// Maximum movement speed of a SlowZone.
    /// </summary>
    public override float MAX_MOVEMENT_SPEED => 0f;

    /// <summary>
    /// Minimum movement speed of a SlowZone.
    /// </summary>
    public override float MIN_MOVEMENT_SPEED => 0f;

    /// <summary>
    /// Maximum health of a SlowZone.
    /// </summary>
    public override int MAX_HEALTH => int.MaxValue;

    /// <summary>
    /// Minimum health of a SlowZone.
    /// </summary>
    public override int MIN_HEALTH => 0;

    /// <summary>
    /// The current rate by which this SlowZone slows its target.
    /// </summary>
    private float slowRate;

    /// <summary>
    /// Starting amount, between 0 (0%) and 1 (100%), by which this
    /// SlowZone slows its target.
    /// </summary>
    public abstract float BASE_SLOW_RATE { get; }

    /// <summary>
    /// Maximum amount, between 0 and 1, by which this
    /// SlowZone can slow its target.
    /// </summary>
    public virtual float MAX_SLOW_RATE => 1;

    /// <summary>
    /// Minimum amount, between 0 and 1, by which this
    /// SlowZone can slow its target.
    /// </summary>
    public virtual float MIN_SLOW_RATE => 0;


    /// <summary>
    /// Returns the rate by which this SlowZone slows
    /// its target.
    /// </summary>
    /// <returns>the rate (0-1) by which this SlowZone slows
    /// its target.</returns>
    public float GetSlowRate() { return slowRate; }

    /// <summary>
    /// Adds some amount to the rate by which this SlowZone slows
    /// its target.
    /// </summary>
    public void AdjustSlowRate(float amount)
    {
        slowRate = Mathf.Clamp(slowRate + amount, MIN_SLOW_RATE, MAX_SLOW_RATE);
    }

    /// <summary>
    /// Resets this SlowZone's slow rate to its starting value.
    /// </summary>
    public void ResetSlowRate() { slowRate = BASE_SLOW_RATE; }

    /// <summary>
    /// Resets this SlowZone's stats to their starting values.
    /// </summary>
    public override void ResetStats()
    {
        base.ResetStats();
        ResetSlowRate();
    }
}
