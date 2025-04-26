using UnityEngine;

public class GoalHole : Mob
{
    #region Fields

    #endregion

    #region Stats

    /// <summary>
    /// Type of a GoalHole.
    /// </summary>
    public override ModelType TYPE => ModelType.GOAL_HOLE;

    /// <summary>
    /// Starting attack range of a GoalHole.
    /// </summary>
    public override float BASE_MAIN_ACTION_RANGE => 0f;

    /// <summary>
    /// Maximum attack range of a GoalHole.
    /// </summary>
    public override float MAX_MAIN_ACTION_RANGE => float.MaxValue;

    /// <summary>
    /// Minimum attack range of a GoalHole.
    /// </summary>
    public override float MIN_MAIN_ACTION_RANGE => 0f;

    /// <summary>
    /// Starting attack cooldown of a GoalHole.
    /// </summary>
    public override float BASE_MAIN_ACTION_SPEED => float.MaxValue;

    /// <summary>
    /// Maximum attack cooldown of a GoalHole.
    /// </summary>
    public override float MAX_MAIN_ACTION_SPEED => float.MaxValue;

    /// <summary>
    /// Starting main action animation duration of a GoalHole.
    /// </summary>
    public override float BaseMainActionAnimationDuration => 0f;

    /// <summary>
    /// Starting chase range of a GoalHole.
    /// </summary>
    public override float BASE_CHASE_RANGE => 0f;

    /// <summary>
    /// Maximum chase range of a GoalHole.
    /// </summary>
    public override float MAX_CHASE_RANGE => float.MaxValue;

    /// <summary>
    /// Minimum chase range of a GoalHole.
    /// </summary>
    public override float MIN_CHASE_RANGE => 0f;

    /// <summary>
    /// Starting movement speed of a GoalHole.
    /// </summary>
    public override float BASE_MOVEMENT_SPEED => 0f;

    /// <summary>
    /// Maximum movement speed of a GoalHole.
    /// </summary>
    public override float MAX_MOVEMENT_SPEED => float.MaxValue;

    /// <summary>
    /// Minimum movement speed of a GoalHole.
    /// </summary>
    public override float MIN_MOVEMENT_SPEED => 0f;

    /// <summary>
    /// Starting movement animation duration of a GoalHole.
    /// </summary>
    public override float BaseMovementAnimationDuration => 0;

    /// <summary>
    /// Starting health of a GoalHole.
    /// </summary>
    public override float BaseHealth => 100;

    /// <summary>
    /// Maximum health of a GoalHole.
    /// </summary>
    public override float MaxHealth => int.MaxValue;

    /// <summary>
    /// Minimum health of a GoalHole.
    /// </summary>
    public override float MinHealth => 0;

    /// <summary>
    /// GoalHoles are always traversable.
    /// </summary>
    public override bool IsTraversable => true;

    #endregion

    #region Methods

    /// <summary>
    /// Returns the Sprite that represents this GoalHole when placing.
    /// </summary>
    /// <returns> the Sprite that represents this GoalHole when placing.
    /// </returns>
    public override Sprite[] GetPlacementTrack() => HoleFactory.GetGoalHolePlacementTrack();

    /// <summary>
    /// Returns an instantiated copy of this GoalHole.
    /// </summary>
    /// <returns>an instantiated copy of this GoalHole.</returns>
    public override GameObject CreateNew() => HoleFactory.GetHolePrefab(ModelType.GOAL_HOLE);

    #endregion
}
