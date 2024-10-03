using UnityEngine;
using UnityEngine.Assertions;
using TMPro;
using System.Collections.Generic;
using Requirements = ModelUpgradeRequirements.ModelUpgradeRequirementsData;

/// <summary>
/// Controls events that occur when a level is completed.
/// </summary>
public class LevelCompletionController : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// The state of the completion panel.
    /// </summary>
    private enum CompletionState
    {
        INITIAL_RESULT,
        PROGRESS_TRACKS_UPDATING,
        PROGRESS_TRACKS_UPDATED,
    }

    /// <summary>
    /// The current state of the completion panel.
    /// </summary>
    private CompletionState completionState;

    /// <summary>
    /// Reference to the LevelCompletionController singleton.
    /// </summary>
    private static LevelCompletionController instance;

    /// <summary>
    /// The most recent game state.
    /// </summary>
    private GameState gameState;

    /// <summary>
    /// Reference to the level completion panel GameObject.
    /// </summary>
    [SerializeField]
    private GameObject levelCompletionPanel;

    /// <summary>
    /// The parent object for all display result objects.
    /// </summary>
    [SerializeField]
    private GameObject displayResultObjects;

    /// <summary>
    /// The parent object for all progress track objects.
    /// </summary>
    [SerializeField]
    private GameObject progressTrackObjects;

    /// <summary>
    /// Text that displays whether the player won, lost, or tied.
    /// </summary>
    [SerializeField]
    private TMP_Text resultText;

    /// <summary>
    /// List of progress tracks.
    /// </summary>
    [SerializeField]
    private List<ProgressTrack> progressTracks;

    /// <summary>
    /// true if the progress tracks have been updated and
    /// completed their animations; false otherwise.
    /// </summary>
    private bool progressTracksUpdated;

    /// <summary>
    /// The number of progress points to feed the progress track per update tick.
    /// </summary>
    private int POINTS_PER_TICK => 1;

    /// <summary>
    /// The time between each tick of the progress track.
    /// </summary>
    private float timeBetweenTicks => 0.075f;

    /// <summary>
    /// The timer for the progress track tick.
    /// </summary>
    private float tickTimer;

    /// <summary>
    /// true if the end game data has been loaded; false otherwise.
    /// </summary>
    private bool dataLoaded;

    #endregion

    #region Methods

    /// <summary>
    /// Finds and sets the LevelCompletionController singleton.
    /// </summary>
    /// <param name="levelController">The LevelController singleton.</param>
    public static void SetSingleton(LevelController levelController)
    {
        if (levelController == null) return;

        LevelCompletionController[] levelCompletionControllers = FindObjectsOfType<LevelCompletionController>();
        Assert.IsNotNull(levelCompletionControllers, "Array of LevelCompletionControllers is null.");
        Assert.AreEqual(1, levelCompletionControllers.Length);
        instance = levelCompletionControllers[0];
        instance.resultText.text = "";
        instance.levelCompletionPanel.SetActive(false);
    }

    /// <summary>
    /// Main loop for the LevelCompletionController.
    /// </summary>
    /// <param name="gameState">The most recent game state.</param>
    public static void UpdateLevelCompletionController(GameState gameState)
    {
        if (instance == null) return;
        instance.gameState = gameState;
        if(!instance.LevelOver()) return;   

        instance.ExecuteInitialResultState();
        instance.ExecuteProgressTracksUpdatingState();
        instance.ExecuteProgressTracksUpdatedState();
    }

    /// <summary>
    /// Returns true if the Level Completion panel is open.
    /// </summary>
    /// <returns>true if the Level Completion panel is open;
    /// otherwise, false. </returns>
    private bool LevelCompletePanelOpen() => levelCompletionPanel.activeSelf;

    /// <summary>
    /// Opens the Level Completion panel and populates it
    /// depending on the most recent game state.
    /// </summary>
    private void OpenLevelCompletePanel()
    {
        if (LevelCompletePanelOpen()) return;

        if (!dataLoaded) PopulateLevelCompletionControllerData();
        levelCompletionPanel.SetActive(true);
        if (gameState == GameState.WIN) resultText.text = "You Win!";
        else if (gameState == GameState.LOSE) resultText.text = "You Lose!";
        else if (gameState == GameState.TIE) resultText.text = "You Tied!";
    }

    /// <summary>
    /// Close the Level Completion panel.
    /// </summary>
    private void CloseLevelCompletionPanel()
    {
        if (!LevelCompletePanelOpen()) return;

        levelCompletionPanel.SetActive(false);
    }

    /// <summary>
    /// Returns true if the level is over and the reward has been collected.
    /// </summary>
    /// <returns>true if the level is over and the reward has been collected;
    /// false otherwise</returns>
    private bool LevelOver()
    {
        bool gameOver = gameState == GameState.LOSE ||
            gameState == GameState.WIN ||
            gameState == GameState.TIE;
        bool rewardCollected = ControllerController.LevelRewardCollected();
        return gameOver && rewardCollected;
    }

    /// <summary>
    /// Instantly completes the remaining progress for all progress tracks.
    /// </summary>
    public void CompleteRemainingProgressInstantly()
    {
        foreach (ProgressTrack progressTrack in progressTracks)
        {
            if (!progressTrack.Initialized()) continue;

            ModelType typeUpgrading = progressTrack.GetModelTypeTrackIsUpgrading();
            int remainingPointsToAdd = CollectionManager.GetModelUpgradePoints(typeUpgrading);
            if (remainingPointsToAdd > 0)
            {
                progressTrack.AddProgress(remainingPointsToAdd);
                CollectionManager.RemoveModelUpgradePoints(typeUpgrading, remainingPointsToAdd);
            }
        }
        completionState = CompletionState.PROGRESS_TRACKS_UPDATED;
        tickTimer = 0f;
    }

    private void PopulateLevelCompletionControllerData()
    {
        List<ModelType> unlockedModels = CollectionManager.GetAllUnlockedModelTypes();
        for (int i = 0; i < instance.progressTracks.Count; i++)
        {
            if (i >= unlockedModels.Count) break;
            ModelType typeToLoadProgressTrackWith = unlockedModels[i];
            ModelUpgradeSaveData saveDataForType = CollectionManager.GetUnlockedModelUpgradeSaveData(typeToLoadProgressTrackWith);
            Requirements requirementsForType = CollectionManager.GetUpgradeRequirementsData(typeToLoadProgressTrackWith);
            instance.progressTracks[i].InitializeProgressTrack(saveDataForType, requirementsForType);
        }

        foreach (ProgressTrack progressTrack in instance.progressTracks)
        {
            if (!progressTrack.Initialized()) progressTrack.gameObject.SetActive(false);
        }
    }

    #endregion

    #region State Logic

    /// <summary>
    /// Runs logic for the Initial Result state.
    /// </summary>
    private void ExecuteInitialResultState()
    {
        if(completionState != CompletionState.INITIAL_RESULT) return;

        if(!LevelCompletePanelOpen()) OpenLevelCompletePanel();
        if(!displayResultObjects.activeSelf) displayResultObjects.SetActive(true);
    }

    /// <summary>
    /// Runs logic for the Progress Tracks Updating state.
    /// </summary>
    private void ExecuteProgressTracksUpdatingState()
    {
        if (completionState != CompletionState.PROGRESS_TRACKS_UPDATING) return;

        if (!LevelCompletePanelOpen()) OpenLevelCompletePanel();

        // Increment the tick timer by the time elapsed since the last frame
        tickTimer += Time.deltaTime;

        // Check if it's time to add points
        if (tickTimer >= timeBetweenTicks)
        {
            progressTracksUpdated = true;
            foreach (ProgressTrack progressTrack in progressTracks)
            {
                if (!progressTrack.Initialized()) continue;
                ModelType typeUpgrading = progressTrack.GetModelTypeTrackIsUpgrading();
                int totalPointsToAdd = CollectionManager.GetModelUpgradePoints(typeUpgrading);
                if (totalPointsToAdd <= 0) continue;

                progressTracksUpdated = false;
                int pointsToAddThisTick = Mathf.Min(POINTS_PER_TICK, totalPointsToAdd);
                progressTrack.AddProgress(pointsToAddThisTick);
                CollectionManager.RemoveModelUpgradePoints(typeUpgrading, pointsToAddThisTick);
            }
            tickTimer = 0f;
        }

        if (progressTracksUpdated) completionState = CompletionState.PROGRESS_TRACKS_UPDATED;
    }

    /// <summary>
    /// Runs logic for the Progress Tracks Updated state.
    /// </summary>
    private void ExecuteProgressTracksUpdatedState()
    {
        if(completionState != CompletionState.PROGRESS_TRACKS_UPDATED) return;
    }

    #endregion

    #region Button Events

    /// <summary>
    /// # BUTTON EVENT #
    /// 
    /// Called when the player clicks the OK/Next button on the Level
    /// completion panel.
    /// </summary>
    public void ClickLevelCompleteNextButton()
    {
        if (!LevelCompletePanelOpen()) OpenLevelCompletePanel();

        switch (completionState)
        {
            case CompletionState.INITIAL_RESULT:
                completionState = CompletionState.PROGRESS_TRACKS_UPDATING;
                displayResultObjects.SetActive(false);
                progressTrackObjects.SetActive(true);
                break;
            case CompletionState.PROGRESS_TRACKS_UPDATING:
                CompleteRemainingProgressInstantly();
                completionState = CompletionState.PROGRESS_TRACKS_UPDATED;
                break;
            case CompletionState.PROGRESS_TRACKS_UPDATED:
                CloseLevelCompletionPanel();
                SceneController.LoadNextLevelWithFadeDelay();
                break;
        }
    }

    #endregion
}
