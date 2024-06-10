using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds all animations for an Enemy. This is necessary
/// because Enemies might have multiple skins, and declaring
/// a field for each skin would be cumbersome.
/// </summary>
[CreateAssetMenu(fileName = "New Enemy Animation Set", menuName = "Enemy Animation Set")]
[System.Serializable]
public class EnemyAnimationSet : ScriptableObject
{
    /// <summary>
    /// Animation track for placing the Enemy.
    /// </summary>
    public Sprite[] placementTrack;

    // -- Attack -- //

    /// <summary>
    /// Attack animations for the healthy Enemy facing north.
    /// </summary>
    public Sprite[] attackAnimationNorthHealthy;

    /// <summary>
    /// Attack animations for the healthy Enemy facing east.
    /// </summary>
    public Sprite[] attackAnimationEastHealthy;

    /// <summary>
    /// Attack animations for the healthy Enemy facing south.
    /// </summary>
    public Sprite[] attackAnimationSouthHealthy;

    /// <summary>
    /// Attack animations for the healthy Enemy facing west.
    /// </summary>
    public Sprite[] attackAnimationWestHealthy;

    /// <summary>
    /// Attack animations for the damaged Enemy facing north.
    /// </summary>
    public Sprite[] attackAnimationNorthDamaged;

    /// <summary>
    /// Attack animations for the damaged Enemy facing east.
    /// </summary>
    public Sprite[] attackAnimationEastDamaged;

    /// <summary>
    /// Attack animations for the damaged Enemy facing south.
    /// </summary>
    public Sprite[] attackAnimationSouthDamaged;

    /// <summary>
    /// Attack animations for the damaged Enemy facing west.
    /// </summary>
    public Sprite[] attackAnimationWestDamaged;

    /// <summary>
    /// Attack animations for the critical Enemy facing north.
    /// </summary>
    public Sprite[] attackAnimationNorthCritical;

    /// <summary>
    /// Attack animations for the critical Enemy facing east.
    /// </summary>
    public Sprite[] attackAnimationEastCritical;

    /// <summary>
    /// Attack animations for the critical Enemy facing south.
    /// </summary>
    public Sprite[] attackAnimationSouthCritical;

    /// <summary>
    /// Attack animations for the critical Enemy facing west.
    /// </summary>
    public Sprite[] attackAnimationWestCritical;

    // -- Idle -- //

    /// <summary>
    /// Idle animations for the healthy Enemy facing north.
    /// </summary>
    public Sprite[] idleAnimationNorthHealthy;

    /// <summary>
    /// Idle animations for the healthy Enemy facing east.
    /// </summary>
    public Sprite[] idleAnimationEastHealthy;

    /// <summary>
    /// Idle animations for the healthy Enemy facing south.
    /// </summary>
    public Sprite[] idleAnimationSouthHealthy;

    /// <summary>
    /// Idle animations for the healthy Enemy facing west.
    /// </summary>
    public Sprite[] idleAnimationWestHealthy;

    /// <summary>
    /// Idle animations for the damaged Enemy facing north.
    /// </summary>
    public Sprite[] idleAnimationNorthDamaged;

    /// <summary>
    /// Idle animations for the damaged Enemy facing east.
    /// </summary>
    public Sprite[] idleAnimationEastDamaged;

    /// <summary>
    /// Idle animations for the damaged Enemy facing south.
    /// </summary>
    public Sprite[] idleAnimationSouthDamaged;

    /// <summary>
    /// Idle animations for the damaged Enemy facing west.
    /// </summary>
    public Sprite[] idleAnimationWestDamaged;

    /// <summary>
    /// Idle animations for the critical Enemy facing north.
    /// </summary>
    public Sprite[] idleAnimationNorthCritical;

    /// <summary>
    /// Idle animations for the critical Enemy facing east.
    /// </summary>
    public Sprite[] idleAnimationEastCritical;

    /// <summary>
    /// Idle animations for the critical Enemy facing south.
    /// </summary>
    public Sprite[] idleAnimationSouthCritical;

    /// <summary>
    /// Idle animations for the critical Enemy facing west.
    /// </summary>
    public Sprite[] idleAnimationWestCritical;

    // -- Movement -- //

