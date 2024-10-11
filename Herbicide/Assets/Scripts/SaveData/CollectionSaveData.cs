using System.Collections.Generic;

/// <summary>
/// Stores data about the Collection.
/// </summary>
[System.Serializable]
public class CollectionSaveData
{
    /// <summary>
    /// The Models that the player has unlocked and the
    /// progress they have made on each Model.
    /// </summary>
    public List<ModelUpgradeSaveData> modelSaveData;

    /// <summary>
    /// true if the ability to combine Defenders is unlocked; false otherwise.
    /// </summary>
    public bool isCombinationUnlocked;
}
