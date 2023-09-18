using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.Linq;

/// <summary>
/// Represents some entity whose goal is to make the player
/// lose the current level.
/// </summary>
public abstract class Enemy : Mob, IAttackable
{
    /// <summary>
    /// Type of this Enemy.
    /// </summary>
    public abstract EnemyType TYPE { get; }

    /// <summary>
    /// Starting health of an Enemy. 
    /// </summary>
    public override int BASE_HEALTH => 100;

    /// <summary>
    /// This Enemy's largest possible health value.
    /// </summary>
    public override int MAX_HEALTH => 100;

    /// <summary>
    /// This Enemy's smallest possible health value.
    /// </summary>
    public override int MIN_HEALTH => 0;

    /// <summary>
    /// Starting attack range of an Enemy.
    /// </summary>
    public override float BASE_ATTACK_RANGE => 1f;

    /// <summary>
    /// Upper bound of an Enemy's attack range.
    /// </summary>
    public override float MAX_ATTACK_RANGE => float.MaxValue;

    /// <summary>
    /// Lower bound of an Enemy's attack range.
    /// </summary>
    public override float MIN_ATTACK_RANGE => 0f;

    /// <summary>
    /// Starting attack speed of an Enemy (number of attacks / second).
    /// </summary>
    public override float BASE_ATTACK_SPEED => 1f;

    /// <summary>
    /// Upper bound of an Enemy's attack speed (number of attacks / second).
    /// </summary>
    public override float MAX_ATTACK_SPEED => 10f;

    /// <summary>
    /// Lower bound of an Enemy's attack speed (number of attacks / second).
    /// </summary>
    public override float MIN_ATTACK_SPEED => 0f;

    /// <summary>
    /// Damage inflicted on a target(s) every time this Enemy attacks.
    /// </summary>
    public virtual int DAMAGE_PER_ATTACK => 10;

    /// <summary>
    /// How long to flash when this Tree takes damage
    /// </summary>
    public virtual float DAMAGE_FLASH_TIME => .1f;

    /// <summary>
    /// Time of an attack animation. Depends on attack speed.
    /// </summary>
    public virtual float ATTACK_ANIMATION_TIME => 1f / GetAttackSpeed();

    /// <summary>
    ///  Delay, in seconds, in-between movement animation frames.
    /// </summary>
    public virtual float MOVE_ANIMATION_TIME => .25f;


    /// <summary>
    /// How much currency this Enemy drops.
    /// </summary>
    public virtual int MONEY_ON_DROP => 1;

    /// <summary>
    /// Percent chance this Enemy drops currency on death. 
    /// </summary>
    public virtual float MONEY_DROP_CHANCE => .4f;

    /// <summary>
    /// Health of this Enemy. 
    /// </summary>
    private int health;

    /// <summary>
    /// Attack speed of this Enemy. 
    /// </summary>
    private float attackSpeed;

    /// <summary>
    /// Attack range of this Enemy. 
    /// </summary>
    private float attackRange;

    /// <summary>
    /// How many seconds remain before this Enemy can attack again.
    /// </summary>
    private float attackCooldownTimer;

    /// <summary>
    /// Reference to the Prefab for this Enemy.
    /// </summary>
    [SerializeField]
    private GameObject prefab;

    /// <summary>
    /// Reference to this Enemy's Animator component.
    /// </summary>
    [SerializeField]
    private Animator enemyAnimator;

    /// <summary>
    /// Current Direction of this Enemy.
    /// </summary>
    private Direction direction;

    /// <summary>
    /// The X-Coordinate of the Tile this Enemy is on.
    /// </summary>
    private int tileX;

    /// <summary>
    /// The Y-Coordinate of the Tile this Enemy is on.
    /// </summary>
    private int tileY;

    /// <summary>
    /// The X world position where this Enemy spawns.
    /// </summary>
    private float spawnX;

    /// <summary>
    /// The Y world position where this Enemy spawns.
    /// </summary>
    private float spawnY;

    /// <summary>
    /// Current health state of this Enemy.
    /// </summary>
    private EnemyHealthState healthState;

