using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents something that can be animated.
/// </summary>
public interface IAnimatable
{
    /// <summary>
    /// The IAnimatable's current animation track. 
    /// </summary>
    public Sprite[] currentAnimationTrack { get; set; }

    /// <summary>
    /// The duration of the IAnimatable's current animation. 
    /// </summary>
    public float currentAnimationDuration { get; set; }
}
