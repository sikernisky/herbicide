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
    private bool ShouldPulseShop { get; set; }

    /// <summary>
    /// true if the trees should pulse; otherwise, false.
    /// </summary>
    private bool ShouldPulseTrees { get; set; }

    /// <summary>
    /// Number of seconds between pulses.
    /// </summary>
    private float PulseTimer { get; set; }

    /// <summary>
    /// Number of seconds since the player used their first item.
    /// </summary>
    private float AfterUseFirstItemTimer { get; set; }

    /// <summary>
    /// Number of seconds since the player placed their first Defender.
    /// </summary>
    private float AfterPlaceFirstDefenderTimer { get; set; }

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

    #endregion

    #region Methods

    /// <summary>
    /// Sets up the tutorial level's specific events.
    /// </summary>
    protected override void InitializeLevelBehaviorEvents()
    {
        AddSequentialEvent(new LevelBehaviourEvent(
            () => DidPurchaseShopCard(),
            () => OnPurchaseShopCard()));

        AddSequentialEvent(new LevelBehaviourEvent(
            () => DidPlaceDefender(),
            () => OnPlaceDefender()));

        AddDynamicEvent(new LevelBehaviourEvent(
            () => DidWaitForTimeAfterPlacingFirstDefender(),
            () => OnWaitForTimeAfterPlacingFirstDefender()));

        AddSequentialEvent(new LevelBehaviourEvent(
            () => DidSatisfyTicketRequirements(),
            () => OnSatisfyFirstTicket()));

        AddSequentialEvent(new LevelBehaviourEvent(
            () => DidCraftFirstTicket(),
            () => OnCompleteFirstTicket()));

        AddSequentialEvent(new LevelBehaviourEvent(
            () => DidUseFirstItem(),
            () => OnUseFirstItem()));

        AddDynamicEvent(new LevelBehaviourEvent(
            () => DidWaitForTimeAfterUsingItem(),
            () => OnWaitForTimeAfterUsingFirstItem()));
    }

    /// <summary>
    /// Main update loop for the TutorialLevelBehaviourController.
    /// </summary>
    protected override void UpdateLevelBehaviourInstance()
    {
        if(ShouldPulseShop) PulseShop();
        if(ShouldPulseTrees) PulseTrees();
        if(DidUseFirstItem()) AfterUseFirstItemTimer += Time.deltaTime;
        if(DidPlaceDefender()) AfterPlaceFirstDefenderTimer += Time.deltaTime;
    }

    /// <summary>
    /// Darkens the shop cards over a set interval,
    /// then lightens them back up. Repeats this process
    /// as long as the shop is pulsing.
    /// </summary>
    private void PulseShop()
    {
        float pulseTime = TutorialConstants.DefaultModelPulseTime;
        Color32 pulseLightColor = TutorialConstants.PulseLightColor;
        Color32 pulseDarkColor = TutorialConstants.PulseDarkColor;
        UpdateShopSlotColors(PulseTimer, pulseTime, pulseLightColor, pulseDarkColor);
        if (PulseTimer > pulseTime) PulseTimer = 0;
        else PulseTimer += Time.deltaTime;
    }

    /// <summary>
    /// Updates the shop slot colors based on the current pulse timer.
    /// </summary>
    /// <param name="timer">The current pulse timer.</param>
    /// <param name="time">The total time for the pulse.</param>
    /// <param name="lightColor">The light color to pulse to.</param>
    /// <param name="darkColor">The dark color to pulse to.</param>
    private void UpdateShopSlotColors(float timer, float time, Color32 lightColor, Color32 darkColor)
    {
        Color32 startColor = timer < time / 2 ? lightColor : darkColor;
        Color32 endColor = timer < time / 2 ? darkColor : lightColor;
        float lerpFactor = timer < time / 2 ? timer / (time / 2) : (timer - time / 2) / (time / 2);
        foreach (ShopSlot shopSlot in shopSlots) { shopSlot.SetOccupantCardColor(Color.Lerp(startColor, endColor, lerpFactor)); }
    }

    /// <summary>
    /// Darkens all Trees over a set interval,
    /// then lightens them back up.
    /// </summary>
    private void PulseTrees()
    {
        float pulseTime = TutorialConstants.DefaultModelPulseTime;
        Color32 pulseLightColor = TutorialConstants.PulseLightColor;
        Color32 pulseDarkColor = TutorialConstants.PulseDarkColor;
        if (PulseTimer < pulseTime / 2)
        {
            ControllerManager.SetColorOfAllTrees(this, Color.Lerp(pulseLightColor, pulseDarkColor, PulseTimer / (pulseTime / 2)));
        }
        else
        {
            ControllerManager.SetColorOfAllTrees(this, Color.Lerp(pulseDarkColor, pulseLightColor, (PulseTimer - pulseTime / 2) / (pulseTime / 2)));
        }
        if (PulseTimer > pulseTime) PulseTimer = 0;
        else PulseTimer += Time.deltaTime;
    }

    /// <summary>
    /// Removes all effects that the tutorial has applied.
    /// </summary>
    private void RemoveAllEffects()
    {
        ShouldPulseShop = false;
        ShouldPulseTrees = false;
        tutorialText.text = string.Empty;
        tutorialText.enabled = false;
        tutorialTextBackground.enabled = false;
        ControllerManager.SetColorOfAllTrees(this, TutorialConstants.PulseLightColor);
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
    /// Returns true if the player has satisfied the ticket requirements.
    /// </summary>
    /// <returns>true if the player has satisfied the ticket requirements;
    /// otherwise, false.</returns>
    private bool DidSatisfyTicketRequirements() => TicketManager.GetNumberOfSatisfiedTickets() > 0;

    /// <summary>
    /// Returns true if the player has crafted their first ticket.
    /// </summary>
    /// <returns>true if the player has crafted their first ticket;
    /// otherwise, false.</returns>
    private bool DidCraftFirstTicket() => TicketManager.NumCompletedTickets > 0;

    /// <summary>
    /// Returns true if the player has used their first item.
    /// </summary>
    /// <returns>true if the player has used their first item;
    /// otherwise, false.</returns>
    private bool DidUseFirstItem() => PlacementManager.NumUseablesEquipped > 0;

    /// <summary>
    /// Returns true if the player has waited for the specified time
    /// after placing their first Defender.
    /// </summary>
    /// <returns>true if the player has waited for the specified time
    /// after placing their first Defender; otherwise, false.</returns>
    private bool DidWaitForTimeAfterPlacingFirstDefender() => AfterPlaceFirstDefenderTimer > TutorialConstants.OnPlaceFirstDefenderMessageDuration;

    /// <summary>
    /// Returns true if the player has waited for the specified time
    /// after using their first item.
    /// </summary>
    /// <returns>true if the player has waited for the specified time
    /// after using their first item; otherwise, false.</returns>
    private bool DidWaitForTimeAfterUsingItem() => AfterUseFirstItemTimer > TutorialConstants.OnUseFirstItemMessageDuration;

    #endregion

    #region Event Consequences

    /// <summary>
    /// Called when the player purchases a card from the shop.
    /// </summary>
    private void OnPurchaseShopCard()
    {
        ShouldPulseShop = false;
        ShouldPulseTrees = true;
        tutorialTextBackground.enabled = true;
        tutorialText.enabled = true;
        tutorialText.text = TutorialConstants.OnBuyFirstDefenderMessage;
    }

    /// <summary>
    /// Called when the player places their first Defender in the scene.
    /// </summary>
    private void OnPlaceDefender()
    {
        ShouldPulseTrees = false;
        ControllerManager.SetColorOfAllTrees(this, TutorialConstants.PulseLightColor);
        tutorialTextBackground.enabled = true;
        tutorialText.enabled = true;
        tutorialText.text = TutorialConstants.OnPlaceFirstDefenderMessage;
    }

    /// <summary>
    /// Called when the player waits for the specified time after placing their first Defender.
    /// </summary>
    private void OnWaitForTimeAfterPlacingFirstDefender()
    {
        tutorialText.text = string.Empty;
        tutorialText.enabled = false;
        tutorialTextBackground.enabled = false;
    }

    /// <summary>
    /// Called when the player satisfies the first ticket.
    /// </summary>
    private void OnSatisfyFirstTicket()
    {
        tutorialText.enabled = true;
        tutorialTextBackground.enabled = true;
        tutorialText.text = TutorialConstants.OnSatisfyFirstTicketMessage;
    }

    /// <summary>
    /// Called when the player crafts the satisfied ticket.
    /// </summary>
    private void OnCompleteFirstTicket()
    {
        tutorialText.enabled = true;
        tutorialTextBackground.enabled = true;
        tutorialText.text = TutorialConstants.OnCompleteFirstTicketMessage;
    }

    /// <summary>
    /// Called when the player uses the first item.
    /// </summary>
    private void OnUseFirstItem()
    {
        tutorialText.enabled = true;
        tutorialTextBackground.enabled = true;
        tutorialText.text = TutorialConstants.OnUseFirstItemMessage;
    }

    /// <summary>
    /// Called when the player waits for the specified time after using their first item.
    /// </summary>
    private void OnWaitForTimeAfterUsingFirstItem()
    {
        tutorialText.text = string.Empty;
        tutorialText.enabled = false;
        tutorialTextBackground.enabled = false;
    }

    /// <summary>
    /// Called when the level starts
    /// </summary>
    protected override void OnStart()
    {
        ShouldPulseShop = true;
        tutorialTextBackground.gameObject.SetActive(true);
        tutorialText.enabled = true;
        tutorialTextBackground.enabled = true;
        tutorialText.text = TutorialConstants.OnStartTutorialMessage;
    }

    /// <summary>
    /// Called when the player loses the level.
    /// </summary>
    protected override void OnLose() => RemoveAllEffects();

    /// <summary>
    /// Called when the player loses the level.
    /// </summary>
    protected override void OnWin() => RemoveAllEffects();

    #endregion  
}
