using Requirements = ModelUpgradeRequirements.ModelUpgradeRequirementsData;

/// <summary>
/// Stores data about the upgrade progress of a Model.
/// </summary>
[System.Serializable]
public class ModelUpgradeSaveData
{
    #region Data

    /// <summary>
    /// The type of Model being upgraded.
    /// </summary>
    private ModelType modelType;

    /// <summary>
    /// The level of the Model, starting from 0.
    /// </summary>
    private int level;

    /// <summary>
    /// Current progress of the Model upgrade on the current level.
    /// </summary>
    private int currentProgress;

    /// <summary>
    /// The requirements for the Model being upgraded.
    /// </summary>
    private Requirements requirementsForModel => CollectionManager.GetUpgradeRequirementsData(modelType);

    #endregion

    #region Methods

    /// <summary>
    /// Creates a new ModelUpgradeSaveData object.
    /// </summary>
    /// <param name="modelType">the type of Model to upgrade. </param>
    public ModelUpgradeSaveData(ModelType modelType) { this.modelType = modelType; }

    /// <summary>
    /// Returns the type of Model being upgraded.
    /// </summary>
    /// <returns>the type of Model being upgraded.</returns>
    public ModelType GetModelType() => modelType;

    /// <summary>
    /// Sets the level of the Model being upgraded.
    /// </summary>
    /// <param name="newLevel">the level to set to.</param>
    public void SetLevel(int newLevel)
    {
        if(!requirementsForModel.ValidLevel(newLevel)) return;
        level = newLevel;
    }

    /// <summary>
    /// Returns the level of the Model being upgraded.
    /// </summary>
    /// <returns>the level of the Model being upgraded. </returns>
    public int GetLevel() => level;

    /// <summary>
    /// Sets the current progress of the Model upgrade on the current level.
    /// </summary>
    /// <param name="newCurrentProgress">the current progress of the Model upgrade on the current level.</param>
    public void SetCurrentProgress(int newCurrentProgress)
    {
        if(newCurrentProgress < 0) return;
        int maxProgressForCurrentLevel = requirementsForModel.GetPointRequirementsByLevel(level);
        if(newCurrentProgress > maxProgressForCurrentLevel) return;
        currentProgress = newCurrentProgress;
    }

    /// <summary>
    /// Returns the current progress of the Model upgrade on the current level.
    /// </summary>
    /// <returns> the current progress of the Model upgrade on the current level.</returns>
    public int GetCurrentProgress() => currentProgress;

    #endregion
}
