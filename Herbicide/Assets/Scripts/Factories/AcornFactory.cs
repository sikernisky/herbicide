using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Produces assets related to Acorns.
/// </summary>
public class AcornFactory : MonoBehaviour
{
    /// <summary>
    /// Reference to the AcornFactory singleton.
    /// </summary>
    private static AcornFactory instance;

    /// <summary>
    /// GameObject with Acorn component attached. 
    /// </summary>
    [SerializeField]
    private GameObject acornPrefab;

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
    /// Finds and sets the AcornFactory singleton.
    /// </summary>
    /// <param name="levelController">The LevelController singleton.</param>
    public static void SetSingleton(LevelController levelController)
    {
        if (levelController == null) return;
        if (instance != null) return;

        AcornFactory[] acornFactories = FindObjectsOfType<AcornFactory>();
        Assert.IsNotNull(acornFactories, "Array of TileFactories is null.");
        Assert.AreEqual(1, acornFactories.Length);
        instance = acornFactories[0];
    }

    /// <summary>
    /// Returns an original, non-copied GameObject with a Acorn component
    /// attached to it.
    /// </summary>
    /// <returns>an original, non-copied GameObject with a Acorn component
    /// attached to it.</returns>
    public static GameObject GetAcornPrefab() { return instance.acornPrefab; }

    /// <summary>
    /// Returns the animation track that represents this Acorn when placing. 
    /// </summary>
    /// <returns>the animation track that represents this Acorn when placing. 
    /// </returns>
    public static Sprite[] GetPlacementTrack() { return instance.placementTrack; }

    /// <summary>
    /// Returns the animation track that represents this Acorn when on a boat. 
    /// </summary>
    /// <returns>the animation track that represents this Acorn when on a boat. 
    /// </returns>
    public static Sprite[] GetBoatTrack() { return instance.boatTrack; }

}
