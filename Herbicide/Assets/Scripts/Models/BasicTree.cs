using UnityEngine;

/// <summary>
/// Represents a Basic Tree that holds Defenders.
/// </summary>
public class BasicTree : Tree
{
    #region Fields

    #endregion

    #region Stats

    /// <summary>
    /// Type of a BasicTree.
    /// </summary>
    public override ModelType TYPE => ModelType.BASIC_TREE;

    /// <summary>
    /// Starting health of a BasicTree.
    /// </summary>
    public override float BaseHealth => 200;

    /// <summary>
    /// Maximum health of a BasicTree.
    /// </summary>
    public override float MaxHealth => 200;

    /// <summary>
    /// Minimum health of a BasicTree.
    /// </summary>
    public override float MinHealth => 0;

    /// <summary>
    /// Starting attack range of a BasicTree.
    /// </summary>
    public override float BASE_MAIN_ACTION_RANGE => 0;

    /// <summary>
    /// Maximum attack range of a BasicTree.
    /// </summary>
    public override float MAX_MAIN_ACTION_RANGE => 0;

    /// <summary>
    /// Minimum attack range of a BasicTree.
    /// </summary>
    public override float MIN_MAIN_ACTION_RANGE => 0;

    /// <summary>
    /// Amount of attack cooldown this BasicTree starts with.
    /// </summary>
    public override float BASE_MAIN_ACTION_SPEED => 0;

    /// <summary>
    /// Most amount of attack cooldown this BasicTree can have.
    /// </summary>
    public override float MAX_MAIN_ACTION_SPEED => 0;

    /// <summary>
    /// Starting chase range of a BasicTree.
    /// </summary>
    public override float BASE_CHASE_RANGE => 0f;

    /// <summary>
    /// Maximum chase range of a BasicTree.
    /// </summary>
    public override float MAX_CHASE_RANGE => 0f;

    /// <summary>
    /// Minimum chase range of a BasicTree.
    /// </summary>
    public override float MIN_CHASE_RANGE => 0f;

    /// <summary>
    /// Starting movement speed of a BasicTree.
    /// </summary>
    public override float BASE_MOVEMENT_SPEED => 0f;

    /// <summary>
    /// Maximum movement speed of a BasicTree.
    /// </summary>
    public override float MAX_MOVEMENT_SPEED => 0f;

    /// <summary>
    /// Minumum movement speed of a BasicTree.
    /// </summary>
    public override float MIN_MOVEMENT_SPEED => 0f;

    #endregion

    #region Methods

    /// <summary>
    /// Returns the animation track that represents this BasicTree when placing.
    /// </summary>
    /// <returns>the animation track that represents this BasicTree when placing.
    /// </returns>
    public override Sprite[] GetPlacementTrack() => TreeFactory.GetPlacementTrack(ModelType.BASIC_TREE);

    /// <summary>
    /// Returns the GameObject that represents this BasicTree on the grid.
    /// </summary>
    /// <returns>the GameObject that represents this BasicTree on the grid.
    /// </returns>
    public override GameObject CreateNew() => TreeFactory.GetTreePrefab(ModelType.BASIC_TREE);

    #endregion
}
