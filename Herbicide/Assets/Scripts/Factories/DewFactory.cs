using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Produces assets related to Dew.
/// </summary>
public class DewFactory : Factory
{
    /// <summary>
    /// Reference to the DewFactory singleton.
    /// </summary>
    private static DewFactory instance;

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
    /// Finds and sets the DewFactory singleton.
    /// </summary>
    /// <param name="levelController">The LevelController singleton.</param>
    public static void SetSingleton(LevelController levelController)
    {
        if (levelController == null) return;
        if (instance != null) return;

        DewFactory[] dewFactories = FindObjectsOfType<DewFactory>();
        Assert.IsNotNull(dewFactories, "Array of TileFactories is null.");
        Assert.AreEqual(1, dewFactories.Length);
        instance = dewFactories[0];
        instance.SpawnPools();
    }

    /// <summary>
    /// Returns a fresh Dew prefab from the object pool.
    /// </summary>
    /// <returns>a GameObject with a Dew component attached to it</returns>
    public static GameObject GetDewPrefab() { return instance.RequestObject(ModelType.DEW); }

    /// <summary>
    /// Accepts a Dew prefab that the caller no longer needs. Adds it back
    /// to the object pool.
    /// </summary>
    /// <param name="prefab">The Dew prefab to return.</param>
    public static void ReturnDewPrefab(GameObject prefab)
    {
        Assert.IsNotNull(prefab);
        Assert.IsTrue(prefab.GetComponent<Dew>() != null);
        instance.ReturnObject(prefab);
    }

    /// <summary>
    /// Returns the animation track that represents this Dew when placing. 
    /// </summary>
    /// <returns>the animation track that represents this Dew when placing. 
    /// </returns>
    public static Sprite[] GetPlacementTrack() { return instance.placementTrack; }

    /// <summary>
    /// Returns the animation track that represents this Dew when on a boat. 
    /// </summary>
    /// <returns>the animation track that represents this Dew when on a boat. 
    /// </returns>
    public static Sprite[] GetBoatTrack() { return instance.boatTrack; }

    /// <summary>
    /// Returns the Transform component of the DewFactory instance. 
    /// </summary>
    /// <returns>the Transform component of the DewFactory instance. </returns>
    protected override Transform GetTransform() { return instance.transform; }
}
