using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attached to a GameObject in the Tutorial scene. Controls
/// the tutorial level's specific behaviours.
/// </summary>
public class TutorialLevelBehaviourController : LevelBehaviourController
{
    #region Fields

    /// <summary>
    /// true if the shop should pulse; otherwise, false.
    /// </summary>
    private bool pulseShop;

    /// <summary>
    /// true if the trees should pulse; otherwise, false.
    /// </summary>
    private bool pulseTrees;

    /// <summary>
    /// The amount of time it takes for the shop to pulse from
    /// light to dark and back again.
    /// </summary>
    private float pulseTime => 2.0f;

    /// <summary>
    /// Keeps track of the time between pulses.
    /// </summary>
    private float pulseTimer;

    /// <summary>
    /// The ShopSlots to manipulate in the scene.
    /// </summary>
    [SerializeField]
    private List<ShopSlot> shopSlots;

    /// <summary>
    /// The text component that displays the tutorial text.
    /// </summary>
    [SerializeField]
    private TMP_Text tutorialText;

    /// <summary>
    /// The background image for the tutorial text.
    /// </summary>
    [SerializeField]
    private Image tutorialTextBackground;

    /// <summary>
    /// The dark color to pulse the shop cards to.
    /// </summary>
    private Color32 pulseDarkColor => new Color32(100, 100, 100, 255);

    /// <summary>
    /// The light color to pulse the shop cards to.
    /// </summary>
    private Color32 pulseLightColor => new Color32(255, 255, 255, 255);

    #endregion

    #region Methods

    /// <summary>
    /// Sets up the tutorial level's specific events.
    /// </summary>
    protected override void InitializeLevelBehaviorEvents()
    {
        AddDynamicEvent(new LevelBehaviourEvent(
            () => DidPurchaseShopCard(),
            () => OnPurchaseShopCard(),
            false));

        AddDynamicEvent(new LevelBehaviourEvent(
            () => DidPlaceDefender(),
            () => OnPlaceDefender(),
            false));

        AddSequentialEvent(new LevelBehaviourEvent(
            () => IsInFirstIntermission(),
            () => OnFirstIntermission(),
            true));

        AddSequentialEvent(new LevelBehaviourEvent(
            () => DidCompleteFirstIntermission(),
            () => OnCompleteFirstIntermission(),
            true));

        pulseShop = true;
    }

    /// <summary>
    /// Main update loop for the TutorialLevelBehaviourController.
    /// </summary>
    protected override void UpdateLevelBehaviourInstance()
    {
        if(pulseShop) PulseShop();
        if(pulseTrees) PulseTrees();
    }

    /// <summary>
    /// Darkens the shop cards over a set interval,
    /// then lightens them back up. Repeats this process
    /// as long as the shop is pulsing.
    /// </summary>
    private void PulseShop()
    {
        if (pulseTimer < pulseTime / 2)
        {
            foreach (ShopSlot shopSlot in shopSlots)
            {
                shopSlot.SetOccupantCardColor(Color.Lerp(pulseLightColor, pulseDarkColor, pulseTimer / (pulseTime / 2)));
            }
        }
        else
        {
            foreach (ShopSlot shopSlot in shopSlots)
            {
                shopSlot.SetOccupantCardColor(Color.Lerp(pulseDarkColor, pulseLightColor, (pulseTimer - pulseTime / 2) / (pulseTime / 2)));
            }
        }

        if (pulseTimer > pulseTime) pulseTimer = 0;
        else pulseTimer += Time.deltaTime;
    }

    /// <summary>
    /// Darkens all Trees over a set interval,
    /// then lightens them back up. Repeats this process
    /// as long as the player has purchased but not placed
    /// their first Defender.
    /// </summary>
    private void PulseTrees()
    {
        if (pulseTimer < pulseTime / 2)
        {
            ControllerManager.SetColorOfAllTrees(this, Color.Lerp(pulseLightColor, pulseDarkColor, pulseTimer / (pulseTime / 2)));
        }
        else
        {
            ControllerManager.SetColorOfAllTrees(this, Color.Lerp(pulseDarkColor, pulseLightColor, (pulseTimer - pulseTime / 2) / (pulseTime / 2)));
        }

        if (pulseTimer > pulseTime) pulseTimer = 0;
        else pulseTimer += Time.deltaTime;
    }
    

    #endregion

    #region Event Conditions

    /// <summary>
    /// Returns true if the player has purchased a card from the shop.
    /// </summary>
    /// <returns>true if the player has purchased a card from the shop;
    /// otherwise, false.</returns>
    private bool DidPurchaseShopCard() => ShopController.GetNumCardsPurchased() > 0;

    /// <summary>
    /// Returns true if the player has placed a Defender in the scene.
    /// </summary>
    /// <returns>true if the player has placed a Defender in the scene;
    /// otherwise, false.</returns>
    private bool DidPlaceDefender() => ControllerManager.NumPlacedDefenders() > 0;

    /// <summary>
    /// Returns true if the player is entering the first intermission,
    /// which happens after the morning stage.
    /// </summary>
    /// <returns>true if the player is entering the first intermission;
    /// otherwise, false.</returns>
    private bool IsInFirstIntermission() => StageController.IsIntermissionActive();

    /// <summary>
    /// Returns true if the player has completed the first intermission.
    /// </summary>
    /// <returns>true if the player has completed the first intermission;
    /// otherwise, false.</returns>
    private bool DidCompleteFirstIntermission() => !StageController.IsIntermissionActive();

    #endregion

    #region Event Consequences

    /// <summary>
    /// Called when the player purchases a card from the shop.
    /// </summary>
    private void OnPurchaseShopCard()
    {
        pulseShop = false;
        pulseTrees = true;
        tutorialTextBackground.enabled = true;
        tutorialText.enabled = true;
        tutorialText.text = "Place the Defender on a Tree to protect the eggplant!";
    }

    /// <summary>
    /// Called when the player places their first Defender in the scene.
    /// </summary>
    private void OnPlaceDefender()
    {
        pulseTrees = false;
        ControllerManager.SetColorOfAllTrees(this, pulseLightColor);
        tutorialText.text = "";
        tutorialText.enabled = false;
        tutorialTextBackground.enabled = false;
    }

    /// <summary>
    /// Called when the player enters the first intermission.
    /// </summary>
    private void OnFirstIntermission()
    {
        tutorialText.enabled = true;
        tutorialTextBackground.enabled = true;
        tutorialText.text = "You've made it to the first intermission!";
    }

    /// <summary>
    /// Called when the player completes the first intermission.
    /// </summary>
    private void OnCompleteFirstIntermission()
    {
        tutorialText.text = "";
        tutorialText.enabled = false;
        tutorialTextBackground.enabled = false;
    }

    #endregion  
}
