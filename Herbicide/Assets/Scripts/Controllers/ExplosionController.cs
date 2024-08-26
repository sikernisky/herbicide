using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls explosions and area of effect events. 
/// </summary>
public class ExplosionController
{
    #region Fields

    #endregion

    #region Methods

    /// <summary>
    /// Detonates an explosion at the given GameObject's position. The
    /// explosion will deal damage to all PlaceableObjects within the
    /// GameObject's BoxCollider area
    /// </summary>
    /// <param name="explosionArea">The GameObject that determines the position
    /// and area of the explosion.</param>
    /// <param name="damage">How much damage to inflict upon each PlaceableObject
    /// within the explosion area. </param>
    /// <param name="immuneObjects">List of PlaceableObjects that are immune to
    /// the explosion's damage. </param>
    public static void DetonateExplosion(GameObject explosionArea, float damage, HashSet<PlaceableObject> immuneObjects, HashSet<Type> immuneTypes)
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
                PlaceableObject damageable = hit.GetComponent<PlaceableObject>();
                if (immuneObjects != null && immuneObjects.Contains(damageable)) continue;
                if(immuneTypes != null && immuneTypes.Contains(damageable.GetType())) continue;
                if(damageable != null) damageable.AdjustHealth(-damage);
            }
        }
    }

    #endregion
}
