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
    public override float BaseHealth => int.MaxValue;

    /// <summary>
    /// Maximum health of a Wall
    /// </summary> 
    public override float MaxHealth => int.MaxValue;
    /// <summary>
    /// Minimum health of a Wall
    /// </summary>
    public override float MinHealth => 0;

    #endregion

    #region Methods

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
