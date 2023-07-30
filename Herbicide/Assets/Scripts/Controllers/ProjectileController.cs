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
    /// <param name="targetPos">The target location to shoot towards.</param>
    public static void Shoot(GameObject projectileOb, Vector3 targetPos)
    {
        //Safety checks and extraction
        Assert.IsNotNull(projectileOb);
        Projectile projectileComp = projectileOb.GetComponent<Projectile>();
        Assert.IsNotNull(projectileComp);

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
}
