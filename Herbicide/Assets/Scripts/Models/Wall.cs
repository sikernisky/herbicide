using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a Wall. 
/// </summary>
public abstract class Wall : PlaceableObject
{
    /// <summary>
    /// Starting health of a Wall
    /// </summary>
    public override int BASE_HEALTH => int.MaxValue;

    /// <summary>
    /// Maximum health of a Wall
    /// </summary> 
    public override int MAX_HEALTH => int.MaxValue;
    /// <summary>
    /// Minimum health of a Wall
    /// </summary>
    public override int MIN_HEALTH => 0;

    /// <summary>
    /// true if this Wall is an occupier, false otherwise.
    /// </summary>
    public override bool OCCUPIER => true;

    /// <summary>
    /// Index of the Sprite in tile set this Wall takes on
    /// </summary>
    private int tilingIndex;

    /// <summary>
    /// Unique types of Walls.
    /// </summary>
    public enum FlooringType
    {
        STONE
    }

    /// <summary>
    /// Updates this Wall with its newest four neighbors. Sets its sprite
    /// accordingly. /// 
    /// </summary>
    /// <param name="neighbors"> The four neighbors that surround this Wall.</param>
    public override void UpdateNeighbors(PlaceableObject[] neighbors)
    {
        base.UpdateNeighbors(neighbors);
    }

    protected abstract void SetTilingIndex();

    protected abstract int GetTilingIndex();



}
