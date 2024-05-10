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
    /// Initializes this EnemyController with the Enemy it controls.
    /// </summary>
    /// <param name="enemy">the enemy this EnemyController controls.</param>
    public EnemyController(Enemy enemy) : base(enemy) { NUM_ENEMIES++; }

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

        UpdateEnemyHealthState();
        UpdateEnemyCollider();
    }

    /// <summary>
    /// Spawns this EnemyController's Enemy. Only does this if it
    /// is ready to spawn.
    /// </summary>
    protected override void SpawnMob()
    {
        if (!GetEnemy().ReadyToSpawn()) return;
        SetPathfindingCache();
        GetMob().SetWorldPosition(NexusHoleSpawnPos(GetMob().GetSpawnPos()));
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
    /// Returns true if this controller's Enemy should be destoyed and
    /// set to null.
    /// </summary>
    /// <returns>true if this controller's Enemy should be destoyed and
    /// set to null; otherwise, false.</returns>
    public override bool ValidModel()
    {
        if (!GetEnemy().Spawned()) return true;

        if (!TileGrid.OnWalkableTile(GetEnemy().GetPosition())) return false;
        else if (GetEnemy().Dead()) return false;
        else if (GetEnemy().Exited()) return false;

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

        if (acorn != null && acorn.GetVictim() == null)
        {
            SoundController.PlaySoundEffect("kudzuHit");
            GetEnemy().AdjustHealth(-acorn.GetDamage());
            acorn.SetCollided(GetEnemy());
        }
    }

    /// <summary>
    /// Performs logic right before the Enemy is destroyed.
    /// </summary>
    protected override void OnDestroyModel()
    {
        // Drop held targets.
        foreach (PlaceableObject heldTarget in GetHeldTargets())
        {
            if (heldTarget == null) continue;
            heldTarget.Drop();
            Nexus nexusTarget = heldTarget as Nexus;
            if (nexusTarget != null)
            {
                if (GetEnemy().Exited()) nexusTarget.CashIn();
                else TileGrid.PlaceOnTile(new Vector2Int(GetEnemy().GetX(), GetEnemy().GetY()), nexusTarget, true);
            }

        }

        base.OnDestroyModel();
    }

    /// <summary>
    /// Returns the spawn position of the Enemy when in a NexusHole.
    /// </summary>
    /// <param name="originalSpawnPos">The position of the NexusHole it is
    /// spawning from.</param>
    /// <returns> the spawn position of the Enemy when in a NexusHole.</returns>
    protected virtual Vector3 NexusHoleSpawnPos(Vector3 originalSpawnPos)
    {
        return originalSpawnPos;
    }
}
