using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Represents an object deserialized from a Tiled JSON
/// file. This is abstract; subclasses implement more
/// specific variations, such as enemies, defenders, etc.
/// </summary>
[SerializeField]
public class ObjectData
{
    /// <summary>
    /// All of this object's custom properties.
    /// </summary>
    public List<PropertiesData> properties;

    /// <summary>
    /// The type of object.
    /// </summary>
    public string type;

    /// <summary>
    /// The X-Spawn position of this object, multiplied by the number
    /// of pixels per tile.
    /// </summary>
    public int x;

    /// <summary>
    /// The Y-Spawn position of this object, multiplied by the number
    /// of pixels per tile.
    /// </summary>
    public int y;

    /// <summary>
    /// Height, in pixels, of this object.
    /// </summary>
    public int height;

    /// <summary>
    /// Width, in pixels, of this object.
    /// </summary>
    public int width;

    /// <summary>
    /// The time at which this ObjectData spawns in the level.
    /// </summary>
    private float spawnTime = -1;

    /// <summary>
    /// The name of this ObjectData. The Tiled2d mappings are:<br></br>
    /// 
    /// (1) Enemy: enemyName
    /// </summary>
    private string objectName;

    /// <summary>
    /// The (X, Y) coordinates where this ObjectData should spawn.
    /// </summary>
    private Vector2Int spawnCoords;

    /// <summary>
    /// Returns true if this ObjectData is a deserialized enemy.
    /// </summary>
    /// <returns>true if this ObjectData is a deserialized enemy;
    /// otherwise, false.</returns>
    public bool IsEnemy()
    {
        Assert.IsNotNull(type, "This ObjectData's `type` field is null.");
        return type.ToLower() == "enemy";
    }

    /// <summary>
    /// Returns the name of this Enemy ObjectData.
    /// </summary>
    /// <returns>the name of this Enemy ObjectData.</returns>
    public string GetEnemyName()
    {
        Assert.IsTrue(IsEnemy(), "This ObjectData is not an Enemy.");
        if (objectName == null) FindObjectName();
        return objectName;
    }

    /// <summary>
    /// Searches through this ObjectData's custom properties to find the
    /// name of this object. Sets it.
    /// </summary>
    private void FindObjectName()
    {
        if (objectName != null) return;

        foreach (PropertiesData pd in properties)
        {
            if (pd.GetPropertyName().ToLower() == "enemyname")
            {
                objectName = pd.GetPropertyValue();
                return;
            }
        }

        throw new System.Exception("Name not found.");
    }

    /// <summary>
    /// Returns the spawn time of this ObjectData.
    /// </summary>
    /// <returns>the time at which this ObjectData spawns in the game.</returns>
    public float GetSpawnTime()
    {
        Assert.IsTrue(IsEnemy(), "This ObjectData has no spawn time property.");
        if (spawnTime < 0) FindSpawnTime();
        return spawnTime;
    }

    /// <summary>
    /// Searches through this ObjectData's custom properties to find the spawn time
    /// custom property. Sets it.
    /// </summary>
    private void FindSpawnTime()
    {
        Assert.IsTrue(IsEnemy(), "This ObjectData has no spawn time property.");
        if (spawnTime > 0) return;

        foreach (PropertiesData pd in properties)
        {
            if (pd.GetPropertyName().ToLower() == "spawntime")
            {
                spawnTime = float.Parse(pd.GetPropertyValue());
                return;
            }
        }
    }

    /// <summary>
    /// Returns the (X, Y) coordinates where this ObjectData spawns.
    /// </summary>
    /// <returns>the (X, Y) coordinates where this ObjectData spawns.</returns>
    /// <param name="mapHeight">The height, in tiles, of the map. Needed because Tiled
    /// starts Y's 0 at the top, but we start it at the bottom. </param>
    public Vector2Int GetSpawnCoordinates(int mapHeight)
    {
        if (spawnCoords == null) spawnCoords = new Vector2Int();
        spawnCoords.Set(x / width, mapHeight - (y / height));
        return spawnCoords;
    }
}
