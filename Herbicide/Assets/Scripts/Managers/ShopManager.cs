using UnityEngine;
using UnityEngine.Assertions;

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
    private GameObject shopTwoSlots;

    /// <summary>
    /// Prefab of the shop when it has three slots.
    /// </summary>
    [SerializeField]
    private GameObject shopThreeSlots;

    /// <summary>
    /// Prefab of the shop when it has four slots.
    /// </summary>
    [SerializeField]
    private GameObject shopFourSlots;

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
        instance.LoadShopBasedOnLevel();
    }

    /// <summary>
    /// Loads the correct shop based on the current level.
    /// </summary>
    private void LoadShopBasedOnLevel()
    {
        shopTwoSlots.SetActive(false); 
        shopThreeSlots.SetActive(false);
        shopFourSlots.SetActive(false);

        int level = SaveLoadManager.GetLevel();
        switch (level)
        {
            case 0:
                shopTwoSlots.SetActive(true);
                break;
            case 1:
                shopThreeSlots.SetActive(true);
                break;
            case 2:
                shopFourSlots.SetActive(true);
                break;
            default:
                shopFourSlots.SetActive(true);
                break;
        }
    }

    #endregion
}
