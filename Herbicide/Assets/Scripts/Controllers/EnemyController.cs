using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System.Linq;

/// <summary>
/// Controls an Enemy.
/// </summary>
public abstract class EnemyController
{
    /// <summary>
    /// Total number of DefenderControllers created during this level so far.
    /// </summary>
    private static int NUM_ENEMIES;

    /// <summary>
    /// The Enemy controlled by this EnemyController
    /// </summary>
    private Enemy enemy;

    /// <summary>
    /// The current state of this EnemyController's enemy
    /// </summary>
    private EnemyState state;

    /// <summary>
    /// Timer for keeping track of attacks per second.
    /// </summary>
    private float attackTimer;

    /// <summary>
    /// Unique ID of this EnemyController.
    /// </summary>
    private int id;

    /// <summary>
    /// Target of the Enemy controlled by this EnemyController
    /// </summary>
    private ITargetable target;

    /// <summary>
    /// The most recent GameState.
    /// </summary>
    private GameState gameState;


    /// <summary>
    /// FSM to represent an Enemy's current state.
    /// </summary>
    public enum EnemyState
    {
        INACTIVE, //This Enemy is ready but not spawned
        SPAWN, //This Enemy has spawned
        CHASE, //This Enemy is moving towards its target
        ATTACK, //This Enemy is attacking its target
        INVALID //Something wrong happened, debug needed
    }

    /// <summary>
    /// Initializes this EnemyController with the Enemy it controls.
    /// </summary>
    /// <param name="enemy">the enemy this EnemyController controls.</param>
    /// <param name="spawnTime">when the Enemy should spawn in the level. </param>
    /// <param name="spawnCoords">where this Enemy should spawn</param>
    public EnemyController(Enemy enemy, float spawnTime, Vector2 spawnCoords)
    {
        //Safety checks
        Assert.IsNotNull(enemy);

        this.id = NUM_ENEMIES;
        GameObject enemyOb = enemy.CloneEnemy();
        Enemy newEnemy = enemyOb.GetComponent<Enemy>();
        Assert.IsNotNull(newEnemy);

        this.enemy = newEnemy;
        this.enemy.SetSpawnTime(spawnTime);
        this.enemy.SetSpawnWorldPosition(spawnCoords);
        this.enemy.gameObject.SetActive(false);
        SetState(EnemyState.INACTIVE);

        NUM_ENEMIES++;
    }

    /// <summary>
    /// Returns the Enemy controlled by this EnemyController.
    /// </summary>
    /// <returns>the Enemy controlled by this EnemyController.</returns>
    public Enemy GetEnemy()
    {
        if (!ValidEnemy()) return null;

        return enemy;
    }

    /// <summary>
    /// Returns this EnemyController's ID.
    /// </summary>
    /// <returns>this EnemyController's ID.</returns>
    public int GetId()
    {
        return id;
    }

    /// <summary>
    /// Updates the Enemy controlled by this EnemyController.
    /// </summary>
    /// <param name="targets">All ITargetables on the grid.</param>
    /// <param name="dt">Current game time.</param>
    public virtual void UpdateEnemy(List<ITargetable> targets, float dt)
    {
        if (!ValidEnemy()) return;
        if (targets == null) return;


        bool readyToSpawn = !GetEnemy().Spawned() && dt >= GetEnemy().GetSpawnTime();
        if (readyToSpawn)
        {
            GetEnemy().gameObject.SetActive(true);
            GetEnemy().OnSpawn();
            SetState(EnemyState.SPAWN);
        }

        if (gameState == GameState.ONGOING && GetEnemy().Spawned())
        {
            UpdateEnemyCooldowns();
            UpdateEnemyTilePosition();
            RectifyEnemySortingOrder();
            SelectTarget(targets);
            UpdateState();
            UpdateEnemyHealthState();
            UpdateEnemyCollider();
            TryAttackTarget();
        }

        if (GetEnemy().Spawned()) TryClearEnemy();
    }

    /// <summary>
    /// Informs this EnemyController of the most recent GameState so
    /// that it knows how to update its Enemy.
    /// </summary>
    /// <param name="state">The most recent GameState.</param>
    public void InformOfGameState(GameState state)
    {
        gameState = state;
    }

