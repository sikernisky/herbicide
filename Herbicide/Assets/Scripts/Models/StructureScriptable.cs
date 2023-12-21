using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Stores data for structures.
/// </summary>
[CreateAssetMenu(fileName = "StructureScriptable", menuName = "Structure Scriptable", order = 0)]
public class StructureScriptable : ScriptableObject
{
    /// <summary>
    /// Prefab of this Structure.
    /// </summary>
    [SerializeField]
    private GameObject structurePrefab;

    /// <summary>
    /// Type of the Structure.
    /// </summary>
    [SerializeField]
    private Structure.StructureType structureType;


    /// <summary>
    /// Returns the StructureType of this StructureScriptable.
    /// </summary>
    /// <returns>the StructureType of this StructureScriptable.
    /// </returns>
    public Structure.StructureType GetStructureType() => structureType;

    /// <summary>
    /// Returns the prefab that represents this Structure.
    /// </summary>
    /// <returns>the prefab that represents this Structure.</returns>
    public GameObject GetPrefab()
    {
        Assert.IsNotNull(structurePrefab.GetComponent<Structure>(),
            "Prefab has no Structure component.");
        return structurePrefab;
    }
}
