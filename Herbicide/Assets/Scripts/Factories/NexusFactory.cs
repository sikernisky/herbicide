using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Produces assets related to Nexii. 
/// </summary>
public class NexusFactory : Factory
{
    /// <summary>
    /// Reference to the NexusFactory singleton.
    /// </summary>
    private static NexusFactory instance;

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
    /// Finds and sets the NexusFactory singleton.
    /// </summary>
    /// <param name="levelController">The LevelController singleton.</param>
    public static void SetSingleton(LevelController levelController)
    {
        if (levelController == null) return;
        if (instance != null) return;

        NexusFactory[] nexusFactories = FindObjectsOfType<NexusFactory>();
        Assert.IsNotNull(nexusFactories, "Array of TileFactories is null.");
        Assert.AreEqual(1, nexusFactories.Length);
        instance = nexusFactories[0];
        instance.SpawnPools();
    }

    /// <summary>
    /// Returns a fresh Nexus prefab from the object pool.
    /// </summary>
    /// <returns>a GameObject with a Nexus component attached to it</returns>
    public static GameObject GetNexusPrefab() { return instance.RequestObject(ModelType.NEXUS); }

    /// <summary>
    /// Accepts a Nexus prefab that the caller no longer needs. Adds it back
    /// to the object pool.
    /// </summary>
    /// <param name="prefab">The Nexus prefab to return.</param>
    public static void ReturnNexusPrefab(GameObject prefab)
    {
        Assert.IsTrue(prefab.GetComponent<Nexus>() != null);
        instance.ReturnObject(prefab);
    }

    /// <summary>
    /// Returns the animation track that represents this Nexus when placing. 
    /// </summary>
    /// <returns>the animation track that represents this Nexus when placing. 
    /// </returns>
    public static Sprite[] GetPlacementTrack() { return instance.placementTrack; }

    /// <summary>
    /// Returns the Transform component of the NexusFactory instance.
    /// </summary>
    /// <returns>the Transform component of the NexusFactory instance.</returns>
    protected override Transform GetTransform() { return instance.transform; }
}
