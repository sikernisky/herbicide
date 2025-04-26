using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Represents a set of space in the TileGrid with unique (X, Y) coordinates.
/// Tiles may be floored and placed on.
/// </summary>
public abstract class Tile : Model, IFixedSurface
{
    #region Fields

    /// <summary>
    /// The PlaceableObject that is currently on this Tile.
    /// </summary>
    public PlaceableObject Occupant { get; private set; }

    /// <summary>
    /// The Ghost GameObject that is currently on this Tile.
    /// </summary>
    public GameObject GhostOccupant { get; private set; }

    /// <summary>
    /// Backing field for the LineRenderer property.
    /// </summary>
    [SerializeField]
    private LineRenderer _lineRenderer;

    /// <summary>
    /// Reference to the LineRenderer for this Tile.
    /// </summary>
    public LineRenderer LineRenderer { get { return _lineRenderer; } }

    /// <summary>
    /// true a Mob can traverse this; otherwise, false.
    /// </summary>
    public virtual bool IsTraversable => true;

    /// <summary>
    /// Backing field for the neighbors property.
    /// </summary>
    private ISurface[] _neighbors;

    /// <summary>
    /// The eight ISurfaces that are adjacent to this Tile.
    /// </summary>
    public ISurface[] Neighbors
    {
        get { return (ISurface[])_neighbors.Clone(); }
        private set { _neighbors = value; }
    }

    /// <summary>
    /// The (X, Y) coordinates of this Tile.
    /// </summary>
    public Vector2Int Coordinates { get; private set; }

    /// <summary>
    /// true if this Tile has been defined with coordinates; otherwise, false.
    /// </summary>
    public bool Defined { get; private set; }

    #endregion

    #region Methods

    /// <summary>
    /// Defines this Tile with its coordinates.
    /// </summary>
    /// <param name="x">The X-coordinate of this Tile.</param>
    /// <param name="y">The Y-coordinate of this Tile.</param>
    public virtual void DefineWithCoordinates(int x, int y)
    {
        if (Defined) return;

        Coordinates = new Vector2Int(x, y);
        name = TYPE.ToString() + " " + Coordinates.ToString();
        Defined = true;
    }

    /// <summary>
    /// Sets the four ISurfaces that are adjacent to this Tile. If this Tile has an occupant
    /// that is an IFixedSurface, sets the neighbors of that occupant as well.
    /// </summary>
    /// <param name="neighbors">the four ISurfaces that are adjacent to this Tile.</param>
    public void SetNeighbors(ISurface[] neighbors)
    {
        Assert.IsNotNull(neighbors);
        Assert.IsTrue(neighbors.Length == 4);
        Neighbors = neighbors;
        if(IsOccupied() && Occupant.Object is IFixedSurface fixedSurface) fixedSurface.SetNeighbors(neighbors);
    }

    /// <summary>
    /// Returns true if an IFixedSurface candidate can be placed on this Tile. If so,
    /// places the candidate.
    /// </summary>
    /// <param name="candidate">The candidate to place.</param>
    /// <returns>true if a candidate can be placed on this Tile; otherwise,
    /// false.</returns>
    public bool Place(IFixedSurface candidate)
    {
        if (candidate is not ISurfacePlaceable surfacePlaceable) return false;
        if (!Place(surfacePlaceable)) return false;
        candidate.DefineWithCoordinates(Coordinates.x, Coordinates.y);
        candidate.SetNeighbors(Neighbors);
        UpdateAppearanceBasedOnNeighbors();
        UpdateAppearanceOfNeighbors();
        return true;
    }

    /// <summary>
    /// Returns true if a candidate can be placed on this Tile. If so,
    /// places the candidate.
    /// </summary>
    /// <param name="candidate">The candidate to place.</param>
    /// <returns>true if a candidate can be placed on this Tile; otherwise,
    /// false.</returns>
    public bool Place(ISurfacePlaceable candidate)
    {
        if (Occupant is ISurface surface) return surface.Place(candidate);
        if (!CanPlace(candidate)) return false;
        SetupCandidateOnTile(candidate.Object);
        Occupant = candidate.Object;
        UpdateAppearanceBasedOnNeighbors();
        UpdateAppearanceOfNeighbors();
        return true;
    }

    /// <summary>
    /// Returns true if a candidate can be placed on this Tile.
    /// </summary>
    /// <param name="candidate">The candidate to place.</param>
    /// <returns>true if a candidate can be placed on this Tile;
    /// otherwise, false.</returns>
    private bool CanPlace(ISurfacePlaceable candidate)
    {
        if (IsOccupied()) return false;
        if (candidate == null) return false;
        if (candidate as Flooring == null && candidate as SpawnHole == null && candidate as GoalHole == null) return false;
        return true;
    }

    /// <summary>
    /// Returns true if this Tile is occupied.
    /// </summary>
    /// <returns>true if this Tile is occupied.</returns>
    public bool IsOccupied() => Occupant != null;

