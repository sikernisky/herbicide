using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using TMPro;
using Requirements = ModelUpgradeRequirements.ModelUpgradeRequirementsData;

/// <summary>
/// Represents a progress track the player can fill up
/// to unlock upgrades for a Model.
/// </summary>
public class ProgressTrack : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// The current progress the player has made on the given Model's upgrades.
    /// </summary>
    public ModelSaveData SavedModelUpgradeData { get; private set; }

    /// <summary>
    /// The upgrade requirements for the given Model.
    /// </summary>
    public Requirements UpgradeRequirementsData { get; private set; }

    /// <summary>
    /// The fill image of the ProgressTrack.
    /// </summary>
    [SerializeField]
    private Image progressFillImage;

    /// <summary>
    /// The GameObject that holds the card that represents the Model being upgraded.
    /// </summary>
    [SerializeField]
    private GameObject trackCardParentObject;

    /// <summary>
    /// The prefab for the card that represents the Model being upgraded.
    /// </summary>
    private GameObject trackCardPrefab;

    /// <summary>
    /// The text that displays the progress of the ProgressTrack.
    /// </summary>
    [SerializeField]
    private TMP_Text trackProgressText;

    /// <summary>
    /// The text that displays how much progress the player has made on the ProgressTrack.
    /// </summary>
    [SerializeField]
    private TMP_Text trackProgressMadeText;

    /// <summary>
    /// The total progress the player has made on this progress track so far.
    /// </summary>
    private int totalProgressMade;

    /// <summary>
    /// How much of the ProgressTrack is currently filled.
    /// </summary>
    private int currentFillAmount;

    /// <summary>
    /// The maximum progress value of the ProgressTrack.
    /// </summary>
    private int maxFillAmount;

    /// <summary>
    /// true if the ProgressTrack has been fully upgraded; false otherwise.
    /// </summary>
    private bool doneUpgrading;

    #endregion

    #region Methods

    /// <summary>
    /// Sets up the ProgressTrack, assigning it a type of Model to upgrade
    /// and a maximum progress value.
    /// </summary>
    /// <param name="upgradeSaveData">The saved progress the player has made on the given Model's upgrades.</param>
    /// <param name="upgradeRequirementsData">The requirements, per level, to upgrade the given Model.</param>
    public void InitializeProgressTrack(ModelSaveData upgradeSaveData, Requirements upgradeRequirementsData)
    {
        Assert.IsNotNull(upgradeSaveData, "upgradeSaveData is null.");
        Assert.IsTrue(upgradeSaveData.GetModelType() == upgradeRequirementsData.GetModelType(), "Saved data for " + 
            upgradeSaveData.GetModelType() + " does not match" +
            " loaded requirements: " + upgradeRequirementsData.GetModelType());

        SavedModelUpgradeData = upgradeSaveData;
        UpgradeRequirementsData = upgradeRequirementsData;

        int currentLevelForModel = SavedModelUpgradeData.GetLevel();
        int maxProgressPointsForModelAtCurrentLevel = UpgradeRequirementsData.GetPointRequirementsByLevel(currentLevelForModel);
        maxFillAmount = maxProgressPointsForModelAtCurrentLevel;
        currentFillAmount = SavedModelUpgradeData.GetCurrentProgress();

        UpdateTrackDisplay();
    }

    /// <summary>
    /// Returns true if the ProgressTrack has been initialized; false otherwise.
    /// </summary>
    /// <returns>true if the ProgressTrack has been initialized; false otherwise.</returns>
    public bool Initialized() => SavedModelUpgradeData != null;

    /// <summary>
    /// Updates the text of the ProgressTrack to display the current progress
    /// and name of the Model being upgraded.
    /// </summary>
    private void UpdateTrackDisplay()
    {
        if(trackCardPrefab == null)
        {
            ModelType modelType = SavedModelUpgradeData.GetModelType();
            trackCardPrefab = ShopFactory.GetShopCardPrefab(modelType);
        }

        trackCardPrefab.transform.SetParent(trackCardParentObject.transform);
        trackCardPrefab.transform.localScale = new Vector3(1, 1, 1);
        trackCardPrefab.transform.position = trackCardParentObject.transform.position;
        trackProgressText.text = currentFillAmount + "/" + maxFillAmount;
        trackProgressMadeText.text = "+" + totalProgressMade.ToString();
    }

    /// <summary>
    /// Adds progress to the ProgressTrack. If it overflows, the ProgressTrack is
    /// set to the next level and the overflow is added to the next level's progress.
    /// </summary>
    /// <param name="progressToAdd">the number of points to add.</param>
    public void AddProgress(int progressToAdd)
    {
        Assert.IsTrue(progressToAdd >= 0, "Progress to add must be greater than or equal to 0.");

        int overflow = currentFillAmount + progressToAdd - maxFillAmount;
        if (overflow >= 0)
        {
            // Level up and set current progress to 0 on overflow
            currentFillAmount = 0;
            totalProgressMade += (progressToAdd - overflow); // Only count the progress that filled the current level

            int newLevel = SavedModelUpgradeData.GetLevel() + 1;
            if (UpgradeRequirementsData.ValidLevel(newLevel))
            {
                SavedModelUpgradeData.SetLevel(newLevel);
                maxFillAmount = UpgradeRequirementsData.GetPointRequirementsByLevel(newLevel);
                AddProgress(overflow); // Recursively add the overflow to the next level
            }
            else
            {
                currentFillAmount = maxFillAmount;
                SavedModelUpgradeData.SetCurrentProgress(maxFillAmount);
            }
        }
        else
        {
            currentFillAmount += progressToAdd;
            totalProgressMade += progressToAdd;
            SavedModelUpgradeData.SetCurrentProgress(currentFillAmount);
        }

        UpdateTrackDisplay();
    }

    /// <summary>
    /// Returns the model type the ProgressTrack is upgrading.
    /// </summary>
    /// <returns>the model type the ProgressTrack is upgrading.</returns>
    public ModelType GetModelTypeTrackIsUpgrading()
    {
        return SavedModelUpgradeData.GetModelType();
    }

    #endregion
}
