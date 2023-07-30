using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents all data that a Level needs to load correctly.
/// </summary>
[System.Serializable]
public class LevelData
{
    /// <summary>
    /// List of Enemy data.
    /// </summary>
    [SerializeField]
    private List<EnemyData> enemies;

    /// <summary>
    /// Returns a copy of the List of this level's Enemy data.
    /// </summary>
    /// <returns>a copy of the List of this level's Enemy data.</returns>
    public List<EnemyData> GetEnemyData()
    {
        return new List<EnemyData>(enemies);
    }
}
