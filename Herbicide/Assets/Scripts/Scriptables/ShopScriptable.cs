using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores data for the shop of a specific level.
/// </summary>
[CreateAssetMenu(fileName = "ShopScriptable", menuName = "Shop Scriptable", order = 0)]
public class ShopScriptable : ScriptableObject
{
    /// <summary>
    /// The Models that can spawn in this Shop and their
    /// stats.
    /// </summary>
    [SerializeField]
    private List<ShopModel> shopModels;

    /// <summary>
    /// The level number this ShopScriptable represents.
    /// </summary>
    [SerializeField]
    private int levelNum;

    /// <summary>
    /// The maximum amount of time between two spawns.
    /// </summary>
    [SerializeField]
    private float maxSpawnGap;

    /// <summary>
    /// The minimum amount of time between two spawns.
    /// </summary>
    [SerializeField]
    private float minSpawnGap;

    /// <summary>
    /// Returns a copied, complete list of ShopModels that make up
    /// this level's shop.
    /// </summary>
    /// <param name="List<ShopModel>(shopModels"></param>
    /// <typeparam name="ShopModel"></typeparam>
    /// <returns></returns>
    public List<ShopModel> GetShop() { return new List<ShopModel>(shopModels); }

    /// <summary>
    /// Returns the maximum amount of time between two spawns.
    /// </summary>
    /// <returns>the maximum amount of time between two spawns.</returns>
    public float GetMaxSpawnGap() { return maxSpawnGap; }

    /// <summary>
    /// Returns the minimum amount of time between two spawns.
    /// </summary>
    /// <returns>the minimum amount of time between two spawns.</returns>
    public float GetMinSpawnGap() { return minSpawnGap; }
}
