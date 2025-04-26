using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Produces assets related to Tiles.
/// </summary>
public class TileFactory : Factory
{
    #region Fields

    /// <summary>
    /// Reference to the TileFactory singleton.
    /// </summary>
    private static TileFactory instance;

    /// <summary>
    /// The largest index possible in a Grass Tile sprite array.
    /// </summary>
    private const int MAX_GRASS_INDEX = 1;

    /// <summary>
    /// Array of Sprites for Grass Tiles
    /// </summary>
    [SerializeField]
    private Sprite[] grassTileSprites;

    #endregion

    #region Methods

    /// <summary>
    /// Finds and sets the TileFactory singleton.
    /// </summary>
    /// <param name="levelController">The LevelController singleton.</param>
    public static void SetSingleton(LevelController levelController)
    {
        if (levelController == null) return;
        if (instance != null) return;

        TileFactory[] tileFactories = FindObjectsOfType<TileFactory>();
        Assert.IsNotNull(tileFactories, "Array of TileFactories is null.");
        Assert.AreEqual(1, tileFactories.Length);
        instance = tileFactories[0];
        instance.SpawnPools();
    }

    /// <summary>
    /// Returns a fresh Tile prefab for a given type from its object pool.
    /// </summary>
    /// <param name="modelType">The type of Tile model to get.</param>
    /// <returns>a GameObject with a Tile component attached to it</returns>
    public static GameObject GetTilePrefab(ModelType modelType) => instance.RequestObject(modelType);

    /// <summary>
    /// Accepts a Tile prefab that the caller no longer needs. Adds it back
    /// to its object pool.
    /// </summary>
    /// <param name="prefab">The Tile prefab to return.</param>
    public static void ReturnTilePrefab(GameObject prefab) => instance.ReturnObject(prefab);

    /// <summary>
    /// Returns the TileFactory instance's Transform component.
    /// </summary>
    /// <returns></returns>
    protected override Transform GetTransform() { return instance.transform; }

    /// <summary>
    /// Returns true if an index is within the bounds for an Tile index of
    /// a given type.
    /// </summary>
    /// <param name="type">the type of Tile</param>
    /// <param name="index">the index to check</param>
    /// <returns>true if an index is within the bounds for a Tile index;
    /// otherwise, false.</returns>
    public static bool ValidTileIndex(ModelType type, int index)
    {
        if (index < 0) return false;
        if (type == ModelType.GRASS_TILE) return index <= MAX_GRASS_INDEX;
        throw new System.Exception("Invalid type " + type + ".");
    }

    /// <summary>
    /// Returns the correct Sprite asset for a Tile object based on an index.
    /// </summary>
    /// <param name="type">the type/name of the Tile</param>
    /// <param name="index">the Sprite index </param>
    public static Sprite GetTileSprite(ModelType type, int index)
    {
        Assert.IsTrue(ValidTileIndex(type, index));

        if (type == ModelType.GRASS_TILE) return instance.grassTileSprites[index];

        return null;
    }

    #endregion
}
