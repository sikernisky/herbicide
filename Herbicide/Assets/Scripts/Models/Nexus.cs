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
    public override ModelType TYPE => ModelType.NEXUS;

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
    public override float BASE_ATTACK_SPEED => float.MaxValue;

    /// <summary>
    /// Maximum attack cooldown of a Nexus.
    /// </summary>
    public override float MAX_ATTACK_SPEED => float.MaxValue;

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
    /// A Nexus can be held.
    /// </summary>
    public override bool HOLDABLE => true;

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
    /// Returns the Sprite that represents this Nexus when placing.
    /// </summary>
    /// <returns>the Sprite that represents this Nexus when placing.
    /// </returns>
    public override Sprite[] GetPlacementTrack() { return null; }

    /// <summary>
    /// Returns a GameObject representing this Nexus on the TileGrid.
    /// </summary>
    /// <returns>a GameObject representing this Nexus on the TileGrid.</returns>
    public override GameObject Copy() { return NexusFactory.GetNexusPrefab(); }

    /// <summary>
    /// Sets this Nexus' 2D collider properties.
    /// </summary>
    public override void SetColliderProperties() { return; }

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
}
