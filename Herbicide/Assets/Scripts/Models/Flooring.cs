using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Represents a Model that can place on a Tile to expand its
/// behavior and complexity. 
/// </summary>
public abstract class Flooring : PlaceableObject, IFixedSurface
{
    #region Fields

    /// <summary>
    /// The ISurfacePlaceable that is currently on this Flooring.
    /// </summary>
    public PlaceableObject Occupant { get; private set; }

    /// <summary>
    /// The Ghost GameObject that is currently on this Flooring.
    /// </summary>
    public GameObject GhostOccupant { get; private set; }

    /// <summary>
    /// Backing field for the LineRenderer property.
    /// </summary>
    [SerializeField]
    private LineRenderer _lineRenderer;

    /// <summary>
    /// Reference to the LineRenderer for this Flooring.
    /// </summary>
    public LineRenderer LineRenderer { get { return _lineRenderer; } }

    /// <summary>
    /// true a Mob can traverse this; otherwise, false.
    /// </summary>
    public override bool IsTraversable => true;

    /// <summary>
    /// Backing field for the neighbors property.
    /// </summary>
    private ISurface[] _neighbors;

    /// <summary>
    /// The eight ISurfaces that are adjacent to this Flooring.
    /// </summary>
    public ISurface[] Neighbors
    {
        get { return (ISurface[])_neighbors.Clone(); }
        private set { _neighbors = value; }
    }

    /// <summary>
    /// true if this Flooring has been defined with coordinates; otherwise, false.
    /// </summary>
    public bool Defined { get; private set; }

    /// <summary>
    /// The base health of this Flooring.
    /// </summary>
    public override float BaseHealth => ModelStatConstants.FlooringBaseHealth;

    /// <summary>
    /// The maximum health of this Flooring.
    /// </summary>
    public override float MaxHealth => ModelStatConstants.FlooringMaxHealth;

    /// <summary>
    /// The minimum health of this Flooring.
    /// </summary>
    public override float MinHealth => ModelStatConstants.FlooringMinHealth;

    #endregion

    #region Methods

    /// <summary>
    /// Defines this Flooring with its neighbors and coordinates.
    /// </summary>
    /// <param name="neighbors">The eight ISurfaces that are adjacent to this Flooring.</param>
    /// <param name="x">The X-coordinate of this Flooring.</param>
    /// <param name="y">The Y-coordinate of this Flooring.</param>
    public override void DefineWithCoordinates(int x, int y)
    {
        if (Defined) return;

        Coordinates = new Vector2Int(x, y);
        name = TYPE.ToString() + " " + Coordinates.ToString();
        Defined = true;
    }

    /// <summary>
    /// Sets the four ISurfaces that are adjacent to this Flooring. If this Flooring has
    /// an IFixedSurface occupant, sets the neighbors of that occupant as well.
    /// </summary>
    /// <param name="neighbors">the four ISurfaces that are adjacent to this Flooring.</param>
    public void SetNeighbors(ISurface[] neighbors)
    {
        Assert.IsNotNull(neighbors);
        Assert.IsTrue(neighbors.Length == 4);
        Neighbors = neighbors;
        if(IsOccupied() && Occupant.Object is IFixedSurface surface) surface.SetNeighbors(neighbors);
    }

    /// <summary>
    /// Returns true if an IFixedSurface candidate can be placed on this Flooring. If so,
    /// places the candidate.
    /// </summary>
    /// <param name="candidate">The candidate to place.</param>
    /// <returns>true if a candidate can be placed on this Flooring; otherwise,
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
    /// Returns true if a candidate can be placed on this Flooring. If so,
    /// places the candidate.
    /// </summary>
    /// <param name="candidate">The PlaceableObject to place.</param>
    /// <returns>true if a candidate can be placed on this Flooring;
    /// false otherwise.</returns>
    public bool Place(ISurfacePlaceable candidate)
    {
        if (Occupant is ISurface surface) return surface.Place(candidate);
        if (!CanPlace(candidate)) return false;
        SetupCandidateOnFlooring(candidate.Object);
        Occupant = candidate.Object;
        UpdateAppearanceBasedOnNeighbors();
        UpdateAppearanceOfNeighbors();
        return true;
    }

    /// <summary>
    /// Returns true if a candidate can be placed on this Flooring.
    /// </summary>
    /// <param name="candidate">The candidate to place.</param>
    /// <returns>true if a candidate can be placed on this Flooring;
    /// otherwise, false.</returns>
    private bool CanPlace(ISurfacePlaceable candidate)
    {
        if (IsOccupied()) return false;
        if (candidate == null) return false;
        if (candidate.Object as Tree == null) return false;
        return true;
    }

    /// <summary>
    /// Returns true if this Flooring hosts an IUseable.
    /// </summary>
    /// <returns>true if this Flooring hosts an IUseable; otherwise, false.
    /// </returns>
    public bool IsOccupied() => Occupant != null;

