using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Represents a Structure that Enemies target.
/// </summary>
public class Nexus : Mob
{
    #region Fields

    /// <summary>
    /// true if this Nexus was brought to the target position.
    /// </summary>
    private bool cashedIn;

    /// <summary>
    /// The Transform of the Model that picked this Nexus up; null if
    /// it is not picked up.
    /// </summary>
    private Transform holder;

    /// <summary>
    /// The HOLDER_OFFSET of the model that picked this Model up.
    /// </summary>
    private Vector2 holdingOffset;

    #endregion

    #region Stats

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
    public override float BASE_HEALTH => 300;

    /// <summary>
    /// Maximum health of a Nexus.
    /// </summary>
    public override float MAX_HEALTH => int.MaxValue;

    /// <summary>
    /// Minimum health of a Nexus.
    /// </summary>
    public override float MIN_HEALTH => 0;

    /// <summary>
    /// true if the Nexus occupies tiles.
    /// </summary>
    public override bool OCCUPIER => false;

    /// <summary>
    /// Size of a Nexus on the TileGrid.
    /// </summary>
    public override Vector2Int SIZE => new Vector2Int(1, 1);

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

    #endregion

    #region Methods

    /// <summary>
    /// Returns true if this Nexus is dead. This is when its
    /// health is <= 0.
    /// </summary>
    /// <returns>true if this Nexus is dead; otherwise, false.
    /// </returns>
    public override bool Dead() => GetHealth() <= 0;

    /// <summary>
    /// Returns the Sprite that represents this Nexus when placing.
    /// </summary>
    /// <returns>the Sprite that represents this Nexus when placing.
    /// </returns>
    public override Sprite[] GetPlacementTrack() => throw new System.NotSupportedException();

    /// <summary>
    /// Returns a GameObject representing this Nexus on the TileGrid.
    /// </summary>
    /// <returns>a GameObject representing this Nexus on the TileGrid.</returns>
    public override GameObject Copy() => NexusFactory.GetNexusPrefab();

    /// <summary>
    /// Sets this Nexus' 2D collider properties.
    /// </summary>
    public override void SetColliderProperties() { }

    /// <summary>
    /// Returns true if this Nexus was brought to the target spot (usually a 
    /// NexusHole).
    /// </summary>
    /// <returns>true if this Nexus was brought to the target spot; otherwise,
    /// false. </returns>
    public bool CashedIn() => cashedIn;

    /// <summary>
    /// Informs this Nexus that it was brought to the target spot.
    /// </summary>
    public void CashIn() => cashedIn = true;

    /// <summary>
    /// Informs this Model that it has been picked up.
    /// </summary>
    /// <param name="holder">The Model that picked it up.</param>
    /// <param name="holdingOffset">The holder offset of the Model that picked it up.</param>
    public void SetPickedUp(Model holder, Vector3 holdingOffset)
    {
        Assert.IsNull(this.holder, "Already picked up.");
        this.holder = holder.transform;
        this.holdingOffset = holdingOffset;
        holder.OnHoldNexus();
        SetSortingLayer(SortingLayers.PICKEDUPITEMS);
    }

    /// <summary>
    /// Informs this Model that it not picked up anymore.
    /// </summary>
    public void SetDropped()
    {
        Assert.IsNotNull(holder, "Not picked up.");
        holder = null;
        SetSortingLayer(SortingLayers.GROUNDMOBS);
    }

    /// <summary>
    /// Returns true if this Model is picked up.
    /// </summary>
    /// <returns> true if this Model is picked up; otherwise, false.</returns>
    public bool PickedUp() => holder != null;

    /// <summary>
    /// Returns the position of this Model when held. This is the position of
    /// the Model holding it + the Model holding it's HOLDER_OFFSET.
    /// </summary>
    /// <returns>the HOLDER_OFFSET of the Model that picked this Model up.</returns>
    public Vector2 GetHeldPosition() => holder.position + new Vector3(holdingOffset.x, holdingOffset.y, 1);

    #endregion
}
