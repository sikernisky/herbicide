using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Stores data for projectiles.
/// </summary>
[CreateAssetMenu(fileName = "ProjectileScriptable", menuName = "Projectile Scriptable", order = 0)]
public class ProjectileScriptable : ModelScriptable
{

    /// <summary>
    /// Movement animation of the Projectile.
    /// </summary>
    [SerializeField]
    private Sprite[] moveAnimation;


    /// <summary>
    /// Returns a copy of this Projectile's movement animation.
    /// </summary>
    /// <returns>a copy of this Projectile's movement animation.</returns>
    public Sprite[] GetMovementAnimation() { return (Sprite[])moveAnimation.Clone(); }
}
