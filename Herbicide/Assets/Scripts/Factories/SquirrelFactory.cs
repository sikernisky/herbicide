using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Produces assets related to Squirrels.
/// </summary>
public class SquirrelFactory : Factory
{
    /// <summary>
    /// Reference to the SquirrelFactory singleton.
    /// </summary>
    private static SquirrelFactory instance;

    /// <summary>
    /// Sprite tracks for the tier one Squirrel.
    /// </summary>
    [SerializeField]
    private DefenderAnimationSet tierOneSquirrel;

    /// <summary>
    /// Sprite tracks for the tier two Squirrel.
    /// </summary>
    [SerializeField]
    private DefenderAnimationSet tierTwoSquirrel;

    /// <summary>
    /// Sprite tracks for the tier three Squirrel.
    /// </summary>
    [SerializeField]
    private DefenderAnimationSet tierThreeSquirrel;


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
        instance.SpawnPools();
    }

    /// <summary>
    /// Returns a fresh Squirrel prefab from the object pool.
    /// </summary>
    /// <returns>a GameObject with a Squirrel component attached to it</returns>
    public static GameObject GetSquirrelPrefab() { return instance.RequestObject(ModelType.SQUIRREL); }

    /// <summary>
    /// Accepts a Squirrel prefab that the caller no longer needs. Adds it back
    /// to the object pool.
    /// </summary>
    /// <param name="prefab">The Squirrel prefab to return.</param>
    public static void ReturnSquirrelPrefab(GameObject prefab)
    {
        Assert.IsTrue(prefab.GetComponent<Squirrel>() != null);
        instance.ReturnObject(prefab);
    }

    /// <summary>
    /// Returns the animation track that represents the Squirrel attacking.
    /// </summary>
    /// <returns>the animation track that represents the Squirrel attacking.</returns>
    /// <param name="d">The Squirrel's Direction. </param>
    /// <param name="tier">The Squirrel's tier. </param>
    public static Sprite[] GetAttackTrack(Direction d, int tier)
    {
        Assert.IsTrue(tier >= Defender.MIN_TIER && tier <= Defender.MAX_TIER, "Invalid tier.");

        if (tier == 1) return instance.tierOneSquirrel.GetAttackAnimation(d);
        else if (tier == 2) return instance.tierTwoSquirrel.GetAttackAnimation(d);
        else return instance.tierThreeSquirrel.GetAttackAnimation(d);
    }

    /// <summary>
    /// Returns the animation track that represents the Squirrel when idle.
    /// </summary>
    /// <returns>the animation track that represents the Squirrel when idle.</returns>
    /// <param name="d">The Squirrel's Direction. </param>
    /// <param name="tier">The Squirrel's tier. </param>
    public static Sprite[] GetIdleTrack(Direction d, int tier)
    {
        Assert.IsTrue(tier >= Defender.MIN_TIER && tier <= Defender.MAX_TIER, "Invalid tier.");

        if (tier == 1) return instance.tierOneSquirrel.GetIdleAnimation(d);
        else if (tier == 2) return instance.tierTwoSquirrel.GetIdleAnimation(d);
        else return instance.tierThreeSquirrel.GetIdleAnimation(d);
    }

    /// <summary>
    /// Returns the animation track that represents this Squirrel when placing. 
    /// </summary>
    /// <returns>the animation track that represents this Squirrel when placing. 
    /// </returns>
    /// <param name="tier">The Squirrel's tier. </param>
    public static Sprite[] GetPlacementTrack(int tier)
    {
        Assert.IsTrue(tier >= Defender.MIN_TIER && tier <= Defender.MAX_TIER, "Invalid tier.");

        if (tier == 1) return instance.tierOneSquirrel.GetPlacementAnimation();
        else if (tier == 2) return instance.tierTwoSquirrel.GetPlacementAnimation();
        else return instance.tierThreeSquirrel.GetPlacementAnimation();
    }

    /// <summary>
    /// Returns the SquirrelFactory instance's Transform component.
    /// </summary>
    /// <returns>the SquirrelFactory instance's Transform component.</returns>
    protected override Transform GetTransform() { return instance.transform; }
}
