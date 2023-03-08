using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Manages assets for Flooring components. With a factory,
/// we save tremendous amounts of memory -- each Flooring object
/// can access its needed Sprite here rather than instantiating its own
/// array of possible Sprites.
/// </summary>
public class FlooringFactory : MonoBehaviour
{
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
    public static Sprite GetFlooringSprite(TileGrid.FlooringType type, int index)
    {
        Assert.IsTrue(ValidFlooringIndex(index));

        if (type == TileGrid.FlooringType.SOIL) return instance.soilFlooringSprites[index];

        return null;
    }
}
