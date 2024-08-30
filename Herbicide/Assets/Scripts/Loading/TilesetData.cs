/// <summary>
/// Represents a block of Tileset info within a Tiled JSON file,
/// deserialized.
/// </summary>
[System.Serializable]
public class TilesetData
{
    #region Fields

    /// <summary>
    /// The smallest Global Tile ID (GID) that exists in this tileset.
    /// </summary>
    public int firstgid;

    /// <summary>
    /// The source path to the tileset. We use this field not for its
    /// path, but to get the name of the tileset.
    /// </summary>
    public string source;

    /// <summary>
    /// The name of this tileset, parsed from the source field.
    /// </summary>
    private string tilesetName;

    #endregion

    #region Methods

    /// <summary>
    /// Returns the smallest GID within this tileset.
    /// </summary>
    /// <returns>the smallest GID in the tileset.</returns>
    public int GetTilesetFirstGID() => firstgid;

    #endregion  
}
