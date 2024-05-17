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
    /// Name of this Kudzu.
    /// </summary>
    public override string NAME => "Kudzu";

    /// <summary>
    /// Cost of placing a Kudzu from the inventory. 
    /// </summary>
    public override int COST => 0;

    /// <summary>
    /// Type of a Kudzu.
    /// </summary>
    public override ModelType TYPE => ModelType.KUDZU;

    /// <summary>
    /// The cooldown between each hop. <br></br>
    /// 
    /// This is different from movement speed. Movement speed is how fast
    /// the Kudzu travels while hopping. This cooldown is how long the Kudzu
    /// must wait before hopping again.
    /// </summary>
    public float HOP_COOLDOWN => 1f; //DEFAULT: 2.5f

    /// <summary>
    /// Base health of a Kudzu.
    /// </summary>
    public override int BASE_HEALTH => 100;

    /// <summary>
    /// Upper bound of a Kudzu's health. 
    /// </summary>
    public override int MAX_HEALTH => 150;

    /// <summary>
    /// Minimum health of a Kudzu
    /// </summary>
    public override int MIN_HEALTH => 0;

    /// <summary>
    /// Amount of attack cooldown this Kudzu starts with.
    /// </summary>
    public override float BASE_ATTACK_SPEED => 1f;

    /// <summary>
    /// Most amount of attack cooldown this Kudzu can have.
    /// </summary>
    public override float MAX_ATTACK_SPEED => float.MaxValue;

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
    /// Starting chase range of a Kudzu.
    /// </summary>
    public override float BASE_CHASE_RANGE => float.MaxValue;

    /// <summary>
    /// Maximum chase range of a Kudzu.
    /// </summary>
    public override float MAX_CHASE_RANGE => float.MaxValue;

    /// <summary>
    /// Minimum chase range of a Kudzu.
    /// </summary>
    public override float MIN_CHASE_RANGE => float.MaxValue;

    /// <summary>
    /// How many seconds a Kudzu's move animation lasts,
    /// from start to finish. 
    /// </summary>
    public float MOVE_ANIMATION_DURATION => .4f;

    /// <summary>
    /// How many seconds a Kudzu's attack animation lasts,
    /// from start to finish. 
    /// </summary>
    public float ATTACK_ANIMATION_DURATION => .25f;

    /// <summary>
    /// How many seconds a Kudzu's idle animation lasts,
    /// from start to finish. 
    /// </summary>
    public float IDLE_ANIMATION_DURATION => .3f;

    /// <summary>
    /// Starting movement speed of a Kudzu.
    /// </summary>
    public override float BASE_MOVEMENT_SPEED => 4f;

    /// <summary>
    /// Maximum movement speed of a Kudzu.
    /// </summary>
    public override float MAX_MOVEMENT_SPEED => float.MaxValue;

    /// <summary>
    /// Minumum movement speed of a Kudzu.
    /// </summary>
    public override float MIN_MOVEMENT_SPEED => 0f;

    /// <summary>
    /// The offset of the Model held by this Kudzu.
    /// </summary>
    public override Vector2 HOLDER_OFFSET => new Vector2(0, .25f);

    /// <summary>
    /// How many seconds remain before this Kudzu can hop.
    /// </summary>
    private float hopCooldownTimer;


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
    /// Returns the sprite that represents this Kudzu when placing.
    /// </summary>
    /// <returns>the sprite that represents this Kudzu when placing.</returns>
    public override Sprite[] GetPlacementTrack() { return KudzuFactory.GetPlacementTrack(); }

    /// <summary>
    /// Returns the GameObject that represents this Kudzu on the grid.
    /// </summary>
    /// <returns>the GameObject that represents this Kudzu on the grid.
    /// </returns>
    public override GameObject Copy() { return KudzuFactory.GetKudzuPrefab(); }
}
