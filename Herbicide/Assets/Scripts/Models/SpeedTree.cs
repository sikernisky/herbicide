using UnityEngine;

/// <summary>
/// Represents a Tree that holds Defenders and
/// increases their attack speed / main ability rate.
/// </summary>
public class SpeedTree : Tree
{
    #region Fields

    #endregion

    #region Stats

    /// <summary>
    /// How much currency it takes to place a SpeedTree
    /// </summary>
    public override int COST => 1;

    /// <summary>
    /// Type of a SpeedTree.
    /// </summary>
    public override ModelType TYPE => ModelType.SPEED_TREE;

    /// <summary>
    /// Starting health of a SpeedTree.
    /// </summary>
    public override float BASE_HEALTH => 200;

    /// <summary>
    /// Maximum health of a SpeedTree.
    /// </summary>
    public override float MAX_HEALTH => 200;

    /// <summary>
    /// Minimum health of a SpeedTree.
    /// </summary>
    public override float MIN_HEALTH => 0;

    /// <summary>
    /// Starting attack range of a SpeedTree.
    /// </summary>
    public override float BASE_ATTACK_RANGE => 0;

    /// <summary>
    /// Maximum attack range of a SpeedTree.
    /// </summary>
    public override float MAX_ATTACK_RANGE => 0;

    /// <summary>
    /// Minimum attack range of a SpeedTree.
    /// </summary>
    public override float MIN_ATTACK_RANGE => 0;

    /// <summary>
    /// Amount of attack cooldown this SpeedTree starts with.
    /// </summary>
    public override float BASE_ATTACK_SPEED => 0;

    /// <summary>
    /// Most amount of attack cooldown this SpeedTree can have.
    /// </summary>
    public override float MAX_ATTACK_SPEED => 0;

    /// <summary>
    /// Starting chase range of a SpeedTree.
    /// </summary>
    public override float BASE_CHASE_RANGE => 0f;

    /// <summary>
    /// Maximum chase range of a SpeedTree.
    /// </summary>
    public override float MAX_CHASE_RANGE => 0f;

    /// <summary>
    /// Minimum chase range of a SpeedTree.
    /// </summary>
    public override float MIN_CHASE_RANGE => 0f;

    /// <summary>
    /// Starting movement speed of a SpeedTree.
    /// </summary>
    public override float BASE_MOVEMENT_SPEED => 0f;

    /// <summary>
    /// Maximum movement speed of a SpeedTree.
    /// </summary>
    public override float MAX_MOVEMENT_SPEED => 0f;

    /// <summary>
    /// Minumum movement speed of a SpeedTree.
    /// </summary>
    public override float MIN_MOVEMENT_SPEED => 0f;

    #endregion

    #region Methods

    /// <summary>
    /// Returns the animation track that represents this SpeedTree when placing.
    /// </summary>
    /// <returns>the animation track that represents this SpeedTree when placing.
    /// </returns>
    public override Sprite[] GetPlacementTrack() => TreeFactory.GetPlacementTrack(ModelType.SPEED_TREE);

    /// <summary>
    /// Returns the GameObject that represents this SpeedTree on the grid.
    /// </summary>
    /// <returns>the GameObject that represents this SpeedTree on the grid.
    /// </returns>
    public override GameObject CreateNew() => TreeFactory.GetTreePrefab(ModelType.SPEED_TREE);

    #endregion
}