    /// <summary>
    /// Physically sets up the candidate on this Flooring.
    /// </summary>
    /// <param name="candidate">the candidate to set up.</param>
    private void SetupCandidateOnFlooring(PlaceableObject candidate)
    {
        Assert.IsNotNull(candidate);
        candidate.SetSortingOrder(Coordinates.y);
        candidate.transform.SetParent(transform);
        candidate.transform.localPosition = new Vector3(BoardConstants.XPlacementOffset, BoardConstants.YPlacementOffset, transform.localPosition.z + BoardConstants.ZPlacementOffset);
        candidate.transform.localScale = Vector3.one;
    }

    /// <summary>
    /// Updates the appearance of this Flooring based on its neighbors.
    /// </summary>
    public void UpdateAppearanceBasedOnNeighbors() 
    {
        if (!Defined) return;
        int index = GetTilingIndex(Neighbors);
        if (!FlooringFactory.ValidFlooringIndex(index)) return;
        SetSprite(FlooringFactory.GetFlooringSprite(TYPE, index));
        if (Occupant is IFixedSurface fixedSurfaceOccupant) fixedSurfaceOccupant.UpdateAppearanceBasedOnNeighbors();
    }

    /// <summary>
    /// Updates the appearance of this Flooring's neighbors who
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
    /// Returns true if the given Model type lies on this Flooring or
    /// any of its occupants.
    /// </summary>
    /// <typeparam name="T">The Model type to check for.</typeparam>
    /// <returns> true if the given Model type lies on this Flooring or
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
    /// Returns true if the given Model type lies on this Flooring or
    /// any of its occupants.
    /// </summary>
    /// <param name="modelType">The Model type to check for.</param>
    /// <returns>true if the given Model type lies on this Flooring or
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
    /// Returns and removes the PlaceableObject from this Flooring. 
    /// </summary>
    /// <returns>the PlaceableObject on this Flooring; null if there is none.</returns>
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
    /// this Flooring and is called during a hover / placement action.
    /// This method does not carry out actual placement of the candidate on
    /// this Flooring. Instead, it displays a potential placement scenario.
    /// </summary>
    /// <param name="ghost">The candidate object that we are
    /// trying to virtually place on this Flooring.</param>
    /// <returns> true if the ghost place was successful; otherwise,
    /// false. </returns> 
    public bool GhostPlace(ISurfacePlaceable candidate)
    {
        //Occupied by a Tree? Pass the ghost place.
        if(Occupant is ISurface surface) return surface.GhostPlace(candidate);
        if (!CanGhostPlace(candidate)) return false;
        if (candidate.Object is Defender defender) DrawRangeIndicator(defender);
        GhostOccupant = SetupGhostCandidateOnFlooring(candidate.Object);
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
    /// on this Flooring. This method is invoked alongside GhostPlace() during a
    /// hover or placement action to validate the placement feasibility.
    /// </summary>
    /// <param name="candidate">The candidate object that we are
    /// trying to virtually place on this Flooring.</param>
    /// <returns>true if the candidate object can be placed on this Flooring;
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
    /// Returns the GameObject that represents the ghost candidate on this Flooring.
    /// Sets up the ghost candidate's SpriteRenderer and MaterialPropertyBlock.
    /// </summary>
    /// <param name="ghostCandidate">The ghost candidate to set up.</param>
    /// <returns>the GameObject that represents the ghost candidate on this Flooring.</returns>
    private GameObject SetupGhostCandidateOnFlooring(PlaceableObject ghostCandidate)
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
    /// Returns true if the ghost occupant on this Flooring is not null.
    /// </summary>
    /// <returns>true if the ghost occupant on this Flooring is not null;
    /// otherwise, false.</returns>
    public bool IsGhostOccupied() => GhostOccupant != null;

    /// <summary>
    /// Removes all visual simulations of placing an IUseable on this
    /// Flooring. If there are none, does nothing.
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
    /// Returns true if a pathfinder can walk across this Flooring.
    /// </summary>
    /// <returns>true if a pathfinder can walk across this Flooring;
    /// otherwise, false.</returns>
    public bool IsCurrentlyWalkable()
    {
        if (Occupant is IFixedSurface fixedSurface) return fixedSurface.IsCurrentlyWalkable();
        else if (IsOccupied() && !Occupant.IsTraversable) return false;
        return IsTraversable;
    }

    /// <summary>
    /// Resets this Tile's stats to their starting values.
    /// </summary>
    public override void ResetModel() => base.ResetModel();

    /// <summary>
    /// Returns a Sprite that represents this Flooring when it is
    /// being placed.
    /// </summary>
    /// <returns> a Sprite that represents this Flooring when it is
    /// being placed.</returns>
    public override Sprite[] GetPlacementTrack() => new Sprite[] { GetSprite() };

    /// <summary>
    /// Returns the index representing the correct Sprite in this 
    /// Flooring's tile set. The correct sprite is determined by whether its
    /// neighbors are null or valid. <br></br>
    /// </summary>
    /// <param name="neighbors">this Flooring's neighbors</param>
    /// <returns>the index representing the correct Sprite in this 
    /// Flooring's tile set.</returns>
    protected abstract int GetTilingIndex(ISurface[] neighbors);

    #endregion
}
