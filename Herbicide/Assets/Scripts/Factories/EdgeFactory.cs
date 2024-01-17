using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Manages assets for Edge components. With a factory,
/// we save tremendous amounts of memory -- each Edge object
/// can access its needed Sprite here rather than instantiating its own
/// array of possible Sprites.
/// </summary>
public class EdgeFactory : MonoBehaviour
{
    /// <summary>
    /// Reference to the EdgeFactory singleton.
    /// </summary>
    private static EdgeFactory instance;

    /// <summary>
    /// The largest index of an Edge Sprite.
    /// </summary>
    private const int MAX_INDEX = 22;

    /// <summary>
    /// Array of Sprites for Shore Edges
    /// </summary>
    [SerializeField]
    private Sprite[] shoreEdgeSprites;

    /// <summary>
    /// Finds and sets the EdgeFactory singleton.
    /// </summary>
    /// <param name="levelController">The LevelController singleton.</param>
    public static void SetSingleton(LevelController levelController)
    {
        if (levelController == null) return;
        if (instance != null) return;

        EdgeFactory[] edgeFactories = FindObjectsOfType<EdgeFactory>();
        Assert.IsNotNull(edgeFactories, "Array of EdgeFactories is null.");
        Assert.AreEqual(1, edgeFactories.Length);
        instance = edgeFactories[0];
    }

    /// <summary>
    /// Returns true if an index is within the bounds for an Edge index.
    /// </summary>
    /// <param name="index">the index to check</param>
    /// <returns>true if an index is within the bounds for an Edge index;
    /// otherwise, false.</returns>
    public static bool ValidEdgeIndex(int index)
    {
        return index >= 0 && index <= MAX_INDEX;
    }

    /// <summary>
    /// Returns the correct Sprite asset for an Edge object based on an index.
    /// </summary>
    /// <param name="type">the type/name of the Edge</param>
    /// <param name="index">the Sprite index </param>
    public static Sprite GetEdgeSprite(string type, int index)
    {
        Assert.IsTrue(ValidEdgeIndex(index));

        if (type.ToLower() == "shore") return instance.shoreEdgeSprites[index];

        return null;
    }
}
