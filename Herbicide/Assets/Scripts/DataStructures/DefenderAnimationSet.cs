using UnityEngine;

/// <summary>
/// DataStructure to hold all animations for a Defender.
/// </summary>
[CreateAssetMenu(fileName = "New Defender Animation Set", menuName = "Defender Animation Set")]
[System.Serializable]
public class DefenderAnimationSet : ScriptableObject
{
    #region Placement Tracks

    /// <summary>
    /// Animation track for placing the tier 1 Defender.
    /// </summary>
    public Sprite[] placementTrackTier1;

    /// <summary>
    /// Animation track for placing the tier 2 Defender.
    /// </summary>
    public Sprite[] placementTrackTier2;

    /// <summary>
    /// Animation track for placing the tier 3Defender.
    /// </summary>
    public Sprite[] placementTrackTier3;

    #endregion

    #region Attack Tracks

    /// <summary>
    /// Attack animations for the tier 1 Defender facing north.
    /// </summary>
    public Sprite[] attackAnimationNorthTier1;

    /// <summary>
    /// Attack animations for the tier 1 Defender facing east.
    /// </summary>
    public Sprite[] attackAnimationEastTier1;

    /// <summary>
    /// Attack animations for the tier 1 Defender facing south.
    /// </summary>
    public Sprite[] attackAnimationSouthTier1;

    /// <summary>
    /// Attack animations for the tier 1 Defender facing west.
    /// </summary>
    public Sprite[] attackAnimationWestTier1;

    /// <summary>
    /// Attack animations for the tier 2 Defender facing north.
    /// </summary>
    public Sprite[] attackAnimationNorthTier2;

    /// <summary>
    /// Attack animations for the tier 2 Defender facing east.
    /// </summary>
    public Sprite[] attackAnimationEastTier2;

    /// <summary>
    /// Attack animations for the tier 2 Defender facing south.
    /// </summary>
    public Sprite[] attackAnimationSouthTier2;

    /// <summary>
    /// Attack animations for the tier 2 Defender facing west.
    /// </summary>
    public Sprite[] attackAnimationWestTier2;

    /// <summary>
    /// Attack animations for the tier 3 Defender facing north.
    /// </summary>
    public Sprite[] attackAnimationNorthTier3;

    /// <summary>
    /// Attack animations for the tier 3 Defender facing east.
    /// </summary>
    public Sprite[] attackAnimationEastTier3;

    /// <summary>
    /// Attack animations for the tier 3 Defender facing south.
    /// </summary>
    public Sprite[] attackAnimationSouthTier3;

    /// <summary>
    /// Attack animations for the tier 3 Defender facing west.
    /// </summary>
    public Sprite[] attackAnimationWestTier3;

    #endregion

    #region Idle Tracks

    /// <summary>
    /// Idle animations for the tier 1 Defender facing north.
    /// </summary>
    public Sprite[] idleAnimationNorthTier1;

    /// <summary>
    /// Idle animations for the tier 1 Defender facing east.
    /// </summary>
    public Sprite[] idleAnimationEastTier1;

    /// <summary>
    /// Idle animations for the tier 1 Defender facing south.
    /// </summary>
    public Sprite[] idleAnimationSouthTier1;

    /// <summary>
    /// Idle animations for the tier 1 Defender facing west.
    /// </summary>
    public Sprite[] idleAnimationWestTier1;

    /// <summary>
    /// Idle animations for the tier 2 Defender facing north.
    /// </summary>
    public Sprite[] idleAnimationNorthTier2;

    /// <summary>
    /// Idle animations for the tier 2 Defender facing east.
    /// </summary>
    public Sprite[] idleAnimationEastTier2;

    /// <summary>
    /// Idle animations for the tier 2 Defender facing south.
    /// </summary>
    public Sprite[] idleAnimationSouthTier2;

    /// <summary>
    /// Idle animations for the tier 2 Defender facing west.
    /// </summary>
    public Sprite[] idleAnimationWestTier2;

    /// <summary>
    /// Idle animations for the tier 3 Defender facing north.
    /// </summary>
    public Sprite[] idleAnimationNorthTier3;

    /// <summary>
    /// Idle animations for the tier 3 Defender facing east.
    /// </summary>
    public Sprite[] idleAnimationEastTier3;

    /// <summary>
    /// Idle animations for the tier 3 Defender facing south.
    /// </summary>
    public Sprite[] idleAnimationSouthTier3;

    /// <summary>
    /// Idle animations for the tier 3 Defender facing west.
    /// </summary>
    public Sprite[] idleAnimationWestTier3;

    #endregion

    #region Methods

    /// <summary>
    /// Returns the attack animation track for the given direction.
    /// </summary>
    /// <param name="d">The direction of the track to get</param>
    /// <param name="tier">The tier of the track to get</param>
    /// <returns>the attack animation track for the given direction </returns>
    public Sprite[] GetAttackAnimation(Direction d, int tier)
    {
        switch (d)
        {
            case Direction.NORTH:
                if (tier == 1) return attackAnimationNorthTier1;
                else if (tier == 2) return attackAnimationNorthTier2;
                else return attackAnimationNorthTier3;
            case Direction.EAST:
                if (tier == 1) return attackAnimationEastTier1;
                else if (tier == 2) return attackAnimationEastTier2;
                else return attackAnimationEastTier3;
            case Direction.SOUTH:
                if (tier == 1) return attackAnimationSouthTier1;
                else if (tier == 2) return attackAnimationSouthTier2;
                else return attackAnimationSouthTier3;
            case Direction.WEST:
                if (tier == 1) return attackAnimationWestTier1;
                else if (tier == 2) return attackAnimationWestTier2;
                else return attackAnimationWestTier3;
            default:
                throw new System.InvalidOperationException("Invalid direction.");
        }
    }

    /// <summary>
    /// Returns the idle animation track for the given direction.
    /// </summary>
    /// <returns>the idle animation track for the given direction </returns>
    /// <param name="d">The direction of the track to get</param>
    /// <param name="tier">The tier of the track to get</param>
    public Sprite[] GetIdleAnimation(Direction d, int tier)
    {
        switch (d)
        {
            case Direction.NORTH:
                if (tier == 1) return idleAnimationNorthTier1;
                else if (tier == 2) return idleAnimationNorthTier2;
                else return idleAnimationNorthTier3;
            case Direction.EAST:
                if (tier == 1) return idleAnimationEastTier1;
                else if (tier == 2) return idleAnimationEastTier2;
                else return idleAnimationEastTier3;
            case Direction.SOUTH:
                if (tier == 1) return idleAnimationSouthTier1;
                else if (tier == 2) return idleAnimationSouthTier2;
                else return idleAnimationSouthTier3;
            case Direction.WEST:
                if (tier == 1) return idleAnimationWestTier1;
                else if (tier == 2) return idleAnimationWestTier2;
                else return idleAnimationWestTier3;
            default:
                throw new System.InvalidOperationException("Invalid direction.");
        }
    }

    /// <summary>
    /// Returns the placement animation track.
    /// </summary>
    /// <param name="tier">The tier of the track to get</param>
    /// <returns>the placement animation track </returns>
    public Sprite[] GetPlacementAnimation(int tier)
    {
        if (tier == 1) return placementTrackTier1;
        else if (tier == 2) return placementTrackTier2;
        else return placementTrackTier3;
    }

    #endregion
}
