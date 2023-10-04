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

    //-------------------- ----- --------------------- // 

   

    /// <summary>
    /// Sets this Mob to play some Animation. If the animation
    /// passed into this method is the one it is currently
    /// playing, restarts it.
    /// </summary>
    /// <param name="animation"></param>
    protected void PlayAnimation(Enum animation)
    {
        if (animation == currentAnimation) return; //RESTART!
        currentAnimation = animation;
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
    /// Plays the current animation of this Mob. Acts like a flipbook;
    /// keeps track of frames and increments this counter to apply
    /// the correct Sprites to the Mob's SpriteRenderer.
    /// </summary>
    /// <returns>A reference to the coroutine.</returns>
    protected abstract IEnumerator CoPlayAnimation();


}
