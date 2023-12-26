using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Produces assets related to Butterflies.
/// </summary>
public class ButterflyFactory : MonoBehaviour
{
    /// <summary>
    /// Reference to the ButterflyFactory singleton.
    /// </summary>
    private static ButterflyFactory instance;

    /// <summary>
    /// GameObject with Butterfly component attached. 
    /// </summary>
    [SerializeField]
    private GameObject butterflyPrefab;

    /// <summary>
    /// Butterfly's animation track for movement. 
    /// </summary>
    [SerializeField]
    private Sprite[] movementTrack;

    /// <summary>
    /// Butterfly's animation track for attacking. 
    /// </summary>
    [SerializeField]
    private Sprite[] attackTrack;

    /// <summary>
    /// Butterfly's animation track when idle. 
    /// </summary>
    [SerializeField]
    private Sprite[] idleTrack;

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
    /// Finds and sets the ButterflyFactory singleton.
    /// </summary>
    /// <param name="levelController">The LevelController singleton.</param>
    public static void SetSingleton(LevelController levelController)
    {
        if (levelController == null) return;
        if (instance != null) return;

        ButterflyFactory[] butterflyFactories = FindObjectsOfType<ButterflyFactory>();
        Assert.IsNotNull(butterflyFactories, "Array of TileFactories is null.");
        Assert.AreEqual(1, butterflyFactories.Length);
        instance = butterflyFactories[0];
    }

    /// <summary>
    /// Returns an original, non-copied GameObject with a Butterfly component
    /// attached to it.
    /// </summary>
    /// <returns>an original, non-copied GameObject with a Butterfly component
    /// attached to it.</returns>
    public static GameObject GetButterflyPrefab() { return instance.butterflyPrefab; }

    /// <summary>
    /// Returns the animation track that represents the Butterfly moving.
    /// </summary>
    /// <returns>the animation track that represents the Butterfly moving.</returns>
    public static Sprite[] GetMovementTrack() { return instance.movementTrack; }

    /// <summary>
    /// Returns the animation track that represents the Butterfly attacking.
    /// </summary>
    /// <returns>the animation track that represents the Butterfly attacking.</returns>
    public static Sprite[] GetAttackTrack() { return instance.attackTrack; }

    /// <summary>
    /// Returns the animation track that represents the Butterfly when idle.
    /// </summary>
    /// <returns>the animation track that represents the Butterfly when idle.</returns>
    public static Sprite[] GetIdleTrack() { return instance.idleTrack; }

    /// <summary>
    /// Returns the animation track that represents this Butterfly when placing. 
    /// </summary>
    /// <returns>the animation track that represents this Butterfly when placing. 
    /// </returns>
    public static Sprite[] GetPlacementTrack() { return instance.placementTrack; }

    /// <summary>
    /// Returns the animation track that represents this Butterfly on a boat.
    /// </summary>
    /// <returns>the animation track that represents this Butterfly on a boat.</returns>
    public static Sprite[] GetBoatTrack() { return instance.boatTrack; }
}
