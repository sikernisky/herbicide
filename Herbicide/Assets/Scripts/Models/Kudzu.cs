using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Represents a Kudzu enemy.
/// </summary>
public class Kudzu : MovingEnemy
{
    /// <summary>
    /// Type of a Kudzu.
    /// </summary>
    public override EnemyType TYPE => EnemyType.KUDZU;

    /// <summary>
    /// Base speed of a Kudzu: represents the cooldown between
    /// hops.
    /// </summary>
    public override float BASE_SPEED => .4f;

    /// <summary>
    /// Base health of a Kudzu.
    /// </summary>
    public override int BASE_HEALTH => 100;

    /// <summary>
    /// Upper bound of a Kudzu's health. 
    /// </summary>
    public override int MAX_HEALTH => 100;

    /// <summary>
    /// Starting attack range of a Kudzu.
    /// </summary>
    public override float BASE_ATTACK_RANGE => 1f;

    /// <summary>
    /// Starting attack speed of a Kudzu.
    /// </summary>
    public override float BASE_ATTACK_SPEED => 3f;

    /// <summary>
    /// Damage a Kudzu does each attack.
    /// </summary>
    public override int DAMAGE_PER_ATTACK => 50;

    /// <summary>
    /// Time for a Kudzu move/hop animation.
    /// </summary>
    public override float MOVE_ANIMATION_TIME => .2f;

    /// <summary>
    /// Name of a Kudzu.
    /// </summary>
    protected override string NAME => "Kudzu";

    /// <summary>
    /// Cost of placing a Kudzu from the inventory. 
    /// </summary>
    protected override int COST => 0;

    /// <summary>
    /// true if this Kudzu is currently hopping.
    /// </summary>
    private bool hopping;

    /// <summary>
    /// true if this Kudzu is currently bonking.
    /// </summary>
    private bool bonking;

    /// <summary>
    /// How many seconds remain before this Kudzu can hop.
    /// </summary>
    private float hopCooldownTimer;


    /// <summary>
    /// Moves this Kudzu to a target position, causing it to "hop"
    /// there.
    /// </summary>
    /// <param name="targetPosition">The position to move to.</param>

    public override void MoveTo(Vector3 movePosition)
    {
        base.MoveTo(movePosition);
        if (CanHop())
        {
            //If we're able to hop somewhere, do it. Otherwise, hop in place.
            if (hopCooldownTimer <= 0) StartCoroutine(CoHop(movePosition));
            else StartCoroutine(CoHop());
        }
    }

    /// <summary>
    /// Returns true if this Kudzu is bonking an ITargetable right now.
    /// </summary>
    /// <returns>true if this Kudzu is bonking an ITargetable; otherwise,
    /// false.</returns>
    public override bool Attacking()
    {
        return bonking;
    }

    /// <summary>
    /// Plays this Kudzu's bonking attack animation.
    /// </summary>
    /// <param name="target">The ITargetable to bonk.</param>
    /// <returns>A reference to the coroutine.</returns>
    private IEnumerator CoBonk(ITargetable target)
    {
        bonking = true;
        bool bonked = false;
        float time = 0f;

        PlayAnimation(EnemyAnimationType.ATTACK);
        while (time < ATTACK_ANIMATION_TIME)
        {
            if (!bonked)
            {
                //Bonk logic here!
                target.TakeDamage(DAMAGE_PER_ATTACK);
                bonked = true;
            }
            time += Time.deltaTime;
            yield return null;
        }
        ResetAttackTimer();
        bonking = false;
    }

    /// <summary>
    /// Hops to a target position and resets the hop cooldown once done.
    /// </summary>
    /// <param name="targetPosition">The position to hop towards.</param>
    /// <returns>A reference to the coroutine.</returns>
    private IEnumerator CoHop(Vector3 targetPosition)
    {
        hopping = true;
        Vector3 startPos = transform.position;
        PlayAnimation(EnemyAnimationType.MOVE);

        float time = 0f;
        while (time < MOVE_ANIMATION_TIME)
        {
            float t = time / MOVE_ANIMATION_TIME;
            transform.position = Vector3.MoveTowards(
                startPos, targetPosition,
                t * Vector3.Distance(startPos, targetPosition));
            time += Time.deltaTime;

            yield return null;
        }
        hopCooldownTimer = GetMoveSpeed();
        transform.position = targetPosition;
        hopping = false;
    }

    /// <summary>
    /// Hops in place. Does not reset the hop cooldown when finished. 
    /// </summary>
    /// <returns>A reference to the coroutine.</returns>
    private IEnumerator CoHop()
    {
        hopping = true;
        Vector3 startPos = transform.position;
        PlayAnimation(EnemyAnimationType.MOVE);

        float time = 0f;
        while (time < MOVE_ANIMATION_TIME)
        {
            float t = time / MOVE_ANIMATION_TIME;
            transform.position = Vector3.MoveTowards(
                startPos, GetPosition(),
                t * Vector3.Distance(startPos, GetPosition()));
            time += Time.deltaTime;

            yield return null;
        }
        transform.position = GetPosition();
        hopping = false;
    }

