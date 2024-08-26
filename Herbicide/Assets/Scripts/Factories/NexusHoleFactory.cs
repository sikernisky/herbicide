using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Produces assets related to NexusHoles. 
/// </summary>
public class NexusHoleFactory : Factory
{
    #region Fields

    /// <summary>
    /// Reference to the NexusHoleFactory singleton.
    /// </summary>
    private static NexusHoleFactory instance;

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

    #endregion

    #region Methods

    /// <summary>
    /// Finds and sets the NexusHoleFactory singleton.
    /// </summary>
    /// <param name="levelController">The LevelController singleton.</param>
    public static void SetSingleton(LevelController levelController)
    {
        if (levelController == null) return;
        if (instance != null) return;

        NexusHoleFactory[] nexusHoleFactories = FindObjectsOfType<NexusHoleFactory>();
        Assert.IsNotNull(nexusHoleFactories, "Array of TileFactories is null.");
        Assert.AreEqual(1, nexusHoleFactories.Length);
        instance = nexusHoleFactories[0];
        instance.SpawnPools();
    }

    /// <summary>
    /// Returns a fresh NexusHole prefab from the object pool.
    /// </summary>
    /// <returns>a GameObject with a NexusHole component attached to it</returns>
    public static GameObject GetNexusHolePrefab() { return instance.RequestObject(ModelType.NEXUS_HOLE); }

    /// <summary>
    /// Accepts a NexusHole prefab that the caller no longer needs. Adds it back
    /// to the object pool.
    /// </summary>
    /// <param name="prefab">The NexusHole prefab to return.</param>
    public static void ReturnNexusHolePrefab(GameObject prefab)
    {
        Assert.IsTrue(prefab.GetComponent<NexusHole>() != null);
        instance.ReturnObject(prefab);
    }

    /// <summary>
    /// Returns the animation track that represents this NexusHole when placing. 
    /// </summary>
    /// <returns>the animation track that represents this NexusHole when placing. 
    /// </returns>
    public static Sprite[] GetPlacementTrack() { return instance.placementTrack; }

    /// <summary>
    /// Returns the animation track that represents this NexusHole on a boat. 
    /// </summary>
    /// <returns>the animation track that represents this NexusHole on a boat. 
    /// </returns>
    public static Sprite[] GetBoatTrack() { return instance.boatTrack; }

    /// <summary>
    /// Returns the Transform component of the NexusHoleFactory instance.
    /// </summary>
    /// <returns>the Transform component of the NexusHoleFactory instance.</returns>
    protected override Transform GetTransform() { return instance.transform; }

    #endregion
}