using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents something that is placed on the TileGrid
/// and helps the player win.
/// </summary>
public abstract class Defender : Mob, IAttackable
{
    /// <summary>
    /// Type of this Defender.
    /// </summary>
    public abstract DefenderType TYPE { get; }

    /// <summary>
    /// Class of this Defender.
    /// </summary>
    public abstract DefenderClass CLASS { get; }

    /// <summary>
    /// Starting health of a Defender. 
    /// </summary>
    public override int BASE_HEALTH => 200;

    /// <summary>
    /// This Defender's largest possible health value.
    /// </summary>
    public override int MAX_HEALTH => 200;

    /// <summary>
    /// This Defender's smallest possible health value.
    /// </summary>
    public override int MIN_HEALTH => 0;

    /// <summary>
    /// Starting attack range of a Defender.
    /// </summary>
    public override float BASE_ATTACK_RANGE => 5f;

    /// <summary>
    /// Upper bound of a Defender's attack range.
    /// </summary>
    public override float MAX_ATTACK_RANGE => float.MaxValue;

    /// <summary>
    /// Lower bound of a Defender's attack range.
    /// </summary>
    public override float MIN_ATTACK_RANGE => 0f;

    /// <summary>
    /// Starting attack speed of a Defender (number of attacks / second).
    /// </summary>
    public override float BASE_ATTACK_SPEED => 1.5f;

    /// <summary>
    /// Upper bound of a Defender's attack speed (number of attacks / second).
    /// </summary>
    public override float MAX_ATTACK_SPEED => 10f;

    /// <summary>
    /// Lower bound of a Defender's attack speed (number of attacks / second).
    /// </summary>
    public override float MIN_ATTACK_SPEED => 0f;

    /// <summary>
    /// Damage inflicted on a target(s) every time this Defender attacks.
    /// </summary>
    public int DAMAGE_PER_ATTACK => 20;

    /// <summary>
    /// How long to flash when this Defender takes damage
    /// </summary>
    public float DAMAGE_FLASH_TIME => .5f;

    /// <summary>
    /// How long an attack lasts
    /// </summary>
    public float ATTACK_DURATION => .25f;

    /// <summary>
    /// Duration of attack as a percentage of time between attacks
    /// </summary>
    public float ATTACK_DURATION_PERCENTAGE => .9f;

    /// <summary>
    /// Health of this Defender. 
    /// </summary>
    private int health;

    /// <summary>
    /// Attack speed of this Defender. 
    /// </summary>
    private float attackSpeed;

    /// <summary>
    /// Attack range of this Defender. 
    /// </summary>
    private float attackRange;

    /// <summary>
    /// Current direction of this Defender.
    /// </summary>
    private Direction direction;

    /// <summary>
    /// How many seconds this Defender has been flashing damage
    /// for.
    /// </summary>
    private float damageFlashingTime;

    /// <summary>
    /// This Defender's Collider component. 
    /// </summary>
    [SerializeField]
    private Collider2D defenderCollider;

    /// <summary>
    /// Type of this Defender.
    /// </summary>
    public enum DefenderType
    {
        SQUIRREL
    }

    /// <summary>
    /// Class of this Defender.
    /// </summary>
    public enum DefenderClass
    {
        TREBUCHET,
        MAULER,
        PATHFINDER,
        ANIMYST
    }

    /// <summary>
    /// Returns a Sprite component that represents this Defender
    /// in an InventorySlot.
    /// </summary>
    /// <returns> a Sprite component that represents this Defender
    /// in an InventorySlot.</returns>
    public override Sprite GetInventorySprite()
    {
        return DefenderFactory.GetDefenderInventorySprite(TYPE);
    }

    /// <summary>
    /// Returns a Sprite component that represents this Defender
    /// when it is placing.
    /// </summary>
    /// <returns> a Sprite component that represents this Defender
    /// when it is placing.</returns>
    public override Sprite GetPlacementSprite()
    {
        return DefenderFactory.GetDefenderPlacedSprite(TYPE);
    }

    /// <summary>
    /// Returns a Defender GameObject that can be placed on the
    /// TileGrid.
    /// </summary>
    /// <returns>a Defender GameObject that can be placed on the
    /// TileGrid.</returns>
    public override GameObject MakePlaceableObject()
    {
        return Instantiate(DefenderFactory.GetDefenderPrefab(TYPE));
    }

    /// <summary>
    /// Performs actions when this Defender first enters the scene.
    /// </summary>
    public override void OnSpawn()
    {
        base.OnSpawn();
        ResetStats();
    }

