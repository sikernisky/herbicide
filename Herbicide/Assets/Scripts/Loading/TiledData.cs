using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Represents map data from a Tiled JSON file, deserialized.
/// </summary>
[System.Serializable]
public class TiledData
{
    #region Fields

    /// <summary>
    /// The maximum height of this level's TileGrid.
    /// </summary>
    public int height;

    /// <summary>
    /// The maximum width of this level's TileGrid.
    /// </summary>
    public int width;

    /// <summary>
    /// The number of stages in this level.
    /// </summary> 
    private int numStages;

    /// <summary>
    /// All custom properties for this level. 
    /// </summary>
    public List<PropertiesData> properties;

    /// <summary>
    /// All deserialized layers packed within this TiledData object.
    /// </summary>
    public List<LayerData> layers;

    /// <summary>
    /// All deserialized tilesets packaged within this TileData object.
    /// </summary>
    public List<TilesetData> tilesets;

    #endregion

    #region Methods

    /// <summary>
    /// Returns a list of LayerData objects that represent Tile layers.
    /// </summary>
    /// <returns>a list of tile LayerData objects.</returns>
    private List<LayerData> GetTileLayers()
    {
        List<LayerData> tileLayers = new List<LayerData>();
        layers.ForEach(l => { if (l.IsTileLayer()) tileLayers.Add(l); });
        return tileLayers;
    }

    /// <summary>
    /// Returns all LayerData objects that represent a layer of Water
    /// Tiles.
    /// </summary>
    /// <returns>a list of layers of Water Tiles.</returns>
    public List<LayerData> GetWaterLayers()
    {
        List<LayerData> tileLayers = GetTileLayers();
        List<LayerData> waterLayers = new List<LayerData>();
        tileLayers.ForEach(l => { if (l.GetLayerName() == "Water") waterLayers.Add(l); });
        return waterLayers;
    }

    /// <summary>
    /// Returns all LayerData objects that represent a layer of Grass
    /// Tiles.
    /// </summary>
    /// <returns>a list of layers of Grass Tiles.</returns>
    public List<LayerData> GetGrassLayers()
    {
        List<LayerData> tileLayers = GetTileLayers();
        List<LayerData> grassLayers = new List<LayerData>();
        tileLayers.ForEach(l => { if (l.GetLayerName() == "Grass") grassLayers.Add(l); });
        return grassLayers;
    }

    /// <summary>
    /// Returns all LayerData objects that represent a layer of Shore
    /// Tiles.
    /// </summary>
    /// <returns>a list of layers of Shore Tiles.</returns>
    public List<LayerData> GetShoreLayers()
    {
        List<LayerData> edgeLayers = GetEdgeLayers();
        List<LayerData> shoreLayers = new List<LayerData>();
        edgeLayers.ForEach(l => { if (l.GetLayerName() == "Shore") shoreLayers.Add(l); });
        return shoreLayers;
    }

    /// <summary>
    /// Returns a list of LayerData objects that represent Edge layers.
    /// </summary>
    /// <returns>a list of Edge LayerData objects.</returns>
    private List<LayerData> GetEdgeLayers()
    {
        List<LayerData> edgeLayers = new List<LayerData>();
        layers.ForEach(l => { if (l.IsEdgeLayer()) edgeLayers.Add(l); });
        return edgeLayers;
    }

    /// <summary>
    /// Returns a list of LayerData objects that represent enemy objects.
    /// </summary>
    /// <returns>a list of enemy object LayerData objects.</returns>
    public List<LayerData> GetEnemyLayers()
    {
        List<LayerData> enemyLayers = new List<LayerData>();
        foreach (LayerData ld in layers)
        {
            if (ld.IsEnemyLayer() && !enemyLayers.Contains(ld)) enemyLayers.Add(ld);
        }
        return enemyLayers;
    }

    /// <summary>
    /// Returns a list of LayerData objects that represent object layers.
    /// </summary>
    /// <returns> a list of LayerData objects that represent object layers.</returns>
    public List<LayerData> GetObjectLayers()
    {
        List<LayerData> objectLayers = new List<LayerData>();
        foreach (LayerData ld in layers)
        {
            if (ld.IsObjectLayer() && !objectLayers.Contains(ld)) objectLayers.Add(ld);
        }
        return objectLayers;
    }

    /// <summary>
    /// Returns a Vector2Int whose X and Y values represent the width and height
    /// of LayerData objects in this TiledData object, respectively.
    /// </summary>
    /// <returns>a Vector2Int with x as width and y as height, in tiles</returns>
    public Vector2Int GetLayerDimensions()
    {
        return new Vector2Int(width, height);
    }

    /// <summary>
    /// Returns a list, in ascending order, of all first Global Tile IDs
    /// from all Tilesets used in the map represented by this TiledData.
    /// </summary>
    /// <returns>a sorted list of all first GIDs</returns>
    public List<int> GetAllFirstGIDs()
    {
        List<int> gids = new List<int>();
        tilesets.ForEach(t => gids.Add(t.GetTilesetFirstGID()));
        gids.Sort();
        return gids;
    }

    /// <summary>
    /// Returns the height, in Tiles, of this map.
    /// </summary>
    /// <returns>the height, in Tiles, of this map.</returns>
    public int GetMapHeight() => height;

    /// <summary>
    /// Returns the number of stages in this map.
    /// </summary>
    /// <returns> the number of stages in this map.</returns>
    public int GetNumStages()
    {
        if (numStages == 0) FindNumStages();
        Assert.IsFalse(numStages <= 0, "numStages is <= 0.");
        return numStages;
    }

    /// <summary>
    /// Searches for the numStages property in this TiledData object.
    /// If found, sets the numStages field to the value of the property.
    /// </summary>
    private void FindNumStages()
    {
        foreach (PropertiesData pd in properties)
        {
            if (pd.name == "numStages")
            {
                numStages = int.Parse(pd.value);
                return;
            }
        }
    }

    #endregion
}
