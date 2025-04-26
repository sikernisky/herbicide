/// <summary>
/// Represents a Tile that borders at least one unwalkable boundary and 
/// at least one walkable Tile.
/// </summary>
public abstract class EdgeTile : Tile
{
    #region Fields

    /// <summary>
    /// true if this EdgeTile is walkable; otherwise, false.
    /// </summary>
    public override bool IsTraversable => false;

    #endregion

    #region Methods

    /// <summary>
    /// Sets the local Tiled index of this EdgeTile.
    /// </summary>
    /// <param name="localIndex">the local Tiled ID of this EdgeTile in its Tiled tile set.</param>
    public override void SetSpriteUsingLocalTiledIndex(int localIndex) => SetSprite(EdgeFactory.GetEdgeSprite(TYPE, localIndex));

    #endregion
}
