using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Produces assets related to Hedgehogs.
/// </summary>
public class HedgehogFactory : Factory
{
    /// <summary>
    /// Reference to the HedgehogFactory singleton.
    /// </summary>
    private static HedgehogFactory instance;

    /// <summary>
    /// Animation track when on a boat.
    /// </summary>
    [SerializeField]
    private Sprite[] boatTrack;

    /// <summary>
    /// Animation track when placing.
    /// </summary>
    [SerializeField]
    private Sprite[] placementTrack;

    /// <summary>
    /// Attack animation when this Hedgehog is facing north.
    /// </summary>
    [SerializeField]
    private Sprite[] attackAnimationNorth;

    /// <summary>
    /// Attack animation when this Hedgehog is facing north.
    /// </summary>
    [SerializeField]
    private Sprite[] attackAnimationEast;

    /// <summary>
    /// Attack animation when this Hedgehog is facing south.
    /// </summary>
    [SerializeField]
    private Sprite[] attackAnimationSouth;

    /// <summary>
    /// Attack animation when this Hedgehog is facing west.
    /// </summary>
    [SerializeField]
    private Sprite[] attackAnimationWest;

    /// <summary>
    /// Idle animation when this Hedgehog is facing north.
    /// </summary>
    [SerializeField]
    private Sprite[] idleAnimationNorth;

    /// <summary>
    /// Idle animation when this Hedgehog is facing east.
    /// </summary>
    [SerializeField]
    private Sprite[] idleAnimationEast;

    /// <summary>
    /// Idle animation when this Hedgehog is facing south.
    /// </summary>
    [SerializeField]
    private Sprite[] idleAnimationSouth;

    /// <summary>
    /// Idle animation when this Hedgehog is facing west.
    /// </summary>
    [SerializeField]
    private Sprite[] idleAnimationWest;


    /// <summary>
    /// Finds and sets the HedgehogFactory singleton.
    /// </summary>
    /// <param name="levelController">The LevelController singleton.</param>
    public static void SetSingleton(LevelController levelController)
    {
        if (levelController == null) return;
        if (instance != null) return;

        HedgehogFactory[] hedgehogFactories = FindObjectsOfType<HedgehogFactory>();
        Assert.IsNotNull(hedgehogFactories, "Array of HedgehogFactories is null.");
        Assert.AreEqual(1, hedgehogFactories.Length);
        instance = hedgehogFactories[0];
        instance.SpawnPools();
    }

    /// <summary>
    /// Returns a fresh Hedgehog prefab from the object pool.
    /// </summary>
    /// <returns>a GameObject with an attached Hedgehog component.</returns>
    public static GameObject GetHedgehogPrefab()
    {
        GameObject hedgehogPrefab = instance.RequestObject(ModelType.HEDGEHOG);
        Debug.Log("gave out " + hedgehogPrefab.GetInstanceID());
        return instance.RequestObject(ModelType.HEDGEHOG);
    }

    /// <summary>
    /// Accepts a Hedgehog prefab that the caller no longer needs. Adds it back
    /// to the object pool.
    /// </summary>
    /// <param name="prefab">The Hedgehog prefab to return.</param>
    public static void ReturnHedgehogPrefab(GameObject prefab)
    {
        Assert.IsTrue(prefab.GetComponent<Hedgehog>() != null);
        Debug.Log("taking in " + prefab.GetInstanceID());
        instance.ReturnObject(prefab);
    }

    /// <summary>
    /// Returns the animation track that represents the Hedgehog attacking.
    /// </summary>
    /// <returns>the animation track that represents the Hedgehog attacking.</returns>
    /// <param name="d">The Hedgehog's Direction. </param>
    public static Sprite[] GetAttackTrack(Direction d)
    {
        switch (d)
        {
            case Direction.NORTH:
                return instance.attackAnimationNorth;
            case Direction.EAST:
                return instance.attackAnimationEast;
            case Direction.SOUTH:
                return instance.attackAnimationSouth;
            case Direction.WEST:
                return instance.attackAnimationWest;
        }

        throw new System.InvalidOperationException("Invalid direction.");
    }

    /// <summary>
    /// Returns the animation track that represents the Hedgehog when idle.
    /// </summary>
    /// <returns>the animation track that represents the Hedgehog when idle.</returns>
    /// <param name="d">The Hedgehog's Direction. </param>
    public static Sprite[] GetIdleTrack(Direction d)
    {
        switch (d)
        {
            case Direction.NORTH:
                return instance.idleAnimationNorth;
            case Direction.EAST:
                return instance.idleAnimationEast;
            case Direction.SOUTH:
                return instance.idleAnimationSouth;
            case Direction.WEST:
                return instance.idleAnimationWest;
        }

        throw new System.InvalidOperationException("Invalid direction.");
    }

    /// <summary>
    /// Returns the animation track that represents this Hedgehog when placing. 
    /// </summary>
    /// <returns>the animation track that represents this Hedgehog when placing. 
    /// </returns>
    public static Sprite[] GetPlacementTrack() { return instance.placementTrack; }

    /// <summary>
    /// Returns the animation track that represents this Hedgehog on a boat. 
    /// </summary>
    /// <returns>the animation track that represents this Hedgehog on a boat. 
    /// </returns>
    public static Sprite[] GetBoatTrack() { return instance.boatTrack; }

    /// <summary>
    /// Returns the HedgehogFactory instance's Transform component. 
    /// </summary>
    /// <returns>the HedgehogFactory instance's Transform component. </returns>
    protected override Transform GetTransform() { return instance.transform; }
}
