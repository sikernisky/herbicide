using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Represents something the Enemies are trying to destroy.
/// </summary>
public class Nexus : Structure
{
    /// <summary>
    /// The StructureType of a Nexus.
    /// </summary>
    public override StructureType TYPE => StructureType.NEXUS;

    /// <summary>
    /// Cost to place a Nexus.
    /// </summary>
    public override int COST => 0;

    /// <summary>
    /// Starting health of a Nexus.
    /// </summary>
    public override int BASE_HEALTH => 300;

    /// <summary>
    /// Maximum health of a Nexus.
    /// </summary>
    public override int MAX_HEALTH => int.MaxValue;

    /// <summary>
    /// Minimum health of a Nexus.
    /// </summary>
    public override int MIN_HEALTH => 0;

    /// <summary>
    /// true if the Nexus occupies tiles.
    /// </summary>
    public override bool OCCUPIER => false;

    /// <summary>
    /// Size of a Nexus on the TileGrid.
    /// </summary>
    public override Vector2Int SIZE => new Vector2Int(1, 1);

    /// <summary>
    /// Name of a Nexus.
    /// </summary>
    public override string NAME => "Nexus";

    /// <summary>
    /// Starting attack range of a Nexus.
    /// </summary>
    public override float BASE_ATTACK_RANGE => 0f;

    /// <summary>
    /// Maximum attack range of a Nexus.
    /// </summary>
    public override float MAX_ATTACK_RANGE => float.MaxValue;

    /// <summary>
    /// Minimum attack range of a Nexus.
    /// </summary>
    public override float MIN_ATTACK_RANGE => 0f;

    /// <summary>
    /// Starting attack cooldown of a Nexus.
    /// </summary>
    public override float BASE_ATTACK_COOLDOWN => float.MaxValue;

    /// <summary>
    /// Maximum attack cooldown of a Nexus.
    /// </summary>
    public override float MAX_ATTACK_COOLDOWN => float.MaxValue;

    /// <summary>
    /// Starting chase range of a Nexus.
    /// </summary>
    public override float BASE_CHASE_RANGE => 0f;

    /// <summary>
    /// Maximum chase range of a Nexus.
    /// </summary>
    public override float MAX_CHASE_RANGE => float.MaxValue;

    /// <summary>
    /// Minimum chase range of a Nexus.
    /// </summary>
    public override float MIN_CHASE_RANGE => 0f;

    /// <summary>
    /// Starting movement speed of a Nexus.
    /// </summary>
    public override float BASE_MOVEMENT_SPEED => 0f;

    /// <summary>
    /// Maximum movement speed of a Nexus.
    /// </summary>
    public override float MAX_MOVEMENT_SPEED => float.MaxValue;

    /// <summary>
    /// Minimum movement speed of a Nexus.
    /// </summary>
    public override float MIN_MOVEMENT_SPEED => 0f;

    /// <summary>
    /// The Transform of the Model that picked this Nexus up; null if
    /// it is not picked up.
    /// </summary>
    private Transform holder;

    /// <summary>
    /// The offset of the holder Transform's position.
    /// </summary>
    private Vector3 holderOffset;

    /// <summary>
    /// true if this Nexus was brought to the target position.
    /// </summary>
    private bool cashedIn;


    /// <summary>
    /// Returns true if this Nexus is dead. This is when its
    /// health is <= 0.
    /// </summary>
    /// <returns>true if this Nexus is dead; otherwise, false.
    /// </returns>
    public override bool Dead() { return GetHealth() <= 0; }

    /// <summary>
    /// Returns the Sprite that represents this Nexus in the inventory.
    /// </summary>
    /// <returns>the Sprite that represents this Nexus in the inventory.
    /// </returns>
    public override Sprite GetInventorySprite() { return null; }

    /// <summary>
    /// Returns the Sprite that represents this Nexus when placing.
    /// </summary>
    /// <returns>the Sprite that represents this Nexus when placing.
    /// </returns>
    public override Sprite GetPlacementSprite() { return null; }

    /// <summary>
    /// Returns a GameObject representing this Nexus on the TileGrid.
    /// </summary>
    /// <returns>a GameObject representing this Nexus on the TileGrid.</returns>
    public override GameObject MakePlaceableObject() { return Instantiate(StructureFactory.GetStructurePrefab(TYPE)); }

    /// <summary>
    /// Sets this Nexus' 2D collider properties.
    /// </summary>
    public override void SetColliderProperties() { return; }

    /// <summary>
    /// Informs this Nexus that it has been picked up.
    /// </summary>
    /// <param name="holder">The transform of the Model that picked it up.</param>
    /// <param name="offset">The amount to add to the transform's position.</param>
    public void PickUp(Transform holder, Vector3 offset)
    {
        Assert.IsNull(this.holder, "Already picked up.");
        this.holder = holder;
        holderOffset = offset;
        SetSortingLayer(SortingLayer.DROPPED_ITEMS);
    }

    /// <summary>
    /// Informs this Nexus that it not picked up anymore.
    /// </summary>
    public void Drop()
    {
        Assert.IsNotNull(holder, "Not picked up.");
        holder = null;
        SetSortingLayer(SortingLayer.DEFENDERS);
    }

    /// <summary>
    /// Returns true if this Nexus is picked up.
    /// </summary>
    /// <returns> true if this Nexus is picked up; otherwise, false.</returns>
    public bool PickedUp() { return holder != null; }

    /// <summary>
    /// Returns true if this Nexus was brought to the target spot (usually a 
    /// NexusHole).
    /// </summary>
    /// <returns>true if this Nexus was brought to the target spot; otherwise,
    /// false. </returns>
    public bool CashedIn() { return cashedIn; }

    /// <summary>
    /// Informs this Nexus that it was brought to the target spot.
    /// </summary>
    public void CashIn() { cashedIn = true; }

    /// <summary>
    /// Returns the position of the Transform holding this Nexus.
    /// </summary>
    /// <returns>the position of the Transform holding this Nexus; null if nothing is
    /// holding this Nexus./// </returns>
    public Vector3 GetHolderPosition() { return holder.position + holderOffset; }
}
