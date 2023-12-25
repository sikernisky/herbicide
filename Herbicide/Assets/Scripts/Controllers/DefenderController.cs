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
    /// Returns true if the Defender can target the PlaceableObject passed
    /// into this method.
    /// </summary>
    /// <param name="target">The Placeable object to check for targetability.</param>
    /// <returns></returns>
    protected override bool CanTarget(PlaceableObject target)
    {
        if (target == null) return false;
        if (target as Enemy == null) return false;
        if (!target.Targetable()) return false;

        return true;
    }

    /// <summary>
    /// Returns true if this controller's Defender should be destoyed and
    /// set to null.
    /// </summary>
    /// <returns>true if this controller's Defender should be destoyed and
    /// set to null; otherwise, false.</returns>
    protected override bool ShouldRemoveModel()
    {
        if (GetDefender().Dead()) return true;

        return false;
    }
}


