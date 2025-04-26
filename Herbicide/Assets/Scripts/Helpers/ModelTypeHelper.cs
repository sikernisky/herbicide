using System;
using UnityEngine.Assertions;

/// <summary>
/// Helper class for the ModelType enum.
/// </summary>
public static class ModelTypeHelper {

    #region Methods

    /// <summary>
    /// Converts a string to the corresponding ModelType enum.
    /// </summary>
    /// <param name="modelTypeString">The string to convert.</param>
    public static ModelType ConvertStringToModelType(string modelTypeString)
    {
        if (Enum.TryParse(modelTypeString, true, out ModelType result)) return result;
        throw new ArgumentException($"Invalid ModelType: {modelTypeString}");
    }

    /// <summary>
    /// Returns the ShopCard ModelType based on the given ModelType.
    /// </summary>
    /// <param name="modelType">The ModelType to convert to a ShopCard ModelType.</param>
    /// <returns>the ShopCard ModelType based on the given ModelType.</returns>
    public static ModelType GetShopCardModelTypeFromModelType(ModelType modelType)
    {
        if (ModelTypeConstants.ShopCardTypes.Contains(modelType)) return modelType;
        Assert.IsTrue(ModelTypeConstants.ModelToShopCard.ContainsKey(modelType), "ModelType does not have a corresponding ShopCard ModelType.");
        return ModelTypeConstants.ModelToShopCard[modelType];
    }

    /// <summary>
    /// Returns the ModelType based on the given ShopCard ModelType.
    /// </summary>
    /// <param name="shopCardModelType">The ShopCard ModelType to convert to a ModelType.</param>
    /// <returns>the ModelType based on the given ShopCard ModelType.</returns>
    public static ModelType GetModelTypeFromShopCardModelType(ModelType shopCardModelType)
    {       
        Assert.IsTrue(ModelTypeConstants.ShopCardTypes.Contains(shopCardModelType), "ShopCard ModelType does not have a corresponding ModelType.");
        return ModelTypeConstants.ShopCardToModel[shopCardModelType];
    }

    /// <summary>
    /// Returns true if the given ModelType is a defender; false otherwise.
    /// </summary>
    /// <param name="modelType">The ModelType to check.</param>
    /// <returns>true if the given ModelType is a defender; false otherwise.</returns>
    public static bool IsDefender(ModelType modelType) => ModelTypeConstants.DefenderTypes.Contains(modelType);

    /// <summary>
    /// Returns true if the given ModelType is a ticket; false otherwise.
    /// </summary>
    /// <param name="modelType">the ModelType to check.</param>
    /// <returns>true if the given ModelType is a ticket; false otherwise.</returns>
    public static bool IsTicket(ModelType modelType) => modelType.ToString().ToLower().Contains("ticket");

    /// <summary>
    /// Returns true if the given ModelType is an inventory item; false otherwise.
    /// </summary>
    /// <param name="modelType">the ModelType to check.</param>
    /// <returns>true if the given ModelType is an inventory item; false otherwise.</returns>
    public static bool IsInventoryItem(ModelType modelType) => modelType.ToString().ToLower().Contains("inventory_item");

    /// <summary>
    /// Returns the ModelType of the inventory item corresponding to the given ticket ModelType.
    /// </summary>
    /// <param name="ticketModelType">the ticket ModelType to convert.</param>
    /// <returns>the ModelType of the inventory item corresponding to the given ticket ModelType.</returns>
    public static ModelType GetInventoryItemTypeFromTicketType(ModelType ticketModelType)
    {
        Assert.IsTrue(IsTicket(ticketModelType), "ModelType is not a ticket.");
        Assert.IsTrue(ModelTypeConstants.TicketToInventory.ContainsKey(ticketModelType), ticketModelType + " does not have a corresponding inventory item ModelType.");
        return ModelTypeConstants.TicketToInventory[ticketModelType];
    }

    /// <summary>
    /// Returns the ModelType of the inventory item container corresponding to the given inventory item ModelType.
    /// </summary>
    /// <param name="inventoryItemType">the inventory item type to convert.</param>
    /// <returns>the ModelType of the inventory item container corresponding to the given inventory item ModelType.</returns>
    public static ModelType GetInventoryItemContainerTypeFromInventoryType(ModelType inventoryItemType)
    {
        Assert.IsTrue(IsInventoryItem(inventoryItemType), "ModelType is not an inventory item.");
        Assert.IsTrue(ModelTypeConstants.InventoryItemToInventoryItemContainer.ContainsKey(inventoryItemType), inventoryItemType + " does not have a corresponding inventory item container ModelType.");
        return ModelTypeConstants.InventoryItemToInventoryItemContainer[inventoryItemType];
    }

    /// <summary>
    /// Returns the ModelType of the ticket corresponding to the given inventory item ModelType.
    /// </summary>
    /// <param name="inventoryModelType">the inventory item ModelType to convert.</param>
    /// <returns>the ModelType of the ticket corresponding to the given inventory item ModelType.</returns>
    public static ModelType GetTicketTypeFromInventoryItemType(ModelType inventoryModelType)
    {
        Assert.IsTrue(IsInventoryItem(inventoryModelType), "ModelType is not an inventory item.");
        Assert.IsTrue(ModelTypeConstants.InventoryToTicket.ContainsKey(inventoryModelType), "ModelType does not have a corresponding ticket ModelType.");
        return ModelTypeConstants.InventoryToTicket[inventoryModelType];
    }

    /// <summary>
    /// Returns true if the given ModelType is an equipment; false otherwise.
    /// </summary>
    /// <param name="modelType">The ModelType to check.</param>
    /// <returns>true if the given ModelType is an equipment; false otherwise.</returns>
    public static bool IsEquipment(ModelType modelType) => ModelTypeConstants.EquipmentTypes.Contains(modelType);

    /// <summary>
    /// Returns true if the given ModelType is a projectile; false otherwise.
    /// </summary>
    /// <param name="modelType">the ModelType to check.</param>
    /// <returns>true if the given ModelType is a projectile; false otherwise.</returns>
    public static bool IsProjectile(ModelType modelType) => ModelTypeConstants.ProjectileTypes.Contains(modelType);

    #endregion
}
