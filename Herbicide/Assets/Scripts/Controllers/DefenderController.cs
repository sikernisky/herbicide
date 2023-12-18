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
/// <typeparam name="T">Enum to represent state of the Defender.</typeparam>
public abstract class DefenderController<T> : MobController<T> where T : Enum
{
    /// <summary>
    /// Total number of DefenderControllers created during this level so far.
    /// </summary>
    private static int NUM_DEFENDERS;

    /// <summary>
    /// The TripleThreat Synergy applied to the Defender.
    /// </summary>
    private TripleThreat tripleThreat;

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
    /// Queries the SynergyController to determine which Synergies are
    /// active. Performs logic based on the active Synergies.
    /// </summary>
    protected override void UpdateSynergies()
    {
        base.UpdateSynergies();
        tripleThreat.ChangeTier(SynergyController.
            GetSynergyTier(SynergyController.Synergy.TRIPLE_THREAT));
    }

    /// <summary>
    /// Adds to the Defender all Synergy effects that could affect it.
    /// </summary>
    protected override void ApplySynergies()
    {
        base.ApplySynergies();
        tripleThreat = new TripleThreat(GetModel(), 0);
        GetModel().ApplyEffect(tripleThreat);
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
    /// Returns true if this controller's Defender should be destoyed and
    /// set to null.
    /// </summary>
    /// <returns>true if this controller's Defender should be destoyed and
    /// set to null; otherwise, false.</returns>
    protected override bool ShouldRemoveModel()
    {
        if (GetDefender().GetHealth() <= 0) return true;

        return false;
    }
}


