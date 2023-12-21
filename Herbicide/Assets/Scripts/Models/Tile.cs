using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Represents a set of space in the TileGrid with unique (X, Y) coordinates.
/// Tiles may be floored and placed on.
/// </summary>
public abstract class Tile : Model, ISurface
{
    /// <summary>
    /// The PlaceableObject on this Tile; null if Tile is unoccupied.
    /// </summary>
    private PlaceableObject occupant;

    /// <summary>
    /// The ghost/preview of something that would place on this Tile.
    /// </summary>
    private GameObject ghostOccupant;

    /// <summary>
    /// The flooring on this Tile; null if there is none.
    /// </summary>
    private Flooring flooring;

    /// <summary>
    /// true if this Tile is spawned in a TileGrid.
    /// </summary>
    private bool defined;

    /// <summary>
    /// This Tile's four neighbors.
    /// </summary>
    private Tile[] neighbors;

    /// <summary>
    /// true if an Enemy is on this Tile; otherwise, false.
    /// </summary>
    private bool occupiedByEnemy;

    /// <summary>
    /// This Tile's Type.
    /// </summary>
    protected abstract TileType type { get; }

    /// <summary>
    /// Represents a Type of Tile.
    /// </summary>
    public enum TileType
    {
        GRASS,
        WATER,
        SHORE
    }

    //-------------PATHFINDING-------------

    /// <summary>
    /// A* pathfinding G-cost of moving to this Tile
    /// </summary>
    private int movementCost;

    /// <summary>
    /// A* pathfinding H-cost of moving to this Tile
    /// </summary>
    private int heuristicCost;

    /// <summary>
    /// A* pathfinding F-cost (G + H) of moving to this Tile
    /// </summary>
    private int totalCost;

    /// <summary>
    /// A* pathfinding Tile from which this Tile was reached
    /// </summary>
    private Tile pathfindParent;

    /// <summary>
    /// true if an Enemy can walk on this Tile
    /// </summary>
    public virtual bool WALKABLE => true;


    //-------------PATHFINDING-------------


    /// <summary>
    /// Defines this Tile to be within a TileGrid at coordinates (x, y).
    /// </summary>
    /// <param name="x">the x-coordinate of this Tile</param>
    /// <param name="y">the y-coordinate of this Tile</param>
    public virtual void Define(int x, int y)
    {
        //Safety checks

        if (defined) return;

        SetTileCoordinates(x, y);
        name = type.ToString() + " (" + x + ", " + y + ")";
        defined = true;
    }

    /// <summary>
    /// Main update loop for this Tile.
    /// </summary>
    public void UpdateTile()
    {
        Nexus nexusOccupant = GetOccupant() as Nexus;
        if (nexusOccupant != null)
        {
            ISurface[] neigbors = GetSurfaceNeighbors();
            if (nexusOccupant.PickedUp() && CanRemove(neigbors)) Remove(neigbors);
        }
    }

    /// <summary>
    /// Returns true if this Tile has an Enemy on it; otherwise, returns
    /// false.
    /// </summary>
    /// <returns>true if this Tile has an Enemy on it; otherwise, false.
    /// </returns>
    public bool OccupiedByEnemy() { return occupiedByEnemy; }

    /// <summary>
    /// Returns true if this Tile is occupied.
    /// </summary>
    /// <returns>true if this Tile is occupied.</returns>
    public bool Occupied()
    {
        AssertDefined();
        if (Floored()) return GetFlooring().Occupied();
        return GetOccupant() != null;
    }

    /// <summary>
    /// Sets this Tile's occupant.
    /// </summary>
    /// <param name="occupant">The occupant.</param>
    public void SetOccupant(PlaceableObject occupant) { this.occupant = occupant; }

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
        UpdateSurfaceNeighbors(neighbors);

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
    /// Places a PlaceableObject on this Tile.
    /// </summary>
    /// <param name="candidate">The PlaceableObject to place.</param>
    /// <param name="neighbors">This Tile's neighbors.</param>
    public virtual void Place(PlaceableObject candidate, ISurface[] neighbors)
    {
        //Safety check
        AssertDefined();
        Assert.IsNotNull(candidate, "Placement candidate can't be null.");
        Assert.IsNotNull(neighbors, "Placement candidate's neighbors can't be null.");
        Assert.IsTrue(CanPlace(candidate, neighbors), "Need to make sure placement is valid.");

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
            GetFlooring().Place(candidate, flooringNeighbors);
            return;
        }



