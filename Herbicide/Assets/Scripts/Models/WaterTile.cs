/// <summary>
/// Represents a Water Tile.
/// </summary>
public class WaterTile : Tile
{
    #region Fields

    #endregion

    #region Stats

    /// <summary>
    /// true if this WaterTile is walkable; otherwise, false.
    /// </summary>
    public override bool IsTraversable => false;

    /// <summary>
    /// ModelType of a Water Tile.
    /// </summary>
    public override ModelType TYPE => ModelType.WATER_TILE;

    #endregion

    #region Methods

    /// <summary>
    /// Sets the local Tiled index of this WaterTile.
    /// </summary>
    /// <param name="localIndex">the local Tiled ID of this WaterTile in its Tiled tile set.</param>
    public override void SetSpriteUsingLocalTiledIndex(int localIndex) { }

    #endregion
}
