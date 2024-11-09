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
    /// Text to represent the ShopCard's title.
    /// </summary>
    [SerializeField]
    private TMP_Text cardTitle;

    /// <summary>
    /// Text to represent the ShopCard's cost.
    /// </summary>
    [SerializeField]
    private TMP_Text cardCost;

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
        return ModelTypeHelper.GetShopCardModelTypeFromModelType(model.TYPE);
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
    /// Sets the color of the ShopCard's background and title.
    /// </summary>
    /// <param name="color">The color to set the ShopCard to.</param>
    public void SetCardColor(Color32 color)
    {
        cardBackgroundImage.color = color;
        cardTitle.color = color;
        cardCost.color = color;
    }

    #endregion
}