    /// <summary>
    /// Returns a GameObject that holds a SpriteRenderer component with
    /// this Defender's placed Sprite. No other components are
    /// copied. 
    /// </summary>
    /// <returns>A GameObject with a SpriteRenderer component. </returns>
    public override GameObject MakeHollowObject()
    {
        GameObject hollowCopy = new GameObject("Hollow " + name);
        SpriteRenderer hollowRenderer = hollowCopy.AddComponent<SpriteRenderer>();
        hollowRenderer.sprite = DefenderFactory.GetDefenderPlacedSprite(TYPE);

        hollowCopy.transform.position = transform.position;
        hollowCopy.transform.localScale = transform.localScale;

        return hollowCopy;
    }

    /// <summary>
    /// Attacks an ITargetable.
    /// </summary>
    /// <param name="target">The ITargetable to attack.</param>
    public virtual void Attack(ITargetable target)
    {
        if (target == null) return;
        if (!CanAttackNow(target) || !CanAttackEver(target))
        {
            RotateDefender(Direction.SOUTH);
            return;
        }

        float xDistance = GetPosition().x - target.GetPosition().x;
        float yDistance = GetPosition().y - target.GetPosition().y;

        bool xGreater = Mathf.Abs(xDistance) > Mathf.Abs(yDistance)
            ? true : false;

        if (xGreater && xDistance <= 0) RotateDefender(Direction.EAST);
        if (xGreater && xDistance > 0) RotateDefender(Direction.WEST);
        if (!xGreater && yDistance <= 0) RotateDefender(Direction.NORTH);
        if (!xGreater && yDistance > 0) RotateDefender(Direction.SOUTH);
    }


    /// <summary>
    /// Returns true if this Defender can ever attack another IAttackable.
    /// </summary>
    /// <param name="target">the target to try to attack.</param>
    /// <returns>true if this Defender can ever attack another IAttackable.
    public virtual bool CanAttackEver(ITargetable target)
    {
        if (target == null) return false;

        return true;
    }

    /// <summary>
    /// Returns true if this Defender can attack another IAttackable at
    /// the current frame.
    /// </summary>
    /// <param name="target">the target to try to attack.</param>
    /// <returns>true if this Defender can attack another IAttackable at
    /// the current frame.</returns>
    public virtual bool CanAttackNow(ITargetable target)
    {
        if (DistanceToTarget(target) > GetAttackRange()) return false;
        if (GetAttackSpeed() <= 0) return false;

        return true;
    }

    /// <summary>
    /// Returns the Euclidian distance between this Defender and some
    /// IAttackable.
    /// </summary>
    /// <param name="target">The IAttackable to measure the distance from this 
    /// Defender.</param>
    public float DistanceToTarget(ITargetable target)
    {
        if (target == null) return float.MaxValue;

        Vector2 defenderPosition = GetPosition();
        Vector2 targetPosition = target.GetPosition();
        return Vector2.Distance(defenderPosition, targetPosition);
    }

    /// <summary>
    /// Returns this Defender's current attack range.
    /// </summary>
    /// <returns>this Defender's current attack range.</returns>
    public float GetAttackRange()
    {
        return attackRange;
    }

    /// <summary>
    /// Returns this Defender's current health.
    /// </summary>
    /// <returns>this Defender's current health.</returns>
    public int GetHealth()
    {
        return health;
    }

    /// <summary>
    /// Returns this Defender's current position.
    /// </summary>
    /// <returns>this Defender's current position.</returns>
    public Vector3 GetPosition()
    {
        return transform.position;
    }

    /// <summary>
    /// Adds to this Defender's health.
    /// </summary>
    /// <param name="amount">Amount to heal.</param>
    public void Heal(int amount)
    {
        if (amount >= 0)
            health = Mathf.Clamp(health + amount, MIN_HEALTH, MAX_HEALTH);
    }

    /// <summary>
    /// Subtracts from this Defender's health.
    /// </summary>
    /// <param name="amount">Amount to damage.</param>
    public void TakeDamage(int amount)
    {
        if (amount >= 0)
        {
            health = Mathf.Clamp(health - amount, MIN_HEALTH, MAX_HEALTH);
            FlashDamage();
        }
    }

    /// <summary>
    /// Sets this Defender's attack range.
    /// </summary>
    /// <param name="amount">New attack range.</param>
    public void SetAttackRange(float amount)
    {
        if (amount >= MIN_ATTACK_RANGE && amount <= MAX_ATTACK_RANGE)
            attackRange = amount;
    }

