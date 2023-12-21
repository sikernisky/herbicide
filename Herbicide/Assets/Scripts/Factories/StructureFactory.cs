using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Manages assets for Structures.
/// </summary>
public class StructureFactory : MonoBehaviour
{
    /// <summary>
    /// Reference to the StructureFactory Singleton.
    /// </summary>
    private static StructureFactory instance;

    /// <summary>
    /// All Structure Scriptables.
    /// </summary>
    [SerializeField]
    private List<StructureScriptable> structureScriptables;


    /// <summary>
    /// Finds and sets the StructureFactory singleton.
    /// </summary>
    /// <param name="levelController">The LevelController singleton.</param>
    public static void SetSingleton(LevelController levelController)
    {
        if (levelController == null) return;

        StructureFactory[] structureFactories = FindObjectsOfType<StructureFactory>();
        Assert.IsNotNull(structureFactories, "Array of DefenderFactories is null.");
        Assert.AreEqual(1, structureFactories.Length);
        instance = structureFactories[0];
    }

    /// <summary>
    /// Returns the GameObject prefab that represents a Structure.
    /// </summary>
    /// <param name="structureType">the type of Structure</param>
    /// <returns>the GameObject prefab that represents a Structure</returns>
    public static GameObject GetStructurePrefab(Structure.StructureType structureType)
    {
        StructureScriptable data = instance.structureScriptables.Find(
            x => x.GetStructureType() == structureType);
        GameObject prefabToClone = data.GetPrefab();
        Assert.IsNotNull(prefabToClone);
        return prefabToClone;
    }

    /// <summary>
    /// Returns the GameObject prefab that represents a Structure.
    /// </summary>
    /// <param name="structureName">the name of the Structure</param>
    /// <returns>the GameObject prefab that represents a Structure</returns>
    public static GameObject GetStructurePrefab(string structureName)
    {
        if (structureName.ToLower() == "nexus") return GetStructurePrefab(Structure.StructureType.NEXUS);
        else if (structureName.ToLower() == "nexushole") return GetStructurePrefab(Structure.StructureType.NEXUS_HOLE);

        // Backup
        return GetStructurePrefab(Structure.StructureType.NEXUS);
    }


}
