using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents impassable water.
/// </summary>
public class WaterTile : Tile
{
    /// <summary>
    /// true if WaterTiles are walkable.
    /// </summary>
    public override bool WALKABLE => false;

    /// <summary>
    /// Type of a WaterTile.
    /// </summary>
    protected override TileType type => TileType.WATER;
}
