using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Data structure to hold information about a PlaceableObject's spawn
/// rate in the shop (customizable in the Unity Editor, per level).
/// </summary>
[System.Serializable]
public class ShopModel
{
    /// <summary>
    /// The prefab for this ShopModel.
    /// </summary>
    [SerializeField]
    private PlaceableObject prefab;

    /// <summary>
    /// The percent chance the PlaceableObject has to be on the next spawned
    /// ShopBoat.
    /// </summary>
    [SerializeField]
    private float spawnRate;

    /// <summary>
    /// Returns the percent chance the PlaceableObject has to be on the next
    /// spawned ShopBoat, in [0, 1].
    /// </summary>
    /// <returns></returns>
    public float GetSpawnRate()
    {
        Assert.IsTrue(spawnRate >= 0f && spawnRate <= 1f, "Spawn rate needs to be in [0,1]");
        return spawnRate;
    }

    /// <summary>
    /// Returns this ShopModel's PlaceableObject prefab.
    /// </summary>
    /// <returns>this ShopModel's PlaceableObject prefab.</returns>
    public PlaceableObject GetPlaceablePrefab() { return prefab; }
}
