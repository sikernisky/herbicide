using System.Collections.Generic;
using System.Linq;
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
    /// How much currency it costs to reroll the shop.
    /// </summary>
    private static readonly int REROLL_COST = 25;

    /// <summary>
    /// How long the shop will wait before automatically rerolling.
    /// </summary>
    private static readonly float AUTOMATIC_REROLL_TIME = 15f;

    /// <summary>
    /// How long it has been since the last reroll.
    /// </summary>
    private float timeSinceLastReroll;

    /// <summary>
    /// Defines a delegate for buying a Model from the shop.
    /// </summary>
    /// <param name="modelType">The Model that was bought.</param>
    public delegate void BuyModelDelegate(Model purchasedModel);

    /// <summary>
    /// Event triggered when a Model is bought from the Shop.
    /// </summary>
    public event BuyModelDelegate OnBuyModel;

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

        if (InputController.DidKeycodeDown(KeyCode.S)) { Reroll(false); } // TEMP

        // Check & Handle ShopCard click
        foreach (ShopSlot shopSlot in shopSlots)
        {
            if (shopSlot.SlotClicked())
            {
                ClickShopSlotButton(shopSlot.GetSlotIndex());
                shopSlot.ResetSlotClickStatus();
            }
        }

        // Enable / Disable reroll button depending on balance
        if (EconomyController.GetBalance(ModelType.DEW) < REROLL_COST) rerollButton.interactable = false;
        else rerollButton.interactable = true;

        // Update reroll timer
        float timePercentage = timeSinceLastReroll / AUTOMATIC_REROLL_TIME;
        timerBarFill.transform.localScale = new Vector3(timePercentage, 1, 1);
        if (timeSinceLastReroll <= 0) Reroll(true);
        else timeSinceLastReroll -= Time.deltaTime;
    }

    /// <summary>
    /// Sets up the shop with the types of ShopCards it can sell.
    /// </summary>
    /// <param name="shopManager">the ShopManager singleton</param>
    /// <param name="startingModels">the starting models for the shop</param>
    public void InitializeShop(ShopManager shopManager, List<ModelType> startingModels)
    {
        Assert.IsNotNull(shopManager, "ShopManager is null.");
        Assert.IsNotNull(startingModels, "Starting models list is null.");

        if (shopActive) return;
        if (!shopLoaded)
        {
            SetRerollButtonActive(false);
            EconomyController.SubscribeToBalanceUpdatedDelegate(UpdateShopCardLighting);
            numCardsPurchased = 0;
            PlacementController.SubscribeToFinishPlacingDelegate(OnFinishPlacing);
            int slotCounter = 0;
            foreach (ShopSlot shopSlot in shopSlots)
            {
                shopSlot.SetupSlot(slotCounter);
                slotCounter++;
            }

            Reroll(true, startingModels);
            timeSinceLastReroll = AUTOMATIC_REROLL_TIME;
        }
        else shopSlots.ForEach(ss => ss.EnableSlot());
        shopLoaded = true;
        shopActive = true;
    }

    /// <summary>
    /// Returns the set of ModelTypes that can be purchased from the shop.
    /// </summary>
    /// <returns>a set of ModelTypes that can be purchased from the shop.</returns>
    private List<ModelType> GetUpdatedCardPool()
    {
        HashSet<ModelType> shopCardsToLoad = new HashSet<ModelType>();
        List<ModelType> unlockedModelTypes = CollectionManager.GetAllUnlockedModelTypes();
        unlockedModelTypes.ForEach(umt => shopCardsToLoad.Add(umt));

        // Remove any ModelTypes that can't be purchased during the current stage
        StageController.StageOfDay currentStage = StageController.GetCurrentStage();
        unlockedModelTypes.RemoveAll(umt => !ModelTypeHelper.CanPurchaseDuringStage(umt, currentStage));

        // Remove any duplicate ModelTypes
        List<ModelType> uniqueModelTypes = new List<ModelType>();
        unlockedModelTypes.ForEach(umt => { if (!uniqueModelTypes.Contains(umt)) uniqueModelTypes.Add(umt); });

        return uniqueModelTypes;
    }

    /// <summary>
    /// Sets the Reroll button to be active or inactive.
    /// </summary>
    /// <param name="active">true if setting the button to be active;
    /// otherwise false. </param>
    public void SetRerollButtonActive(bool active) => rerollButton.gameObject.SetActive(active);

    /// <summary>
    /// Rerolls the shop, replacing every ShopSlot with a new ShopCard
    /// from the ShopManager's card pool.
    /// </summary>
    /// <param name="isFree">true if this reroll won't charge the player;
    /// false if it will. </param>
    private void Reroll(bool isFree)
    {
        if (!isFree)
        {
            bool canReroll = EconomyController.GetBalance(ModelType.DEW) >= REROLL_COST;
            if (!canReroll) return;
            EconomyController.Withdraw(ModelType.DEW, REROLL_COST);
        }

        List<ModelType> pool = GetUpdatedCardPool();
        foreach (ShopSlot cardSlot in shopSlots)
        {
            Assert.IsNotNull(cardSlot);
            int randomIndex = Random.Range(0, pool.Count);
            var randomKey = pool[randomIndex];
            ModelType randomType = randomKey;
            GameObject shopCardPrefab = ShopFactory.GetShopCardPrefab(randomType);
            Assert.IsNotNull(shopCardPrefab);
            ShopCard shopCardComp = shopCardPrefab.GetComponent<ShopCard>();
            Assert.IsNotNull(shopCardComp);
            cardSlot.Fill(shopCardComp);
        }

        UpdateShopCardLighting();

        timeSinceLastReroll = AUTOMATIC_REROLL_TIME;
        shopActive = true;
    }

    /// <summary>
    /// Rerolls the shop, replacing every ShopSlot with a new ShopCard
    /// from the ShopManager's card pool.
    /// </summary>
    /// <param name="isFree">true if this reroll won't charge the player;
    /// false if it will. </param>
    /// <param name="guaranteedRolls">A list of ModelTypes that will be
    /// included in the reroll. </param>
    private void Reroll(bool isFree, List<ModelType> guaranteedRolls)
    {
        if (!isFree)
        {
            bool canReroll = EconomyController.GetBalance(ModelType.DEW) >= REROLL_COST;
            if (!canReroll) return;
        }
        Assert.IsNotNull(guaranteedRolls);
        Reroll(isFree);

        for(int i = 0; i < shopSlots.Count; i++)
        {
            if(i >= guaranteedRolls.Count) break;
            shopSlots[i].FillWithBlank();
            ModelType modelType = guaranteedRolls[i];
            GameObject shopCardPrefab = ShopFactory.GetShopCardPrefab(modelType);
            Assert.IsNotNull(shopCardPrefab);
            ShopCard shopCardComp = shopCardPrefab.GetComponent<ShopCard>();
            Assert.IsNotNull(shopCardComp);
            shopSlots[i].Fill(shopCardComp);
        }
    }

    /// <summary>
    /// Called when the player finishes placing a Model.
    /// </summary>
    private void OnFinishPlacing(Model m) => Assert.IsNotNull(m, "Placed model is null.");

    /// <summary>
    /// Iterates through the ShopSlots and darkens the ones the player
    /// cannot afford. Lightens the ones they can.
    /// </summary>
    private void UpdateShopCardLighting()
    {
        foreach (ShopSlot shopSlot in shopSlots)
        {
            if (shopSlot.Empty()) continue;
            if (!shopSlot.CanBuy(EconomyController.GetBalance(ModelType.DEW))) shopSlot.SetOccupantCardColor(new Color32(100, 100, 100, 255));
            else shopSlot.SetOccupantCardColor(new Color32(255, 255, 255, 255));
        }
    }

    /// <summary>
    /// Subscribes a handler (the ControllerManager) to the request upgrade event.
    /// </summary>
    /// <param name="handler">The handler to subscribe.</param>
    public void SubscribeToBuyDefenderDelegate(BuyModelDelegate handler)
    {
        Assert.IsNotNull(handler, "Handler is null.");
        OnBuyModel += handler;
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
        PlacementController.StartPlacingObject(basicTreeModel);
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
        PlacementController.StartPlacingObject(speedTreeModel);
    }

    /// <summary>
    /// #BUTTON EVENT#
    /// 
    /// Called when a ShopSlot button is clicked. Starts placing
    /// from the clicked slot if possible. /// </summary>
    /// <param name="slotIndex">The Slot that was clicked.</param>
    public void ClickShopSlotButton(int slotIndex)
    {
        Assert.IsTrue(slotIndex >= 0 && slotIndex < shopSlots.Count);
        ShopSlot clickedSlot = shopSlots.First(ss => ss.GetSlotIndex() == slotIndex);
        if (clickedSlot.Empty()) return;
        if (PlacementController.IsPlacing()) return;
        if (PlacementController.IsCombining()) return;
        if (!clickedSlot.CanBuy(EconomyController.GetBalance(ModelType.DEW))) return;
        ModelType modelTypeInSlot = ModelTypeHelper.GetModelTypeFromShopCardModelType(clickedSlot.GetModelTypeOfCardInSlot());
        if (!ControllerManager.IsSpaceForModelOnSomeTree(modelTypeInSlot)) return;

        // Get a fresh copy of the Model we bought
        GameObject slotPrefab = clickedSlot.GetCardPrefab();
        Assert.IsNotNull(slotPrefab);
        Model slotModel = slotPrefab.GetComponent<Model>();
        Assert.IsNotNull(slotModel);

        PlacementController.StartPlacingObject(slotModel);
        EconomyController.Withdraw(ModelType.DEW, clickedSlot.Buy(EconomyController.GetBalance(ModelType.DEW)));
        UpdateShopCardLighting();

        // The ControllerManager handles upgrading and combination logic
        OnBuyModel?.Invoke(slotModel);
        numCardsPurchased++;

        bool allSlotsEmpty = true;
        foreach (ShopSlot shopSlot in shopSlots) { if (!shopSlot.Empty()) allSlotsEmpty = false; }
        if (allSlotsEmpty) Reroll(true);
    }

    #endregion
}
