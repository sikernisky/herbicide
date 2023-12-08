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
    public Sprite[] CurrentAnimationTrack { get; }

    /// <summary>
    /// The duration of the IAnimatable's current animation. 
    /// </summary>
    public float CurrentAnimationDuration { get; }

    /// <summary>
    /// The current frame number (0-indexed) of the IAnimatable's animation. 
    /// </summary>
    public int CurrentFrame { get; }


    /// <summary>
    /// Sets the current animation track.
    /// </summary>
    /// <param name="track">the current animation track.</param>
    /// <param name="startFrame">optionally, choose which frame to start with.</param>
    public void SetAnimationTrack(Sprite[] track, int startFrame = 0);

    /// <summary>
    /// Returns true if the IAnimatable is set up with an animation track.
    /// </summary>
    /// <returns>true if the IAnimatable is set up with an animation track;
    /// otherwise, false. </returns>
    public bool HasAnimationTrack();

    /// <summary>
    /// Sets the current animation's duration.
    /// </summary>
    /// <param name="duration">the current animation durations.</param>
    public void SetAnimationDuration(float duration);

    /// <summary>
    /// Returns the length of the current animation track.
    /// </summary>
    /// <returns>the length of the current animation track.</returns>
    public int NumFrames();

    /// <summary>
    /// Increments the frame count by one; or, if it is already
    /// the final frame in the current animation, sets it to 0.
    /// </summary>
    public void NextFrame();

    /// <summary>
    /// Returns the Sprite at the current frame of the current
    /// animation.
    /// </summary>
    /// <returns>the Sprite at the current frame of the current
    /// animation</returns>
    public Sprite GetSpriteAtCurrentFrame();
}
