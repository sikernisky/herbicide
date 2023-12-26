using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Produces assets related to Nexii. 
/// </summary>
public class NexusFactory : MonoBehaviour
{
    /// <summary>
    /// Reference to the NexusFactory singleton.
    /// </summary>
    private static NexusFactory instance;

    /// <summary>
    /// GameObject with Nexus component attached. 
    /// </summary>
    [SerializeField]
    private GameObject nexusPrefab;

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
    }

    /// <summary>
    /// Returns an original, non-copied GameObject with a Nexus component
    /// attached to it.
    /// </summary>
    /// <returns>an original, non-copied GameObject with a Nexus component
    /// attached to it.</returns>
    public static GameObject GetNexusPrefab() { return instance.nexusPrefab; }

    /// <summary>
    /// Returns the animation track that represents this Nexus when placing. 
    /// </summary>
    /// <returns>the animation track that represents this Nexus when placing. 
    /// </returns>
    public static Sprite[] GetPlacementTrack() { return instance.placementTrack; }

}
