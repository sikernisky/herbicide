using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

/// <summary>
/// Represents a card in the Shop for sale.
/// </summary>
public class ShopCard : UIModel
{
    #region Fields

    /// <summary>
    /// The ShopCardData that defines this ShopCard.
    /// </summary>
    public ShopCardData ShopCardData { get; private set; }

    /// <summary>
    /// true if this ShopCard is defined; otherwise, false.
    /// </summary>
    public bool IsDefined => ShopCardData != null;

    /// <summary>
    /// Image that represents the ShopCard's Defender.
    /// </summary>
    [SerializeField]
    private Image splashImage;

    /// <summary>
    /// Text to represent the ShopCard's title.
    /// </summary>
    [SerializeField]
    private TMP_Text nameText;

    /// <summary>
    /// Text to represent the ShopCard's cost.
    /// </summary>
    [SerializeField]
    private TMP_Text costText;

    #endregion

    #region Methods

    /// <summary>
    /// Defines this ShopCard with the given ShopCardData.
    /// </summary>
    public void DefineShopCard(ShopCardData data)
    {
        Assert.IsNotNull(data, "ShopCardData is null.");
        ShopCardData = data;
        SetNameText();
        SetCostText();
        SetShopCardSplash();
    }

    /// <summary>
    /// Sets the name text of this ShopCard to the name of the Defender
    /// it represents.
    /// </summary>
    private void SetNameText() => nameText.text = ShopCardData.CardName;

    /// <summary>
    /// Sets the cost text of this ShopCard to the cost of the Defender it represents.
    /// </summary>
    private void SetCostText() => costText.text = ShopCardData.CardCost.ToString();

    /// <summary>
    /// Sets the splash image of this ShopCard to the Defender it represents.
    /// </summary>
    private void SetShopCardSplash()
    {
        Sprite splash = ShopFactory.GetShopCardSprite(ShopCardData.DefenderType);
        splashImage.sprite = splash;
    }

    /// <summary>
    /// Returns the ModelType of this ShopCard.
    /// </summary>
    /// <returns>the ModelType of this ShopCard.</returns>
    public override ModelType GetModelType() => ModelType.SHOP_CARD;

    /// <summary>
    /// Sets the color of the ShopCard's background and title.
    /// </summary>
    /// <param name="color">The color to set the ShopCard to.</param>
    public void SetCardColor(Color32 color)
    {
        splashImage.color = color;
        nameText.color = color;
        costText.color = color;
    }

    /// <summary>
    /// Returns the Image of this ShopCard.
    /// </summary>
    /// <returns>the Image of this ShopCard.</returns>
    public Image GetCardBackgroundImage() => splashImage;

    #endregion
}
