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
public abstract class PlaceableObject : MonoBehaviour, ISlottable, ITargetable, IAnimatable
{
    //--------------------BEGIN STATS----------------------//

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
    /// How long to flash when this PlaceableObject takes damage.
    /// </summary>
    public virtual float DAMAGE_FLASH_TIME => 0.5f;

    /// <summary>
    /// How much time remains in the current Damage Flash animation.
    /// </summary>
    private float remainingFlashAnimationTime;

    /// <summary>
    /// Name of this PlaceableObject.
    /// </summary>
    public abstract string NAME { get; }

    /// <summary>
    /// How much currency is required to place this PlaceableObject.
    /// </summary>
    public abstract int COST { get; }

    //---------------------END STATS-----------------------//

    //--------------------BEGIN ANIM----------------------//

    /// <summary>
    /// This PlaceableObject's active animation track.
    /// </summary>
    public Sprite[] CurrentAnimationTrack { get; private set; }

    /// <summary>
    /// The duration of this PlaceableObject's active animation.
    /// </summary>
    public float CurrentAnimationDuration { get; private set; }

    /// <summary>
    /// The most up to date frame number of this PlaceableObject's active animation.
    /// </summary>
    public int CurrentFrame { get; private set; }

    //----------------------END ANIM----------------------//

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
    /// SpriteRenderer component for this PlaceableObject
    /// </summary>
    [SerializeField]
    private SpriteRenderer placeableRenderer;

    /// <summary>
    /// Coordinates of the Tile on which this PlaceableObject sits.
    /// </summary>
    private Vector2Int coordinates;

    /// <summary>
    /// Returns the Sprite component that represents this PlaceableObject in
    /// the Inventory.
    /// </summary>
    /// <returns>the Sprite component that represents this PlaceableObject in
    /// the Inventory.</returns>
    public abstract Sprite GetInventorySprite();

    /// <summary>
    /// This PlaceableObject's Collider component. 
    /// </summary>
    [SerializeField]
    private Collider2D placeableCollider;

    /// <summary>
    /// The Direction to which this PlaceableObject faces.
    /// </summary>
    private Direction direction;

    /// <summary>
    /// Delegate for handling collision events.
    /// </summary>
    public delegate void CollisionHandler(Collider2D other);

    /// <summary>
    /// Event triggered upon collision detection.
    /// </summary>
    public event CollisionHandler OnCollision;

    /// <summary>
    /// Every Effect that is currently modifying this PlaceableObject.s
    /// </summary>
    private HashSet<Effect> activeEffects = new HashSet<Effect>();


    /// <summary>
    /// Returns this PlaceableObject's placement scale.
    /// </summary>
    /// <returns>this PlaceableObject's placement scale.</returns>
    public Vector3 GetPlacementScale() { return PLACEMENT_SCALE; }

    /// <summary>
    /// Sets this PlaceableObject's (X, Y) Tile coordinates.
    /// </summary>
    /// <param name="x">The X-Coordinate.</param>
    /// <param name="y">The Y-Coordinate.</param>
    public void SetTileCoordinates(int x, int y) { coordinates = new Vector2Int(x, y); }

    /// <summary>
    /// Sets the Sprite component of this PlaceableObject.
    /// </summary>
    /// <param name="s">the Sprite to set to</param>
    public virtual void SetSprite(Sprite s) { if (s != null) placeableRenderer.sprite = s; }

    /// <summary>
    /// Sets this PlaceableObject's SpriteRenderer's color.
    /// </summary>
    /// <param name="color">the color to set to.</param>
    public void SetColor(Color32 newColor)
    {
        if (placeableRenderer != null) placeableRenderer.color = newColor;
    }

    /// <summary>
    /// Sets the world position of this PlaceableObject.
    /// </summary>
    /// <param name="pos">the position to set.</param>
    public void SetWorldPosition(Vector3 pos) { transform.position = pos; }

    /// <summary>
    /// Sets this PlaceableObject's SpriteRenderer component's sorting
    /// order (order in layer).
    /// </summary>
    public void SetSortingOrder(int layer)
    {
        if (layer >= 0) placeableRenderer.sortingOrder = layer;
    }

    /// <summary>
    /// Returns this PlaceableObject's sorting order.
    /// </summary>
    /// <returns>this PlaceableObject's sorting order.</returns>
    public int GetSortingOrder() { return placeableRenderer.sortingOrder; }

    /// <summary>
    /// Returns the X-Coordinate of the Tile on which this PlaceableObject sits.
    /// </summary>
    /// <returns>the X-Coordinate of the Tile on which this PlaceableObject sits.</returns>
    public int GetX() { return coordinates.x; }

