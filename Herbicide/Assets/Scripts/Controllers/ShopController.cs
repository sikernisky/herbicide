using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

/// <summary>
/// Manages the Shop and its ShopCards.
/// </summary>
public class ShopController : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// Reference to the Shop's timer bar background Image component.
    /// </summary>
    [SerializeField]
    private Image timerBarBackground;

    /// <summary>
    /// Reference to the Shop's timer bar fill SpriteRenderer component.
    /// </summary> 
    [SerializeField]
    private Image timerBarFill;

    /// <summary>
    /// The slots for ShopCards. These are GameObjects that
    /// will dynamically take on the form of a specific 
    /// ShopCard. 
    /// </summary>
    [SerializeField]
    private List<ShopSlot> shopSlots;

    /// <summary>
    /// Reference to the Reroll button component.
    /// </summary>
    [SerializeField]
    private Button rerollButton;

    /// <summary>
    /// Reference to the Reroll button's TextMeshPro component.
    /// </summary>
    [SerializeField]
    private TMP_Text rerollText;

    /// <summary>
    /// How much currency it costs to reroll the shop.
    /// </summary>
    private static readonly int REROLL_COST = 25;

    /// <summary>
    /// How long the shop will wait before automatically rerolling.
    /// </summary>
    private static readonly float AUTOMATIC_REROLL_TIME = 25f;

    /// <summary>
    /// How long it has been since the last reroll.
    /// </summary>
    private float timeSinceLastReroll;

    /// <summary>
    /// Defines a delegate for buying a Defender from the shop.
    /// </summary>
    /// <param name="defenderType">The type of Defender that was bought.</param>
    public delegate Defender BuyModelDelegate(ModelType defenderType);

    /// <summary>
    /// Event triggered when a Defender is bought from the Shop.
    /// </summary>
    public event BuyModelDelegate OnBuyDefender;

    /// <summary>
    /// true if the shop has been loaded; false otherwise.
    /// </summary>
    private bool shopLoaded;

    /// <summary>
    /// true if the shop is active; false if the upgrades are active.
    /// </summary>
    private bool shopActive;

    /// <summary>
    /// The number of cards purchased from the shop this level.
    /// </summary>
    private static int numCardsPurchased;

    #endregion

    #region Methods

    /// <summary>
    /// Main update loop for the ShopManager.
    /// </summary>
    /// <param name="gameState">The most recent GameState.</param>
    public void UpdateShop(GameState gameState)
    {
        if (gameState != GameState.ONGOING) return;

        UpdateRerollButtonBasedOnPlayerBalance();
        UpdateRerollTimer();
    }

    /// <summary>
    /// Updates the Reroll button based on the player's balance.
    /// </summary>
    private void UpdateRerollButtonBasedOnPlayerBalance()
    {
        bool canAffordReroll = EconomyController.GetBalance(ModelType.DEW) >= REROLL_COST;
        if (canAffordReroll) EnableRerollButton();
        else DisableRerollButton();
    }

    /// <summary>
    /// Activates the Reroll button.
    /// </summary>
    private void EnableRerollButton()
    {
        rerollButton.interactable = true;
        rerollText.color = new Color32(255, 255, 255, 255);
    }

    /// <summary>
    /// Deactivates the Reroll button.
    /// </summary>
    private void DisableRerollButton()
    {
        rerollButton.interactable = false;
        rerollText.color = new Color32(100, 100, 100, 255);
    }

    /// <summary>
    /// Updates the Reroll timer.
    /// </summary>
    private void UpdateRerollTimer()
    {
        float timePercentage = timeSinceLastReroll / AUTOMATIC_REROLL_TIME;
        timerBarFill.transform.localScale = new Vector3(timePercentage, 1, 1);
        if (timeSinceLastReroll <= 0) Reroll(true);
        else timeSinceLastReroll -= Time.deltaTime;
    }

    /// <summary>
    /// Sets up the shop with the types of ShopCards it can sell.
    /// </summary>
    /// <param name="shopManager">the ShopManager singleton</param>
    public void InitializeShop(ShopManager shopManager)
    {
        Assert.IsNotNull(shopManager, "ShopManager is null.");
        Assert.IsFalse(shopActive || shopLoaded);

        SubscribeToDelegates();
        SetupSerializedShopSlots();
        Reroll(true);
        timeSinceLastReroll = AUTOMATIC_REROLL_TIME;
        shopLoaded = true;
        shopActive = true;
    }

    /// <summary>
    /// Subscribes to the necessary delegates.
    /// </summary>
    private void SubscribeToDelegates()
    {
        EconomyController.SubscribeToBalanceUpdatedDelegate(UpdateShopCards);
    }

    /// <summary>
    /// Sets up the ShopSlots with ShopCards.
    /// </summary>
    private void SetupSerializedShopSlots()
    {
        for (int slotIndex = 0; slotIndex < shopSlots.Count; slotIndex++)
        {
            ShopSlot slotToActivate = shopSlots[slotIndex];
            slotToActivate.Setup(slotIndex, ShopFactory.GetShopCardPrefab(ModelType.SHOP_CARD).GetComponent<ShopCard>(), HandleShopSlotButtonClick);
        }
    }

    /// <summary>
    /// Returns the set of ModelTypes that can be purchased from the shop.
    /// </summary>
    /// <returns>a set of ModelTypes that can be purchased from the shop.</returns>
    private List<ModelType> GetUpdatedCardPool()
    {
        List<ModelType> unlockedModelTypes = new List<ModelType>(CollectionManager.GetAllUnlockedDefenders());
        unlockedModelTypes.RemoveAll(mt => !ModelTypeHelper.IsDefender(mt));
        unlockedModelTypes = unlockedModelTypes.Distinct().ToList();
        return unlockedModelTypes;
    }

    /// <summary>
    /// Rerolls the shop, replacing every ShopSlot with a new ShopCard
    /// from the ShopManager's card pool.
    /// </summary>
    /// <param name="isFree">true if this reroll won't charge the player;
    /// false if it will. </param>
    private void Reroll(bool isFree)
    {
        if(!isFree && EconomyController.GetBalance(ModelType.DEW) < REROLL_COST) return;
        if(!isFree) EconomyController.Withdraw(ModelType.DEW, REROLL_COST);
        shopSlots.ForEach(ss => DefineSlotWithShopCard(ss, PullRandomDefenderType()));
        PostReroll();
    }

    /// <summary>
    /// Returns a random Defender from the ShopManager's card pool.
    /// </summary>
    /// <returns>a random Defender from the ShopManager's card pool.</returns>
    private ModelType PullRandomDefenderType()
    {
        List<ModelType> pool = GetUpdatedCardPool();
        int randomIndex = Random.Range(0, pool.Count);
        var randomKey = pool[randomIndex];
        ModelType randomType = randomKey;
        return randomType;
    }

    /// <summary>
    /// Called after a reroll is completed.
    /// </summary>
    private void PostReroll()
    {
        UpdateShopCards();
        timeSinceLastReroll = AUTOMATIC_REROLL_TIME;
    }

    /// <summary>
    /// Defines a ShopSlot with a ShopCard of a given ModelType.
    /// </summary>
    /// <param name="defenderType">The ModelType of the Defender to define.</param>
    /// <param name="slotToDefine">The ShopSlot to define.</param>
    private void DefineSlotWithShopCard(ShopSlot slotToDefine, ModelType defenderType)
    {
        Assert.IsNotNull(slotToDefine, "Slot is null");
        Assert.IsTrue(slotToDefine.Filled, "Slot is not filled.");
        Assert.IsTrue(ModelTypeHelper.IsDefender(defenderType), "ModelType is not a Defender.");
        ShopCardData cardData = ShopFactory.GetShopCardData(defenderType);
        slotToDefine.DefineCard(cardData);
    }

    /// <summary>
    /// Called when the player finishes placing an IUseable.
    /// </summary>
    private void OnFinishPlacing() => UpdateShopCards();

    /// <summary>
    /// Iterates through the ShopSlots and darkens the ones the player
    /// cannot afford. Lightens the ones they can.
    /// </summary>
    private void UpdateShopCards()
    {
        // Color / Appearance of ShopSlots
        foreach (ShopSlot shopSlot in shopSlots)
        {
            if (!shopSlot.Defined) continue;
            if (!shopSlot.CanBuy(EconomyController.GetBalance(ModelType.DEW))) shopSlot.SetOccupantCardColor(new Color32(100, 100, 100, 255));
            else shopSlot.SetOccupantCardColor(new Color32(255, 255, 255, 255));
        }
    }

    /// <summary>
    /// Returns true if the shop is empty; false otherwise.
    /// </summary>
    /// <returns>true if the shop is empty; false otherwise.</returns>
    private bool IsShopEmpty() => shopSlots.All(ss => !ss.Filled);

    /// <summary>
    /// Subscribes a handler (the ControllerManager) to the request upgrade event.
    /// </summary>
    /// <param name="handler">The handler to subscribe.</param>
    public void SubscribeToBuyDefenderDelegate(BuyModelDelegate handler)
    {
        Assert.IsNotNull(handler, "Handler is null.");
        OnBuyDefender += handler;
    }

    /// <summary>
    /// Returns the number of cards purchased from the shop this level.
    /// </summary>
    /// <returns>the number of cards purchased from the shop this level.</returns>
    public static int GetNumCardsPurchased() => numCardsPurchased;

    #endregion

    #region Button Events

    /// <summary>
    /// #BUTTON EVENT#
    /// 
    /// Called when the player clicks the Reroll button. Refreshes
    /// the shop if they have enough money.
    /// </summary>
    public void ClickRerollButton() => Reroll(false);

    /// <summary>
    /// # BUTTON EVENT #
    /// 
    /// Called when the player clicks the BasicTreeSeed button.
    /// If possible, the player will spend a BasicTreeSeed and
    /// start placing a BasicTree.
    /// </summary>
    public void ClickBasicTreeSeedButton()
    {
        if (EconomyController.GetBalance(ModelType.BASIC_TREE_SEED) < 1) return;
        EconomyController.Withdraw(ModelType.BASIC_TREE_SEED, 1);
        GameObject basicTreePrefab = TreeFactory.GetTreePrefab(ModelType.BASIC_TREE);
        Assert.IsNotNull(basicTreePrefab, "BasicTree prefab is null.");
        Model basicTreeModel = basicTreePrefab.GetComponent<Model>();
        Assert.IsNotNull(basicTreeModel, "BasicTree model is null.");
        PlacementManager.StartPlacing(basicTreeModel);
    }

    /// <summary>
    /// # BUTTON EVENT #
    /// 
    /// Called when the player clicks the BasicTreeSeed button.
    /// If possible, the player will spend a BasicTreeSeed and
    /// start placing a BasicTree.
    /// </summary>
    public void ClickSpeedTreeSeedButton()
    {
        if (EconomyController.GetBalance(ModelType.SPEED_TREE_SEED) < 1) return;
        EconomyController.Withdraw(ModelType.SPEED_TREE_SEED, 1);
        GameObject speedTreePrefab = TreeFactory.GetTreePrefab(ModelType.SPEED_TREE);
        Assert.IsNotNull(speedTreePrefab, "SpeedTree prefab is null.");
        Model speedTreeModel = speedTreePrefab.GetComponent<Model>();
        Assert.IsNotNull(speedTreeModel, "SpeedTree model is null.");
        PlacementManager.StartPlacing(speedTreeModel);
    }

    /// <summary>
    /// #BUTTON EVENT#
    /// 
    /// Called when a ShopSlot button is clicked. Starts placing
    /// from the clicked slot if possible. /// </summary>
    /// <param name="slotIndex">The Slot that was clicked.</param>
    public void HandleShopSlotButtonClick(int slotIndex)
    {
        Assert.IsTrue(slotIndex >= 0 && slotIndex < shopSlots.Count);
        ShopSlot clickedSlot = shopSlots.First(ss => ss.SlotIndex == slotIndex);
        if(!CanBuyClickedSlotFromShop(clickedSlot)) return;

        ModelType modelTypeInSlot = clickedSlot.Occupant.ShopCardData.DefenderType;
        EconomyController.Withdraw(ModelType.DEW, clickedSlot.Occupant.ShopCardData.CardCost);
        clickedSlot.OnBuy();
        UpdateShopCards();
        Defender constructedDefender = OnBuyDefender?.Invoke(modelTypeInSlot);
        Assert.IsNotNull(constructedDefender, "Constructed Defender is null.");
        TicketManager.OnPurchaseOrUseModel(modelTypeInSlot);
        PlacementManager.OnPlacementFinished += (success) => { OnFinishPlacing(); };
        PlacementManager.StartPlacing(constructedDefender);

        numCardsPurchased++;
        if (IsShopEmpty()) Reroll(true);
    }


    /// <summary>
    /// Returns true if the player can buy the clicked slot's contents from the shop.
    /// </summary>
    /// <param name="clickedSlot">the slot clicked.</param>
    /// <returns>true if the player can buy the clicked slot's contents from the shop;
    /// otherwise, false. </returns>
    private bool CanBuyClickedSlotFromShop(ShopSlot clickedSlot)
    {
        if(!clickedSlot.Filled) return false;
        if(!clickedSlot.CanBuy(EconomyController.GetBalance(ModelType.DEW))) return false;
        if(PlacementManager.IsPlacing) return false;
        if(!ControllerManager.IsSpaceForModelOnSomeTree(clickedSlot.Occupant.ShopCardData.DefenderType)) return false;
        return true;
    }

    #endregion
}
