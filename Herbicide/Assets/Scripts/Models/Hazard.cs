using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents something on the TileGrid that can affect
/// ITargetables in some way. Hazards are usually temporary
/// and immobile.
/// </summary>
public abstract class Hazard : Mob
{
    /// <summary>
    /// How long the Hazard lasts in the scene.
    /// </summary>
    public abstract float LIFESPAN { get; }

    /// <summary>
    /// How long this Hazard has been active
    /// </summary>
    private float age;

    /// <summary>
    /// Cost to place a SlowZone from the Inventory.
    /// </summary>
    public override int COST => int.MaxValue;

    /// <summary>
    /// Types of Hazards.
    /// </summary>
    public enum HazardType
    {
        BOMB_SPLAT
    }


    /// <summary>
    /// Adds to this Hazard's current age.
    /// </summary>
    /// <param name="time">the amount of time to add.</param>
    public void AddToLifespan(float time) { age = time <= 0 ? age : age + time; }

    /// <summary>
    /// Returns true if this Hazard has hit its lifespan.
    /// </summary>
    /// <returns>true if this Hazard has hit its lifespan;
    /// otherwise, false.</returns>
    public bool Expired() { return age >= LIFESPAN; }
}
