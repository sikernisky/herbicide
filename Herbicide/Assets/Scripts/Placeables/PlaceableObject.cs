using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract class for something that can be placed on the
/// TileGrid.
/// </summary>
public abstract class PlaceableObject : MonoBehaviour, IPlaceable
{
    /// <summary>
    /// SpriteRenderer component for this PlaceableObject;
    /// </summary>
    private SpriteRenderer placeableRenderer;

    /// <summary>
    /// The four neighboring PlaceableObjects
    /// </summary>
    private PlaceableObject[] neighbors;

    /// <summary>
    /// Returns the Sprite component that represents this PlaceableObject in
    /// the Inventory.
    /// </summary>
    /// <returns>the Sprite component that represents this PlaceableObject in
    /// the Inventory.</returns>
    public abstract Sprite GetInventorySprite();

    /// <summary>
    /// Initializes this PlaceableObject. Called when it is placed on the TileGrid.
    /// Sets the SpriteRenderer component.
    /// </summary>
    /// <param name="neighbors">The neighboring PlaceableObjects</param>
    public virtual void Setup(PlaceableObject[] neighbors)
    {
        if (neighbors == null) return;

        placeableRenderer = GetComponent<SpriteRenderer>();
        this.neighbors = neighbors;
    }

    /// <summary>
    /// Sets the Sprite component of this PlaceableObject.
    /// </summary>
    /// <param name="s">the Sprite to set to</param>
    public virtual void SetSprite(Sprite s)
    {
        if (s != null) placeableRenderer.sprite = s;
    }

    /// <summary>
    /// Returns a Sprite that represents this PlaceableObject when it is
    /// being placed.
    /// </summary>
    /// <returns> a Sprite that represents this PlaceableObject when it is
    /// being placed.</returns>
    public abstract Sprite GetPlacementSprite();

    /// <summary>
    /// Returns the GameObject that represents this PlaceableObject on the grid.
    /// </summary>
    /// <returns>the GameObject that represents this PlaceableObject on the grid.
    /// </returns>
    public abstract GameObject MakePlaceableObject();
}
