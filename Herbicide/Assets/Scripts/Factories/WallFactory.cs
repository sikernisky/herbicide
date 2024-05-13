using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Manages assets for Wall components. With a factory,
/// we save tremendous amounts of memory -- each Wall object
/// can access its needed Sprite here rather than instantiating its own
/// array of possible Sprites.
/// </summary>
public class WallFactory : Factory
{
    /// <summary>
    /// Reference to the WallFactory singleton.
    /// </summary>
    private static WallFactory instance;

    /// <summary>
    /// The largest index of a Wall Sprite.
    /// </summary>
    private const int MAX_INDEX = 15;

    /// <summary>
    /// Array of Sprites for Soil Flooring
    /// </summary>
    [SerializeField]
    private Sprite[] stoneWallSprites;


    /// <summary>
    /// Finds and sets the WallFactory singleton.
    /// </summary>
    /// <param name="levelController">The LevelController singleton.</param>
    public static void SetSingleton(LevelController levelController)
    {
        if (levelController == null) return;
        if (instance != null) return;

        WallFactory[] wallFactories = FindObjectsOfType<WallFactory>();
        Assert.IsNotNull(wallFactories, "Array of WallFactories is null.");
        Assert.AreEqual(1, wallFactories.Length);
        instance = wallFactories[0];
        instance.SpawnPools();
    }

    /// <summary>
    /// Returns true if an index is within the bounds for a Wall index.
    /// </summary>
    /// <param name="index">the index to check</param>
    /// <returns>true if an index is within the bounds for a Wall index;
    /// otherwise, false.</returns>
    public static bool ValidWallIndex(int index)
    {
        return index >= 0 && index <= MAX_INDEX;
    }

    /// <summary>
    /// Returns the correct Sprite asset for a Wall object based on an index.
    /// </summary>
    /// <param name="type">the type of Wall</param>
    /// <param name="index">the Sprite index </param>
    public static Sprite GetWallSprite(ModelType type, int index)
    {
        Assert.IsTrue(ValidWallIndex(index));

        if (type == ModelType.STONE_WALL) return instance.stoneWallSprites[index];

        return null;
    }

    /// <summary>
    /// Returns a fresh Wall prefab from the object pool.
    /// </summary>
    /// <param name="modelType">The type of Wall prefab to get. </param>
    /// <returns>a GameObject with a Wall component attached to it</returns>
    public static GameObject GetWallPrefab(ModelType modelType)
    {
        HashSet<ModelType> validModels = new HashSet<ModelType>()
        {
            ModelType.STONE_WALL
        };
        Assert.IsTrue(validModels.Contains(modelType));
        return instance.RequestObject(ModelType.STONE_WALL);
    }

    /// <summary>
    /// Accepts a Wall prefab that the caller no longer needs. Adds it back
    /// to the object pool.
    /// </summary>
    /// <param name="prefab">The Wall prefab to return.</param>
    public static void ReturnWallPrefab(GameObject prefab)
    {
        HashSet<ModelType> validModels = new HashSet<ModelType>()
        {
            ModelType.STONE_WALL
        };
        Assert.IsTrue(validModels.Contains(prefab.GetComponent<Model>().TYPE));
        instance.ReturnObject(prefab);
    }

    /// <summary>
    /// Returns the Transform component of the WallFactory instance.
    /// </summary>
    /// <returns>the Transform component of the WallFactory instance.</returns>
    protected override Transform GetTransform() { return instance.transform; }
}
