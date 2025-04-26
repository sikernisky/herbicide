using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Represents a Tree.
/// </summary>
public abstract class Tree : Defender, ISurface
{
    #region Fields

    /// <summary>
    /// The DefenderClass of a Tree.
    /// </summary>
    public override DefenderClass CLASS => DefenderClass.COG;

    /// <summary>
    /// The ISurfacePlaceable that is currently on this Tree.
    /// </summary>
    public PlaceableObject Occupant { get; private set; }

    /// <summary>
    /// The Ghost GameObject that is currently on this Tree.
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
    public override bool IsTraversable => false;

    /// <summary>
    /// Starting main action animation duration of a Tree.
    /// </summary>
    public override float BaseMainActionAnimationDuration => AnimationConstants.TreeMainActionAnimationDuration;

    /// <summary>
    /// Starting movement animation duration of a Tree.
    /// </summary>
    public override float BaseMovementAnimationDuration => AnimationConstants.TreeMovementAnimationDuration;

    #endregion

    #region Methods

    /// <summary>
    /// Returns true if a candidate can be placed on this Tree. If so,
    /// places the candidate.
    /// </summary>
    /// <param name="candidate">The candidate.</param>
    /// <returns>true if a candidate was placed on this Tree; otherwise,
    /// false. </returns>
    public virtual bool Place(ISurfacePlaceable candidate)
    {
        if (Occupant is ISurface surface) return surface.Place(candidate);
        if (!CanPlace(candidate)) return false;
        SetupCandidateOnTree(candidate.Object as Defender);
        Occupant = candidate.Object;
        return true;
    }

    /// <summary>
    /// Returns true if a candidate can be placed on this Tree. A
    /// candidate can be placed if it is a Defender and
    /// this Tree is not already occupied. 
    /// </summary>
    /// <param name="candidate">The candidate.</param>
    /// <returns>true if a candidate can place on this Tree; otherwise,
    /// returns false. </returns>
    private bool CanPlace(ISurfacePlaceable candidate)
    {
        if (IsOccupied()) return false;
        if (candidate == null) return false;
        if (candidate.Object as Defender == null) return false;
        return true;
    }

    /// <summary>
    /// Returns true if there is an occupant on this Tree.
    /// </summary>
    /// <returns>true if there is an occupant on this Tree; otherwise,
    /// false.</returns>
    public bool IsOccupied() => Occupant != null;

    /// <summary>
    /// Physically sets up the candidate on this Flooring.
    /// </summary>
    /// <param name="candidate">the candidate to set up.</param>
    private void SetupCandidateOnTree(Defender candidate)
    {
        Assert.IsNotNull(candidate);
        candidate.ProvideTreePosition(GetWorldPosition());
        candidate.SetSortingOrder(GetSortingOrder());
        candidate.transform.SetParent(transform);
        candidate.transform.localPosition = new Vector3(BoardConstants.XPlacementOffset, BoardConstants.YPlacementOffset,
            transform.localPosition.z + BoardConstants.ZPlacementOffset) + GetOffsetOfOccupantOnTree(candidate.TYPE);
        candidate.transform.localScale = Vector3.one;
    }

    /// <summary>
    /// Returns true if the given Model type lies on this Tree or
    /// any of its occupants.
    /// </summary>
    /// <typeparam name="T">The Model type to check for.</typeparam>
    /// <returns> true if the given Model type lies on this Tree or
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
    /// Returns true if the given Model type lies on this Tree or
    /// any of its occupants.
    /// </summary>
    /// <param name="modelType">The Model type to check for.</param>
    /// <returns>true if the given Model type lies on this Tree or
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
    /// Returns and removes the PlaceableObject from this Tree. This does not
    /// destroy the occupant; that is the responsibility of its controller. 
    /// </summary>
    /// <param name="neighbors">This Tree's neighbors.</param>
    /// <returns>The PlaceableObject that was removed from this Tree.</returns>
    public virtual PlaceableObject Remove()
    {
        if (Occupant is ISurface surface) return surface.Remove();
        if (!IsOccupied()) return null;
        PlaceableObject removed = Occupant;
        Occupant = null;
        return removed;
    }

