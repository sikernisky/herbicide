using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Produces assets for Hazards.
/// </summary>
public class HazardFactory : MonoBehaviour
{
    /// <summary>
    /// Reference to the HazardFactory Singleton.
    /// </summary>
    private static HazardFactory instance;

    /// <summary>
    /// All Hazard Scriptables.
    /// </summary>
    [SerializeField]
    private List<HazardScriptable> hazardScriptables;

    /// <summary>
    /// Finds and sets the HazardFactory singleton.
    /// </summary>
    /// <param name="levelController">The LevelController singleton.</param>
    public static void SetSingleton(LevelController levelController)
    {
        if (levelController == null) return;

        HazardFactory[] hazardFactories = FindObjectsOfType<HazardFactory>();
        Assert.IsNotNull(hazardFactories, "Array of DefenderFactories is null.");
        Assert.AreEqual(1, hazardFactories.Length);
        instance = hazardFactories[0];
    }

    /// <summary>
    /// Returns the GameObject prefab that represents a Hazard.
    /// </summary>
    /// <param name="hazardType">the type of Hazard</param>
    /// <returns>the GameObject prefab that represents a Hazard</returns>
    public static GameObject GetHazardPrefab(ModelType hazardType)
    {
        HazardScriptable data = instance.hazardScriptables.Find(
            x => x.GetModelType() == hazardType);
        GameObject prefabToClone = data.GetModelPrefab();
        Assert.IsNotNull(prefabToClone);
        return prefabToClone;
    }
}
