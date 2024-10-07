using System;
using UnityEngine;

/// <summary>
/// Stores the requirements for upgrading a model.
/// </summary>
[CreateAssetMenu(fileName = "UpgradeRequirements", menuName = "GameData/UpgradeRequirements")]
public class ModelUpgradeRequirements : ScriptableObject
{
    #region Fields

    /// <summary>
    /// Serializable struct to hold upgrade data for a single model.
    /// </summary>
    [System.Serializable]
    public struct ModelUpgradeRequirementsData
    {
        /// <summary>
        /// The type of model this data is for. This is a string
        /// to prevent shifting in the inspector when we add new models.
        /// </summary>
        [SerializeField]
        private string modelType;

        /// <summary>
        /// The points required, at each level, to upgrade the model.
        /// </summary>
        [SerializeField]
        private int[] pointRequirementsByLevel;


        /// <summary>
        /// Returns the number of points required to upgrade the model at a given level.
        /// If the level is out of bounds, throws an exception.
        /// </summary>
        /// <param name="level">the level to get the point requirements of</param>
        /// <returns>the number of points required to upgrade the model at a given level.</returns>
        public int GetPointRequirementsByLevel(int level)
        {
            if (!ValidLevel(level)) throw new System.Exception("Level out of bounds.");
            return pointRequirementsByLevel[level];
        }

        /// <summary>
        /// Returns the type of model this data is for.
        /// </summary>
        /// <returns>the type of model this data is for.</returns>
        public ModelType GetModelType() => ModelTypeHelper.ConvertStringToModelType(modelType);

        /// <summary>
        /// Returns true if the level is valid; false otherwise.
        /// </summary>
        /// <param name="level">the level to check</param>
        /// <returns>true if the level is valid; false otherwise.</returns>
        public bool ValidLevel(int level) => level >= 0 && level < pointRequirementsByLevel.Length;
    }

    /// <summary>
    /// Holds upgrade data for all models.
    /// </summary>
    [SerializeField]
    private ModelUpgradeRequirementsData[] modelUpgrades;

    #endregion

    #region Methods

    /// <summary>
    /// Returns the upgrade data for a given Model type.
    /// </summary>
    /// <param name="modelType">the given Model type.</param>
    /// <returns>the upgrade data for a given Model type.</returns>
    public ModelUpgradeRequirementsData GetUpgradeData(ModelType modelType)
    {
        foreach (ModelUpgradeRequirementsData data in modelUpgrades)
        {
            if (data.GetModelType() == modelType) return data;
        }

        throw new System.Exception("Model type " + modelType + " not found in upgrade data.");
    }

    #endregion
}
