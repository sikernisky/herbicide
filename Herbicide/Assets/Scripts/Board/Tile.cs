using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Represents a set of space in the TileGrid with unique (X, Y) coordinates.
/// Tiles may be floored and placed on.
/// </summary>
public abstract class Tile : MonoBehaviour, ISurface
{
    /// <summary>
    /// (X, Y) coordinates of this Tile.
    /// </summary>
    private Vector2Int coordinates;

    /// <summary>
    /// The IPlaceable on this Tile; null if Tile is unoccupied.
    /// </summary>
    private PlaceableObject occupant;

    /// <summary>
    /// The flooring on this Tile; null if there is none.
    /// </summary>
    private Flooring flooring;

    /// <summary>
    /// true if this Tile is spawned in a TileGrid.
    /// </summary>
    private bool defined;

    /// <summary>
    /// This Tile's SpriteRenderer component.
    /// </summary>
    private SpriteRenderer tileRenderer;

    /// <summary>
    /// This Tile's four neighbors.
    /// </summary>
    private Tile[] neighbors;

    /// <summary>
    /// This Tile's Type.
    /// </summary>
    protected abstract TileType type { get; }

    /// <summary>
    /// Represents a Type of Tile.
    /// </summary>
    public enum TileType
    {
        GRASS
    }


    /// <summary>
    /// Defines this Tile to be within a TileGrid at coordinates (x, y).
    /// </summary>
    /// <param name="x">the x-coordinate of this Tile</param>
    /// <param name="y">the y-coordinate of this Tile</param>
    /// <param name="texture">the Sprite asset of this Tile</param>
    public virtual void Define(int x, int y, Sprite texture)
    {
        //Safety checks
        if (defined || x < 0 || y < 0) return;
        if (texture == null) return;

        coordinates = new Vector2Int(x, y);
        tileRenderer = GetComponent<SpriteRenderer>();
        SetSprite(texture);
        name = type.ToString() + " (" + x + ", " + y + ")";
        defined = true;
    }

    /// <summary>
    /// Returns the X-Coordinate of this Tile.
    /// </summary>
    /// <returns>the X-Coordinate of this Tile.</returns>
    public int GetX()
    {
        AssertDefined();
        return coordinates.x;
    }

    /// <summary>
    /// Returns the Y-Coordinate of this Tile.
    /// </summary>
    /// <returns>the Y-Coordinate of this Tile.</returns>
    public int GetY()
    {
        AssertDefined();
        return coordinates.y;
    }

    /// <summary>
    /// Returns true if this Tile is occupied.
    /// </summary>
    /// <returns>true if this Tile is occupied.</returns>
    public bool Occupied()
    {
        AssertDefined();
        return occupant != null;
    }

    /// <summary>
    /// Sets the Sprite of this Tile's SpriteRenderer component.
    /// </summary>
    /// <param name="newSprite">the new Sprite.</param>
    protected void SetSprite(Sprite newSprite)
    {
        if (newSprite == null) return;
        if (tileRenderer == null) tileRenderer = GetComponent<SpriteRenderer>();
        tileRenderer.sprite = newSprite;
    }

    /// <summary>
    /// Returns this Tile's TileType.
    /// </summary>
    /// <returns>this Tile's TileType.</returns>
    public TileType GetTileType()
    {
        AssertDefined();
        return type;
    }

    /// <summary>
    /// Returns true if this Tile has a flooring on it.
    /// </summary>
    /// <returns>true if this Tile has a flooring; otherwise, false.</returns>
    public bool Floored()
    {
        AssertDefined();
        return flooring != null;
    }

    /// <summary>
    /// Returns the Flooring on this Tile, or null if there is none.
    /// </summary>
    /// <returns>the Flooring on this Tile, or null if there is none.
    /// </returns>
    private Flooring GetFlooring()
    {
        AssertDefined();
        return flooring;
    }

