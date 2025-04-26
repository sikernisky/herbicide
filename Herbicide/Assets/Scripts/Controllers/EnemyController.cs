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
    /// Enemies find targets.
    /// </summary>
    protected override bool FINDS_TARGETS => true;

    /// <summary>
    /// The path the Enemy is following.
    /// </summary>
    private int pathId;

    /// <summary>
    /// The index of the current waypoint in the path.
    /// </summary>
    private int currentWaypointIndex;

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
        if (GetGameState() != GameState.ONGOING) return;
        base.UpdateMob();
        if (!GetEnemy().Spawned()) return;

        UpdateEnemyHealthState();
        UpdateEnemyCollider();
        GetEnemy().UpdateDamageOverTimeEffects();
        UpdateCurrentWaypoint();
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
            float distA = Vector2.Distance(GetEnemy().GetWorldPosition(), a.GetWorldPosition());
            float distB = Vector2.Distance(GetEnemy().GetWorldPosition(), b.GetWorldPosition());
            return distA.CompareTo(distB);
        });
    }

    /// <summary>
    /// Spawns this EnemyController's Enemy. Only does this if it
    /// is ready to spawn.
    /// </summary>
    protected override void SpawnMob()
    {
        base.SpawnMob();
        GetMob().SetWorldPosition(SpawnHoleSpawnPos(GetMob().GetSpawnWorldPosition()));
        pathId = UnityEngine.Random.Range(0, Waypoint.GetNumPaths());
        currentWaypointIndex = 0;
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
    /// Updates the current waypoint index by checking the Enemy's position
    /// against the waypoint it is currently moving towards. If the Enemy has
    /// reached the waypoint, the current waypoint index is incremented.
    /// </summary>
    private void UpdateCurrentWaypoint()
    {
        Vector3 worldPositionOfNextWaypoint = TileGrid.GetNextWaypointWorldPosition(GetPathId(), GetCurrentWaypointIndex());
        bool reachedWaypoint = Vector2.Distance(GetEnemy().GetWorldPosition(), worldPositionOfNextWaypoint) <= BoardConstants.TileSize / 2f;
        if(reachedWaypoint) currentWaypointIndex++;
    }

    /// <summary>
    /// Returns the ID of the path the Enemy is following.
    /// </summary>
    /// <returns>the ID of the path the Enemy is following.</returns>
    protected int GetPathId() => pathId;    

    /// <summary>
    /// Returns the index of the current waypoint in the path.
    /// </summary>
    /// <returns>the index of the current waypoint in the path.</returns>
    protected int GetCurrentWaypointIndex() => currentWaypointIndex;

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
        if (GetEnemy().IsEscaped()) return false;
        if (!GetEnemy().Spawned()) return true;
        if (IsCurrentStateImmune()) return true;
        if (GetEnemy().Dead()) return false;

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
        GetEnemy().AdjustHealth(-projectile.Damage);
        SoundController.PlaySoundEffect("kudzuHit");
    }

    /// <summary>
    /// Drops a resource at the Kudzu's resource drop rate.
    /// </summary>
    protected override void DropDeathLoot()
    {
        if (DroppedDeathLoot()) return;
        if (GetEnemy().IsEscaped()) return;

        if(ControllerManager.NumEnemiesRemainingInclusive() == 0)
        {
            LevelReward levelReward = CollectableFactory.GetCollectablePrefab(ModelType.LEVEL_REWARD).GetComponent<LevelReward>();
            LevelRewardController levelRewardController = new LevelRewardController(levelReward, GetEnemy().GetWorldPosition());
            ControllerManager.AddModelController(levelRewardController);
        }

        base.DropDeathLoot();
    }

    /// <summary>
    /// Applies the Enemy's death effects right before its reference is
    /// removed from the scene.
    /// </summary>
    protected override void OnDestroyModel()
    {
        base.OnDestroyModel();
        EjectQuills();
        RemoveLivesOnEscape();
        RewardCurrencyOnKill();
    }

    /// <summary>
    /// Fires out the Quills stuck in the Enemy.
    /// </summary>
    private void EjectQuills()
    {
        if (GetEnemy().GetQuillsStuckInEnemy() != null)
        {
            List<Quill> quillsStuck = GetEnemy().GetQuillsStuckInEnemy();
            GetEnemy().RemoveQuills();
            int totalQuills = quillsStuck.Sum(quill => quill.IsDoubleQuill() ? 2 : 1);
            float angleStep = 360.0f / totalQuills;
            Vector3 enemyPosition = GetEnemy().GetWorldPosition();
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
                    ControllerManager.AddModelController(quillController);
                    quillIndex++;
                }
            }
        }
    }

    /// <summary>
    /// Removes lives from the player when the Enemy escapes.
    /// </summary>
    private void RemoveLivesOnEscape()
    {
        if(!GetEnemy().IsEscaped()) return;
        for (int i = 0; i < GetEnemy().LIVES_LOST_ON_EXIT; i++) { HealthController.LoseLife(); }
    }

    /// <summary>
    /// Grants the player currency when the Enemy is killed.
    /// </summary>
    private void RewardCurrencyOnKill()
    {
        if (GetEnemy().IsEscaped()) return;
        EconomyController.CashIn(GetEnemy());
    }

    /// <summary>
    /// Returns the spawn position of the Enemy when in a SpawnHole.
    /// </summary>
    /// <param name="originalSpawnPos">The position of the SpawnHole it is
    /// spawning from.</param>
    /// <returns> the spawn position of the Enemy when in a SpawnHole.</returns>
    protected virtual Vector3 SpawnHoleSpawnPos(Vector3 originalSpawnPos) => originalSpawnPos;

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
            if (enemyCarrier == currentEnemy) continue;
            if (enemyCarrier.IsEscaped()) continue;
            if (enemyCarrier.IsExiting()) continue;

            float dist = Vector2.Distance(currentEnemy.GetWorldPosition(), enemyCarrier.GetWorldPosition());
            if (dist < nearestCarrierDist)
            {
                nearestCarrier = enemyCarrier;
                nearestCarrierDist = dist;
            }
        }

        return nearestCarrier;
    }

    /// <summary>
    /// Returns the Enemy prefab to the EnemyFactory object pool.
    /// </summary>
    public override void ReturnModelToFactory() => EnemyFactory.ReturnEnemyPrefab(GetEnemy().gameObject);

    #endregion
}
