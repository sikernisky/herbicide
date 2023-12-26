using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Produces assets related to BombSplats.
/// </summary>
public class BombSplatFactory : MonoBehaviour
{
    /// <summary>
    /// Reference to the BombSplatFactory singleton.
    /// </summary>
    private static BombSplatFactory instance;

    /// <summary>
    /// GameObject with BombSplat component attached. 
    /// </summary>
    [SerializeField]
    private GameObject bombSplatPrefab;

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
    }

    /// <summary>
    /// Returns an original, non-copied GameObject with a BombSplat component
    /// attached to it.
    /// </summary>
    /// <returns>an original, non-copied GameObject with a BombSplat component
    /// attached to it.</returns>
    public static GameObject GetBombSplatPrefab() { return instance.bombSplatPrefab; }

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
}
