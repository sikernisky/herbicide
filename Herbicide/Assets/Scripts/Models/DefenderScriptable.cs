using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Stores data for Defenders.
/// </summary>
[CreateAssetMenu(fileName = "DefenderScriptable", menuName = "Defender Scriptable", order = 0)]
public class DefenderScriptable : ScriptableObject
{
    /// <summary>
    /// This Defender's prefab.
    /// </summary>
    public GameObject defenderPrefab;

    /// <summary>
    /// Type of the Defender.
    /// </summary>
    [SerializeField]
    private Defender.DefenderType defenderType;

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
    /// Idle animation when this Defender is facing north.
    /// </summary>
    [SerializeField]
    private Sprite[] idleAnimationNorth;

    /// <summary>
    /// Idle animation when this Defender is facing east.
    /// </summary>
    [SerializeField]
    private Sprite[] idleAnimationEast;

    /// <summary>
    /// Idle animation when this Defender is facing south.
    /// </summary>
    [SerializeField]
    private Sprite[] idleAnimationSouth;

    /// <summary>
    /// Idle animation when this Defender is facing west.
    /// </summary>
    [SerializeField]
    private Sprite[] idleAnimationWest;


    /// <summary>
    /// Returns the DefenderType that this DefenderScriptable is storing.
    /// </summary>
    /// <returns>the DefenderAnimation that this DefenderScriptable is storing.
    /// </returns>
    public Defender.DefenderType GetDefenderType() => defenderType;

    /// Returns a copy of an attack animation array for a given direction.
    /// </summary>
    /// <param name="direction">The direction of the attack animation</param>
    /// <returns>A copy of the attack animation array.</returns>
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

    /// Returns a copy of a movement animation array for a given direction.
    /// </summary>
    /// <param name="direction">The direction of the movement animation</param>
    /// <returns>A copy of the movement animation array.</returns>
    public Sprite[] GetMovementAnimation(Direction direction)
    {
        switch (direction)
        {
            case Direction.NORTH:
                return (Sprite[])movementAnimationNorth.Clone();
            case Direction.EAST:
                return (Sprite[])movementAnimationEast.Clone();
            case Direction.SOUTH:
                return (Sprite[])movementAnimationSouth.Clone();
            case Direction.WEST:
                return (Sprite[])attackAnimationWest.Clone();
            default:
                return null;
        }
    }

    /// Returns a copy of an idle animation array for a given direction.
    /// </summary>
    /// <param name="direction">The direction of the idle animation</param>
    /// <returns>A copy of the idle animation array.</returns>
    public Sprite[] GetIdleAnimation(Direction direction)
    {
        switch (direction)
        {
            case Direction.NORTH:
                return (Sprite[])idleAnimationNorth.Clone();
            case Direction.EAST:
                return (Sprite[])idleAnimationEast.Clone();
            case Direction.SOUTH:
                return (Sprite[])idleAnimationSouth.Clone();
            case Direction.WEST:
                return (Sprite[])idleAnimationWest.Clone();
            default:
                return null;
        }
    }

    /// <summary>
    /// Returns the prefab that represents this Defender.
    /// </summary>
    /// <returns>the prefab that represents this Defender.</returns>
    public GameObject GetPrefab()
    {
        Assert.IsNotNull(defenderPrefab.GetComponent<Defender>(), "Prefab has no defender component.");
        return defenderPrefab;
    }
}
