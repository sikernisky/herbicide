using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyData
{
    /// <summary>
    /// Type of the Enemy to spawn.
    /// </summary>
    [SerializeField]
    private string type;

    /// <summary>
    /// X-Coordinate of where to spawn this Enemy.
    /// </summary>
    [SerializeField]
    private int xSpawn;

    /// <summary>
    /// Y-Coordinate of where to spawn this Enemy.
    /// </summary>
    [SerializeField]
    private int ySpawn;

    /// <summary>
    /// Time in the level at which to spawn this Enemy.
    /// </summary>
    [SerializeField]
    private float time;



    /// <summary>
    /// Returns the type of Enemy represented by this EnemyData.
    /// </summary>
    /// <returns>the type of Enemy represented by this EnemyData.</returns>
    public string GetEnemyType()
    {
        return type;
    }

    /// <summary>
    /// Returns the spawn position, in Tile coordinates, of the Enemy represented
    /// by this EnemyData.
    /// </summary>
    /// <returns>this Enemy's spawn position.</returns>
    public Vector2Int GetSpawnPos()
    {
        return new Vector2Int(xSpawn, ySpawn);
    }

    /// <summary>
    /// Returns the time at which this Enemy should spawn in the level.
    /// </summary>
    /// <returns>the time at which this Enemy should spawn in the level.
    /// </returns>
    public float GetTime()
    {
        return time;
    }
}
