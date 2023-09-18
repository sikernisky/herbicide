using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents something that can be stored in the inventory.
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

    /// <summary>
    /// Returns the amount of currency required to place this ISlottable.
    /// </summary>
    /// <returns>the amount of currency required to plae this ISlottable.
    /// </returns>
    int GetCost();




}
