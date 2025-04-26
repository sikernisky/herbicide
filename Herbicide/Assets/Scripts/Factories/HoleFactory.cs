using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Produces assets related to SpawnHoles and GoalHoles. 
/// </summary>
public class HoleFactory : Factory
{
    #region Fields

    /// <summary>
    /// Reference to the HoleFactory singleton.
    /// </summary>
    private static HoleFactory instance;

    /// <summary>
    /// Spawn hole animation track when placing.
    /// </summary>
    [SerializeField]
    private Sprite[] spawnHolePlacementTrack;

    /// <summary>
    /// Goal hole animation track when placing.
    /// </summary>
    [SerializeField]
    private Sprite[] goalHolePlacementTrack;

    #endregion

    #region Methods

    /// <summary>
    /// Finds and sets the HoleFactory singleton.
    /// </summary>
    /// <param name="levelController">The LevelController singleton.</param>
    public static void SetSingleton(LevelController levelController)
    {
        if (levelController == null) return;
        if (instance != null) return;

        HoleFactory[] HoleFactories = FindObjectsOfType<HoleFactory>();
        Assert.IsNotNull(HoleFactories, "Array of HoleFactories is null.");
        Assert.AreEqual(1, HoleFactories.Length);
        instance = HoleFactories[0];
        instance.SpawnPools();
    }

    /// <summary>
    /// Returns a fresh SpawnHole or GoalHole prefab from the object pool.
    /// </summary>
    /// <returns>a GameObject with a SpawnHole or GoalHole component attached to it</returns>
    public static GameObject GetHolePrefab(ModelType modelType) => instance.RequestObject(modelType);

    /// <summary>
    /// Accepts a SpawnHole or GoalHole prefab that the caller no longer needs. Adds it back
    /// to the object pool.
    /// </summary>
    /// <param name="prefab">The prefab to return.</param>
    public static void ReturnHolePrefab(GameObject prefab) => instance.ReturnObject(prefab);

    /// <summary>
    /// Returns the animation track that represents a SpawnHole when placing. 
    /// </summary>
    /// <returns>the animation track that represents a SpawnHole when placing. 
    /// </returns>
    public static Sprite[] GetSpawnHolePlacementTrack() => instance.spawnHolePlacementTrack;

    /// <summary>
    /// Returns the animation track that represents a GoalHole when placing. 
    /// </summary>
    /// <returns>the animation track that represents a GoalHole when placing. 
    /// </returns>
    public static Sprite[] GetGoalHolePlacementTrack() => instance.goalHolePlacementTrack;

    /// <summary>
    /// Returns the Transform component of the HoleFactory instance.
    /// </summary>
    /// <returns>the Transform component of the HoleFactory instance.</returns>
    protected override Transform GetTransform() => instance.transform;

    #endregion
}