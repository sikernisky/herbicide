using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Represents a block of Tileset info within a Tiled JSON file,
/// deserialized.
/// </summary>
[System.Serializable]
public class TilesetData
{
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

    /// <summary>
    /// Returns the name of this tileset.
    /// </summary>
    /// <returns>the name of this tileset.</returns>
    public string GetTilesetName()
    {
        if (tilesetName == null) ParseTilesetName();
        Assert.IsNotNull(tilesetName);
        return tilesetName;
    }

    /// <summary>
    /// Returns the smallest GID within this tileset.
    /// </summary>
    /// <returns>the smallest GID in the tileset.</returns>
    public int GetTilesetFirstGID()
    {
        return firstgid;
    }

    /// <summary>
    /// Extracts the actual name of this Tileset from its source
    /// path. Sets the `tilesetName` field to this value.
    /// </summary>
    private void ParseTilesetName()
    {
        Assert.IsNotNull(source, "Field `source` is null.");
        int lastSlashIndex = source.LastIndexOf('/');
        tilesetName = source.Substring(lastSlashIndex + 1);
    }
}
