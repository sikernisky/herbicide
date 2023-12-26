using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Produces assets related to BasicTrees.
/// </summary>
public class BasicTreeFactory : MonoBehaviour
{
    /// <summary>
    /// Reference to the BasicTreeFactory singleton.
    /// </summary>
    private static BasicTreeFactory instance;

    /// <summary>
    /// GameObject with BasicTree component attached. 
    /// </summary>
    [SerializeField]
    private GameObject basicTreePrefab;

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
    /// Finds and sets the BasicTreeFactory singleton.
    /// </summary>
    /// <param name="levelController">The LevelController singleton.</param>
    public static void SetSingleton(LevelController levelController)
    {
        if (levelController == null) return;
        if (instance != null) return;

        BasicTreeFactory[] basicTreeFactories = FindObjectsOfType<BasicTreeFactory>();
        Assert.IsNotNull(basicTreeFactories, "Array of TileFactories is null.");
        Assert.AreEqual(1, basicTreeFactories.Length);
        instance = basicTreeFactories[0];
    }

    /// <summary>
    /// Returns an original, non-copied GameObject with a BasicTree component
    /// attached to it.
    /// </summary>
    /// <returns>an original, non-copied GameObject with a BasicTree component
    /// attached to it.</returns>
    public static GameObject GetBasicTreePrefab() { return instance.basicTreePrefab; }

    /// <summary>
    /// Returns the animation track that represents this BasicTree when placing. 
    /// </summary>
    /// <returns>the animation track that represents this BasicTree when placing. 
    /// </returns>
    public static Sprite[] GetPlacementTrack() { return instance.placementTrack; }

    /// <summary>
    /// Returns the animation track that represents this BasicTree when on a boat. 
    /// </summary>
    /// <returns>the animation track that represents this BasicTree when on a boat. 
    /// </returns>
    public static Sprite[] GetBoatTrack() { return instance.boatTrack; }
}
