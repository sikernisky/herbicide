using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Produces assets related to Kudzus.
/// </summary>
public class KudzuFactory : Factory
{
    /// <summary>
    /// Reference to the KudzuFactory singleton.
    /// </summary>
    private static KudzuFactory instance;

    /// <summary>
    /// Animation track when placing.
    /// </summary>
    [SerializeField]
    private Sprite[] placementTrack;

    /// <summary>
    /// Animation track when on a boat.
    /// </summary>
    [SerializeField]
    private Sprite[] boatTrack;

    /// <summary>
    /// Movement animation when this Kudzu is healthy.
    /// </summary>
    [SerializeField]
    private Sprite[] healthyMovementAnimationNorth;

    /// <summary>
    /// Movement animation when this Kudzu is damaged.
    /// </summary>
    [SerializeField]
    private Sprite[] damagedMovementAnimationNorth;

    /// <summary>
    /// Movement animation when this Kudzu is critically damaged.
    /// </summary>
    [SerializeField]
    private Sprite[] criticalMovementAnimationNorth;

    /// <summary>
    /// Movement animation when this Kudzu is healthy.
    /// </summary>
    [SerializeField]
    private Sprite[] healthyMovementAnimationEast;

    /// <summary>
    /// Movement animation when this Kudzu is damaged.
    /// </summary>
    [SerializeField]
    private Sprite[] damagedMovementAnimationEast;

    /// <summary>
    /// Movement animation when this Kudzu is critically damaged.
    /// </summary>
    [SerializeField]
    private Sprite[] criticalMovementAnimationEast;

    /// <summary>
    /// Movement animation when this Kudzu is healthy.
    /// </summary>
    [SerializeField]
    private Sprite[] healthyMovementAnimationSouth;

    /// <summary>
    /// Movement animation when this Kudzu is damaged.
    /// </summary>
    [SerializeField]
    private Sprite[] damagedMovementAnimationSouth;

    /// <summary>
    /// Movement animation when this Kudzu is critically damaged.
    /// </summary>
    [SerializeField]
    private Sprite[] criticalMovementAnimationSouth;

    /// <summary>
    /// Movement animation when this Kudzu is healthy.
    /// </summary>
    [SerializeField]
    private Sprite[] healthyMovementAnimationWest;

    /// <summary>
    /// Movement animation when this Kudzu is damaged.
    /// </summary>
    [SerializeField]
    private Sprite[] damagedMovementAnimationWest;

    /// <summary>
    /// Movement animation when this Kudzu is damaged.
    /// </summary>
    [SerializeField]
    private Sprite[] criticalMovementAnimationWest;


    /// <summary>
    /// Attack animation when this Kudzu is healthy.
    /// </summary>
    [SerializeField]
    private Sprite[] healthyAttackAnimationNorth;

    /// <summary>
    /// Attack animation when this Kudzu is damaged.
    /// </summary>
    [SerializeField]
    private Sprite[] damagedAttackAnimationNorth;

    /// <summary>
    /// Attack animation when this Kudzu is critically damaged.
    /// </summary>
    [SerializeField]
    private Sprite[] criticalAttackAnimationNorth;

    /// <summary>
    /// Attack animation when this Kudzu is healthy.
    /// </summary>
    [SerializeField]
    private Sprite[] healthyAttackAnimationEast;

    /// <summary>
    /// Attack animation when this Kudzu is damaged.
    /// </summary>
    [SerializeField]
    private Sprite[] damagedAttackAnimationEast;

    /// <summary>
    /// Attack animation when this Kudzu is critically damaged.
    /// </summary>
    [SerializeField]
    private Sprite[] criticalAttackAnimationEast;

    /// <summary>
    /// Attack animation when this Kudzu is healthy.
    /// </summary>
    [SerializeField]
    private Sprite[] healthyAttackAnimationSouth;

    /// <summary>
    /// Attack animation when this Kudzu is damaged.
    /// </summary>
    [SerializeField]
    private Sprite[] damagedAttackAnimationSouth;

    /// <summary>
    /// Attack animation when this Kudzu is critically damaged.
    /// </summary>
    [SerializeField]
    private Sprite[] criticalAttackAnimationSouth;

    /// <summary>
    /// Attack animation when this Kudzu is healthy.
    /// </summary>
    [SerializeField]
    private Sprite[] healthyAttackAnimationWest;

    /// <summary>
    /// Attack animation when this Kudzu is damaged.
    /// </summary>
    [SerializeField]
    private Sprite[] damagedAttackAnimationWest;

    /// <summary>
    /// Attack animation when this Kudzu is critically damaged.
    /// </summary>
    [SerializeField]
    private Sprite[] criticalAttackAnimationWest;

    /// <summary>
    /// Idle animation when this Kudzu is healthy.
    /// </summary>
    [SerializeField]
    private Sprite[] healthyIdleAnimationNorth;

