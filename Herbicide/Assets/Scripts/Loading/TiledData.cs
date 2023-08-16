using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Represents all map data that a Level needs to load correctly. An
/// instance of this class is deserialized from JSON.
/// </summary>
[System.Serializable]
public class TiledData
{
    /// <summary>
    /// The maximum height of this level's TileGrid.
    /// </summary>
    public int height;

    /// <summary>
    /// The maximum width of this level's TileGrid.
    /// </summary>
    public int width;

    /// <summary>
    /// All deserialized layers packed within this TiledData object.
    /// </summary>
    public List<LayerData> layers;

    /// <summary>
    /// All deserialized tilesets packaged within this TileData object.
    /// </summary>
    public List<TilesetData> tilesets;

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
    /// Returns all LayerData objects that represent a layer of Soil
    /// Flooring.
    /// </summary>
    /// <returns>a list of layers of Soil flooring.</returns>
    public List<LayerData> GetSoilLayers()
    {
        List<LayerData> flooringLayers = GetFlooringLayers();
        List<LayerData> soilLayers = new List<LayerData>();
        flooringLayers.ForEach(l => { if (l.GetLayerName() == "Soil") soilLayers.Add(l); });
        return soilLayers;
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
    /// Returns a list of LayerData objects that represent Flooring layers.
    /// </summary>
    /// <returns>a list of Flooring LayerData objects.</returns>
    private List<LayerData> GetFlooringLayers()
    {
        List<LayerData> flooringLayers = new List<LayerData>();
        layers.ForEach(l => { if (l.IsFlooringLayer()) flooringLayers.Add(l); });
        return flooringLayers;
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
            if (ld.IsObjectLayer() && !enemyLayers.Contains(ld)) enemyLayers.Add(ld);
        }
        return enemyLayers;
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
    public int GetMapHeight()
    {
        return height;
    }
}
