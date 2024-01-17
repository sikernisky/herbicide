using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents something that the player can click on
/// to aquire/collect. 
/// </summary>
public abstract class Collectable : Model
{
    /// <summary>
    /// Seconds to complete a bob cycle.
    /// </summary>
    public virtual float BOB_SPEED => 3f;

    /// <summary>
    /// How "tall" this Currency bobs.
    /// </summary>
    public virtual float BOB_HEIGHT => .15f;

    /// <summary>
    /// How fast this Collectable moves towards the cursor.
    /// </summary>
    public virtual float HOME_SPEED => 12f;

    /// <summary>
    /// true if the player collected this Collectable; false otherwise
    /// </summary>
    private bool collected;

    /// <summary>
    /// The AnimationCurve this Collectable follows when drifting upwards.
    /// </summary>
    [SerializeField]
    private AnimationCurve driftUpCurve;

    /// <summary>
    /// The AnimationCurve this Collectable follows when homing towards the mouse.
    /// </summary>
    [SerializeField]
    private AnimationCurve homingCurve;

    /// <summary>
    /// Types of Collectables.
    /// </summary>
    public enum CollectableType
    {
        SEED_TOKEN,
        DEW
    }

    /// <summary>
    /// Does something when the player collects this Collectable.
    /// </summary>
    public virtual void OnCollect()
    {
        if (collected) return;
        collected = true;
    }

    /// <summary>
    /// Returns true if the player picked up this Collectable;
    /// otherwise, false.
    /// </summary>
    /// <returns>true if the player picked up this Collectable;
    /// otherwise, false.</returns>
    public bool Collected() { return collected; }

    /// <summary>
    /// Returns this Collectable's drift-up curve.
    /// </summary>
    /// <returns>this Collectable's drift-up curve.</returns>
    public AnimationCurve GetDriftUpCurve() { return driftUpCurve; }

    /// <summary>
    /// Returns this Collectable's homing curve.
    /// </summary>
    /// <returns>this Collectable's homing curve.</returns>
    public AnimationCurve GetHomingCurve() { return homingCurve; }

}
