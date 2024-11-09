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
    /// The currently active shop.
    /// </summary>
    private ShopController activeShop;

    /// <summary>
    /// The delegate to subscribe to the BuyModelDelegate.
    /// </summary>
    private BuyModelDelegate handlerToSubscribe;

    /// <summary>
    /// true if the reroll feature is unlocked; false otherwise.
    /// </summary>
    private bool isRerollUnlocked;

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
        SaveLoadManager.SubscribeToToSaveEvent(instance.SaveShopData);
    }

    /// <summary>
    /// Unlocks the reroll feature.
    /// </summary>
    public static void UnlockReroll()
    {
        instance.isRerollUnlocked = true;
        instance.activeShop.SetRerollEnabled(true);
    }

    /// <summary>
    /// Loads the shop data.
    /// </summary>
    private void LoadShopData()
    {
        int level = SaveLoadManager.GetLoadedGameLevel();

        ShopSaveData shopSaveData = SaveLoadManager.LoadShop(instance);
        if(shopSaveData == null) shopSaveData = new ShopSaveData();
        isRerollUnlocked = shopSaveData.isRerollUnlocked;

        List<ModelType> starterModels = new List<ModelType>();
        shopTwoSlots.gameObject.SetActive(true);
        activeShop = shopTwoSlots;
        int slotsToInitialize;
        switch (level)
        {
            case 0:
                slotsToInitialize = 2;
                break;
            case 1:
                slotsToInitialize = 3;
                break;
            case 2: 
                slotsToInitialize = 4;
                starterModels.Add(ModelType.BUNNY);
                starterModels.Add(ModelType.SQUIRREL);
                break;
            case 3:
                slotsToInitialize = 5;
                shopTwoSlots.gameObject.SetActive(true);
                activeShop = shopTwoSlots;
                starterModels.Add(ModelType.BUNNY);
                starterModels.Add(ModelType.SQUIRREL);
                break;
            case 4:
                slotsToInitialize = 5;
                starterModels.Add(ModelType.BUNNY);
                starterModels.Add(ModelType.SQUIRREL);
                starterModels.Add(ModelType.OWL);
                break;
            default:
                slotsToInitialize = 5;
                break;
        }

        activeShop.InitializeShop(instance, starterModels, slotsToInitialize);
        activeShop.SubscribeToBuyDefenderDelegate(handlerToSubscribe);
        if (isRerollUnlocked) activeShop.SetRerollEnabled(true);
    }

    /// <summary>
    /// Saves the shop data.
    /// </summary>
    private void SaveShopData() 
    {
        ShopSaveData shopSaveData = new ShopSaveData();
        shopSaveData.isRerollUnlocked = isRerollUnlocked;
        SaveLoadManager.SaveShop(instance, shopSaveData);
    }
    
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
