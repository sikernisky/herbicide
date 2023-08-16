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
    /// Type of this GrassTile.
    /// </summary>
    protected override TileType type => Tile.TileType.GRASS;

    /// <summary>
    /// true if this GrassTile is at the edge of the grid.
    /// </summary>
    private bool isEdge;


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

    /// <summary>
    /// Marks this GrassTile as an edge Tile.
    /// </summary>
    public void MarkEdge()
    {
        isEdge = true;
    }

    /// <summary>
    /// Unmarks this GrassTile as an edge Tile.
    /// </summary>
    public void UnmarkEdge()
    {
        isEdge = false;
    }

    /// <summary>
    /// Returns true if this GrassTile lies on the edge of the TileGrid.
    /// </summary>
    /// <returns>true if this GrassTile is an edge Tile; otherwise, false.
    /// </returns>
    public bool IsEdgeTile()
    {
        return isEdge;
    }
}