    /// <summary>
    /// Movement animations for the healthy Enemy facing north.
    /// </summary>
    public Sprite[] movementAnimationNorthHealthy;

    /// <summary>
    /// Movement animations for the healthy Enemy facing east.
    /// <summary>
    public Sprite[] movementAnimationEastHealthy;

    /// <summary>
    /// Movement animations for the healthy Enemy facing south.
    /// <summary>
    public Sprite[] movementAnimationSouthHealthy;

    /// <summary>
    /// Movement animations for the healthy Enemy facing west.
    /// <summary>
    public Sprite[] movementAnimationWestHealthy;

    /// <summary>
    /// Movement animations for the damaged Enemy facing north.
    /// </summary>
    public Sprite[] movementAnimationNorthDamaged;

    /// <summary>
    /// Movement animations for the damaged Enemy facing east.
    /// <summary>
    public Sprite[] movementAnimationEastDamaged;

    /// <summary>
    /// Movement animations for the damaged Enemy facing south.
    /// <summary>
    public Sprite[] movementAnimationSouthDamaged;

    /// <summary>
    /// Movement animations for the damaged Enemy facing west.
    /// <summary>
    public Sprite[] movementAnimationWestDamaged;

    /// <summary>
    /// Movement animations for the critical Enemy facing north.
    /// </summary>
    public Sprite[] movementAnimationNorthCritical;

    /// <summary>
    /// Movement animations for the critical Enemy facing east.
    /// <summary>
    public Sprite[] movementAnimationEastCritical;

    /// <summary>
    /// Movement animations for the critical Enemy facing south.
    /// <summary>
    public Sprite[] movementAnimationSouthCritical;

    /// <summary>
    /// Movement animations for the critical Enemy facing west.
    /// <summary>
    public Sprite[] movementAnimationWestCritical;

    // -- Spawn -- //

    /// <summary>
    /// Spawn animations for the healthy Enemy facing north.
    /// </summary>
    public Sprite[] spawnAnimationNorthHealthy;

    /// <summary>
    /// Spawn animations for the healthy Enemy facing east.
    /// <summary>
    public Sprite[] spawnAnimationEastHealthy;

    /// <summary>
    /// Spawn animations for the healthy Enemy facing south.
    /// <summary>
    public Sprite[] spawnAnimationSouthHealthy;

    /// <summary>
    /// Spawn animations for the healthy Enemy facing west.
    /// <summary>
    public Sprite[] spawnAnimationWestHealthy;

    /// <summary>
    /// Spawn animations for the damaged Enemy facing north.
    /// </summary>
    public Sprite[] spawnAnimationNorthDamaged;

    /// <summary>
    /// Spawn animations for the damaged Enemy facing east.
    /// <summary>
    public Sprite[] spawnAnimationEastDamaged;

    /// <summary>
    /// Spawn animations for the damaged Enemy facing south.
    /// <summary>
    public Sprite[] spawnAnimationSouthDamaged;

    /// <summary>
    /// Spawn animations for the damaged Enemy facing west.
    /// <summary>
    public Sprite[] spawnAnimationWestDamaged;

    /// <summary>
    /// Spawn animations for the critical Enemy facing north.
    /// </summary>
    public Sprite[] spawnAnimationNorthCritical;

    /// <summary>
    /// Spawn animations for the critical Enemy facing east.
    /// <summary>
    public Sprite[] spawnAnimationEastCritical;

    /// <summary>
    /// Spawn animations for the critical Enemy facing south.
    /// <summary>
    public Sprite[] spawnAnimationSouthCritical;

    /// <summary>
    /// Spawn animations for the critical Enemy facing west.
    /// <summary>
    public Sprite[] spawnAnimationWestCritical;


