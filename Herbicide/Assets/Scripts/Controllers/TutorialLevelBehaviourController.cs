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
    /// true if the nexii should pulse; otherwise, false.
    /// </summary>
    private bool pulseNexii;

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
    /// The amount of time that has elapsed since the evening text
    /// displayed.
    /// </summary>
    private float eveningTextTimeElapsed;

    /// <summary>
    /// true if the evening text timer should be updated; otherwise, false.
    /// </summary>
    private bool updateEveningTextTimer;

    /// <summary>
    /// The number of seconds the evening text should display for.
    /// </summary>
    private float eveningTextDisplayTime => 10.0f;

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

        AddDynamicEvent(new LevelBehaviourEvent(
            () => IsInEveningStage(),
            () => OnEnterEveningStage(),
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
        tutorialTextBackground.gameObject.SetActive(true);
    }

    /// <summary>
    /// Main update loop for the TutorialLevelBehaviourController.
    /// </summary>
    protected override void UpdateLevelBehaviourInstance()
    {
        if(pulseShop) PulseShop();
        if(pulseTrees) PulseTrees();
        if(pulseNexii) PulseNexii();
        if(updateEveningTextTimer) UpdateEveningTextTimer();
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
    /// then lightens them back up.
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

    /// <summary>
    /// Darkens all Nexii over a set interval,
    /// then lightens them back up.
    private void PulseNexii()
    {
        if (pulseTimer < pulseTime / 2)
        {
            ControllerManager.SetColorOfAllNexii(this, Color.Lerp(pulseLightColor, pulseDarkColor, pulseTimer / (pulseTime / 2)));
        }
        else
        {
            ControllerManager.SetColorOfAllNexii(this, Color.Lerp(pulseDarkColor, pulseLightColor, (pulseTimer - pulseTime / 2) / (pulseTime / 2)));
        }

        if (pulseTimer > pulseTime) pulseTimer = 0;
        else pulseTimer += Time.deltaTime;
    }

    /// <summary>
    /// Updates the evening text timer.
    /// </summary>
    private void UpdateEveningTextTimer()
    {
        if (updateEveningTextTimer)
        {
            eveningTextTimeElapsed += Time.deltaTime;
            if (eveningTextTimeElapsed > eveningTextDisplayTime)
            {
                tutorialText.text = "";
                tutorialText.enabled = false;
                tutorialTextBackground.enabled = false;
                updateEveningTextTimer = false;
            }
        }
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

    /// <summary>
    /// Returns true if the current stage is the evening stage.
    /// </summary>
    /// <returns>true if the current stage is the evening stage;
    /// otherwise, false.</returns>
    private bool IsInEveningStage() => StageController.GetCurrentStage() == StageController.StageOfDay.EVENING;

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
        tutorialText.text = "Place the Defender on a Tree to protect your eggplant!";
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
        pulseNexii = true;
        tutorialText.enabled = true;
        tutorialTextBackground.enabled = true;
        tutorialText.text = "Keep your eggplant safe through the night to win !";
    }

    /// <summary>
    /// Called when the player completes the first intermission.
    /// </summary>
    private void OnCompleteFirstIntermission()
    {
        pulseNexii = false;
        tutorialText.text = "";
        tutorialText.enabled = false;
        tutorialTextBackground.enabled = false;
        ControllerManager.SetColorOfAllNexii(this, pulseLightColor);
    }

    /// <summary>
    /// Called when the player enters the evening stage.
    /// </summary>
    private void OnEnterEveningStage()
    {
        updateEveningTextTimer = true;
        tutorialText.enabled = true;
        tutorialTextBackground.enabled = true;
        tutorialText.text = "Your talent pool shows new Defenders if you wait long enough!";
    }

    #endregion  
}
