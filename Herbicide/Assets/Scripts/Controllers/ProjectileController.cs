using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Responsible for creating, destroying, moving, and manipulating
/// projectiles.
/// </summary>
public class ProjectileController : MonoBehaviour
{
    /// <summary>
    /// Reference to the ProjectileController singleton.
    /// </summary>
    private static ProjectileController instance;

    /// <summary>
    /// All active Projectiles.
    /// </summary>
    private static List<Projectile> activeProjectiles;

    /// <summary>
    /// Enum to represent all different types of projectiles.
    /// </summary>
    public enum ProjectileType
    {
        ACORN
    }

    /// <summary>
    /// Prefab for an acorn Projectile.
    /// </summary>
    [SerializeField]
    private GameObject acorn;



    /// <summary>
    /// Finds and sets the ProjectileController singleton. Instantiates
    /// its list of active projectiles.
    /// </summary>
    /// <param name="levelController">The LevelController singleton.</param>
    public static void SetSingleton(LevelController levelController)
    {
        Assert.IsNotNull(levelController);

        ProjectileController[] projectileControllers = FindObjectsOfType<ProjectileController>();
        Assert.IsNotNull(projectileControllers);
        Assert.AreEqual(1, projectileControllers.Length);
        instance = projectileControllers[0];

        activeProjectiles = new List<Projectile>();
    }

    /// <summary>
    /// Shoots a Projectile at a target location.
    /// </summary>
    /// <param name="projectile">The GameObject representing a projectile.</param>
    /// <param name="owner">The Transform that is shooting the projectile.</param>
    /// <param name="targetPos">The target location to shoot towards.</param>
    public static void Shoot(ProjectileType projectile, Transform owner, Vector3 targetPos)
    {
        //Safety checks and extraction
        GameObject projectileOb = instance.GetProjectileFromType(projectile);
        Assert.IsNotNull(projectileOb);
        Projectile projectileComp = projectileOb.GetComponent<Projectile>();
        Assert.IsNotNull(projectileComp);

        //Put the projectile in its starting spot
        projectileOb.transform.SetParent(owner);
        projectileOb.transform.localPosition = new Vector3(0, 0, 1);

        //Physics calculations
        Vector3 projectPos = projectileOb.transform.position;
        Vector3 direction = targetPos - projectileOb.transform.position;
        Vector3 unitDirection = direction.normalized;
        Vector3 velocity = unitDirection * projectileComp.GetSpeed();

        //Apply velocity
        projectileComp.GetBody().velocity = velocity;

        //Add to active Projectiles
        activeProjectiles.Add(projectileComp);
    }

    /// <summary>
    /// Examines all active Projectiles: <br></br>
    /// 
    /// (1) Adds to lifespan of Projectiles <br></br>
    /// (2) Removes inactive Projectiles
    /// </summary>
    public static void CheckProjectiles()
    {
        if (activeProjectiles == null) return;

        foreach (Projectile proj in activeProjectiles)
        {
            if (proj == null) continue;
            proj.AddToLifespan(Time.deltaTime);
            if (!proj.IsActive() || proj.Expired()) Destroy(proj.gameObject);
        }
        activeProjectiles.RemoveAll(proj => proj == null);
    }

    /// <summary>
    /// Returns a copy of the GameObject that represents a given projectile 
    /// type.
    /// </summary>
    /// <param name="projectile">The type of projectile to generate</param>
    /// <returns>A GameObject that represents the given projectile type.</returns>
    private GameObject GetProjectileFromType(ProjectileType projectile)
    {
        switch (projectile)
        {
            case ProjectileType.ACORN:
                return Instantiate(acorn);
            default:
                throw new System.Exception("Projectile doesn't exist.");
        }
    }
}