    /// <summary>
    /// Returns the attack animation track for the given direction.
    /// </summary>
    /// <param name="d">The direction of the track to get</param>
    /// <param name="s">The health state of the Enemy</param>
    /// <returns>the attack animation track for the given direction </returns>
    public Sprite[] GetAttackAnimation(Direction d, Enemy.EnemyHealthState s)
    {
        switch (d)
        {
            case Direction.NORTH:
                if (s == Enemy.EnemyHealthState.HEALTHY)
                    return attackAnimationNorthHealthy;
                else if (s == Enemy.EnemyHealthState.DAMAGED)
                    return attackAnimationNorthDamaged;
                else if (s == Enemy.EnemyHealthState.CRITICAL)
                    return attackAnimationNorthCritical;
                else
                    throw new System.InvalidOperationException("Invalid health state.");
            case Direction.EAST:
                if (s == Enemy.EnemyHealthState.HEALTHY)
                    return attackAnimationEastHealthy;
                else if (s == Enemy.EnemyHealthState.DAMAGED)
                    return attackAnimationEastDamaged;
                else if (s == Enemy.EnemyHealthState.CRITICAL)
                    return attackAnimationEastCritical;
                else
                    throw new System.InvalidOperationException("Invalid health state.");
            case Direction.SOUTH:
                if (s == Enemy.EnemyHealthState.HEALTHY)
                    return attackAnimationSouthHealthy;
                else if (s == Enemy.EnemyHealthState.DAMAGED)
                    return attackAnimationSouthDamaged;
                else if (s == Enemy.EnemyHealthState.CRITICAL)
                    return attackAnimationSouthCritical;
                else
                    throw new System.InvalidOperationException("Invalid health state.");
            case Direction.WEST:
                if (s == Enemy.EnemyHealthState.HEALTHY)
                    return attackAnimationWestHealthy;
                else if (s == Enemy.EnemyHealthState.DAMAGED)
                    return attackAnimationWestDamaged;
                else if (s == Enemy.EnemyHealthState.CRITICAL)
                    return attackAnimationWestCritical;
                else
                    throw new System.InvalidOperationException("Invalid health state.");
            default:
                throw new System.InvalidOperationException("Invalid direction.");
        }
    }

    /// <summary>
    /// Returns the idle animation track for the given direction.
    /// </summary>
    /// <returns>the idle animation track for the given direction </returns>
    /// <param name="d">The direction of the track to get</param>
    /// <param name="s">The health state of the Enemy</param>
    public Sprite[] GetIdleAnimation(Direction d, Enemy.EnemyHealthState s)
    {
        switch (d)
        {
            case Direction.NORTH:
                if (s == Enemy.EnemyHealthState.HEALTHY)
                    return idleAnimationNorthHealthy;
                else if (s == Enemy.EnemyHealthState.DAMAGED)
                    return idleAnimationNorthDamaged;
                else if (s == Enemy.EnemyHealthState.CRITICAL)
                    return idleAnimationNorthCritical;
                else
                    throw new System.InvalidOperationException("Invalid health state.");
            case Direction.EAST:
                if (s == Enemy.EnemyHealthState.HEALTHY)
                    return idleAnimationEastHealthy;
                else if (s == Enemy.EnemyHealthState.DAMAGED)
                    return idleAnimationEastDamaged;
                else if (s == Enemy.EnemyHealthState.CRITICAL)
                    return idleAnimationEastCritical;
                else
                    throw new System.InvalidOperationException("Invalid health state.");
            case Direction.SOUTH:
                if (s == Enemy.EnemyHealthState.HEALTHY)
                    return idleAnimationSouthHealthy;
                else if (s == Enemy.EnemyHealthState.DAMAGED)
                    return idleAnimationSouthDamaged;
                else if (s == Enemy.EnemyHealthState.CRITICAL)
                    return idleAnimationSouthCritical;
                else
                    throw new System.InvalidOperationException("Invalid health state.");
            case Direction.WEST:
                if (s == Enemy.EnemyHealthState.HEALTHY)
                    return idleAnimationWestHealthy;
                else if (s == Enemy.EnemyHealthState.DAMAGED)
                    return idleAnimationWestDamaged;
                else if (s == Enemy.EnemyHealthState.CRITICAL)
                    return idleAnimationWestCritical;
                else
                    throw new System.InvalidOperationException("Invalid health state.");
            default:
                throw new System.InvalidOperationException("Invalid direction.");
        }
    }

