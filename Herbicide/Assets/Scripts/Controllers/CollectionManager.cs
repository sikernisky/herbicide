using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using static TreeEditor.TreeEditorHelper;
using Requirements = ModelUpgradeRequirements.ModelUpgradeRequirementsData;

/// <summary>
/// Manages the player's collection of Models, including upgrade points
/// and other related data.
/// </summary>
public class CollectionManager : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// Reference to the SaveData singleton.
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
    private List<ModelSaveData> unlockedModels;

    /// <summary>
    /// Tracks the number of points the player has earned for each ModelType
    /// during the current level.
    /// </summary>
    private Dictionary<ModelType, int> modelUpgradePointsTracker;

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
        #if UNITY_EDITOR
            if (InputManager.DidKeycodeDown(KeyCode.U))
            {
                UnlockModel(ModelType.BUNNY);
                UnlockModel(ModelType.RACCOON);
                UnlockModel(ModelType.OWL);
                UnlockModel(ModelType.PORCUPINE);
                ShopManager.UnlockReroll();
            }
            if (InputManager.DidKeycodeDown(KeyCode.B)) UnlockModel(ModelType.BUNNY);
            if (InputManager.DidKeycodeDown(KeyCode.A)) UnlockModel(ModelType.RACCOON);
            if (InputManager.DidKeycodeDown(KeyCode.O)) UnlockModel(ModelType.OWL);
            if (InputManager.DidKeycodeDown(KeyCode.R)) ShopManager.UnlockReroll();  
        #endif
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
    /// Returns true if the given ModelType is unlocked.
    /// </summary>
    /// <param name="modelType">the given ModelType to check. </param>
    /// <returns>true if the given ModelType is unlocked; otherwise, false.</returns>
    public static bool IsModelUnlocked(ModelType modelType) => 
        instance.unlockedModels.Exists(data => data.GetModelType() == modelType);

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
        Assert.IsFalse(instance.unlockedModels.Exists(data => data.GetModelType() == modelToUnlock));

        ModelSaveData data = new ModelSaveData(modelToUnlock, instance.requirements.GetUpgradeData(modelToUnlock));
        instance.unlockedModels.Add(data);
        instance.modelUpgradePointsTracker[modelToUnlock] = 0;
    }

    /// <summary>
    /// Returns a set of all unlocked ModelTypes.
    /// </summary>
    /// <returns>a set of all unlocked ModelTypes.</returns>
    public static HashSet<ModelType> GetAllUnlockedModels()
    {
        List<ModelType> modelTypes = instance.unlockedModels.ConvertAll(data => data.GetModelType());
        return new HashSet<ModelType>(modelTypes);
    }

    /// <summary>
    /// Returns a set of all unlocked Defenders.
    /// </summary>
    /// <returns> a set of all unlocked Defenders.</returns>
    public static HashSet<ModelType> GetAllUnlockedDefenders()
    {
        HashSet<ModelType> unlockedDefenders = new HashSet<ModelType>();
        foreach (ModelType modelType in GetAllUnlockedModels())
        {
            if (ModelTypeHelper.IsDefender(modelType)) unlockedDefenders.Add(modelType);
        }
        return unlockedDefenders;
    }

    /// <summary>
    /// Returns a set of all unlocked Tickets.
    /// </summary>
    /// <returns>a set of all unlocked Tickets.</returns>
    public static HashSet<ModelType> GetAllUnlockedTickets()
    {
        HashSet<ModelType> unlockedTickets = new HashSet<ModelType>();
        foreach (ModelType modelType in GetAllUnlockedModels())
        {
            if (ModelTypeHelper.IsTicket(modelType)) unlockedTickets.Add(modelType);
        }

        return unlockedTickets;
    }

    /// <summary>
    /// Returns the upgrade data for the given unlocked ModelType.
    /// </summary>
    /// <param name="modelType">the ModelType to get data for. </param>
    /// <returns>the upgrade data for the given unlocked ModelType.</returns>
    public static ModelSaveData GetUnlockedModelSaveData(ModelType modelType)
    {
        Assert.IsTrue(IsModelUnlocked(modelType), modelType + " is not unlocked and has no upgrade data.");
        return instance.unlockedModels.Find(data => data.GetModelType() == modelType);
    }

    /// <summary>
    /// Returns the upgrade requirements for the given ModelType.
    /// </summary>
    /// <param name="modelType">the Model Type to get info for. </param>
    /// <returns>the upgrade requirements for the given ModelType.</returns>
    public static Requirements GetModelUpgradeRequirementsData(ModelType modelType)
    {
        Assert.IsTrue(IsModelUnlocked(modelType), modelType + " is not unlocked and has no upgrade data.");
        return instance.requirements.GetUpgradeData(modelType);
    }

    /// <summary>
    /// Saves the player's collection of Models and their upgrade data.
    /// </summary>
    private void SaveCollectionData()
    {
        // Pack
        CollectionSaveData collectionSaveData = new CollectionSaveData();
        collectionSaveData.modelSaveData = unlockedModels;

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
        List<ModelSaveData> modelSaveData = saveData.modelSaveData;
        if (modelSaveData == null) instance.unlockedModels = new List<ModelSaveData>();
        else instance.unlockedModels = saveData.modelSaveData;

        instance.modelUpgradePointsTracker = new Dictionary<ModelType, int>();
        if (instance.unlockedModels == null) instance.unlockedModels = new List<ModelSaveData>();
        UnlockModel(ModelType.SQUIRREL);
        foreach (ModelSaveData data in instance.unlockedModels)
        {
            instance.modelUpgradePointsTracker[data.GetModelType()] = 0;
        }
    }

    #endregion
}
