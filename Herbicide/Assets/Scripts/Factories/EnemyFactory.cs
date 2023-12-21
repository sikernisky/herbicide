using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System;

/// <summary>
/// Manages assets for Enemies. 
/// </summary>
public class EnemyFactory : MonoBehaviour
{
    /// <summary>
    /// Reference to the EnemyFactory Singleton.
    /// </summary>
    private static EnemyFactory instance;

    /// <summary>
    /// All ScriptableObjects containing animation data about
    /// different enemies.
    /// </summary>
    [SerializeField]
    private List<EnemyScriptable> enemyScriptables;

    /// <summary>
    /// Finds and sets the EnemyFactory singleton.
    /// </summary>
    /// <param name="levelController">The LevelController singleton.</param>
    public static void SetSingleton(LevelController levelController)
    {
        if (levelController == null) return;

        EnemyFactory[] enemyFactories = FindObjectsOfType<EnemyFactory>();
        Assert.IsNotNull(enemyFactories, "Array of EnemyFactories is null.");
        Assert.AreEqual(1, enemyFactories.Length);
        instance = enemyFactories[0];
    }

    /// <summary>
    /// Returns a movement animation track for a given enemy type and health state.
    /// </summary>
    /// <param name="type">The enemy type.</param>
    /// <param name="state">The enemy health state.</param>
    /// <param name="direction">The direction of the enemy.</param>
    /// <returns>The movement animation track as a Sprite array or
    ///  null if the enemy type is not found.</returns>
    public static Sprite[] GetMovementTrack(Enemy.EnemyType type, Enemy.EnemyHealthState state, Direction direction)
    {
        EnemyScriptable data = instance.enemyScriptables.Find(x => x.GetEnemyType() == type);

        if (data != null)
        {
            switch (state)
            {
                case Enemy.EnemyHealthState.HEALTHY:
                    return data.GetHealthyMovementAnimation(direction);
                case Enemy.EnemyHealthState.DAMAGED:
                    return data.GetDamagedMovementAnimation(direction);
                case Enemy.EnemyHealthState.CRITICAL:
                    return data.GetCriticalMovementAnimation(direction);
            }
        }

        return null;
    }

    /// <summary>
    /// Returns an attack animation track for a given enemy type and health state.
    /// </summary>
    /// <param name="type">The enemy type.</param>
    /// <param name="state">The enemy health state.</param>
    /// <param name="direction">The direction of the enemy.</param>
    /// <returns>The attack animation track as a Sprite array or null 
    /// if the enemy type is not found.</returns>
    public static Sprite[] GetAttackTrack(Enemy.EnemyType type, Enemy.EnemyHealthState state, Direction direction)
    {
        EnemyScriptable data = instance.enemyScriptables.Find(x => x.GetEnemyType() == type);

        if (data != null)
        {
            switch (state)
            {
                case Enemy.EnemyHealthState.HEALTHY:
                    return data.GetHealthyAttackAnimation(direction);
                case Enemy.EnemyHealthState.DAMAGED:
                    return data.GetDamagedAttackAnimation(direction);
                case Enemy.EnemyHealthState.CRITICAL:
                    return data.GetCriticalAttackAnimation(direction);
            }
        }

        return null;
    }

    /// <summary>
    /// Returns an attack animation track for a given enemy type and health state.
    /// </summary>
    /// <param name="type">The enemy type.</param>
    /// <param name="state">The enemy health state.</param>
    /// <param name="direction">The direction of the enemy.</param>
    /// <returns>The attack animation track as a Sprite array or null 
    /// if the enemy type is not found.</returns>
    public static Sprite[] GetIdleTrack(Enemy.EnemyType type, Enemy.EnemyHealthState state, Direction direction)
    {
        EnemyScriptable data = instance.enemyScriptables.Find(x => x.GetEnemyType() == type);

        if (data != null)
        {
            switch (state)
            {
                case Enemy.EnemyHealthState.HEALTHY:
                    return data.GetHealthyIdleAnimation(direction);
                case Enemy.EnemyHealthState.DAMAGED:
                    return data.GetDamagedIdleAnimation(direction);
                case Enemy.EnemyHealthState.CRITICAL:
                    return data.GetCriticalIdleAnimation(direction);
            }
        }

        return null;
    }

    /// <summary>
    /// Returns the prefab GameObject of a given Enemy type. Does not instantiate anything --
    /// it is important to do so with the returned component outside of this method.
    /// </summary>
    /// <param name="enemyType">The type of Enemy to get.</param>
    /// <returns>an Enemy component of the given type. </returns>
    public static GameObject GetEnemyPrefab(string enemyType)
    {
        Assert.IsNotNull(enemyType, "enemyType is null.");

        Enemy.EnemyType type;
        if (enemyType.ToLower() == "kudzu") type = Enemy.EnemyType.KUDZU;
        else type = Enemy.EnemyType.KUDZU;

        EnemyScriptable data = instance.enemyScriptables.Find(x => x.GetEnemyType() == type);
        GameObject prefabToClone = data.GetPrefab();
        Assert.IsNotNull(prefabToClone);
        return prefabToClone;
    }
}