    /// <summary>
    /// The current animation track for this Enemy when it is healthy.
    /// </summary>
    private Sprite[] healthyTrack;

    /// <summary>
    /// The current animation track for this Enemy when it is damaged.
    /// </summary>
    private Sprite[] damagedTrack;

    /// <summary>
    /// The current animation track for this Enemy when it is 
    /// critically damaged.
    /// </summary>
    private Sprite[] criticalTrack;

    /// <summary>
    /// Enemy's SpriteRenderer component. 
    /// </summary>
    [SerializeField]
    private SpriteRenderer enemyRenderer;

    /// <summary>
    /// Enemy's Collider2D component. 
    /// </summary>
    [SerializeField]
    private Collider2D enemyCollider;

    /// <summary>
    /// The time at which this Enemy spawns.
    /// </summary>
    private float spawnTime;

    /// <summary>
    /// Type of this Enemy.
    /// </summary>
    public enum EnemyType
    {
        KUDZU
    }

    /// <summary>
    /// State of an Enemy based on its health.
    /// </summary>
    public enum EnemyHealthState
    {
        HEALTHY,
        DAMAGED,
        CRITICAL
    }

    /// <summary>
    /// Resets this Enemy's stats to its starting/base values.
    /// </summary>
    public virtual void ResetStats()
    {
        ResetAttackRange();
        ResetHealth();
        ResetAttackSpeed();
        ResetPosition();
    }

    /// <summary>
    /// Resets this Enemy's position to be its spawn position.
    /// </summary>
    private void ResetPosition()
    {
        transform.position = new Vector3(spawnX, spawnY, 1);
    }

    /// <summary>
    /// Called when this Enemy appears on the TileGrid and is assigned
    /// an EnemyController (or a subclass of EnemyController).
    /// </summary>
    public override void OnSpawn()
    {
        base.OnSpawn();
        ResetStats();
    }


    /// <summary>
    /// Called when this Enemy dies.
    /// </summary>
    public virtual void Die()
    {
        DropDeathLoot();
    }

    /// <summary>
    /// Spawns the loot dropped by this Enemy when it dies. 
    /// </summary>
    public virtual void DropDeathLoot()
    {
        Assert.IsTrue(MONEY_DROP_CHANCE >= 0f && MONEY_DROP_CHANCE <= 1f);
        int multipliedChance = (int)(MONEY_DROP_CHANCE * 100);
        int randomNumber = UnityEngine.Random.Range(1, 101);
        if (multipliedChance <= randomNumber) return; //Loot drop "missed".

        EconomyController.SpawnSeedToken(GetAttackPosition());
    }

    /// <summary>
    /// Attacks an ITargetable. This base method just sets the enemy's
    /// direction.
    /// </summary>
    /// <param name="target">The ITargetable to attack.</param>
    public virtual void Attack(ITargetable target)
    {
        Vector3 targetPos = target.GetPosition();
        Vector3 enemyPos = GetPosition();

        float diffX = targetPos.x - enemyPos.x;
        float diffY = targetPos.y - enemyPos.y;

        Direction direction = Math.Abs(diffX) > Math.Abs(diffY) ?
                              (diffX > 0 ? Direction.EAST : Direction.WEST) :
                              (diffY > 0 ? Direction.NORTH : Direction.SOUTH);

        SetDirection(direction);
    }

    /// <summary>
    /// Resets the cooldown timer associated with this Enemy's attack.
    /// </summary>
    protected void ResetAttackTimer()
    {
        attackCooldownTimer = 1f / GetAttackSpeed();
    }

    /// <summary>
    /// Updates this Enemy's HealthState.
    /// </summary>
    public void UpdateHealthState()
    {
        float currentPercent = (float)GetHealth() / (float)MAX_HEALTH;
        if (currentPercent >= .66f) healthState = EnemyHealthState.HEALTHY;
        else if (currentPercent >= .33f) healthState = EnemyHealthState.DAMAGED;
        else healthState = EnemyHealthState.CRITICAL;
    }

