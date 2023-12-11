using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Abstract class to represent controllers of Defenders.
/// 
/// The DefenderController is responsible for manipulating its Defender and bringing
/// it to life. This includes moving it, choosing targets, playing animations,
/// and more.
/// </summary>
public abstract class DefenderController<T> : MobController<T> where T : Enum
{
    /// <summary>
    /// Total number of DefenderControllers created during this level so far.
    /// </summary>
    private static int NUM_DEFENDERS;

    /// <summary>
    /// Makes a new DefenderController for a Defender.
    /// </summary>
    /// <param name="defender">The Defender controlled by this
    ///  DefenderController.</param>
    public DefenderController(Defender defender) : base(defender) { NUM_DEFENDERS++; }

    /// <summary>
    /// Updates the Defender controlled by this DefenderController.
    /// </summary>
    /// <param name="targets">A complete list of ITargetables in the scene.</param>
    protected override void UpdateMob()
    {
        if (!ValidModel()) return;
        base.UpdateMob();

        if (GetGameState() != GameState.ONGOING) return;
    }

    /// <summary>
    /// Returns this DefenderController's Defender reference. 
    /// </summary>
    /// <returns>the reference to this DefenderController's defender.</returns>
    private Defender GetDefender() { return GetMob() as Defender; }

    /// <summary>
    /// Parses the list of all ITargetables in the scene such that it
    /// only contains ITargetables that this DefenderController's Defender is allowed
    /// to target. <br></br><br></br>
    /// 
    /// The Defender is allowed to target Enemy Mobs within its attack range.
    /// </summary>
    /// <param name="targetables">the list of all ITargetables in the scene</param>
    /// <returns>a list containing Enemy ITargetables that this DefenderController's Defender can
    /// reach./// </returns>
    protected override List<ITargetable> FilterTargets(List<ITargetable> targetables)
    {
        Assert.IsNotNull(targetables, "List of targets is null.");
        List<ITargetable> filteredTargets = new List<ITargetable>();
        targetables.RemoveAll(t => t == null);
        foreach (ITargetable target in targetables)
        {
            Enemy targetAsEnemy = target as Enemy;
            if (targetAsEnemy == null) continue;
            if (!targetAsEnemy.Spawned()) continue;
            float distanceToTarget = GetDefender().DistanceToTarget(targetAsEnemy);
            if (distanceToTarget > GetDefender().GetChaseRange()) continue;
            if (!targetAsEnemy.Targetable()) continue;
            filteredTargets.Add(targetAsEnemy);
        }
        return filteredTargets;
    }

    /// <summary>
    /// Checks if this DefenderController's Defender should be removed from
    /// the game. If so, clears it.
    /// </summary>
    protected override void TryRemoveModel()
    {
        if (!ValidModel()) return;
        if (GetDefender().GetHealth() > 0) return;

        GetDefender().OnDie();
        GameObject.Destroy(GetDefender().gameObject);
        GameObject.Destroy(GetDefender());

        //We are done with our Defender.
        RemoveModel();
    }
}


