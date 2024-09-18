using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Produces assets related to Collectables.
/// </summary>
public class CollectableFactory : Factory
{
    #region Fields

    /// <summary>
    /// Reference to the CollectableFactory singleton.
    /// </summary>
    private static CollectableFactory instance;

    /// <summary>
    /// Animation track when placing a Dew.
    /// </summary>
    [SerializeField]
    private Sprite[] dewPlacementTrack;

    /// <summary>
    /// Animation track when placing a BasicTreeSeed.
    /// </summary>
    [SerializeField]
    private Sprite[] basicTreeSeedPlacementTrack;

    #endregion

    #region Methods

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
        instance.SpawnPools();
    }

    /// <summary>
    /// Returns a prefab for a given Collectable type from the object pool.
    /// </summary>
    /// <param name="modelType">The ModelType of the Collectable to get</param>
    /// <returns>a prefab for a given Collectable type from the object pool.</returns>
    public static GameObject GetCollectablePrefab(ModelType modelType) => instance.RequestObject(modelType);

    /// <summary>
    /// Accepts a Collectable prefab and puts it back in the object pool.
    /// </summary>
    /// <param name="prefab">the prefab to accept.</param>
    public static void ReturnCollectablePrefab(GameObject prefab) => instance.ReturnObject(prefab);

    /// <summary>
    /// Returns the animation track that represents this Collectable when placing.
    /// </summary>
    /// <param name="m">The ModelType of the Collectable to get</param>
    /// <returns>the animation track that represents this Collectable when placing.</returns>
    public static Sprite[] GetPlacementTrack(ModelType m)
    {
        switch (m)
        {
            case ModelType.DEW:
                return instance.dewPlacementTrack;
            case ModelType.BASIC_TREE_SEED:
                return instance.basicTreeSeedPlacementTrack;
            default:
                throw new System.Exception("Invalid ModelType");
        }
    }

    /// <summary>
    /// Returns the CollectableFactory's transform component.
    /// </summary>
    /// <returns>the CollectableFactory's transform component.</returns>
    protected override Transform GetTransform() => instance.transform;

    #endregion
}