    /// <summary>
    /// Returns the current HealthState of this Enemy.
    /// </summary>
    /// <returns>the current HealthState of this Enemy.</returns>
    public EnemyHealthState GetHealthState()
    {
        return healthState;
    }

    /// <summary>
    /// Returns true if this Enemy can ever attack another IAttackable.
    /// </summary>
    /// <param name="target">the target to try to attack.</param>
    /// <returns>true if this Enemy can ever attack another IAttackable.
    public virtual bool CanAttackEver(ITargetable target)
    {
        if (target == null || target.Dead()) return false;
        return true;
    }

    /// <summary>
    /// Returns true if this Enemy can target some ITargetable. Overidden
    /// by sub-classes who implement specific targeting rules.
    /// </summary>
    /// <param name="target">The target to check.</param>
    /// <returns>true if this Enemy can target the ITargetable; otherwise,
    /// false.</returns>
    public abstract bool CanTarget(ITargetable target);

    /// <summary>
    /// Returns true if this Enemy can attack another IAttackable at
    /// the current frame.
    /// </summary>
    /// <param name="target">the target to try to attack.</param>
    /// <returns>true if this Enemy can attack another IAttackable at
    /// the current frame.</returns>
    public virtual bool CanAttackNow(ITargetable target)
    {
        if (target == null || target.Dead()) return false;
        if (DistanceToTarget(target) > GetAttackRange()) return false;
        if (GetAttackSpeed() <= 0) return false;
        if (attackCooldownTimer > 0) return false;

        return true;
    }

    /// <summary>
    /// Returns true if this Enemy is within its attack range of
    /// some ITargetable.
    /// </summary>
    /// <param name="target">the ITargetable to check.</param>
    /// <returns>true if this Enemy is within the range of the ITargetable;
    /// otherwise, false. </returns>
    public bool InAttackRange(ITargetable target)
    {
        if (target == null) return false;
        if (DistanceToTarget(target) > GetAttackRange()) return false;
        if (GetAttackSpeed() <= 0) return false;

        return true;
    }

    /// <summary>
    /// Returns the Euclidian distance between this Enemy and some
    /// IAttackable.
    /// </summary>
    /// <param name="target">The IAttackable to measure the distance from this Enemy.</param>
    public float DistanceToTarget(ITargetable target)
    {
        if (target == null) return float.MaxValue;

        Vector2 enemyPosition = GetPosition();
        Vector2 targetPosition = target.GetPosition();
        return Vector2.Distance(enemyPosition, targetPosition);
    }

    /// <summary>
    /// Returns this Enemy's current attack range.
    /// </summary>
    /// <returns>this Enemy's current attack range.</returns>
    public float GetAttackRange()
    {
        return attackRange;
    }

    /// <summary>
    /// Returns this Enemy's current health.
    /// </summary>
    /// <returns>this Enemy's current health.</returns>
    public int GetHealth()
    {
        return health;
    }

    /// <summary>
    /// Returns this Enemy's current position.
    /// </summary>
    /// <returns>this Enemy's current position.</returns>
    public Vector3 GetPosition()
    {
        return transform.position;
    }

    /// <summary>
    /// Adds to this Enemy's health.
    /// </summary>
    /// <param name="amount">Amount to heal.</param>
    public void Heal(int amount)
    {
        if (amount >= 0)
            health = Mathf.Clamp(health + amount, MIN_HEALTH, MAX_HEALTH);
    }

    /// <summary>
    /// Subtracts from this Enemy's health.
    /// </summary>
    /// <param name="amount">Amount to damage.</param>
    public void TakeDamage(int amount)
    {
        if (amount >= 0)
            health = Mathf.Clamp(health - amount, MIN_HEALTH, MAX_HEALTH);
        FlashDamage();
    }

    /// <summary>
    /// Sets this Enemy's attack range.
    /// </summary>
    /// <param name="amount">New attack range.</param>
    public void SetAttackRange(float amount)
    {
        if (amount >= MIN_ATTACK_RANGE && amount <= MAX_ATTACK_RANGE)
            attackRange = amount;
    }

