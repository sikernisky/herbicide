using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a GameObject that can be placed on a Tile to expand its
/// behavior and complexity. 
/// </summary>
public abstract class Flooring : MonoBehaviour, ISurface
{
    /// <summary>
    /// Tile set for this Flooring
    /// </summary>
    [SerializeField]
    private List<Sprite> tileSet;

    /// <summary>
    /// Index of the Sprite in tile set this Flooring takes on
    /// </summary>
    private int tilingIndex;

    /// <summary>
    /// Type of this Flooring
    /// </summary>
    protected abstract TileGrid.FlooringType type { get; }

    /// <summary>
    /// Type of Tile this flooring is on
    /// </summary>
    private Tile.TileType typeOn;

    /// <summary>
    /// This Flooring's four neighbors.
    /// </summary>
    private Flooring[] neighbors;

    /// <summary>
    /// This Flooring's SpriteRenderer component
    /// </summary>
    private SpriteRenderer flooringRenderer;

    /// <summary>
    /// The IPlaceable on this Flooring.
    /// </summary>
    private IPlaceable occupant;


    /// <summary>
    /// Defines this Flooring on a Tile, setting its Sprite based on an
    /// index.
    /// </summary>
    /// <param name="typeOn">type of Tile this Flooring sits on</param>
    /// <param name="neighbors">this Flooring's neighbors</param>
    public virtual void Define(Tile.TileType typeOn, ISurface[] neighbors)
    {
        if (neighbors == null) return;
        if (flooringRenderer == null)
            flooringRenderer = GetComponent<SpriteRenderer>();

        UpdateNeighbors(neighbors);
        this.typeOn = typeOn;
    }

    /// <summary>
    /// Sets the neighbors of this Flooring.
    /// </summary>
    /// <param name="newNeighbors">this Floorings's new neighbors.</param>
    public void UpdateNeighbors(ISurface[] newNeighbors)
    {
        Flooring[] flooringNeighbors = new Flooring[newNeighbors.Length];
        for (int i = 0; i < newNeighbors.Length; i++)
        {
            flooringNeighbors[i] = newNeighbors[i] as Flooring;
        }
        SetTilingIndex(GetTilingIndex(flooringNeighbors));
        neighbors = flooringNeighbors;
    }

    /// <summary>
    /// Returns this Flooring's four neighbors.
    /// </summary>
    /// <returns>this Flooring's four neighbors.</returns>
    public ISurface[] GetNeighbors()
    {
        return neighbors;
    }

    /// <summary>
    /// Sets this Flooring's tiling index and updates its Sprite to reflect
    /// that index.
    /// </summary>
    /// <param name="newIndex">the new index to set to.</param>
    public void SetTilingIndex(int newIndex)
    {
        if (!ValidTilingIndex(newIndex)) return;
        if (flooringRenderer == null) flooringRenderer = GetComponent<SpriteRenderer>();
        flooringRenderer.sprite = tileSet[newIndex];
        tilingIndex = newIndex;
    }

    /// <summary>
    /// Returns true if an index is a valid tiling index.
    /// </summary>
    /// <param name="index">the index to check</param>
    /// <returns>true if the index is a valid tiling index; otherwise,
    /// false.</returns>
    private bool ValidTilingIndex(int index)
    {
        return index >= 0 && index < tileSet.Count;
    }

    /// <summary>
    /// Returns true if this Flooring hosts an IPlaceable.
    /// </summary>
    /// <returns>true if this Flooring hosts an IPlaceable; otherwise, false.
    /// </returns>
    public bool Occupied()
    {
        return occupant != null;
    }


    /// <summary>
    /// Returns true if an IPlaceable can be placed on this Flooring.
    /// If it can, places the IPlaceable.
    /// </summary>
    /// <param name="candidate">The IPlaceable to place.</param>
    /// <param name="neighbors">This Flooring's neighbors.</param>
    /// <returns>true if an IPlaceable can be placed on this Flooring;
    /// otherwise, false.</returns>
    public bool Place(IPlaceable candidate, ISurface[] neighbors)
    {
        if (candidate == null || neighbors == null) return false;
        if (!CanPlace(candidate, neighbors)) return false;

        //Placement logic here
        SetTilingIndex(GetTilingIndex(neighbors));

        return true;

    }

    /// <summary>
    /// Returns true if an IPlaceable can be placed on this Flooring.
    /// </summary>
    /// <param name="candidate">The IPlaceable to place.</param>
    /// <param name="neighbors">This Flooring's neighbors.</param>
    /// <returns>true if an IPlaceable can be placed on this Flooring;
    /// otherwise, false.</returns>
    public bool CanPlace(IPlaceable candidate, ISurface[] neighbors)
    {
        if (candidate == null || neighbors == null) return false;

        return true;
    }

    /// <summary>
    /// Returns true if there is an IPlaceable on this Flooring that can
    /// be removed. If so, removes the IPlaceable.
    /// </summary>
    /// <param name="neighbors">This Flooring's neighbors.</param>
    /// <returns>true if an IPlaceable can be removed from this Flooring;
    /// otherwise, false.</returns>
    public bool Remove(ISurface[] neighbors)
    {
        if (!Occupied() || neighbors == null) return false;

        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Returns true if there is an IPlaceable on this Flooring that can
    /// be removed.
    /// </summary>
    /// <param name="neighbors">This Flooring's neighbors.</param>
    /// <returns>true if an IPlaceable can be removed from this Flooring;
    /// otherwise, false.</returns>
    public bool CanRemove(IPlaceable candidate, ISurface[] neighbors)
    {
        if (!Occupied() || candidate == null || neighbors == null) return false;

        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Returns the index representing the correct Sprite in this 
    /// Flooring's tile set. The correct sprite is determined by whether its
    /// neighbors are null or valid. <br></br>
    /// </summary>
    /// <param name="neighbors">this Flooring's neighbors</param>
    /// <returns>the index representing the correct Sprite in this 
    /// Flooring's tile set.</returns>
    protected abstract int GetTilingIndex(ISurface[] neighbors);
}
