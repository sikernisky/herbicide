using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Stores data for projectiles.
/// </summary>
[CreateAssetMenu(fileName = "ProjectileScriptable", menuName = "Projectile Scriptable", order = 0)]
public class ProjectileScriptable : ScriptableObject
{
    /// <summary>
    /// Prefab of this Projectile.
    /// </summary>
    [SerializeField]
    private GameObject projectilePrefab;

    /// <summary>
    /// Type of the Projectile.
    /// </summary>
    [SerializeField]
    private Projectile.ProjectileType projectileType;

    /// <summary>
    /// Movement animation of the Projectile.
    /// </summary>
    [SerializeField]
    private Sprite[] moveAnimation;


    /// <summary>
    /// Returns the ProjectileType of this ProjectileScriptable.
    /// </summary>
    /// <returns>the ProjectileType of this ProjectileScriptable.
    /// </returns>
    public Projectile.ProjectileType GetProjectileType() => projectileType;

    /// <summary>
    /// Returns a copy of this Projectile's movement animation.
    /// </summary>
    /// <returns>a copy of this Projectile's movement animation.</returns>
    public Sprite[] GetMovementAnimation() { return (Sprite[])moveAnimation.Clone(); }

    /// <summary>
    /// Returns the prefab that represents this Projectiles.
    /// </summary>
    /// <returns>the prefab that represents this Projectile.</returns>
    public GameObject GetPrefab()
    {
        Assert.IsNotNull(projectilePrefab.GetComponent<Projectile>(), "Prefab has no Projectile component.");
        return projectilePrefab;
    }



}
