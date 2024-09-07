using UnityEngine;

/// <summary>
/// DataStructure to hold all animations for a Quill.
/// </summary>
[CreateAssetMenu(fileName = "New Quill Animation Set", menuName = "Quill Animation Set")]
[System.Serializable]
public class QuillAnimationSet : ProjectileAnimationSet
{
    #region Placement Tracks
    
    /// <summary>
    /// Animation track for placing a double Quill.
    /// </summary>
    [SerializeField]
    private Sprite[] doubleQuillPlacementAnimation;

    #endregion

    #region Mid-Air Tracks

    /// <summary>
    /// Animation track for the Quill in mid-air.
    /// </summary>
    [SerializeField]
    private Sprite[] doubleQuillMidAirAnimation;

    #endregion

    #region Methods

    /// <summary>
    /// Returns the double Quill mid-air animation.
    /// </summary>
    /// <returns>the double Quill mid-air animation.</returns>
    public Sprite[] GetDoubleQuillMidAirAnimation() => doubleQuillMidAirAnimation;

    /// <summary>
    /// Returns the double Quill placement animation.
    /// </summary>
    /// <returns>the double Quill placement animation</returns>
    public Sprite[] GetDoubleQuillPlacementAnimation() => doubleQuillPlacementAnimation;

    #endregion
}
