using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Produces assets related to Bears.
/// </summary>
public class BearFactory : Factory
{
    /// <summary>
    /// Reference to the BearFactory singleton.
    /// </summary>
    private static BearFactory instance;

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
    /// Attack animation when this Bear is facing north.
    /// </summary>
    [SerializeField]
    private Sprite[] attackAnimationNorth;

    /// <summary>
    /// Attack animation when this Bear is facing north.
    /// </summary>
    [SerializeField]
    private Sprite[] attackAnimationEast;

    /// <summary>
    /// Attack animation when this Bear is facing south.
    /// </summary>
    [SerializeField]
    private Sprite[] attackAnimationSouth;

    /// <summary>
    /// Attack animation when this Bear is facing west.
    /// </summary>
    [SerializeField]
    private Sprite[] attackAnimationWest;

    /// <summary>
    /// Idle animation when this Bear is facing north.
    /// </summary>
    [SerializeField]
    private Sprite[] idleAnimationNorth;

    /// <summary>
    /// Idle animation when this Bear is facing east.
    /// </summary>
    [SerializeField]
    private Sprite[] idleAnimationEast;

    /// <summary>
    /// Idle animation when this Bear is facing south.
    /// </summary>
    [SerializeField]
    private Sprite[] idleAnimationSouth;

    /// <summary>
    /// Idle animation when this Bear is facing west.
    /// </summary>
    [SerializeField]
    private Sprite[] idleAnimationWest;

    /// <summary>
    /// The animation created when the Bear attacks an enemy.
    /// </summary>
    [SerializeField]
    private Sprite[] biteTrack;


    /// <summary>
    /// Finds and sets the BearFactory singleton.
    /// </summary>
    /// <param name="levelController">The BearFactory singleton.</param>
    public static void SetSingleton(LevelController levelController)
    {
        if (levelController == null) return;
        if (instance != null) return;

        BearFactory[] bearFactories = FindObjectsOfType<BearFactory>();
        Assert.IsNotNull(bearFactories, "Array of TileFactories is null.");
        Assert.AreEqual(1, bearFactories.Length);
        instance = bearFactories[0];
        instance.SpawnPools();
    }

    /// <summary>
    /// Returns a fresh Bear prefab from the object pool.
    /// </summary>
    /// <returns>a GameObject with a Bear component attached to it</returns>
    public static GameObject GetBearPrefab() { return instance.RequestObject(ModelType.BEAR); }

    /// <summary>
    /// Accepts a Bear prefab that the caller no longer needs. Adds it back
    /// to the object pool.
    /// </summary>
    /// <param name="prefab">The Bear prefab to return.</param>
    public static void ReturnBearPrefab(GameObject prefab)
    {
        Assert.IsNotNull(prefab);
        Assert.IsTrue(prefab.GetComponent<Bear>() != null);
        instance.ReturnObject(prefab);
    }

    /// <summary>
    /// Returns the animation track that represents the Bear attacking.
    /// </summary>
    /// <returns>the animation track that represents the Bear attacking.</returns>
    /// <param name="d">The Bear's Direction. </param>
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
    /// Returns the animation track that represents the Bear when idle.
    /// </summary>
    /// <returns>the animation track that represents the Bear when idle.</returns>
    /// <param name="d">The Bear's Direction. </param>
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
    /// Returns the animation track that represents this Bear when placing. 
    /// </summary>
    /// <returns>the animation track that represents this Bear when placing. 
    /// </returns>
    public static Sprite[] GetPlacementTrack() { return instance.placementTrack; }

    /// <summary>
    /// Returns the animation track that represents this Bear on a boat. 
    /// </summary>
    /// <returns>the animation track that represents this Bear on a boat. 
    /// </returns>
    public static Sprite[] GetBoatTrack() { return instance.boatTrack; }

    /// <summary>
    /// Returns the animation track that represents the Bear's biting animation.
    /// </summary>
    /// <returns>the animation track that represents the Bear's biting animation.
    /// </returns>

    public static Sprite[] GetBiteTrack() { return instance.biteTrack; }

    /// <summary>
    /// Returns the Transform component of the BearFacotry instance.
    /// </summary>
    /// <returns>the Transform component of the BearFacotry instance</returns>
    protected override Transform GetTransform() { return instance.transform; }
}
