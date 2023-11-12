using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Represents something that is placed on the TileGrid
/// and helps the player win.
/// </summary>
public abstract class Defender : Mob
{
    /// <summary>
    /// Type of this Defender.
    /// </summary>
    public abstract DefenderType TYPE { get; }

    /// <summary>
    /// Class of this Defender.
    /// </summary>
    public abstract DefenderClass CLASS { get; }

    /// <summary>
    /// Type of this Defender.
    /// </summary>
    public enum DefenderType
    {
        SQUIRREL,
        BUTTERFLY
    }

    /// <summary>
    /// Class of this Defender.
    /// </summary>
    public enum DefenderClass
    {
        TREBUCHET,
        MAULER,
        PATHFINDER,
        ANIMYST
    }


    /// <summary>
    /// Returns a Sprite component that represents this Defender
    /// in an InventorySlot.
    /// </summary>
    /// <returns> a Sprite component that represents this Defender
    /// in an InventorySlot.</returns>
    public override Sprite GetInventorySprite() { return DefenderFactory.GetDefenderInventorySprite(TYPE); }

    /// <summary>
    /// Returns a Sprite component that represents this Defender
    /// when it is placing.
    /// </summary>
    /// <returns> a Sprite component that represents this Defender
    /// when it is placing.</returns>
    public override Sprite GetPlacementSprite() { return DefenderFactory.GetDefenderPlacedSprite(TYPE); }

    /// <summary>
    /// Returns a Defender GameObject that can be placed on the
    /// TileGrid.
    /// </summary>
    /// <returns>a Defender GameObject that can be placed on the
    /// TileGrid.</returns>
    public override GameObject MakePlaceableObject() { return Instantiate(DefenderFactory.GetDefenderPrefab(TYPE)); }

    /// <summary>
    /// Returns a GameObject that holds a SpriteRenderer component with
    /// this Defender's placed Sprite. No other components are
    /// copied. 
    /// </summary>
    /// <returns>A GameObject with a SpriteRenderer component. </returns>
    public override GameObject MakeHollowObject()
    {
        GameObject baseResult = base.MakeHollowObject();
        baseResult.GetComponent<SpriteRenderer>().sprite = DefenderFactory.GetDefenderPlacedSprite(TYPE);
        return baseResult;
    }
}
