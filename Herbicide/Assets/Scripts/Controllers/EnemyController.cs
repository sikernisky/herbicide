using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Controls an Enemy. <br></br>
/// 
/// The EnemyController is responsible for manipulating its Enemy and bringing
/// it to life. This includes moving it, choosing targets, playing animations,
/// and more.
/// </summary>
/// <typeparam name="T">The type of enum that represents distinct states or attributes
/// of the Enemy.</typeparam>
public abstract class EnemyController : MobController<EnemyController.EnemyState>
{
    #region Fields

    /// <summary>
    /// All possible states of an Enemy. In the future,
    /// enemies will define their own states just like
    /// Defenders do.
    /// </summary>
    public enum EnemyState
    {
        INACTIVE, // Not spawned yet.
        ENTERING, // Entering map.
        IDLE, // Static, nothing to do.
        CHASE, // Pursuing target.
        ATTACK, // Attacking target.
        ESCAPE, // Running back to start.
        PROTECT, // Protecting other escaping enemies.
        EXITING, // Leaving map.
        DEAD, // Dead.
        INVALID // Something went wrong.
    }

    /// <summary>
    /// true if the Enemy has popped out of a NexusHole and is fully spawned.
    /// </summary>
    private bool poppedOutOfHole;

    /// <summary>
    /// Counts the number of seconds in the spawn animation; resets
    /// on step.
    /// </summary>
    protected float spawnAnimationCounter;

    /// <summary>
    /// Counts the number of seconds in the idle animation; resets
    /// on step.
    /// </summary>
    protected float idleAnimationCounter;

    /// <summary>
    /// Counts the number of seconds in the chase animation; resets
    /// on step.
    /// </summary>
    protected float chaseAnimationCounter;

    /// <summary>
    /// Counts the number of seconds in the attack animation; resets
    /// on step.
    /// </summary>
    protected float attackAnimationCounter;

    /// <summary>
    /// Counts the number of seconds in the protect animation; resets
    /// on step.
    /// </summary>
    protected float protectAnimationCounter;

    /// <summary>
    /// Counts the number of seconds in the escape animation; resets
    /// on step.
    /// </summary>
    protected float escapeAnimationCounter;

    /// <summary>
    /// Counts the number of seconds in the exiting animation; resets
    /// on step.
    /// </summary>
    protected float exitingAnimationCounter;

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
        GetEnemy().ToggleHealthBar(SettingsController.SHOW_HEALTH_BARS);

