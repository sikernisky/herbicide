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
    /// Target of the Enemy controlled by this EnemyController
    /// </summary>
    private PlaceableObject target;


    /// <summary>
    /// FSM to represent an Enemy's current state
    /// </summary>
    protected enum EnemyState
    {
        SPAWN,
        CHASE,
        ATTACK,
        INVALID
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
        this.enemy.Spawn();
        enemyOb.transform.position = spawnPosition;

        SetState(EnemyState.SPAWN);
    }

    /// <summary>
    /// Returns the Enemy controlled by this EnemyController.
    /// </summary>
    /// <returns>the Enemy controlled by this EnemyController.</returns>
    protected Enemy GetEnemy()
    {
        if (!ValidEnemy()) return null;

        return enemy;
    }

    /// <summary>
    /// Updates the Enemy controlled by this EnemyController.
    /// </summary>
    public virtual void UpdateEnemy(List<PlaceableObject> targets)
    {
        if (!ValidEnemy()) return;
        if (targets == null) return;

        if (!TileGrid.OnTile(enemy.transform.position)) KillEnemy();
        UpdateState();
        SelectTarget(targets);
    }

    /// <summary>
    /// Updates the state of the Enemy controlled by this EnemyController.
    /// </summary>
    protected abstract void UpdateState();

    /// <summary>
    /// Selects the Enemy's target.
    /// </summary>
    /// <param name="targets"> all possible targets for this MovingEnemy to select.
    /// </param>
    protected abstract void SelectTarget(List<PlaceableObject> targets);

    /// <summary>
    /// Returns true if the Enemy controlled by this EnemyController
    /// can target a PlaceableObject.
    /// </summary>
    /// <param name="candidate">the target to check</param>
    /// <returns>true if the Enemy controlled by this EnemyController
    /// can target a PlaceableObject; otherwise, false.</returns>
    protected virtual bool CanTarget(PlaceableObject candidate)
    {
        if (!ValidEnemy()) return false;
        if (candidate == null) return false;

        return true;
    }

    /// <summary>
    /// Sets the Enemy controlled by this EnemyController's target.
    /// </summary>
    /// <param name="target">the new target</param>
    protected void SetTarget(PlaceableObject target)
    {
        if (!ValidEnemy()) return;

        this.target = target;
    }
    /// <summary>
    /// Returns the Enemy controlled by this EnemyController's target.
    /// </summary>
    /// <returns>the Enemy controlled by this EnemyController's 
    /// target.</returns>
    protected PlaceableObject GetTarget()
    {
        if (!ValidEnemy()) return null;

        return target;
    }

    /// <summary>
    /// Sets the State of this EnemyController.
    /// </summary>
    /// <param name="newState">The state to set to.</param>
    protected void SetState(EnemyState newState)
    {
        if (!ValidEnemy()) return;

        state = newState;
    }

    /// <summary>
    /// Returns the state of this EnemyController.
    /// </summary>
    /// <returns>the state of this EnemyController.</returns>
    protected EnemyState GetState()
    {
        if (!ValidEnemy()) return EnemyState.INVALID;

        return state;
    }

    /// <summary>
    /// Returns the world distance between the Enemy controlled by this
    /// EnemyController and its target.
    /// </summary>
    /// <returns>the world distance between the Enemy controlled by this
    /// EnemyController and its target.</returns>
    protected float DistanceToTarget()
    {
        if (!ValidEnemy()) return float.MaxValue;
        if (target == null) return float.MaxValue;

        Vector3 enemyPosition = enemy.transform.position;
        Vector3 targetPosition = target.transform.position;
        return Vector3.Distance(enemyPosition, targetPosition);
    }

    /// <summary>
    /// Kills the Enemy controlled by this EnemyController.
    /// </summary>
    protected virtual void KillEnemy()
    {
        if (!ValidEnemy()) return;

        enemy.OnDie();
    }


    /// <summary>
    /// Returns true if this EnemyController has a defined, active Enemy.
    /// </summary>
    /// <returns>true if this EnemyController has a defined, active Enemy;
    /// otherwise, false.</returns>
    protected bool ValidEnemy()
    {
        return enemy != null;
    }
}
