using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Represents a living entity in the game. All Mobs are placeable.
/// </summary>
public abstract class Mob : PlaceableObject
{
    //-------------------- STATS --------------------- // 

    /// <summary>
    /// Starting health of a Mob.
    /// </summary>
    public abstract int BASE_HEALTH { get; }

    /// <summary>
    /// This Mob's largest possible health value.
    /// </summary>
    public abstract int MAX_HEALTH { get; }

    /// <summary>
    /// This Mob's smallest possible health value.
    /// </summary>
    public abstract int MIN_HEALTH { get; }

    /// <summary>
    /// Starting attack range of this Mob.
    /// </summary>
    public abstract float BASE_ATTACK_RANGE { get; }

    /// <summary>
    /// Upper bound of attack range of this Mob.
    /// </summary>
    public abstract float MAX_ATTACK_RANGE { get; }

    /// <summary>
    /// Lower bound of attack range of this Mob.
    /// </summary>
    public abstract float MIN_ATTACK_RANGE { get; }

    /// <summary>
    /// Starting attack speed of this Mob.
    /// </summary>
    public abstract float BASE_ATTACK_SPEED { get; }

    /// <summary>
    /// Upper bound of attack speed of this Mob.
    /// </summary>
    public abstract float MAX_ATTACK_SPEED { get; }

    /// <summary>
    /// Lower bound of attack speed of this Mob.
    /// </summary>
    public abstract float MIN_ATTACK_SPEED { get; }

    /// <summary>
    /// Current AnimationType this Mob is playing.
    /// </summary>
    private Enum currentAnimation;

    /// <summary>
    /// Reference to this Mob's animation coroutine.
    /// </summary>
    private IEnumerator animationReference;

    /// <summary>
    /// true if this Mob is spawned in the scene.
    /// </summary>
    private bool spawned;

    /// <summary>
    /// The current frame number of the animation loop. Zero-indexed.
    /// </summary>
    private int frame;

    /// <summary>
    /// The maxmimum number of frames for the animation playing
    /// in the current animation loop. Zero-indexed.
    /// </summary>
    private int frameCount;

    /// <summary>
    /// true if the frame count/limit was updated after the animation
    /// changed; otherwise, false.
    /// </summary>
    private bool frameCountUpdated;

    /// <summary>
    /// The "flip-book" Sprite track of the current animation.
    /// </summary>
    private Sprite[] currentAnimationTrack;

    //-------------------- ----- --------------------- // 

    /// <summary>
    /// Sets this Mob to play some Animation. If the animation
    /// passed into this method is the one it is currently
    /// playing, restarts it.
    /// </summary>
    /// <param name="animation">The animation to play.</param>
    /// <param name="frameCount">The maximum number of frames in the animation
    /// to play.</param>
    protected void PlayAnimation(Enum animation)
    {
        Assert.IsTrue(frameCountUpdated, "Need to call SetFrameCount.");

        if (animation == currentAnimation) frame = 0;
        else currentAnimation = animation;
        frameCountUpdated = false;
    }

    /// <summary>
    /// Sets the maximum number of frames in the current animation loop.
    /// </summary>
    /// <param name="frameCount">the maximum number of frames in the current animation loop. </param>
    protected void SetFrameCount(int frameCount)
    {
        Assert.IsTrue(frameCount >= 0, "Frame count must be greater than or equal to 0.");
        this.frameCount = frameCount;
        frameCountUpdated = true;
    }

    /// <summary>
    /// Returns the current frame limit.
    /// </summary>
    /// <returns>the current frame limit.</returns>
    public int GetFrameCount()
    {
        return frameCount;
    }

    /// <summary>
    /// Increments the frame count by one; or, if it is already
    /// the final frame in the current animation, sets it to 0.
    /// </summary>
    protected void NextFrame()
    {
        if (frame + 1 > GetFrameCount()) frame = 0;
        else frame++;
    }

    /// <summary>
    /// Returns the current frame number of the animation in this Mob's
    /// animation loop.
    /// </summary>
    /// <returns>the current frame number of the animation in this Mob's
    /// animation loop. </returns>
    public int GetFrameNumber()
    {
        return frame;
    }

    /// <summary>
    /// Returns true if this Mob is currently playing an Animation.
    /// </summary>
    /// <returns>true if this Mob is currently playing an Animation;
    /// otherwise, false.</returns>
    public bool PlayingAnimation()
    {
        return animationReference != null;
    }

    /// <summary>
    /// Returns the AnimationType this Mob is currently playing.
    /// </summary>
    /// <returns>the AnimationType this Mob is playing.</returns>
    protected Enum GetCurrentAnimation()
    {
        return currentAnimation;
    }

    /// <summary>
    /// Performs actions when this Mob first enters the scene.
    /// </summary>
    public virtual void OnSpawn()
    {
        Assert.IsFalse(PlayingAnimation(), GetName() + " is already playing an animation.");
        animationReference = CoPlayAnimation();
        StartCoroutine(animationReference);
        spawned = true;
    }

    /// <summary>
    /// Returns true if this Mob is spawned in the scene.
    /// </summary>
    /// <returns>true if this Mob is spawned in the scene.</returns>
    public bool Spawned()
    {
        return spawned;
    }

    /// <summary>
    /// Sets the animation track of the current animation. 
    /// </summary>
    /// <param name="track">the animation track to set.</param>
    protected void SetCurrentAnimationTrack(Sprite[] track)
    {
        Assert.IsNotNull(track, "Animation track is null.");
        Assert.IsTrue(frameCountUpdated, "Need to call SetFrameCount.");

        currentAnimationTrack = track;
    }

    /// <summary>
    /// Returns true if this Mob is set up with an animation track.
    /// </summary>
    /// <returns>true if this Mob is set up with an animation track;
    /// otherwise, false. </returns>
    protected bool HasAnimationTrack()
    {
        return currentAnimationTrack != null;
    }

    /// <summary>
    /// Returns the Sprite at the current frame of the current
    /// animation.
    /// </summary>
    /// <returns>the Sprite at the current frame of the current
    /// animation</returns>
    protected Sprite GetSpriteAtCurrentFrame()
    {
        Assert.IsTrue(HasAnimationTrack());

        return currentAnimationTrack[GetFrameNumber()];
    }

    /// <summary>
    /// Plays the current animation of this Mob. Acts like a flipbook;
    /// keeps track of frames and increments this counter to apply
    /// the correct Sprites to the Mob's SpriteRenderer.
    /// </summary>
    /// <returns>A reference to the coroutine.</returns>
    protected abstract IEnumerator CoPlayAnimation();
}
