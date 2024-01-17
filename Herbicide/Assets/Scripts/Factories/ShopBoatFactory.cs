using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Produces assets related to ShopBoats.
/// </summary>
public class ShopBoatFactory : MonoBehaviour
{
    /// <summary>
    /// Reference to the ShopBoatFactory singleton.
    /// </summary>
    private static ShopBoatFactory instance;

    /// <summary>
    /// GameObject with ShopBoat component attached. 
    /// </summary>
    [SerializeField]
    private GameObject shopBoatPrefab;

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
    /// Animation track when cruising.
    /// </summary>
    [SerializeField]
    private Sprite[] movementTrack;


    /// <summary>
    /// Finds and sets the ShopBoatFactory singleton.
    /// </summary>
    /// <param name="levelController">The LevelController singleton.</param>
    public static void SetSingleton(LevelController levelController)
    {
        if (levelController == null) return;
        if (instance != null) return;

        ShopBoatFactory[] nexusHoleFactories = FindObjectsOfType<ShopBoatFactory>();
        Assert.IsNotNull(nexusHoleFactories, "Array of TileFactories is null.");
        Assert.AreEqual(1, nexusHoleFactories.Length);
        instance = nexusHoleFactories[0];
    }

    /// <summary>
    /// Returns an original, non-copied GameObject with a ShopBoat component
    /// attached to it.
    /// </summary>
    /// <returns>an original, non-copied GameObject with a ShopBoat component
    /// attached to it.</returns>
    public static GameObject GetShopBoatPrefab() { return instance.shopBoatPrefab; }

    /// <summary>
    /// Returns the animation track that represents this ShopBoat when placing. 
    /// </summary>
    /// <returns>the animation track that represents this ShopBoat when placing. 
    /// </returns>
    public static Sprite[] GetPlacementTrack() { return instance.placementTrack; }

    /// <summary>
    /// Returns the animation track that represents this ShopBoat on a boat. 
    /// </summary>
    /// <returns>the animation track that represents this ShopBoat on a boat. 
    /// </returns>
    public static Sprite[] GetBoatTrack() { return instance.boatTrack; }

    /// <summary>
    /// Returns the animation track that represents this ShopBoat when moving. 
    /// </summary>
    /// <returns>the animation track that represents this ShopBoat when moving. 
    /// </returns>
    public static Sprite[] GetMovementTrack() { return instance.movementTrack; }
}
