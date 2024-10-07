using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
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
    /// The parent object for all progress track objects.
    /// </summary>
    [SerializeField]
    private GameObject progressTrackObjects;

    /// <summary>
    /// The prefab for the progress track.
    /// </summary>
    [SerializeField]
    private GameObject progressTrackPrefab;

    /// <summary>
    /// Text that displays whether the player won, lost, or tied.
    /// </summary>
    [SerializeField]
    private TMP_Text resultText;

    /// <summary>
    /// true if the end game data has been loaded; false otherwise.
    /// </summary>
    private bool dataLoaded;

    /// <summary>
    /// The ordered list of UpgradeSaveData for each model that our progress tracks
    /// will use.
    /// </summary>
    private List<ModelUpgradeSaveData> orderedUpgradeSaveData;

    /// <summary>
    /// The position the progress track moves towards after upgrading.
    /// </summary>
    [SerializeField]
    private Transform postUpgradePosition;

    /// <summary>
    /// The position the progress track sits at while upgrading.
    /// </summary>
    [SerializeField]
    private Transform currUpgradePosition;

    /// <summary>
    /// The position the progress track moves from before upgrading.
    /// </summary>
    [SerializeField]
    private Transform preUpgradePosition;

    /// <summary>
    /// true if the coroutine for upgrading is running; false otherwise.
    /// </summary>
    private bool upgrading;


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

        if (!dataLoaded) LoadUpgradeQueue();
        levelCompletionPanel.SetActive(true);
        resultText.gameObject.SetActive(true);
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

        if (gameState == GameState.WIN) return rewardCollected;
        else return gameOver;
    }

    /// <summary>
    /// Initializes the progress tracks with the most recent data.
    /// </summary>
    private void LoadUpgradeQueue()
    {
        orderedUpgradeSaveData = new List<ModelUpgradeSaveData>();
        List<ModelType> unlockedModels = CollectionManager.GetAllUnlockedModelTypes(); 
        foreach(ModelType unlockedModel in unlockedModels)
        {
            orderedUpgradeSaveData.Add(CollectionManager.GetUnlockedModelUpgradeSaveData(unlockedModel));
        }
    }

    /// <summary>
    /// Returns an instantiated progress track at the given position. Sets it up with the
    /// correct upgrade requirements and save data.
    /// </summary>
    /// <param name="startPosition">The position to instantiate the progress track.</param>
    private GameObject InstantiateAndSetupProgressTrack(Vector3 startPosition)
    {
        Assert.IsNotNull(orderedUpgradeSaveData, "orderedUpgradeSaveData is null.");
        Assert.IsTrue(orderedUpgradeSaveData.Count > 0, "orderedUpgradeSaveData is empty.");

        ModelUpgradeSaveData upgradeSaveData = orderedUpgradeSaveData[0];
        orderedUpgradeSaveData.RemoveAt(0);
        Requirements upgradeRequirements = CollectionManager.GetUpgradeRequirementsData(upgradeSaveData.GetModelType());
        GameObject progressTrack = Instantiate(progressTrackPrefab, progressTrackObjects.transform);
        progressTrack.transform.position = startPosition;
        ProgressTrack progressComp = progressTrack.GetComponent<ProgressTrack>();  
        progressComp.InitializeProgressTrack(upgradeSaveData, upgradeRequirements);

        return progressTrack;
    }

    /// <summary>
    /// Plays the entire upgrade animation for all progress tracks. Each
    /// track starts at the bottom, moves to the middle, upgrades, then moves
    /// to the top.
    /// </summary>
    /// <returns>A reference to the coroutine.</returns>
    private IEnumerator PlayUpgradeAnimation()
    {
        if (upgrading) yield break;
        upgrading = true;
        while (orderedUpgradeSaveData.Count > 0)
        {
            GameObject progressTrack = InstantiateAndSetupProgressTrack(preUpgradePosition.position);
            CanvasGroup progressTrackGroup = progressTrack.GetComponent<CanvasGroup>();
            progressTrackGroup.alpha = 0f;

            float UPGRADE_TIME = 1f;
            float MOVE_TIME = 1f;
            float FADE_TIME = 1f;
            ProgressTrack progressTrackComp = progressTrack.GetComponent<ProgressTrack>();

            yield return MoveToPositionAndFade(progressTrack.transform, currUpgradePosition.position, MOVE_TIME, progressTrackGroup, FADE_TIME, true);
            yield return UpgradeProgressTrackOverTime(progressTrackComp, UPGRADE_TIME);
            yield return MoveToPositionAndFade(progressTrack.transform, postUpgradePosition.position, MOVE_TIME, progressTrackGroup, FADE_TIME, false);
        }
        upgrading = false;
        yield return new WaitForSeconds(0.5f);
        completionState = CompletionState.PROGRESS_TRACKS_UPDATED;
        CloseLevelCompletionPanel();
        SceneController.LoadNextLevelWithFadeDelay();
        yield return null;
    }

    /// <summary>
    /// Moves a Transform (most likely a progress track) to a target position and
    /// fades in or out via a CanvasGroup over time.
    /// </summary>
    /// <param name="objectToMove">The object to move.</param>
    /// <param name="targetPosition">The target position to move to.</param>
    /// <param name="moveTime">The time it takes to move to the target position.</param>
    /// <param name="trackGroup">The CanvasGroup to fade in or out.</param>
    /// <param name="fadeDuration">The time it takes to fade in or out.</param> 
    /// <param name="fadeIn">true to fade in; false to fade out.</param>
    /// <returns></returns>
    private IEnumerator MoveToPositionAndFade(Transform objectToMove, Vector3 targetPosition,
        float moveTime, CanvasGroup trackGroup, float fadeDuration, bool fadeIn)
    {
        Vector3 startPosition = objectToMove.position;
        float elapsedTime = 0f;
        float fadeElapsed = 0f;

        if (fadeIn) trackGroup.alpha = 0f;
        else trackGroup.alpha = 1f;

        while (elapsedTime < moveTime)
        {
            float t = elapsedTime / moveTime;
            objectToMove.position = Vector3.Lerp(startPosition, targetPosition, t);
            fadeElapsed += Time.deltaTime;
            if (fadeIn) trackGroup.alpha = Mathf.Clamp01(fadeElapsed / fadeDuration);
            else trackGroup.alpha = Mathf.Clamp01(1f - (fadeElapsed / fadeDuration));

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        objectToMove.position = targetPosition;
        if (fadeIn) trackGroup.alpha = 1f;
        else trackGroup.alpha = 0f;
    }

    /// <summary>
    /// Upgrades an initialized progress track over time.
    /// </summary>
    /// <param name="track">The progress track to upgrade.</param>
    /// <param name="upgradeTime">The seconds it takes to fully upgrade.</param>
    /// <returns>a reference to the coroutine.</returns>
    private IEnumerator UpgradeProgressTrackOverTime(ProgressTrack track, float upgradeTime)
    {
        Assert.IsTrue(track.Initialized(), "ProgressTrack is not initialized.");
        ModelType typeUpgrading = track.GetModelTypeTrackIsUpgrading();
        int totalPointsToAdd = CollectionManager.GetModelUpgradePoints(typeUpgrading);
        float pointIncrementDelay = upgradeTime / totalPointsToAdd; 
        while (totalPointsToAdd > 0)
        {
            int pointsToAdd = Mathf.Min(1, totalPointsToAdd);
            track.AddProgress(pointsToAdd);
            totalPointsToAdd -= pointsToAdd;
            yield return new WaitForSeconds(pointsToAdd * pointIncrementDelay);
        }
    }

    /// <summary>
    /// Runs logic for the Initial Result state.
    /// </summary>
    private void ExecuteInitialResultState()
    {
        if(completionState != CompletionState.INITIAL_RESULT) return;

        if(!LevelCompletePanelOpen()) OpenLevelCompletePanel();
    }

    /// <summary>
    /// Runs logic for the Progress Tracks Updating state.
    /// </summary>
    private void ExecuteProgressTracksUpdatingState()
    {
        if (completionState != CompletionState.PROGRESS_TRACKS_UPDATING) return;

        if (!LevelCompletePanelOpen()) OpenLevelCompletePanel();
    }

    /// <summary>
    /// Runs logic for the Progress Tracks Updated state.
    /// </summary>
    private void ExecuteProgressTracksUpdatedState()
    {
        if(completionState != CompletionState.PROGRESS_TRACKS_UPDATED) return;
    }

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

                if(gameState == GameState.WIN)
                {
                    completionState = CompletionState.PROGRESS_TRACKS_UPDATING;
                    progressTrackObjects.SetActive(true);
                    // resultText.gameObject.SetActive(false);
                    StartCoroutine(PlayUpgradeAnimation());
                }
                else
                {
                    CloseLevelCompletionPanel();
                    SceneController.LoadSceneWithFadeDelay("MainMenu");
                }
                break;
            case CompletionState.PROGRESS_TRACKS_UPDATING:
                completionState = CompletionState.PROGRESS_TRACKS_UPDATED;
                CloseLevelCompletionPanel();
                SceneController.LoadNextLevelWithFadeDelay();
                break;
            case CompletionState.PROGRESS_TRACKS_UPDATED:
                break;
        }
    }

    #endregion
}
