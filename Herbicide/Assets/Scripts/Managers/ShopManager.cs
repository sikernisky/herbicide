using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

/// <summary>
/// Manages the Shop: decides when to spawn ShopBoats and which
/// Models will be on them, interacts with EconomyController for
/// purchasing, interacts with PlacementController for placing.
/// </summary>
public class ShopManager : MonoBehaviour
{
    /// <summary>
    /// Reference to the ShopManager singleton.
    /// </summary>
    private static ShopManager instance;

    /// <summary>
    /// The most recent GameState.
    /// </summary>
    private static GameState gameState;

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
    private static readonly int REROLL_COST = 50;

    /// <summary>
    /// Defines a delegate for buying a Model from the shop.
    /// </summary>
    /// <param name="modelType">The Model that was bought.</param>
    public delegate void BuyDefenderDelegate(Model purchasedModel);

    /// <summary>
    /// Event triggered when a Model is bought from the Shop.
    /// </summary>
    public event BuyDefenderDelegate OnBuyModel;

    /// <summary>
    /// true if the shop has been loaded; false otherwise.
    /// </summary>
    private bool shopLoaded;

    /// <summary>
    /// true if the shop is active; false if the upgrades are active.
    /// </summary>
    private bool shopActive;



    /// <summary>
    /// Finds and sets the ShopManager singleton.
    /// </summary>
    /// <param name="levelController">The LevelController singleton.</param>
    public static void SetSingleton(LevelController levelController)
    {
        if (levelController == null) return;
        if (instance != null) return;

        ShopManager[] shopManagers = FindObjectsOfType<ShopManager>();
        Assert.IsNotNull(shopManagers, "Array of ShopManagers is null.");
        Assert.AreEqual(1, shopManagers.Length);
        instance = shopManagers[0];
        instance.cardPool = new Dictionary<ModelType, int>();

    }
 

    /// <summary>
    /// Main update loop for the ShopManager.
    /// </summary>
    public static void UpdateShop()
    {
        if (gameState != GameState.ONGOING) return;

        if (InputController.DidKeycodeDown(KeyCode.S)) { instance.Reroll(false); } // TEMP

        // Check & Handle ShopCard click
        foreach (ShopSlot shopSlot in instance.shopSlots)
        {
            if (shopSlot.SlotClicked()) instance.ClickShopSlotButton(shopSlot.GetSlotIndex());
        }

        // Enable / Disable reroll button depending on balance
        if (EconomyController.GetBalance() < REROLL_COST) instance.rerollButton.interactable = false;
        else instance.rerollButton.interactable = true;

        // Lighten / Darken shop cards depending on if player can afford
        foreach (ShopSlot shopSlot in instance.shopSlots)
        {
            if (shopSlot.Empty()) continue;
            if (!shopSlot.CanBuy(EconomyController.GetBalance())) shopSlot.DarkenSlot();
            else shopSlot.LightenSlot();
        }
    }

    /// <summary>
    /// Loads the shop with the types of ShopCards it can sell.
    /// </summary>
    /// <param name="modelTypes">the types of ShopCards 
    /// the Shop can sell. If null, the Shop can sell all types.</param>
    public static void LoadShop(HashSet<ModelType> modelTypes = null)
    {
        if(instance.shopActive) return;

        if (!instance.shopLoaded)
        {
            int slotCounter = 0;
            foreach (ShopSlot shopSlot in instance.shopSlots)
            {
                shopSlot.SetupSlot(shopSlot.GetComponent<RectTransform>().position, slotCounter);
                slotCounter++;
            }

            if (modelTypes == null)
            {
                modelTypes = new HashSet<ModelType>(){
                ModelType.SHOP_CARD_SQUIRREL,
                ModelType.SHOP_CARD_BEAR
            };
            }

            Assert.IsNotNull(modelTypes);
            foreach (ModelType modelType in modelTypes)
            {
                instance.cardPool.Add(modelType, 5); //5 is placeholder.
            }

            instance.Reroll(true);
        }
        else
        {
            instance.shopSlots.ForEach(ss => ss.EnableSlot());
        }


        instance.shopLoaded = true;
        instance.shopActive = true;
    }

    public static void LoadUpgrades()
    {
        if(!instance.shopActive) return;

        instance.shopSlots.ForEach(ss => ss.DisableSlot());


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
            bool canReroll = EconomyController.GetBalance() >= REROLL_COST;
            if (!canReroll) return;
            EconomyController.Withdraw(REROLL_COST);
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
    }

    /// <summary>
    /// Informs the ShopManager of the most recent GameState.
    /// </summary>
    /// <param name="gameState">The most recent GameState.</param>
    public static void InformOfGameState(GameState gameState) { ShopManager.gameState = gameState; }

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

        if (!clickedSlot.CanBuy(EconomyController.GetBalance())) return;
        if (PlacementController.Placing()) return;

        GameObject slotPrefab = clickedSlot.GetCardPrefab();
        Assert.IsNotNull(slotPrefab);
        Model slotModel = slotPrefab.GetComponent<Model>();
        Assert.IsNotNull(slotModel);

        PlacementController.StartPlacingObject(slotModel);
        EconomyController.Withdraw(clickedSlot.Buy(EconomyController.GetBalance()));

        // Let the ControllerController bring this Model to life
        OnBuyModel?.Invoke(slotModel);

        bool allSlotsEmpty = true;
        foreach (ShopSlot shopSlot in shopSlots) { if (!shopSlot.Empty()) allSlotsEmpty = false; }
        if (allSlotsEmpty) Reroll(true);
    }

    /// <summary>
    /// #BUTTON EVENT#
    /// 
    /// Called when the player clicks the Reroll button. Refreshes
    /// the shop if they have enough money.
    /// </summary>
    public void ClickRerollButton() { Reroll(false); }


    /// <summary>
    /// Subscribes a handler (the ControllerController) to the request upgrade event.
    /// </summary>
    /// <param name="handler">The handler to subscribe.</param>
    public static void SubscribeToBuyDefenderDelegate(BuyDefenderDelegate handler)
    {
        Assert.IsNotNull(handler, "Handler is null.");
        instance.OnBuyModel += handler;
    }
}
