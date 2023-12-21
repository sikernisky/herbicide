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
    /// How much currency is required to place this PlaceableObject.
    /// </summary>
    public abstract int COST { get; }

    /// <summary>
    /// Amount of health this PlaceableObject starts with.
    /// </summary>
    public abstract int BASE_HEALTH { get; }

    /// <summary>
    /// Most amount of health this PlaceableObject can have.
    /// </summary>
    public abstract int MAX_HEALTH { get; }

    /// <summary>
    /// Least amount of health this PlaceableObject can have.
    /// </summary>
    public abstract int MIN_HEALTH { get; }

    /// <summary>
    /// Current amount of health.
    /// </summary>
    private int health;

    /// <summary>
    /// The scale of this PlaceableObject when placed.
    /// </summary>
    protected virtual Vector3 PLACEMENT_SCALE => Vector3.one;

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
    /// Returns the GameObject that represents this PlaceableObject on the grid.
    /// </summary>
    /// <returns>the GameObject that represents this PlaceableObject on the grid.
    /// </returns>
    public abstract GameObject MakePlaceableObject();

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
    /// Returns the Euclidian distance from this PlaceableObject to another PlaceableObject.
    /// </summary>
    /// <param name="target">The PlaceableObject from which to calculate distance.</param>
    public float DistanceToTarget(PlaceableObject target)
    {
        float minDistance = float.MaxValue;

        foreach (var coord1 in GetExpandedTileCoordinates())
        {
            foreach (var coord2 in target.GetExpandedTileCoordinates())
            {
                float distance = Vector2Int.Distance(coord1, coord2);
                if (distance < minDistance)
                {
                    minDistance = distance;
                }
            }
        }

        return minDistance;
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
    public virtual bool Targetable() { return !Dead(); }

    /// <summary>
    /// Returns the position of where attacks directed at this PlaceableObject
    /// should go.
    /// </summary>
    /// <returns>the position of where attacks directed at this PlaceableObject
    /// should go.</returns>
    public virtual Vector3 GetAttackPosition() { return GetPosition(); }

    /// <summary>
    /// Called when this PlaceableObject is placed.
    /// </summary>
    public virtual void OnPlace() { return; }


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
    public virtual void AdjustHealth(int amount)
    {
        int healthBefore = GetHealth();
        health = Mathf.Clamp(GetHealth() + amount, MIN_HEALTH, MAX_HEALTH);
        if (GetHealth() < healthBefore) FlashDamage();
    }

    /// <summary>
    /// Returns this PlaceableObject's current health.
    /// </summary>
    /// <returns>this PlaceableObject's current health.</returns>
    public int GetHealth() { return health; }

    /// <summary>
    /// Resets this PlaceableObject's health to its starting health value.
    /// </summary>
    public void ResetHealth() { health = BASE_HEALTH; }

    /// <summary>
    /// Resets this PlaceableObject's stats to their default values.
    /// </summary>
    public override void ResetStats() { ResetHealth(); }


}
