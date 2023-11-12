using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Represents a Kudzu enemy.
/// </summary>
public class Kudzu : Enemy
{
    /// <summary>
    /// Name of a Kudzu.
    /// </summary>
    public override string NAME => "Kudzu";

    /// <summary>
    /// Cost of placing a Kudzu from the inventory. 
    /// </summary>
    public override int COST => 0;

    /// <summary>
    /// Type of a Kudzu.
    /// </summary>
    public override EnemyType TYPE => EnemyType.KUDZU;

    /// <summary>
    /// The cooldown between each hop. 
    /// </summary>
    public float HOP_COOLDOWN => .1f;

    /// <summary>
    /// Base health of a Kudzu.
    /// </summary>
    public override int BASE_HEALTH => 100;

    /// <summary>
    /// Upper bound of a Kudzu's health. 
    /// </summary>
    public override int MAX_HEALTH => 100;

    /// <summary>
    /// Minimum health of a Kudzu
    /// </summary>
    public override int MIN_HEALTH => 0;

    /// <summary>
    /// Starting attack speed of a Kudzu.
    /// </summary>
    public override float BASE_ATTACK_SPEED => 3f;

    /// <summary>
    /// Damage a Kudzu does each attack.
    /// </summary>
    public int BONK_DAMAGE => 50;

    /// <summary>
    /// The chance, between 0-1, that a Kudzu drops loot
    /// upon its death.
    /// </summary>
    public override float LOOT_DROP_CHANCE => .5f;

    /// <summary>
    /// Starting attack range of a Kudzu.
    /// </summary>
    public override float BASE_ATTACK_RANGE => 1f;

    /// <summary>
    /// Maximum attack range of a Kudzu.
    /// </summary>
    public override float MAX_ATTACK_RANGE => float.MaxValue;

    /// <summary>
    /// Minimum attack range of a Kudzu.
    /// </summary>
    public override float MIN_ATTACK_RANGE => 0;

    /// <summary>
    /// Maximum attack speed of a Kudzu.
    /// </summary>
    public override float MAX_ATTACK_SPEED => float.MaxValue;

    /// <summary>
    /// Minimum attack speed of a Kudzu.
    /// </summary>
    public override float MIN_ATTACK_SPEED => 0;

    /// <summary>
    /// How many seconds a Kudzu's move animation lasts,
    /// from start to finish. 
    /// </summary>
    public float MOVE_ANIMATION_DURATION => .2f;

    /// <summary>
    /// How many seconds a Kudzu's attack animation lasts,
    /// from start to finish. 
    /// </summary>
    public float ATTACK_ANIMATION_DURATION => .3f;

    /// <summary>
    /// How many seconds a Kudzu's idle animation lasts,
    /// from start to finish. 
    /// </summary>
    public float IDLE_ANIMATION_DURATION => .3f;

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
    /// Sets this Kudzu to be hopping or not hopping.
    /// </summary>
    /// <param name="hopping">true if this Kudzu is hopping, false if not.</param>
    public void SetHopping(bool hopping) { this.hopping = hopping; }

    /// <summary>
    /// Returns true if this Kudzu is hopping.
    /// </summary>
    /// <returns>true if this Kudzu is hopping, false if not.</returns>
    public bool IsHopping() { return hopping; }

    /// <summary>
    /// Resets the cooldown for hopping. Call this when the Kudzu just
    /// hopped and needs to wait again.
    /// </summary>
    public void ResetHopCooldown() { hopCooldownTimer = HOP_COOLDOWN; }

    /// <summary>
    /// Takes a small bit of time off the cooldown. Call this every frame
    /// when the Kudzu should be trying to hop again.
    /// </summary>
    public void DecrementHopCooldown() { hopCooldownTimer -= Mathf.Clamp(Time.deltaTime, 0f, float.MaxValue); }

    /// <summary>
    /// Returns the number of seconds before this Kudzu can hop again.
    /// </summary>
    /// <returns>the number of seconds before this Kudzu can hop again. </returns>
    public float GetHopCooldown() { return hopCooldownTimer; }

    /// <summary>
    /// Sets this Kudzu's Collider2D properties.
    /// </summary>
    public override void SetColliderProperties()
    {
        BoxCollider2D collider = GetColllider() as BoxCollider2D;
        Assert.IsNotNull(collider, "Collider is null.");
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
        return new Vector3(transform.position.x, adjustedY, transform.position.z);
    }

    /// <summary>
    /// Returns the sprite that represents this Kudzu in the
    /// Inventory.
    /// </summary>
    /// <returns>the sprite that represents this Kudzu in the
    /// inventory.</returns>
    public override Sprite GetInventorySprite() { throw new System.NotImplementedException(); }

    /// <summary>
    /// Returns the sprite that represents this Kudzu when placing from the
    /// the Inventory.
    /// </summary>
    /// <returns>the sprite that represents this Kudzu when placing from the
    /// inventory.</returns>
    public override Sprite GetPlacementSprite() { throw new System.NotImplementedException(); }

    /// <summary>
    /// Returns the GameObject that represents this Kudzu on the grid.
    /// </summary>
    /// <returns>the GameObject that represents this Kudzu on the grid.
    /// </returns>
    public override GameObject MakePlaceableObject() { throw new System.NotImplementedException(); }
}
