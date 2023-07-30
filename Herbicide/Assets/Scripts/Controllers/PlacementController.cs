using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

/// <summary>
/// Handles selecting and placing items. This may happen in regards to the
/// Inventory or Tiles.
/// </summary>
public class PlacementController : MonoBehaviour
{
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
    /// The InventorySlot that an ISlottable is being placed from; null
    /// if no placement event is active. 
    /// </summary>
    private InventorySlot placingSlot;

    /// <summary>
    /// The Tile on which the player is ghost placing; null if they aren't.
    /// </summary>
    private Tile ghostSubject;

    /// <summary>
    /// The current GameState.
    /// </summary>
    private GameState gameState;


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
    }


    /// <summary>
    /// Checks for necessary placement events. Looks at mouse input to determine
    /// if something needs to be placed or returned. If something is being
    /// placed, glues its subject to the mouse.<br></br>
    /// 
    /// Also checks if the player pressed escape. If so, cancels any active placement
    /// and/or ghost placement events.
    /// </summary>
    public static void CheckPlacementEvents(bool didEscape)
    {
        if (instance == null) return;

        if (instance.gameState == GameState.ONGOING)
        {
            if (Placing()) instance.GlueSubjectToMouse();
            if (didEscape)
            {
                if (Placing()) StopPlacingObject(false);
                if (GhostPlacing()) StopGhostPlacing();
            }
            if (GhostPlacing() && !instance.ghostSubject.HasActiveGhostOccupant())
            {
                StopGhostPlacing();
            }
        }
        else if (Placing()) StopPlacingObject(false);
    }

    /// <summary>
    /// Starts a placement event.
    /// </summary>
    /// <param name="slot">the InventorySlot to place from. </param>
    public static void StartPlacingObject(InventorySlot slot)
    {
        if (slot == null) return;
        if (Placing()) return;

        instance.dummy.SetActive(true);
        instance.dummyImage.sprite = slot.GetOccupant().GetPlacementSprite();
        instance.dummyImage.color = PLACE_COLOR;
        instance.placingSlot = slot;
    }

    /// <summary>
    /// Stops an active placement event if there is one.
    /// </summary>
    /// <param name="wasPlaced">true if the placing object was placed;
    /// false if it was canceled or otherwise invalid. </param>
    public static void StopPlacingObject(bool wasPlaced)
    {
        if (!Placing()) return;
        if (!instance.placingSlot.CanUse()) return;

        if (wasPlaced) instance.placingSlot.Use();
        instance.dummy.SetActive(false);
        instance.dummyImage.sprite = null;
        instance.dummyImage.color = Color.white;
        instance.placingSlot = null;
    }

    /// <summary>
    /// Returns the ISlottable that is currently being placed. If there is none,
    /// returns null. 
    /// </summary>
    /// <returns>the ISlottable that is currently being placed; null if no ISlottable
    /// is being placed.</returns>
    public static ISlottable GetObjectPlacing()
    {
        return instance.placingSlot.GetOccupant();
    }

    /// <summary>
    /// Returns true if there is an item being placed.
    /// </summary>
    /// <returns>true if there is an item being placed; otherwise, false.</returns>
    public static bool Placing()
    {
        return instance.placingSlot != null;
    }

    /// <summary>
    /// Returns true if the player is ghost placing.
    /// </summary>
    /// <returns>true if the player is ghost placing; otherwise, false.
    /// </returns>
    public static bool GhostPlacing()
    {
        return instance.ghostSubject != null;
    }

    /// <summary>
    /// Transforms the subject of an active place event such that it is centered on
    /// the player's mouse.
    /// </summary>
    private void GlueSubjectToMouse()
    {
        if (!Placing()) return;

        dummy.transform.position = InputController.GetMousePosition();
    }

    /// <summary>
    /// Starts a ghost place.
    /// </summary>
    /// <param name="subject">The Tile that the player is ghost placing on.</param>
    public static void StartGhostPlacing(Tile subject)
    {
        if (GhostPlacing()) return;
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
        if (!GhostPlacing()) return;

        Assert.IsNotNull(instance.ghostSubject);
        Assert.IsNotNull(instance.dummyImage);

        instance.dummyImage.color = new Color(
                        instance.dummyImage.color.r,
                        instance.dummyImage.color.g,
                        instance.dummyImage.color.b,
                        255
                        );

        instance.ghostSubject.GhostRemove();
        instance.ghostSubject = null;
    }

    /// <summary>
    /// Informs the PlacementController of the most recent GameState.
    /// </summary>
    /// <param name="state">the most recent GameState.</param>
    public static void InformOfGameState(GameState state)
    {
        instance.gameState = state;
    }
}
