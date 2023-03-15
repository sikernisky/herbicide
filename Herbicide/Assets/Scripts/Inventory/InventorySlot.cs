using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Represents a portion of the Inventory. Holds some
/// item and keeps track of its count.
/// </summary>
public class InventorySlot : MonoBehaviour
{
    /// <summary>
    /// The ISlottable that occupies this InventorySlot; null if this
    /// Inventory slot is empty.
    /// </summary>
    private ISlottable occupant;

    /// <summary>
    /// How many of occupant are in this InventorySlot
    /// </summary>
    private int count;

    /// <summary>
    /// The greatest number of items that can exist in this InventorySlot.
    /// </summary>
    private const int MAX_COUNT = 999;

    /// <summary>
    /// The number of items that an InventorySlot starts with. 
    /// </summary>
    private const int START_COUNT = 2;

    /// <summary>
    /// Color for the slot when unoccupied
    /// </summary>
    private readonly Color32 EMPTY_COLOR = new Color32(100, 100, 100, 255);


    /// <summary>
    /// Color for the slot when occupied
    /// </summary>
    private readonly Color32 FILLED_COLOR = new Color32(255, 255, 255, 255);

    /// <summary>
    /// Image component for the inner body of the InventorySlot
    /// </summary>
    [SerializeField]
    private Image slotImage;

    /// <summary>
    /// Image component for the slot background
    /// </summary>
    [SerializeField]
    private Image slotBackgroundImage;

    /// <summary>
    /// TMP_Text component to display the InventorySlot's count
    /// </summary>
    [SerializeField]
    private TMP_Text countText;


    /// <summary>
    /// Returns true if this InventorySlot is not empty.
    /// </summary>
    /// <returns>true if this InventorySlot is not empty; otherwise, false.
    /// </returns>
    public bool Occupied()
    {
        return occupant != null;
    }

    /// <summary>
    /// Returns true if this InventorySlot can be loaded with an ISlottable.
    /// If so, loads it.
    /// </summary>
    /// <param name="item"></param>
    /// <returns>true if this InventorySlot can be loaded with an ISlottable;
    /// otherwise, false.</returns>
    public bool Load(ISlottable item)
    {
        if (!CanLoad(item)) return false;

        occupant = item;
        FillSprite(occupant.GetInventorySprite());
        ResetCount();
        return true;
    }

    /// <summary>
    /// Removes the occupant from this InventorySlot if it has one.
    /// </summary>
    public void Wipe()
    {
        if (!Occupied()) return;

        occupant = null;
        slotImage.sprite = null;
        slotImage.enabled = false;
        countText.enabled = false;
        slotBackgroundImage.color = EMPTY_COLOR;
    }

    /// <summary>
    /// Called when this slot is clicked. "Uses" an item from its count.
    /// </summary>
    public void Use()
    {
        if (!CanUse()) return;

        DecrementCount();
    }

    /// <summary>
    /// Returns true if an item can be used from this InventorySlot.
    /// </summary>
    /// <returns>true if an item can be used from this slot</returns>
    public bool CanUse()
    {
        if (!Occupied()) return false;
        if (GetCount() == 0) return false;

        return true;
    }

    /// <summary>
    /// Sets the Sprite that goes inside of the InventorySlot's border. 
    /// </summary>
    /// <param name="s">The Sprite to fill with</param>
    private void FillSprite(Sprite s)
    {
        if (s == null) return;
        slotImage.enabled = true;
        slotImage.sprite = s;
        slotBackgroundImage.color = FILLED_COLOR;
    }

    /// <summary>
    /// Adds one to this InventorySlot's count. If the count is MAX_COUNT,
    /// does nothing.
    /// </summary>
    private void IncrementCount()
    {
        if (GetCount() + 1 <= MAX_COUNT) count++;
        countText.text = GetCount().ToString();
    }

    /// <summary>
    /// Removes one from this InventorySlot's count. If the count is zero,
    /// does nothing.
    /// </summary>
    private void DecrementCount()
    {
        if (GetCount() - 1 >= 0) count--;
        countText.text = GetCount().ToString();
    }

    /// <summary>
    /// Sets this InventorySlot's count to START_COUNT.
    /// </summary>
    private void ResetCount()
    {
        count = START_COUNT;
        countText.enabled = true;
        countText.text = GetCount().ToString();
    }

    /// <summary>
    /// Returns the count of this InventorySlot.
    /// </summary>
    /// <returns>the count of this InventorySlot.</returns>
    public int GetCount()
    {
        return count;
    }

    /// <summary>
    /// Returns true if this InventorySlot can load a given ISlottable.
    /// </summary>
    /// <param name="slottable">the ISlottable to check</param>
    /// <returns>true if this InventorySlot can load the ISlottable; otherwise,
    /// false.</returns>
    private bool CanLoad(ISlottable slottable)
    {
        if (slottable == null) return false;
        if (Occupied()) return false;
        return true;
    }

    /// <summary>
    /// Returns the ISlottable that occupies this InventorySlot.
    /// </summary>
    /// <returns>the ISlottable that occupies this InventorySlot.</returns>
    public ISlottable GetOccupant()
    {
        return occupant;
    }
}
