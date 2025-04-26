/// <summary>
/// Represents an Edge Tile that borders some water and
/// some terrain.
/// </summary>
public class ShoreTile : EdgeTile
{
    #region Fields

    #endregion

    #region Stats

    /// <summary>
    /// Shore Tiles are not walkable.
    /// </summary>
    public override bool IsTraversable => false;

    /// <summary>
    /// Model type of a ShoreTile.
    /// </summary>
    public override ModelType TYPE => ModelType.SHORE_TILE;

    #endregion
}