    /// <summary>
    /// Physically sets up the candidate on this Tile.
    /// </summary>
    /// <param name="candidate">the candidate to set up.</param>
    private void SetupCandidateOnTile(PlaceableObject candidate)
    {
        Assert.IsNotNull(candidate);
        candidate.SetSortingOrder(Coordinates.y);
        candidate.transform.SetParent(transform);
        candidate.transform.localPosition = new Vector3(BoardConstants.XPlacementOffset, BoardConstants.YPlacementOffset, transform.localPosition.z + BoardConstants.ZPlacementOffset);
        candidate.transform.localScale = Vector3.one;
    }

    /// <summary>
    /// Updates the appearance of this Tile based on its neighbors.
    /// </summary>
    public void UpdateAppearanceBasedOnNeighbors()
    {
        if (!Defined) return;
        if (Occupant is IFixedSurface fixedSurfaceOccupant) fixedSurfaceOccupant.UpdateAppearanceBasedOnNeighbors();
    }

    /// <summary>
    /// Updates the appearance of this Tile's neighbors who
    /// implement IFixedSurface.
    /// </summary>
    private void UpdateAppearanceOfNeighbors()
    {
        foreach (IFixedSurface neighbor in Neighbors)
        {
            if (neighbor is IFixedSurface fixedSurfaceNeighbor) fixedSurfaceNeighbor.UpdateAppearanceBasedOnNeighbors();
        }
    }

    /// <summary>
    /// Returns true if the given Model type lies on this Tile or
    /// any of its occupants.
    /// </summary>
    /// <typeparam name="T">The Model type to check for.</typeparam>
    /// <returns> true if the given Model type lies on this Tile or
    /// any of its occupants; otherwise, false.</returns>
    public bool HasModel<T>()
    {
        ISurface current = this;
        while (current != null)
        {
            if (current.Occupant?.Object is T) return true;
            current = current.Occupant as ISurface;
        }
        return false;
    }

    /// <summary>
    /// Returns true if the given Model type lies on this Tile or
    /// any of its occupants.
    /// </summary>
    /// <param name="modelType">The Model type to check for.</param>
    /// <returns>true if the given Model type lies on this Tile or
    /// any of its occupants; otherwise, false.</returns>
    public bool HasModel(ModelType modelType)
    {
        ISurface current = this;
        while (current != null)
        {
            if (current.Occupant?.Object.TYPE == modelType) return true;
            current = current.Occupant as ISurface;
        }
        return false;
    }

    /// <summary>
    /// Returns and removes the PlaceableObject on this Tile.
    /// </summary>
    /// <returns>the PlaceableObject that was removed; null if there was none.</returns>
    public PlaceableObject Remove()
    {
        if (Occupant is ISurface surface) return surface.Remove();
        if (!IsOccupied()) return null;
        PlaceableObject removed = Occupant;
        Occupant = null;
        return removed;
    }

    /// <summary>
    /// Provides a visual simulation of placing a candidate on
    /// this Tile and is called during a hover / placement action.
    /// This method does not carry out actual placement of the candidate on
    /// this Tile. Instead, it displays a potential placement scenario.
    /// </summary>
    /// <param name="candidate">The candidate object that we are
    /// trying to virtually place on this Tile.</param>
    /// <returns> true if the ghost place was successful; otherwise,
    /// false. </returns> 
    public bool GhostPlace(ISurfacePlaceable candidate)
    {
        if (Occupant is ISurface surface) return surface.GhostPlace(candidate);
        if (!CanGhostPlace(candidate)) return false;
        if (candidate.Object is Defender defender) DrawRangeIndicator(defender);
        GhostOccupant = SetupGhostCandidateOnTile(candidate.Object);
        return true;
    }

    /// <summary>
    /// Draws a range indicator around this Surface.
    /// </summary>
    /// <param name="defender">The Defender to draw the range indicator for.</param>
    public void DrawRangeIndicator(Defender defender)
    {
        if (!ShouldDrawRangeIndicator(defender)) return;
        SetupRangeIndicator();
        float ar = defender.BASE_MAIN_ACTION_RANGE;
        Assert.IsTrue(ar >= 0, "Range must be greater than or equal to 0.");
        LineRenderer.positionCount = RenderingConstants.TileRangeIndicatorSegments + 1;
        DrawRangeCircle(ar);
    }

    /// <summary>
    /// Returns true if the given Defender should draw a range indicator.
    /// </summary>
    /// <returns>true if the given Defender should draw a range indicator;
    /// otherwise, false.</returns> 
    private bool ShouldDrawRangeIndicator(Defender defender) => defender != null && defender.DRAWS_RANGE_INDICATOR;

    /// <summary>
    /// Sets the LineRenderer component values for the range
    /// indicator LineRenderer.
    /// </summary>
    private void SetupRangeIndicator()
    {
        LineRenderer.useWorldSpace = false;
        LineRenderer.startWidth = RenderingConstants.SurfaceRangeIndicatorStartWidth;
        LineRenderer.endWidth = RenderingConstants.SurfaceRangeIndicatorEndWidth;
        LineRenderer.material = new Material(Shader.Find(FilePathConstants.DefaultMaterialPath));
        LineRenderer.startColor = ColorConstants.SurfaceRangeIndicatorColor;
        LineRenderer.endColor = ColorConstants.SurfaceRangeIndicatorColor;
    }

