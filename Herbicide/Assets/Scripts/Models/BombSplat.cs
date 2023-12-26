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
    public override ModelType TYPE => ModelType.BOMB_SPLAT;

    /// <summary>
    /// Starting health of a BombSplat.
    /// </summary>
    public override int BASE_HEALTH => 100;

    /// <summary>
    /// Name of a BombSplat.
    /// </summary>
    public override string NAME => "BombSplat";

    /// <summary>
    /// BombSplat Hazards occupy Tiles.
    /// </summary>
    public override bool OCCUPIER => false;


    /// <summary>
    /// Returns the Sprite that represents a BombSplat in
    /// the inventory.
    /// </summary>
    /// <returns>the Sprite that represents a BombSplat in
    /// the inventory.</returns>
    public override Sprite[] GetBoatTrack() { return BombSplatFactory.GetBoatTrack(); }

    /// <summary>
    /// Returns the Sprite that represents a BombSplat when
    /// placing from the Inventory.
    /// </summary>
    /// <returns>the Sprite that represents a BombSplat when 
    /// placing from the Inventory.</returns>
    public override Sprite[] GetPlacementTrack() { return BombSplatFactory.GetPlacementTrack(); }

    /// <summary>
    /// Returns a BombSplat GameObject that can be placed
    /// on the TileGrid. 
    /// </summary>
    /// <returns>a BombSplat GameObject that can be placed
    /// on the TileGrid. </returns>
    public override GameObject Copy()
    {
        return Instantiate(BombSplatFactory.GetBombSplatPrefab());
    }

    /// <summary>
    /// Sets the BombSplat's 2D Collider properties.
    /// </summary>
    public override void SetColliderProperties() { return; }
}
