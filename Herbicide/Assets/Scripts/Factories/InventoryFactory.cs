using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Produces assets related to the Inventory.
/// </summary>
public class InventoryFactory : Factory
{
    #region Fields

    /// <summary>
    /// Reference to the InventoryFactory singleton.
    /// </summary>
    private static InventoryFactory instance;

    /// <summary>
    /// How many prefabs to fill each ObjectPool with at start.
    /// </summary>
    protected override int poolStartingCount => 1;

    /// <summary>
    /// Struct that holds a model type and its corresponding inventory item icon.
    /// </summary>
    [System.Serializable]
    private struct InventoryItemIcon
    {
        /// <summary>
        /// Type of model.
        /// </summary>
        public ModelType modelType;

        /// <summary>
        /// The icon that represents the model.
        /// </summary>
        public Sprite icon;
    }

    /// <summary>
    /// All icons that can appear on an InventoryItem.
    /// </summary>
    [SerializeField]
    private InventoryItemIcon[] icons;

    /// <summary>
    /// All InventoryItems and their data.
    /// </summary>
    private List<InventoryItemData> inventoryItemData;

    #endregion

    #region Methods

    /// <summary>
    /// Finds and sets the InventoryFactory singleton.
    /// </summary>
    /// <param name="levelController">The LevelController singleton.</param>
    public static void SetSingleton(LevelController levelController)
    {
        if (levelController == null) return;
        if (instance != null) return;

        InventoryFactory[] inventoryFactories = FindObjectsOfType<InventoryFactory>();
        Assert.IsNotNull(inventoryFactories, "Array of InventoryFactories is null.");
        Assert.AreEqual(1, inventoryFactories.Length);
        instance = inventoryFactories[0];
        instance.SpawnPools();
        instance.ConstructInventoryItemData();
    }

    /// <summary>
    /// Constructs the inventory item data dictionary. Eventually, we will replace
    /// this with JSON deserialization if needed.
    /// </summary>
    private void ConstructInventoryItemData()
    {
        inventoryItemData = new List<InventoryItemData>();

        // Bunadryl Item
        InventoryItemData bunadryl = new InventoryItemData(
            ModelType.INVENTORY_ITEM_BUNADRYL,
            "Bunadryl");
        AddInventoryItemData(bunadryl);

        // Acornol Item
        InventoryItemData acornol = new InventoryItemData(
            ModelType.INVENTORY_ITEM_ACORNOL,
            "Acornol");
        AddInventoryItemData(acornol);
    }

    /// <summary>
    /// Adds a InventoryItem object to the inventoryItemData list.
    /// </summary>
    /// <param name="data">The InventoryItemData object to add.</param>
    private void AddInventoryItemData(InventoryItemData data)
    {
        Assert.IsNotNull(data, "InventoryItemData is null.");
        Assert.IsNotNull(inventoryItemData, "InventoryItemData list is null.");
        inventoryItemData.ForEach(t => Assert.AreNotEqual(data.InventoryItemType, t.InventoryItemType));
        inventoryItemData.Add(data);
    }

    /// <summary>
    /// Returns a fresh InventoryItem prefab for a given type from its object pool.
    /// </summary>
    /// <param name="modelType">The type of Inventory model to get.</param>
    /// <returns>a GameObject with a InventoryItem component attached to it</returns>
    public static GameObject GetInventoryItemPrefab(ModelType modelType) => instance.RequestObject(modelType);

    /// <summary>
    /// Accepts a InventoryItem prefab that the caller no longer needs. Adds it back
    /// to its object pool.
    /// </summary>
    /// <param name="prefab">The InventoryItem prefab to return.</param>
    public static void ReturnInventoryItemPrefab(GameObject prefab) => instance.ReturnObject(prefab);

    /// <summary>
    /// Returns the InventoryFactory instance's Transform component.
    /// </summary>
    /// <returns></returns>
    protected override Transform GetTransform() { return instance.transform; }

    /// <summary>
    /// Returns the Sprite that represents the given ModelType on an inventory item.
    /// </summary>
    /// <param name="modelType">the given model type. </param>
    /// <returns>the Sprite that represents the given ModelType on an inventory item.</returns>
    public static Sprite GetInventoryItemIcon(ModelType modelType)
    {
        foreach (InventoryItemIcon icon in instance.icons) { if (icon.modelType == modelType) return icon.icon; }
        return null;
    }

    /// <summary>
    /// Returns the InventoryItemData object for a given InventoryItem type.
    /// </summary>
    /// <param name="inventoryItemType">the InventoryItem type.</param>
    /// <returns> the InventoryItemData object for a given InventoryItem type.</returns>
    public static InventoryItemData GetInventoryItemData(ModelType inventoryItemType)
    {
        Assert.IsTrue(ModelTypeHelper.IsInventoryItem(inventoryItemType), "ModelType is not an InventoryItem.");
        Assert.IsTrue(instance.inventoryItemData.Exists(t => t.InventoryItemType == inventoryItemType), "InventoryItemData does not exist.");
        return instance.inventoryItemData.Find(t => t.InventoryItemType == inventoryItemType);
    }

    #endregion
}
