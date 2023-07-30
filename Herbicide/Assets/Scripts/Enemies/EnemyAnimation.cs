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
    /// Returns the EnemyType that this EnemyAnimation is animating.
    /// </summary>
    /// <returns>the EnemyType that this EnemyAnimation is animating.
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
}
