using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Represents a container for an item that can be stored in the inventory.
/// </summary>
public abstract class InventoryItem : UIModel, IUseable
{
    #region Fields

    /// <summary>
    /// The information about this InventoryItem.
    /// </summary>
    public InventoryItemData InventoryItemData { get; set; }

    /// <summary>
    /// The object that will be placed.
    /// </summary>
    public object PlacingObject => this;

    /// <summary>
    /// true if the InventoryItem is defined; otherwise, false.
    /// </summary>
    public bool IsDefined => InventoryItemData != null;

    /// <summary>
    /// The event that will be triggered when the InventoryItem is used.
    /// </summary>
    private UnityAction OnUseEvent { get; set;}

    /// <summary>
    /// The sprite of this InventoryItem.
    /// </summary>
    [SerializeField]
    private Image splashImage;

    /// <summary>
    /// The shadow of this InventoryItem.
    /// </summary>
    [SerializeField]
    private Image splashShadowImage;

    #endregion

    #region Methods

    /// <summary>
    /// Defines this InventoryItem with the given data.
    /// </summary>
    /// <param name="inventoryItemData">the data that will define this InventoryItem.</param>
    /// <param name="onUseEvent">the event that will be triggered when the InventoryItem is used.</param>
    public void DefineAndResetInventoryItem(InventoryItemData inventoryItemData, UnityAction onUseEvent)
    {
        Assert.IsNotNull(inventoryItemData, "InventoryItemData is null.");
        Assert.IsNotNull(onUseEvent, "OnUseEvent is null.");
        InventoryItemData = inventoryItemData;
        OnUseEvent = onUseEvent;
        SetInventoryItemSplash();
    }

    /// <summary>
    /// Sets the splash image of this ticket.
    /// </summary>
    private void SetInventoryItemSplash()
    {
        Sprite splashArt = InventoryFactory.GetInventoryItemIcon(InventoryItemData.InventoryItemType);
        splashImage.sprite = splashArt;
        splashShadowImage.sprite = splashArt;
    }

    /// <summary>
    /// Returns the placement size of this InventoryItem.
    /// </summary>
    /// <returns>the placement size of this InventoryItem.</returns>
    public Vector2Int GetPlacementSize() => InventoryItemData.PlacementSize;

    /// <summary>
    /// Returns the sprite animation track to be used during placement.
    /// By default, this is the sprite of the InventoryItem.
    /// </summary>
    /// <returns>the sprite animation track to be used during placement.</returns>
    public virtual Sprite[] GetPlacementTrack() => new Sprite[] { splashImage.sprite };

    /// <summary>
    /// Triggers an event when the InventoryItem is successfully placed.
    /// </summary>
    public void OnPlace(Vector3 worldPosition) => OnUseEvent.Invoke();

    #endregion
}
