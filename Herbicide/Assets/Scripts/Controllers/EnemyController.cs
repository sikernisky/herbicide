using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System.Linq;
using System;

/// <summary>
/// Controls an Enemy. <br></br>
/// 
/// The EnemyController is responsible for manipulating its Enemy and bringing
/// it to life. This includes moving it, choosing targets, playing animations,
/// and more.
/// </summary>
/// <typeparam name="T">The type of enum that represents distinct states or attributes
/// of the Enemy.</typeparam>
public abstract class EnemyController<T> : MobController<T> where T : Enum
{
    #region Fields

    /// <summary>
    /// true if the Enemy has popped out of a NexusHole and is fully spawned.
    /// </summary>
    private bool poppedOutOfHole;

    /// <summary>
    /// Enemies find targets.
    /// </summary>
    protected override bool FINDS_TARGETS => true;

    #endregion

    #region Methods

    /// <summary>
    /// Initializes this EnemyController with the Enemy it controls.
    /// </summary>
    /// <param name="enemy">the enemy this EnemyController controls.</param>
    public EnemyController(Enemy enemy) : base(enemy) { }

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
        GetEnemy().UpdateDamageOverTimeEffects();
    }

    /// <summary>
    /// Sorts the targets in ascending order of priority. The
    /// target at the 0th index is the highest priority target.
    /// </summary>
    /// <param name="targets">The list of targets to sort. </param>
    protected override void SortTargets(List<Model> targets)
    {
        targets.Sort((a, b) =>
        {
            float distA = Vector2.Distance(GetEnemy().GetPosition(), a.GetPosition());
            float distB = Vector2.Distance(GetEnemy().GetPosition(), b.GetPosition());
            return distA.CompareTo(distB);
        });
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
    private void UpdateEnemyHealthState() => GetEnemy().UpdateHealthState();

    /// <summary>
    /// Returns the Enemy controlled by this EnemyController.
    /// </summary>
    /// <returns>the Enemy controlled by this EnemyController.</returns>
    public Enemy GetEnemy() => GetMob() as Enemy;

    /// <summary>
    /// Updates the Enemy managed by this EnemyController's Collider2D
    /// properties.
    /// </summary>
    private void UpdateEnemyCollider() => GetEnemy().SetColliderProperties();

    /// <summary>
    /// Returns true if the Enemy's current state renders it immune
    /// to damage.
    /// </summary>
    /// <returns>true if the Enemy's current state renders it immune
    /// to damage; otherwise, false. </returns>
    protected abstract bool IsCurrentStateImmune();

    /// <summary>
    /// Returns true if this controller's Enemy should be destoyed and
    /// set to null.
    /// </summary>
    /// <returns>true if this controller's Enemy should be destoyed and
    /// set to null; otherwise, false.</returns>
    public override bool ValidModel()
    {
        if (!GetEnemy().Spawned()) return true;
        if (IsCurrentStateImmune()) return true;

        if (!TileGrid.OnWalkableTile(GetEnemy().GetPosition())) return false;
        else if (GetEnemy().Dead()) return false;
        else if (GetEnemy().Exited()) return false;

        return true;
    }

    /// <summary>
    /// Responds to a collision with a Projectile.
    /// </summary>
    /// <param name="projectile">the Projectile that hit this Enemy. </param>
    protected override void HandleProjectileCollision(Projectile projectile)
    {
        if(projectile == null) return;
        switch (projectile.TYPE) {
            case ModelType.QUILL:
                Quill quill = projectile as Quill;
                if(quill != null) GetEnemy().StickWithQuill(quill);
                TakeProjectileHit(projectile);
                break;
            case ModelType.ICE_CHUNK:
                GetEnemy().AddEffect(new IceChunkEffect());
                TakeProjectileHit(projectile);
                break;
            default:
                TakeProjectileHit(projectile);
                break;
        }
    }

    /// <summary>
    /// Helper method to handle a Projectile hit on this Enemy.
    /// Applies the damage from the Projectile to the Enemy
    /// and plays a sound effect.
    /// </summary>
    /// <param name="projectile">the Projectile that hit this
    /// Enemy.</param>
    private void TakeProjectileHit(Projectile projectile)
    {
        if (projectile == null) return;
        GetEnemy().AdjustHealth(-projectile.GetDamage());
        SoundController.PlaySoundEffect("kudzuHit");
    }

    /// <summary>
    /// Destroys the Enemy and removes it from the scene.
    /// </summary>
    protected override void DestroyAndRemoveModel()
    {
        if(GetEnemy() == null) return;
        if (GetEnemy().GetQuillsStuckInEnemy() != null)
        {
            List<Quill> quillsStuck = GetEnemy().GetQuillsStuckInEnemy();
            GetEnemy().RemoveQuills();
            int totalQuills = quillsStuck.Sum(quill => quill.IsDoubleQuill() ? 2 : 1);
            float angleStep = 360.0f / totalQuills;
            Vector3 enemyPosition = GetEnemy().GetPosition();
            int quillIndex = 0;

            foreach (Quill stuckQuill in quillsStuck)
            {
                int quillsToCreate = stuckQuill.IsDoubleQuill() ? 2 : 1;
                for (int j = 0; j < quillsToCreate; j++)
                {
                    float angle = quillIndex * angleStep;
                    Vector3 direction = Quaternion.Euler(0, 0, angle) * Vector3.right;
                    Vector3 targetPosition = enemyPosition + direction * 1000; // Arbitrary distance multiplier
                    GameObject quillPrefab = ProjectileFactory.GetProjectilePrefab(ModelType.QUILL);
                    Assert.IsNotNull(quillPrefab);
                    Quill quillComp = quillPrefab.GetComponent<Quill>();
                    Assert.IsNotNull(quillComp);
                    QuillController quillController = new QuillController(quillComp, enemyPosition, targetPosition, false);
                    ControllerController.AddModelController(quillController);
                    quillIndex++;
                }
            }
        }
        base.DestroyAndRemoveModel();
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
            Nexus nexusTarget = heldTarget as Nexus;
            if (nexusTarget != null)
            {
                nexusTarget.SetDropped();
                if (GetEnemy().Exited()) nexusTarget.CashIn();
                else TileGrid.PlaceOnTileUsingCoordinates(new Vector2Int(GetEnemy().GetX(), GetEnemy().GetY()), nexusTarget);
            }

        }
        base.OnDestroyModel();
    }

    /// <summary>
    /// Drops a resource at the Kudzu's resource drop rate.
    /// </summary>
    protected override void DropDeathLoot()
    {
        if (DroppedDeathLoot()) return;
        if (GetEnemy().Exited()) return;

/*        Vector3 lootPos = GetEnemy().Exited() ? GetEnemy().GetExitPos() : GetEnemy().GetPosition();
        int value = GetEnemy().CURRENCY_VALUE_ON_DEATH;*/

/*        Dew dew = CollectableFactory.GetCollectablePrefab(ModelType.DEW).GetComponent<Dew>();
        DewController dewController = new DewController(dew, lootPos, value);
        ControllerController.AddModelController(dewController);*/

/*        BasicTreeSeed basicTreeSeed = CollectableFactory.GetCollectablePrefab(ModelType.BASIC_TREE_SEED).GetComponent<BasicTreeSeed>();
        BasicTreeSeedController basicTreeSeedController = new BasicTreeSeedController(basicTreeSeed, lootPos);
        ControllerController.AddModelController(basicTreeSeedController);*/

/*        SpeedTreeSeed speedTreeSeed = CollectableFactory.GetCollectablePrefab(ModelType.SPEED_TREE_SEED).GetComponent<SpeedTreeSeed>();
        SpeedTreeSeedController speedTreeSeedController = new SpeedTreeSeedController(speedTreeSeed, lootPos);
        ControllerController.AddModelController(speedTreeSeedController);*/

        if(ControllerController.NumEnemiesRemainingInclusive() == 0)
        {
            LevelReward levelReward = CollectableFactory.GetCollectablePrefab(ModelType.LEVEL_REWARD).GetComponent<LevelReward>();
            LevelRewardController levelRewardController = new LevelRewardController(levelReward, GetEnemy().GetPosition());
            ControllerController.AddModelController(levelRewardController);
        }

        base.DropDeathLoot();
    }

    /// <summary>
    /// Returns the spawn position of the Enemy when in a NexusHole.
    /// </summary>
    /// <param name="originalSpawnPos">The position of the NexusHole it is
    /// spawning from.</param>
    /// <returns> the spawn position of the Enemy when in a NexusHole.</returns>
    protected virtual Vector3 NexusHoleSpawnPos(Vector3 originalSpawnPos) => originalSpawnPos;

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
    protected bool PoppedOutOfHole() => poppedOutOfHole;

    /// <summary>
    /// Returns the Enemy closest to the one controlled by this EnemyController
    /// that is carrying an object. Returns null if there are no carriers.
    /// </summary>
    /// <returns>the closest carrier.</returns>
    protected Enemy GetNearestCarrier()
    {
        Enemy currentEnemy = GetEnemy();
        Enemy nearestCarrier = null;
        float nearestCarrierDist = float.MaxValue;

        foreach (Model carrier in GetAllModels())
        {
            Enemy enemyCarrier = carrier as Enemy;
            if (enemyCarrier == null) continue;
            if (!enemyCarrier.IsHoldingNexus()) continue;
            if (enemyCarrier == currentEnemy) continue;
            if (enemyCarrier.Exited()) continue;
            if (enemyCarrier.IsExiting()) continue;

            float dist = Vector2.Distance(currentEnemy.GetPosition(), enemyCarrier.GetPosition());
            if (dist < nearestCarrierDist)
            {
                nearestCarrier = enemyCarrier;
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

    /// <summary>
    /// Returns the Enemy prefab to the EnemyFactory object pool.
    /// </summary>
    public override void ReturnModelToFactory() => EnemyFactory.ReturnEnemyPrefab(GetEnemy().gameObject);

    #endregion
}
