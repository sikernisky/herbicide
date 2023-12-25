using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Stores data for Enemies.
/// </summary>
[CreateAssetMenu(fileName = "EnemyScriptable", menuName = "Enemy Scriptable", order = 0)]
public class EnemyScriptable : ModelScriptable
{
    /// <summary>
    /// Movement animation when this Enemy is healthy.
    /// </summary>
    [SerializeField]
    private Sprite[] healthyMovementAnimationNorth;

    /// <summary>
    /// Movement animation when this Enemy is damaged.
    /// </summary>
    [SerializeField]
    private Sprite[] damagedMovementAnimationNorth;

    /// <summary>
    /// Movement animation when this Enemy is critically damaged.
    /// </summary>
    [SerializeField]
    private Sprite[] criticalMovementAnimationNorth;

    /// <summary>
    /// Movement animation when this Enemy is healthy.
    /// </summary>
    [SerializeField]
    private Sprite[] healthyMovementAnimationEast;

    /// <summary>
    /// Movement animation when this Enemy is damaged.
    /// </summary>
    [SerializeField]
    private Sprite[] damagedMovementAnimationEast;

    /// <summary>
    /// Movement animation when this Enemy is critically damaged.
    /// </summary>
    [SerializeField]
    private Sprite[] criticalMovementAnimationEast;

    /// <summary>
    /// Movement animation when this Enemy is healthy.
    /// </summary>
    [SerializeField]
    private Sprite[] healthyMovementAnimationSouth;

    /// <summary>
    /// Movement animation when this Enemy is damaged.
    /// </summary>
    [SerializeField]
    private Sprite[] damagedMovementAnimationSouth;

    /// <summary>
    /// Movement animation when this Enemy is critically damaged.
    /// </summary>
    [SerializeField]
    private Sprite[] criticalMovementAnimationSouth;

    /// <summary>
    /// Movement animation when this Enemy is healthy.
    /// </summary>
    [SerializeField]
    private Sprite[] healthyMovementAnimationWest;

    /// <summary>
    /// Movement animation when this Enemy is damaged.
    /// </summary>
    [SerializeField]
    private Sprite[] damagedMovementAnimationWest;

    /// <summary>
    /// Movement animation when this Enemy is damaged.
    /// </summary>
    [SerializeField]
    private Sprite[] criticalMovementAnimationWest;


    /// <summary>
    /// Attack animation when this Enemy is healthy.
    /// </summary>
    [SerializeField]
    private Sprite[] healthyAttackAnimationNorth;

    /// <summary>
    /// Attack animation when this Enemy is damaged.
    /// </summary>
    [SerializeField]
    private Sprite[] damagedAttackAnimationNorth;

    /// <summary>
    /// Attack animation when this Enemy is critically damaged.
    /// </summary>
    [SerializeField]
    private Sprite[] criticalAttackAnimationNorth;

    /// <summary>
    /// Attack animation when this Enemy is healthy.
    /// </summary>
    [SerializeField]
    private Sprite[] healthyAttackAnimationEast;

    /// <summary>
    /// Attack animation when this Enemy is damaged.
    /// </summary>
    [SerializeField]
    private Sprite[] damagedAttackAnimationEast;

    /// <summary>
    /// Attack animation when this Enemy is critically damaged.
    /// </summary>
    [SerializeField]
    private Sprite[] criticalAttackAnimationEast;

    /// <summary>
    /// Attack animation when this Enemy is healthy.
    /// </summary>
    [SerializeField]
    private Sprite[] healthyAttackAnimationSouth;

    /// <summary>
    /// Attack animation when this Enemy is damaged.
    /// </summary>
    [SerializeField]
    private Sprite[] damagedAttackAnimationSouth;

    /// <summary>
    /// Attack animation when this Enemy is critically damaged.
    /// </summary>
    [SerializeField]
    private Sprite[] criticalAttackAnimationSouth;

    /// <summary>
    /// Attack animation when this Enemy is healthy.
    /// </summary>
    [SerializeField]
    private Sprite[] healthyAttackAnimationWest;

    /// <summary>
    /// Attack animation when this Enemy is damaged.
    /// </summary>
    [SerializeField]
    private Sprite[] damagedAttackAnimationWest;

    /// <summary>
    /// Attack animation when this Enemy is critically damaged.
    /// </summary>
    [SerializeField]
    private Sprite[] criticalAttackAnimationWest;