    /// <summary>
    /// Checks if this EnemyContoller's enemy should be removed from
    /// the game. If so, clears it.
    /// </summary>
    private void TryClearEnemy()
    {
        //Only try to clear it if it has spawned. 
        if (!GetEnemy().Spawned()) return;

        if (!TileGrid.OnWalkableTile(GetEnemy().GetPosition(), GetEnemy()))
        {
            KillEnemy();
        }

        if (GetEnemy().GetHealth() <= 0)
        {
            KillEnemy();
        }
    }

    /// <summary>
    /// Tries to attack the Enemy's target. This depends on the
    /// Enemy's attack speed and target validity.
    /// </summary>
    public void TryAttackTarget()
    {
        //Safety checks
        if (!ValidEnemy()) return;
        if (target == null) return;

        attackTimer -= Time.deltaTime;
        if (GetState() != EnemyState.ATTACK) return;
        if (attackTimer <= 0)
        {
            attackTimer = 1f / GetEnemy().GetAttackSpeed();
            GetEnemy().Attack(target);
        }
    }

    /// <summary>
    /// Updates the HealthState of the Enemy controlled by this EnemyController.
    /// </summary>
    private void UpdateEnemyHealthState()
    {
        GetEnemy().UpdateHealthState();
    }

    /// <summary>
    /// Updates the state of this EnemyController.
    /// </summary>
    protected abstract void UpdateState();


    /// <summary>
    /// Selects the Enemy's target.
    /// </summary>
    /// <param name="targets"> all possible targets for this MovingEnemy to select.
    /// </param>
    protected virtual void SelectTarget(List<ITargetable> targets)
    {
        Assert.IsNotNull(targets);

        //Reasons we shouldn't choose a new target.
        if (!ValidEnemy()) return;
        if (ValidTarget() && !GetTarget().Dead()) return;
        if (targets.Count == 0) return;

        SetTarget(FindTarget(targets));
    }

    /// <summary>
    /// Sets this Enemy's target: an ITargetable that this EnemyController's Enemy
    /// should target out of a list of all possible targets. This logic is determined
    /// by the Enemy, but this controller method is responsible for ensuring there 
    /// is a valid path towards its selection.
    /// </summary>
    /// <param name="targets">All possible ITargetables to target.</param>
    /// <returns>The ITargetable that this EnemyController's Enemy should target from a
    /// list of possible targets.</returns>
    public virtual ITargetable FindTarget(List<ITargetable> targets)
    {

        targets.RemoveAll(t => t == null);
        List<ITargetable> treeTargets = new List<ITargetable>();
        treeTargets.AddRange(targets.Where(t => t as Tree != null));
        treeTargets.ForEach(t => Assert.IsNotNull(t));
        treeTargets = treeTargets.OrderBy(t => GetEnemy().DistanceToTarget(t)).ToList();

        List<ITargetable> defenderTargets = new List<ITargetable>();
        defenderTargets.AddRange(targets.Where(d => d as Defender != null));
        defenderTargets.ForEach(d => Assert.IsNotNull(d));
        defenderTargets = defenderTargets.OrderBy(d => GetEnemy().DistanceToTarget(d)).ToList();

        //(1) Closest Tree with a Defender
        foreach (Tree t in treeTargets)
        {
            Tree tree = t as Tree;
            if (tree != null && tree.Occupied() && CanTarget(tree)) return tree;
        }

        //(2) Closest Tree without a Defender
        foreach (Tree t in treeTargets)
        {
            Tree tree = t as Tree;
            if (tree != null && CanTarget(tree)) return tree;
        }

        //(3) Closest Defender (SUBJECT TO CHANGE.)
        foreach (Defender d in treeTargets)
        {
            Defender defender = d as Defender;
            if (defender != null && CanTarget(defender)) return defender;
        }

        //(4) Closest.. random... default.
        return targets[0];
    }

