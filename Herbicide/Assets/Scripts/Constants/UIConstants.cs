using UnityEngine;

/// <summary>
/// Constants for UI elements.
/// </summary>
public static class UIConstants
{
    public static readonly Vector3 InsightEquipmentImageScreenOffset = new Vector3(0, 2, 0);
    public static readonly Vector2 TicketSize = new Vector2(120f, 24f);
    public static readonly Vector2 FirstTicketPosition = new Vector2(-320f, 86f);
    public static readonly Vector2 TicketPositionOffset = new Vector2(0f, -32f);
    public static readonly Vector2 FirstIngredientBarPosition = FirstTicketPosition - TicketPositionOffset;
    public static readonly Vector2 TicketIconStartPosition = new Vector2(29f, 0f);
    public static readonly Vector2 TicketIconOffset = new Vector2(23f, 0f);
}