using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Manages assets for Projectiles.
/// </summary>
public class ProjectileFactory : MonoBehaviour
{
    /// <summary>
    /// Reference to the ProjectileFactory Singleton.
    /// </summary>
    private static ProjectileFactory instance;

    /// <summary>
    /// All Projectile Scriptables.
    /// </summary>
    [SerializeField]
    private List<ProjectileScriptable> projectileScriptables;


    /// <summary>
    /// Finds and sets the ProjectileFactory singleton.
    /// </summary>
    /// <param name="levelController">The LevelController singleton.</param>
    public static void SetSingleton(LevelController levelController)
    {
        if (levelController == null) return;

        ProjectileFactory[] projectileFactories = FindObjectsOfType<ProjectileFactory>();
        Assert.IsNotNull(projectileFactories, "Array of DefenderFactories is null.");
        Assert.AreEqual(1, projectileFactories.Length);
        instance = projectileFactories[0];
    }

    /// <summary>
    /// Returns the movement track for a given Projectile type.
    /// </summary>
    /// <param name="projectileType">The type of Projectile.</param>
    /// <returns>the movement track for a given Projectile type.</returns>
    public static Sprite[] GetMovementTrack(ModelType projectileType)
    {
        ProjectileScriptable data = instance.projectileScriptables.Find(
            x => x.GetModelType() == projectileType);
        Sprite[] track = data.GetMovementAnimation();
        Assert.IsNotNull(track);
        return track;
    }

    /// <summary>
    /// Returns the GameObject prefab that represents a Projectile.
    /// </summary>
    /// <param name="projectileType">the type of Projectile</param>
    /// <returns>the GameObject prefab that represents a Projectile</returns>
    public static GameObject GetProjectilePrefab(ModelType projectileType)
    {
        ProjectileScriptable data = instance.projectileScriptables.Find(
            x => x.GetModelType() == projectileType);
        GameObject prefabToClone = data.GetModelPrefab();
        Assert.IsNotNull(prefabToClone);
        return prefabToClone;
    }
}
