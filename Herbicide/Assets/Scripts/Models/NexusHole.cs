using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a Model from which Enemies spawn.
/// </summary>
public class NexusHole : Mob
{
    #region Fields

    /// <summary>
    /// All Nexii in this NexusHole.
    /// </summary>
    private List<Nexus> occupants = new List<Nexus>();

    #endregion

    #region Stats

    /// <summary>
    /// Type of a NexusHole.
    /// </summary>
    public override ModelType TYPE => ModelType.NEXUS_HOLE;

    /// <summary>
    /// Starting attack range of a NexusHole.
    /// </summary>
    public override float BASE_ATTACK_RANGE => 0f;

    /// <summary>
    /// Maximum attack range of a NexusHole.
    /// </summary>
    public override float MAX_ATTACK_RANGE => float.MaxValue;

    /// <summary>
    /// Minimum attack range of a NexusHole.
    /// </summary>
    public override float MIN_ATTACK_RANGE => 0f;

    /// <summary>
    /// Starting attack cooldown of a NexusHole.
    /// </summary>
    public override float BASE_ATTACK_SPEED => float.MaxValue;

    /// <summary>
    /// Maximum attack cooldown of a NexusHole.
    /// </summary>
    public override float MAX_ATTACK_SPEED => float.MaxValue;

    /// <summary>
    /// Starting chase range of a NexusHole.
    /// </summary>
    public override float BASE_CHASE_RANGE => 0f;

    /// <summary>
    /// Maximum chase range of a NexusHole.
    /// </summary>
    public override float MAX_CHASE_RANGE => float.MaxValue;

    /// <summary>
    /// Minimum chase range of a NexusHole.
    /// </summary>
    public override float MIN_CHASE_RANGE => 0f;

    /// <summary>
    /// Starting movement speed of a NexusHole.
    /// </summary>
    public override float BASE_MOVEMENT_SPEED => 0f;

    /// <summary>
    /// Maximum movement speed of a NexusHole.
    /// </summary>
    public override float MAX_MOVEMENT_SPEED => float.MaxValue;

    /// <summary>
    /// Minimum movement speed of a NexusHole.
    /// </summary>
    public override float MIN_MOVEMENT_SPEED => 0f;

    /// <summary>
    /// Cost to place a NexusHole.
    /// </summary>
    public override int COST => int.MaxValue;

    /// <summary>
    /// Starting health of a NexusHole.
    /// </summary>
    public override float BASE_HEALTH => 100;

    /// <summary>
    /// Maximum health of a NexusHole.
    /// </summary>
    public override float MAX_HEALTH => int.MaxValue;

    /// <summary>
    /// Minimum health of a NexusHole.
    /// </summary>
    public override float MIN_HEALTH => 0;

    /// <summary>
    /// NexusHoles do not occupy Tiles.
    /// </summary>
    public override bool OCCUPIER => false;

    #endregion

    #region Methods

    /// <summary>
    /// Returns the Sprite that represents this NexusHole when placing.
    /// </summary>
    /// <returns> the Sprite that represents this NexusHole when placing.
    /// </returns>
    public override Sprite[] GetPlacementTrack() => NexusHoleFactory.GetPlacementTrack();

    /// <summary>
    /// Returns an instantiated copy of this NexusHole.
    /// </summary>
    /// <returns>an instantiated copy of this NexusHole.</returns>
    public override GameObject Copy() => NexusHoleFactory.GetNexusHolePrefab();

    /// <summary>
    /// Sets the 2D Collider properties of this NexusHole.
    /// </summary>
    public override void SetColliderProperties() { }

    #endregion
}
