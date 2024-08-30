using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Produces assets related to Enemies. 
/// </summary>
public class EnemyFactory : Factory
{
    #region Fields

    /// <summary>
    /// Reference to the EnemyFactory singleton.
    /// </summary>
    private static EnemyFactory instance;

    /// <summary>
    /// Animation set for a Knotwood
    /// </summary>
    [SerializeField]
    private EnemyAnimationSet knotwoodAnimationSet;

    /// <summary>
    /// Animation set for a Kudzu
    /// </summary>
    [SerializeField]
    private EnemyAnimationSet kudzuAnimationSet;

    #endregion

    #region Methods

    /// <summary>
    /// Finds and sets the EnemyFactory singleton.
    /// </summary>
    /// <param name="levelController">The LevelController singleton.</param>
    public static void SetSingleton(LevelController levelController)
    {
        if (levelController == null) return;
        if (instance != null) return;

        EnemyFactory[] enemyFactories = FindObjectsOfType<EnemyFactory>();
        Assert.IsNotNull(enemyFactories, "Array of EnemyFactories is null.");
        Assert.AreEqual(1, enemyFactories.Length);
        instance = enemyFactories[0];
        instance.SpawnPools();
    }

    /// <summary>
    /// Returns a prefab for a given Enemy type from the object pool.
    /// </summary>
    /// <param name="modelType">The ModelType of the Enemy to get</param>
    /// <returns>a prefab for a given Enemy type from the object pool.</returns>
    public static GameObject GetEnemyPrefab(ModelType modelType) => instance.RequestObject(modelType);

    /// <summary>
    /// Accepts an Enemy prefab and puts it back in the object pool.
    /// </summary>
    /// <param name="prefab">the prefab to accept. </param>
    public static void ReturnEnemyPrefab(GameObject prefab) => instance.ReturnObject(prefab);

    /// <summary>
    /// Returns the movement track for an Enemy of a given type facing
    /// a given direction. Supports health states.
    /// </summary>
    /// <param name="e">The type of enemy</param>
    /// <param name="d">The direction of the enemy</param>
    /// <param name="s">The health state of the enemy</param>
    /// <returns>the movement track for an Enemy of a given type</returns>
    public static Sprite[] GetMovementTrack(ModelType e, Direction d, Enemy.EnemyHealthState s)
    {
        switch (e)
        {
            case ModelType.KNOTWOOD:
                return instance.knotwoodAnimationSet.GetMovementAnimation(d, s);
            case ModelType.KUDZU:
                return instance.kudzuAnimationSet.GetMovementAnimation(d, s);
        }

        throw new System.Exception("Enemy " + e + " not supported.");
    }

    /// <summary>
    /// Returns the attack track for an Enemy of a given type facing
    /// a given direction. Supports health states.
    /// </summary>
    /// <param name="e">The type of enemy</param>
    /// <param name="d">The direction of the enemy</param>
    /// <param name="s">The health state of the enemy</param>
    /// <returns>the attack track for an Enemy of a given type</returns>
    public static Sprite[] GetAttackTrack(ModelType e, Direction d, Enemy.EnemyHealthState s)
    {
        switch (e)
        {
            case ModelType.KNOTWOOD:
                return instance.knotwoodAnimationSet.GetAttackAnimation(d, s);
            case ModelType.KUDZU:
                return instance.kudzuAnimationSet.GetAttackAnimation(d, s);
        }

        throw new System.Exception("Enemy " + e + " not supported.");
    }

    /// <summary>
    /// Returns the idle track for an Enemy of a given type facing
    /// a given direction. Supports health states.
    /// </summary>
    /// <param name="e">The type of enemy</param>
    /// <param name="d">The direction of the enemy</param>
    /// <param name="s">The health state of the enemy</param>
    /// <returns>the idle track for an Enemy of a given type</returns>
    public static Sprite[] GetIdleTrack(ModelType e, Direction d, Enemy.EnemyHealthState s) => GetMovementTrack(e, d, s);

    /// <summary>
    /// Returns the spawn track for an Enemy of a given type facing
    /// a given direction. Supports health states.
    /// </summary>
    /// <param name="e">The type of enemy</param>
    /// <param name="d">The direction of the enemy</param>
    /// <param name="s">The health state of the enemy</param>
    /// <returns>the spawn track for an Enemy of a given type</returns>
    public static Sprite[] GetSpawnTrack(ModelType e, Direction d, Enemy.EnemyHealthState s) => GetMovementTrack(e, d, s);

    /// <summary>
    /// Returns the placement animation track for an Enemy of a given type.
    /// </summary>
    /// <param name="e">The type of enemy </param>
    /// <returns>the placement animation track for an Enemy of a given type.</returns>
    public static Sprite[] GetPlacementTrack(ModelType e)
    {
        switch (e)
        {
            case ModelType.KNOTWOOD:
                return instance.knotwoodAnimationSet.GetPlacementAnimation();
            case ModelType.KUDZU:
                return instance.kudzuAnimationSet.GetPlacementAnimation();
        }

        throw new System.Exception("Enemy " + e + " not supported.");
    }

    /// <summary>
    /// Returns the EnemyFactory's transform component. 
    /// </summary>
    /// <returns></returns>
    protected override Transform GetTransform() => instance.transform;

    #endregion
}
