using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Manages assets for Defenders.
/// </summary>
public class DefenderFactory : MonoBehaviour
{
    /// <summary>
    /// Reference to the DefenderFactory singleton.
    /// </summary>
    private static DefenderFactory instance;

    /// <summary>
    /// Holds all Defender inventory sprites. They are indexed by
    /// their Type enum: <br></br>
    /// 
    /// 0 --> SQUIRREL
    /// </summary>
    [SerializeField]
    private Sprite[] inventorySprites;

    /// <summary>
    /// Holds all placed Defender sprites. They are indexed by
    /// their Type enum: <br></br>
    /// 
    /// 0 --> SQUIRREL
    /// </summary>
    [SerializeField]
    private Sprite[] placedSprites;

    /// <summary>
    /// Holds all placed Defender prefabs. They are indexed by
    /// their Type enum: <br></br>
    /// 
    /// 0 --> SQUIRREL
    /// </summary>
    [SerializeField]
    private GameObject[] prefabs;


    /// <summary>
    /// Finds and sets the DefenderFactory singleton.
    /// </summary>
    /// <param name="levelController">The LevelController singleton.</param>
    public static void SetSingleton(LevelController levelController)
    {
        if (levelController == null) return;
        if (instance != null) return;

        DefenderFactory[] defenderFactories = FindObjectsOfType<DefenderFactory>();
        Assert.IsNotNull(defenderFactories, "Array of DefenderFactories is null.");
        Assert.AreEqual(1, defenderFactories.Length);
        instance = defenderFactories[0];
    }

    /// <summary>
    /// Returns the Sprite component that represents a Defender in the inventory.
    /// </summary>
    /// <param name="type">the type of Defender</param>
    /// <returns>the Sprite component that represents a Defender in the inventory</returns>
    public static Sprite GetDefenderInventorySprite(Defender.DefenderType type)
    {
        int index = (int)type;
        if (index < 0 || index >= instance.inventorySprites.Length) return null;

        return instance.inventorySprites[index];
    }

    /// <summary>
    /// Returns the Sprite component that represents a placed Defender.
    /// </summary>
    /// <param name="type">the type of Defender</param>
    /// <returns>the Sprite component that represents a placed Defender</returns>
    public static Sprite GetDefenderPlacedSprite(Defender.DefenderType type)
    {
        int index = (int)type;
        if (index < 0 || index >= instance.placedSprites.Length) return null;

        return instance.placedSprites[index];
    }

    /// <summary>
    /// Returns the GameObject prefab that represents a placed Defender.
    /// </summary>
    /// <param name="type">the type of Defender</param>
    /// <returns>the GameObject prefab that represents a placed Defender</returns>
    public static GameObject GetDefenderPrefab(Defender.DefenderType type)
    {
        int index = (int)type;
        if (index < 0 || index >= instance.prefabs.Length) return null;

        return instance.prefabs[index];
    }
}