    /// <summary>
    /// Returns the Y-Coordinate of the Tile on which this PlaceableObject sits.
    /// </summary>
    /// <returns>the Y-Coordinate of the Tile on which this PlaceableObject sits.</returns>
    public int GetY() { return coordinates.y; }

    /// <summary>
    /// Returns this PlaceableObject's Transform component.
    /// </summary>
    /// <returns>this PlaceableObject's Transform component.</returns>
    public Transform GetTransform() { return transform; }

    /// <summary>
    /// Returns a Sprite that represents this PlaceableObject when it is
    /// being placed.
    /// </summary>
    /// <returns> a Sprite that represents this PlaceableObject when it is
    /// being placed.</returns>
    public abstract Sprite GetPlacementSprite();

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
    /// Resets this PlaceableObject's stats to their default values.
    /// </summary>
    public virtual void ResetStats() { ResetHealth(); }

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
    /// Called when this PlaceableObject dies.
    /// </summary>
    public abstract void OnDie();

    /// <summary>
    /// Returns true if this PlaceableObject has a non-positive
    /// health-value.
    /// </summary>
    /// <param name="GetHealth("></param>
    /// <returns>true if this PlaceableObject's health is less than
    /// or equal to zero; otherwise, returns false. /// </returns>
    public virtual bool Dead() { return GetHealth() <= 0; }

    /// <summary>
    /// Returns true if something can target this PlaceableObject. As 
    /// a base/default, this is when this PlaceableObject is not dead. 
    /// </summary>
    /// <returns>true if something can target this PlaceableObject;
    /// otherwise, false</returns>
    public virtual bool Targetable() { return !Dead(); }

    /// <summary>
    /// Returns the position of this PlaceableObject.
    /// </summary>
    /// <returns>the position of this PlaceableObject.</returns>
    public Vector3 GetPosition() { return transform.position; }

    /// <summary>
    /// Returns the position of where attacks directed at this PlaceableObject
    /// should go.
    /// </summary>
    /// <returns>the position of where attacks directed at this PlaceableObject
    /// should go.</returns>
    public virtual Vector3 GetAttackPosition() { return GetPosition(); }

    /// <summary>
    /// Returns the Collider2D component used by this PlaceableObject.
    /// </summary>
    /// <returns>the Collider2D component used by this PlaceableObject.</returns>
    public Collider2D GetColllider() { return placeableCollider; }

    /// <summary>
    /// Sets this PlaceableObjects's Collider2D properties, such as its position,
    /// width, and height.
    /// </summary>
    public abstract void SetColliderProperties();

    /// <summary>
    /// Sets this PlaceableObject's Transform's local scale.
    /// </summary>
    /// <param name="scale">The scale to set to.</param>
    public void SetSize(Vector3 scale) { transform.localScale = scale; }

    /// <summary>
    /// Sets this PlaceableObject's SpriteRenderer to its most up-to date
    /// (attached) component.
    /// </summary>
    public void RefreshRenderer() { placeableRenderer = GetComponent<SpriteRenderer>(); }

    /// <summary>
    /// Returns the Sprite occupying this PlaceableObject's SpriteRenderer
    /// component.
    /// </summary>
    /// <returns>this PlaceableObject's current Sprite.</returns>
    public Sprite GetSprite() { return placeableRenderer.sprite; }

    /// <summary>
    /// Adds an effect to this PlaceableObject's list of active effects.
    /// If this effect is already applied or exceeds the maximum number
    /// of stacks possible, does nothing.
    /// </summary>
    public void ApplyEffect(Effect effect)
    {
        Assert.IsNotNull(effect, "Effect is null.");
        Assert.IsNotNull(activeEffects, "Effects list is null.");
        int maxStacks = effect.MAX_STACKS;
        Type effectTYPE = effect.GetType();
        int sameType = activeEffects.Count(e => e.GetType() == effectTYPE);
        if (sameType >= maxStacks) return;
        activeEffects.Add(effect);
    }

    /// <summary>
    /// Removes an Effect from this PlaceableObject's list of
    /// active effects. If this Effect does not exist on this
    /// PlaceableObject, does nothing.
    /// </summary>
    /// <param name="effect">The effect to remove.</param>
    public void RemoveEffect(Effect effect)
    {
        Assert.IsNotNull(effect, "Effect is null.");
        Assert.IsNotNull(activeEffects, "Effects list is null.");
        if (!activeEffects.Contains(effect)) return;
        effect.EndEffect();
        activeEffects.Remove(effect);
    }