    /// <summary>
    /// Determines whether a candidate object can be potentially placed
    /// on this Tile. This method is invoked alongside GhostPlace() during a
    /// hover or placement action to validate the placement feasibility.
    /// </summary>
    /// <param name="candidate">The candidate object that we are
    /// trying to virtually place on this Tile.</param>
    /// <returns>true if the candidate object can be placed on this Tile;
    /// otherwise, false.</returns>
    private bool CanGhostPlace(ISurfacePlaceable candidate) => !IsOccupied() && !IsGhostOccupied() && CanPlace(candidate);

    /// <summary>
    /// Draws the circular range indicator.
    /// </summary>
    private void DrawRangeCircle(float range)
    {
        float lineAngle = 0f;
        for (int i = 0; i <= RenderingConstants.TileRangeIndicatorSegments; i++)
        {
            SetLineRendererPosition(i, lineAngle, range);
            lineAngle += 360f / RenderingConstants.TileRangeIndicatorSegments;
        }
    }

    /// <summary>
    /// Sets a position in the line renderer.
    /// </summary>
    private void SetLineRendererPosition(int index, float angle, float range)
    {
        float x = Mathf.Sin(Mathf.Deg2Rad * angle) * range;
        float y = Mathf.Cos(Mathf.Deg2Rad * angle) * range;
        LineRenderer.SetPosition(index, new Vector3(x, y, 1));
    }

    /// <summary>
    /// Returns the GameObject that represents the ghost candidate on this Tile.
    /// Sets up the ghost candidate's SpriteRenderer and MaterialPropertyBlock.
    /// </summary>
    /// <param name="ghostCandidate">The ghost candidate to set up.</param>
    /// <returns>the GameObject that represents the ghost candidate on this Tile.</returns>
    private GameObject SetupGhostCandidateOnTile(PlaceableObject ghostCandidate)
    {
        GameObject hollowCopy = ghostCandidate.MakeHollowObject();
        SpriteRenderer hollowRenderer = hollowCopy.GetComponent<SpriteRenderer>();
        ConfigureGhostCandidateSpriteRenderer(hollowRenderer, ghostCandidate.GetColor());
        hollowCopy.transform.position = transform.position;
        hollowCopy.transform.SetParent(transform);
        return hollowCopy;
    }

    /// <summary>
    /// Configures the ghost candidate's SpriteRenderer.
    /// </summary>
    /// <param name="ghostRenderer">the ghost candidate's SpriteRenderer.</param>
    /// <param name="ghostColor">the ghost candidate's color.</param>
    private void ConfigureGhostCandidateSpriteRenderer(SpriteRenderer ghostRenderer, Color32 ghostColor)
    {
        ghostRenderer.sortingLayerName = GetSortingLayerName();
        ghostRenderer.sortingOrder = GetSortingOrder() + 1;
        ghostRenderer.color = new Color32(ghostColor.r, ghostColor.g, ghostColor.b, ColorConstants.GhostOccupantAlpha);
    }

    /// <summary>
    /// Returns true if the ghost occupant on this Tile is not null.
    /// </summary>
    /// <returns>true if the ghost occupant on this Tile is not null;
    /// otherwise, false.</returns>
    public bool IsGhostOccupied() => GhostOccupant != null;

    /// <summary>
    /// Removes all visual simulations of placing a PlaceableObject on this
    /// Tile. If there are none, does nothing.
    /// </summary>
    public void GhostRemove()
    {
        if(Occupant is ISurface surface) surface.GhostRemove();
        EraseRangeIndicator();
        if (GhostOccupant == null) return;
        Destroy(GhostOccupant);
        GhostOccupant = null;
    }

    /// <summary>
    /// Erases the range indicator.
    /// </summary>
    private void EraseRangeIndicator() => LineRenderer.positionCount = 0;

    /// <summary>
    /// Returns true if a pathfinder can walk across this Tile.
    /// </summary>
    /// <returns>true if a pathfinder can walk across this Tile;
    /// otherwise, false.</returns>
    public virtual bool IsCurrentlyWalkable()
    {
        if (Occupant is IFixedSurface fixedSurface) return fixedSurface.IsCurrentlyWalkable();
        else if(IsOccupied() && !Occupant.IsTraversable) return false;
        return IsTraversable;
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
    public override Sprite[] GetPlacementTrack() => new Sprite[] { GetSprite() };

    /// <summary>
    /// Sets the Sprite of this Tile using the given local Tiled ID.
    /// </summary>
    /// <param name="localIndex">the local Tiled ID of this Tile in its Tiled tile set.</param>
    public virtual void SetSpriteUsingLocalTiledIndex(int localIndex) => SetSprite(TileFactory.GetTileSprite(TYPE, localIndex));

    #endregion
}