    /// <summary>
    /// Sets this Enemy's attack range to its base value.
    /// </summary>
    public void ResetAttackRange()
    {
        attackRange = BASE_ATTACK_RANGE;
    }

    /// <summary>
    /// Sets this Enemy's health to its base value.
    /// </summary>
    public void ResetHealth()
    {
        health = BASE_HEALTH;
    }

    /// <summary>
    /// Sets this Enemy's attack speed to its base value.
    /// </summary>
    public void ResetAttackSpeed()
    {
        attackSpeed = BASE_ATTACK_SPEED;
    }

    /// <summary>
    /// Returns this Enemy's current attack speed.
    /// </summary>
    /// <returns>this Enemy's current attack speed.</returns>
    public float GetAttackSpeed()
    {
        return attackSpeed;
    }

    /// <summary>
    /// Sets this Enemy's attack speed.
    /// </summary>
    /// <param name="amount">The new attack speed.</param>
    public void SetAttackSpeed(float amount)
    {
        if (amount >= MIN_ATTACK_SPEED && amount <= MAX_ATTACK_SPEED)
            attackSpeed = amount;
    }

    /// <summary>
    /// Returns a copy of the prefab that this Enemy represents.
    /// </summary>
    /// <returns>a copy of the prefab that this Enemy represents.</returns>
    public GameObject CloneEnemy()
    {
        return Instantiate(prefab);
    }

    /// <summary>
    /// Flashes this Enemy to signal it has taken damage.
    /// </summary>
    public void FlashDamage()
    {
        enemyAnimator.SetTrigger("FlashRed");
    }

    /// <summary>
    /// Called when this Projectile collides with another body. Destroys
    /// the projectile.
    /// </summary>
    /// <param name="other">The other body.</param>
    public void OnTriggerEnter2D(Collider2D other)
    {
        Projectile projectile = other.gameObject.GetComponent<Projectile>();
        if (projectile != null)
        {
            SoundController.PlaySoundEffect("kudzuHit");
            TakeDamage(projectile.GetDamage());
            projectile.SetAsInactive();
        }
    }

    /// <summary>
    /// Updates the tile coordinates that this Enemy is standing on.
    /// </summary>
    /// <param name="x">The X-Coordinate</param>
    /// <param name="y">The Y-Coordinate</param>
    public void UpdateTilePosition(int x, int y)
    {
        if (x < 0 || y < 0) return;
        tileX = x;
        tileY = y;
    }

    /// <summary>
    /// Sets this Enemy's Direction.
    /// </summary>
    /// <param name="d">The Direction to set to.</param>
    protected void SetDirection(Direction d)
    {
        if (d == GetDirection()) return;
        direction = d;
    }

    /// <summary>
    /// Returns this Enemy's Direction.
    /// </summary>
    /// <returns>this Enemy's Direction.</returns>
    protected Direction GetDirection()
    {
        return direction;
    }


