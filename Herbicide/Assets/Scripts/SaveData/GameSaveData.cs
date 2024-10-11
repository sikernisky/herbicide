/// <summary>
/// Stores data about the game. We save this data to a file
/// using binary serialization.
/// </summary>
[System.Serializable]
public class GameSaveData
{
    #region Data

    /// <summary>
    /// The furthest level the player has reached.
    /// </summary>
    public int furthestLevel;

    /// <summary>
    /// The CollectionSaveData object that stores data about the Collection.
    /// </summary>
    public CollectionSaveData collectionSaveData;

    /// <summary>
    /// The ShopSaveData object that stores data about the Shop.
    /// </summary>
    public ShopSaveData shopSaveData;

    #endregion
}
