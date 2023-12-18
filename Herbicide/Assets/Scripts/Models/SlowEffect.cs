using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// A Debuff that reduces a Mob's movement speed.
/// </summary>
public class SlowEffect : Effect
{
    /// <summary>
    /// How many seconds the SlowEffect lasts.
    /// </summary>
    public override float DURATION => 3f;

    /// <summary>
    /// How many instances of this SlowEffect can be applied
    /// to the same Mob at once.
    /// </summary>
    public override int MAX_STACKS => 1;

    /// <summary>
    /// The percentage of movement speed that this SlowEffect
    /// removes from its subject (0 - 1).
    /// </summary>
    protected virtual float SLOW_RATE => .75f;


    /// <summary>
    /// Creates a new SlowEffect.
    /// </summary>
    /// <param name="subject">This SlowEffect's subject.</param>
    /// <returns></returns>
    public SlowEffect(Mob subject) : base(subject) { }

    /// <summary>
    /// Slows the subject by SLOW_RATE.
    /// </summary>
    public override void UpdateEffect()
    {
        base.UpdateEffect();
        if (GetSubject() == null) return;
        if (Applied()) return;
        Mob subject = GetSubject() as Mob;
        float reduction = subject.BASE_MOVEMENT_SPEED * SLOW_RATE;
        subject.AdjustMovementSpeed(-reduction);
        SetApplied(true);
    }

    /// <summary>
    /// Ends the SlowEffect and restores the subject's
    /// siphoned movement speed. 
    /// </summary>
    public override void EndEffect()
    {
        base.EndEffect();
        if (GetSubject() == null) return;
        if (!Applied()) return;
        Mob subject = GetSubject() as Mob;
        float reduction = subject.BASE_MOVEMENT_SPEED * SLOW_RATE;
        subject.AdjustMovementSpeed(reduction);
    }
}
