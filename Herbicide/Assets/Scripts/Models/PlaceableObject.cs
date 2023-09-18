using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract class for something that can be placed on the
/// TileGrid.
/// </summary>
public abstract class PlaceableObject : MonoBehaviour, ISlottable
{
    /// <summary>
    /// Name of this PlaceableObject.
    /// </summary>
    protected abstract string NAME { get; }

    /// <summary>
    /// How much currency is required to place this PlaceableObject.
    /// </summary>
    protected abstract int COST { get; }

    /// <summary>
    /// true if this PlaceableObject has been defined; otherwise, false.
    /// </summary>
    private bool defined;

    /// <summary>
    /// SpriteRenderer component for this PlaceableObject
    /// </summary>
    [SerializeField]
    private SpriteRenderer placeableRenderer;

    /// <summary>
    /// The four neighboring PlaceableObjects
    /// </summary>
    private PlaceableObject[] neighbors;

    /// <summary>
    /// Coordinates of the Tile on which this PlaceableObject sits.
    /// </summary>
    private Vector2Int coordinates;

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
    public virtual void Define(Vector2Int coordinates, PlaceableObject[] neighbors)
    {
        if (neighbors == null) return;

        placeableRenderer = GetComponent<SpriteRenderer>();
        this.coordinates = coordinates;
        this.neighbors = neighbors;
        defined = true;
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
    /// Sets this PlaceableObject's SpriteRenderer's color.
    /// </summary>
    /// <param name="color">the color to set to.</param>
    public void SetColor(Color32 newColor)
    {
        if (placeableRenderer == null) return;
        placeableRenderer.color = newColor;
    }

    /// <summary>
    /// Sets this PlaceableObject's SpriteRenderer component's sorting
    /// order (order in layer).
    /// </summary>
    public void SetSortingOrder(int layer)
    {
        if (layer >= 0) placeableRenderer.sortingOrder = layer;
    }

    /// <summary>
    /// Returns this PlaceableObject's sorting order.
    /// </summary>
    /// <returns>this PlaceableObject's sorting order.</returns>
    public int GetSortingOrder()
    {
        return placeableRenderer.sortingOrder;
    }

    /// <summary>
    /// Sets the neighbors of this PlaceableObject.
    /// </summary>
    /// <param name="neighbors">the new neighbors of this PlaceableObject.</param>
    protected void UpdatePlaceableNeighbors(PlaceableObject[] neighbors)
    {
        if (neighbors == null) return;
        this.neighbors = neighbors;
    }

    /// <summary>
    /// Returns a copy of the array containing this PlaceableObject's neighbors.
    /// </summary>
    protected PlaceableObject[] GetPlaceableNeighbors()
    {
        PlaceableObject[] copyNeighbors = new PlaceableObject[GetPlaceableNeighbors().Length];
        for (int i = 0; i < GetPlaceableNeighbors().Length; i++)
        {
            copyNeighbors[i] = GetPlaceableNeighbors()[i];
        }
        return copyNeighbors;
    }

    /// <summary>
    /// Returns true if this PlaceableObject is defined.
    /// </summary>
    /// <returns>true if this PlaceableObject is defined; otherwise, 
    /// false. </returns>
    public bool Defined()
    {
        return defined;
    }

    /// <summary>
    /// Returns the X-Coordinate of the Tile on which this PlaceableObject sits.
    /// </summary>
    /// <returns>the X-Coordinate of the Tile on which this PlaceableObject sits.</returns>
    public int GetX()
    {
        return coordinates.x;
    }

    /// <summary>
    /// Returns the Y-Coordinate of the Tile on which this PlaceableObject sits.
    /// </summary>
    /// <returns>the Y-Coordinate of the Tile on which this PlaceableObject sits.</returns>
    public int GetY()
    {
        return coordinates.y;
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


    /// <summary>
    /// Returns a GameObject that holds a SpriteRenderer component with
    /// this PlaceableObject's placed Sprite. No other components are
    /// copied. 
    /// </summary>
    /// <returns>A GameObject with a SpriteRenderer component. </returns>
    public abstract GameObject MakeHollowObject();

    /// <summary>
    /// Returns the amount of currency required to place this PlaceableObject
    /// from the Inventory.
    /// </summary>
    /// <returns>the amount of currency required to place this PlaceableObject
    /// from the Inventory.</returns>
    public virtual int GetCost()
    {
        return COST;
    }

    /// <summary>
    /// Returns this PlaceableObject's name.
    /// </summary>
    /// <returns>this PlaceableObject's name.</returns>
    public string GetName()
    {
        return NAME;
    }
}
