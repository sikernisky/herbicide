using UnityEngine;

/// <summary>
/// Represents a static Model that has a health value and can be
/// placed on the TileGrid. <br></br>
/// </summary>
public abstract class PlaceableObject : Model, ISurfacePlaceable, IHasCoordinates
{
    #region Fields

    /// <summary>
    /// The Object of the IPlaceable.
    /// </summary>
    public PlaceableObject Object => this;

    /// <summary>
    /// The (X, Y) coordinates of where this PlaceableObject was last placed.
    /// </summary>
    public Vector2Int Coordinates { get; protected set; }

    /// <summary>
    /// true if this PlaceableObject prevents surface traversal; otherwise,
    /// false.
    /// </summary>
    public virtual bool IsTraversable => false;

    /// <summary>
    /// true if this PlaceableObject is placed; otherwise, false.
    /// </summary>
    public bool PlacedOnSurface { get; private set; }

    /// <summary>
    /// How much time remains in the current Damage Flash animation.
    /// </summary>
    private float remainingFlashAnimationTime;

    /// <summary>
    /// Current amount of health.
    /// </summary>
    private float health;

    /// <summary>
    /// Amount of health this PlaceableObject starts with.
    /// </summary>
    public abstract float BaseHealth { get; }

    /// <summary>
    /// Most amount of health this PlaceableObject can have.
    /// </summary>
    public abstract float MaxHealth { get; }

    /// <summary>
    /// Least amount of health this PlaceableObject can have.
    /// </summary>
    public abstract float MinHealth { get; }

    /// <summary>
    /// How long to flash when this PlaceableObject takes damage.
    /// </summary>
    public virtual float DAMAGE_FLASH_TIME => 0.5f;

    #endregion

    #region Methods

    /// <summary>
    /// Defines this PlaceableObject with its coordinates.
    /// </summary>
    /// <param name="x">The X-coordinate of this PlaceableObject.</param>
    /// <param name="y">The Y-coordinate of this PlaceableObject.</param>
    public virtual void DefineWithCoordinates(int x, int y) => Coordinates = new Vector2Int(x, y);

    /// <summary>
    /// Returns a GameObject that holds a SpriteRenderer component with
    /// this PlaceableObject's placed Sprite. No other components are
    /// copied. 
    /// </summary>
    /// <returns>A GameObject with a SpriteRenderer component. </returns>
    public virtual GameObject MakeHollowObject()
    {
        GameObject hollowCopy = new GameObject("Hollow " + NAME);
        SpriteRenderer hollowRenderer = hollowCopy.AddComponent<SpriteRenderer>();
        hollowRenderer.sprite = GetSprite();
        hollowCopy.transform.position = transform.position;
        // hollowCopy.transform.localScale = transform.localScale;
        return hollowCopy;
    }

    /// <summary>
    /// Returns true if this PlaceableObject is "Dead", which implementers
    /// define in their own way.
    /// </summary>
    /// <returns>true if this PlaceableObject is "Dead"; otherwise,
    /// false. </returns>
    public abstract bool Dead();

    /// <summary>
    /// Returns true if something can target this PlaceableObject. As 
    /// a base/default, this is when this PlaceableObject is not dead. 
    /// </summary>
    /// <returns>true if something can target this PlaceableObject;
    /// otherwise, false</returns>
    public virtual bool Targetable() => !Dead() && gameObject.activeSelf;

    /// <summary>
    /// Called when this PlaceableObject is placed or moved on the TileGrid.
    /// </summary>
    /// <param name="worldPosition">the world position where this PlaceableObject was
    /// placed.</param>
    public override void OnPlace(Vector3 worldPosition)
    {
        base.OnPlace(worldPosition);
        PlacedOnSurface = true;
        int coordX = TileGrid.PositionToCoordinate(worldPosition.x);
        int coordY = TileGrid.PositionToCoordinate(worldPosition.y);
        DefineWithCoordinates(coordX, coordY);
    }

    /// <summary>
    /// Starts the Damage Flash animation by resetting the amount of time
    /// left in the animation to the total amount of time it takes to
    /// complete one animation cycle
    /// </summary>
    public void FlashDamage() => SetRemainingFlashAnimationTime(DAMAGE_FLASH_TIME);

    /// <summary>
    /// Sets the amount of time this PlaceableObject's has left in its
    /// flash animation.
    /// </summary>
    /// <param name="value">The new amount of time that this PlaceableObject
    /// has left in its flash animation. .</param>
    public void SetRemainingFlashAnimationTime(float value) => remainingFlashAnimationTime = Mathf.Clamp(value, 0, DAMAGE_FLASH_TIME);

    /// <summary>
    /// Returns the amount of time that remains in this PlaceableObject's
    /// flash animation. 
    /// </summary>
    /// <returns>the amount of time that remains in this PlaceableObject's
    /// flash animation</returns>
    public float TimeRemainingInFlashAnimation() => remainingFlashAnimationTime;

    /// <summary>
    /// Adds some amount (can be negative) of health to this PlaceableObject.
    /// </summary>
    /// <param name="amount">The amount of health to adjust.</param>
    public virtual void AdjustHealth(float amount)
    {
        float healthBefore = GetHealth();
        health = Mathf.Clamp(GetHealth() + amount, MinHealth, MaxHealth);
        if (GetHealth() < healthBefore) FlashDamage();
    }

    /// <summary>
    /// Returns this PlaceableObject's current health.
    /// </summary>
    /// <returns>this PlaceableObject's current health.</returns>
    public float GetHealth() => health;

    /// <summary>
    /// Resets this PlaceableObject's health to its starting health value.
    /// </summary>
    public void ResetHealth() => health = Mathf.Clamp(BaseHealth, MinHealth, MaxHealth);

    /// <summary>
    /// Resets this PlaceableObject's stats to their default values.
    /// </summary>
    public override void ResetModel()
    {
        base.ResetModel();
        ResetHealth();
    }

    #endregion
}
