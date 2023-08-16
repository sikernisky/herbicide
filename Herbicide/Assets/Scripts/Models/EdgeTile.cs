using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a Tile that borders at least one unwalkable boundary and 
/// at least one walkable Tile.
/// </summary>
public abstract class EdgeTile : Tile
{

    /// <summary>
    /// Defines this EdgeTile to be within a TileGrid at coordinates (x, y).
    /// Sets its sprite to reflect the passed in tiling index.
    /// </summary>
    /// <param name="x">the x-coordinate of this EdgeTile</param>
    /// <param name="y">the y-coordinate of this EdgeTile</param>
    /// <param name="tilingIndex"> the tiling index of this EdgeTile</param>
    public void Define(int x, int y, int tilingIndex)
    {
        base.Define(x, y);
        SetSprite(EdgeFactory.GetEdgeSprite(GetTileType().ToString(), tilingIndex));
    }
}
