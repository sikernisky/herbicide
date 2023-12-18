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
    /// All InventorySlots in the Inventory.
    /// </summary>
    [SerializeField]
    private InventorySlot[] slots;

    /// <summary>
    /// Current state of the game.
    /// </summary>
    private GameState gameState;


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
    /// Updates all InventorySlots controlled by this InventoryController.
    /// </summary>
    /// <param name="playerCurrency">How much currency the player has this frame.
    /// </param>
    public static void UpdateSlots(int playerCurrency)
    {
        if (instance.slots == null) return;
        foreach (InventorySlot slot in instance.slots)
        {
            slot.UpdateSlot(playerCurrency);
        }
    }

    /// <summary>
    /// Given an index, defines what goes in the InventorySlot at that
    /// index. If that InventorySlot is already loaded, wipes it first.
    /// </summary>
    /// <param name="index">the index of the InventorySlot to load</param>
    /// <param name="item">the item to load into the InventorySlot</param>
    private void LoadSlot(int index, Model item)
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
    /// This method takes in an array of Models. One by one, in order,
    /// the InventoryController will load its InventorySlots with each
    /// Model in the array. This means the number of items passed into
    /// this method may not exceed the number of InventorySlots managed
    /// by the InventoryController.
    /// </summary>
    /// <param name="items">The items to load, in order and one by one,
    /// into the Inventory.</param>
    public static void LoadEntireInventory(Model[] items)
    {
        //Safety checks.
        if (items == null) return;
        if (items.Length > instance.slots.Length) return;
        foreach (Model item in items)
        {
            if (item == null) return;
        }
        foreach (InventorySlot slot in instance.slots)
        {
            if (slot == null) return;
        }

        //Load the slots.
        int counter = 0;
        foreach (Model item in items)
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
    /// [!!BUTTON EVENT!!]
    /// 
    /// Triggers the mouse up event for an InventorySlot.
    /// </summary>
    /// <param name="slotIndex">the index of the slot that was clicked</param>
    public void SlotMouseUp(int slotIndex)
    {
        //Buttons don't know about the game state. We add manual protection.
        if (gameState != GameState.ONGOING) return;
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
        if (!slot.Occupied()) return;
        if (!slot.CanUse()) return;

        //Start the placement event
        Model occupant = slot.GetOccupant();
        if (occupant == null) return;
        PlacementController.StartPlacingObject(slot);
    }


    /// <summary>
    /// Informs the InventoryController of the current GameState.
    /// </summary>
    /// <param name="state">The current GameState</param>
    public static void InformOfGameState(GameState state)
    {
        instance.gameState = state;
    }
}
