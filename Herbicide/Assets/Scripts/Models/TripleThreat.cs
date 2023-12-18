using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Class to represent the TripleThreat synergy.
/// </summary>
public class TripleThreat : Synergy
{
    /// <summary>
    /// The TripleThreat tier I attack speed cooldown reduction.
    /// </summary>
    protected virtual float BOOST_I => 0.3f;

    /// <summary>
    /// The TripleThreat tier II attack speed cooldown reduction.
    /// </summary>
    protected virtual float BOOST_II => 0.5f;

    /// <summary>
    /// The TripleThreat tier III attack speed cooldown reduction.
    /// </summary>
    protected virtual float BOOST_III => 0.8f;

    /// <summary>
    /// Highest TripleThreat tier.
    /// </summary>
    public override int HIGHEST_TIER => 3;


    /// <summary>
    /// Creates a new TripleThreat Synergy.
    /// </summary>
    /// <param name="subject">The affected Model.</param>
    /// <param name="tier">The tier of the synergy.</param>
    /// <returns></returns>
    public TripleThreat(Model subject, int tier) : base(subject, tier) { }

    /// <summary>
    /// Main update loop for the TripleThreat Synergy.
    /// </summary>
    public override void UpdateEffect()
    {
        base.UpdateEffect();
        if (GetSubject() == null) return;
        if (Applied()) return;
        Mob subject = GetSubject() as Mob;
        subject.AdjustAttackCooldownCap(-CalculateAttackCooldownReduction());
        SetApplied(true);
    }

    /// <summary>
    /// Ends the TripleThreat Synergy.
    /// </summary>
    public override void EndEffect()
    {
        base.EndEffect();
        if (GetSubject() == null) return;
        if (!Applied()) return;
        Mob subject = GetSubject() as Mob;
        subject.AdjustAttackCooldownCap(CalculateAttackCooldownReduction());
    }

    /// <summary>
    /// Changes the TripleThreat tier, altering the amount of attack
    /// cooldown reduced from the subject.
    /// </summary>
    /// <param name="newTier">The new tier. </param>
    public override void ChangeTier(int newTier)
    {
        if (GetSubject() == null) return;
        if (newTier == GetTier()) return;

        Mob subject = GetSubject() as Mob;

        // Remove old bonus
        subject.AdjustAttackCooldownCap(CalculateAttackCooldownReduction());

        // Change the tier
        base.ChangeTier(newTier);

        // Add new bonus
        subject.AdjustAttackCooldownCap(-CalculateAttackCooldownReduction());
    }

    /// <summary>
    /// Returns the amount of attack cooldown to add or reduce from
    /// the subject based on the current TripleThreat tier.
    /// </summary>
    /// <returns>the amount of attack cooldown to add or reduce.</returns>
    private float CalculateAttackCooldownReduction()
    {
        if (GetSubject() == null || GetTier() == 0) return 0f;
        Mob subject = GetSubject() as Mob;
        if (GetTier() == 1) return subject.BASE_ATTACK_COOLDOWN * BOOST_I;
        else if (GetTier() == 2) return subject.BASE_ATTACK_COOLDOWN * BOOST_II;
        else return subject.BASE_ATTACK_COOLDOWN * BOOST_III;
    }
}
