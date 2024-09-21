using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using System.Linq;

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

    /// <summary>
    /// The Defender's current target. We need this to implement
    /// sticky targeting.
    /// </summary>
    private Enemy stickyTarget;

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
        if (DistanceToPositionFromTree(enemyTarget.GetPosition()) > GetDefender().GetMainActionRange()) return false;

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
        return DistanceToPositionFromTree(target.GetPosition());
    }

    /// <summary>
    /// Returns the distance from the Defender's tree to some position.
    /// </summary>
    /// <returns>the distance from the Defender's tree to some position.
    /// </returns>
    protected float DistanceToPositionFromTree(Vector3 targetPosition)
    {
        float tileScale = TileGrid.TILE_SIZE;
        Vector3 treePosition = GetDefender().GetTreePosition();
        float distance = Vector3.Distance(treePosition, targetPosition);
        return distance / tileScale; // Normalize the distance by the tile scale
    }

    /// <summary>
    /// Sorts the targets list. They are ordered by targeting priority.
    /// </summary>
    /// <param name="targets">The list of targets to sort. </param>
    protected override void SortTargets(List<Model> targets)
    {
        base.SortTargets(targets);

        Assert.IsTrue(targets.All(t => t is Enemy), "Not all targets are valid Enemies.");
        List<Enemy> enemyTargets = targets.Cast<Enemy>().ToList();
        Enemy nexusCarrier = null;

        foreach (Enemy enemy in enemyTargets)
        {
            Assert.IsNotNull(enemy, "Enemy is null.");
            if (enemy.IsHoldingNexus())
            {
                nexusCarrier = enemy;
                break;
            }
        }

        if (nexusCarrier != null) stickyTarget = nexusCarrier;
        else if (stickyTarget != null && enemyTargets.Contains(stickyTarget))
        {
            enemyTargets.Remove(stickyTarget);
            enemyTargets.Insert(0, stickyTarget);
            targets.Clear();
            targets.AddRange(enemyTargets);
            return;
        }
        else if (enemyTargets.Count > 0)
        {
            stickyTarget = enemyTargets[0];
        }

        if (stickyTarget != null)
        {
            enemyTargets.Remove(stickyTarget); 
            enemyTargets.Insert(0, stickyTarget);
        }

        targets.Clear();
        targets.AddRange(enemyTargets);
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


