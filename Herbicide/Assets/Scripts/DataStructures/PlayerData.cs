using System.Collections.Generic;

/// <summary>
/// Stores data about the player. We save this data to a file
/// using binary serialization.
/// </summary>
[System.Serializable]
public class PlayerData
{
    #region Data

    /// <summary>
    /// The furthest level the player has reached.
    /// </summary>
    public int furthestLevel;

    /// <summary>
    /// The Models that the player has unlocked and the
    /// progress they have made on each Model.
    /// </summary>
    public List<ModelUpgradeSaveData> modelSaveData;

    #endregion
}
