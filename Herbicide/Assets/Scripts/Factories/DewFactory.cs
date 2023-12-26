using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Produces assets related to Dew.
/// </summary>
public class DewFactory : MonoBehaviour
{
    /// <summary>
    /// Reference to the DewFactory singleton.
    /// </summary>
    private static DewFactory instance;

    /// <summary>
    /// GameObject with Dew component attached. 
    /// </summary>
    [SerializeField]
    private GameObject dewPrefab;

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
    }

    /// <summary>
    /// Returns an original, non-copied GameObject with a Dew component
    /// attached to it.
    /// </summary>
    /// <returns>an original, non-copied GameObject with a Dew component
    /// attached to it.</returns>
    public static GameObject GetDewPrefab() { return instance.dewPrefab; }

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
}
