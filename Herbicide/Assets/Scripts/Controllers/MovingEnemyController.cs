using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Controls a MovingEnemy.
/// </summary>
public class MovingEnemyController : EnemyController
{
    /// <summary>
    /// Initializes this MovingEnemyController with the MovingEnemy it controls.
    /// </summary>
    /// <param name="movingEnemy">The MovingEnemy this MovingEnemyController controls.</param>
    /// <param name="spawnPosition">The position at which to spawn the MovingEnemy. </param>
    ///<param name="id">the unique ID of this EnemyController</param>
    public MovingEnemyController(MovingEnemy movingEnemy, Vector3 spawnPosition, int id)
     : base(movingEnemy, spawnPosition, id)
    {
        return;
    }

    /// <summary>
    /// Updates the MovingEnemy controlled by this Controller.
    /// </summary>
    public override void UpdateEnemy()
    {
        MovingEnemy enemy = GetEnemy() as MovingEnemy;
        Assert.IsNotNull(enemy);
    }


}
