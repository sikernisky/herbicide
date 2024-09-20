using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Produces assets related to Trees.
/// </summary>
public class TreeFactory : Factory
{
    #region Fields

    /// <summary>
    /// Reference to the TreeFactory singleton.
    /// </summary>
    private static TreeFactory instance;

    /// <summary>
    /// Animation track when placing for a BasicTree.
    /// </summary>
    [SerializeField]
    private Sprite[] basicTreePlacementTrack;

    /// <summary>
    /// Animation track when placing for a SpeedTree.
    /// </summary>
    [SerializeField]
    private Sprite[] speedTreePlacementTrack;

    #endregion

    #region Methods

    /// <summary>
    /// Finds and sets the TreeFactory singleton.
    /// </summary>
    /// <param name="levelController">The LevelController singleton.</param>
    public static void SetSingleton(LevelController levelController)
    {
        if (levelController == null) return;
        if (instance != null) return;

        TreeFactory[] treeFactories = FindObjectsOfType<TreeFactory>();
        Assert.IsNotNull(treeFactories, "Array of treeFactories is null.");
        Assert.AreEqual(1, treeFactories.Length);
        instance = treeFactories[0];
        instance.SpawnPools();
    }

    /// <summary>
    /// Returns a prefab for a given Tree type from the object pool.
    /// </summary>
    /// <param name="modelType">The ModelType of the Tree to get</param>
    /// <returns>a prefab for a given Tree type from the object pool.</returns>
    public static GameObject GetTreePrefab(ModelType modelType) => instance.RequestObject(modelType);

    /// <summary>
    /// Accepts a Tree prefab and puts it back in the object pool.
    /// </summary>
    /// <param name="prefab">the prefab to accept.</param>
    public static void ReturnTreePrefab(GameObject prefab) => instance.ReturnObject(prefab);

    /// <summary>
    /// Returns the animation track that represents this Tree when placing.
    /// </summary>
    /// <param name="m">The ModelType of the Tree to get</param>
    /// <returns>the animation track that represents this Tree when placing.</returns>
    public static Sprite[] GetPlacementTrack(ModelType m)
    {
        switch (m)
        {
            case ModelType.BASIC_TREE:
                return instance.basicTreePlacementTrack;
            case ModelType.SPEED_TREE:
                return instance.speedTreePlacementTrack;
            default:
                throw new System.Exception("Invalid ModelType");
        }
    }

    /// <summary>
    /// Returns the Transform component of the TreeFactory instance. 
    /// </summary>
    /// <returns>the Transform component of the TreeFactory instance. </returns>
    protected override Transform GetTransform() => instance.transform;

    #endregion
}
