using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a placeable Tree.
/// </summary>
public abstract class Tree : PlaceableObject
{
    /// <summary>
    /// Type of this Tree.
    /// </summary>
    public abstract TreeType type { get; }

    /// <summary>
    /// Represents a type of Tree.
    /// </summary>
    public enum TreeType
    {
        RED,
        BLUE,
        WHITE,
        YELLOW
    }

    /// <summary>
    /// Returns a Sprite component that represents this Tree in an 
    /// Inventoryslot.
    /// </summary>
    /// <returns>a Sprite component that represents this Tree in an 
    /// Inventoryslot.</returns>
    public override Sprite GetInventorySprite()
    {
        return TreeFactory.GetTreeInventorySprite(type);
    }

    /// <summary>
    /// Returns a Sprite component that represents this Tree in an 
    /// Inventoryslot.
    /// </summary>
    /// <returns>a Sprite component that represents this Tree in an 
    /// Inventoryslot.</returns>
    public override Sprite GetPlacementSprite()
    {
        return TreeFactory.GetTreePlacedSprite(type);
    }

    /// <summary>
    /// Returns a Tree GameObject that can be placed on the grid.
    /// </summary>
    /// <returns>a Tree GameObject that can be placed on the grid.</returns>
    public override GameObject MakePlaceableObject()
    {
        return Instantiate(TreeFactory.GetTreePrefab(type));
    }

    /// <summary>
    /// Initializes this Tree when placed on the TileGrid. Sets its Sprite to
    /// its placed Sprite.
    /// </summary>
    /// <param name="neighbors">The PlaceableObjects surrounding this Tree.</param>
    public override void Setup(PlaceableObject[] neighbors)
    {
        base.Setup(neighbors);
        SetSprite(TreeFactory.GetTreePlacedSprite(type));
    }

}
