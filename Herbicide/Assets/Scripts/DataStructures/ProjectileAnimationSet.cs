using UnityEngine;

/// <summary>
/// DataStructure to hold all animations for a Projectile.
/// </summary>
[CreateAssetMenu(fileName = "New Projectile Animation Set", menuName = "Projectile Animation Set")]
[System.Serializable]
public class ProjectileAnimationSet : ScriptableObject
{
    #region Placement Tracks

    /// <summary>
    /// Animation track for placing the Projectile.
    /// </summary>
    public Sprite[] placementTrack;

    #endregion

    #region Mid-Air Tracks

    /// <summary>
    /// Animation track for the Projectile in mid-air.
    /// </summary>
    public Sprite[] midAirTrack;

    #endregion

    #region Methods

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

    #endregion
}
