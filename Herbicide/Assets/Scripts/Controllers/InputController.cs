using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

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
    /// the Tile the player is hovering; null if not hovering over a Tile.
    /// </summary>
    private Tile tileHovering;

    /// <summary>
    /// Finds and sets the InputController singleton.
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
    }

    /// <summary>
    /// Returns the Tile the player hovered on at this specific frame; returns
    /// null if no Tile was hovered on this frame.
    /// </summary>
    /// <returns>the Tile the player hovered on this frame, 
    /// or null if they didn't.</returns>
    public static Tile TileHovered()
    {
        Vector2 mousePos = GetMousePosition();
        Vector2 worldPos = CameraController.ScreenToWorldPoint(mousePos);
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

        if (hit.collider == null) return null;
        if (instance.tileHovering != null) return null;
        Tile hovered = hit.collider.gameObject.GetComponent<Tile>();
        instance.tileHovering = hovered;
        return hovered;
    }

    /// <summary>
    /// Returns the Tile the player hovered off at this specific frame; returns
    /// null if no Tile was hovered off this frame.
    /// </summary>
    /// <returns>the Tile the player hovered off this frame, 
    /// or null if they didn't.</returns>
    public static Tile TileDehovered()
    {
        Vector2 mousePos = GetMousePosition();
        Vector2 worldPos = CameraController.ScreenToWorldPoint(mousePos);
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);


        if (hit.collider != null) return null;
        if (instance.tileHovering == null) return null;
        Tile dehoveredTile = instance.tileHovering;
        instance.tileHovering = null;
        return dehoveredTile;
    }

    /// <summary>
    /// Returns the Tile the player clicked on at this specific frame; returns
    /// null if no Tile was clicked on this frame.
    /// </summary>
    /// <returns>the Tile the player clicked on this frame, 
    /// or null if they didn't.</returns>
    public static Tile TileClickedDown()
    {
        Vector2 mousePos = GetMousePosition();
        Vector2 worldPos = CameraController.ScreenToWorldPoint(mousePos);
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

        if (!DidPrimaryDown()) return null;
        if (hit.collider == null) return null;
        return hit.collider.gameObject.GetComponent<Tile>();
    }

    /// <summary>
    /// Returns the Tile the player clicked on at this specific frame; returns
    /// null if no Tile was clicked on this frame.
    /// </summary>
    /// <returns>the Tile the player clicked on this frame, 
    /// or null if they didn't.</returns>
    public static Tile TileClickedUp()
    {
        Vector2 mousePos = GetMousePosition();
        Vector2 worldPos = CameraController.ScreenToWorldPoint(mousePos);
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

        if (!DidPrimaryUp()) return null;
        if (hit.collider == null) return null;
        return hit.collider.gameObject.GetComponent<Tile>();
    }

    /// <summary>
    /// Gets the player's mouse position.
    /// </summary>
    /// <returns>the player's mouse position.</returns>
    public static Vector2 GetMousePosition()
    {
        return Input.mousePosition;
    }

    /// <summary>
    /// Returns true if the player has clicked their primary button down (PC: mouse).
    /// </summary>
    /// <returns>true if the player clicked their primary down; otherwise, false.
    /// </returns>
    public static bool DidPrimaryDown()
    {
        if (Input.GetMouseButtonDown(0)) return true;
        return false;
    }

    /// <summary>
    /// Returns true if the player has clicked their primary button up (PC: mouse).
    /// </summary>
    /// <returns>true if the player clicked their primary up; otherwise, false.
    /// </returns>
    public static bool DidPrimaryUp()
    {
        if (Input.GetMouseButtonUp(0)) return true;
        return false;
    }
}