    /// <summary>
    /// Idle animation when this Kudzu is damaged.
    /// </summary>
    [SerializeField]
    private Sprite[] damagedIdleAnimationNorth;

    /// <summary>
    /// Idle animation when this Kudzu is damaged.
    /// </summary>
    [SerializeField]
    private Sprite[] criticalIdleAnimationNorth;

    /// <summary>
    /// Idle animation when this Kudzu is healthy.
    /// </summary>
    [SerializeField]
    private Sprite[] healthyIdleAnimationEast;

    /// <summary>
    /// Idle animation when this Kudzu is damaged.
    /// </summary>
    [SerializeField]
    private Sprite[] damagedIdleAnimationEast;

    /// <summary>
    /// Idle animation when this Kudzu is damaged.
    /// </summary>
    [SerializeField]
    private Sprite[] criticalIdleAnimationEast;

    /// <summary>
    /// Idle animation when this Kudzu is healthy.
    /// </summary>
    [SerializeField]
    private Sprite[] healthyIdleAnimationSouth;

    /// <summary>
    /// Idle animation when this Kudzu is damaged.
    /// </summary>
    [SerializeField]
    private Sprite[] damagedIdleAnimationSouth;

    /// <summary>
    /// Idle animation when this Kudzu is damaged.
    /// </summary>
    [SerializeField]
    private Sprite[] criticalIdleAnimationSouth;

    /// <summary>
    /// Idle animation when this Kudzu is healthy.
    /// </summary>
    [SerializeField]
    private Sprite[] healthyIdleAnimationWest;

    /// <summary>
    /// Idle animation when this Kudzu is damaged.
    /// </summary>
    [SerializeField]
    private Sprite[] damagedIdleAnimationWest;


    /// <summary>
    /// Finds and sets the KudzuFactory singleton.
    /// </summary>
    /// <param name="levelController">The LevelController singleton.</param>
    public static void SetSingleton(LevelController levelController)
    {
        if (levelController == null) return;
        if (instance != null) return;

        KudzuFactory[] kudzuFactories = FindObjectsOfType<KudzuFactory>();
        Assert.IsNotNull(kudzuFactories, "Array of TileFactories is null.");
        Assert.AreEqual(1, kudzuFactories.Length);
        instance = kudzuFactories[0];
        instance.SpawnPools();
    }

    /// <summary>
    /// Returns a fresh Kudzu prefab from the object pool.
    /// </summary>
    /// <returns>a GameObject with a Kudzu component attached to it.</returns>
    public static GameObject GetKudzuPrefab() { return instance.RequestObject(ModelType.KUDZU); }

    /// <summary>
    /// Accepts a Kudzu prefab that the caller no longer needs. Adds it back
    /// to the object pool.
    /// </summary>
    /// <param name="prefab">The Kudzu prefab to return.</param>
    public static void ReturnKudzuPrefab(GameObject prefab)
    {
        Assert.IsTrue(prefab.GetComponent<Kudzu>() != null);
        instance.ReturnObject(prefab);
    }

    /// <summary>
    /// Returns the animation track that represents the Kudzu moving.
    /// </summary>
    /// <returns>the animation track that represents the Kudzu moving.</returns>
    /// <param name="d">The Enemy's Direction. </param>
    /// <param name="s">The Enemy's HealthState. </param>
    public static Sprite[] GetMovementTrack(Direction d, Enemy.EnemyHealthState s)
    {
        switch (d)
        {
            case Direction.NORTH:
                if (s == Enemy.EnemyHealthState.HEALTHY) return instance.healthyMovementAnimationNorth;
                if (s == Enemy.EnemyHealthState.DAMAGED) return instance.damagedMovementAnimationNorth;
                else return instance.criticalMovementAnimationNorth;
            case Direction.EAST:
                if (s == Enemy.EnemyHealthState.HEALTHY) return instance.healthyMovementAnimationEast;
                if (s == Enemy.EnemyHealthState.DAMAGED) return instance.damagedMovementAnimationEast;
                else return instance.criticalMovementAnimationEast;
            case Direction.SOUTH:
                if (s == Enemy.EnemyHealthState.HEALTHY) return instance.healthyMovementAnimationSouth;
                if (s == Enemy.EnemyHealthState.DAMAGED) return instance.damagedMovementAnimationSouth;
                else return instance.criticalMovementAnimationSouth;
            case Direction.WEST:
                if (s == Enemy.EnemyHealthState.HEALTHY) return instance.healthyMovementAnimationWest;
                if (s == Enemy.EnemyHealthState.DAMAGED) return instance.damagedMovementAnimationWest;
                else return instance.criticalMovementAnimationWest;
        }

        throw new System.InvalidOperationException("Invalid direction " + d + " or enemy health state " + s);
    }

