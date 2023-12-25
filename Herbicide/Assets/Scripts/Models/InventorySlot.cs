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
    /// The Model that occupies this InventorySlot; null if this
    /// Inventory slot is empty.
    /// </summary>
    private Model occupant;


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
    /// This InventorySlot's Button component.
    /// </summary>
    [SerializeField]
    private Button slotButton;

    /// <summary>
    /// The player's current currency balance.
    /// </summary>
    private int currentBalance;


    /// <summary>
    /// Updates this InventorySlot with recent information.
    /// </summary>
    /// <param name="playerCurrency">How much currency the player has this frame.
    /// </param>
    public void UpdateSlot(int playerCurrency)
    {
        currentBalance = playerCurrency;
    }

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
    /// Returns true if this InventorySlot can be loaded with an Model.
    /// If so, loads it.
    /// </summary>
    /// <param name="item"></param>
    /// <returns>true if this InventorySlot can be loaded with an Model;
    /// otherwise, false.</returns>
    public bool Load(Model item)
    {
        if (!CanLoad(item)) return false;

        occupant = item;
        FillSprite(occupant.GetBoatSprite());
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
        slotBackgroundImage.color = EMPTY_COLOR;
        slotButton.interactable = false;
    }

    /// <summary>
    /// Called when this slot is clicked. "Uses" an item from its count.
    /// </summary>
    public void Use()
    {
        if (!CanUse()) return;
    }

    /// <summary>
    /// Returns true if an item can be used from this InventorySlot.
    /// </summary>
    /// <returns>true if an item can be used from this slot</returns>
    public bool CanUse()
    {
        if (!Occupied()) return false;
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
        slotButton.interactable = true;
    }

    /// <summary>
    /// Returns true if this InventorySlot can load a given Model.
    /// </summary>
    /// <param name="slottable">the Model to check</param>
    /// <returns>true if this InventorySlot can load the Model; otherwise,
    /// false.</returns>
    private bool CanLoad(Model slottable)
    {
        if (slottable == null) return false;
        if (Occupied()) return false;
        return true;
    }

    /// <summary>
    /// Returns the Model that occupies this InventorySlot.
    /// </summary>
    /// <returns>the Model that occupies this InventorySlot.</returns>
    public Model GetOccupant()
    {
        return occupant;
    }
}
