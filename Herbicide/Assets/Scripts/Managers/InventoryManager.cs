using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

/// <summary>
/// Manages the inventory of completed tickets.
/// </summary>
public class InventoryManager : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// Reference to the InventoryManager singleton.
    /// </summary>
    private static InventoryManager instance;

    /// <summary>
    /// The inventory slots that hold the inventory items.
    /// </summary>
    [SerializeField]
    private List<InventorySlot> inventorySlots;

    /// <summary>
    /// The dictionary of events that are triggered when an InventoryItem is triggered.
    /// </summary>
    private Dictionary<ModelType, UnityAction> inventoryItemEvents;

    #endregion

    #region Methods

    /// <summary>
    /// Finds and sets the InventoryManager singleton.
    /// </summary>
    /// <param name="levelController">the LevelController singleton.</param>
    public static void SetSingleton(LevelController levelController)
    {
        if (levelController == null) return;

        InventoryManager[] inventoryManagers = FindObjectsOfType<InventoryManager>();
        Assert.IsNotNull(inventoryManagers, "Array of InventoryManagers is null.");
        Assert.AreEqual(1, inventoryManagers.Length);
        instance = inventoryManagers[0];
        SetupInventory();    
    }

    /// <summary>
    /// Takes a completed ticket and adds it to the inventory. 
    /// </summary>
    /// <param name="completedTicket">the ticket that was just completed. </param>
    public static void CompleteTicket(Ticket completedTicket)
    {
        Assert.IsNotNull(completedTicket, "Completed ticket is null.");
        ModelType completedTicketType = completedTicket.TicketData.TicketType;
        InventoryItem inventoryItem = instance.SpawnAndDefineInventoryItemFromTicketType(completedTicketType);
        instance.FillOpenSlotWithInventoryItem(inventoryItem);
    }

    /// <summary>
    /// Sets up the Inventory and its InventorySlots.
    /// </summary>
    private static void SetupInventory()
    {
        instance.ConstructItemEventDictionary();
        instance.InitializeSlots();
    }

    /// <summary>
    /// Sets up the dictionary of InventoryItem events that are triggered when a ticket is completed.
    /// </summary>
    private void ConstructItemEventDictionary()
    {
        inventoryItemEvents = new Dictionary<ModelType, UnityAction>()
        {
            { ModelType.INVENTORY_ITEM_ACORNOL, instance.CompleteAcornolTicket },
            { ModelType.INVENTORY_ITEM_CLAWCAINE, instance.CompleteClawcaineTicket },
            { ModelType.INVENTORY_ITEM_HAREOIN, instance.CompleteHareoinTicket },
            { ModelType.INVENTORY_ITEM_BUNADRYL, instance.CompleteBunadrylTicket }
        };
    }

    /// <summary>
    /// Initializes the inventory slots. Adds listeners to each slot.
    /// </summary>
    private void InitializeSlots() => inventorySlots.ForEach(slot => slot.InitializeSlotInInventory(HandleInventorySlotClick));

    /// <summary>
    /// Loops through the inventory slots and fills the first open slot with
    /// the given inventory item.
    /// </summary>
    /// <param name="itemToSlot">the inventory item to slot.</param>
    private void FillOpenSlotWithInventoryItem(InventoryItem itemToSlot)
    {
        Assert.IsNotNull(itemToSlot, "Inventory item is null.");
        foreach (InventorySlot slot in inventorySlots)
        {
            if (slot.IsEmpty())
            {
                slot.Fill(itemToSlot);
                return;
            }
        }
    }

    /// <summary>
    /// Returns an InventoryItem prefab for a given ticket type.
    /// </summary>
    /// <param name="ticketType">the given Ticket type. </param>
    /// <returns>an InventoryItem prefab for a given ticket type.</returns>
    private InventoryItem SpawnAndDefineInventoryItemFromTicketType(ModelType ticketType)
    {
        Assert.IsTrue(ModelTypeHelper.IsTicket(ticketType));
        ModelType inventoryItemType = ModelTypeHelper.GetInventoryItemTypeFromTicketType(ticketType);
        ModelType inventoryItemContainerType = ModelTypeHelper.GetInventoryItemContainerTypeFromInventoryType(inventoryItemType);
        GameObject inventoryItem = InventoryFactory.GetInventoryItemPrefab(inventoryItemContainerType);
        Assert.IsNotNull(inventoryItem, "Inventory item is null.");
        InventoryItem inventoryItemComponent = inventoryItem.GetComponent<InventoryItem>();
        Assert.IsNotNull(inventoryItemComponent, "Inventory item component is null.");
        inventoryItemComponent.DefineAndResetInventoryItem(InventoryFactory.GetInventoryItemData(inventoryItemType), inventoryItemEvents[inventoryItemType]);
        return inventoryItemComponent;
    }
    
    /// <summary>
    /// Handles the click event of an inventory slot.
    /// </summary>
    /// <param name="clickedSlot">the clicked slot.</param>
    private void HandleInventorySlotClick(InventorySlot clickedSlot)
    {
        Assert.IsNotNull(clickedSlot, "Clicked slot is null.");
        if(PlacementManager.IsPlacing) return;
        if(clickedSlot.IsEmpty()) return;
        ModelType ticketType = ModelTypeHelper.GetTicketTypeFromInventoryItemType(clickedSlot.GetOccupantModelType());
        TicketManager.OnPurchaseOrUseModel(ticketType);
        SubscribeToPlacementEvent(clickedSlot);
        clickedSlot.StartPlacementEvent();
        clickedSlot.EmptyForPlacement();
    }

    /// <summary>
    /// Subscribes to the PlacementManager's OnPlacementFinished event.
    /// </summary>
    /// <param name="clickedSlot">the slot that was clicked.</param>
    private void SubscribeToPlacementEvent(InventorySlot clickedSlot)
    {
        Assert.IsNotNull(clickedSlot, "Clicked slot is null.");
        PlacementManager.OnPlacementFinished += HandlePlacementResult;
        void HandlePlacementResult(bool success)
        {
            if (!success) clickedSlot.RestoreOccupant(); // Restore the item if placement is canceled
            else clickedSlot.Empty(); // Remove item permanently
            PlacementManager.OnPlacementFinished -= HandlePlacementResult;
        }
    }

    #endregion

    #region Inventory Events

    /// <summary>
    /// Called when a Bunadryl inventory item is triggered.
    /// </summary>
    private void CompleteBunadrylTicket() { }

    /// <summary>
    /// Called when a Acornol inventory item item is triggered.
    /// </summary>
    private void CompleteAcornolTicket() { }

    /// <summary>
    /// Called when a Clawcaine inventory item item is triggered.
    /// </summary> 
    private void CompleteClawcaineTicket() { }

    /// <summary>
    /// Called when a Hareoin inventory item is triggered.
    /// </summary>
    private void CompleteHareoinTicket() { }

    #endregion
}
