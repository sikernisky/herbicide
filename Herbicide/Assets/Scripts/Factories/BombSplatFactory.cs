using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Produces assets related to BombSplats.
/// </summary>
public class BombSplatFactory : Factory
{
    /// <summary>
    /// Reference to the BombSplatFactory singleton.
    /// </summary>
    private static BombSplatFactory instance;

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
    /// Finds and sets the BombFactory singleton.
    /// </summary>
    /// <param name="levelController">The LevelController singleton.</param>
    public static void SetSingleton(LevelController levelController)
    {
        if (levelController == null) return;
        if (instance != null) return;

        BombSplatFactory[] bombSplatFactories = FindObjectsOfType<BombSplatFactory>();
        Assert.IsNotNull(bombSplatFactories, "Array of TileFactories is null.");
        Assert.AreEqual(1, bombSplatFactories.Length);
        instance = bombSplatFactories[0];
        instance.SpawnPools();
    }

    /// <summary>
    /// Returns a fresh BombSplat prefab from the object pool.
    /// </summary>
    /// <returns>a GameObject with a BombSplat component attached to it</returns>
    public static GameObject GetBombSplatPrefab() { return instance.RequestObject(ModelType.BOMB_SPLAT); }

    /// <summary>
    /// Accepts a BombSplat prefab that the caller no longer needs. Adds it back
    /// to the object pool.
    /// </summary>
    /// <param name="prefab">The BombSplat prefab to return.</param>
    public static void ReturnBombSplatPrefab(GameObject prefab)
    {
        Assert.IsNotNull(prefab);
        Assert.IsTrue(prefab.GetComponent<BombSplat>() != null);
        instance.ReturnObject(prefab);
    }

    /// <summary>
    /// Returns the animation track that represents this BombSplat when placing. 
    /// </summary>
    /// <returns>the animation track that represents this BombSplat when placing. 
    /// </returns>
    public static Sprite[] GetPlacementTrack() { return instance.placementTrack; }

    /// <summary>
    /// Returns the animation track that represents this BombSplat when on a boat. 
    /// </summary>
    /// <returns>the animation track that represents this BombSplat when on a boat. 
    /// </returns>
    public static Sprite[] GetBoatTrack() { return instance.boatTrack; }

    /// <summary>
    /// Returns the Transform component of the BombSplatFactory instance.
    /// </summary>
    /// <returns>the Transform component of the BombSplatFactory instance.</returns>
    protected override Transform GetTransform() { return instance.transform; }
}
