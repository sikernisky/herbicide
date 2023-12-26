using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Represents the place where Enemies bring a nexus too.
/// </summary>
public class NexusHole : Structure
{
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
    public override float BASE_ATTACK_COOLDOWN => float.MaxValue;

    /// <summary>
    /// Maximum attack cooldown of a NexusHole.
    /// </summary>
    public override float MAX_ATTACK_COOLDOWN => float.MaxValue;

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
    public override int BASE_HEALTH => 100;

    /// <summary>
    /// Maximum health of a NexusHole.
    /// </summary>
    public override int MAX_HEALTH => int.MaxValue;

    /// <summary>
    /// Minimum health of a NexusHole.
    /// </summary>
    public override int MIN_HEALTH => 0;

    /// <summary>
    /// Name of a NexusHole.
    /// </summary>
    public override string NAME => "Nexus Hole";

    /// <summary>
    /// NexusHoles do not occupy Tiles.
    /// </summary>
    public override bool OCCUPIER => false;

    /// <summary>
    /// All Nexii in this NexusHole.
    /// </summary>
    private List<Nexus> occupants = new List<Nexus>();

    /// <summary>
    /// Maximum number of Nexii that can be in this NexusHole.
    /// </summary>
    protected virtual int MAX_OCCUPANTS => 1;


    /// <summary>
    /// Returns the Sprite that represents this NexusHole on a boat.
    /// </summary>
    /// <returns> the Sprite that represents this NexusHole on a boat.
    /// </returns>
    public override Sprite[] GetBoatTrack() { return NexusHoleFactory.GetBoatTrack(); }

    /// <summary>
    /// Returns the Sprite that represents this NexusHole when placing.
    /// </summary>
    /// <returns> the Sprite that represents this NexusHole when placing.
    /// </returns>
    public override Sprite[] GetPlacementTrack() { return NexusHoleFactory.GetPlacementTrack(); }

    /// <summary>
    /// Returns an instantiated copy of this NexusHole.
    /// </summary>
    /// <returns>an instantiated copy of this NexusHole.</returns>
    public override GameObject Copy()
    {
        return Instantiate(NexusHoleFactory.GetNexusHolePrefab());
    }

    /// <summary>
    /// Sets the 2D Collider properties of this NexusHole.
    /// </summary>
    public override void SetColliderProperties() { return; }

    /// <summary>
    /// Returns true if no more Nexii can fit in this NexusHole.
    /// </summary>
    /// <returns>true if this NexusHole has reached capacity;
    /// otherwise, false. </returns>
    public bool Filled() { return occupants.Count >= MAX_OCCUPANTS; }

    /// <summary>
    /// Puts a Nexus in this NexusHole.
    /// </summary>
    /// <param name="nexus">The Nexus to fill with.</param>
    public void Fill(Nexus nexus)
    {
        Assert.IsFalse(Filled(), "You need to check if this NexusHole is filled.");
        occupants.Add(nexus);
    }

    /// <summary>
    /// Removes a Nexus from this NexusHole.
    /// </summary>
    /// <param name="nexus">The Nexus to remove.</param>
    public void RemoveNexus(Nexus nexus)
    {
        if (!occupants.Contains(nexus)) return;
        occupants.Remove(nexus);
    }
}
