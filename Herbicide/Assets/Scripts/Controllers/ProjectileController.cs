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
        ACORN,
        BUTTERFLY_BOMB
    }

    /// <summary>
    /// Prefab for an acorn Projectile.
    /// </summary>
    [SerializeField]
    private GameObject acorn;

    /// <summary>
    /// Prefab for an Butterfly  Bomb Projectile.
    /// </summary>
    [SerializeField]
    private GameObject butterflyBomb;



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
    /// Shoots a Projectile at a target location. These projectiles fire in a straight line
    /// until its lifespan runs out or it hits a target. 
    /// </summary>
    /// <param name="projectile">The type of projectile to Instantiate.</param>
    /// <param name="owner">The Transform that is shooting the projectile.</param>
    /// <param name="targetPos">The target location to shoot towards.</param>
    public static void LinearShot(ProjectileType projectile, Transform owner, Vector3 targetPos)
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
        Vector3 direction = targetPos - projectileOb.transform.position;
        Vector3 unitDirection = direction.normalized;
        Vector3 velocity = unitDirection * projectileComp.GetSpeed();

        //Apply velocity
        projectileComp.GetBody().velocity = velocity;

        //Add to active Projectiles
        activeProjectiles.Add(projectileComp);
    }

    /// <summary>
    /// Lobs a Projectile at a target location. These projectiles require a fixed
    /// travel time and curve intensity/height. 
    /// </summary>
    /// <param name="projectile">The type of projectile to Instantiate.</param>
    /// <param name="owner">The Transform that is shooting the projectile.</param>
    /// <param name="targetTransform">The target's transform to shoot towards.</param>
    /// <param name="travelTime">How long it takes for this projectile to reach
    /// its target.</param>
    /// <param name="height">How tall the lob is.</param>
    public static void LobShot(ProjectileType projectile,
     Transform owner, Transform targetTransform, float travelTime, float height)
    {
        //Safety checks and extraction
        GameObject projectileOb = instance.GetProjectileFromType(projectile);
        Assert.IsNotNull(projectileOb);
        Projectile projectileComp = projectileOb.GetComponent<Projectile>();
        Assert.IsNotNull(projectileComp);

        //Put the projectile in its starting spot
        projectileOb.transform.SetParent(owner);
        projectileOb.transform.localPosition = new Vector3(0, 0, 1);

        // Start the lobbing coroutine
        projectileComp.StartCoroutine(
            instance.CoLobParabola(
                projectileComp,
                projectileOb.transform.position,
                targetTransform.position,
                travelTime,
                height
            )
        );
    }
    /// <summary>
    /// Executes a lob shot to a moving target. Updates the projectile's position
    /// to hit a moving target over a specified time frame, accounting for the target's motion.
    /// </summary>
    /// <param name="projectile">The Projectile to lob.</param>
    /// <param name="startPos">The starting position of the projectile.</param>
    /// <param name="targetPos">The target position.</param>
    /// <param name="travelTime">How long it takes for the projectile to reach its target.</param>
    /// <param name="height">The peak height of the lob trajectory.</param>
    /// <returns>Returns an IEnumerator for coroutine support.</returns>
    private IEnumerator CoLobParabola(Projectile projectile, Vector3 startPos, Vector3 targetPos, float travelTime, float height)
    {
        Transform projectileTransform = projectile.transform;
        float time = 0f;

        while (time < travelTime)
        {
            time += Time.deltaTime;
            float linearTime = time / travelTime;

            // Parabolic trajectory calculation
            float heightAtTime = (-4 * height * linearTime * linearTime) + (4 * height * linearTime);
            Vector3 pos = Vector3.Lerp(startPos, targetPos, linearTime) + new Vector3(0, heightAtTime, 0);

            projectileTransform.position = pos;
            yield return null;
        }

        projectileTransform.position = targetPos; // Ensure projectile reaches the exact target position
        yield return null;
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
            case ProjectileType.BUTTERFLY_BOMB:
                return Instantiate(butterflyBomb);
            default:
                throw new System.Exception("Projectile doesn't exist.");
        }
    }
}
