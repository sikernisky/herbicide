/// <summary>
/// Represents a Water Tile.
/// </summary>
public class WaterTile : Tile
{
    #region Fields

    #endregion

    #region Stats

    /// <summary>
    /// WaterTiles are not walkable.
    /// </summary>
    public override bool WALKABLE => false;

    /// <summary>
    /// ModelType of a Water Tile.
    /// </summary>
    public override ModelType TYPE => ModelType.WATER_TILE;

    /// <summary>
    /// Type of a WaterTile.
    /// </summary>
    protected override TileType type => TileType.WATER;

    #endregion
}
