using System.Collections.Generic;
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
    /// Image that represents the ShopCard's background.
    /// </summary>
    [SerializeField]
    private Image cardBackgroundImage;

    /// <summary>
    /// Image that represents the ShopCard's art.
    /// </summary>
    [SerializeField]
    private Image cardSplash;

    /// <summary>
    /// Text to represent the ShopCard's title.
    /// </summary>
    [SerializeField]
    private TMP_Text cardTitle;

    /// <summary>
    /// Text components to represent the ShopCard model's traits.<br></br>
    /// 
    /// This is disabled if the ShopCard is an upgrade card.
    /// </summary>
    [SerializeField]
    private List<TMP_Text> traitNames;

    /// <summary>
    /// The ShopCard's RectTransform component.
    /// </summary>
    [SerializeField]
    private RectTransform cardTransform;

    /// <summary>
    /// The Model this ShopCard produces
    /// </summary>
    [SerializeField]
    private Model model;

    /// <summary>
    /// The ShopCard's button component.
    /// </summary>
    [SerializeField]
    private Button cardButton;

    /// <summary>
    /// true if the player clicked on this ShopCard; otherwise,
    /// false. 
    /// </summary>
    private bool clicked;

    /// <summary>
    /// Color of this ShopCard when darkened.
    /// </summary>
    private readonly Color32 darkenedColor = new Color32(100, 100, 100, 255);

    /// <summary>
    /// Color of this ShopCard normally.
    /// </summary>
    private readonly Color32 defaultColor = new Color32(255, 255, 255, 255);

    #endregion

    #region Methods

    /// <summary>
    /// Returns the ModelType of this ShopCard. To do this, examines
    /// the Model attached to this ShopCard. We need to do this
    /// in order to not make a class for each ShopCard type.
    /// </summary>
    /// <returns> the ModelType of this ShopCard</returns>
    public override ModelType GetModelType()
    {
        if(model == null) return ModelType.SHOP_CARD_BLANK;
        return ShopFactory.GetShopCardModelTypeFromModelType(model.TYPE);
    }

    /// <summary>
    /// Returns this ShopCard's RectTransform component.
    /// </summary>
    /// <returns>this ShopCard's RectTransform component.</returns>
    public RectTransform GetCardTransform() => cardTransform;

    /// <summary>
    /// Returns an instantiated GameObject with this ShopCard's Model
    /// attached. 
    /// </summary>
    /// <returns>an instantiated GameObject with this ShopCard's Model
    /// attached.</returns>
    public GameObject GetShopCardModelPrefab()
    {
        Assert.IsNotNull(model, "Model is null.");
        GameObject modelCopy = model.CreateNew();
        modelCopy.transform.position = new Vector3(-1000, -1000, 1);
        return modelCopy;
    }

    /// <summary>
    /// Returns the price of this ShopCard's Model. Returns 0 if this
    /// is an upgrade card.
    /// </summary>
    /// <returns>the price of this ShopCard's Model, or 0 if this is
    /// an upgrade card.</returns>
    public int GetPrice() => model.COST;

    /// <summary>
    /// #BUTTON EVENT#
    /// 
    /// Called when the player clicks this ShopCard. Stores
    /// the click.
    /// </summary>
    public void ClickShopCard()
    {
        if (EconomyController.GetBalance(ModelType.DEW) < model.COST) return;
        clicked = true;
    }

    /// <summary>
    /// Returns true if the player clicked this ShopCard.
    /// </summary>
    /// <returns>true if the player clicked this ShopCard;
    /// otherwise, false. /// </returns>
    public bool ClickedOn() => clicked;

    /// <summary>
    /// Resets the ShopCard's click status.
    /// </summary>
    public void ResetClick() => clicked = false;

    /// <summary>
    /// Turns the ShopCard's background a darker color.
    /// </summary>
    public void TurnDark()
    {
        cardBackgroundImage.color = darkenedColor;
        cardTitle.color = darkenedColor;
    }

    /// <summary>
    /// Turns the ShopCard's background to its normal color.
    /// </summary>
    public void TurnLight()
    {
        cardBackgroundImage.color = defaultColor;
        cardTitle.color = defaultColor;
    }

    #endregion
}
