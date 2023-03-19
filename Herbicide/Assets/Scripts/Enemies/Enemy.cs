using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Represents some entity whose goal is to make the player
/// lose the current level.
/// </summary>
public abstract class Enemy : MonoBehaviour
{
    /// <summary>
    /// How much health this Enemy starts with.
    /// </summary>
    protected abstract int STARTING_HEALTH { get; }

    /// <summary>
    /// The lowest amount of health this Enemy can have.
    /// </summary>
    protected abstract int MIN_HEALTH { get; }

    /// <summary>
    /// The greatest amount of health this Enemy can have.
    /// </summary>
    protected abstract int MAX_HEALTH { get; }

    /// <summary>
    /// Default / base attack range of this Enemy
    /// </summary>
    protected abstract float ATTACK_RANGE { get; }

    /// <summary>
    /// Name of this enemy.
    /// </summary>
    protected abstract string NAME { get; }

    /// <summary>
    /// How much health this Enemy currently has
    /// </summary>
    private int health;

    /// <summary>
    /// Current attack range of this Enemy
    /// </summary>
    private float attackRange;

    /// <summary>
    /// This Enemy's SpriteRenderer component.
    /// </summary>
    [SerializeField]
    private SpriteRenderer enemyRenderer;

    /// <summary>
    /// This Enemy's target.
    /// </summary>
    private PlaceableObject target;


    /// <summary>
    /// Attacks some target.
    /// </summary>
    /// <param name="target">the thing to attack.</param>
    public virtual void Attack(PlaceableObject target)
    {
        Debug.Log("Attacking a " + target.GetName());
    }

    /// <summary>
    /// Called when this Enemy spawns. Sets its health and attack range
    /// to their starting values.
    /// </summary>
    public virtual void Spawn()
    {
        SetHealth(STARTING_HEALTH);
        SetAttackRange(ATTACK_RANGE);
    }

    public void Hello()
    {
        Debug.Log("Hello");
    }

    /// <summary>
    /// Called when this Enemy dies. Destroys its Enemy script and 
    /// GameObject.
    /// </summary>
    public virtual void OnDie()
    {
        Enemy e = this;
        Destroy(gameObject);
        Destroy(e);
    }

    /// <summary>
    /// Sets the health of this Enemy if it is within its max and min
    /// health.
    /// </summary>
    /// <param name="newHealth">the new health to set to.</param>
    protected virtual void SetHealth(int newHealth)
    {
        if (newHealth > MAX_HEALTH || newHealth < MIN_HEALTH) return;
        health = newHealth;
    }

    /// <summary>
    /// Sets the non-negative attack range of this Enemy.
    /// </summary>
    /// <param name="newRange">the new attack range to set to</param>
    protected virtual void SetAttackRange(float newRange)
    {
        if (newRange < 0) return;
        attackRange = newRange;
    }



    /// <summary>
    /// Returns the attack range of this Enemy.
    /// </summary>
    /// <returns>the attack range of this Enemy.</returns>
    public float GetAttackRange()
    {
        return attackRange;
    }

    /// <summary>
    /// Returns a GameObject copy of this Enemy with default values.
    /// </summary>
    /// <returns>a GameObject copy of this Enemy with default values.</returns>
    public virtual GameObject CloneEnemy()
    {
        GameObject enemyOb = Instantiate(gameObject);
        Enemy enemy = enemyOb.GetComponent<Enemy>();
        Assert.IsNotNull(enemy);

        //Set values to starting numbers
        enemy.SetHealth(STARTING_HEALTH);

        return enemyOb;
    }

    /// <summary>
    /// Sets the Sprite of this Enemy.
    /// </summary>
    /// <param name="s">The Sprite to set to</param>
    public void SetSprite(Sprite s)
    {
        if (s != null) enemyRenderer.sprite = s;
    }

    /// <summary>
    /// Returns this Enemy's name.
    /// </summary>
    /// <returns>the name of this Enemy.</returns>
    public string GetName()
    {
        return NAME;
    }

}
