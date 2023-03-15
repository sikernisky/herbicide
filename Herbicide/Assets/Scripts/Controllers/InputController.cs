using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Controls and detects player input. All checks for input events must
/// go through the InputController.
/// </summary>
public class InputController : MonoBehaviour
{
    /// <summary>
    /// Reference to the InputController singleton.
    /// </summary>
    private static InputController instance;

    /// <summary>
    /// Reference to the game Canvas
    /// </summary>
    [SerializeField]
    private Canvas canvas;

    /// <summary>
    /// the Tile the player is hovering; null if not hovering over a Tile.
    /// </summary>
    private Tile tileHovering;

    /// <summary>
    /// Reference to the GraphicRaycaster Canvas component.
    /// </summary>
    private GraphicRaycaster graphicRayCaster;

    /// <summary>
    /// Current PointerEventData.
    /// </summary>
    private PointerEventData pointerEventData;

    /// <summary>
    /// Reference to the EventSystem component.
    /// </summary>
    [SerializeField]
    private EventSystem eventSystem;

    /// <summary>
    /// The player's mouse position
    /// </summary>
    private Vector3 mousePosTemp;

    /// <summary>
    /// The player's mouse position in world coordinates
    /// </summary>
    private Vector3 worldPosTemp;

    /// <summary>
    /// Raycast of objects hit.
    /// </summary>
    private RaycastHit2D[] hitTemp;


    /// <summary>
    /// Results from a raycast.
    /// </summary>
    private List<RaycastResult> raycastResults;


    /// <summary>
    /// Finds and sets the InputController singleton. Also instantiates the
    /// InputController's temporary objects.
    /// </summary>
    /// <param name="levelController">The LevelController singleton.</param>
    public static void SetSingleton(LevelController levelController)
    {
        if (levelController == null) return;
        if (instance != null) return;

        InputController[] inputControllers = FindObjectsOfType<InputController>();
        Assert.IsNotNull(inputControllers, "Array of InputControllers is null.");
        Assert.AreEqual(1, inputControllers.Length);
        instance = inputControllers[0];
        instance.MakeTempObjects();
    }

    /// <summary>
    /// Initializes all objects when this InputController controller is instantiated
    /// to save memory.
    /// </summary>
    private void MakeTempObjects()
    {
        Assert.IsNotNull(canvas);

        //Get & set components
        graphicRayCaster = canvas.GetComponent<GraphicRaycaster>();

        //Make objects
        mousePosTemp = new Vector3();
        worldPosTemp = new Vector3();
        raycastResults = new List<RaycastResult>();
        pointerEventData = new PointerEventData(eventSystem);
    }

    /// <summary>
    /// Returns the Tile the player hovered on at this specific frame; returns
    /// null if no Tile was hovered on this frame.
    /// </summary>
    /// <returns>the Tile the player hovered on this frame, 
    /// or null if they didn't.</returns>
    public static Tile TileHovered()
    {
        instance.AssertTempObjectsMade();

        instance.mousePosTemp = GetMousePosition();
        instance.worldPosTemp = CameraController.ScreenToWorldPoint(instance.mousePosTemp);
        instance.hitTemp = Physics2D.RaycastAll(instance.worldPosTemp, Vector2.zero);

        foreach (RaycastHit2D hit in instance.hitTemp)
        {
            if (hit.collider != null && instance.tileHovering == null)
            {
                Tile t = hit.collider.gameObject.GetComponent<Tile>();
                if (t != null) instance.tileHovering = t;
                return t;
            }
        }
        return null;
    }

    /// <summary>
    /// Returns the Tile the player hovered off at this specific frame; returns
    /// null if no Tile was hovered off this frame.
    /// </summary>
    /// <returns>the Tile the player hovered off this frame, 
    /// or null if they didn't.</returns>
    public static Tile TileDehovered()
    {
        instance.AssertTempObjectsMade();

        instance.mousePosTemp = GetMousePosition();
        instance.worldPosTemp = CameraController.ScreenToWorldPoint(instance.mousePosTemp);
        instance.hitTemp = Physics2D.RaycastAll(instance.worldPosTemp, Vector2.zero);

        foreach (RaycastHit2D hit in instance.hitTemp)
        {
            //If any raycast hit in the array equal to the hovering tile,
            //you're still hovering over that tile, so return null -- nothing was dehovered
            if (hit.collider != null && instance.tileHovering != null)
            {
                Tile t = hit.collider.GetComponent<Tile>();
                if (t != null && t == instance.tileHovering) return null;
            }
        }

        Tile dehoveredTile = instance.tileHovering;
        instance.tileHovering = null;
        return dehoveredTile;
    }

