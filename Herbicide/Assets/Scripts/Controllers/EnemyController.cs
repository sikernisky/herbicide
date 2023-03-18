using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Controls an Enemy.
/// </summary>
public abstract class EnemyController
{
    /// <summary>
    /// The Enemy controlled by this EnemyController
    /// </summary>
    private Enemy enemy;

    /// <summary>
    /// The current state of this EnemyController's enemy
    /// </summary>
    private EnemyState state;

    /// <summary>
    /// Unique ID of this EnemyController.
    /// </summary>
    private int id;

    /// <summary>
    /// FSM to represent an Enemy's current state
    /// </summary>
    protected enum EnemyState
    {
        SPAWN,
        CHASE,
        ATTACK
    }

    /// <summary>
    /// Initializes this EnemyController with the Enemy it controls.
    /// </summary>
    /// <param name="enemy">the enemy this EnemyController controls.</param>
    /// <param name="spawnPosition">the position to spawn the Enemy</param>
    ///<param name="id">the unique ID of this EnemyController</param>
    public EnemyController(Enemy enemy, Vector3 spawnPosition, int id)
    {
        //Safety checks
        Assert.IsNotNull(enemy);

        this.id = id;
        GameObject enemyOb = enemy.CloneEnemy();
        Enemy newEnemy = enemyOb.GetComponent<Enemy>();
        Assert.IsNotNull(newEnemy);

        this.enemy = newEnemy;
        Debug.Log("Controller with Id: " + id + " is spawning.");
        this.enemy.OnSpawn();
        enemyOb.transform.position = spawnPosition;
    }

    /// <summary>
    /// Returns the Enemy controlled by this EnemyController.
    /// </summary>
    /// <returns>the Enemy controlled by this EnemyController.</returns>
    protected Enemy GetEnemy()
    {
        return enemy;
    }

    /// <summary>
    /// Updates the Enemy controlled by this EnemyController.
    /// </summary>
    public abstract void UpdateEnemy();
}