    /// <summary>
    /// Returns true if this Tile can be floored with a Flooring. If so,
    /// floors the Tile.
    /// </summary>
    /// <param name="flooring">the GameObject with a Flooring component to check
    ///  with.</param>
    /// <returns>true if this Tile can be floored with a Flooring; otherwise,
    /// false.</returns>
    public bool Floor(GameObject flooring, ISurface[] neighbors)
    {
        //Safety checks
        AssertDefined();
        if (flooring == null || neighbors == null) return false;
        if (flooring.GetComponent<Flooring>() == null) return false;

        //Already floored
        if (Floored()) return false;

        //Update this Tile
        UpdateNeighbors(neighbors);

        //Make the new flooring
        GameObject gob = Instantiate(flooring);
        Flooring newFlooring = gob.GetComponent<Flooring>();
        newFlooring.Define(GetX(), GetY(), GetTileType(), GetFlooringNeighbors());
        newFlooring.transform.position = transform.position;
        newFlooring.transform.localScale = transform.localScale;
        newFlooring.transform.SetParent(transform);
        this.flooring = newFlooring;

        return true;
    }

    /// <summary>
    /// Returns an array of this Tile's neighbors' Flooring components.
    /// </summary>
    /// <returns>an array of this Tile's neighbors' Flooring components.
    /// </returns>
    private Flooring[] GetFlooringNeighbors()
    {
        AssertDefined();
        Assert.IsNotNull(neighbors);
        Flooring[] flooringNeighbors = new Flooring[neighbors.Length];
        for (int i = 0; i < neighbors.Length; i++)
        {
            if (neighbors[i] != null) flooringNeighbors[i] = neighbors[i].GetFlooring();
        }
        return flooringNeighbors;
    }

    /// <summary>
    /// Returns true if an IPlaceable can be placed on this Tile.
    /// If it can, places the IPlaceable.
    /// </summary>
    /// <param name="candidate">The IPlaceable to place.</param>
    /// <param name="neighbors">This Tile's neighbors.</param>
    /// <returns>true if an IPlaceable can be placed on this Tile;
    /// otherwise, false.</returns>
    public virtual bool Place(IPlaceable candidate, ISurface[] neighbors)
    {
        //Safety check
        AssertDefined();
        if (candidate == null || neighbors == null) return false;
        foreach (ISurface surface in neighbors)
        {
            if (surface != null) Assert.IsNotNull(surface as Tile);
        }

        //1. If has a flooring, pass event to that flooring.
        if (Floored())
        {
            Flooring[] flooringNeighbors = new Flooring[4];
            for (int i = 0; i < flooringNeighbors.Length; i++)
            {
                if (neighbors[i] == null) continue;
                Tile t = (Tile)neighbors[i];
                flooringNeighbors[i] = t.GetFlooring();
            }
            return GetFlooring().Place(candidate, flooringNeighbors);
        }

        if (!CanPlace(candidate, neighbors)) return false;

        //2. Placement on Tile logic here
        GameObject prefabClone = candidate.MakePlaceableObject();
        Assert.IsNotNull(prefabClone);
        PlaceableObject placeableObject = prefabClone.GetComponent<PlaceableObject>();
        Assert.IsNotNull(placeableObject);

        prefabClone.transform.position = transform.position;
        prefabClone.transform.localScale = Vector3.one;
        prefabClone.transform.SetParent(transform);
        occupant = placeableObject;
        occupant.Setup(GetPlaceableObjectNeighbors());

        return true;
    }

    /// <summary>
    /// Returns true if an IPlaceable can be placed on this Tile.
    /// </summary>
    /// <param name="candidate">The IPlaceable to place.</param>
    /// <param name="neighbors">This Tile's neighbors.</param>
    /// <returns>true if an IPlaceable can be placed on this Tile;
    /// otherwise, false.</returns>
    public virtual bool CanPlace(IPlaceable candidate, ISurface[] neighbors)
    {
        AssertDefined();
        if (candidate == null || neighbors == null) return false;
        if (Occupied()) return false;

        return true;
    }

    /// <summary>
    /// Returns true if an IUsable can be used on this Tile.
    /// If it can, uses the IUsable.
    /// </summary>
    /// <param name="candidate">The IUsable to use.</param>
    /// <param name="neighbors">This Tile's neighbors.</param>
    /// <returns>true if an IUsable can be used on this Tile;
    /// otherwise, false.</returns>
    public virtual bool Use(IUsable candidate, ISurface[] neighbors)
    {
        //Safety check
        AssertDefined();

        //TODO: Implement in future sprint

        return false;
    }

