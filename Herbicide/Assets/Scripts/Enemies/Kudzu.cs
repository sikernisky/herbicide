using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a Kudzu enemy.
/// </summary>
public class Kudzu : MovingEnemy
{
    /// <summary>
    /// Kudzu's base speed.
    /// </summary>
    protected override float BASE_SPEED => 2f;

    /// <summary>
    /// Kudzu's starting health.
    /// </summary>
    protected override int STARTING_HEALTH => 20;

    /// <summary>
    /// Maxmium health of a Kudzu.
    /// </summary>
    protected override int MAX_HEALTH => 20;

    /// <summary>
    /// Minimum health of a Kudzu.
    /// </summary>
    protected override int MIN_HEALTH => 0;

    /// <summary>
    /// Kudzu's attack range.
    /// </summary>
    protected override float ATTACK_RANGE => 2f;

    /// <summary>
    /// Attacks a target.
    /// </summary>
    /// <param name="target">The target to attack.</param>
    public override void Attack(PlaceableObject target)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Performs an action when this Kudzu dies.
    /// </summary>
    public override void OnDie()
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Performs an action when this Kudzu spawns.
    /// </summary>
    public override void OnSpawn()
    {
        Debug.Log("Spawned a Kudzu.");
    }


}