    /// <summary>
    /// Coroutine to play an animation from a Sprite animation track,
    /// mimicking a "flip-book."
    /// </summary>
    /// <param name="animationTrack">The Sprite animation track to play.</param>
    /// <param name="time">How long the animation should last.</param>
    /// <param name="startFrame">Which frame to start at.</param>
    /// <returns>A reference to the coroutine.</returns>
    protected override IEnumerator CoPlayAnimation()
    {
        int frame = 0;
        while (true)
        {
            //Get the right track.
            float animationTime;
            switch (GetCurrentAnimation())
            {
                case AnimationType.ATTACK:
                    healthyTrack = EnemyFactory.GetAttackTrack(TYPE, EnemyHealthState.HEALTHY, GetDirection());
                    damagedTrack = EnemyFactory.GetAttackTrack(TYPE, EnemyHealthState.DAMAGED, GetDirection());
                    criticalTrack = EnemyFactory.GetAttackTrack(TYPE, EnemyHealthState.CRITICAL, GetDirection());
                    animationTime = ATTACK_ANIMATION_TIME;
                    break;
                case AnimationType.MOVE:
                    healthyTrack = EnemyFactory.GetMovementTrack(TYPE, EnemyHealthState.HEALTHY, GetDirection());
                    damagedTrack = EnemyFactory.GetMovementTrack(TYPE, EnemyHealthState.DAMAGED, GetDirection());
                    criticalTrack = EnemyFactory.GetMovementTrack(TYPE, EnemyHealthState.CRITICAL, GetDirection());
                    animationTime = MOVE_ANIMATION_TIME;
                    break;
                default:
                    animationTime = 1f;
                    break;
            }

            if (healthyTrack != null && damagedTrack != null && criticalTrack != null)
            {
                //Transitive property
                Assert.AreEqual(healthyTrack.Length, damagedTrack.Length);
                Assert.AreEqual(damagedTrack.Length, criticalTrack.Length);

                //Using healthy track for length, but any one works after assertions
                float waitTime = animationTime / healthyTrack.Length;

                Sprite s;
                if (GetHealthState() == EnemyHealthState.HEALTHY) s = healthyTrack[frame];
                else if (GetHealthState() == EnemyHealthState.DAMAGED) s = damagedTrack[frame];
                else s = criticalTrack[frame];

                SetSprite(s);
                yield return new WaitForSeconds(waitTime);
                if (frame + 1 >= healthyTrack.Length) frame = 0;
                else frame++;
            }
            else yield return null;
        }
    }

    /// <summary>
    /// Updates any cooldowns managed by this Enemy.
    /// </summary>
    public virtual void UpdateCooldowns()
    {
        if (attackCooldownTimer > 0) attackCooldownTimer -= Time.deltaTime;
    }

    /// <summary>
    /// Returns this Enemy's Collider component.
    /// </summary>
    /// <returns>this Enemy's Collider component.</returns>
    public Collider2D GetColllider()
    {
        return enemyCollider;
    }

    /// <summary>
    /// Sets this Enemy's Collider2D properties, such as its position,
    /// width, and height.
    /// </summary>
    public abstract void SetColliderProperties();

    /// <summary>
    /// Returns true if this Enemy is in the middle of an attack.
    /// </summary>
    /// <returns>true if this Enemy is attacking an ITargetable right now;
    /// otherwise, false.</returns>
    public abstract bool Attacking();


    /// <summary>
    /// Returns the position at which an IAttackable will aim at when
    /// attacking this Enemy.
    /// </summary>
    /// <returns>this Enemy's attack position.</returns>
    public virtual Vector3 GetAttackPosition()
    {
        return transform.position;
    }

    /// <summary>
    /// Returns true if this Enemy is dead. This Enemy is dead if 
    /// its health is below or at zero.
    /// </summary>
    /// <returns>true if this Enemy is dead; otherwise, false.</returns>
    public bool Dead()
    {
        return GetHealth() <= 0;
    }

    /// <summary>
    /// Sets the SpawnTime of this Enemy.
    /// </summary>
    /// <param name="time">the time at which this Enemy spawns.</param>
    public void SetSpawnTime(float time)
    {
        Assert.IsTrue(time >= 0);
        spawnTime = time;
    }

    /// <summary>
    /// Returns this Enemy's SpawnTime.
    /// </summary>
    /// <returns>this Enemy's SpawnTime.</returns>
    public float GetSpawnTime()
    {
        return spawnTime;
    }

    /// <summary>
    /// Sets the (X, Y) world coordinates where this Enemy spawns in the level.
    /// </summary>
    /// <param name="coords">the (X, Y) coordinates.</param>
    public void SetSpawnWorldPosition(Vector2 coords)
    {
        spawnX = coords.x;
        spawnY = coords.y;
    }

    /// <summary>
    /// Returns the (X, Y) world position where this Enemy spawns in the level.
    /// </summary>
    /// <returns>the (X, Y) world position where this Enemy spawns.</returns>
    public Vector2 GetSpawnWorldPosition()
    {
        return new Vector2(spawnX, spawnY);
    }
}
