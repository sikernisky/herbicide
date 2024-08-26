using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Produces assets related to the Shop.
/// </summary>
public class ShopFactory : Factory
{
    #region Fields

    /// <summary>
    /// Reference to the ShopFactory singleton.
    /// </summary>
    private static ShopFactory instance;

    /// <summary>
    /// How many prefabs to fill each ObjectPool with at start.
    /// </summary>
    protected override int poolStartingCount => 4;

    #endregion

    #region Methods

    /// <summary>
    /// Finds and sets the ShopFactory singleton.
    /// </summary>
    /// <param name="levelController">The LevelController singleton.</param>
    public static void SetSingleton(LevelController levelController)
    {
        if (levelController == null) return;
        if (instance != null) return;

        ShopFactory[] shopFactories = FindObjectsOfType<ShopFactory>();
        Assert.IsNotNull(shopFactories, "Array of SquirrelFactories is null.");
        Assert.AreEqual(1, shopFactories.Length);
        instance = shopFactories[0];
        instance.SpawnPools();
    }

    /// <summary>
    /// Returns a fresh ShopCard prefab for a given type from its object pool.
    /// </summary>
    /// <param name="modelType">The type of ShopCard model to get.</param>
    /// <returns>a GameObject with a ShopCard component attached to it</returns>
    public static GameObject GetShopCardPrefab(ModelType modelType)
    {
        return instance.RequestObject(modelType);
    }

    /// <summary>
    /// Accepts a ShopCard prefab that the caller no longer needs. Adds it back
    /// to its object pool.
    /// </summary>
    /// <param name="prefab">The ShopCard prefab to return.</param>
    public static void ReturnShopCardPrefab(GameObject prefab)
    {
        instance.ReturnObject(prefab);
    }

    /// <summary>
    /// Returns the ShopFactory instance's Transform component.
    /// </summary>
    /// <returns></returns>
    protected override Transform GetTransform() { return instance.transform; }

    #endregion
}
