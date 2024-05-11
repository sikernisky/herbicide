using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a Stone Wall. These are the most basic type of Wall:
/// they have infinite health, are immovable, and are impassable.
/// </summary>
public class StoneWall : Wall
{
    /// <summary>
    /// Name of this Stone Wall.
    /// </summary>
    public override string NAME => "Stone Wall";

    /// <summary>
    /// Type of a Stone Wall.
    /// </summary>
    public override ModelType TYPE => ModelType.STONE_WALL;

    /// <summary>
    /// Returns the GameObject that represents this StoneWall on the grid.
    /// </summary>
    /// <returns>the GameObject that represents this StoneWall on the grid</returns>
    public override GameObject Copy() { throw new System.NotImplementedException(); }

    /// <summary>
    /// Returns true if this StoneWall is dead, false otherwise.
    /// </summary>
    /// <returns>true if this StoneWall is dead, false otherwise.</returns>
    public override bool Dead() { return GetHealth() <= 0; }

    public override Sprite[] GetPlacementTrack()
    {
        // 
    }

    /// <summary>
    /// Sets the collider properties of this StoneWall.
    /// </summary>
    public override void SetColliderProperties() { return; }

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
        if (hasEast && hasWest && !hasSouth && !hasNorth) return 0;
        if (!hasEast && hasWest && !hasSouth && !hasNorth) return 0;
        if (!hasEast && !hasWest && hasSouth && !hasNorth) return 0;
        if (hasEast && !hasWest && hasSouth && !hasNorth) return 3;

        // WE LEFT OFF HERE, NEED TO REDESIGN TILESET. 

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
}
