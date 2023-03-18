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
    /// The ISlottable being placed; null if no placement event is active
    /// </summary>
    private ISlottable subject;


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
    /// placed, glues its subject to the mouse.
    /// </summary>
    /// <param name="levelController">the TileGrid singleton</param>
    public static void CheckPlacementEvents(TileGrid tileGrid)
    {
        if (tileGrid == null) return;
        if (instance == null) return;

        if (Placing()) instance.GlueSubjectToMouse();
    }

    /// <summary>
    /// Starts a placement event.
    /// </summary>
    /// <param name="item">the ISlottable to start placing</param>
    public static void StartPlacingObject(ISlottable item)
    {
        if (item == null) return;
        if (Placing()) return;

        instance.dummy.SetActive(true);
        instance.dummyImage.sprite = item.GetPlacementSprite();
        instance.subject = item;
    }

    /// <summary>
    /// Stops an active placement event if there is one.
    /// </summary>
    public static void StopPlacingObject()
    {
        if (!Placing()) return;

        instance.dummy.SetActive(false);
        instance.dummyImage.sprite = null;
        instance.subject = null;
    }

    /// <summary>
    /// Returns the ISlottable that is currently being placed. If there is none,
    /// returns null. 
    /// </summary>
    /// <returns>the ISlottable that is currently being placed; null if no ISlottable
    /// is being placed.</returns>
    public static ISlottable GetObjectPlacing()
    {
        return instance.subject;
    }

    /// <summary>
    /// Returns true if there is an item being placed.
    /// </summary>
    /// <returns>true if there is an item being placed; otherwise, false.</returns>
    public static bool Placing()
    {
        return instance.subject != null;
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
}