    /// <summary>
    /// Returns true if an IUsable can be used on this Tile.
    /// </summary>
    /// <param name="candidate">The IUsable to use.</param>
    /// <param name="neighbors">This Tile's neighbors.</param>
    /// <returns>true if an IUsable can be placed on this Tile;
    /// otherwise, false.</returns>
    public virtual bool CanUse(IUsable candidate, ISurface[] neighbors)
    {
        AssertDefined();
        if (candidate == null || neighbors == null) return false;
        if (Occupied()) return false;

        return true;
    }

    /// <summary>
    /// Returns true if there is an IPlaceable on this Tile that can
    /// be removed. If so, removes the IPlaceable.
    /// </summary>
    /// <param name="neighbors">This Tiles's neighbors.</param>
    /// <returns>true if an IPlaceable can be removed from this Tile;
    /// otherwise, false.</returns>
    public virtual bool Remove(ISurface[] neighbors)
    {
        AssertDefined();
        if (!Occupied() || neighbors == null) return false;

        //TODO: Implement

        return true;

    }

    /// <summary>
    /// Returns true if there is an IPlaceable on this Tile that can
    /// be removed.
    /// </summary>
    /// <param name="neighbors">This Tile's neighbors.</param>
    /// <returns>true if an IPlaceable can be removed from this Tile;
    /// otherwise, false.</returns>
    public virtual bool CanRemove(IPlaceable candidate, ISurface[] neighbors)
    {
        AssertDefined();
        if (!Occupied() || candidate == null || neighbors == null) return false;

        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Updates this Tile's array of neighbors. If it has a Flooring component,
    /// updates the neighbors of that component too. 
    /// </summary>
    /// <param name="newNeighbors">this Tile's new neighbors.</param>
    public void UpdateNeighbors(ISurface[] newNeighbors)
    {
        AssertDefined();
        Assert.IsNotNull(newNeighbors);

        Tile[] tileNeighbors = new Tile[4];
        for (int i = 0; i < newNeighbors.Length; i++)
        {
            tileNeighbors[i] = newNeighbors[i] as Tile;
        }
        neighbors = tileNeighbors;

        if (Floored()) flooring.UpdateNeighbors(GetFlooringNeighbors());
    }

    /// <summary>
    /// Returns this Tile's four neighbors.
    /// </summary>
    /// <returns>this Tile's four neighbors.</returns>
    public ISurface[] GetNeighbors()
    {
        AssertDefined();
        return neighbors;
    }

    /// <summary>
    /// Returns this Tile's four neighbors' PlaceableObjects.
    /// </summary>
    /// <returns>this Tile's four neighbors' PlaceableObjects.</returns>
    public PlaceableObject[] GetPlaceableObjectNeighbors()
    {
        AssertDefined();
        if (neighbors == null) return null;

        PlaceableObject[] placeableNeighbors = new PlaceableObject[4];
        for (int i = 0; i < neighbors.Length; i++)
        {
            ISurface neighbor = neighbors[i];
            if (neighbor != null) placeableNeighbors[i] = neighbor.GetPlaceableObject();
        }

        return placeableNeighbors;
    }

    /// <summary>
    /// Returns the PlaceableObject on this Tile.
    /// </summary>
    /// <returns>the PlaceableObject on this Tile; null if
    /// it is unoccupied.</returns>
    public PlaceableObject GetPlaceableObject()
    {
        return occupant;
    }

    /// <summary>
    /// Sets the color of this Tile's SpriteRenderer.
    /// </summary>
    /// <param name="paintColor">the color with which to paint this Tile.</param>
    public void PaintTile(Color32 paintColor)
    {
        tileRenderer.color = paintColor;
    }

    /// <summary>
    /// Asserts that this Tile has been formally defined with a type and coordinates
    /// by the TileGrid.
    /// </summary>
    public void AssertDefined()
    {
        Assert.IsTrue(defined, "Tile not defined.");
    }

    /// <summary>
    /// Returns the string representation of this Tile:<br></br>
    /// 
    /// "(X, Y)"
    /// </summary>
    /// <returns>the string representation of this Tile</returns>
    public override string ToString()
    {
        return "(" + GetX() + ", " + GetY() + ")";
    }
}
