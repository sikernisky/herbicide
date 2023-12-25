using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents impassable water.
/// </summary>
public class WaterTile : Tile
{
    /// <summary>
    /// Name of this WaterTile.
    /// </summary>
    public override string NAME => "WaterTile";

    /// <summary>
    /// WaterTiles are not walkable.
    /// </summary>
    public override bool WALKABLE => false;

    /// <summary>
    /// ModelType of a Water Tile.
    /// </summary>
    public override ModelType TYPE => ModelType.WATER_TILE;

    /// <summary>
    /// Type of a WaterTile.
    /// </summary>
    protected override TileType type => TileType.WATER;
}
