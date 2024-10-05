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
    /// The types of ShopCards the Shop can display and
    /// the number of that type remaining. 
    /// </summary>
    private Dictionary<ModelType, int> cardPool;

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

        // Lighten / Darken shop cards depending on if player can afford
        foreach (ShopSlot shopSlot in shopSlots)
        {
            if (shopSlot.Empty()) continue;
            if (!shopSlot.CanBuy(EconomyController.GetBalance(ModelType.DEW))) shopSlot.DarkenSlot();
            else shopSlot.LightenSlot();
        }

        // Update reroll timer
        float timePercentage = timeSinceLastReroll / AUTOMATIC_REROLL_TIME;
        timerBarFill.transform.localScale = new Vector3(timePercentage, 1, 1);
        if (timeSinceLastReroll <= 0)
        {
            Reroll(true);
        }
        else timeSinceLastReroll -= Time.deltaTime;
    }

    /// <summary>
    /// Loads the shop with the types of ShopCards it can sell.
    /// </summary>
    /// <param name="shopManager">the ShopManager singleton</param>
    /// the Shop can sell. If null, the Shop can sell all types.</param>
    public void LoadShop(ShopManager shopManager)
    {
        if (shopManager == null) return;
        if (shopActive) return;

        if (!shopLoaded)
        {
            PlacementController.SubscribeToFinishPlacingDelegate(OnFinishPlacing);
            cardPool = new Dictionary<ModelType, int>();
            int slotCounter = 0;
            foreach (ShopSlot shopSlot in shopSlots)
            {
                shopSlot.SetupSlot(slotCounter);
                slotCounter++;
            }

            HashSet<ModelType> shopCardsToLoad = new HashSet<ModelType>();
            foreach(ModelType unlockedModelType in CollectionManager.GetAllUnlockedModelTypes())
            {
                shopCardsToLoad.Add(unlockedModelType);
            }

            Assert.IsNotNull(shopCardsToLoad);
            Assert.IsTrue(shopCardsToLoad.Count > 0);


            foreach (ModelType modelType in shopCardsToLoad)
            {
                cardPool.Add(modelType, 5); //5 is placeholder.
            }

            Reroll(true);
            timeSinceLastReroll = AUTOMATIC_REROLL_TIME;
        }
        else shopSlots.ForEach(ss => ss.EnableSlot());

        shopLoaded = true;
        shopActive = true;
    }

    /// <summary>
    /// Rerolls the shop, replacing every ShopSlot with a new ShopCard
    /// from the ShopManager's card pool.
    /// </summary>
    /// <param name="free">true if this reroll won't charge the player;
    /// false if it will. </param>
    private void Reroll(bool free)
    {
        if (!free)
        {
            bool canReroll = EconomyController.GetBalance(ModelType.DEW) >= REROLL_COST;
            if (!canReroll) return;
            EconomyController.Withdraw(ModelType.DEW, REROLL_COST);
        }

        foreach (ShopSlot cardSlot in shopSlots)
        {
            Assert.IsNotNull(cardSlot);
            int randomIndex = Random.Range(0, cardPool.Count);
            var randomKey = cardPool.Keys.ElementAt(randomIndex);
            ModelType randomType = randomKey;
            GameObject shopCardPrefab = ShopFactory.GetShopCardPrefab(randomType);
            Assert.IsNotNull(shopCardPrefab);
            ShopCard shopCardComp = shopCardPrefab.GetComponent<ShopCard>();
            Assert.IsNotNull(shopCardComp);
            cardSlot.Fill(shopCardComp);
        }

        timeSinceLastReroll = AUTOMATIC_REROLL_TIME;
        shopActive = true;
    }

    /// <summary>
    /// Called when the player finishes placing a Model.
    /// </summary>
    private void OnFinishPlacing(Model m) => Assert.IsNotNull(m, "Placed model is null.");

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
        if(PlacementController.IsCombining()) return;
        if (!clickedSlot.CanBuy(EconomyController.GetBalance(ModelType.DEW))) return;

        // Get a fresh copy of the Model we bought
        GameObject slotPrefab = clickedSlot.GetCardPrefab();
        Assert.IsNotNull(slotPrefab);
        Model slotModel = slotPrefab.GetComponent<Model>();
        Assert.IsNotNull(slotModel);

        PlacementController.StartPlacingObject(slotModel);
        EconomyController.Withdraw(ModelType.DEW, clickedSlot.Buy(EconomyController.GetBalance(ModelType.DEW)));

        // The ControllerController handles upgrading and combination logic
        OnBuyModel?.Invoke(slotModel);

        bool allSlotsEmpty = true;
        foreach (ShopSlot shopSlot in shopSlots) { if (!shopSlot.Empty()) allSlotsEmpty = false; }
        if (allSlotsEmpty) Reroll(true);
    }

    /// <summary>
    /// Subscribes a handler (the ControllerController) to the request upgrade event.
    /// </summary>
    /// <param name="handler">The handler to subscribe.</param>
    public void SubscribeToBuyDefenderDelegate(BuyModelDelegate handler)
    {
        Assert.IsNotNull(handler, "Handler is null.");
        OnBuyModel += handler;
    }

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

    #endregion
}
