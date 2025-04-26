using Requirements = ModelUpgradeRequirements.ModelUpgradeRequirementsData;

/// <summary>
/// Stores data about a Model.
/// </summary>
[System.Serializable]
public class ModelSaveData
{
    #region General Data

    /// <summary>
    /// The type of Model being saved.
    /// </summary>
    private ModelType modelType;

    #endregion

    #region Upgrade Data

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
    private Requirements requirements;

    #endregion

    #region Methods

    /// <summary>
    /// Creates a new ModelSaveData object.
    /// </summary>
    /// <param name="modelType">the type of Model to upgrade. </param>
    /// <param name="requirements">the requirements for the Model being upgraded.</param>
    public ModelSaveData(ModelType modelType, Requirements requirements)
    { 
        this.modelType = modelType; 
        this.requirements = requirements;
    }

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
        if(!requirements.ValidLevel(newLevel)) return;
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
        int maxProgressForCurrentLevel = requirements.GetPointRequirementsByLevel(level);
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
