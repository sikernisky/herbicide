using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Stores data related to an inventory item.
/// </summary>
public class InventoryItemData
{
    /// <summary>
    /// The type of InventoryItem.
    /// </summary>
    public ModelType InventoryItemType { get; }

    /// <summary>
    /// The name of the InventoryItem.
    /// </summary>
    public string InventoryItemName { get; }

    /// <summary>
    /// The size of the InventoryItem when placing.
    /// </summary>
    public Vector2Int PlacementSize { get; }

    /// <summary>
    /// Creates a new instance of InventoryItemData.
    /// </summary>
    /// <param name="type">The type of inventory item.</param>
    /// <param name="name">The name of the inventory item.</param>
    /// <param name="size">The size of the inventory item when placing.</param>
    public InventoryItemData(ModelType type, string name, Vector2Int size)
    {
        AssertValidInventoryItemInformation(type, name, size);
        InventoryItemType = type;
        InventoryItemName = name;
        PlacementSize = size;
    }

    /// <summary>
    /// Creates a new instance of InventoryItemData.
    /// </summary>
    /// <param name="type">The type of inventory item.</param>
    /// <param name="name">The name of the inventory item.</param>
    public InventoryItemData(ModelType type, string name)
    {
        AssertValidInventoryItemInformation(type, name);
        InventoryItemType = type;
        InventoryItemName = name;
        PlacementSize = new Vector2Int(16, 16);
    }

    /// <summary>
    /// Asserts that the given inventory item information is valid.
    /// </summary>
    /// <param name="type">The ModelType of this InventoryItem.</param>
    /// <param name="name">The name of this InventoryItem.</param>
    private void AssertValidInventoryItemInformation(ModelType type, string name)
    {
        Assert.IsTrue(ModelTypeHelper.IsInventoryItem(type), "ModelType is not a InventoryItem.");
        Assert.IsTrue(name != null && name.Length > 0, "Name is null or empty.");
    }

    /// <summary>
    /// Asserts that the given inventory item information is valid.
    /// </summary>
    /// <param name="type">The ModelType of this InventoryItem.</param>
    /// <param name="name">The name of this InventoryItem.</param>
    /// <param name="size">The size of this InventoryItem.</param>
    private void AssertValidInventoryItemInformation(ModelType type, string name, Vector2Int size)
    {
        Assert.IsTrue(ModelTypeHelper.IsInventoryItem(type), "ModelType is not a InventoryItem.");
        Assert.IsTrue(name != null && name.Length > 0, "Name is null or empty.");
        Assert.IsTrue(size.x > 0 && size.y > 0, "Size is invalid.");
    }
}