    /// <summary>
    /// Iterates through the list of active Effects and updates
    /// each one.
    /// </summary>
    public void UpdateEffects()
    {
        Assert.IsNotNull(activeEffects, "Effects list is null.");

        List<Effect> exp = new List<Effect>();
        foreach (Effect effect in activeEffects) { if (effect.Expired()) exp.Add(effect); }
        foreach (Effect effect in exp) { RemoveEffect(effect); }
        foreach (Effect effect in activeEffects) { effect.InflictEffect(); }
    }

    /// <summary>
    /// Starts the Damage Flash animation by resetting the amount of time
    /// left in the animation to the total amount of time it takes to
    /// complete one animation cycle/// 
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
    /// Returns the amount of time that remains in this ITargetable's
    /// flash animation. 
    /// </summary>
    /// <returns>the amount of time that remains in this ITargetable's
    /// flash animation</returns>
    public float TimeRemaningInFlashAnimation() { return remainingFlashAnimationTime; }

    /// <summary>
    /// Rotates this PlaceableObject such that it faces a given direction.
    /// </summary>
    /// <param name="direction">The direction to face.</param>
    public void FaceDirection(Direction direction) { this.direction = direction; }

    /// <summary>
    /// Returns the Direction to which this PlaceableObject faces.
    /// </summary>
    /// <returns>the Direction to which this PlaceableObject faces</returns>
    public Direction GetDirection() { return direction; }


    /// <summary>
    /// Returns the Euclidian distance from this PlaceableObject to an ITargetable.
    /// </summary>
    /// <param name="target">The ITargetable from which to calculate distance.</param>
    public float DistanceToTarget(ITargetable target)
    {
        try { return Vector3.Distance(GetPosition(), target.GetPosition()); }
        catch { return -1f; }
    }

    /// <summary>
    /// Subscribes a handler (this model's controller) to the collision event.
    /// </summary>
    /// <param name="handler">The collision handler to subscribe.</param>
    public void SubscribeToCollision(CollisionHandler handler)
    {
        Assert.IsNotNull(handler, "Handler is null.");
        OnCollision += handler;
    }

    /// <summary>
    /// Called when this PlaceableObject's collider bumps into
    /// some other collider.
    /// </summary>
    /// <param name="other">The other collider.</param>
    public void OnTriggerEnter2D(Collider2D other)
    {
        //!! Ignore 0 references, this is an event. 
        OnCollision?.Invoke(other);
    }

    /// <summary>
    /// Called when this PlaceableObject is placed.
    /// </summary>
    public virtual void OnPlace() { return; }

    //--------------------BEGIN ANIM----------------------//

    /// <summary>
    /// Sets this PlaceableObject's current animation track.
    /// </summary>
    /// <param name="track">The current animation track.</param>
    /// <param name="startFrame">optionally, choose which frame to start with.</param>
    public void SetAnimationTrack(Sprite[] track, int startFrame = 0)
    {
        Assert.IsNotNull(track, "Cannot set to a null animation track.");
        Assert.IsTrue(track.Length > 0, "Cannot have an empty animation track.");
        CurrentAnimationTrack = track;
        CurrentFrame = startFrame;
    }

    /// <summary>
    /// Returns true if this PlaceableObject has a valid animation track set up.
    /// </summary>
    /// <returns> true if this PlaceableObject has a valid animation track set up;
    /// otherwise, false. /// </returns>
    public bool HasAnimationTrack() { return CurrentAnimationTrack != null; }

    /// <summary>
    /// Sets the duration of this PlaceableObject's current animation.
    /// </summary>
    /// <param name="duration">The duration of the current animation track.</param>
    public void SetAnimationDuration(float duration)
    {
        Assert.IsTrue(duration > 0, "Must have positive animation duration.");
        CurrentAnimationDuration = duration;
    }

    /// <summary>
    /// Returns the length of this PlaceableObject's current animationt track.
    /// </summary>
    /// <returns>the length of this PlaceableObject's current animationt track;
    /// 0 if null.</returns>
    public int NumFrames() { return HasAnimationTrack() ? CurrentAnimationTrack.Length : 0; }


    /// <summary>
    /// Increments the frame count by one; or, if it is already
    /// the final frame in the current animation, sets it to 0.
    /// </summary>
    public void NextFrame()
    {
        CurrentFrame = (CurrentFrame + 1 >= NumFrames()) ? 0 : CurrentFrame + 1;
    }

    /// <summary>
    /// Returns the Sprite at the current frame of the current
    /// animation.
    /// </summary>
    /// <returns>the Sprite at the current frame of the current
    /// animation</returns>
    public Sprite GetSpriteAtCurrentFrame()
    {
        Assert.IsTrue(HasAnimationTrack(), NAME + " has no animation set up.");
        return CurrentAnimationTrack[CurrentFrame];
    }

    //----------------------END ANIM----------------------//
}
