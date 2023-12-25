using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Data structure to hold information about a Model's spawn
/// rate in the shop (customizable in the Unity Editor, per level).
/// </summary>
[System.Serializable]
public class ShopModel
{
    /// <summary>
    /// The prefab for this ShopModel.
    /// </summary>
    [SerializeField]
    private Model prefab;

    /// <summary>
    /// The percent chance the Model has to be on the next spawned
    /// ShopBoat.
    /// </summary>
    [SerializeField]
    private float spawnRate;

    /// <summary>
    /// Returns the percent chance the Model has to be on the next
    /// spawned ShopBoat, in [0, 1].
    /// </summary>
    /// <returns></returns>
    public float GetSpawnRate()
    {
        Assert.IsTrue(spawnRate >= 0f && spawnRate <= 1f, "Spawn rate needs to be in [0,1]");
        return spawnRate;
    }

    /// <summary>
    /// Returns this ShopModel's model prefab.
    /// </summary>
    /// <returns>this ShopModel's model prefab.</returns>
    public Model GetModelPrefab() { return prefab; }
}
