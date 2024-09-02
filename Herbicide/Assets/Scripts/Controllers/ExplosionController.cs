using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Controls explosions and area of effect events. 
/// </summary>
public class ExplosionController : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// Reference to the ExplosionController instance.
    /// </summary>
    private static ExplosionController instance;

    #endregion

    #region Methods

    /// <summary>
    /// Finds and sets the ExplosionController singleton for a level.
    /// </summary>
    /// <param name="levelController">The LevelController singleton.</param>
    public static void SetSingleton(LevelController levelController)
    {
        if (levelController == null) return;
        if (instance != null) return;

        ExplosionController[] explosionControllers = FindObjectsOfType<ExplosionController>();
        Assert.IsNotNull(explosionControllers, "Array of InputControllers is null.");
        Assert.AreEqual(1, explosionControllers.Length);
        instance = explosionControllers[0];
    }

    /// <summary>
    /// Detonates an explosion at the given GameObject's position. The
    /// explosion will deal damage to all PlaceableObjects within the
    /// GameObject's BoxCollider area. This explosion hurts Enemy objects.
    /// </summary>
    /// <param name="explosionArea">The GameObject that determines the position
    /// and area of the explosion.</param>
    /// <param name="damage">How much damage to inflict upon each PlaceableObject
    /// within the explosion area. </param>
    /// <param name="immuneEnemies">List of PlaceableObjects that are immune to
    /// the explosion's damage. </param>
    public static void ExplodeOnEnemies(GameObject explosionArea, float damage, HashSet<Enemy> immuneEnemies)
    {
        BoxCollider2D boxCollider = explosionArea.GetComponent<BoxCollider2D>();
        if (boxCollider != null)
        {
            // Calculate the world space position and size of the BoxCollider2D
            Vector2 worldCenter = explosionArea.transform.TransformPoint(boxCollider.offset);
            Vector2 size = boxCollider.size;
            Vector3 scale = explosionArea.transform.lossyScale;
            Vector2 worldSize = new Vector2(size.x * scale.x, size.y * scale.y);

            // Using OverlapBox to find colliders in 2D
            Collider2D[] colliders = Physics2D.OverlapBoxAll(worldCenter, worldSize, explosionArea.transform.eulerAngles.z);
            foreach (Collider2D hit in colliders)
            {
                Enemy damageable = hit.GetComponent<Enemy>();
                if (immuneEnemies != null && immuneEnemies.Contains(damageable)) continue;
                if(damageable != null) damageable.AdjustHealth(-damage);
            }
        }
    }

    /// <summary>
    /// Detonates an explosion at the given position with a specified radius. The
    /// explosion will deal damage to all PlaceableObjects within the radius. This explosion
    /// hurts Enemy objects.
    /// </summary>
    /// <param name="center">The center position of the explosion.</param>
    /// <param name="radius">The radius of the explosion area.</param>
    /// <param name="damage">How much damage to inflict upon each PlaceableObject
    /// within the explosion area.</param>
    /// <param name="immuneEnemies">List of PlaceableObjects that are immune to
    /// the explosion's damage.</param>
    public static void ExplodeOnEnemies(Vector2 center, float radius, float damage, HashSet<Enemy> immuneEnemies)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(center, radius);
        foreach (Collider2D hit in colliders)
        {
            Enemy damageable = hit.GetComponent<Enemy>();
            if (damageable != null && (immuneEnemies == null || !immuneEnemies.Contains(damageable)))
            {
                damageable.AdjustHealth(-damage);
            }
        }
    }

    #endregion
}
