/// <summary>
/// Represents an Impassable Wall. 
/// </summary>
public abstract class Wall : Mob
{
    #region Fields

    #endregion

    #region Stats

    /// <summary>
    /// Starting health of a Wall
    /// </summary>
    public override float BASE_HEALTH => int.MaxValue;

    /// <summary>
    /// Maximum health of a Wall
    /// </summary> 
    public override float MAX_HEALTH => int.MaxValue;
    /// <summary>
    /// Minimum health of a Wall
    /// </summary>
    public override float MIN_HEALTH => 0;

    /// <summary>
    /// true if this Wall is an occupier, false otherwise.
    /// </summary>
    public override bool OCCUPIER => true;

    #endregion

    #region Methods

    /// <summary>
    /// Updates this Wall with its newest four neighbors. Sets its sprite
    /// accordingly. /// 
    /// </summary>
    /// <param name="neighbors"> The four neighbors that surround this Wall.</param>
    public override void UpdateNeighbors(PlaceableObject[] neighbors)
    {
        base.UpdateNeighbors(neighbors);
        SetTilingIndex(GetTilingIndex(neighbors));
    }

    /// <summary>
    /// Sets the tiling index of this Wall to the given index.
    /// </summary>
    /// <param name="newIndex"> the new tiling index. </param>
    protected void SetTilingIndex(int newIndex)
    {
        if (!WallFactory.ValidWallIndex(newIndex)) return;
        RefreshRenderer();
        SetSprite(WallFactory.GetWallSprite(TYPE, newIndex));
    }

    /// <summary>
    /// Returns the tiling index of this Wall based off its neighbors.
    /// </summary>
    /// <param name="neighbors">Up to date neighbors of this Wall. </param>
    /// <returns>the tiling index of this Wall based off its neighbors. </returns>
    protected abstract int GetTilingIndex(PlaceableObject[] neighbors);

    #endregion
}
