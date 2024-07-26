using UnityEngine;
using System.Linq;
using UnityEngine.Assertions;
using System.Collections.Generic;
using System;

/// <summary>
/// Abstract class for something unreactive that can be placed on the
/// TileGrid. These objects can be targeted, but only have
/// one base animation / state. <br></br>
/// 
/// If the PlaceableObject should
/// react to the world and update its animation based off
/// game events, please inherit from Mob. 
/// </summary>
public abstract class PlaceableObject : Model
{
    /// <summary>
    /// Amount of health this PlaceableObject starts with.
    /// </summary>
    public abstract float BASE_HEALTH { get; }

    /// <summary>
    /// Most amount of health this PlaceableObject can have.
    /// </summary>
    public abstract float MAX_HEALTH { get; }

    /// <summary>
    /// Least amount of health this PlaceableObject can have.
    /// </summary>
    public abstract float MIN_HEALTH { get; }

    /// <summary>
    /// This PlaceableObject's four neighbors.
    /// </summary>
    private PlaceableObject[] neighbors;

    /// <summary>
    /// Current amount of health.
    /// </summary>
    private float health;

    /// <summary>
    /// The scale of this PlaceableObject when placed.
    /// </summary>
    protected virtual Vector3 PLACEMENT_SCALE => Vector3.one;

    /// <summary>
    /// The tile coordinates where this PlaceableObject is placed.
    /// </summary>
    private Vector2Int placedCoords;

    /// <summary>
    /// true if this PlaceableObject is placed; otherwise, false.
    /// </summary>
    private bool placed;

    /// <summary>
    /// true if this PlaceableObject occupies a Tile, preventing
    /// further placement; otherwise, false.
    /// </summary>
    public abstract bool OCCUPIER { get; }
    /// <summary>
    /// How long to flash when this PlaceableObject takes damage.
    /// </summary>
    public virtual float DAMAGE_FLASH_TIME => 0.5f;

    /// <summary>
    /// How much time remains in the current Damage Flash animation.
    /// </summary>
    private float remainingFlashAnimationTime;


    /// <summary>
    /// Returns this PlaceableObject's placement scale.
    /// </summary>
    /// <returns>this PlaceableObject's placement scale.</returns>
    public Vector3 GetPlacementScale() { return PLACEMENT_SCALE; }

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
        hollowCopy.transform.localScale = transform.localScale;
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
    public virtual bool Targetable() { return !Dead() && gameObject.activeSelf; }

    /// <summary>
    /// Called when this PlaceableObject is placed.
    /// </summary>
    /// <param name="placedCoords">The coordinates where this PlaceableObject
    /// is placed.</param>
    public virtual void OnPlace(Vector2Int placedCoords)
    {
        this.placedCoords = placedCoords;
        placed = true;
    }

    /// <summary>
    /// Returns true if this PlaceableObject is placed.
    /// </summary>
    public bool IsPlaced() { return placed; }

    /// <summary>
    /// Starts the Damage Flash animation by resetting the amount of time
    /// left in the animation to the total amount of time it takes to
    /// complete one animation cycle
    /// </summary>
    public void FlashDamage() { SetRemainingFlashAnimationTime(DAMAGE_FLASH_TIME); }

    /// <summary>
    /// Sets the amount of time this PlaceableObject's has left in its
    /// flash animation.
    /// </summary>
    /// <param name="value">The new amount of time that this PlaceableObject
    /// has left in its flash animation. .</param>
    public void SetRemainingFlashAnimationTime(float value)
    {
        remainingFlashAnimationTime = Mathf.Clamp(value, 0, DAMAGE_FLASH_TIME);
    }

    /// <summary>
    /// Returns the amount of time that remains in this PlaceableObject's
    /// flash animation. 
    /// </summary>
    /// <returns>the amount of time that remains in this PlaceableObject's
    /// flash animation</returns>
    public float TimeRemaningInFlashAnimation() { return remainingFlashAnimationTime; }

    /// <summary>
    /// Adds some amount (can be negative) of health to this PlaceableObject.
    /// </summary>
    /// <param name="amount">The amount of health to adjust.</param>
    public virtual void AdjustHealth(float amount)
    {
        float healthBefore = GetHealth();
        health = Mathf.Clamp(GetHealth() + amount, MIN_HEALTH, MAX_HEALTH);
        if (GetHealth() < healthBefore) FlashDamage();
    }

    /// <summary>
    /// Returns this PlaceableObject's current health.
    /// </summary>
    /// <returns>this PlaceableObject's current health.</returns>
    public float GetHealth() { return health; }

    /// <summary>
    /// Resets this PlaceableObject's health to its starting health value.
    /// </summary>
    public void ResetHealth() { health = Mathf.Clamp(BASE_HEALTH, MIN_HEALTH, MAX_HEALTH); }

    /// <summary>
    /// Resets this PlaceableObject's stats to their default values.
    /// </summary>
    public override void ResetModel() { ResetHealth(); }

    /// <summary>
    /// Updates this PlaceableObject with its newest four neighbors.
    /// </summary>
    /// <param name="neighbors"> The four neighbors that surround this PlaceableObject.</param>
    public virtual void UpdateNeighbors(PlaceableObject[] neighbors)
    {
        Assert.IsNotNull(neighbors, "Neighbors cannot be null.");
        Assert.IsTrue(neighbors.Length == 4, "Neighbors must have 8 elements.");

        this.neighbors = neighbors;
    }

    /// <summary>
    /// Returns this PlaceableObject's neighbors.
    /// </summary>
    /// <returns>this PlaceableObject's neighbors.</returns>
    protected PlaceableObject[] GetNeighbors() { return neighbors; }

    /// <summary>
    /// Returns the Tile coordinates where this PlaceableObject is placed.
    /// </summary>
    public Vector2Int GetPlacedCoords() { return placedCoords; }
}