    /// <summary>
    /// Returns the movement animation track for the given direction.
    /// </summary>
    /// <returns>the movement animation track for the given direction </returns>
    /// <param name="d">The direction of the track to get</param>
    /// <param name="s">The health state of the Enemy</param> 
    public Sprite[] GetMovementAnimation(Direction d, Enemy.EnemyHealthState s)
    {
        switch (d)
        {
            case Direction.NORTH:
                if (s == Enemy.EnemyHealthState.HEALTHY)
                    return movementAnimationNorthHealthy;
                else if (s == Enemy.EnemyHealthState.DAMAGED)
                    return movementAnimationNorthDamaged;
                else if (s == Enemy.EnemyHealthState.CRITICAL)
                    return movementAnimationNorthCritical;
                else
                    throw new System.InvalidOperationException("Invalid health state.");
            case Direction.EAST:
                if (s == Enemy.EnemyHealthState.HEALTHY)
                    return movementAnimationEastHealthy;
                else if (s == Enemy.EnemyHealthState.DAMAGED)
                    return movementAnimationEastDamaged;
                else if (s == Enemy.EnemyHealthState.CRITICAL)
                    return movementAnimationEastCritical;
                else
                    throw new System.InvalidOperationException("Invalid health state.");
            case Direction.SOUTH:
                if (s == Enemy.EnemyHealthState.HEALTHY)
                    return movementAnimationSouthHealthy;
                else if (s == Enemy.EnemyHealthState.DAMAGED)
                    return movementAnimationSouthDamaged;
                else if (s == Enemy.EnemyHealthState.CRITICAL)
                    return movementAnimationSouthCritical;
                else
                    throw new System.InvalidOperationException("Invalid health state.");
            case Direction.WEST:
                if (s == Enemy.EnemyHealthState.HEALTHY)
                    return movementAnimationWestHealthy;
                else if (s == Enemy.EnemyHealthState.DAMAGED)
                    return movementAnimationWestDamaged;
                else if (s == Enemy.EnemyHealthState.CRITICAL)
                    return movementAnimationWestCritical;
                else
                    throw new System.InvalidOperationException("Invalid health state.");
            default:
                throw new System.InvalidOperationException("Invalid direction.");
        }
    }

    /// <summary>
    /// Returns the spawn animation track for the given direction.
    /// </summary>
    /// <returns>the spawn animation track for the given direction </returns>
    /// <param name="d">The direction of the track to get</param>
    /// <param name="s">The health state of the Enemy</param> 
    public Sprite[] GetSpawnAnimation(Direction d, Enemy.EnemyHealthState s)
    {
        switch (d)
        {
            case Direction.NORTH:
                if (s == Enemy.EnemyHealthState.HEALTHY)
                    return spawnAnimationNorthHealthy;
                else if (s == Enemy.EnemyHealthState.DAMAGED)
                    return spawnAnimationNorthDamaged;
                else if (s == Enemy.EnemyHealthState.CRITICAL)
                    return spawnAnimationNorthCritical;
                else
                    throw new System.InvalidOperationException("Invalid health state.");
            case Direction.EAST:
                if (s == Enemy.EnemyHealthState.HEALTHY)
                    return spawnAnimationEastHealthy;
                else if (s == Enemy.EnemyHealthState.DAMAGED)
                    return spawnAnimationEastDamaged;
                else if (s == Enemy.EnemyHealthState.CRITICAL)
                    return spawnAnimationEastCritical;
                else
                    throw new System.InvalidOperationException("Invalid health state.");
            case Direction.SOUTH:
                if (s == Enemy.EnemyHealthState.HEALTHY)
                    return spawnAnimationSouthHealthy;
                else if (s == Enemy.EnemyHealthState.DAMAGED)
                    return spawnAnimationSouthDamaged;
                else if (s == Enemy.EnemyHealthState.CRITICAL)
                    return spawnAnimationSouthCritical;
                else
                    throw new System.InvalidOperationException("Invalid health state.");
            case Direction.WEST:
                if (s == Enemy.EnemyHealthState.HEALTHY)
                    return spawnAnimationWestHealthy;
                else if (s == Enemy.EnemyHealthState.DAMAGED)
                    return spawnAnimationWestDamaged;
                else if (s == Enemy.EnemyHealthState.CRITICAL)
                    return spawnAnimationWestCritical;
                else
                    throw new System.InvalidOperationException("Invalid health state.");
            default:
                throw new System.InvalidOperationException("Invalid direction.");
        }
    }


    /// <summary>
    /// Returns the placement animation track.
    /// </summary>
    /// <returns>the placement animation track </returns>
    public Sprite[] GetPlacementAnimation() { return placementTrack; }
}
