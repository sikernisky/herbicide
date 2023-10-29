using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Controller for a Kudzu Enemy.
/// </summary>
public class KudzuController : EnemyController
{

    /// <summary>
    /// Makes a new KudzuController.
    /// </summary>
    /// <param name="defender">The Kudzu Enemy. </param>
    /// <returns>The created KudzuController.</returns>
    public KudzuController(Enemy enemy, float spawnTime, Vector2 spawnCoords)
    : base(enemy, spawnTime, spawnCoords)
    {
        Assert.IsNotNull(enemy as Kudzu, "KudzuControllers must be " +
            "assigned Kudzu Enemies.");
    }

    /// <summary>
    /// Returns true if the Enemy this KudzuController controls
    /// is a Kudzu and not null.
    /// </summary>
    /// <returns>true if this KudzuController controls a valid Kudzu;
    /// otherwise, false./// </returns>
    protected override bool ValidEnemy()
    {
        return base.ValidEnemy() && GetEnemy() as Kudzu != null;
    }

}
