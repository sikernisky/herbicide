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

    #region Main Action Tracks

    /// <summary>
    /// Main action animations for the tier 1 Defender facing north.
    /// </summary>
    public Sprite[] mainActionAnimationNorthTier1;

    /// <summary>
    /// Main Action animations for the tier 1 Defender facing east.
    /// </summary>
    public Sprite[] mainActionAnimationEastTier1;

    /// <summary>
    /// Main Action animations for the tier 1 Defender facing south.
    /// </summary>
    public Sprite[] mainActionAnimationSouthTier1;

    /// <summary>
    /// Main Action animations for the tier 1 Defender facing west.
    /// </summary>
    public Sprite[] mainActionAnimationWestTier1;

    /// <summary>
    /// Main Action animations for the tier 2 Defender facing north.
    /// </summary>
    public Sprite[] mainActionAnimationNorthTier2;

    /// <summary>
    /// Main Action animations for the tier 2 Defender facing east.
    /// </summary>
    public Sprite[] mainActionAnimationEastTier2;

    /// <summary>
    /// Main Action animations for the tier 2 Defender facing south.
    /// </summary>
    public Sprite[] mainActionAnimationSouthTier2;

    /// <summary>
    /// Main Action animations for the tier 2 Defender facing west.
    /// </summary>
    public Sprite[] mainActionAnimationWestTier2;

    /// <summary>
    /// Main Action animations for the tier 3 Defender facing north.
    /// </summary>
    public Sprite[] mainActionAnimationNorthTier3;

    /// <summary>
    /// Main Action animations for the tier 3 Defender facing east.
    /// </summary>
    public Sprite[] mainActionAnimationEastTier3;

    /// <summary>
    /// Main Action animations for the tier 3 Defender facing south.
    /// </summary>
    public Sprite[] mainActionAnimationSouthTier3;

    /// <summary>
    /// Main Action animations for the tier 3 Defender facing west.
    /// </summary>
    public Sprite[] mainActionAnimationWestTier3;

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
    /// Returns the main action animation track for the given direction.
    /// </summary>
    /// <param name="d">The direction of the track to get</param>
    /// <param name="tier">The tier of the track to get</param>
    /// <returns>the main action animation track for the given direction </returns>
    public Sprite[] GetMainActionAnimation(Direction d, int tier)
    {
        switch (d)
        {
            case Direction.NORTH:
                if (tier == 1) return mainActionAnimationNorthTier1;
                else if (tier == 2) return mainActionAnimationNorthTier2;
                else return mainActionAnimationNorthTier3;
            case Direction.EAST:
                if (tier == 1) return mainActionAnimationEastTier1;
                else if (tier == 2) return mainActionAnimationEastTier2;
                else return mainActionAnimationEastTier3;
            case Direction.SOUTH:
                if (tier == 1) return mainActionAnimationSouthTier1;
                else if (tier == 2) return mainActionAnimationSouthTier2;
                else return mainActionAnimationSouthTier3;
            case Direction.WEST:
                if (tier == 1) return mainActionAnimationWestTier1;
                else if (tier == 2) return mainActionAnimationWestTier2;
                else return mainActionAnimationWestTier3;
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
