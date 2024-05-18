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
    /// Enemies who are holding an object and escaping.
    /// </summary>
    private static List<Enemy> activeCarriers;

    /// <summary>
    /// true if the Enemy has popped out of a NexusHole and is fully spawned.
    /// </summary>
    private bool poppedOutOfHole;


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
            if(heldTarget as Nexus != null) RemoveAsCarrier();
            Nexus nexusTarget = heldTarget as Nexus;
            if (nexusTarget != null)
            {
                if (GetEnemy().Exited()) nexusTarget.CashIn();
                else TileGrid.PlaceOnTile(new Vector2Int(GetEnemy().GetX(), GetEnemy().GetY()), nexusTarget);
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

    /// <summary>
    /// Sets the EnemyController as popped out of a NexusHole.
    /// </summary>
    protected void SetPoppedOutOfHole() 
    { 
        poppedOutOfHole = true;
        GetEnemy().SetMaskInteraction(SpriteMaskInteraction.None);
    }

    /// <summary>
    /// Returns true if the Enemy has popped out of a NexusHole.
    /// </summary>
    /// <returns> true if the Enemy has popped out of a NexusHole; otherwise,
    /// false. </returns>
    protected bool PoppedOutOfHole() { return poppedOutOfHole; }

    /// <summary>
    /// Adds the Enemy as a carrier.
    /// </summary>
    protected void AddAsCarrier()
    {
        if(activeCarriers == null) activeCarriers = new List<Enemy>();
        activeCarriers.Add(GetEnemy());
    }

    /// <summary>
    /// Removes the Enemy as a carrier.
    /// </summary>
    protected void RemoveAsCarrier()
    {
        if(activeCarriers == null) return;
        activeCarriers.Remove(GetEnemy());
    }

    /// <summary>
    /// Holds a PlaceableObject target.
    /// </summary>
    /// <param name="target">the target to hold. </param>
    protected override void HoldTarget(PlaceableObject target)
    {
        base.HoldTarget(target);
        if(target as Nexus != null) AddAsCarrier();
    }

    /// <summary>
    /// Returns the Enemy closest to the one controlled by this EnemyController
    /// that is carrying an object. Returns null if there are no carriers.
    /// </summary>
    /// <returns>the closest carrier.</returns>
    protected Enemy GetNearestCarrier()
    {
        if (activeCarriers == null || activeCarriers.Count == 0) return null;

        Enemy currentEnemy = GetEnemy();
        Enemy nearestCarrier = null;
        float nearestCarrierDist = float.MaxValue;

        foreach (Enemy carrier in activeCarriers)
        {
            if (carrier == currentEnemy) continue;
            if (carrier.Exited()) continue;
            if(carrier.IsExiting()) continue;
                 
            float dist = Vector2.Distance(currentEnemy.GetPosition(), carrier.GetPosition());
            if (dist < nearestCarrierDist)
            {
                nearestCarrier = carrier;
                nearestCarrierDist = dist;
            }
        }

        return nearestCarrier;
    }

    /// <summary>
    /// Returns true if a given Nexus is the closest Nexus to the
    /// Enemy that it can target.
    /// </summary>
    /// <param name="nexus">The Nexus to check.</param>
    /// <returns>true if a given Nexus is the closest Nexus to the
    /// Enemy that it can target; otherwise, false. </returns>
    protected bool IsClosestDroppedNexus(Nexus nexus)
    {
        Assert.IsNotNull(nexus);
        if (nexus.CashedIn() || nexus.PickedUp()) return false;

        Nexus closestNexus = null;
        double minDistance = double.MaxValue;

        foreach (Model model in GetAllModels())
        {
            Nexus nexusObject = model as Nexus;
            if (nexusObject != null && !nexusObject.CashedIn() && !nexusObject.PickedUp())
            {
                double distance = nexusObject.GetDistanceTo(GetEnemy());
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestNexus = nexusObject;
                }
            }
        }

        return closestNexus == nexus;
    }

    /// <summary>
    /// Returns true if a given NexusHole is the closest NexusHole to the
    /// Enemy that it can target.
    /// </summary>
    /// <param name="nexusHole">The NexusHole to check.</param>
    /// <returns>true if a given NexusHole is the closest NexusHole to the
    /// Enemy that it can target; otherwise, false. </returns>
    protected bool IsClosestNexusHole(NexusHole nexusHole)
    {
        Assert.IsNotNull(nexusHole);

        NexusHole closestHole = null;
        double minDistance = double.MaxValue;

        foreach (Model model in GetAllModels())
        {
            NexusHole nexusHoleObject = model as NexusHole;
            if (nexusHoleObject != null)
            {
                double distance = nexusHoleObject.GetDistanceTo(GetEnemy());
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestHole = nexusHoleObject;
                }
            }
        }

        return closestHole == nexusHole;
    }
}
