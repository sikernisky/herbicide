using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Represents a set of space in the TileGrid with unique (X, Y) coordinates.
/// Tiles may be floored and placed on.
/// </summary>
public abstract class Tile : Model, ISurface
{
    #region Fields

    /// <summary>
    /// Represents a Type of Tile.
    /// </summary>
    public enum TileType
    {
        GRASS,
        WATER,
        SHORE
    }

    /// <summary>
    /// Reference to the LineRenderer for this Tile.
    /// </summary>
    [SerializeField]
    private LineRenderer lineRenderer;

    /// <summary>
    /// Number of segments in the range indicator.
    /// </summary>
    private int RANGE_INDICATOR_SEGMENTS => 50;

    /// <summary>
    /// The PlaceableObject on this Tile; null if Tile is unoccupied.
    /// </summary>
    private PlaceableObject occupant;

    /// <summary>
    /// The NexusHole on this Tile; null if there is none.
    /// </summary>
    private NexusHole nexusHoleOccupant;

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


    #endregion

    #region Stats

    /// <summary>
    /// true if an Enemy can walk on this Tile
    /// </summary>
    public virtual bool WALKABLE => true;

    #endregion

    #region Methods

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
        SetupRangeIndicator();
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
    public bool OccupiedByEnemy() => occupiedByEnemy;

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
    public void SetOccupant(PlaceableObject occupant) => this.occupant = occupant;

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
        return GetFlooring() != null;
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
    /// Returns true if this Tile can be Floored with some flooring prefab.
    /// </summary>
    /// <param name="flooring">The flooring prefab to check.</param>
    /// <param name="neighbors">This Tile's neighbors.</param>
    /// <returns>true if this Tile can be Floored with some flooring prefab;
    /// otherwise, false. </returns>
    public bool CanFloor(Flooring flooring, ISurface[] neighbors)
    {
        AssertDefined();
        if (flooring == null || neighbors == null) return false;
        if (flooring.GetComponent<Flooring>() == null) return false;
        if (Floored()) return false;
        if (HostsNexusHole()) return false;

        return true;
    }

    /// <summary>
    /// Returns true if this Tile can be floored with a Flooring. If so,
    /// floors the Tile.
    /// </summary>
    /// <param name="flooring">the GameObject with a Flooring component to check
    ///  with.</param>
    /// <returns>true if this Tile can be floored with a Flooring; otherwise,
    /// false.</returns>
    public void Floor(Flooring flooring, ISurface[] neighbors)
    {
        //Safety checks
        AssertDefined();
        Assert.IsTrue(CanFloor(flooring, neighbors));

        UpdateSurfaceNeighbors(neighbors);

        //Setup the new flooring on this Tile.
        flooring.Define(GetX(), GetY(), GetTileType(), GetFlooringNeighbors());
        flooring.transform.position = transform.position;
        flooring.transform.SetParent(transform);
        flooring.SetLocalScale(Vector3.one);
        this.flooring = flooring;
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
        candidate.transform.SetParent(transform);
        candidate.SetLocalScale(Vector3.one);
        candidate.OnPlace(new Vector2Int(GetX(), GetY()));
        if (candidate.OCCUPIER) SetOccupant(candidate);

        NexusHole nexusHole = candidate as NexusHole;
        if (nexusHole != null) nexusHoleOccupant = nexusHole;
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

        ModelType candidateType = candidate.TYPE;
        List<ModelType> acceptedTypes = new List<ModelType>()
        {
            ModelType.NEXUS,
            ModelType.NEXUS_HOLE,
            ModelType.SOIL_FLOORING,
            ModelType.STONE_WALL
        };

        if (Occupied()) return false;
        if (!acceptedTypes.Contains(candidateType)) return false;

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
        else
        {
            occupant.OnRemove();
            nexusHoleOccupant = null;
            occupant = null;
        }
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
    private PlaceableObject GetOccupant() => occupant;

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

        // If hosting a flooring, pass this event to the flooring.
        if (Floored()) flooring.UpdateSurfaceNeighbors(GetFlooringNeighbors());
        else
        {
            PlaceableObject occupant = GetOccupant();
            if (occupant != null) occupant.UpdateNeighbors(GetPlaceableObjectNeighbors());
        }
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
    public void AssertDefined() => Assert.IsTrue(defined, "Tile not defined.");

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
        if (Floored())
        {
            bool result = GetFlooring().GhostPlace(ghost);
            if(result) DrawRangeIndicator(ghost as Defender);
            return result;
        }

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
        EraseRangeIndicator();
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
    public override void ResetModel() => base.ResetModel();

    /// <summary>
    /// Returns the GameObject that represents this Tile on the grid.
    /// </summary>
    /// <returns>the GameObject that represents this Tile on the grid.
    /// </returns>
    public override GameObject CreateNew() => throw new System.NotSupportedException("Not allowed to copy Tiles");

    /// <summary>
    /// Returns a Sprite that represents this Tile when it is
    /// being placed.
    /// </summary>
    /// <returns> a Sprite that represents this Tile when it is
    /// being placed.</returns>
    public override Sprite[] GetPlacementTrack() => throw new System.NotSupportedException("Tile placing not supported.");

    /// <summary>
    /// Returns true if there is a NexusHole on this Tile.
    /// </summary>
    /// <returns>true if there is a NexusHole on this Tile; otherwise,
    /// false. </returns>
    public bool HostsNexusHole() => nexusHoleOccupant != null;

    /// <summary>
    /// Sets the LineRenderer component values for the range
    /// indicator LineRenderer.
    /// </summary>
    private void SetupRangeIndicator()
    {
        lineRenderer.useWorldSpace = false;
        lineRenderer.startWidth = 0.05f; // Adjust width as needed
        lineRenderer.endWidth = 0.05f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default")); // Ensure this material supports transparency
        lineRenderer.startColor = new Color(0.2f, 1f, 0f, 0.5f);
        lineRenderer.endColor = new Color(0.2f, 1f, 0f, 0.5f);
    }

    /// <summary>
    /// Draws a range indicator around this Mob.
    /// </summary>
    /// <param name="defender">The Defender to draw the range indicator for.</param>
    private void DrawRangeIndicator(Defender defender)
    {
        if (defender == null) return;
        if(!defender.DRAWS_RANGE_INDICATOR) return;

        int ar = Mathf.FloorToInt(defender.BASE_MAIN_ACTION_RANGE);
        if (ar == float.MaxValue || ar <= 0) return;

        lineRenderer.positionCount = RANGE_INDICATOR_SEGMENTS + 1;

        float lineAngle = 0f;
        for (int i = 0; i <= RANGE_INDICATOR_SEGMENTS; i++)
        {
            float x = Mathf.Sin(Mathf.Deg2Rad * lineAngle) * ar;
            float y = Mathf.Cos(Mathf.Deg2Rad * lineAngle) * ar;
            lineRenderer.SetPosition(i, new Vector3(x, y, 1));
            lineAngle += 360f / RANGE_INDICATOR_SEGMENTS;
        }
    }

    /// <summary>
    /// Erases the range indicator.
    /// </summary>
    private void EraseRangeIndicator() => lineRenderer.positionCount = 0;

    #endregion
}


