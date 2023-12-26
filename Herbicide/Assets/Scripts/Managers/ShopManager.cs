using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System.Linq;

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
    /// The ScriptableObject that stores the shop information for
    /// the current level.
    /// </summary>
    [SerializeField]
    private ShopScriptable shopScriptable;

    /// <summary>
    /// Prefab for the ShopBoat.<br></br>
    /// 
    /// NOTE: We are not using a factory because boats are so unique. They
    /// are not defenders nor enemies nor structures -- they are something
    /// else. Therefore, we trade performance for simplicity and isolation.  
    /// /// </summary>
    [SerializeField]
    private GameObject boatPrefab;

    /// <summary>
    /// The current shop.
    /// </summary>
    private List<ShopModel> shop;

    /// <summary>
    /// The number of seconds since the last ShopBoat was spawned.
    /// </summary>
    private float timeSinceLastSpawn;

    /// <summary>
    /// The number of seconds until the next ShopBoat is spawned.
    /// </summary>
    private float nextSpawnGap;


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

    }

    /// <summary>
    /// Main update loop for the ShopManager.
    /// </summary>
    /// <param name="spawnPos">Where ShopBoats spawn. </param>
    public static void UpdateShop(Vector3 spawnPos)
    {
        instance.timeSinceLastSpawn += Time.deltaTime;
        if (instance.timeSinceLastSpawn >= instance.nextSpawnGap)
        {
            instance.timeSinceLastSpawn = 0;
            instance.ResetSpawnGap();

            // Spawn a boat.

            Model rolledRider = instance.RollRider();
            Assert.IsNotNull(rolledRider);

            Assert.IsNotNull(instance.boatPrefab, "Boat prefab is null.");
            GameObject clonedShopBoat = Instantiate(instance.boatPrefab);
            Assert.IsNotNull(clonedShopBoat);
            ShopBoat clonedShopBoatComp = clonedShopBoat.GetComponent<ShopBoat>();
            clonedShopBoatComp.SetWorldPosition(spawnPos);
            clonedShopBoatComp.SetRider(rolledRider);
            ControllerController.MakeController(clonedShopBoatComp);
        }

    }

    /// <summary>
    /// Returns a rider given a list of ShopModels and their statistical probabilities
    /// of spawning.
    /// </summary>
    /// <returns>a Rider this ShopManager should spawn next.</returns>
    private Model RollRider()
    {
        float totalProbability = shop.Sum(model => model.GetSpawnRate());
        Assert.IsTrue(totalProbability == 1f, "Probabilities sum to " + totalProbability + ", not 1.");
        float randomPoint = Random.Range(0f, totalProbability);

        float cumulativeProbability = 0f;
        foreach (ShopModel shopModel in shop)
        {
            cumulativeProbability += shopModel.GetSpawnRate();
            if (randomPoint <= cumulativeProbability)
            {
                return shopModel.GetPlaceablePrefab();
            }
        }

        // Fallback, in case no model is selected
        Assert.IsTrue(false, "No model was selected. Check the spawn rates.");
        return null;
    }

    /// <summary>
    /// Initializes the shop, gathering data about what Models should spawn
    /// and more.
    /// </summary>
    public static void LoadShop()
    {
        Assert.IsNotNull(instance.shopScriptable, "Scriptable is null.");

        instance.shop = instance.shopScriptable.GetShop();
        instance.ResetSpawnGap();
    }

    /// <summary>
    /// Sets the next spawn gap to be a random value within the minimum
    /// and maximum spawn gap values.
    /// </summary>
    private void ResetSpawnGap()
    {
        float maxSpawnGap = shopScriptable.GetMaxSpawnGap();
        float minSpawnGap = shopScriptable.GetMinSpawnGap();
        Assert.IsTrue(maxSpawnGap > 0);
        Assert.IsTrue(minSpawnGap > 0);
        Assert.IsTrue(maxSpawnGap >= minSpawnGap);
        nextSpawnGap = Random.Range(minSpawnGap, maxSpawnGap);
    }
}