    /// <summary>
    /// Returns the animation track that represents the Kudzu attacking.
    /// </summary>
    /// <returns>the animation track that represents the Kudzu attacking.</returns>
    /// <param name="d">The Enemy's Direction. </param>
    /// <param name="s">The Enemy's HealthState. </param>
    public static Sprite[] GetAttackTrack(Direction d, Enemy.EnemyHealthState s)
    {
        switch (d)
        {
            case Direction.NORTH:
                if (s == Enemy.EnemyHealthState.HEALTHY) return instance.healthyAttackAnimationNorth;
                if (s == Enemy.EnemyHealthState.DAMAGED) return instance.damagedAttackAnimationNorth;
                else return instance.criticalAttackAnimationNorth;
            case Direction.EAST:
                if (s == Enemy.EnemyHealthState.HEALTHY) return instance.healthyAttackAnimationEast;
                if (s == Enemy.EnemyHealthState.DAMAGED) return instance.damagedAttackAnimationEast;
                else return instance.criticalAttackAnimationEast;
            case Direction.SOUTH:
                if (s == Enemy.EnemyHealthState.HEALTHY) return instance.healthyAttackAnimationSouth;
                if (s == Enemy.EnemyHealthState.DAMAGED) return instance.damagedAttackAnimationSouth;
                else return instance.criticalAttackAnimationSouth;
            case Direction.WEST:
                if (s == Enemy.EnemyHealthState.HEALTHY) return instance.healthyAttackAnimationWest;
                if (s == Enemy.EnemyHealthState.DAMAGED) return instance.damagedAttackAnimationWest;
                else return instance.criticalAttackAnimationWest;
        }

        throw new System.InvalidOperationException("Invalid direction or enemy health state.");
    }

    /// <summary>
    /// Returns the animation track that represents the Kudzu when idle. The Kudzu's
    /// idle animation is the same as its movement animation, so this calls the existing
    /// get movement animation method.
    /// </summary>
    /// <returns>the animation track that represents the Kudzu when idle.</returns>
    /// <param name="d">The Enemy's Direction. </param>
    /// <param name="s">The Enemy's HealthState. </param>
    public static Sprite[] GetIdleTrack(Direction d, Enemy.EnemyHealthState s)
    {
        return GetMovementTrack(d, s);
    }

    /// <summary>
    /// Returns the animation track that represents the Kudzu when Spawning. The Kudzu's
    /// Spawn animation is the same as its movement animation, so this calls the existing
    /// get movement animation method.
    /// </summary>
    /// <returns>the animation track that represents the Kudzu when spawned.</returns>
    /// <param name="d">The Enemy's Direction. </param>
    /// <param name="s">The Enemy's HealthState. </param>
    public static Sprite[] GetSpawnTrack(Direction d, Enemy.EnemyHealthState s)
    {
        return GetMovementTrack(d, s);
    }

    /// <summary>
    /// Returns the animation track that represents the Kudzu when Escaping. The Kudzu's
    /// Escape animation is the same as its movement animation, so this calls the existing
    /// get movement animation method.
    /// </summary>
    /// <returns>the animation track that represents the Kudzu when escaping.</returns>
    /// <param name="d">The Enemy's Direction. </param>
    /// <param name="s">The Enemy's HealthState. </param>
    public static Sprite[] GetEscapeTrack(Direction d, Enemy.EnemyHealthState s)
    {
        return GetMovementTrack(d, s);
    }

    /// <summary>
    /// Returns the animation track that represents the Kudzu when Exiting. The Kudzu's
    /// exiting animation is the same as its movement animation, so this calls the existing
    /// get movement animation method.
    /// </summary>
    /// <returns>the animation track that represents the Kudzu when exiting.</returns>
    /// <param name="d">The Enemy's Direction. </param>
    /// <param name="s">The Enemy's HealthState. </param>
    public static Sprite[] GetExitingTrack(Direction d, Enemy.EnemyHealthState s)
    {
        return GetMovementTrack(d, s);
    }

    /// <summary>
    /// Returns the animation track that represents this Kudzu when placing. 
    /// </summary>
    /// <returns>the animation track that represents this Kudzu when placing. 
    /// </returns>
    public static Sprite[] GetPlacementTrack() { return instance.placementTrack; }

    /// <summary>
    /// Returns the animation track that represents this Kudzu when on a boat. 
    /// </summary>
    /// <returns>the animation track that represents this Kudzu when on a boat. 
    /// </returns>
    public static Sprite[] GetBoatTrack() { return instance.boatTrack; }

    /// <summary>
    /// Returns the KudzuFactory instance's Transform compmonent. 
    /// </summary>
    /// <returns> the KudzuFactory instance's Transform compmonent. </returns>
    protected override Transform GetTransform() { return instance.transform; }
}
