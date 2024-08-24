using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileAnimationSet : ScriptableObject
{
    /// <summary>
    /// Animation track for placing the Projectile.
    /// </summary>
    public Sprite[] placementTrack;


    /// <summary>
    /// Returns the placement animation track.
    /// </summary>
    /// <param name="tier">The tier of the track to get</param>
    /// <returns>the placement animation track </returns>
    public Sprite[] GetPlacementAnimation() => placementTrack;
}
