using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Produces assets related to Bombs.
/// </summary>
public class BombFactory : MonoBehaviour
{
    /// <summary>
    /// Reference to the BombFactory singleton.
    /// </summary>
    private static BombFactory instance;

    /// <summary>
    /// GameObject with Bomb component attached. 
    /// </summary>
    [SerializeField]
    private GameObject bombPrefab;

    /// <summary>
    /// Bomb's animation track for mid-air movement. 
    /// </summary>
    [SerializeField]
    private Sprite[] midAirMovementTrack;

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

        BombFactory[] bombFactories = FindObjectsOfType<BombFactory>();
        Assert.IsNotNull(bombFactories, "Array of TileFactories is null.");
        Assert.AreEqual(1, bombFactories.Length);
        instance = bombFactories[0];
    }

    /// <summary>
    /// Returns an original, non-copied GameObject with a Bomb component
    /// attached to it.
    /// </summary>
    /// <returns>an original, non-copied GameObject with a Bomb component
    /// attached to it.</returns>
    public static GameObject GetBombPrefab() { return instance.bombPrefab; }

    /// <summary>
    /// Returns the animation track that represents the Bomb in mid-air.
    /// </summary>
    /// <returns>the animation track that represents the Bomb in mid-air.</returns>
    public static Sprite[] GetMidAirMovementTrack() { return instance.midAirMovementTrack; }

    /// <summary>
    /// Returns the animation track that represents this Bomb when placing. 
    /// </summary>
    /// <returns>the animation track that represents this Bomb when placing. 
    /// </returns>
    public static Sprite[] GetPlacementTrack() { return instance.placementTrack; }

    /// <summary>
    /// Returns the animation track that represents this Bomb when on a boat. 
    /// </summary>
    /// <returns>the animation track that represents this Bomb when on a boat. 
    /// </returns>
    public static Sprite[] GetBoatTrack() { return instance.boatTrack; }
}