        ExecuteInactiveState();
        ExecuteEnteringState();
        ExecuteIdleState();
        ExecuteChaseState();
        ExecuteAttackState();
        ExecuteProtectState();
        ExecuteEscapeState();
        ExecuteExitState();
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
    }

    /// <summary>
    /// Responds to a collision with a Projectile.
    /// </summary>
    /// <param name="projectile">the Projectile that hit this Enemy. </param>
    protected override void HandleProjectileCollision(Projectile projectile)
    {
        base.HandleProjectileCollision(projectile);
        if(projectile == null) return;
        switch (projectile.TYPE) {

            case ModelType.ACORN:
                Acorn acorn = projectile as Acorn;
                if(acorn == null) return;
                GetEnemy().AdjustHealth(-acorn.GetDamage());
                SoundController.PlaySoundEffect("kudzuHit");
                break;
            case ModelType.QUILL:
                Quill quill = projectile as Quill;
                if(quill == null) return;
                GetEnemy().AdjustHealth(-quill.GetDamage());
                SoundController.PlaySoundEffect("kudzuHit");
                break; 
            default:
                break;
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
            Nexus nexusTarget = heldTarget as Nexus;
            if (nexusTarget != null)
            {
                nexusTarget.SetDropped();
                if (GetEnemy().Exited()) nexusTarget.CashIn();
                else TileGrid.PlaceOnTile(new Vector2Int(GetEnemy().GetX(), GetEnemy().GetY()), nexusTarget);
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

        Dew dew = DewFactory.GetDewPrefab().GetComponent<Dew>();
        Vector3 lootPos = GetEnemy().Exited() ? GetEnemy().GetExitPos() : GetEnemy().GetPosition();
        int value = GetEnemy().CURRENCY_VALUE_ON_DEATH;
        DewController dewController = new DewController(dew, lootPos, value);
        ControllerController.AddModelController(dewController);
        base.DropDeathLoot();
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
    /// Returns the Knotwood prefab to the KnotwoodFactory singleton.
    /// </summary>
    public override void DestroyModel()
    {
        EnemyFactory.ReturnEnemyPrefab(GetEnemy().gameObject);
    }

    #endregion

    #region State Logic

    /// <summary>
    /// Returns true if two EnemyStates are equal.
    /// </summary>
    /// <param name="stateA">The first EnemyState</param>
    /// <param name="stateB">The second EnemyState</param>
    /// <returns>true if two EnemyStates are equal; otherwise, false. </returns>
    public override bool StateEquals(EnemyState stateA, EnemyState stateB)
    {
        return stateA == stateB;
    }

    /// <summary>
    /// Executes the logic for the INACTIVE state.
    /// </summary>
    protected abstract void ExecuteInactiveState();

    /// <summary>
    /// Executes the logic for the ENTERING state.
    /// </summary> 
    protected abstract void ExecuteEnteringState();

    /// <summary>
    /// Executes the logic for the IDLE state.
    /// </summary> 
    protected abstract void ExecuteIdleState();

    /// <summary>
    /// Executes the logic for the CHASE state.
    /// </summary> 
    protected abstract void ExecuteChaseState();

    /// <summary>
    /// Executes the logic for the ATTACK state.
    /// </summary> 
    protected abstract void ExecuteAttackState();

    /// <summary>
    /// Executes the logic for the PROTECT state.
    /// </summary> 
    protected abstract void ExecuteProtectState();

    /// <summary>
    /// Executes the logic for the ESCAPE state.
    /// </summary> 
    protected abstract void ExecuteEscapeState();

    /// <summary>
    /// Executes the logic for the EXIT state.
    /// </summary> 
    protected abstract void ExecuteExitState();

    #endregion

    #region Animation Logic

    /// <summary>
    /// Adds one chunk of Time.deltaTime to the animation
    /// counter that tracks the current state.
    /// </summary>
    public override void AgeAnimationCounter()
    {
        EnemyState state = GetState();
        if (state == EnemyState.ENTERING) spawnAnimationCounter += Time.deltaTime;
        else if (state == EnemyState.IDLE) idleAnimationCounter += Time.deltaTime;
        else if (state == EnemyState.CHASE) chaseAnimationCounter += Time.deltaTime;
        else if (state == EnemyState.ATTACK) attackAnimationCounter += Time.deltaTime;
        else if (state == EnemyState.PROTECT) protectAnimationCounter += Time.deltaTime;
        else if (state == EnemyState.ESCAPE) escapeAnimationCounter += Time.deltaTime;
        else if (state == EnemyState.EXITING) exitingAnimationCounter += Time.deltaTime;
    }

    /// <summary>
    /// Returns the animation counter for the current state.
    /// </summary>
    /// <returns>the animation counter for the current state.</returns>
    public override float GetAnimationCounter()
    {
        EnemyState state = GetState();
        if (state == EnemyState.ENTERING) return spawnAnimationCounter;
        else if (state == EnemyState.IDLE) return idleAnimationCounter;
        else if (state == EnemyState.CHASE) return chaseAnimationCounter;
        else if (state == EnemyState.ATTACK) return attackAnimationCounter;
        else if (state == EnemyState.PROTECT) return protectAnimationCounter;
        else if (state == EnemyState.ESCAPE) return escapeAnimationCounter;
        else if (state == EnemyState.EXITING) return exitingAnimationCounter;
        else return 0;
    }

    /// <summary>
    /// Sets the animation counter for the current state to 0.
    /// </summary>
    public override void ResetAnimationCounter()
    {
        EnemyState state = GetState();
        if (state == EnemyState.ENTERING) spawnAnimationCounter = 0;
        else if (state == EnemyState.IDLE) idleAnimationCounter = 0;
        else if (state == EnemyState.CHASE) chaseAnimationCounter = 0;
        else if (state == EnemyState.ATTACK) attackAnimationCounter = 0;
        else if (state == EnemyState.PROTECT) protectAnimationCounter = 0;
        else if (state == EnemyState.ESCAPE) escapeAnimationCounter = 0;
        else if (state == EnemyState.EXITING) exitingAnimationCounter = 0;
    }

    #endregion
}
