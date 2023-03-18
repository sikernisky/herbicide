using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Controls the main Inventory and its InventorySlots. Responsible
/// for loading, clearing, and updating all InventorySlots. 
/// </summary>
public class InventoryController : MonoBehaviour
{
    /// <summary>
    /// Reference to the InventoryController singleton.
    /// </summary>
    private static InventoryController instance;

    /// <summary>
    /// The InventorySlot currently being placed from; null if
    /// no slot is being placed from.
    /// </summary>
    private InventorySlot placingSlot;

    /// <summary>
    /// All InventorySlots in the Inventory.
    /// </summary>
    [SerializeField]
    private InventorySlot[] slots;


    /// <summary>
    /// Finds and sets the InventoryController singleton.
    ///  </summary>
    /// <param name="levelController">The LevelController singleton.</param>
    public static void SetSingleton(LevelController levelController)
    {
        if (levelController == null) return;
        if (instance != null) return;

        InventoryController[] inventoryControllers = FindObjectsOfType<InventoryController>();
        Assert.IsNotNull(inventoryControllers, "Array of InventoryControllers is null.");
        Assert.AreEqual(1, inventoryControllers.Length);
        instance = inventoryControllers[0];
    }

    /// <summary>
    /// Given an index, defines what goes in the InventorySlot at that
    /// index. If that InventorySlot is already loaded, wipes it first.
    /// </summary>
    /// <param name="index">the index of the InventorySlot to load</param>
    /// <param name="item">the item to load into the InventorySlot</param>
    private void LoadSlot(int index, ISlottable item)
    {
        //Safety checks
        if (index < 0 || index >= instance.slots.Length) return;
        if (item == null) return;
        InventorySlot slot = instance.slots[index];
        if (slot == null) return;

        if (slot.Occupied()) slot.Wipe();
        slot.Load(item);
    }

    /// <summary>
    /// Wipes an InventorySlot at a given index.
    /// </summary>
    /// <param name="index">the index of the InventorySlot to wipe</param>
    private void WipeSlot(int index)
    {
        if (index < 0 || index >= instance.slots.Length) return;
        InventorySlot slot = instance.slots[index];
        if (slot == null) return;

        if (slot.Occupied()) slot.Wipe();
    }


    /// <summary>
    /// Loads the entire Inventory.<br><br>
    /// 
    /// This method takes in an array of ISlottables. One by one, in order,
    /// the InventoryController will load its InventorySlots with each
    /// ISlottable in the array. This means the number of items passed into
    /// this method may not exceed the number of InventorySlots managed
    /// by the InventoryController.
    /// </summary>
    /// <param name="items">The items to load, in order and one by one,
    /// into the Inventory.</param>
    public static void LoadEntireInventory(ISlottable[] items)
    {
        //Safety checks.
        if (items == null) return;
        if (items.Length > instance.slots.Length) return;
        foreach (ISlottable item in items)
        {
            if (item == null) return;
        }
        foreach (InventorySlot slot in instance.slots)
        {
            if (slot == null) return;
        }

        //Load the slots.
        int counter = 0;
        foreach (ISlottable item in items)
        {
            instance.LoadSlot(counter, items[counter]);
            counter++;
        }
    }

    /// <summary>
    /// Shows the Inventory if hidden.
    /// </summary>
    public static void ShowInventory()
    {
        //TODO: Implement this in the future
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Hides the Inventory if showing.
    /// </summary>
    public static void HideInventory()
    {
        //TODO: Implement this in the future
        throw new System.NotImplementedException();
    }


    /// <summary>
    /// Triggers the mouse up event for an InventorySlot.
    /// </summary>
    /// <param name="slotIndex">the index of the slot that was clicked</param>
    public void SlotMouseUp(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= slots.Length) return;

        InventorySlot slot = slots[slotIndex];
        if (slot == null) return;

        StartPlacingFromSlot(slot);
    }

    /// <summary>
    /// Starts a placement event (through PlacementController) with the ISlottable
    /// in a given InventorySlot. Updates the InventoryController with that InventorySlot
    /// so it knows which slot to stop placing from.
    /// </summary>
    /// <param name="slot">The InventorySlot to start placing from</param>
    private void StartPlacingFromSlot(InventorySlot slot)
    {
        //Safety checks
        if (PlacementController.Placing()) return;
        if (slot == null) return;
        if (placingSlot != null) return;
        if (!slot.Occupied()) return;
        if (!slot.CanUse()) return;

        //Start the placement event
        ISlottable occupant = slot.GetOccupant();
        if (occupant == null) return;
        placingSlot = slot;
        PlacementController.StartPlacingObject(occupant);

    }

    /// <summary>
    /// Checks for and executes any input events that affect InventoryController
    /// logic.
    /// </summary>
    /// <param name="levelController">reference to the LevelController singleton</param>
    /// <param name="didEscape">if the player pressed escape</param>
    public static void CheckInventoryInputEvents(LevelController levelController, bool didEscape)
    {
        //Safety check
        if (levelController == null) return;

        if (didEscape)
        {
            if (PlacementController.Placing())
            {
                StopPlacingFromSlot(false);
            }
        }
    }

    /// <summary>
    /// Notifies the InventoryController that an ISlottable from one of its slots
    /// was successfully placed. The InventoryController stops the placement
    /// event and dereferences its placing InventorySlot. 
    ///  </summary>
    /// <param name="successfulPlace">true if the item was successfully placed onto
    /// the grid; otherwise, false.</param>
    public static void StopPlacingFromSlot(bool successfulPlace = true)
    {
        //Safety checks
        if (!PlacementController.Placing()) return;
        if (instance.placingSlot == null) return;
        if (!instance.placingSlot.CanUse()) return;

        if (successfulPlace) instance.placingSlot.Use();
        instance.placingSlot = null;
        PlacementController.StopPlacingObject();
    }
}
