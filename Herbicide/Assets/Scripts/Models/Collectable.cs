using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Represents a Model that can be collected.
/// </summary>
public abstract class Collectable : Model
{
    #region Fields

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
    /// The SpriteRenderer for the shadow of this Collectable.
    /// </summary>
    [SerializeField]
    private SpriteRenderer shadowRenderer;

    #endregion

    #region Stats

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

    #endregion

    #region Methods

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
    public bool Collected() => collected;

    /// <summary>
    /// Returns this Collectable's homing curve.
    /// </summary>
    /// <returns>this Collectable's homing curve.</returns>
    public AnimationCurve GetHomingCurve() => homingCurve;

    /// <summary>
    /// Returns a fresh Collectable prefab from the object pool.
    /// </summary>
    /// <returns>a fresh Collectable prefab from the object pool. </returns>
    public override GameObject CreateNew() => CollectableFactory.GetCollectablePrefab(TYPE);

    /// <summary>
    /// Returns a Sprite that represents this Collectable when it is
    /// being placed.
    /// </summary>
    /// <returns> a Sprite that represents this Collectable when it is
    /// being placed.</returns>
    public override Sprite[] GetPlacementTrack() => CollectableFactory.GetPlacementTrack(TYPE);

    /// <summary>
    /// Sets the shadow sprite for this Collectable.
    /// </summary>
    /// <param name="sprite"></param>
    public void SetShadowSprite(Sprite sprite)
    {
        Assert.IsNotNull(sprite, "Sprite is null.");
        shadowRenderer.sprite = sprite;
    }

    /// <summary>
    /// Sets the sorting order for this Collectable.
    /// Also sets the sorting order for the shadow.
    /// </summary>
    /// <param name="order">the new sorting order to set to. </param>
    public override void SetSortingOrder(int order)
    {
        base.SetSortingOrder(order);
        shadowRenderer.sortingOrder = order - 1;
    }

    /// <summary>
    /// Resets this Collectable's state.
    /// </summary>
    public override void ResetModel()
    {
        base.ResetModel();
        collected = false;
    }

    #endregion
}
