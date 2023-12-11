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
    /// Checks if this HazardController's Hazard should be removed from
    /// the game. If so, clears it.
    /// </summary>
    protected override void TryRemoveModel()
    {
        if (!ValidModel()) return;
        if (!GetHazard().Expired()) return;

        GetHazard().OnDie();
        GameObject.Destroy(GetHazard().gameObject);
        GameObject.Destroy(GetHazard());

        //We are done with our Hazard.
        RemoveModel();
    }
}
