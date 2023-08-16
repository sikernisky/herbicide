using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The most basic type of Flooring: soil. Gets placed on a base
/// Tile and can host different growables.
/// </summary>
public class SoilFlooring : Flooring
{
    /// <summary>
    /// Flooring type of this SoilFlooring
    /// </summary>
    protected override Flooring.FlooringType type => Flooring.FlooringType.SOIL;


    /// <summary>
    /// Returns the index representing the correct Sprite in this 
    /// SoilFlooring's tile set. The correct sprite is determined by whether its
    /// neighbors are null or valid. <br></br>
    /// </summary>
    /// <param name="neighbors">this SoilFlooring's neighbors</param>
    /// <returns>the index representing the correct Sprite in this 
    /// SoilFlooring's tile set.</returns>
    protected override int GetTilingIndex(ISurface[] neighbors)
    {
        bool hasNorth = neighbors[0] as SoilFlooring != null;
        bool hasEast = neighbors[1] as SoilFlooring != null;
        bool hasSouth = neighbors[2] as SoilFlooring != null;
        bool hasWest = neighbors[3] as SoilFlooring != null;

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
}
