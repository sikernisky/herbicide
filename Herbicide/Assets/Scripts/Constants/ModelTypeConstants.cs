using System.Collections.Generic;
/// <summary>
/// Constants for model types.
/// </summary>
public static class ModelTypeConstants
{
    /// <summary>
    /// Dictionary of ticket ModelTypes to their corresponding inventory item ModelTypes.
    /// </summary>
    public static readonly Dictionary<ModelType, ModelType> TicketToInventory = new Dictionary<ModelType, ModelType>()
    {
        { ModelType.TICKET_ACORNOL, ModelType.INVENTORY_ITEM_ACORNOL },
        { ModelType.TICKET_BUNADRYL, ModelType.INVENTORY_ITEM_BUNADRYL },
        { ModelType.TICKET_HAREOIN, ModelType.INVENTORY_ITEM_HAREOIN },
        { ModelType.TICKET_CLAWCAINE, ModelType.INVENTORY_ITEM_CLAWCAINE },
        { ModelType.TICKET_AMFURRTAMINE, ModelType.INVENTORY_ITEM_AMFURRTAMINE },
        { ModelType.TICKET_PILL, ModelType.INVENTORY_ITEM_PILL },
        { ModelType.TICKET_MASK, ModelType.INVENTORY_ITEM_MASK }
    };

    /// <summary>
    /// Dictionary of inventory item ModelTypes to their corresponding ticket ModelTypes.
    /// </summary>
    public static readonly Dictionary<ModelType, ModelType> InventoryToTicket = new Dictionary<ModelType, ModelType>()
    {
        { ModelType.INVENTORY_ITEM_ACORNOL, ModelType.TICKET_ACORNOL },
        { ModelType.INVENTORY_ITEM_BUNADRYL, ModelType.TICKET_BUNADRYL },
        { ModelType.INVENTORY_ITEM_HAREOIN, ModelType.TICKET_HAREOIN },
        { ModelType.INVENTORY_ITEM_CLAWCAINE, ModelType.TICKET_CLAWCAINE },
        { ModelType.INVENTORY_ITEM_AMFURRTAMINE, ModelType.TICKET_AMFURRTAMINE },
        { ModelType.INVENTORY_ITEM_PILL, ModelType.TICKET_PILL },
        { ModelType.INVENTORY_ITEM_MASK, ModelType.TICKET_MASK }
    };

    /// <summary>
    /// Dictionary of inventory item ModelTypes to their corresponding inventory item container ModelTypes.
    /// </summary>
    public static readonly Dictionary<ModelType, ModelType> InventoryItemToInventoryItemContainer = new Dictionary<ModelType, ModelType>()
    {
        { ModelType.INVENTORY_ITEM_ACORNOL, ModelType.INVENTORY_ITEM_EQUIPABLE },
        { ModelType.INVENTORY_ITEM_BUNADRYL, ModelType.INVENTORY_ITEM_EQUIPABLE },
        { ModelType.INVENTORY_ITEM_HAREOIN, ModelType.INVENTORY_ITEM_USEABLE },
        { ModelType.INVENTORY_ITEM_CLAWCAINE, ModelType.INVENTORY_ITEM_USEABLE },
        { ModelType.INVENTORY_ITEM_AMFURRTAMINE, ModelType.INVENTORY_ITEM_USEABLE },
        { ModelType.INVENTORY_ITEM_PILL, ModelType.INVENTORY_ITEM_USEABLE },
        { ModelType.INVENTORY_ITEM_MASK, ModelType.INVENTORY_ITEM_USEABLE }
    };

    /// <summary>
    /// Dictionary of ModelTypes to their corresponding ShopCard ModelTypes.
    /// </summary>
    public static readonly Dictionary<ModelType, ModelType> ModelToShopCard = new Dictionary<ModelType, ModelType>()
    {
        { ModelType.SQUIRREL, ModelType.SHOP_CARD_SQUIRREL },
        { ModelType.BEAR, ModelType.SHOP_CARD_BEAR },
        { ModelType.BUNNY, ModelType.SHOP_CARD_BUNNY },
        { ModelType.PORCUPINE, ModelType.SHOP_CARD_PORCUPINE },
        { ModelType.RACCOON, ModelType.SHOP_CARD_RACCOON },
        { ModelType.OWL, ModelType.SHOP_CARD_OWL }
    };

    /// <summary>
    /// Dictionary of ShopCard ModelTypes to their corresponding ModelTypes.
    /// </summary>
    public static readonly Dictionary<ModelType, ModelType> ShopCardToModel = new Dictionary<ModelType, ModelType>()
    {
        { ModelType.SHOP_CARD_SQUIRREL, ModelType.SQUIRREL },
        { ModelType.SHOP_CARD_BEAR, ModelType.BEAR },
        { ModelType.SHOP_CARD_BUNNY, ModelType.BUNNY },
        { ModelType.SHOP_CARD_PORCUPINE, ModelType.PORCUPINE },
        { ModelType.SHOP_CARD_RACCOON, ModelType.RACCOON },
        { ModelType.SHOP_CARD_OWL, ModelType.OWL }
    };

    /// <summary>
    /// Set of ShopCard ModelTypes.
    /// </summary>
    public static readonly HashSet<ModelType> ShopCardTypes = new HashSet<ModelType>
    {
        ModelType.SHOP_CARD_SQUIRREL,
        ModelType.SHOP_CARD_BEAR,
        ModelType.SHOP_CARD_BUNNY,
        ModelType.SHOP_CARD_PORCUPINE,
        ModelType.SHOP_CARD_RACCOON,
        ModelType.SHOP_CARD_OWL,
        ModelType.SHOP_CARD_BLANK
    };

    /// <summary>
    /// Set of defender ModelTypes.
    /// </summary>
    public static readonly HashSet<ModelType> DefenderTypes = new HashSet<ModelType>
    {
        ModelType.SQUIRREL,
        ModelType.BEAR,
        ModelType.BUNNY,
        ModelType.PORCUPINE,
        ModelType.RACCOON,
        ModelType.OWL
    };

    /// <summary>
    /// Set of equipment ModelTypes.
    /// </summary>
    public static readonly HashSet<ModelType> EquipmentTypes = new HashSet<ModelType>
    {
        ModelType.INVENTORY_ITEM_ACORNOL,
        ModelType.INVENTORY_ITEM_BUNADRYL
    };

    /// <summary>
    /// Set of projectile ModelTypes.
    /// </summary>
    public static readonly HashSet<ModelType> ProjectileTypes = new HashSet<ModelType>
    {
        ModelType.ACORN
    };

    /// <summary>
    /// Allowed equipment for defenders.
    /// </summary>
    public static readonly HashSet<ModelType> AllowedEquipmentForDefenders = new HashSet<ModelType>
    {
        ModelType.INVENTORY_ITEM_ACORNOL,
        ModelType.INVENTORY_ITEM_BUNADRYL
    };
}