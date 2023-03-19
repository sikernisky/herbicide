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
    /// Sprite representing a light GrassTile.
    /// </summary>
    [SerializeField]
    private Sprite lightGrass;

    /// <summary>
    /// Sprite representing a dark GrassTile.
    /// </summary>
    [SerializeField]
    private Sprite darkGrass;

    /// <summary>
    /// Type of this GrassTile.
    /// </summary>
    protected override TileType type => Tile.TileType.GRASS;

    /// <summary>
    /// true if an Enemy can walk on this GrassTile.
    /// </summary>
    public override bool WALKABLE => true;
}
