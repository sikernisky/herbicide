using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents an Edge Tile that borders some water and
/// some terrain.
/// </summary>
public class ShoreTile : EdgeTile
{
    /// <summary>
    /// Name of this ShoreTile.
    /// </summary>
    public override string NAME => "ShoreTile";

    /// <summary>
    /// Tile type of a ShoreTile.
    /// </summary>
    protected override TileType type => TileType.SHORE;

    /// <summary>
    /// Shore Tiles are not walkable.
    /// </summary>
    public override bool WALKABLE => false;

    /// <summary>
    /// Returns the Sprite component that represents this SeedToken in
    /// the Inventory.
    /// </summary>
    /// <returns>the Sprite component that represents this SeedToken in
    /// the Inventory.</returns>
    public override Sprite GetInventorySprite()
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Returns a Sprite that represents this SeedToken when it is
    /// being placed.
    /// </summary>
    /// <returns> a Sprite that represents this SeedToken when it is
    /// being placed.</returns>
    public override Sprite GetPlacementSprite()
    {
        throw new System.NotImplementedException();
    }
}