    /// <summary>
    /// Returns the first Tile the player clicked on at this specific frame; returns
    /// null if no Tile was clicked on this frame.
    /// </summary>
    /// <returns>the Tile the player clicked on this frame, 
    /// or null if they didn't.</returns>
    public static Tile TileClickedDown()
    {
        instance.AssertTempObjectsMade();

        if (!DidPrimaryDown()) return null;
        return instance.GetTileFromRaycast();
    }

    /// <summary>
    /// Returns the first Tile the player clicked on at this specific frame; returns
    /// null if no Tile was clicked on this frame.
    /// </summary>
    /// <returns>the Tile the player clicked on this frame, 
    /// or null if they didn't.</returns>
    public static Tile TileClickedUp()
    {
        instance.AssertTempObjectsMade();

        if (!DidPrimaryUp()) return null;
        return instance.GetTileFromRaycast();
    }

    /// <summary>
    /// Returns the first Tile component found after iterating through a
    /// RayCastHit2D array generated from the player's mouse position.
    /// Returns null if no Tile component was found.
    /// </summary>
    /// <returns>the first Tile component found after iterating through a
    /// RayCastHit2D array. Returns null if no Tile component was found.
    /// </returns>
    /// <param name="hits">The array of RaycastHits.</param>
    private Tile GetTileFromRaycast()
    {
        AssertTempObjectsMade();

        instance.mousePosTemp = GetMousePosition();
        instance.worldPosTemp = CameraController.ScreenToWorldPoint(instance.mousePosTemp);
        instance.hitTemp = Physics2D.RaycastAll(instance.worldPosTemp, Vector2.zero);

        foreach (RaycastHit2D hit in instance.hitTemp)
        {
            if (hit.collider == null) continue;
            Tile t = hit.collider.gameObject.GetComponent<Tile>();
            if (t != null) return t;
        }

        return null;
    }

    /// <summary>
    /// Asserts that all temporary objects for the InputController have been
    /// instantiated.
    /// </summary>
    private void AssertTempObjectsMade()
    {
        Assert.IsNotNull(eventSystem);
        Assert.IsNotNull(graphicRayCaster);
        Assert.IsNotNull(raycastResults);
        Assert.IsNotNull(pointerEventData);
    }

    /// <summary>
    /// Returns true if the player is hovering over a UI element.
    /// </summary>
    /// <returns>true if the player is hovering over a UI element; otherwise,
    /// false.</returns>
    public static bool HoveringOverUIElement()
    {
        instance.AssertTempObjectsMade();

        instance.pointerEventData.Reset();
        instance.pointerEventData.position = GetMousePosition();
        instance.raycastResults.Clear();
        instance.graphicRayCaster.Raycast(instance.pointerEventData, instance.raycastResults);
        return instance.raycastResults.Count > 0;
    }


    /// <summary>
    /// Returns the player's mouse position.
    /// </summary>
    /// <returns>the player's mouse position.</returns>
    public static Vector2 GetMousePosition()
    {
        instance.AssertTempObjectsMade();

        return Input.mousePosition;
    }

    /// <summary>
    /// Returns true if the player has clicked their primary button down (PC: mouse).
    /// </summary>
    /// <returns>true if the player clicked their primary down; otherwise, false.
    /// </returns>
    public static bool DidPrimaryDown()
    {
        instance.AssertTempObjectsMade();

        if (Input.GetMouseButtonDown(0)) return true;
        return false;
    }

    /// <summary>
    /// Returns true if the player has clicked their escape button.
    /// </summary>
    /// <returns>true if the player clicked their escape; otherwise, false.
    /// </returns>
    public static bool DidEscapeDown()
    {
        instance.AssertTempObjectsMade();

        if (Input.GetKeyDown(KeyCode.Escape)) return true;
        return false;
    }

    /// <summary>
    /// Returns true if the player has clicked their primary button up (PC: mouse).
    /// </summary>
    /// <returns>true if the player clicked their primary up; otherwise, false.
    /// </returns>
    public static bool DidPrimaryUp()
    {
        instance.AssertTempObjectsMade();

        if (Input.GetMouseButtonUp(0)) return true;
        return false;
    }
}
