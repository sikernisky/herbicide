using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

/// <summary>
/// Holds a ShopCard in the Shop.
/// </summary>
public class ShopSlot : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// The index of this ShopSlot -- each slot has its
    /// own index.
    /// </summary>
    private int slotIndex;

    /// <summary>
    /// The ShopCard in this slot; null if this slot is empty.
    /// </summary>
    private ShopCard occupant;

    /// <summary>
    /// true if the ShopManager has initialized this ShopSlot; otherwise, false.
    /// </summary>
    private bool setupByManager;

    /// <summary>
    /// true if this ShopSlot is active in the Shop; otherwise, false if it is
    /// disabled.
    /// </summary>
    private bool activeInShop;

    #endregion

    #region Methods

    /// <summary>
    /// Fills the ShopSlot with a ShopCard.
    /// </summary>
    /// <param name="shopCard">The ShopCard to display.</param>
    public void Fill(ShopCard shopCard)
    {
        Assert.IsNotNull(shopCard);
        Assert.IsTrue(IsSetup());

        if (occupant != null) Destroy(occupant.gameObject);
        occupant = null;

        // TODO: put logic for positioning the card.
        RectTransform cardTransform = shopCard.GetCardTransform();
        cardTransform.SetParent(transform);
        cardTransform.localScale = Vector3.one;
        cardTransform.localPosition = Vector3.zero;

        occupant = shopCard;
    }

    /// <summary>
    /// Fills the ShopSlot with a blank ShopCard.
    /// </summary>
    public void FillWithBlank()
    {
        GameObject blankCard = ShopFactory.GetShopCardPrefab(ModelType.SHOP_CARD_BLANK);
        Assert.IsNotNull(blankCard);
        Fill(blankCard.GetComponent<ShopCard>());
        occupant = null;
    }

    /// <summary>
    /// Returns true if this ShopSlot has been setup by the ShopManager.
    /// </summary>
    /// <returns> true if this ShopSlot has been setup by the ShopManager;
    /// otherwise, false. </returns>
    public bool IsSetup() => setupByManager;    

    /// <summary>
    /// Sets this ShopSlot's necessary attributes so that it
    /// can function in the scene.
    /// </summary>
    /// <param name="slotIndex">The unique index of this ShopSlot.</param>
    public void SetupSlot(int slotIndex)
    {
        Assert.IsFalse(IsSetup());

        EnableSlot();
        this.slotIndex = slotIndex;
        setupByManager = true;
    }

    /// <summary>
    /// Returns an instantiated GameObject with this ShopSlot's Model
    /// attached. 
    /// </summary>
    /// <returns>an instantiated GameObject with this ShopSlot's Model
    /// attached.</returns>
    public GameObject GetCardPrefab()
    {
        Assert.IsTrue(IsSetup(), "Not setup.");
        return occupant.GetShopCardModelPrefab();
    }

    /// <summary>
    /// Returns the ModelType of the ShopCard in this ShopSlot.
    /// </summary>
    /// <returns>the ModelType of the ShopCard in this ShopSlot.</returns>
    public ModelType GetModelTypeOfCardInSlot()
    {
        Assert.IsTrue(IsSetup(), "Not setup.");
        return occupant.GetModelType();
    }

    /// <summary>
    /// Returns true if the player meets all conditions to buy the ShopCard that
    /// fills this ShopSlot.
    /// </summary>
    /// <param name="currentBalance">How much currency the player has.</param>
    /// <returns></returns>
    public bool CanBuy(int currentBalance) => occupant.GetPrice() <= currentBalance;

    /// <summary>
    /// Returns how much currency it takes to buy the ShopCard in this ShopSlot.
    /// Buys and removes the card.
    /// </summary>
    /// <param name="currentBalance">How much currency the player has.</param>
    /// <returns></returns>
    public int Buy(int currentBalance)
    {
        Assert.IsTrue(CanBuy(currentBalance));

        int price = occupant.GetPrice();
        Destroy(occupant.gameObject);

        return price;
    }

    /// <summary>
    /// Returns this ShopSlot's unique index.
    /// </summary>
    /// <returns>this ShopSlot's unique index.</returns>
    public int GetSlotIndex() => slotIndex;

    /// <summary>
    /// Returns true if the player clicked on this ShopSlot.
    /// </summary>
    /// <returns>true if the player clicked on this ShopSlot;
    /// otherwise, false. /// </returns>
    public bool SlotClicked()
    {
        if (Empty()) return false;
        return occupant.ClickedOn();
    }

    /// <summary>
    /// Returns true if this ShopSlot has been purchased and hosts
    /// no ShopCard.
    /// </summary>
    /// <returns>true if this ShopSlot has been purchased and hosts
    /// no ShopCard; otherwise, false. </returns>
    public bool Empty() => occupant == null;

    /// <summary>
    /// Sets the color of the ShopCard occupant's background and title.
    /// </summary>
    /// <param name="color">The color to set the ShopCard to.</param>
    public void SetOccupantCardColor(Color32 color)
    {
        Assert.IsFalse(Empty());
        occupant.SetCardColor(color);
    }

    /// <summary>
    /// Disables this ShopSlot.
    /// </summary>
    public void DisableSlot()
    {
        activeInShop = false;
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Enables this ShopSlot.
    /// </summary>
    public void EnableSlot()
    {
        activeInShop = true;
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Returns true if this ShopSlot is enabled in the Shop.
    /// </summary>
    /// <returns>true if this ShopSlot is enabled; otherwise, false. </returns>
    public bool IsEnabled() => activeInShop;

    #endregion
}
