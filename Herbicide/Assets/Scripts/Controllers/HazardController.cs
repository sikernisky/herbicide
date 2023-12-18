using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Assertions;

/// <summary>
/// Abstract class to represent controllers of Hazards.
/// 
/// The HazardController is responsible for manipulating its Hazared and bringing
/// it to life. This includes moving it, choosing targets, playing animations,
/// and more.
/// </summary>
/// <typeparam name="T">Enum to represent state of the Hazard.</typeparam>
public abstract class HazardController<T> : MobController<T> where T : Enum
{
    /// <summary>
    /// Number of Hazards created since the scene began.
    /// </summary>
    private static int NUM_HAZARDS;

    /// <summary>
    /// Initializes this HazardController with the Hazard it controls.
    /// </summary>
    /// <param name="hazard">the enemy this EnemyController controls.</param>
    /// <param name="startPos">where the Hazard starts.</param>

    protected HazardController(Hazard hazard) : base(hazard)
    {
        Assert.IsNotNull(hazard, "Hazard cannot be null.");
        NUM_HAZARDS++;
    }

    /// <summary>
    /// Returns this HazardController's Hazard model.
    /// </summary>
    /// <returns>this HazardController's Hazard model.</returns>
    protected Hazard GetHazard() { return GetMob() as Hazard; }

    /// <summary>
    /// Returns true if this controller's Hazard should be destoyed and
    /// set to null.
    /// </summary>
    /// <returns>true if this controller's Hazard should be destoyed and
    /// set to null; otherwise, false.</returns>
    protected override bool ShouldRemoveModel()
    {
        if (GetHazard().Expired()) return true;

        return false;
    }
}
