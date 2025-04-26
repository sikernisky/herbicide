using System.Collections.Generic;
using UnityEngine.Assertions;

public class Spurge : Enemy
{
    #region Fields

    /// <summary>
    /// The SpurgeMinions spawned by this Spurge.
    /// </summary>
    private HashSet<SpurgeMinion> spurgeMinions;

    #endregion

    #region Stats

    /// <summary>
    /// Type of a Spurge.
    /// </summary>
    public override ModelType TYPE => ModelType.SPURGE;

    /// <summary>
    /// Base health of a Spurge.
    /// </summary>
    public override float BaseHealth => 35;

    /// <summary>
    /// Upper bound of a Spurge's health. 
    /// </summary>
    public override float MaxHealth => BaseHealth;

    /// <summary>
    /// Minimum health of a Spurge
    /// </summary>
    public override float MinHealth => 0;

    /// <summary>
    /// Amount of attack cooldown this Spurge starts with.
    /// </summary>
    public override float BASE_MAIN_ACTION_SPEED => 1f;

    /// <summary>
    /// Most amount of attack cooldown this Spurge can have.
    /// </summary>
    public override float MAX_MAIN_ACTION_SPEED => float.MaxValue;

    /// <summary>
    /// Starting main action animation duration of a Spurge.
    /// </summary>
    public override float BaseMainActionAnimationDuration => .25f;

    /// <summary>
    /// Starting attack range of a Spurge.
    /// </summary>
    public override float BASE_MAIN_ACTION_RANGE => 1f;

    /// <summary>
    /// Maximum attack range of a Spurge.
    /// </summary>
    public override float MAX_MAIN_ACTION_RANGE => float.MaxValue;

    /// <summary>
    /// Minimum attack range of a Spurge.
    /// </summary>
    public override float MIN_MAIN_ACTION_RANGE => 0;

    /// <summary>
    /// Starting chase range of a Spurge.
    /// </summary>
    public override float BASE_CHASE_RANGE => float.MaxValue;

    /// <summary>
    /// Maximum chase range of a Spurge.
    /// </summary>
    public override float MAX_CHASE_RANGE => float.MaxValue;

    /// <summary>
    /// Minimum chase range of a Spurge.
    /// </summary>
    public override float MIN_CHASE_RANGE => float.MinValue;

    /// <summary>
    /// Starting movement animation duration of a Spurge.
    /// </summary>
    public override float BaseMovementAnimationDuration => 0.4f;

    /// <summary>
    /// How many seconds a Spurge's idle animation lasts,
    /// from start to finish. 
    /// </summary>
    public float IDLE_ANIMATION_DURATION => .3f;

    /// <summary>
    /// Starting movement speed of a Spurge.
    /// </summary>
    public override float BASE_MOVEMENT_SPEED => .75f;

    /// <summary>
    /// Maximum movement speed of a Spurge.
    /// </summary>
    public override float MAX_MOVEMENT_SPEED => float.MaxValue;

    /// <summary>
    /// Minumum movement speed of a Spurge.
    /// </summary>
    public override float MIN_MOVEMENT_SPEED => 0f;

    #endregion

    #region Methods

    /// <summary>
    /// Adds a SpurgeMinion to this Spurge.
    /// </summary>
    /// <param name="minion">the SpurgeMinion to add. </param>
    public void AddMinion(SpurgeMinion minion)
    {
        if(spurgeMinions == null) spurgeMinions = new HashSet<SpurgeMinion>();
        Assert.IsNotNull(minion);
        Assert.IsFalse(spurgeMinions.Contains(minion));
        spurgeMinions.Add(minion);
    }

    /// <summary>
    /// Kills all SpurgeMinions spawned by this Spurge by setting their health to 0.
    /// </summary>
    public void KillMinions()
    {
        Assert.IsNotNull(spurgeMinions);
        foreach (SpurgeMinion minion in spurgeMinions)
        {
            minion.AdjustHealth(-minion.GetHealth());
        }
    }

    #endregion
}
