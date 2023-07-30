using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


/// <summary>
/// Manages assets for Tree components.
/// </summary>
public class TreeFactory : MonoBehaviour
{
    /// <summary>
    /// Reference to the TreeFactory singleton.
    /// </summary>
    private static TreeFactory instance;

    /// <summary>
    /// Holds all Tree inventory sprites. They are indexed by
    /// their Type enum: <br></br>
    /// 
    /// 0 --> BASIC
    /// </summary>
    [SerializeField]
    private Sprite[] inventorySprites;

    /// <summary>
    /// Holds all placed Tree sprites. They are indexed by
    /// their Type enum: <br></br>
    /// 
    /// 0 --> BASIC
    /// </summary>
    [SerializeField]
    private Sprite[] placedSprites;

    /// <summary>
    /// Holds all placed Tree prefabs. They are indexed by
    /// their Type enum: <br></br>
    /// 
    /// 0 --> BASIC
    /// </summary>
    [SerializeField]
    private GameObject[] prefabs;


    /// <summary>
    /// Finds and sets the TreeFactory singleton.
    /// </summary>
    /// <param name="levelController">The LevelController singleton.</param>
    public static void SetSingleton(LevelController levelController)
    {
        if (levelController == null) return;
        if (instance != null) return;

        TreeFactory[] treeFactories = FindObjectsOfType<TreeFactory>();
        Assert.IsNotNull(treeFactories, "Array of TreeFactories is null.");
        Assert.AreEqual(1, treeFactories.Length);
        instance = treeFactories[0];
    }

    /// <summary>
    /// Returns the Sprite component that represents a Tree in the inventory.
    /// </summary>
    /// <param name="type">the type of Tree</param>
    /// <returns>the Sprite component that represents a Tree in the inventory</returns>
    public static Sprite GetTreeInventorySprite(Tree.TreeType type)
    {
        int index = (int)type;
        if (index < 0 || index >= instance.inventorySprites.Length) return null;

        return instance.inventorySprites[index];
    }

    /// <summary>
    /// Returns the Sprite component that represents a placed Tree.
    /// </summary>
    /// <param name="type">the type of Tree</param>
    /// <returns>the Sprite component that represents a placed Tree</returns>
    public static Sprite GetTreePlacedSprite(Tree.TreeType type)
    {
        int index = (int)type;
        if (index < 0 || index >= instance.placedSprites.Length) return null;

        return instance.placedSprites[index];
    }

    /// <summary>
    /// Returns the GameObject prefab that represents a placed Tree.
    /// </summary>
    /// <param name="type">the type of Tree</param>
    /// <returns>the GameObject prefab that represents a placed Tree</returns>
    public static GameObject GetTreePrefab(Tree.TreeType type)
    {
        int index = (int)type;
        if (index < 0 || index >= instance.prefabs.Length) return null;

        return instance.prefabs[index];
    }

}

