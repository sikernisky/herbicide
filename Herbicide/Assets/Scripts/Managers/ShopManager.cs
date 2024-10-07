using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using static ShopController;

/// <summary>
/// Sets up and manages the Shop. 
/// </summary>
public class ShopManager : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// Reference to the ShopManager singleton.
    /// </summary>
    private static ShopManager instance;

    /// <summary>
    /// Prefab of the shop when it has two slots.
    /// </summary>
    [SerializeField]
    private ShopController shopTwoSlots;

    /// <summary>
    /// Prefab of the shop when it has three slots.
    /// </summary>
    [SerializeField]
    private ShopController shopThreeSlots;

    /// <summary>
    /// Prefab of the shop when it has four slots.
    /// </summary>
    [SerializeField]
    private ShopController shopFourSlots;

    /// <summary>
    /// The currently active shop.
    /// </summary>
    private ShopController activeShop;

    /// <summary>
    /// The delegate to subscribe to the BuyModelDelegate.
    /// </summary>
    private BuyModelDelegate handlerToSubscribe;

    #endregion

    #region Methods

    /// <summary>
    /// Finds and sets the ShopManager singleton for a level.
    /// </summary>
    /// <param name="levelController">The LevelController singleton.</param>
    public static void SetSingleton(LevelController levelController)
    {
        if (levelController == null) return;

        ShopManager[] shopManagers = FindObjectsOfType<ShopManager>();
        Assert.IsNotNull(shopManagers, "Array of ShopManagers is null.");
        Assert.AreEqual(1, shopManagers.Length);
        instance = shopManagers[0];
    }

    /// <summary>
    /// Subscribes to the SaveLoadManager's OnLoadRequested and OnSaveRequested events.
    /// </summary>
    /// <param name="levelController">The LevelController singleton.</param>
    public static void SubscribeToSaveLoadEvents(LevelController levelController)
    {
        Assert.IsNotNull(levelController, "LevelController is null.");

        SaveLoadManager.SubscribeToToLoadEvent(instance.LoadShopData);
        SaveLoadManager.SubscribeToToLoadEvent(instance.SaveShopData);
    }

    /// <summary>
    /// Loads the shop data.
    /// </summary>
    private void LoadShopData()
    {
        int level = SaveLoadManager.GetLoadedGameLevel();
        bool rerollUnlocked = SaveLoadManager.GetLoadedIsRerollUnlocked();
        Debug.Log(rerollUnlocked);

        switch (level)
        {
            case 0:
                shopTwoSlots.gameObject.SetActive(true);
                activeShop = shopTwoSlots;
                break;
            case 1:
                shopTwoSlots.gameObject.SetActive(true);
                activeShop = shopTwoSlots;
                break;
            case 2:
                shopThreeSlots.gameObject.SetActive(true);
                activeShop = shopThreeSlots;
                break;
            default:
                shopFourSlots.gameObject.SetActive(true);
                activeShop = shopFourSlots;
                break;
        }

        activeShop.InitializeShop(instance);
        activeShop.SubscribeToBuyDefenderDelegate(handlerToSubscribe);
        if (rerollUnlocked) activeShop.SetRerollButtonActive(true);
    }

    /// <summary>
    /// Saves the shop data.
    /// </summary>
    private void SaveShopData() { }
    
    /// <summary>
    /// Updates the shop manager.
    /// </summary>
    /// <param name="gameState">The most recent game state. </param>
    public static void UpdateShopManager(GameState gameState)
    {
        instance.activeShop.UpdateShop(gameState);
    }

    /// <summary>
    /// Subscribes the active shop to the BuyDefenderDelegate.
    /// </summary>
    /// <param name="handler">the event handler</param>
    public static void SubscribeToOnPurchaseShopCardDelegate(BuyModelDelegate handler)
    {
        instance.handlerToSubscribe = handler;
    }

    #endregion
}
