using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Produces assets related to Flooring. 
/// </summary>
public class FlooringFactory : Factory
{
    #region Fields

    /// <summary>
    /// Reference to the FlooringFactory singleton.
    /// </summary>
    private static FlooringFactory instance;

    /// <summary>
    /// The largest index of a Flooring Sprite.
    /// </summary>
    private const int MAX_INDEX = 15;

    /// <summary>
    /// Array of Sprites for Soil Flooring
    /// </summary>
    [SerializeField]
    private Sprite[] soilFlooringSprites;

    #endregion

    #region Methods

    /// <summary>
    /// Finds and sets the FlooringFactory singleton.
    /// </summary>
    /// <param name="levelController">The LevelController singleton.</param>
    public static void SetSingleton(LevelController levelController)
    {
        if (levelController == null) return;
        if (instance != null) return;

        FlooringFactory[] flooringFactories = FindObjectsOfType<FlooringFactory>();
        Assert.IsNotNull(flooringFactories, "Array of FlooringFactories is null.");
        Assert.AreEqual(1, flooringFactories.Length);
        instance = flooringFactories[0];
        instance.SpawnPools();
    }

    /// <summary>
    /// Returns true if an index is within the bounds for a Flooring index.
    /// </summary>
    /// <param name="index">the index to check</param>
    /// <returns>true if an index is within the bounds for a Flooring index;
    /// otherwise, false.</returns>
    public static bool ValidFlooringIndex(int index)
    {
        return index >= 0 && index <= MAX_INDEX;
    }

    /// <summary>
    /// Returns the correct Sprite asset for a Flooring object based on an index.
    /// </summary>
    /// <param name="type">the type of Flooring Sprite</param>
    /// <param name="index">the Sprite index </param>
    public static Sprite GetFlooringSprite(ModelType type, int index)
    {
        Assert.IsTrue(ValidFlooringIndex(index));

        if (type == ModelType.SOIL_FLOORING) return instance.soilFlooringSprites[index];

        return null;
    }

    /// <summary>
    /// Returns a fresh Flooring prefab from the object pool.
    /// </summary>
    /// <param name="modelType">The type of Flooring prefab to get. </param>
    /// <returns>a GameObject with a Flooring component attached to it</returns>
    public static GameObject GetFlooringPrefab(ModelType modelType)
    {
        HashSet<ModelType> validModels = new HashSet<ModelType>()
        {
            ModelType.SOIL_FLOORING
        };
        Assert.IsTrue(validModels.Contains(modelType));
        return instance.RequestObject(ModelType.SOIL_FLOORING);
    }

    /// <summary>
    /// Accepts a Flooring prefab that the caller no longer needs. Adds it back
    /// to the object pool.
    /// </summary>
    /// <param name="prefab">The Flooring prefab to return.</param>
    public static void ReturnFlooringPrefab(GameObject prefab)
    {
        HashSet<ModelType> validModels = new HashSet<ModelType>()
        {
            ModelType.SOIL_FLOORING
        };
        Assert.IsTrue(validModels.Contains(prefab.GetComponent<Model>().TYPE));
        instance.ReturnObject(prefab);
    }

    /// <summary>
    /// Returns the Transform component of the FlooringFactory instance.
    /// </summary>
    /// <returns>the Transform component of the FlooringFactory instance.</returns>
    protected override Transform GetTransform() { return instance.transform; }

    #endregion
}