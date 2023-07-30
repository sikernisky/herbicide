using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a Tile that borders the TileGrid and a WaterTile.
/// </summary>
public class ShoreTile : AnimatedTile
{
    /// <summary>
    /// true if enemies can walk across ShoreTiles.
    /// </summary>
    public override bool WALKABLE => false;

    /// <summary>
    /// Animation track for when this ShoreTile has a southern neighbor.
    /// </summary>
    [SerializeField]
    private Sprite[] topShoreAnimation;

    /// <summary>
    /// Animation track for when this ShoreTile has a northern neighbor.
    /// </summary>
    [SerializeField]
    private Sprite[] botShoreAnimation;

    /// <summary>
    /// TileType of a ShoreTile.
    /// </summary>
    protected override TileType type => TileType.SHORE;



    /// <summary>
    /// Defines this ShoreTile as a top or bot ShoreTile. A top ShoreTile
    /// has a southern neighbor and a WaterTile northern neighbor; a
    /// bot ShoreTile the opposite.
    /// </summary>
    /// <param name="x">The X-Coordinate of this ShoreTile.</param>
    /// <param name="y">The Y-Coordinate of this ShoreTile.</param>
    /// <param name="isTopShore">true if this ShoreTile is a top shore Tile;
    /// otherwise, false.</param>
    public void Define(int x, int y, bool isTopShore)
    {
        if (isTopShore) SetAnimationTrack(topShoreAnimation);
        else SetAnimationTrack(botShoreAnimation);
        base.Define(x, y);
        string nameAddOn = isTopShore ? "TOP" : "BOT";
        name = nameAddOn + " " + type.ToString() + " (" + x + ", " + y + ")";
    }


}
