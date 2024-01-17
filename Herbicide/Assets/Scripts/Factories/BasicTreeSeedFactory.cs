using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Produces assets related to BasicTreeSeeds.
/// </summary>
public class BasicTreeSeedFactory : MonoBehaviour
{
    /// <summary>
    /// Reference to the BasicTreeSeedFactory singleton.
    /// </summary>
    private static BasicTreeSeedFactory instance;

    /// <summary>
    /// GameObject with BasicTreeSeed component attached. 
    /// </summary>
    [SerializeField]
    private GameObject basicTreeSeedPrefab;

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
    }

    /// <summary>
    /// Returns an original, non-copied GameObject with a BasicTreeSeed component
    /// attached to it.
    /// </summary>
    /// <returns>an original, non-copied GameObject with a BasicTreeSeed component
    /// attached to it.</returns>
    public static GameObject GetBasicTreeSeedPrefab() { return instance.basicTreeSeedPrefab; }

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
}
