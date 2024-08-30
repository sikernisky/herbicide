using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Produces assets related to Edge sprites.
/// </summary>
public class EdgeFactory : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// Reference to the EdgeFactory singleton.
    /// </summary>
    private static EdgeFactory instance;

    /// <summary>
    /// The largest index of an Edge Sprite.
    /// </summary>
    private const int MAX_INDEX = 32;

    /// <summary>
    /// Array of Sprites for Shore Edges
    /// </summary>
    [SerializeField]
    private Sprite[] shoreEdgeSprites;

    #endregion

    #region Methods

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
    public static bool ValidEdgeIndex(int index) => index >= 0 && index <= MAX_INDEX;

    /// <summary>
    /// Returns the correct Sprite asset for an Edge object based on an index.
    /// </summary>
    /// <param name="type">the type/name of the Edge</param>
    /// <param name="index">the Sprite index </param>
    public static Sprite GetEdgeSprite(string type, int index)
    {
        Assert.IsTrue(ValidEdgeIndex(index), index + " is not valid.");

        if (type.ToLower() == "shore") return instance.shoreEdgeSprites[index];

        return null;
    }

    #endregion
}
