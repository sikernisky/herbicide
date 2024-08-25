using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds all animations for a Projectile.
/// </summary>
[CreateAssetMenu(fileName = "New Projectile Animation Set", menuName = "Projectile Animation Set")]
[System.Serializable]
public class ProjectileAnimationSet : ScriptableObject
{
    /// <summary>
    /// Animation track for placing the Projectile.
    /// </summary>
    public Sprite[] placementTrack;

    /// <summary>
    /// Animation track for the Projectile in mid-air.
    /// </summary>
    public Sprite[] midAirTrack;


    /// <summary>
    /// Returns the placement animation track.
    /// </summary>
    /// <returns>the placement animation track </returns>
    public Sprite[] GetPlacementAnimation() => placementTrack;

    /// <summary>
    /// Returns the mid-air animation track.
    /// </summary>
    /// <returns>the mid-air animation track </returns>
    public Sprite[] GetMidAirAnimation() => midAirTrack;
}
