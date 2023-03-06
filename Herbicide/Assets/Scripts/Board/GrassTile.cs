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
    /// Defines this GrassTile to be within a TileGrid at coordinates (x, y).
    /// Sets its Sprite to be a Light Grass or Dark Grass depending on the
    /// parities of its coordinates.
    /// </summary>
    /// <param name="x">the x-coordinate of this GrassTile</param>
    /// <param name="y">the y-coordinate of this GrassTile</param>
    /// <param name="type">the type of this GrassTile</param>
    public override void Define(int x, int y)
    {
        base.Define(x, y);
        if (DifferentParities(x, y)) SetSprite(lightGrass);
        else SetSprite(darkGrass);
    }
}
