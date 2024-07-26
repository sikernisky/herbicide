using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

/// <summary>
/// Represents a card in the Shop. Cards hold information
/// and art related to something the player can buy. 
/// </summary>
public class ShopCard : UIModel
{
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
    /// The prefab of the Model this ShopCard produces
    /// </summary>
    [SerializeField]
    private GameObject modelPrefab;

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


    /// <summary>
    /// Returns this ShopCard's RectTransform component.
    /// </summary>
    /// <returns>this ShopCard's RectTransform component.</returns>
    public RectTransform GetCardTransform() { return cardTransform; }

    /// <summary>
    /// Returns an instantiated GameObject with this ShopCard's Model
    /// attached. 
    /// </summary>
    /// <returns>an instantiated GameObject with this ShopCard's Model
    /// attached.</returns>
    public GameObject GetShopCardModelPrefab()
    {
        Assert.IsNotNull(modelPrefab, "Model prefab is null.");

        return Instantiate(modelPrefab);
    }

    /// <summary>
    /// Returns the price of this ShopCard's Model. Returns 0 if this
    /// is an upgrade card.
    /// </summary>
    /// <returns>the price of this ShopCard's Model, or 0 if this is
    /// an upgrade card.</returns>
    public int GetPrice() { return model.COST; }

    /// <summary>
    /// #BUTTON EVENT#
    /// 
    /// Called when the player clicks this ShopCard. Stores
    /// the click.
    /// </summary>
    public void ClickShopCard()
    {
        if (EconomyController.GetBalance() < model.COST) return;
        clicked = true;
    }

    /// <summary>
    /// Returns true if the player clicked this ShopCard.
    /// </summary>
    /// <returns>true if the player clicked this ShopCard;
    /// otherwise, false. /// </returns>
    public bool ClickedOn() { return clicked; }

    /// <summary>
    /// Turns the ShopCard's background a darker color.
    /// </summary>
    public void TurnDark()
    {
        cardBackgroundImage.color = darkenedColor;
        cardTitle.color = darkenedColor;
        //   cardButton.enabled = false;
    }

    /// <summary>
    /// Turns the ShopCard's background to its normal color.
    /// </summary>
    public void TurnLight()
    {
        cardBackgroundImage.color = defaultColor;
        cardTitle.color = defaultColor;
        //        cardButton.enabled = true;
    }
}
