using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Abstract class to represent controllers of Defenders.
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
    protected override void UpdateMob(List<ITargetable> targets)
    {
        if (!ValidModel()) return;
        if (GetGameState() != GameState.ONGOING) return;
        UpdateStateFSM();
        ElectTarget(FilterTargets(targets));
        ExecuteAttackState();
        ExecuteIdleState();
    }

    /// <summary>
    /// Returns this DefenderController's Defender reference. 
    /// </summary>
    /// <returns>the reference to this DefenderController's defender.</returns>
    private Defender GetDefender() { return GetMob() as Defender; }

    /// <summary>
    /// Sets this DefenderController's target from a filtered list of ITargetables.
    /// </summary>
    /// <param name="filteredTargetables">a list of ITargables that this DefenderController
    /// is allowed to set as its target. /// </param>
    protected override void ElectTarget(List<ITargetable> filteredTargetables)
    {
        if (!ValidModel()) return;
        if (GetTarget() != null) return;
        Assert.IsNotNull(filteredTargetables, "List of targets is null.");

        int random = UnityEngine.Random.Range(0, filteredTargetables.Count);
        if (filteredTargetables.Count == 0) SetTarget(null);
        else SetTarget(filteredTargetables[random]);
    }

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
            if (target as Enemy == null) continue;
            float distanceToTarget = GetDefender().DistanceToTarget(target);
            if (distanceToTarget > GetDefender().GetAttackRange()) continue;
            filteredTargets.Add(target);
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
    }

    //----------------------STATE LOGIC-----------------------//

    /// <summary>
    /// Runs all code relevant to the Defender's attacking state.
    /// </summary>
    protected abstract void ExecuteAttackState();
}


