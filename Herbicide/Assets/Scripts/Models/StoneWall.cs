using UnityEngine;

/// <summary>
/// Represents an impassable Stone Wall.
/// </summary>
public class StoneWall : Wall
{
    #region Fields

    #endregion

    #region Stats

    /// <summary>
    /// Type of a Stone Wall.
    /// </summary>
    public override ModelType TYPE => ModelType.STONE_WALL;

    /// <summary>
    /// Starting attack range of a StoneWall.
    /// </summary>
    public override float BASE_MAIN_ACTION_RANGE => 0f;

    /// <summary>
    /// Maximum attack range of a StoneWall.
    /// </summary>
    public override float MAX_MAIN_ACTION_RANGE => float.MaxValue;

    /// <summary>
    /// Minimum attack range of a StoneWall.
    /// </summary>
    public override float MIN_MAIN_ACTION_RANGE => 0f;

    /// <summary>
    /// Starting attack cooldown of a StoneWall.
    /// </summary>
    public override float BASE_MAIN_ACTION_SPEED => float.MaxValue;

    /// <summary>
    /// Maximum attack cooldown of a StoneWall.
    /// </summary>
    public override float MAX_MAIN_ACTION_SPEED => float.MaxValue;

    /// <summary>
    /// Starting chase range of a StoneWall.
    /// </summary>
    public override float BASE_CHASE_RANGE => float.MaxValue;

    /// <summary>
    /// Maximum chase range of a StoneWall.
    /// </summary>
    public override float MAX_CHASE_RANGE => float.MaxValue;

    /// <summary>
    /// Minimum chase range of a StoneWall.
    /// </summary>
    public override float MIN_CHASE_RANGE => 0f;

    /// <summary>
    /// Starting movement speed of a StoneWall.
    /// </summary>
    public override float BASE_MOVEMENT_SPEED => 0f;

    /// <summary>
    /// Maximum movement speed of a StoneWall.
    /// </summary>
    public override float MAX_MOVEMENT_SPEED => 0f;

    /// <summary>
    /// Minimum movement speed of a StoneWall.
    /// </summary>
    public override float MIN_MOVEMENT_SPEED => 0f;

    #endregion

    #region Methods

    /// <summary>
    /// Returns the GameObject that represents this StoneWall on the grid.
    /// </summary>
    /// <returns>the GameObject that represents this StoneWall on the grid</returns>
    public override GameObject CreateNew() => WallFactory.GetWallPrefab(TYPE);

    /// <summary>
    /// Returns true if this StoneWall is dead, false otherwise.
    /// </summary>
    /// <returns>true if this StoneWall is dead, false otherwise.</returns>
    public override bool Dead() => GetHealth() <= 0;

    /// <summary>
    /// Returns the placement track for this StoneWall.
    /// </summary>
    /// <returns> the placement track for this StoneWall.</returns>
    public override Sprite[] GetPlacementTrack() => throw new System.NotSupportedException();

    /// <summary>
    /// Returns the index representing the correct Sprite for this StoneWall.
    /// </summary>
    /// <param name="neighbors"> the four neighbors that surround this StoneWall.</param>
    /// <returns> the index representing the correct Sprite for this StoneWall.</returns>
    protected override int GetTilingIndex(PlaceableObject[] neighbors)
    {
        bool hasNorth = neighbors[0] as Wall != null;
        bool hasEast = neighbors[1] as Wall != null;
        bool hasSouth = neighbors[2] as Wall != null;
        bool hasWest = neighbors[3] as Wall != null;

        if (hasEast && !hasWest && !hasSouth && !hasNorth) return 0;
        if (hasEast && hasWest && !hasSouth && !hasNorth) return 1;
        if (!hasEast && hasWest && !hasSouth && !hasNorth) return 2;
        if (!hasEast && !hasWest && hasSouth && !hasNorth) return 3;
        if (hasEast && !hasWest && hasSouth && !hasNorth) return 4;
        if (hasEast && hasWest && hasSouth && !hasNorth) return 5;
        if (!hasEast && hasWest && hasSouth && !hasNorth) return 6;
        if (!hasEast && !hasWest && hasSouth && hasNorth) return 7;
        if (hasEast && !hasWest && hasSouth && hasNorth) return 8;
        if (hasEast && hasWest && hasSouth && hasNorth) return 9;
        if (!hasEast && hasWest && hasSouth && hasNorth) return 10;
        if (!hasEast && !hasWest && !hasSouth && hasNorth) return 11;
        if (hasEast && !hasWest && !hasSouth && hasNorth) return 12;
        if (hasEast && hasWest && !hasSouth && hasNorth) return 13;
        if (!hasEast && hasWest && !hasSouth && hasNorth) return 14;
        return 15; // has no neighbors
    }

    #endregion
}
