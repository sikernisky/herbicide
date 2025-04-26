using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// Holds a ShopCard in the Shop.
/// </summary>
public class ShopSlot : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// The ShopCard in this slot; null if this slot is empty.
    /// </summary>
    public ShopCard Occupant { get; private set; }

    /// <summary>
    /// true if this ShopSlot is filled with a ShopCard; otherwise, false.
    /// </summary>
    public bool Filled => Occupant != null;

    /// <summary>
    /// true if this ShopSlot is defined; otherwise, false.
    /// </summary>
    public bool Defined => Filled && Occupant.IsDefined;

    /// <summary>
    /// The index of this ShopSlot -- each slot has its
    /// own index.
    /// </summary>
    public int SlotIndex { get; private set; }

    /// <summary>
    /// The button on this ShopSlot.
    /// </summary>
    [SerializeField]
    private Button button;

    #endregion

    #region Methods

    /// <summary>
    /// Fills the ShopSlot with a ShopCard, index, and assigns an onClick event.
    /// </summary>
    /// <param name="slotIndex">The index of this ShopSlot.</param>
    /// <param name="shopCard">The ShopCard to display.</param>
    /// <param name="onClickEvent">The event to trigger when the ShopCard is clicked.</param>
    public void Setup(int slotIndex, ShopCard shopCard, UnityAction<int> onClickEvent)
    {
        Assert.IsNotNull(shopCard);
        Assert.IsNotNull(onClickEvent);
        Assert.IsFalse(Filled, "Already filled with a ShopCard prefab.");
        
        SlotIndex = slotIndex;
        Occupant = shopCard;
        PositionShopCardInSlot(Occupant);
        ConfigureSlotButton(Occupant);
        button.onClick.AddListener(() => onClickEvent(slotIndex));
    }

    /// <summary>
    /// Positions the ShopCard in the slot.
    /// </summary>
    /// <param name="shopCard">the ShopCard prefab to position.</param>
    private void PositionShopCardInSlot(ShopCard shopCard)
    {
        Assert.IsNotNull(shopCard);
        RectTransform cardTransform = Occupant.gameObject.GetComponent<RectTransform>();
        cardTransform.SetParent(transform);
        cardTransform.localScale = Vector3.one;
        cardTransform.localPosition = Vector3.zero;
    }

    /// <summary>
    /// Defines the filled slot with new ShopCardData, potentially
    /// changing its appearance and functionality.
    /// </summary>
    /// <param name="shopCardData">the ShopCardData to define with. </param>
    public void DefineCard(ShopCardData shopCardData)
    {
        Assert.IsNotNull(shopCardData);
        Assert.IsTrue(Filled, "Need to fill the slot with a ShopCard prefab first.");

        Occupant.gameObject.SetActive(true);
        Occupant.DefineShopCard(shopCardData);
        ConfigureSlotButton(Occupant);
    }

    /// <summary>
    /// Configures the button on this ShopSlot to match the ShopCard that
    /// occupies it.
    /// </summary>
    /// <param name="shopCard">the ShopCard that occupies this slot.</param>
    private void ConfigureSlotButton(ShopCard shopCard)
    {
        Assert.IsNotNull(shopCard);
        ColorBlock buttonColors = button.colors;
        buttonColors.normalColor = new Color32(255, 255, 255, 255);
        buttonColors.highlightedColor = new Color32(200, 200, 200, 255);
        button.colors = buttonColors;
        button.targetGraphic = shopCard.GetCardBackgroundImage();
    }  

    /// <summary>
    /// Returns true if the player meets all conditions to buy the ShopCard that
    /// fills this ShopSlot.
    /// </summary>
    /// <param name="currentBalance">How much currency the player has.</param>
    /// <returns></returns>
    public bool CanBuy(int currentBalance) => Occupant.ShopCardData.CardCost <= currentBalance;

    /// <summary>
    /// Sets the ShopCard in this ShopSlot to be bought.
    /// </summary>
    public void OnBuy() => Occupant.gameObject.SetActive(false);

    /// <summary>
    /// Sets the color of the ShopCard occupant's background and title.
    /// </summary>
    /// <param name="color">The color to set the ShopCard to.</param>
    public void SetOccupantCardColor(Color32 color)
    {
        Assert.IsTrue(Filled);
        Occupant.SetCardColor(color);
    }

    #endregion
}
