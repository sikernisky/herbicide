using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Stores data for Hazards.
/// </summary>
[CreateAssetMenu(fileName = "HazardScriptable", menuName = "Hazard Scriptable", order = 0)]
public class HazardScriptable : ScriptableObject
{
    /// <summary>
    /// Prefab of this Hazard.
    /// </summary>
    [SerializeField]
    private GameObject hazardPrefab;

    /// <summary>
    /// Type of the Hazard.
    /// </summary>
    [SerializeField]
    private Hazard.HazardType hazardType;

    /// <summary>
    /// Returns the HazardType of this HazardScriptable.
    /// </summary>
    /// <returns>the HazardType of this HazardScriptable.
    /// </returns>
    public Hazard.HazardType GetProjectileType() => hazardType;

    /// <summary>
    /// Returns the prefab that represents this Hazard.
    /// </summary>
    /// <returns>the prefab that represents this Hazard.</returns>
    public GameObject GetPrefab()
    {
        Assert.IsNotNull(hazardPrefab.GetComponent<Hazard>(), "Prefab has no Hazard component.");
        return hazardPrefab;
    }
}
