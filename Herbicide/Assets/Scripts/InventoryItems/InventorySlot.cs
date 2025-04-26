using UnityEngine.Assertions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// Represents a slot in the inventory that holds an inventory item.
/// </summary>
public class InventorySlot : UIModel
{
    #region Fields

    /// <summary>
    /// Returns the type of model.
    /// </summary>
    /// <returns>the type of model.</returns>
    public override ModelType GetModelType() => ModelType.INVENTORY_SLOT;

    /// <summary>
    /// The inventory item that occupies the slot; null if the slot is empty.
    /// </summary>
    private InventoryItem Occupant { get; set; }

    /// <summary>
    /// The button that represents the slot.
    /// </summary>
    [SerializeField]
    private Button button;

    /// <summary>
    /// Event that is invoked when the slot is clicked.
    /// </summary>
    private UnityEvent<InventorySlot> onSlotClicked;

    #endregion

    #region Methods

    /// <summary>
    /// Initializes the slot in the inventory. Adds a listener to the button.
    /// </summary>
    /// <param name="action">the action to perform when the slot is clicked.</param>
    public void InitializeSlotInInventory(UnityAction<InventorySlot> action)
    {
        Assert.IsNotNull(button, "Button is null.");
        Assert.IsNotNull(action, "Action is null.");
        if(onSlotClicked == null) onSlotClicked = new UnityEvent<InventorySlot>();
        onSlotClicked.RemoveAllListeners();
        onSlotClicked.AddListener(action);
        button.onClick.AddListener(() => onSlotClicked.Invoke(this));
    }

    /// <summary>
    /// Fills the slot with an inventory item.
    /// </summary>
    /// <param name="item">the inventory item to fill with.</param>
    public void Fill(InventoryItem item)
    {
        Assert.IsNotNull(item, "Inventory item is null.");
        Assert.IsTrue(IsEmpty(), "Inventory slot is already occupied.");
        Assert.IsTrue(item.IsDefined, "Item is not defined.");
        Occupant = item;
        PositionOccupantWithinSlot();
    }

    /// <summary>
    /// Positions the occupant within the slot.
    /// </summary>
    private void PositionOccupantWithinSlot()
    {
        Assert.IsFalse(IsEmpty(), "Inventory slot is empty.");
        Occupant.transform.SetParent(transform);
        Occupant.transform.localPosition = Vector3.zero;
        Occupant.transform.localScale = Vector3.one;
    }

    /// <summary>
    /// Empties the slot, returning the occupant to the inventory factory.
    /// </summary>
    public void Empty()
    {
        Assert.IsFalse(IsEmpty(), "Inventory slot is already empty.");
        InventoryFactory.ReturnInventoryItemPrefab(Occupant.gameObject);
        Occupant = null;
    }

    /// <summary>
    /// Empties the slot for placement by temporarily hiding the occupant.
    /// </summary>
    public void EmptyForPlacement()
    {
        Assert.IsFalse(IsEmpty(), "Inventory slot is already empty.");
        Occupant.gameObject.SetActive(false);
    }

    /// <summary>
    /// Restores the occupant to the slot by setting it to active.
    /// </summary>
    public void RestoreOccupant()
    {
        Assert.IsFalse(IsEmpty(), "Inventory slot is empty.");
        Occupant.gameObject.SetActive(true);
    }

    /// <summary>
    /// Returns true if the slot is empty; false otherwise.
    /// </summary>
    /// <returns>true if the slot is empty; false otherwise.</returns>
    public bool IsEmpty() => Occupant == null;

    /// <summary>
    /// Returns the model type of the occupant. Must be filled
    /// to return a valid model type.
    /// </summary>
    /// <returns>the model type of the occupant</returns>
    public ModelType GetOccupantModelType()
    {
        Assert.IsFalse(IsEmpty(), "Inventory slot is empty.");
        return Occupant.InventoryItemData.InventoryItemType;
    }

    /// <summary>
    /// Starts a placement event for the occupant.
    /// </summary>
    public void StartPlacementEvent() => PlacementManager.StartPlacing(Occupant);

    #endregion
}
