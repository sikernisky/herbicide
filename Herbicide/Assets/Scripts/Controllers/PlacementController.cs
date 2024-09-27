using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

/// <summary>
/// Controls placement events.
/// </summary>
public class PlacementController : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// Reference to the PlacementController singleton.
    /// </summary>
    private static PlacementController instance;

    /// <summary>
    /// Color an Image turns when placing.
    /// </summary>
    private static readonly Color32 PLACE_COLOR = new Color32(255, 255, 255, 200);

    /// <summary>
    /// Dummy GameObject for placement events
    /// </summary>
    [SerializeField]
    private GameObject dummy;

    /// <summary>
    /// Image of dummy GameObject for placement events
    /// </summary>
    [SerializeField]
    private Image dummyImage;

    /// <summary>
    /// Dummy GameObjects for combination events
    /// </summary>
    [SerializeField]
    private GameObject[] combinationDummies;

    /// <summary>
    /// Images of dummy GameObjects for combination events
    /// </summary>
    [SerializeField]
    private Image[] combinationDummyImages;

    /// <summary>
    /// Stores the start times for each combination dummy.
    /// </summary>
    private float[] combinationStartTimes;

    /// <summary>
    /// Number of seconds it takes to combine defenders.
    /// </summary>
    private float combinationMoveDuration = 1f;

    /// <summary>
    /// Animation curve used for the combination lerp. 
    /// </summary>
    [SerializeField]
    private AnimationCurve combinationLerpCurve;

    /// <summary>
    /// true if there is an active combination event; otherwise, false.
    /// </summary>
    private bool combining;

    /// <summary>
    /// The Tile on which the player is ghost placing; null if they aren't.
    /// </summary>
    private Tile ghostSubject;

    /// <summary>
    /// The current GameState.
    /// </summary>
    private GameState gameState;

    /// <summary>
    /// true if there is an active placement event; otherwise, false.
    /// </summary>
    private bool placing;

    /// <summary>
    /// the Model the player is placing. 
    /// </summary>
    private Model subject;

    /// <summary>
    /// Defines a delegate for completing a Model placement.
    /// </summary>
    /// <param name="model">The placed Model.</param>
    public delegate void FinishPlacingDelegate(Model model);

    /// <summary>
    /// Event triggered when a Model is placed.
    /// </summary>
    public event FinishPlacingDelegate OnFinishPlacing;

    /// <summary>
    /// The position of the last placed Model.
    /// </summary>
    private Vector3 lastPlacedPosition;

    #endregion

    #region Methods

    /// <summary>
    /// Finds and sets the PlacementController singleton.
    /// </summary>
    /// <param name="levelController">The LevelController singleton.</param>
    public static void SetSingleton(LevelController levelController)
    {
        if (levelController == null) return;
        if (instance != null) return;

        PlacementController[] placementControllers = FindObjectsOfType<PlacementController>();
        Assert.IsNotNull(placementControllers, "Array of InputControllers is null.");
        Assert.AreEqual(1, placementControllers.Length);
        instance = placementControllers[0];
        Assert.IsNotNull(instance.dummy);
        Assert.IsNotNull(instance.dummyImage);
        Assert.IsNotNull(instance.combinationDummies);
        Assert.IsNotNull(instance.combinationDummyImages);
        Assert.IsNotNull(instance.combinationLerpCurve);

        instance.combinationStartTimes = new float[instance.combinationDummies.Length];
    }


    /// <summary>
    /// Checks for necessary placement events. Looks at mouse input to determine
    /// if something needs to be placed or returned. If something is being
    /// placed, glues its subject to the mouse.<br></br>
    /// 
    /// Also checks if the player pressed escape. If so, cancels any active placement
    /// and/or ghost placement events.
    /// </summary>
    /// <param name="gameState">The current GameState.</param>  
    /// <param name="didEscape">true if the player pressed escape this frame; otherwise,
    /// false. </param>
    public static void UpdatePlacementEvents(GameState gameState, bool didEscape)
    {
        if (instance == null) return;
        instance.gameState = gameState;

        if (instance.gameState == GameState.ONGOING)
        {
            instance.GlueSubjectToMouse();
            if (IsPlacing())
            {
                instance.dummyImage.sprite = instance.subject.GetPlacementTrack()[0];
                instance.lastPlacedPosition = instance.dummy.transform.position;
            }

            if (didEscape)
            {
                if (IsPlacing()) StopPlacingObject();
                if (IsGhostPlacing()) StopGhostPlacing();
            }
            if (IsGhostPlacing() && !instance.ghostSubject.HasActiveGhostOccupant())
            {
                StopGhostPlacing();
            }
        }
        else if (IsPlacing()) StopPlacingObject();
    }

    /// <summary>
    /// Starts a placement event.
    /// </summary>
    public static void StartPlacingObject(Model m)
    {
        if (IsPlacing()) return;
        if (m == null) return;

        Vector2Int placementDimensions = m.GetPlacementTrackDimensions();
        Vector3 placementDimensionsConv = new Vector3(placementDimensions.x, placementDimensions.y, 1);

        instance.dummy.SetActive(true);
        instance.dummyImage.sprite = m.GetPlacementTrack()[0]; // TODO: Animation
        instance.dummyImage.color = PLACE_COLOR;
        instance.dummyImage.rectTransform.sizeDelta = placementDimensionsConv;
        instance.placing = true;
        instance.subject = m;
    }

    /// <summary>
    /// Stops an active placement event if there is one.
    /// </summary>
    public static void StopPlacingObject()
    {
        if (!IsPlacing()) return;

        instance.dummy.SetActive(false);
        instance.dummyImage.sprite = null;
        instance.dummyImage.color = Color.white;
        instance.placing = false;
        instance.OnFinishPlacing?.Invoke(instance.subject);
        instance.subject = null;
    }

    /// <summary>
    /// Returns the Model that is currently being placed. If there is none,
    /// returns null. 
    /// </summary>
    /// <returns>the Model that is currently being placed; null if no Model
    /// is being placed.</returns>
    public static Model GetObjectPlacing() => instance.subject;

    /// <summary>
    /// Returns true if there is an item being placed.
    /// </summary>
    /// <returns>true if there is an item being placed; otherwise, false.</returns>
    public static bool IsPlacing() => instance.placing;

    /// <summary>
    /// Returns true if the player is ghost placing.
    /// </summary>
    /// <returns>true if the player is ghost placing; otherwise, false.
    /// </returns>
    public static bool IsGhostPlacing() => instance.ghostSubject != null;

    /// <summary>
    /// Transforms the subject of an active place event such that it is centered on
    /// the player's mouse.
    /// </summary>
    private void GlueSubjectToMouse() => dummy.transform.position = InputController.GetUIMousePosition();

    /// <summary>
    /// Starts a ghost place.
    /// </summary>
    /// <param name="subject">The Tile that the player is ghost placing on.</param>
    public static void StartGhostPlacing(Tile subject)
    {
        if (IsGhostPlacing()) return;
        if (subject == null) return;

        instance.dummyImage.color = new Color(
                instance.dummyImage.color.r,
                instance.dummyImage.color.g,
                instance.dummyImage.color.b,
                0
            );

        instance.ghostSubject = subject;
    }

    /// <summary>
    /// Stops an active ghost place (if there is one).
    /// </summary>
    /// <returns>The coordinates of the just-canceled ghost place.</returns>
    public static void StopGhostPlacing()
    {
        if (!IsGhostPlacing()) return;

        Assert.IsNotNull(instance.ghostSubject);
        Assert.IsNotNull(instance.dummyImage);

        instance.dummyImage.color = PLACE_COLOR;
        instance.ghostSubject.GhostRemove();
        instance.ghostSubject = null;
    }

    /// <summary>
    /// Triggers a combination event with the given Defenders.
    /// </summary>
    /// <param name="defendersToCombine">The Defenders to combine.</param>
    /// <param name="newTier">The tier of the new Defender.</param>
    public static void AnimateCombination(List<Defender> defendersToCombine)
    {
        Assert.IsNotNull(defendersToCombine);
        Assert.IsTrue(defendersToCombine.Count >= 0 && defendersToCombine.Count <= instance.combinationDummies.Length);

        List<Vector3> initialPositions = new List<Vector3>();
        List<Sprite> combinationSprites = new List<Sprite>();
        List<Vector3> dummySizes = new List<Vector3>();

        foreach (Model model in defendersToCombine)
        {
            Assert.IsNotNull(model);
            initialPositions.Add(CameraController.WorldToScreenPoint(model.GetPosition()));
            combinationSprites.Add(model.GetSprite());
            Vector3 size = new Vector3(model.GetPlacementTrackDimensions().x, model.GetPlacementTrackDimensions().y, 1);
            dummySizes.Add(size);
        }

        // Setup dummies
        for (int i = 0; i < defendersToCombine.Count; i++)
        {
            instance.combinationDummies[i].SetActive(true);
            instance.combinationDummies[i].transform.position = initialPositions[i];
            instance.combinationDummyImages[i].rectTransform.sizeDelta = dummySizes[i];
            instance.combinationDummyImages[i].sprite = combinationSprites[i];
            instance.combinationDummyImages[i].color = PLACE_COLOR;
            instance.combinationStartTimes[i] = Time.time;
        }

        instance.combining = true;
    }

    /// <summary>
    /// Called every frame. Updates the combination events, moving the dummies towards the cursor.
    /// </summary>
    public static void UpdateCombinationEvents()
    {
        if (instance.combinationDummies == null || instance.combinationLerpCurve == null) return;
        if (!instance.combining) return;
        Defender defenderSubject = instance.subject as Defender;
        if (defenderSubject == null)
        {
            instance.combining = false;
            return;
        }

        Vector3 targetPosition;
        if (IsPlacing()) targetPosition = InputController.GetUIMousePosition();
        else targetPosition = instance.lastPlacedPosition;
        targetPosition.z = 0;

        float combinationDuration = instance.combinationMoveDuration;

        for (int i = 0; i < instance.combinationDummies.Length; i++)
        {
            if (!instance.combinationDummies[i].activeSelf) continue;

            float elapsedTime = Time.time - instance.combinationStartTimes[i];
            float t = Mathf.Clamp01(elapsedTime / combinationDuration);
            float lerpFactor = instance.combinationLerpCurve.Evaluate(t);

            instance.combinationDummies[i].transform.position = Vector3.Lerp(
                instance.combinationDummies[i].transform.position,
                targetPosition,
                lerpFactor
            );

            if (t >= 1.0f || Vector3.Distance(instance.combinationDummies[i].transform.position, targetPosition) < 10f)
            {
                instance.combinationDummies[i].SetActive(false);
                instance.combinationDummies[i].transform.position = Vector3.zero;
            }
        }

        // Check to see if all dummies are inactive
        bool allInactive = true;
        for (int i = 0; i < instance.combinationDummies.Length; i++)
        {
            if (instance.combinationDummies[i].activeSelf)
            {
                allInactive = false;
                break;
            }
        }

        if (allInactive)
        {
            instance.combining = false;
        }
    }

    /// <summary>
    /// Returns true if there is an active combination event.
    /// </summary>
    /// <returns>true if there is an active combination event; otherwise,
    /// false. </returns>
    public static bool IsCombining() => instance.combining;

    /// <summary>
    /// Subscribes a handler (the ControllerController) to the finish placing event.
    /// </summary>
    /// <param name="handler">The handler to subscribe. </param>
    public static void SubscribeToFinishPlacingDelegate(FinishPlacingDelegate handler)
    {
        Assert.IsNotNull(handler, "Handler is null.");
        instance.OnFinishPlacing += handler;
    }

    #endregion
}
