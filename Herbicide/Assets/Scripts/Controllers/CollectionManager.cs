using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Requirements = ModelUpgradeRequirements.ModelUpgradeRequirementsData;

/// <summary>
/// Manages the player's collection of Models, including upgrade points
/// and other related data.
/// </summary>
public class CollectionManager : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// Reference to the ModelUpgradeSaveData singleton.
    /// </summary>
    private static CollectionManager instance;

    /// <summary>
    /// The scriptable object containing the upgrade requirements for all Models.
    /// </summary>
    [SerializeField]
    private ModelUpgradeRequirements requirements;

    /// <summary>
    /// The player's collection of Models and their upgrade data.
    /// </summary>
    private List<ModelUpgradeSaveData> modelUpgradeSaveData;

    /// <summary>
    /// Tracks the number of points the player has earned for each ModelType
    /// during the current level.
    /// </summary>
    private Dictionary<ModelType, int> modelUpgradePointsTracker;

    /// <summary>
    /// true if the combination of Defenders is unlocked; otherwise, false.
    /// </summary>
    private bool isCombinationUnlocked;

    #endregion

    #region Methods

    /// <summary>
    /// Finds and sets the CollectionManager singleton.
    /// </summary>
    /// <param name="levelController">The LevelController singleton.</param>
    public static void SetSingleton(LevelController levelController)
    {
        if (levelController == null) return;

        CollectionManager[] collectionManager = FindObjectsOfType<CollectionManager>();
        Assert.IsNotNull(collectionManager, "Array of CollectionManagers is null.");
        Assert.AreEqual(1, collectionManager.Length);
        instance = collectionManager[0];
    }

    /// <summary>
    /// Main update loop for the CollectionManager.
    /// </summary>
    public static void UpdateCollectionManager()
    {
        if (InputController.DidKeycodeDown(KeyCode.U))
        {
            UnlockModel(ModelType.BUNNY);
            UnlockModel(ModelType.RACCOON);
            UnlockCombinations();
            ShopManager.UnlockReroll();
        }
        if (InputController.DidKeycodeDown(KeyCode.B)) UnlockModel(ModelType.BUNNY);
        if (InputController.DidKeycodeDown(KeyCode.A)) UnlockModel(ModelType.RACCOON);
        if (InputController.DidKeycodeDown(KeyCode.C)) UnlockCombinations();
        if (InputController.DidKeycodeDown(KeyCode.R)) ShopManager.UnlockReroll();
        
    }

    /// <summary>
    /// Subscribes to the SaveLoadManager's OnLoadRequested and OnSaveRequested events.
    /// </summary>
    /// <param name="levelController">The LevelController singleton.</param>
    public static void SubscribeToSaveLoadEvents(LevelController levelController)
    {
        Assert.IsNotNull(levelController, "LevelController is null.");

        SaveLoadManager.SubscribeToToLoadEvent(instance.LoadCollectionData);
        SaveLoadManager.SubscribeToToSaveEvent(instance.SaveCollectionData);
    }

    /// <summary>
    /// Returns the upgrade data for a given Model type.
    /// </summary>
    /// <param name="modelType">the given Model type.</param>
    /// <returns>the upgrade data for a given Model type.</returns>
    public static Requirements GetUpgradeRequirementsData(ModelType modelType)
    {
        return instance.requirements.GetUpgradeData(modelType);
    }

    /// <summary>
    /// Returns true if the given ModelType is unlocked.
    /// </summary>
    /// <param name="modelType">the given ModelType to check. </param>
    /// <returns>true if the given ModelType is unlocked; otherwise, false.</returns>
    public static bool IsModelUnlocked(ModelType modelType) => 
        instance.modelUpgradeSaveData.Exists(data => data.GetModelType() == modelType);

    /// <summary>
    /// Adds upgrade points to the given ModelType.
    /// </summary>
    /// <param name="modelType">The type of model on which to add upgrade
    /// points</param>
    /// <param name="points">The number of points to add</param>
    public static void AddModelUpgradePoints(ModelType modelType, int points)
    {
        if (points <= 0) return;
        if (!IsModelUnlocked(modelType)) return;
        Assert.IsTrue(instance.modelUpgradePointsTracker.ContainsKey(modelType));

        instance.modelUpgradePointsTracker[modelType] += points;
    }

    /// <summary>
    /// Removes upgrade points from the given ModelType. These points will be
    /// clamped to zero.
    /// </summary>
    /// <param name="modelType">The type of model from which to remove upgrade
    /// points</param>
    /// <param name="points">the number of points to remove.</param>
    public static void RemoveModelUpgradePoints(ModelType modelType, int points)
    {
        if (points <= 0) return;
        if (!IsModelUnlocked(modelType)) return;
        Assert.IsTrue(instance.modelUpgradePointsTracker.ContainsKey(modelType));

        int newPoints = instance.modelUpgradePointsTracker[modelType] - points;
        int clampedNewPoints = Mathf.Clamp(newPoints, 0, int.MaxValue);
        instance.modelUpgradePointsTracker[modelType] = clampedNewPoints;
    }

    /// <summary>
    /// Returns the number of upgrade points the player has earned for the
    /// given ModelType this level.
    /// </summary>
    /// <param name="modelType">the Model type from which to get the earned points. </param>
    /// <returns>the number of upgrade points the player has earned</returns>
    public static int GetModelUpgradePoints(ModelType modelType)
    {
        if (!IsModelUnlocked(modelType)) return 0;
        Assert.IsTrue(instance.modelUpgradePointsTracker.ContainsKey(modelType));

        return instance.modelUpgradePointsTracker[modelType];
    }

    /// <summary>
    /// Unlocks the given ModelType. If already unlocked, does nothing.
    /// </summary>
    /// <param name="modelToUnlock">The type of Model to unlock. </param>
    public static void UnlockModel(ModelType modelToUnlock) 
    {
        if(IsModelUnlocked(modelToUnlock)) return;
        Assert.IsFalse(instance.modelUpgradeSaveData.Exists(data => data.GetModelType() == modelToUnlock));

        ModelUpgradeSaveData data = new ModelUpgradeSaveData(modelToUnlock);
        instance.modelUpgradeSaveData.Add(data);
        instance.modelUpgradePointsTracker[modelToUnlock] = 0;
    }

    /// <summary>
    /// Returns a list of all unlocked ModelTypes.
    /// </summary>
    /// <returns>a list of all unlocked ModelTypes.</returns>
    public static List<ModelType> GetAllUnlockedModelTypes() => instance.modelUpgradeSaveData.ConvertAll(data => data.GetModelType());

    /// <summary>
    /// Returns the upgrade data for the given unlocked ModelType.
    /// </summary>
    /// <param name="modelType">the ModelType to get data for. </param>
    /// <returns>the upgrade data for the given unlocked ModelType.</returns>
    public static ModelUpgradeSaveData GetUnlockedModelUpgradeSaveData(ModelType modelType)
    {
        Assert.IsTrue(IsModelUnlocked(modelType), modelType + " is not unlocked and has no upgrade data.");
        return instance.modelUpgradeSaveData.Find(data => data.GetModelType() == modelType);
    }

    /// <summary>
    /// Unlocks the ability to combine Defenders.
    /// </summary>
    public static void UnlockCombinations() => instance.isCombinationUnlocked = true;

    /// <summary>
    /// Returns true if the ability to combine Defenders is unlocked.
    /// </summary>
    /// <returns>true if the ability to combine Defenders is unlocked; otherwise, false.</returns>
    public static bool IsCombinationUnlocked() => instance.isCombinationUnlocked;

    /// <summary>
    /// Saves the player's collection of Models and their upgrade data.
    /// </summary>
    private void SaveCollectionData()
    {
        // Pack
        CollectionSaveData collectionSaveData = new CollectionSaveData();
        collectionSaveData.modelSaveData = modelUpgradeSaveData;
        collectionSaveData.isCombinationUnlocked = isCombinationUnlocked;

        // Save
        SaveLoadManager.SaveCollection(instance, collectionSaveData);
    }

    /// <summary>
    /// Loads the player's collection of Models and their upgrade data.
    /// </summary>
    private void LoadCollectionData()
    {
        // Load
        CollectionSaveData saveData = SaveLoadManager.LoadCollection(instance);
        if (saveData == null) saveData = new CollectionSaveData();

        // Unpack
        List<ModelUpgradeSaveData> modelUpgradeSaveData = saveData.modelSaveData;
        if (modelUpgradeSaveData == null) instance.modelUpgradeSaveData = new List<ModelUpgradeSaveData>();
        else instance.modelUpgradeSaveData = saveData.modelSaveData;
        instance.isCombinationUnlocked = saveData.isCombinationUnlocked;

        instance.modelUpgradePointsTracker = new Dictionary<ModelType, int>();
        if (instance.modelUpgradeSaveData == null) instance.modelUpgradeSaveData = new List<ModelUpgradeSaveData>();
        int level = SaveLoadManager.GetLoadedGameLevel();
        if(level == 0) UnlockModel(ModelType.SQUIRREL);
        foreach (ModelUpgradeSaveData data in instance.modelUpgradeSaveData)
        {
            instance.modelUpgradePointsTracker[data.GetModelType()] = 0;
        }
    }

    #endregion
}
