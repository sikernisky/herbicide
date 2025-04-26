using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents an inventory item that can be used generally on the scene.
/// </summary>
public class GeneralInventoryItem : InventoryItem
{
    /// <summary>
    /// Returns the type of model this is.
    /// </summary>
    /// <returns>the type of model this is.</returns>
    public override ModelType GetModelType() => ModelType.INVENTORY_ITEM_GENERAL;
}
