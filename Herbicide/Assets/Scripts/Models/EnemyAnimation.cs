using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores animation data for Enemies.
/// </summary>
[CreateAssetMenu(fileName = "EnemyAnimationData", menuName = "Enemy Animation", order = 0)]
public class EnemyAnimation : ScriptableObject
{

    /// <summary>
    /// Type of the Enemy to animate.
    /// </summary>
    [SerializeField]
    private Enemy.EnemyType enemyType;

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
    private Sprite[] healthyIdleSpriteNorth;

    /// <summary>
    /// Idle animation when this Enemy is damaged.
    /// </summary>
    [SerializeField]
    private Sprite[] damagedIdleSpriteNorth;

    /// <summary>
    /// Idle animation when this Enemy is damaged.
    /// </summary>
    [SerializeField]
    private Sprite[] criticalIdleSpriteNorth;

    /// <summary>
    /// Idle animation when this Enemy is healthy.
    /// </summary>
    [SerializeField]
    private Sprite[] healthyIdleSpriteEast;

    /// <summary>
    /// Idle animation when this Enemy is damaged.
    /// </summary>
    [SerializeField]
    private Sprite[] damagedIdleSpriteEast;

    /// <summary>
    /// Idle animation when this Enemy is damaged.
    /// </summary>
    [SerializeField]
    private Sprite[] criticalIdleSpriteEast;

    /// <summary>
    /// Idle animation when this Enemy is healthy.
    /// </summary>
    [SerializeField]
    private Sprite[] healthyIdleSpriteSouth;

    /// <summary>
    /// Idle animation when this Enemy is damaged.
    /// </summary>
    [SerializeField]
    private Sprite[] damagedIdleSpriteSouth;

    /// <summary>
    /// Idle animation when this Enemy is damaged.
    /// </summary>
    [SerializeField]
    private Sprite[] criticalIdleSpriteSouth;

    /// <summary>
    /// Idle animation when this Enemy is healthy.
    /// </summary>
    [SerializeField]
    private Sprite[] healthyIdleSpriteWest;

    /// <summary>
    /// Idle animation when this Enemy is damaged.
    /// </summary>
    [SerializeField]
    private Sprite[] damagedIdleSpriteWest;

    /// <summary>
    /// Idle animation when this Enemy is damaged.
    /// </summary>
    [SerializeField]
    private Sprite[] criticalIdleSpriteWest;

    /// <summary>
    /// Returns the EnemyType that this EnemyAnimation is storing.
    /// </summary>
    /// <returns>the EnemyType that this EnemyAnimation is storing.
    /// </returns>
    public Enemy.EnemyType GetEnemyType() => enemyType;

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
                return healthyIdleSpriteNorth;
            case Direction.EAST:
                return healthyIdleSpriteEast;
            case Direction.SOUTH:
                return healthyIdleSpriteSouth;
            case Direction.WEST:
                return healthyIdleSpriteWest;
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
                return damagedIdleSpriteNorth;
            case Direction.EAST:
                return damagedIdleSpriteEast;
            case Direction.SOUTH:
                return damagedIdleSpriteSouth;
            case Direction.WEST:
                return damagedIdleSpriteWest;
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
                return criticalIdleSpriteNorth;
            case Direction.EAST:
                return criticalIdleSpriteEast;
            case Direction.SOUTH:
                return criticalIdleSpriteSouth;
            case Direction.WEST:
                return criticalIdleSpriteWest;
            default:
                return null;
        }
    }
}