    /// <summary>
    /// Idle animation when this Enemy is healthy.
    /// </summary>
    [SerializeField]
    private Sprite[] healthyIdleAnimationNorth;

    /// <summary>
    /// Idle animation when this Enemy is damaged.
    /// </summary>
    [SerializeField]
    private Sprite[] damagedIdleAnimationNorth;

    /// <summary>
    /// Idle animation when this Enemy is damaged.
    /// </summary>
    [SerializeField]
    private Sprite[] criticalIdleAnimationNorth;

    /// <summary>
    /// Idle animation when this Enemy is healthy.
    /// </summary>
    [SerializeField]
    private Sprite[] healthyIdleAnimationEast;

    /// <summary>
    /// Idle animation when this Enemy is damaged.
    /// </summary>
    [SerializeField]
    private Sprite[] damagedIdleAnimationEast;

    /// <summary>
    /// Idle animation when this Enemy is damaged.
    /// </summary>
    [SerializeField]
    private Sprite[] criticalIdleAnimationEast;

    /// <summary>
    /// Idle animation when this Enemy is healthy.
    /// </summary>
    [SerializeField]
    private Sprite[] healthyIdleAnimationSouth;

    /// <summary>
    /// Idle animation when this Enemy is damaged.
    /// </summary>
    [SerializeField]
    private Sprite[] damagedIdleAnimationSouth;

    /// <summary>
    /// Idle animation when this Enemy is damaged.
    /// </summary>
    [SerializeField]
    private Sprite[] criticalIdleAnimationSouth;

    /// <summary>
    /// Idle animation when this Enemy is healthy.
    /// </summary>
    [SerializeField]
    private Sprite[] healthyIdleAnimationWest;

    /// <summary>
    /// Idle animation when this Enemy is damaged.
    /// </summary>
    [SerializeField]
    private Sprite[] damagedIdleAnimationWest;

    /// <summary>
    /// Idle animation when this Enemy is damaged.
    /// </summary>
    [SerializeField]
    private Sprite[] criticalIdleAnimationWest;

    /// <summary>
    /// Returns a copy of a healthy movement animation array for a 
    /// given direction.
    /// </summary>
    /// <param name="direction">The direction of the movement animation</param>
    /// <returns>A copy of the healthy movement animation array.</returns>
    public Sprite[] GetHealthyMovementAnimation(Direction direction)
    {
        switch (direction)
        {
            case Direction.NORTH:
                return (Sprite[])healthyMovementAnimationNorth.Clone();
            case Direction.EAST:
                return (Sprite[])healthyMovementAnimationEast.Clone();
            case Direction.SOUTH:
                return (Sprite[])healthyMovementAnimationSouth.Clone();
            case Direction.WEST:
                return (Sprite[])healthyMovementAnimationWest.Clone();
            default:
                return null;
        }
    }

    /// <summary>
    /// Returns a copy of a damaged movement animation array for a given
    /// direction.
    /// </summary>
    /// <param name="direction">The direction of the movement animation</param>
    /// <returns>A copy of the damaged movement animation array.</returns>
    public Sprite[] GetDamagedMovementAnimation(Direction direction)
    {
        switch (direction)
        {
            case Direction.NORTH:
                return (Sprite[])damagedMovementAnimationNorth.Clone();
            case Direction.EAST:
                return (Sprite[])damagedMovementAnimationEast.Clone();
            case Direction.SOUTH:
                return (Sprite[])damagedMovementAnimationSouth.Clone();
            case Direction.WEST:
                return (Sprite[])damagedMovementAnimationWest.Clone();
            default:
                return null;
        }
    }
    /// <summary>
    /// Returns a copy of a critical movement animation array for a given
    /// direction.
    /// </summary>
    /// <param name="direction">The direction of the movement animation</param>
    /// <returns>A copy of the critical movement animation array.</returns>
    public Sprite[] GetCriticalMovementAnimation(Direction direction)
    {
        switch (direction)
        {
            case Direction.NORTH:
                return (Sprite[])criticalMovementAnimationNorth.Clone();
            case Direction.EAST:
                return (Sprite[])criticalMovementAnimationEast.Clone();
            case Direction.SOUTH:
                return (Sprite[])criticalMovementAnimationSouth.Clone();
            case Direction.WEST:
                return (Sprite[])criticalMovementAnimationWest.Clone();
            default:
                return null;
        }
    }

