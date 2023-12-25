using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System.Linq;
using System;

/// <summary>
/// Abstract class to represent controllers of Enemies.
/// 
/// The EnemyController is responsible for manipulating its Enemy and bringing
/// it to life. This includes moving it, choosing targets, playing animations,
/// and more.
/// </summary>
/// <typeparam name="T">Enum to represent state of the Enemy.</typeparam>
public abstract class EnemyController<T> : MobController<T> where T : Enum
{
    /// <summary>
    /// Total number of DefenderControllers created during this level so far.
    /// </summary>
    private static int NUM_ENEMIES;



    /// <summary>
    /// true if this Enemy has picked up as many targets as it is allowed to;
    /// otherwise, false. /// 
    /// </summary>
    private bool handsFull;





    /// <summary>
    /// Initializes this EnemyController with the Enemy it controls.
    /// </summary>
    /// <param name="enemy">the enemy this EnemyController controls.</param>
    public EnemyController(Enemy enemy) : base(enemy)
    {
        //Safety checks
        Assert.IsNotNull(enemy);

        //SetState(Mob.MobState.INACTIVE);
        NUM_ENEMIES++;
    }

    /// <summary>
    /// Updates the Enemy controlled by this EnemyController.
    /// </summary>
    protected override void UpdateMob()
    {
        if (!ValidModel()) return;
        base.UpdateMob();
        if (GetGameState() != GameState.ONGOING) return;

        if (!GetEnemy().Spawned()) SpawnMob();
        if (!GetEnemy().Spawned()) return;

        RectifyEnemySortingOrder();
        UpdateEnemyHealthState();
        UpdateEnemyCollider();
        // Debug.Log("\n");
        // GetTargets().ForEach(target => Debug.Log("target: " + target + ", id: " + target.GetInstanceID()));
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
    /// Returns true if this controller's Enemy should be destoyed and
    /// set to null.
    /// </summary>
    /// <returns>true if this controller's Enemy should be destoyed and
    /// set to null; otherwise, false.</returns>
    protected override bool ShouldRemoveModel()
    {
        if (!GetEnemy().Spawned()) return false;

        if (!TileGrid.OnWalkableTile(GetEnemy().GetPosition())) return true;
        else if (GetEnemy().Dead()) return true;
        else if (GetEnemy().Escaped()) return true;

        return false;
    }

    /// <summary>
    /// Returns true if the Enemy can target the PlaceableObject passed
    /// into this method.
    /// </summary>
    /// <param name="target">The Placeable object to check for targetability.</param>
    /// <returns></returns>
    protected override bool CanTarget(PlaceableObject target)
    {
        Nexus nexusTarget = target as Nexus;
        NexusHole nexusHoleTarget = target as NexusHole;

        if (target == null) return false;
        if (nexusTarget == null && nexusHoleTarget == null) return false;
        if (!target.Targetable()) return false;

        // Nexus logic.
        if (nexusTarget != null && nexusTarget.PickedUp()) return false;
        if (nexusTarget != null && nexusTarget.CashedIn()) return false;
        if (nexusTarget != null && NumTargetsHolding() == HOLDING_LIMIT) return false;

        // NexusHole logic.
        if (nexusHoleTarget != null && nexusHoleTarget.Filled()) return false;

        return true;
    }

    /// <summary>
    /// Handles all collisions between this controller's Enemy
    /// model and some other collider.
    /// </summary>
    /// <param name="other">the other collider.</param>
    protected override void HandleCollision(Collider2D other)
    {
        if (other == null) return;

        Acorn acorn = other.gameObject.GetComponent<Acorn>();
        BombSplat bombSplat = other.gameObject.GetComponent<BombSplat>();

        if (acorn != null)
        {
            SoundController.PlaySoundEffect("kudzuHit");
            GetEnemy().AdjustHealth(-acorn.GetDamage());
            acorn.SetCollided(GetEnemy());
        }
    }

}
