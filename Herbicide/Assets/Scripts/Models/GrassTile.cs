/// <summary>
/// Represents a Grass Tile.
/// </summary>
public class GrassTile : Tile
{
    #region Fields

    #endregion

    #region Stats   

    /// <summary>
    /// Type of this GrassTile.
    /// </summary>
    protected override TileType type => TileType.GRASS;

    /// <summary>
    /// Grass Tiles are walkable.
    /// </summary>
    public override bool WALKABLE => true;

    /// <summary>
    /// ModelType of this GrassTile.
    /// </summary>
    public override ModelType TYPE => ModelType.GRASS_TILE;

    #endregion

    #region Methods

    /// <summary>
    /// Defines this GrassTile and sets its sprite to represent a light
    /// GrassTile.
    /// </summary>
    /// <param name="x">The X-Coordinate of this GrassTile.</param>
    /// <param name="y">The Y-Coordinate of this GrassTile.</param>
    /// <param name="tileId">The local Tile Id of this GrassTile in Tiled.</param>
    public void Define(int x, int y, int tileId)
    {
        base.Define(x, y);
        SetSprite(TileFactory.GetTileSprite(type.ToString(), tileId));
    }

    #endregion
}
