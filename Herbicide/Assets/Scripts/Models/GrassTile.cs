using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a Grass Tile. Grass Tiles can hold Flooring and 
/// certain IPlaceables.
/// </summary>
public class GrassTile : Tile
{
    /// <summary>
    /// Name of this GrassTile.
    /// </summary>
    public override string NAME => "GrassTile";

    /// <summary>
    /// Type of this GrassTile.
    /// </summary>
    protected override TileType type => Tile.TileType.GRASS;

    /// <summary>
    /// Grass Tiles are walkable.
    /// </summary>
    public override bool WALKABLE => true;

    /// <summary>
    /// ModelType of this GrassTile.
    /// </summary>
    public override ModelType TYPE => ModelType.GRASS_TILE;


    /// <summary>
    /// Defines this GrassTile and sets its sprite to represent a light
    /// GrassTile.
    /// </summary>
    /// <param name="x">The X-Coordinate of this GrassTile.</param>
    /// <param name="y">The Y-Coordinate of this GrassTile.</param>
    /// <param name="tileId">The local Tile Id of this GrassTile in Tiled.</param>
    public void Define(int x, int y, int tileId)
    {
        base.Define(x, y);
        SetSprite(TileFactory.GetTileSprite(type.ToString(), tileId));
    }
}
