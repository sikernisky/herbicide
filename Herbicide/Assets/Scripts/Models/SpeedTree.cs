using UnityEngine;

/// <summary>
/// Represents a Tree that holds Defenders and
/// increases their attack speed / main ability rate.
/// </summary>
public class SpeedTree : Tree
{
    #region Fields

    /// <summary>
    /// The SpeedTreeEffect applied to the Defender on this SpeedTree.
    /// </summary>
    private SpeedTreeEffect appliedSpeedTreeEffect;

    /// <summary>
    /// Type of a SpeedTree.
    /// </summary>
    public override ModelType TYPE => ModelType.SPEED_TREE;

    /// <summary>
    /// Starting health of a SpeedTree.
    /// </summary>
    public override float BaseHealth => 200;

    /// <summary>
    /// Maximum health of a SpeedTree.
    /// </summary>
    public override float MaxHealth => 200;

    /// <summary>
    /// Minimum health of a SpeedTree.
    /// </summary>
    public override float MinHealth => 0;

    /// <summary>
    /// Starting attack range of a SpeedTree.
    /// </summary>
    public override float BASE_MAIN_ACTION_RANGE => 0;

    /// <summary>
    /// Maximum attack range of a SpeedTree.
    /// </summary>
    public override float MAX_MAIN_ACTION_RANGE => 0;

    /// <summary>
    /// Minimum attack range of a SpeedTree.
    /// </summary>
    public override float MIN_MAIN_ACTION_RANGE => 0;

    /// <summary>
    /// Amount of attack cooldown this SpeedTree starts with.
    /// </summary>
    public override float BASE_MAIN_ACTION_SPEED => 0;

    /// <summary>
    /// Most amount of attack cooldown this SpeedTree can have.
    /// </summary>
    public override float MAX_MAIN_ACTION_SPEED => 0;

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
    /// Returns true if a candidate can place on this SpeedTree. If
    /// so, places it and applies the SpeedTreeEffect.
    /// </summary>
    /// <param name="candidate">The candidate.</param>
    /// <returns>true if a candidate was placed on this SpeedTree; otherwise,
    /// false. </returns>
    public override bool Place(ISurfacePlaceable candidate)
    {
        if(!base.Place(candidate)) return false;
        SpeedTreeEffect effect = new SpeedTreeEffect();
        if (effect.CanAfflict(candidate.Object))
        {
            appliedSpeedTreeEffect = effect;
            candidate.Object.AddEffect(appliedSpeedTreeEffect);
        }
        return true;
    }

    /// <summary>
    /// Returns and removes the PlaceableObject from this SpeedTree. This does not
    /// destroy the occupant; that is the responsibility of its controller. 
    /// </summary>
    /// <param name="neighbors">This SpeedTree's neighbors.</param>
    /// <returns>the PlaceableObject that was removed from this SpeedTree.</returns>
    public override PlaceableObject Remove()
    {
        if(appliedSpeedTreeEffect != null)
        {
            Occupant.Object.RemoveEffect(appliedSpeedTreeEffect);
            appliedSpeedTreeEffect = null;
        }
        return base.Remove();
    }

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
