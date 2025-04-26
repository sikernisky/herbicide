using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents an inventory item that can be used on a tile.
/// </summary>
public class UseableInventoryItem : InventoryItem, ISurfaceUseable
{
    #region Fields

    #endregion  

    #region Methods

    /// <summary>
    /// Returns the type of model this is.
    /// </summary>
    /// <returns>the type of model this is.</returns>
    public override ModelType GetModelType() => ModelType.INVENTORY_ITEM_USEABLE;

    #endregion
}
