using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Produces assets related to Squirrels.
/// </summary>
public class SquirrelFactory : MonoBehaviour
{
    /// <summary>
    /// Reference to the SquirrelFactory singleton.
    /// </summary>
    private static SquirrelFactory instance;

    /// <summary>
    /// GameObject with Squirrel component attached. 
    /// </summary>
    [SerializeField]
    private GameObject squirrelPrefab;

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
    /// Attack animation when this Squirrel is facing north.
    /// </summary>
    [SerializeField]
    private Sprite[] attackAnimationNorth;

    /// <summary>
    /// Attack animation when this Squirrel is facing north.
    /// </summary>
    [SerializeField]
    private Sprite[] attackAnimationEast;

    /// <summary>
    /// Attack animation when this Squirrel is facing south.
    /// </summary>
    [SerializeField]
    private Sprite[] attackAnimationSouth;

    /// <summary>
    /// Attack animation when this Squirrel is facing west.
    /// </summary>
    [SerializeField]
    private Sprite[] attackAnimationWest;

    /// <summary>
    /// Idle animation when this Squirrel is facing north.
    /// </summary>
    [SerializeField]
    private Sprite[] idleAnimationNorth;

    /// <summary>
    /// Idle animation when this Squirrel is facing east.
    /// </summary>
    [SerializeField]
    private Sprite[] idleAnimationEast;

    /// <summary>
    /// Idle animation when this Squirrel is facing south.
    /// </summary>
    [SerializeField]
    private Sprite[] idleAnimationSouth;

    /// <summary>
    /// Idle animation when this Squirrel is facing west.
    /// </summary>
    [SerializeField]
    private Sprite[] idleAnimationWest;


    /// <summary>
    /// Finds and sets the SquirrelFactory singleton.
    /// </summary>
    /// <param name="levelController">The LevelController singleton.</param>
    public static void SetSingleton(LevelController levelController)
    {
        if (levelController == null) return;
        if (instance != null) return;

        SquirrelFactory[] squirrelFactories = FindObjectsOfType<SquirrelFactory>();
        Assert.IsNotNull(squirrelFactories, "Array of SquirrelFactories is null.");
        Assert.AreEqual(1, squirrelFactories.Length);
        instance = squirrelFactories[0];
    }

    /// <summary>
    /// Returns an original, non-copied GameObject with a Squirrel component
    /// attached to it.
    /// </summary>
    /// <returns>an original, non-copied GameObject with a Squirrel component
    /// attached to it.</returns>
    public static GameObject GetSquirrelPrefab() { return instance.squirrelPrefab; }

    /// <summary>
    /// Returns the animation track that represents the Squirrel attacking.
    /// </summary>
    /// <returns>the animation track that represents the Squirrel attacking.</returns>
    /// <param name="d">The Squirrel's Direction. </param>
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
    /// Returns the animation track that represents the Squirrel when idle.
    /// </summary>
    /// <returns>the animation track that represents the Squirrel when idle.</returns>
    /// <param name="d">The Squirrel's Direction. </param>
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
    /// Returns the animation track that represents this Squirrel when placing. 
    /// </summary>
    /// <returns>the animation track that represents this Squirrel when placing. 
    /// </returns>
    public static Sprite[] GetPlacementTrack() { return instance.placementTrack; }

    /// <summary>
    /// Returns the animation track that represents this Squirrel on a boat. 
    /// </summary>
    /// <returns>the animation track that represents this Squirrel on a boat. 
    /// </returns>
    public static Sprite[] GetBoatTrack() { return instance.boatTrack; }
}