        //3. Place on this Tile.


        SpriteRenderer prefabRenderer = candidate.GetComponent<SpriteRenderer>();

        prefabRenderer.sortingOrder = GetY();
        candidate.transform.position = transform.position;
        candidate.transform.localScale = candidate.GetPlacementScale();
        candidate.transform.SetParent(transform);
        candidate.OnPlace();
        if (candidate.OCCUPIER) SetOccupant(candidate);
    }

    /// <summary>
    /// Returns true if a PlaceableObject can be placed on this Tile.
    /// </summary>
    /// <param name="candidate">The PlaceableObject to place.</param>
    /// <param name="neighbors">This Tile's neighbors.</param>
    /// <returns>true if a PlaceableObject can be placed on this Tile;
    /// otherwise, false.</returns>
    public virtual bool CanPlace(PlaceableObject candidate, ISurface[] neighbors)
    {
        AssertDefined();
        if (candidate == null || neighbors == null) return false;
        if (Floored()) return GetFlooring().CanPlace(candidate, neighbors);

        if (candidate == null || neighbors == null) return false;
        if (candidate as Squirrel != null) return false;
        if (candidate as Tree != null) return false;
        if (Occupied()) return false;

        return true;
    }

    /// <summary>
    /// Returns true if an Model can be used on this Tile.
    /// If it can, uses the Model.
    /// </summary>
    /// <param name="candidate">The Model to use.</param>
    /// <param name="neighbors">This Tile's neighbors.</param>
    /// <returns>true if an Model can be used on this Tile;
    /// otherwise, false.</returns>
    public virtual bool Use(Model candidate, ISurface[] neighbors)
    {
        //Safety check
        AssertDefined();

        //TODO: Implement in future sprint

        return false;
    }

    /// <summary>
    /// Returns true if an Model can be used on this Tile.
    /// </summary>
    /// <param name="candidate">The Model to use.</param>
    /// <param name="neighbors">This Tile's neighbors.</param>
    /// <returns>true if an Model can be placed on this Tile;
    /// otherwise, false.</returns>
    public virtual bool CanUse(Model candidate, ISurface[] neighbors)
    {
        AssertDefined();
        if (candidate == null || neighbors == null) return false;
        if (Occupied()) return false;

        return true;
    }

    /// <summary>
    /// Removes the PlaceableObject on this Tile. This does not
    /// destroy the occupant; that is the responsibility of its controller. 
    /// </summary>
    /// <param name="neighbors">This Tiles's neighbors.</param>
    public virtual void Remove(ISurface[] neighbors)
    {
        AssertDefined();
        Assert.IsTrue(CanRemove(neighbors), "Need to check removal validity.");

        if (Floored()) GetFlooring().Remove(neighbors);
        else occupant = null;
    }

    /// <summary>
    /// Returns true if there is a PlaceableObject on this Tile that can be
    /// removed. 
    /// /// </summary>
    /// <param name="neighbors">This Tile's neighbors.</param>
    /// <returns>true if there is a PlaceableObject on this Tile that can be
    /// removed; otherwise, false. </returns>
    public virtual bool CanRemove(ISurface[] neighbors)
    {
        AssertDefined();
        Assert.IsNotNull(neighbors, "Array of neighbors is null.");

        if (Floored()) return GetFlooring().CanRemove(neighbors);
        if (!Occupied()) return false;

        return true;
    }

    /// <summary>
    /// Returns this Tile's occupant.
    /// </summary>
    /// <returns>this Tile's occupant; null if there is none. </returns>
    private PlaceableObject GetOccupant() { return occupant; }

    /// <summary>
    /// Updates this Tile's array of neighbors. If it has a Flooring component,
    /// updates the neighbors of that component too. 
    /// </summary>
    /// <param name="newNeighbors">this Tile's new neighbors.</param>
    public void UpdateSurfaceNeighbors(ISurface[] newNeighbors)
    {
        AssertDefined();
        Assert.IsNotNull(newNeighbors);

        Tile[] tileNeighbors = new Tile[4];
        for (int i = 0; i < newNeighbors.Length; i++)
        {
            tileNeighbors[i] = newNeighbors[i] as Tile;
        }
        neighbors = tileNeighbors;

        if (Floored()) flooring.UpdateSurfaceNeighbors(GetFlooringNeighbors());
    }

    /// <summary>
    /// Returns this Tile's four neighbors.
    /// </summary>
    /// <returns>this Tile's four neighbors.</returns>
    public ISurface[] GetSurfaceNeighbors()
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
        if (GetOccupant() != null) return GetOccupant();
        if (Floored() && GetFlooring().Occupied()) return GetFlooring().GetPlaceableObject();
        return null;
    }

    /// <summary>
    /// Sets the color of this Tile's SpriteRenderer. If it's floored, passes
    /// this paint command to the flooring instead.
    /// </summary>
    /// <param name="paintColor">the color with which to paint this Tile.</param>
    public override void SetColor(Color32 paintColor)
    {
        if (Floored()) GetFlooring().SetColor(paintColor);
        else base.SetColor(paintColor);
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

    /// <summary>
    /// Returns the pathfinding movement cost of this Tile.
    /// </summary>
    /// <returns>the Pathfinding movement cost of this Tile.</returns>
    public int GetMovementCost()
    {
        return movementCost;
    }

    /// <summary>
    /// Sets the pathfinding movement cost of this Tile.
    /// </summary>
    /// <param name="newCost">the updated movement cost</param>
    public void SetMovementCost(int newCost)
    {
        if (newCost >= 0) movementCost = newCost;
    }

    /// <summary>
    /// Returns the pathfinding heuristic cost of this Tile.
    /// </summary>
    /// <returns>the pathfinding heuristic cost of this Tile</returns>
    public int GetHeuristicCost()
    {
        return heuristicCost;
    }

    /// <summary>
    /// Sets the pathfinding heuristic cost of this Tile.
    /// </summary>
    /// <param name="newCost">the new heuristic cost of this Tile</param>
    public void SetHeuristicCost(int newCost)
    {
        if (newCost >= 0) heuristicCost = newCost;
    }

    /// <summary>
    /// Returns this Tile's pathfinding parent.
    /// </summary>
    /// <returns>this Tile's pathfinding parent.</returns>
    public Tile GetPathfindingParent()
    {
        return pathfindParent;
    }

    /// <summary>
    /// Sets this Tile's pathfinding parent.
    /// </summary>
    /// <param name="newParent">this Tile's pathfinding parent.</param>
    public void SetPathfindingParent(Tile newParent)
    {
        pathfindParent = newParent;
    }

    /// <summary>
    /// Returns this Tile's pathfinding movement and heuristic cost, combined.
    /// </summary>
    /// <returns>this Tile's pathfinding movement and heuristic cost, combined.</returns>
    public int GetTotalPathfindingCost()
    {
        return GetMovementCost() + GetHeuristicCost();
    }

    /// <summary>
    /// Returns the Manhattan Tile distance from this Tile to another.
    /// </summary>
    /// <param name="other">the other Tile.</param>
    /// <returns>the Manhattan Tile distance from this Tile to another.</returns>
    public int GetManhattanDistance(Tile other)
    {
        return GetManhattanDistance(other.GetX(), other.GetY());
    }

    /// <summary>
    /// Returns the Manhattan Tile distance from this Tile to an (X, Y) coordinate.
    /// </summary>
    /// <param name="x">the X-coordinate.</param>
    /// <param name="y">the Y-coordinate.</param>
    /// <returns>the Manhattan Tile distance from this Tile to an (X, Y) coordinate.</returns>
    public int GetManhattanDistance(int x, int y)
    {
        int xDistance = Mathf.Abs(GetX() - x);
        int yDistance = Mathf.Abs(GetY() - y);
        return xDistance + yDistance;
    }

    /// <summary>
    /// Returns true if a pathfinder can walk across this Tile. They
    /// can do so if it is unoccupied and walkable by default.
    /// </summary>
    /// <returns>true if a pathfinder can walk across this Tile;
    /// otherwise, false.</returns>
    public virtual bool IsWalkable()
    {
        if (Floored()) return GetFlooring().IsWalkable();
        return WALKABLE && !Occupied();
    }

    /// <summary>
    /// Determines whether a PlaceableObject object can be potentially placed
    /// on this Tile. This method is invoked alongside GhostPlace() during a
    /// hover or placement action to validate the placement feasibility.
    /// </summary>
    /// <param name="ghost">The PlaceableObject object that we are
    /// trying to virtually place on this Tile.</param>
    /// <returns>true if the PlaceableObject object can be placed on this Tile;
    /// otherwise, false.</returns>
    public bool CanGhostPlace(PlaceableObject ghost)
    {
        return !Occupied() && ghost != null && ghostOccupant == null
            && CanPlace(ghost, GetSurfaceNeighbors());
    }

    /// <summary>
    /// Provides a visual simulation of placing a PlaceableObject on
    /// this Tile and is called during a hover / placement action.
    /// This method does not carry out actual placement of the PlaceableObject on
    /// this Tile. Instead, it displays a potential placement scenario.
    /// </summary>
    /// <param name="ghost">The PlaceableObject object that we are
    /// trying to virtually place on this Tile.</param>
    /// <returns> true if the ghost place was successful; otherwise,
    /// false. </returns> 
    public bool GhostPlace(PlaceableObject ghost)
    {
        //Are we floored? If so, pass the event.
        if (Floored()) return GetFlooring().GhostPlace(ghost);

        if (!CanGhostPlace(ghost)) return false;


        //Ghost place on this Tile.
        GameObject hollowCopy = (ghost as PlaceableObject).MakeHollowObject();
        Assert.IsNotNull(hollowCopy);
        SpriteRenderer hollowRenderer = hollowCopy.GetComponent<SpriteRenderer>();
        Assert.IsNotNull(hollowRenderer);


        hollowRenderer.sortingLayerName = "Trees";
        hollowRenderer.sortingOrder = GetY();
        hollowRenderer.color = new Color32(255, 255, 255, 200);
        hollowCopy.transform.position = transform.position;
        hollowCopy.transform.localScale = Vector3.one;
        hollowCopy.transform.SetParent(transform);

        ghostOccupant = hollowCopy;
        return true;
    }

    /// <summary>
    /// Removes all visual simulations of placing a PlaceableObject on this
    /// Tile. If there are none, does nothing.
    /// </summary>
    public void GhostRemove()
    {
        if (Floored()) GetFlooring().GhostRemove();

        if (ghostOccupant == null) return;

        Destroy(ghostOccupant);
        ghostOccupant = null;
    }

    /// <summary>
    /// Sets whether an Enemy is on this Tile.
    /// </summary>
    /// <param name="occupiedByEnemy">true if an Enemy is 
    /// on this Tile; otherwise, false.</param>
    public void SetOccupiedByEnemy(bool occupiedByEnemy)
    {
        this.occupiedByEnemy = occupiedByEnemy;
        if (Floored()) GetFlooring().SetOccupiedByEnemy(occupiedByEnemy);
    }

    /// <summary>
    /// Returns true if the ghost occupant on this Tile is not null.
    /// </summary>
    /// <returns>true if the ghost occupant on this Tile is not null;
    /// otherwise, false.</returns>
    public bool HasActiveGhostOccupant()
    {
        if (Floored()) return GetFlooring().HasActiveGhostOccupant();
        return ghostOccupant != null;
    }

    /// <summary>
    /// Resets this Tile's stats to their starting values.
    /// </summary>
    public override void ResetStats() { return; }

    /// <summary>
    /// Sets the 2D Collider properties of this Tile.
    /// </summary>
    public override void SetColliderProperties() { return; }

    /// <summary>
    /// Returns the Sprite component that represents this Tile in
    /// the Inventory.
    /// </summary>
    /// <returns>the Sprite component that represents this Tile in
    /// the Inventory.</returns>
    public override Sprite GetInventorySprite()
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Returns a Sprite that represents this Tile when it is
    /// being placed.
    /// </summary>
    /// <returns> a Sprite that represents this Tile when it is
    /// being placed.</returns>
    public override Sprite GetPlacementSprite()
    {
        throw new System.NotImplementedException();
    }
}


