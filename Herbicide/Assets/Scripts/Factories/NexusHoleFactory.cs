using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class NexusHoleFactory : MonoBehaviour
{
    /// <summary>
    /// Reference to the NexusHoleFactory singleton.
    /// </summary>
    private static NexusHoleFactory instance;

    /// <summary>
    /// GameObject with NexusHole component attached. 
    /// </summary>
    [SerializeField]
    private GameObject nexusHolePrefab;

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
    }

    /// <summary>
    /// Returns an original, non-copied GameObject with a NexusHole component
    /// attached to it.
    /// </summary>
    /// <returns>an original, non-copied GameObject with a NexusHole component
    /// attached to it.</returns>
    public static GameObject GetNexusHolePrefab() { return instance.nexusHolePrefab; }

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
}