using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds all animations for a defender. This is necessary
/// because Defenders have multiple skins, and declaring
/// a field for each skin would be cumbersome.
/// </summary>
[System.Serializable]
public class DefenderAnimationSet
{
    /// <summary>
    /// Animation track for placing the Defender.
    /// </summary>
    public Sprite[] placementTrack;

    /// <summary>
    /// Attack animations for the Defender facing north.
    /// </summary>
    public Sprite[] attackAnimationNorth;

    /// <summary>
    /// Attack animations for the Defender facing east.
    /// </summary>
    public Sprite[] attackAnimationEast;

    /// <summary>
    /// Attack animations for the Defender facing south.
    /// </summary>
    public Sprite[] attackAnimationSouth;

    /// <summary>
    /// Attack animations for the Defender facing west.
    /// </summary>
    public Sprite[] attackAnimationWest;

    /// <summary>
    /// Idle animations for the Defender facing north.
    /// </summary>
    public Sprite[] idleAnimationNorth;

    /// <summary>
    /// Idle animations for the Defender facing east.
    /// </summary>
    public Sprite[] idleAnimationEast;

    /// <summary>
    /// Idle animations for the Defender facing south.
    /// </summary>
    public Sprite[] idleAnimationSouth;

    /// <summary>
    /// Idle animations for the Defender facing west.
    /// </summary>
    public Sprite[] idleAnimationWest;

    /// <summary>
    /// Initializes a new instance of the SkinAnimations class with all animations.
    /// </summary>
    /// <param name="placement">Placement animation sprites.</param>
    /// <param name="attackN">North-facing attack animation sprites.</param>
    /// <param name="attackE">East-facing attack animation sprites.</param>
    /// <param name="attackS">South-facing attack animation sprites.</param>
    /// <param name="attackW">West-facing attack animation sprites.</param>
    /// <param name="idleN">North-facing idle animation sprites.</param>
    /// <param name="idleE">East-facing idle animation sprites.</param>
    /// <param name="idleS">South-facing idle animation sprites.</param>
    /// <param name="idleW">West-facing idle animation sprites.</param>
    public DefenderAnimationSet(Sprite[] placement, Sprite[] attackN, Sprite[] attackE, Sprite[] attackS, Sprite[] attackW,
                          Sprite[] idleN, Sprite[] idleE, Sprite[] idleS, Sprite[] idleW)
    {
        placementTrack = placement;
        attackAnimationNorth = attackN;
        attackAnimationEast = attackE;
        attackAnimationSouth = attackS;
        attackAnimationWest = attackW;
        idleAnimationNorth = idleN;
        idleAnimationEast = idleE;
        idleAnimationSouth = idleS;
        idleAnimationWest = idleW;
    }

    /// <summary>
    /// Returns the attack animation track for the given direction.
    /// </summary>
    /// <param name="d">The direction of the track to get</param>
    /// <returns>the attack animation track for the given direction </returns>
    public Sprite[] GetAttackAnimation(Direction d)
    {
        switch (d)
        {
            case Direction.NORTH:
                return attackAnimationNorth;
            case Direction.EAST:
                return attackAnimationEast;
            case Direction.SOUTH:
                return attackAnimationSouth;
            case Direction.WEST:
                return attackAnimationWest;
            default:
                throw new System.InvalidOperationException("Invalid direction.");
        }
    }

    /// <summary>
    /// Returns the idle animation track for the given direction.
    /// </summary>
    /// <returns>the idle animation track for the given direction </returns>
    /// <param name="d">The direction of the track to get</param>
    public Sprite[] GetIdleAnimation(Direction d)
    {
        switch (d)
        {
            case Direction.NORTH:
                return idleAnimationNorth;
            case Direction.EAST:
                return idleAnimationEast;
            case Direction.SOUTH:
                return idleAnimationSouth;
            case Direction.WEST:
                return idleAnimationWest;
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
