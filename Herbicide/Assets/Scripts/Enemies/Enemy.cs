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
    /// Attacks some target.
    /// </summary>
    /// <param name="target">the thing to attack.</param>
    public abstract void Attack(PlaceableObject target);

    /// <summary>
    /// Performs some action when this Enemy is spawned.
    /// </summary>
    public abstract void OnSpawn();

    /// <summary>
    /// Performs some action when this Enemy dies.
    /// </summary>
    public abstract void OnDie();

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

}
