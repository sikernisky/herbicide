using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Represents a bonus earned by the player.
/// </summary>
public abstract class Synergy : Effect
{
    /// <summary>
    /// Duration of a Synergy; they are not bounded by time and can
    /// last forever. 
    /// </summary>
    public override float DURATION => float.MaxValue;

    /// <summary>
    /// Maximum number of a Synergy effect that can be applied to a subject;
    /// always 1.
    /// </summary>
    public override int MAX_STACKS => 1;

    /// <summary>
    /// Highest possible tier of this Synergy.
    /// </summary>
    public abstract int HIGHEST_TIER { get; }

    /// <summary>
    /// Tier of the Synergy. If 0, the Synergy is not active.
    /// </summary>
    private int currentTier;


    /// <summary>
    /// Creates a new Synergy.
    /// </summary>
    /// <param name="subject">The Model affected by the Synergy.</param>
    /// <param name="tier">The tier of the Synergy.</param>
    /// <returns></returns>
    public Synergy(Model subject, int tier) : base(subject)
    {
        Assert.IsTrue(tier >= 0 && tier <= HIGHEST_TIER, "Tier needs to be in "
            + "1 - " + HIGHEST_TIER + ".");
        currentTier = tier;
    }

    /// <summary>
    /// Main update loop for the Synergy. Adds logic to check
    /// whether the current Synergy is active (its tier is > 1.)
    /// </summary>
    public override void UpdateEffect()
    {
        if (GetTier() < 1) return;
        base.UpdateEffect();
    }

    /// <summary>
    /// Returns this Synergy's tier.
    /// </summary>
    /// <returns>this Synergy's tier.</returns>
    public int GetTier() { return currentTier; }

    /// <summary>
    /// Changes the tier of this Synergy.
    /// </summary>
    /// <param name="newTier">The new Tier. </param>
    public virtual void ChangeTier(int newTier)
    {
        Assert.IsTrue(newTier >= 0 && newTier <= HIGHEST_TIER, "Tier needs to be in "
            + "1 - " + HIGHEST_TIER + ".");

        currentTier = newTier;
    }
}
