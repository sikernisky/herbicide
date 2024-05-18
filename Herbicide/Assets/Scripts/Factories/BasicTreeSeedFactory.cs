using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Produces assets related to BasicTreeSeeds.
/// </summary>
public class BasicTreeSeedFactory : Factory
{
    /// <summary>
    /// Reference to the BasicTreeSeedFactory singleton.
    /// </summary>
    private static BasicTreeSeedFactory instance;

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
    /// Finds and sets the BasicTreeSeedFactory singleton.
    /// </summary>
    /// <param name="levelController">The LevelController singleton.</param>
    public static void SetSingleton(LevelController levelController)
    {
        if (levelController == null) return;
        if (instance != null) return;

        BasicTreeSeedFactory[] basicTreeSeedFactories = FindObjectsOfType<BasicTreeSeedFactory>();
        Assert.IsNotNull(basicTreeSeedFactories, "Array of TileFactories is null.");
        Assert.AreEqual(1, basicTreeSeedFactories.Length);
        instance = basicTreeSeedFactories[0];
        instance.SpawnPools();
    }

    /// <summary>
    /// Returns a fresh BasicTreeSeed prefab from the object pool.
    /// </summary>
    /// <returns>a GameObject with a BasicTreeSeed component attached to it</returns>
    public static GameObject GetBasicTreeSeedPrefab()
    {
        return instance.RequestObject(ModelType.BASIC_TREE_SEED);
    }

    /// <summary>
    /// Accepts a BasicTreeSeed prefab that the caller no longer needs. Adds it back
    /// to the object pool.
    /// </summary>
    /// <param name="prefab">The BasicTreeSeed prefab to return.</param>
    public static void ReturnBasicTreeSeedPrefab(GameObject prefab)
    {
        Assert.IsNotNull(prefab);
        Assert.IsTrue(prefab.GetComponent<BasicTreeSeed>() != null);
        instance.ReturnObject(prefab);
    }

    /// <summary>
    /// Returns the animation track that represents this BasicTreeSeed when placing. 
    /// </summary>
    /// <returns>the animation track that represents this BasicTreeSeed when placing. 
    /// </returns>
    public static Sprite[] GetPlacementTrack() { return instance.placementTrack; }

    /// <summary>
    /// Returns the animation track that represents this BasicTreeSeed when on a boat. 
    /// </summary>
    /// <returns>the animation track that represents this BasicTreeSeed when on a boat. 
    /// </returns>
    public static Sprite[] GetBoatTrack() { return instance.boatTrack; }

    /// <summary>
    /// Returns the Transform component of the BasicTreeSeedFactory instance.
    /// </summary>
    /// <returns>the Transform component of the BasicTreeSeedFactory instance.</returns>
    protected override Transform GetTransform() { return instance.transform; }
}
