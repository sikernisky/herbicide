using System.Collections.Generic;
using UnityEngine.Assertions;

/// <summary>
/// Represents a layer within a Tiled JSON file, deserialized.
/// </summary>
[System.Serializable]
public class LayerData
{
    #region Fields

    /// <summary>
    /// All objects within this layer; null if this is not an 
    /// object layer.
    /// </summary>
    public List<ObjectData> objects;

    /// <summary>
    /// All custom properties within this layer. 
    /// </summary>
    public List<PropertiesData> properties;

    /// <summary>
    /// Integers representing this layer's tile info; null if not
    /// a tile layer.
    /// </summary>
    public List<int> data;

    /// <summary>
    /// Name of this Layer, which represents a type of its class. For
    /// example, TileLayer names include Grass and Water, where
    /// a FlooringLayer name is Soil.
    /// </summary>
    public string name;

    /// <summary>
    /// This layer's class in Tiled. Set upon parsing.
    /// </summary>
    private string layerClass;

    #endregion

    #region Methods

    /// <summary>
    /// Returns true if this LayerData layer stores objects.
    /// </summary>
    /// <returns>true if this LayerData layer stores objects.</returns>
    public bool IsObjectLayer() => objects != null && objects.Count > 0;

    /// <summary>
    /// Returns true if this LayerData layer stores Enemy objects.
    /// </summary>
    /// <returns>true if this LayerData stores Enemy objects. </returns>
    public bool IsEnemyLayer()
    {
        Assert.IsNotNull(GetLayerClass(), "This LayerData's `layerClass` is null.");
        return GetLayerClass().ToLower() == "enemylayer";
    }

    /// <summary>
    /// Returns true if this LayerData layer stores Placeable objects.
    /// </summary>
    /// <returns>true if this LayerData stores Placeable objects. </returns>
    public bool IsPlaceableLayer()
    {
        Assert.IsNotNull(GetLayerClass(), "This LayerData's `layerClass` is null.");
        return GetLayerClass().ToLower() == "structurelayer";
    }

    /// <summary>
    /// Returns a copy of the list of all ObjectData objects within 
    /// this LayerData.
    /// </summary>
    /// <returns>a copy of the list of all ObjectData objects within 
    /// this LayerData.</returns>
    private List<ObjectData> GetObjectData()
    {
        Assert.IsTrue(IsObjectLayer());
        Assert.IsNotNull(objects, "Array of ObjectData `objects` is null.");

        return new List<ObjectData>(objects);
    }

    /// <summary>
    /// Returns a list of all ObjectData objects within this LayerData whose
    /// type is an Enemy.
    /// </summary>
    /// <returns>a list of all Enemy ObjectData objects within this LayerData.
    /// </returns>
    public List<ObjectData> GetEnemyObjectData()
    {
        List<ObjectData> enemyObjects = new List<ObjectData>();
        GetObjectData().ForEach(o => { if (o.IsEnemy()) enemyObjects.Add(o); });
        return enemyObjects;
    }

    /// <summary>
    /// Returns a list of all ObjectData objects within this LayerData whose
    /// type is a Structure.
    /// </summary>
    /// <returns>a list of all Structure ObjectData objects within this LayerData.
    /// </returns>
    public List<ObjectData> GetStructureObjectData()
    {
        List<ObjectData> structureObjects = new List<ObjectData>();
        GetObjectData().ForEach(o => { if (o.IsStructure()) structureObjects.Add(o); });
        return structureObjects;
    }

    /// <summary>
    /// Returns a list of all ObjectData objects within this LayerData whose
    /// type is a Marker.
    /// </summary>
    /// <returns>a list of all Marker ObjectData objects within this LayerData.
    /// </returns>
    public List<ObjectData> GetMarkerObjectData()
    {
        List<ObjectData> markerObjects = new List<ObjectData>();
        GetObjectData().ForEach(o => { if (o.IsMarker()) markerObjects.Add(o); });
        return markerObjects;
    }


    /// <summary>
    /// Returns a list of all ObjectData objects within this LayerData whose
    /// type is a Flooring.
    /// </summary>
    /// <returns>a list of all Flooring ObjectData objects within this LayerData.
    /// </returns>
    public List<ObjectData> GetFlooringObjectData()
    {
        List<ObjectData> markerObjects = new List<ObjectData>(); 
        GetObjectData().ForEach(o => { if (o.IsFlooring()) markerObjects.Add(o); });
        return markerObjects;
    }

    /// <summary>
    /// Returns a list of all ObjectData objects within this LayerData whose
    /// type is a Waypoint.
    /// </summary>
    /// <returns>a list of all Waypoint ObjectData objects within this LayerData.
    /// </returns>
    public List<ObjectData> GetWaypointObjectData()
    {
        List<ObjectData> waypointObjects = new List<ObjectData>();
        GetObjectData().ForEach(o => { if (o.IsWaypoint()) waypointObjects.Add(o); });
        return waypointObjects;
    }

    /// <summary>
    /// Returns a copy of the list of integers representing this layer's 
    /// tile data.
    /// </summary>
    /// <returns>a copy of the list of integers representing this layer's 
    /// tile data.</returns>
    public List<int> GetTileData()
    {
        Assert.IsFalse(IsObjectLayer());
        Assert.IsNotNull(data, "Array of integers `data` is null.");

        return new List<int>(data);
    }


    /// <summary>
    /// Returns true if this LayerData is a layer of Tiles.
    /// </summary>
    /// <returns>true if this layer holds Tiles; otherwise, false.</returns>
    public bool IsTileLayer()
    {
        Assert.IsNotNull(GetLayerClass(), "This LayerData's `layerClass` is null.");
        return GetLayerClass().ToLower() == "tilelayer";
    }

    /// <summary>
    /// Returns true if this LayerData is a layer of Edges.
    /// </summary>
    /// <returns>true if this layer holds Edges; otherwise, false.</returns>
    public bool IsEdgeLayer()
    {
        Assert.IsNotNull(GetLayerClass(), "This LayerData's `layerClass` is null.");
        return GetLayerClass().ToLower() == "edgelayer";
    }

    /// <summary>
    /// Returns the name of this LayerData.
    /// </summary>
    /// <returns>the name of this LayerData.</returns>
    public string GetLayerName()
    {
        Assert.IsNotNull(name, "This LayerData's `name` is null.");
        return name;
    }

    /// <summary>
    /// Searches through this LayerData's properties list to find the
    /// layer class property; sets the layerClass field to its value.
    /// </summary>
    public void FindLayerClass()
    {
        foreach (PropertiesData pd in properties)
        {
            if (pd.GetPropertyName().ToLower() == "layerclass")
                layerClass = pd.GetPropertyValue();
        }
    }

    /// <summary>
    /// Returns the name of this layer's class in Tiled.
    /// </summary>
    /// <returns>the name of this layer's class in Tiled.</returns>
    private string GetLayerClass()
    {
        if (layerClass == null) FindLayerClass();
        Assert.IsNotNull(layerClass);
        return layerClass;
    }

    #endregion
}
