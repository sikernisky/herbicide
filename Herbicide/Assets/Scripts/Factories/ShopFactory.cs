using System.Collections.Generic;
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

    /// <summary>
    /// Struct that holds a Defender type and a corresponding ShopCard sprite.
    /// </summary>
    [System.Serializable]
    private struct ShopCardSprite
    {
        /// <summary>
        /// Type of Defender.
        /// </summary>
        public ModelType modelType;

        /// <summary>
        /// The ShopCard sprite that represents the Defender.
        /// </summary>
        public Sprite sprite;
    }

    /// <summary>
    /// All Sprites that represent a defender on a ShopCard.
    /// </summary>
    [SerializeField]
    private ShopCardSprite[] shopCardSprites;

    /// <summary>
    /// All ShopCards and their data.
    /// </summary>
    private List<ShopCardData> shopCardData;

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
        instance.ConstructShopCardData();
    }

    /// <summary>
    /// Constructs the ShopCardData dictionary. Eventually, we will replace
    /// this with JSON deserialization if needed.
    /// </summary>
    private void ConstructShopCardData()
    {
        shopCardData = new List<ShopCardData>();

        // Bunny
        ShopCardData bunny = new ShopCardData(
            ModelType.BUNNY,
            "Bunny",
            25);
        AddShopCardData(bunny);

        // Squirrel
        ShopCardData squirrel = new ShopCardData(
            ModelType.SQUIRREL,
            "Squirrel",
            50);
        AddShopCardData(squirrel);
    }

    /// <summary>
    /// Adds a ShopCardData to the ShopFactory's list of ShopCardData.
    /// </summary>
    /// <param name="data">the data to add. </param>
    private void AddShopCardData(ShopCardData data)
    {
        Assert.IsNotNull(data, "ShopCardData is null.");
        Assert.IsNotNull(shopCardData, "ShopCardData list is null.");
        shopCardData.ForEach(t => Assert.AreNotEqual(data.DefenderType, t.DefenderType));
        shopCardData.Add(data);
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
    public static void ReturnShopCardPrefab(GameObject prefab) => instance.ReturnObject(prefab);

    /// <summary>
    /// Returns the ShopFactory instance's Transform component.
    /// </summary>
    /// <returns></returns>
    protected override Transform GetTransform() { return instance.transform; }

    /// <summary>
    /// Returns the Sprite that represents the given Defender on a ShopCard.
    /// </summary>
    /// <param name="modelType">the given model type. </param>
    /// <returns>the Sprite that represents the given Defender on a ShopCard.</returns>
    public static Sprite GetShopCardSprite(ModelType modelType)
    {
        Assert.IsTrue(ModelTypeHelper.IsDefender(modelType), "ModelType is not a Defender.");
        foreach (ShopCardSprite spriteStruct in instance.shopCardSprites)
        {
            if (spriteStruct.modelType == modelType) return spriteStruct.sprite;
        }
        return null;
    }

    /// <summary>
    /// Returns the ShopCardData object for a given Defender type.
    /// </summary>
    /// <param name="defenderType">the Defender type.</param>
    /// <returns> the ShopCardData object for a given Ticket type.</returns>
    public static ShopCardData GetShopCardData(ModelType defenderType)
    {
        Assert.IsTrue(ModelTypeHelper.IsDefender(defenderType), "ModelType is not a Defender.");
        Assert.IsTrue(instance.shopCardData.Exists(t => t.DefenderType == defenderType), "ShopCardData does not exist.");
        return instance.shopCardData.Find(t => t.DefenderType == defenderType);
    }

    #endregion
}
