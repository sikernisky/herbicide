using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System.Linq;
using System;

/// <summary>
/// Controls an Enemy.
/// </summary>
public abstract class EnemyController<T> : MobController<T> where T : Enum
{
    /// <summary>
    /// Total number of DefenderControllers created during this level so far.
    /// </summary>
    private static int NUM_ENEMIES;


    /// <summary>
    /// Initializes this EnemyController with the Enemy it controls.
    /// </summary>
    /// <param name="enemy">the enemy this EnemyController controls.</param>
    /// <param name="spawnTime">when the Enemy should spawn in the level. </param>
    /// <param name="spawnCoords">where this Enemy should spawn</param>
    public EnemyController(Enemy enemy, float spawnTime, Vector2 spawnCoords) : base(enemy)
    {
        //Safety checks
        Assert.IsNotNull(enemy);

        GameObject enemyOb = enemy.CloneEnemy();
        Enemy newEnemy = enemyOb.GetComponent<Enemy>();
        Assert.IsNotNull(newEnemy);
        SetModel(newEnemy);

        GetEnemy().SetSpawnTime(spawnTime);
        GetEnemy().SetSpawnWorldPosition(spawnCoords);
        GetEnemy().gameObject.SetActive(false);

        //SetState(Mob.MobState.INACTIVE);
        NUM_ENEMIES++;
    }

    /// <summary>
    /// Updates the Enemy controlled by this EnemyController.
    /// </summary>
    /// <param name="targets">A complete list of ITargetables in the scene.</param>
    protected override void UpdateMob(List<ITargetable> targets)
    {
        if (!ValidModel()) return;
        TryRemoveModel();
        if (GetGameState() != GameState.ONGOING) return;

        if (!GetEnemy().Spawned()) SpawnMob();
        if (!GetEnemy().Spawned()) return;

        ElectTarget(FilterTargets(targets));
        DetermineEnemyMovePos();
        UpdateEnemyTilePosition();
        RectifyEnemySortingOrder();
        UpdateEnemyHealthState();
        UpdateEnemyCollider();

        //STATE LOGIC
        UpdateStateFSM();
        ExecuteIdleState();
    }

    /// <summary>
    /// Spawns this EnemyController's Enemy. Only does this if it
    /// is ready to spawn.
    /// </summary>
    protected override void SpawnMob()
    {
        if (!GetEnemy().ReadyToSpawn()) return;
        GetEnemy().gameObject.SetActive(true);
        GetEnemy().SubscribeToCollision(HandleCollision);
        GetEnemy().SetWorldPosition(GetEnemy().GetSpawnWorldPosition());
        base.SpawnMob();
    }

    /// <summary>
    /// Updates the HealthState of the Enemy controlled by this EnemyController.
    /// </summary>
    private void UpdateEnemyHealthState() { GetEnemy().UpdateHealthState(); }

    /// <summary>
    /// Returns true if the Enemy controlled by this EnemyController
    /// is not null.
    /// </summary>
    /// <returns>true if the Enemy controlled by this EnemyController
    /// is not null; otherwise, returns false.</returns>
    public override bool ValidModel() { return GetEnemy() != null; }

    /// <summary>
    /// Returns the Enemy controlled by this EnemyController.
    /// </summary>
    /// <returns>the Enemy controlled by this EnemyController.</returns>
    public Enemy GetEnemy() { return GetMob() as Enemy; }

    /// <summary>
    /// Updates the Enemy managed by this EnemyController's Collider2D
    /// properties.
    /// </summary>
    private void UpdateEnemyCollider() { GetEnemy().SetColliderProperties(); }

    /// <summary>
    /// Updates the Enemy controlled by this EnemyController's sorting order so that
    /// it is in line with its current tile Y position and does not display behind
    /// sprites that are higher up on the TileGrid.
    /// </summary>
    private void RectifyEnemySortingOrder()
    {
        GetEnemy().SetSortingOrder(10000 - (int)(GetEnemy().GetPosition().y * 100));
    }

    /// <summary>
    /// Destroys this EnemyController's Enemy model, removing it
    /// from the scene and triggering its death logic.
    /// </summary>
    protected void DestroyEnemy()
    {
        if (!ValidModel()) return;
        GetEnemy().OnDie();
        GameObject.Destroy(GetEnemy().gameObject);
        GameObject.Destroy(GetEnemy());
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
        GetEnemy().SetTileCoordinates(enemyTileX, enemyTileY);
    }

    /// <summary>
    /// Sets the position that represents where this EnemyController's
    /// Enemy should move towards. If it cannot move, sets the position
    /// to itself.
    /// </summary>
    protected virtual void DetermineEnemyMovePos()
    {
        if (!ValidModel()) GetEnemy().SetNextMovePos(GetEnemy().GetPosition());
        GetEnemy().SetNextMovePos(
            TileGrid.NextTilePosTowardsGoal(
                GetEnemy().GetPosition(), GetTarget().GetPosition()));
    }

    /// <summary>
    /// Checks if this EnemyController's model should be removed from
    /// the scene. If so, removes it.
    /// </summary>
    protected override void TryRemoveModel()
    {
        if (!GetEnemy().Spawned()) return;
        if (!TileGrid.OnWalkableTile(GetEnemy().GetPosition(), GetEnemy())) DestroyEnemy();
        if (GetEnemy().GetHealth() <= 0) DestroyEnemy();
    }

    /// <summary>
    /// Sets this EnemyController's target from a filtered list of ITargetables.
    /// </summary>
    /// <param name="filteredTargetables">a list of ITargables that this EnemyController
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
    /// only contains ITargetables that this EnemyController's Enemy is allowed
    /// to target. <br></br><br></br>
    /// 
    /// The Enemy is allowed to target Defenders and player-sided PlaceableObjects.
    /// </summary>
    /// <param name="targetables">the list of all ITargetables in the scene</param>
    /// <returns>a list containing Enemy ITargetables that this EnemyController's Enemy can
    /// reach./// </returns>
    protected override List<ITargetable> FilterTargets(List<ITargetable> targetables)
    {
        Assert.IsNotNull(targetables, "List of targets is null.");
        List<ITargetable> filteredTargets = new List<ITargetable>();
        filteredTargets.AddRange(targetables.Where(tar => tar as Tree != null &&
            TileGrid.CanReach(GetEnemy().GetPosition(), tar.GetPosition())));
        filteredTargets.AddRange(targetables.Where(tar => tar as Defender != null &&
            TileGrid.CanReach(GetEnemy().GetPosition(), tar.GetPosition())));
        return filteredTargets;
    }
}