    /// <summary>
    /// Returns true if the Enemy controlled by this EnemyController can
    /// target some ITargetable. This method is called when selecting a
    /// target to ensure that measures outside of an Enemy's control
    /// are accounted for. <br></br>
    /// 
    /// For example, MovingEnemyControllers need to ensure their target
    /// has a valid path. But we don't want Enemy models referencing
    /// the TileGrid, so we check in the MovingEnemyController class.
    /// </summary>
    /// <param name="target">the ITargetable to check</param>
    /// <returns>true if this EnemyController's Enemy can target
    /// the ITargetable; otherwise, false. </returns>
    protected virtual bool CanTarget(ITargetable target)
    {
        Assert.IsTrue(ValidEnemy());

        if (target == null) return false;

        return true;
    }

    /// <summary>
    /// Sets the Enemy controlled by this EnemyController's target.
    /// </summary>
    /// <param name="target">the new target</param>
    protected void SetTarget(ITargetable target)
    {
        if (!ValidEnemy()) return;

        this.target = target;
    }

    /// <summary>
    /// Returns the IAttackable that the Enemy controlled by this EnemyController
    /// is targeting.
    /// </summary>
    /// <returns>the IAttackable that the Enemy controlled by this EnemyController
    /// is targeting.</returns>
    protected ITargetable GetTarget()
    {
        if (!ValidEnemy()) return null;

        return target;
    }

    /// <summary>
    /// Returns true if the target of the Enemy controlled by this EnemyController
    /// is alive and not null.
    /// </summary>
    /// <returns>true if the Enemy controlled by this EnemyController's target
    /// is valid; otherwise, false.</returns>
    protected bool ValidTarget()
    {
        return GetTarget() as UnityEngine.Object != null;
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
    /// Kills the Enemy controlled by this EnemyController.
    /// </summary>
    protected virtual void KillEnemy()
    {
        if (!ValidEnemy()) return;

        GetEnemy().Die();
        GameObject.Destroy(GetEnemy().gameObject);
        GameObject.Destroy(GetEnemy());
    }

    /// <summary>
    /// Returns true if this EnemyController is no longer needed and
    /// should be removed by the ControllerController.<br></br>
    /// 
    /// By default, this returns true if the Enemy controlled by this
    /// EnemyController is dead.
    /// </summary>
    /// <returns>true if this EnemyController is no longer needed and
    /// should be removed by the ControllerController; otherwise,
    /// returns false.</returns>
    public virtual bool ShouldRemoveController()
    {
        if (!ValidEnemy()) return true;
        return false;
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

    /// <summary>
    /// Returns true if the Enemy controlled by this EnemyController is alive
    /// and active.
    /// </summary>
    /// <returns>true if the Enemy controlled by this EnemyController is alive
    /// and active; otherwise, false.</returns>
    public bool EnemyAlive()
    {
        if (!ValidEnemy()) return false;
        if (enemy.GetHealth() <= 0) return false;
        if (!enemy.isActiveAndEnabled) return false;

        return true;
    }

    /// <summary>
    /// Updates the Enemy controlled by this EnemyController's sorting order so that
    /// it is in line with its current tile Y position and does not display behind
    /// sprites that are higher up on the TileGrid.
    /// </summary>
    private void RectifyEnemySortingOrder()
    {
        Enemy e = GetEnemy();
        e.SetSortingOrder(10000 - (int)(GetEnemy().GetPosition().y * 100));
    }

    /// <summary>
    /// Updates the Enemy controlled by this EnemyController so that it stores
    /// the correct coordinates of the Tile that it is currently standing on.
    /// </summary>
    private void UpdateEnemyTilePosition()
    {
        Vector2 enemyWorldPos = GetEnemy().GetPosition();
        int enemyTileX = TileGrid.PositionToCoordinate(enemyWorldPos.x);
        int enemyTileY = TileGrid.PositionToCoordinate(enemyWorldPos.y);
        GetEnemy().UpdateTilePosition(enemyTileX, enemyTileY);
    }

    /// <summary>
    /// Updates the cooldowns managed by the Enemy controlled by this
    /// EnemyController.
    /// </summary>
    private void UpdateEnemyCooldowns()
    {
        GetEnemy().UpdateCooldowns();
    }

    /// <summary>
    /// Updates the Enemy managed by this EnemyController's Collider2D
    /// properties.
    /// </summary>
    private void UpdateEnemyCollider()
    {
        GetEnemy().SetColliderProperties();
    }
}
