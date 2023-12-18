using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Stores data for Collectables.
/// </summary>
[CreateAssetMenu(fileName = "CollectableScriptable", menuName = "Collectable Scriptable", order = 0)]
public class CollectableScriptable : ScriptableObject
{
    /// <summary>
    /// This Collectable's prefab.
    /// </summary>
    public GameObject collectablePrefab;

    /// <summary>
    /// Type of the Collectable.
    /// </summary>
    [SerializeField]
    private Collectable.CollectableType collectableType;


    /// <summary>
    /// Returns the CollectableType that this CollectableScriptable is storing.
    /// </summary>
    /// <returns>the CollectableType that this CollectableScriptable is storing.
    /// </returns>
    public Collectable.CollectableType GetCollectableType() => collectableType;

    /// <summary>
    /// Returns the prefab that represents this Collectable.
    /// </summary>
    /// <returns>the prefab that represents this Collectable.</returns>
    public GameObject GetPrefab()
    {
        Assert.IsNotNull(collectablePrefab.GetComponent<Collectable>(),
         "Prefab has no Collectable component.");
        return collectablePrefab;
    }
}