    /// <summary>
    /// Provides a visual simulation of placing a candidate on
    /// this Tree and is called during a hover / placement action.
    /// This method does not carry out actual placement of the candidate on
    /// this Tree. Instead, it displays a potential placement scenario.
    /// </summary>
    /// <param name="candidate">The candidate object that we are
    /// trying to virtually place on this Tree.</param>
    /// <returns> true if the ghost place was successful; otherwise,
    /// false. </returns> 
    public bool GhostPlace(ISurfacePlaceable candidate)
    {
        if (Occupant is ISurface surface) return surface.GhostPlace(candidate);
        if (!CanGhostPlace(candidate)) return false;
        if (candidate.Object is Defender defender) DrawRangeIndicator(defender);
        GhostOccupant = SetupGhostCandidateOnTree(candidate.Object);
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
    /// Determines whether a PlaceableObject object can be potentially placed
    /// on this Tree. This method is invoked alongside GhostPlace() during a
    /// hover or placement action to validate the placement feasibility.
    /// </summary>
    /// <param name="ghost">The candidate object that we are
    /// trying to virtually place on this Tree.</param>
    /// <returns>true if the candidate object can be placed on this Tree;
    /// otherwise, false.</returns>
    public bool CanGhostPlace(ISurfacePlaceable candidate) => !IsOccupied() && !IsGhostOccupied() && CanPlace(candidate);

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
    /// Returns the GameObject that represents the ghost candidate on this Tree.
    /// Sets up the ghost candidate's SpriteRenderer and MaterialPropertyBlock.
    /// </summary>
    /// <param name="ghostCandidate">The ghost candidate to set up.</param>
    /// <returns>the GameObject that represents the ghost candidate on this Tree.</returns>
    private GameObject SetupGhostCandidateOnTree(PlaceableObject ghostCandidate)
    {
        GameObject hollowCopy = ghostCandidate.MakeHollowObject();
        SpriteRenderer hollowRenderer = hollowCopy.GetComponent<SpriteRenderer>();
        ConfigureGhostCandidateSpriteRenderer(hollowRenderer, ghostCandidate.GetColor());
        hollowCopy.transform.position = transform.position + GetOffsetOfOccupantOnTree(ghostCandidate.TYPE);
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
    /// Returns true if the ghost occupant on this Tree is not null.
    /// </summary>
    /// <returns>true if the ghost occupant on this Tree is not null;
    /// otherwise, false.</returns>
    public bool IsGhostOccupied() => GhostOccupant != null;

    /// <summary>
    /// Removes all visual simulations of placing a PlaceableObject on this
    /// Tree. If there are none, does nothing.
    /// </summary>
    public void GhostRemove()
    {
        if (Occupant is ISurface surface) surface.GhostRemove();
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
    /// Returns true if a pathfinder can walk across this Tree.
    /// </summary>
    /// <returns>true if a pathdfinder can walk across this Tree;
    /// otherwise, false.</returns>
    public bool IsCurrentlyWalkable() 
    {
        if (Occupant is IFixedSurface fixedSurface) return fixedSurface.IsCurrentlyWalkable();
        else if (IsOccupied() && !Occupant.IsTraversable) return false;
        return IsTraversable;
    }

    /// <summary>
    /// If this Tree is not occupied, adjusts its health by amount.
    /// If it is occupied, adjusts its occupant's health instead.  
    /// </summary>
    /// <param name="amount">The amount to adjust by.</param>
    public override void AdjustHealth(float amount)
    {
        if (IsOccupied()) Occupant.Object.AdjustHealth(amount);
        else base.AdjustHealth(amount);
    }

    /// <summary>
    /// Returns the offset of a Defender on this Tree.
    /// </summary>
    /// <param name="type">The type of Defender</param>
    /// <returns>the offset of a Defender on this Tree.</returns>
    private Vector3 GetOffsetOfOccupantOnTree(ModelType type)
    {
        switch (type)
        {
            case ModelType.SQUIRREL:
                return new Vector3(0.0f, 0.75f, 0.0f);
            case ModelType.BEAR:
                return new Vector3(0.0f, 0.75f, 0.0f);
            case ModelType.PORCUPINE:
                return new Vector3(0.0f, 0.35f, 0.0f);
            case ModelType.RACCOON:
                return new Vector3(0.1f, 0.85f, 0.0f);
            case ModelType.OWL:
                return new Vector3(0.0f, 0.90f, 0.0f);
            case ModelType.BUNNY:
                return new Vector3(0.0f, 1.10f, 0.0f);
            default:
                return new Vector3(0.0f, 0.00f, 0.0f);
        }
    }

    #endregion
}