    /// <summary>
    /// Sets this Defender's attack range to its base value.
    /// </summary>
    public void ResetAttackRange()
    {
        attackRange = BASE_ATTACK_RANGE;
    }

    /// <summary>
    /// Sets this Defender's health to its base value.
    /// </summary>
    public void ResetHealth()
    {
        health = BASE_HEALTH;
    }

    /// <summary>
    /// Sets this Defender's attack speed to its base value.
    /// </summary>
    public void ResetAttackSpeed()
    {
        attackSpeed = BASE_ATTACK_SPEED;
    }

    /// <summary>
    /// Returns this Defender's current attack speed.
    /// </summary>
    /// <returns>this Defender's current attack speed.</returns>
    public float GetAttackSpeed()
    {
        return attackSpeed;
    }

    /// <summary>
    /// Sets this Defender's attack speed.
    /// </summary>
    /// <param name="amount">The new attack speed.</param>
    public void SetAttackSpeed(float amount)
    {
        if (amount >= MIN_ATTACK_SPEED && amount <= MAX_ATTACK_SPEED)
            attackSpeed = amount;
    }

    /// <summary>
    /// Resets this Defender's health, attack range, and attack speed.
    /// </summary>
    public void ResetStats()
    {
        ResetHealth();
        ResetAttackRange();
        ResetAttackSpeed();
    }

    /// <summary>
    /// Flashes this Defender to signal it has taken damage.
    /// </summary>
    public void FlashDamage()
    {
        SetDamageFlashingTime(DAMAGE_FLASH_TIME);
    }

    /// <summary>
    /// Turns this Defender North, East, South, or West. Updates its
    /// animation track accordingly.
    /// </summary>
    /// <param name="direction">the direction to rotate</param>
    public void RotateDefender(Direction direction)
    {
        this.direction = direction;
    }

    /// <summary>
    /// Returns the Direction this Defender is facing.
    /// </summary>
    /// <returns>the Direction this Defender is facing.</returns>
    protected Direction GetDirection()
    {
        return direction;
    }

    /// <summary>
    /// Sets this Defender's Sprite to represent its attacking
    /// in its current direction.
    /// </summary>
    protected void SetAttackSprite()
    {
        switch (TYPE)
        {
            case DefenderType.SQUIRREL:
                SetSprite(DefenderFactory.GetDefenderAttackSprite(TYPE, GetDirection()));
                break;
            default:
                throw new System.Exception("Defender not supported.");
        }

        //SetSprite(attackSprites[(int)GetDirection()]);
    }

    /// <summary>
    /// Sets this Defender's Sprite to represent its idling
    /// in its current direction.
    /// </summary>
    protected void SetIdleSprite()
    {
        switch (TYPE)
        {
            case DefenderType.SQUIRREL:
                SetSprite(DefenderFactory.GetDefenderIdleSprite(TYPE, GetDirection()));
                break;
            default:
                throw new System.Exception("Defender not supported.");
        }
    }

    /// <summary>
    /// Returns this Defender's Collider component.
    /// </summary>
    /// <returns>this Defender's Collider component.</returns>
    public Collider2D GetColllider()
    {
        return defenderCollider;
    }

    /// <summary>
    /// Returns the position at which an IAttackable will aim at when
    /// attacking this Defender.
    /// </summary>
    /// <returns>this Defender's attack position.</returns>
    public Vector3 GetAttackPosition()
    {
        return transform.position;
    }

    /// <summary>
    /// Called when this Defender dies.
    /// </summary>
    public void Die()
    {
        return;
    }

    /// <summary>
    /// Returns true if this Defender is dead. This Defender is dead if 
    /// its health is below or at zero.
    /// </summary>
    /// <returns>true if this Defender is dead; otherwise, false.</returns>
    public bool Dead()
    {
        return GetHealth() == 0;
    }

    /// <summary>
    /// Sets the number of seconds that this Defender has been
    /// flashing its damage effect.
    /// </summary>
    /// <param name="time">The number of seconds this Defender
    /// has been playing its damage effect </param>
    public void SetDamageFlashingTime(float time)
    {
        if (time < 0 || time > DAMAGE_FLASH_TIME) return;
        damageFlashingTime = time;
    }

    /// <summary>
    /// Returns the number of seconds that this Defender has
    /// been playing its damage flash effect.
    /// </summary>
    /// <returns>the number of seconds that this Defender has
    /// been playing its damage flash effect.</returns>
    public float GetDamageFlashingTime()
    {
        return damageFlashingTime;
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
        throw new System.NotImplementedException();
    }
}