    /// <summary>
    /// Updates this Kudzu's hop cooldown.
    /// </summary>
    public override void UpdateCooldowns()
    {
        base.UpdateCooldowns();
        if (hopCooldownTimer > 0) hopCooldownTimer -= Time.deltaTime;
    }

    /// <summary>
    /// Returns true if this Kudzu can hop.
    /// </summary>
    /// <returns>true if this Kudzu can hop; otherwise, false.
    /// </returns>
    private bool CanHop()
    {
        return !hopping;
    }

    /// <summary>
    /// Returns true if this Kudzu can bonk.
    /// </summary>
    /// <returns>true if this Kudzu can bonk; otherwise, false.</returns>
    private bool CanBonk()
    {
        return !bonking;
    }

    /// <summary>
    /// Sets this Kudzu's Collider2D properties.
    /// </summary>
    public override void SetColliderProperties()
    {
        BoxCollider2D collider = GetColllider() as BoxCollider2D;
        collider.offset = new Vector2(0, .5f);
    }


    /// <summary>
    /// Returns the position at which an IAttackable will aim at when
    /// attacking this Kudzu.
    /// </summary>
    /// <returns>this Kuduu's attack position.</returns>
    public override Vector3 GetAttackPosition()
    {
        float adjustedY = transform.position.y + .25f;
        return new Vector3(
            transform.position.x,
            adjustedY,
            transform.position.z
        );
    }

    /// <summary>
    /// Returns true if this Kudzu can target some ITargetable. Kudzus
    /// can target any defender. 
    /// </summary>
    /// <param name="target">the ITargetable to check.</param>
    /// <returns>true if this Kudzu can target the ITargetable;
    /// otherwise, false. </returns>
    public override bool CanTarget(ITargetable target)
    {
        return target as Defender != null;
    }

    /// <summary>
    /// Returns the sprite that represents this Kudzu in the
    /// Inventory.
    /// </summary>
    /// <returns>the sprite that represents this Kudzu in the
    /// inventory.</returns>
    public override Sprite GetInventorySprite()
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Returns the sprite that represents this Kudzu when placing from the
    /// the Inventory.
    /// </summary>
    /// <returns>the sprite that represents this Kudzu when placing from the
    /// inventory.</returns>
    public override Sprite GetPlacementSprite()
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Returns a GameObject that represents a new Kudzu. It has the
    /// Kudzu component attached and is instantiated from the
    /// EnemyFactory.
    /// </summary>
    /// <returns></returns>
    public override GameObject MakePlaceableObject()
    {
        //TODO: Implement from EnemyFactory

        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Returns a GameObject with this Kudzu's appearance
    /// but not its functionality.
    /// </summary>
    /// <returns>a GameObject with this Kudzu's appearance
    /// but not its functionality.</returns>
    public override GameObject MakeHollowObject()
    {
        throw new System.NotImplementedException();
    }


    /// <summary>
    /// Calculates the state of this Kudzu.
    /// </summary>
    /// <param name="currentState">Most recent state of this Kudzu.</param
    /// <param name="currentState">Distance to this Kudzu's target.</param /// 
    public override MobState DetermineState(MobState currentState,
        float distanceToTarget)
    {
        switch (currentState)
        {
            case MobState.INVALID:
                return MobState.INVALID;
            case MobState.INACTIVE:
                return MobState.INACTIVE;
            case MobState.SPAWN:
                return MobState.CHASE;
            case MobState.IDLE:
                if (distanceToTarget <= GetAttackRange()) return MobState.ATTACK;
                else return MobState.CHASE;
            case MobState.ATTACK:
                if (distanceToTarget > GetAttackRange()) return MobState.CHASE;
                else return MobState.ATTACK;
            case MobState.CHASE:
                if (distanceToTarget <= GetAttackRange()) return MobState.ATTACK;
                else return MobState.CHASE;
            default:
                //should not get here
                throw new System.Exception();
        }
    }

    //------------------STATE LOGIC----------------------//

    /// <summary>
    /// Attacks a target.
    /// </summary>
    /// <param name="target">The ITargetable to attack.</param>
    public override void Attack(ITargetable target)
    {
        //These checks take care of the attack cooldown timer, unlike moving.
        if (!CanAttackNow(target)) return;
        if (!CanAttackEver(target)) return;
        if (!CanBonk()) return;

        FaceTarget(target);
        StartCoroutine(CoBonk(target));
    }

    /// <summary>
    /// Chases its target.
    /// </summary>
    /// <param name="target">The ITargetable to chase.</param>
    public override void Chase(ITargetable target)
    {
        Assert.IsNotNull(target, "Cannot chase a null target.");

        MoveTo(GetNextMovePos());
    }

    /// <summary>
    /// Idles.
    /// </summary>
    public override void Idle()
    {
        throw new System.NotImplementedException();
    }
}
