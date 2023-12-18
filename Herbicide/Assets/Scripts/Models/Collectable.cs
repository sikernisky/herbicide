using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents something that the player can click on
/// to aquire/collect. 
/// </summary>
public abstract class Collectable : Model
{
    /// <summary>
    /// CollectableType of this Collectable.
    /// </summary>
    public abstract CollectableType TYPE { get; }

    /// <summary>
    /// Seconds to complete a bob cycle.
    /// </summary>
    public virtual float BOB_SPEED => 3f;

    /// <summary>
    /// How "tall" this Currency bobs.
    /// </summary>
    public virtual float BOB_HEIGHT => .15f;

    /// <summary>
    /// true if the player collected this Collectable; false otherwise
    /// </summary>
    private bool collected;

    /// <summary>
    /// Types of Collectables.
    /// </summary>
    public enum CollectableType
    {
        SEED_TOKEN,
        DEW
    }

    /// <summary>
    /// Does something when the player collects this Collectable.
    /// </summary>
    public virtual void OnCollect()
    {
        if (collected) return;
        collected = true;
    }

    /// <summary>
    /// Returns true if the player picked up this Collectable;
    /// otherwise, false.
    /// </summary>
    /// <returns>true if the player picked up this Collectable;
    /// otherwise, false.</returns>
    public bool PickedUp() { return collected; }

    /// <summary>
    /// Returns the Sprite component that represents this Collectable in
    /// the Inventory.
    /// </summary>
    /// <returns>the Sprite component that represents this Collectable in
    /// the Inventory.</returns>
    public override Sprite GetInventorySprite()
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Returns a Sprite that represents this Collectable when it is
    /// being placed.
    /// </summary>
    /// <returns> a Sprite that represents this Collectable when it is
    /// being placed.</returns>
    public override Sprite GetPlacementSprite()
    {
        throw new System.NotImplementedException();
    }

}