    /// <summary>
    /// Returns a copy of a healthy attack animation array for a given
    /// direction.
    /// </summary>
    /// <param name="direction">The direction of the attack animation</param>
    /// <returns>A copy of the healthy attack animation array.</returns>
    public Sprite[] GetHealthyAttackAnimation(Direction direction)
    {
        switch (direction)
        {
            case Direction.NORTH:
                return (Sprite[])healthyAttackAnimationNorth.Clone();
            case Direction.EAST:
                return (Sprite[])healthyAttackAnimationEast.Clone();
            case Direction.SOUTH:
                return (Sprite[])healthyAttackAnimationSouth.Clone();
            case Direction.WEST:
                return (Sprite[])healthyAttackAnimationWest.Clone();
            default:
                return null;
        }
    }

    /// <summary>
    /// Returns a copy of a damaged attack animation array for a given
    /// direction.
    /// </summary>
    /// <param name="direction">The direction of the attack animation</param>
    /// <returns>A copy of the damaged attack animation array.</returns>
    public Sprite[] GetDamagedAttackAnimation(Direction direction)
    {
        switch (direction)
        {
            case Direction.NORTH:
                return (Sprite[])damagedAttackAnimationNorth.Clone();
            case Direction.EAST:
                return (Sprite[])damagedAttackAnimationEast.Clone();
            case Direction.SOUTH:
                return (Sprite[])damagedAttackAnimationSouth.Clone();
            case Direction.WEST:
                return (Sprite[])damagedAttackAnimationWest.Clone();
            default:
                return null;
        }
    }

    /// <summary>
    /// Returns a copy of a critical attack animation array for a given
    /// direction.
    /// </summary>
    /// <param name="direction">The direction of the attack animation</param>
    /// <returns>A copy of the critical attack animation array.</returns>
    public Sprite[] GetCriticalAttackAnimation(Direction direction)
    {
        switch (direction)
        {
            case Direction.NORTH:
                return (Sprite[])criticalAttackAnimationNorth.Clone();
            case Direction.EAST:
                return (Sprite[])criticalAttackAnimationEast.Clone();
            case Direction.SOUTH:
                return (Sprite[])criticalAttackAnimationSouth.Clone();
            case Direction.WEST:
                return (Sprite[])criticalAttackAnimationWest.Clone();
            default:
                return null;
        }
    }

    /// <summary>
    /// Returns the healthy idle animation for a given direction.
    /// </summary>
    /// <param name="direction">The direction of the idle animation</param>
    /// <returns>A copy of the healthy idle animation array</returns>
    public Sprite[] GetHealthyIdleAnimation(Direction direction)
    {
        switch (direction)
        {
            case Direction.NORTH:
                return healthyIdleAnimationNorth;
            case Direction.EAST:
                return healthyIdleAnimationEast;
            case Direction.SOUTH:
                return healthyIdleAnimationSouth;
            case Direction.WEST:
                return healthyIdleAnimationWest;
            default:
                return null;
        }
    }

    /// <summary>
    /// Returns the damaged idle animation for a given direction.
    /// </summary>
    /// <param name="direction">The direction of the idle animation</param>
    /// <returns>A copy of the damaged idle animation array</returns>
    public Sprite[] GetDamagedIdleAnimation(Direction direction)
    {
        switch (direction)
        {
            case Direction.NORTH:
                return damagedIdleAnimationNorth;
            case Direction.EAST:
                return damagedIdleAnimationEast;
            case Direction.SOUTH:
                return damagedIdleAnimationSouth;
            case Direction.WEST:
                return damagedIdleAnimationWest;
            default:
                return null;
        }
    }

    /// <summary>
    /// Returns the critical idle animation for a given direction.
    /// </summary>
    /// <param name="direction">The direction of the idle animation</param>
    /// <returns>A copy of the critical idle animation array</returns>
    public Sprite[] GetCriticalIdleAnimation(Direction direction)
    {
        switch (direction)
        {
            case Direction.NORTH:
                return criticalIdleAnimationNorth;
            case Direction.EAST:
                return criticalIdleAnimationEast;
            case Direction.SOUTH:
                return criticalIdleAnimationSouth;
            case Direction.WEST:
                return criticalIdleAnimationWest;
            default:
                return null;
        }
    }
}
