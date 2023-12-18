using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Represents a status that changes a PlaceableObject's
/// behavior in some way. [TODO: Make it animatable...]
/// </summary>
public abstract class Effect
{
    /// <summary>
    /// How many seconds the effect lasts. 
    /// </summary>
    public abstract float DURATION { get; }

    /// <summary>
    /// How many instances of this Effect can be applied
    /// to the same Mob at once.
    /// </summary>
    public abstract int MAX_STACKS { get; }

    /// <summary>
    /// How many seconds of the Effect remain.
    /// </summary>
    private float timeRemaining;

    /// <summary>
    /// The Model that is subject to this Effect. 
    /// </summary>
    private Model subject;

    /// <summary>
    /// true if the effect has been fully applied upon its
    /// subject; otherwise, false.
    /// </summary>
    private bool applied;


    /// <summary>
    /// Creates a new Effect.
    /// </summary>
    /// <param name="subject">The Model subject 
    /// to this Effect. </param>
    public Effect(Model subject)
    {
        Assert.IsNotNull(subject, "The subject is null.");
        this.subject = subject;
        timeRemaining = DURATION;
    }

    /// <summary>
    /// Main update loop for the Effect. Inflicts its logic
    /// on its subject and updates the number of seconds it
    /// has been active. Base classes extend on it.
    /// </summary>
    public virtual void UpdateEffect()
    {
        if (timeRemaining <= 0) return;
        if (GetSubject() == null) return;
        timeRemaining -= Time.deltaTime;
    }

    /// <summary>
    /// Ends the Effect. This may be called before the
    /// Effect has completed its lifespan.
    /// </summary>
    public virtual void EndEffect()
    {
        timeRemaining = 0;
        if (GetSubject() == null) return;
    }

    /// <summary>
    /// Restarts this Effect's lifespan counter.
    /// </summary>
    public void RefreshEffect() { timeRemaining = DURATION; }

    /// <summary>
    /// Returns this Effect's subject.
    /// </summary>
    /// <returns>this Effect's subject.</returns>
    protected Model GetSubject() { return subject; }

    /// <summary>
    /// Returns true if this Effect's subject is not null and alive.
    /// </summary>
    /// <returns>true if this Effect's subject is not null and alive.</returns>
    protected bool ValidSubject() { return subject != null; }

    /// <summary>
    /// Returns true if this Effect has gone through its entire lifespan.
    /// </summary>
    /// <returns>true if this Effect has gone through its entire lifespan;
    /// otherwise, false. /// </returns>
    public bool Expired() { return timeRemaining <= 0; }

    /// <summary>
    /// Formally notifies the Effect that it has been fully applied to
    /// its subject.  
    /// </summary>
    /// <param name="applied">true if the Effect has been fully applied;
    /// otherwise, false.</param>
    protected void SetApplied(bool applied) { this.applied = applied; }

    /// <summary>
    /// Returns true if the Effect has been fully applied to its subject.
    /// </summary>
    /// <returns>true if the Effect has been fully applied to its subject.
    /// </returns>
    protected bool Applied() { return applied; }
}
