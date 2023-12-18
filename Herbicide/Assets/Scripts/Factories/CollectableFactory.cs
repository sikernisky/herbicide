using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Manages assets for Collectables.
/// </summary>
public class CollectableFactory : MonoBehaviour
{
    /// <summary>
    /// Reference to the CollectableFactory Singleton.
    /// </summary>
    private static CollectableFactory instance;

    /// <summary>
    /// All ScriptableObjects containing data about
    /// different Collectables.
    /// </summary>
    [SerializeField]
    private List<CollectableScriptable> collectableScriptables;


    /// <summary>
    /// Finds and sets the CollectableFactory singleton.
    /// </summary>
    /// <param name="levelController">The LevelController singleton.</param>
    public static void SetSingleton(LevelController levelController)
    {
        if (levelController == null) return;
        if (instance != null) return;

        CollectableFactory[] collectableFactories = FindObjectsOfType<CollectableFactory>();
        Assert.IsNotNull(collectableFactories, "Array of CollectableFactories is null.");
        Assert.AreEqual(1, collectableFactories.Length);
        instance = collectableFactories[0];
    }

    /// <summary>
    /// Returns the GameObject prefab that represents a Collectable.
    /// </summary>
    /// <param name="collectableType">the type of Collectable</param>
    /// <returns>the GameObject prefab that represents a placed Defender</returns>Collectable
    public static GameObject GetCollectablePrefab(Collectable.CollectableType collectableType)
    {
        CollectableScriptable data = instance.collectableScriptables.Find(
            x => x.GetCollectableType() == collectableType);
        GameObject prefabToClone = data.GetPrefab();
        Assert.IsNotNull(prefabToClone);
        return prefabToClone;
    }
}
