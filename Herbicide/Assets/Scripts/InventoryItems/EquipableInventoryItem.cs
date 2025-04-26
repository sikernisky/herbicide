/// <summary>
/// Represents an inventory item that can be equipped on a surface.
/// </summary>
public class EquipableInventoryItem : InventoryItem, IEquipable
{
    #region Fields

    /// <summary>
    /// The type of equipment this is.
    /// </summary>
    public ModelType EquipmentType => InventoryItemData.InventoryItemType;

    #endregion

    #region Methods

    /// <summary>
    /// Returns the type of model this is.
    /// </summary>
    /// <returns>the type of model this is.</returns>
    public override ModelType GetModelType() => ModelType.INVENTORY_ITEM_EQUIPABLE;

    #endregion
}
