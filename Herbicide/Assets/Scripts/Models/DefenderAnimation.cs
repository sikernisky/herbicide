using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores animation data for Defenders.
/// </summary>
[CreateAssetMenu(fileName = "DefenderAnimationData", menuName = "Defender Animation", order = 0)]
public class DefenderAnimation : ScriptableObject
{
    /// <summary>
    /// Type of the Defender to animate.
    /// </summary>
    [SerializeField]
    private Defender.DefenderType defenderType;

    /// <summary>
    /// Type of the Defender to animate.
    /// </summary>
    [SerializeField]
    private Enemy.EnemyType enemyType;

    /// <summary>
    /// Movement animation when this Defender is facing north.
    /// </summary>
    [SerializeField]
    private Sprite[] movementAnimationNorth;

    /// <summary>
    /// Movement animation when this Defender is facing north.
    /// </summary>
    [SerializeField]
    private Sprite[] movementAnimationEast;

    /// <summary>
    /// Movement animation when this Defender is facing south.
    /// </summary>
    [SerializeField]
    private Sprite[] movementAnimationSouth;

    /// <summary>
    /// Movement animation when this Defender is facing west.
    /// </summary>
    [SerializeField]
    private Sprite[] movementAnimationWest;

    /// <summary>
    /// Attack animation when this Defender is facing north.
    /// </summary>
    [SerializeField]
    private Sprite[] attackAnimationNorth;

    /// <summary>
    /// Attack animation when this Defender is facing north.
    /// </summary>
    [SerializeField]
    private Sprite[] attackAnimationEast;

    /// <summary>
    /// Attack animation when this Defender is facing south.
    /// </summary>
    [SerializeField]
    private Sprite[] attackAnimationSouth;

    /// <summary>
    /// Attack animation when this Defender is facing west.
    /// </summary>
    [SerializeField]
    private Sprite[] attackAnimationWest;

    /// <summary>
    /// Returns the DefenderType that this DefenderAnimation is storing.
    /// </summary>
    /// <returns>the DefenderAnimation that this DefenderAnimation is storing.
    /// </returns>
    public Defender.DefenderType GetDefenderType() => defenderType;

    /// Returns a copy of a damaged attack animation array for a given
    /// direction.
    /// </summary>
    /// <param name="direction">The direction of the attack animation</param>
    /// <returns>A copy of the damaged attack animation array.</returns>
    public Sprite[] GetAttackAnimation(Direction direction)
    {
        switch (direction)
        {
            case Direction.NORTH:
                return (Sprite[])attackAnimationNorth.Clone();
            case Direction.EAST:
                return (Sprite[])attackAnimationEast.Clone();
            case Direction.SOUTH:
                return (Sprite[])attackAnimationSouth.Clone();
            case Direction.WEST:
                return (Sprite[])attackAnimationWest.Clone();
            default:
                return null;
        }
    }
}
