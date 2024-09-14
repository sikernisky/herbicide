using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Controls a Defender. <br></br>
/// 
/// The DefenderController is responsible for manipulating its Defender and bringing
/// it to life. This includes moving it, choosing targets, playing animations,
/// and more.
/// </summary>
/// <typeparam name="T">The type of enum that represents distinct states or attributes
/// of the Defender.</typeparam>
public abstract class DefenderController<T> : MobController<T> where T : Enum
{
    #region Fields

    /// <summary>
    /// true if the Squirrel executed one call of its Spawn state;
    /// otherwise, false.
    /// </summary>
    private bool spawnStateDone;

    /// <summary>
    /// Defenders find targets.
    /// </summary>
    protected override bool FINDS_TARGETS => true;

    #endregion

    #region Methods

    /// <summary>
    /// Makes a new DefenderController for a Defender.
    /// </summary>
    /// <param name="defender">The Defender controlled by this
    ///  DefenderController.</param>
    public DefenderController(Defender defender) : base(defender) { }

    /// <summary>
    /// Returns this DefenderController's Defender reference. 
    /// </summary>
    /// <returns>the reference to this DefenderController's defender.</returns>
    protected Defender GetDefender() => GetMob() as Defender;

    /// <summary>
    /// Returns true if the Defender can target the Model passed
    /// into this method.
    /// </summary>
    /// <param name="target">The Model object to check for targetability.</param>
    /// <returns></returns>
    protected override bool CanTargetOtherModel(Model target)
    {
        Enemy enemyTarget = target as Enemy;

        if (target == null) return false;
        if (enemyTarget == null) return false;
        if (!enemyTarget.Spawned()) return false;
        if (!enemyTarget.Targetable()) return false;
        if (!GetDefender().IsPlaced()) return false;

        Vector3 treePos = GetDefender().GetTreePosition();
        float distanceFromTree = Vector3.Distance(treePos, enemyTarget.GetPosition());
        if (distanceFromTree > GetDefender().GetAttackRange()) return false;

        return true;
    }

    /// <summary>
    /// Returns true if this controller's Defender should be destoyed and
    /// set to null.
    /// </summary>
    /// <returns>true if this controller's Defender should be destoyed and
    /// set to null; otherwise, false.</returns>
    public override bool ValidModel() => !GetDefender().Dead();
    
    /// <summary>
    /// Performs logic for this Defender's SPAWN state.
    /// </summary>
    protected virtual void ExecuteSpawnState() => spawnStateDone = true;

    /// <summary>
    /// Returns true if the SpawnState has completed one execution.
    /// </summary>
    /// <returns>true if the SpawnState has completed one execution;
    /// otherwise, false. </returns>
    protected bool SpawnStateDone() => spawnStateDone;

    /// <summary>
    /// Returns the distance from the Defender's tree to its first
    /// target.
    /// </summary>
    /// <returns>the distance from the Defender's tree to its first
    /// target. </returns>
    protected float DistanceToTargetFromTree()
    {
        Assert.IsNotNull(GetTargets(), "Targets is null.");
        Model target = GetTargets()[0];
        Assert.IsNotNull(target, "Target is null.");

        return Vector3.Distance(GetDefender().GetTreePosition(), target.GetPosition());
    }

    /// <summary>
    /// Returns the distance from the Defender's tree to one of its
    /// targets.
    /// </summary>
    /// <returns>the distance from the Defender's tree to one of its
    /// targets. </returns>
    protected float DistanceToTargetFromTree(Model target)
    {
        Assert.IsNotNull(target);
        Assert.IsTrue(GetTargets().Contains(target));

        return Vector3.Distance(GetDefender().GetTreePosition(), target.GetPosition());
    }

    /// <summary>
    /// Sorts the targets list. They are ordered by targeting priority.
    /// </summary>
    /// <param name="targets">The list of targets to sort. </param>
    protected override void SortTargets(List<Model> targets)
    {
        base.SortTargets(targets);

        // Sort in the following order:
        // (1) Enemies holding a Nexus
        // (2) Enemies closest, by the TileGrid's walkable pathfinding logic, to a Nexus
        targets.Sort((a, b) =>
        {
            Enemy enemyA = a as Enemy;
            Enemy enemyB = b as Enemy;

            if (enemyA.IsHoldingNexus() && !enemyB.IsHoldingNexus()) return -1;
            if (!enemyA.IsHoldingNexus() && enemyB.IsHoldingNexus()) return 1;

            float distanceA = DistanceToTargetFromTree(enemyA);
            float distanceB = DistanceToTargetFromTree(enemyB);

            return distanceA.CompareTo(distanceB);
        });
    }

    /// <summary>
    /// Returns the Defender prefab to the DefenderFactory object pool.
    /// </summary>
    public override void ReturnModelToFactory()
    {
        GetDefender().ResetTier();
        DefenderFactory.ReturnDefenderPrefab(GetDefender().gameObject);
    }

    #endregion
}


