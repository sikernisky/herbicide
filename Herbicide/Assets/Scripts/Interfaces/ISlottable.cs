using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a GameObject that can be stored in the inventory.
/// </summary>
public interface ISlottable
{
    /// <summary>
    /// Returns a Sprite component that represents this ISlottable in an 
    /// Inventoryslot.
    /// </summary>
    /// <returns>a Sprite component that represents this ISlottable in an 
    /// Inventoryslot.</returns>
    Sprite GetInventorySprite();

    /// <summary>
    /// Returns a Sprite component that represents this ISlottable in a
    /// placement event.
    /// </summary>
    /// <returns>a Sprite component that represents this ISlottable in a
    /// placement event.</returns>
    Sprite GetPlacementSprite();




}
