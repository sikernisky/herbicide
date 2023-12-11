using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a SlowZone that appears
/// after a Butterfly's Bomb explodes.
/// </summary>
public class BombSplat : SlowZone
{
    /// <summary>
    /// How many seconds a BombSplat lasts in the scene.
    /// </summary>
    public override float LIFESPAN => 3f;

    /// <summary>
    /// Type of the BombSplat Hazard.
    /// </summary>
    public override HazardType TYPE => Hazard.HazardType.BOMB_SPLAT;

    /// <summary>
    /// Starting health of a BombSplat.
    /// </summary>
    public override int BASE_HEALTH => 100;

    /// <summary>
    /// Name of a BombSplat.
    /// </summary>
    public override string NAME => "BombSplat";

    /// <summary>
    /// Starting amount, between 0 and 1, by which this
    /// BombSplat slows its target.
    /// </summary>
    public override float BASE_SLOW_RATE => 0.5f;


    /// <summary>
    /// Returns the Sprite that represents a BombSplat in
    /// the inventory.
    /// </summary>
    /// <returns>the Sprite that represents a BombSplat in
    /// the inventory.</returns>
    public override Sprite GetInventorySprite() { throw new System.NotImplementedException(); }

    /// <summary>
    /// Returns the Sprite that represents a BombSplat when
    /// placing from the Inventory.
    /// </summary>
    /// <returns>the Sprite that represents a BombSplat when 
    /// placing from the Inventory.</returns>
    public override Sprite GetPlacementSprite() { throw new System.NotImplementedException(); }

    /// <summary>
    /// Returns a BombSplat GameObject that can be placed
    /// on the TileGrid. 
    /// </summary>
    /// <returns>a BombSplat GameObject that can be placed
    /// on the TileGrid. </returns>
    public override GameObject MakePlaceableObject()
    {
        return Instantiate(HazardFactory.GetHazardPrefab(HazardType.BOMB_SPLAT));
    }

    /// <summary>
    /// Called when a BombSplat dies.
    /// </summary>
    public override void OnDie() { return; }

    /// <summary>
    /// Sets the BombSplat's 2D Collider properties.
    /// </summary>
    public override void SetColliderProperties() { return; }
}
