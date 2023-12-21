using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.Linq;

/// <summary>
/// Represents something tangible and physical in the game. All
/// GameObjects inherit from this: Mobs, Projectiles, Tiles, etc.
/// </summary>
public abstract class Model : MonoBehaviour
{
    /// <summary>
    /// Different sorting layers a Model can take on.
    /// </summary>
    public enum SortingLayer
    {
        DEFAULT,
        BASE_TILE,
        FLOORING,
        TREES,
        DEFENDERS,
        PROJECTILES,
        DROPPED_ITEMS
    }

    /// <summary>
    /// Name of this Model.
    /// </summary>
    public abstract string NAME { get; }

    /// <summary>
    /// This Model's active animation track.
    /// </summary>
    public Sprite[] CurrentAnimationTrack { get; private set; }

    /// <summary>
    /// The duration of this Model's active animation.
    /// </summary>
    public float CurrentAnimationDuration { get; private set; }

    /// <summary>
    /// The most up to date frame number of this Model's active animation.
    /// </summary>
    public int CurrentFrame { get; private set; }

    /// <summary>
    /// SpriteRenderer component for this Model
    /// </summary>
    [SerializeField]
    private SpriteRenderer modelRenderer;

    /// <summary>
    /// Coordinates of the bottom-leftmost Tile on which this Model rests.
    /// </summary>
    private Vector2Int coordinates;

    /// <summary>
    /// Coordinates of all Tiles on which this Model rests.
    /// </summary>
    private HashSet<Vector2Int> expandedCoordinates;

    /// <summary>
    /// This Model's Collider component. 
    /// </summary>
    [SerializeField]
    private Collider2D modelCollider;

    /// <summary>
    /// The Direction to which this Model faces.
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
    /// All active Effects inflicted upon this Model.
    /// </summary>
    private List<Effect> activeEffects = new List<Effect>();

    /// <summary>
    /// The (X, Y) dimensions of this Model.
    /// </summary>
    /// <value></value>
    public virtual Vector2Int SIZE => new Vector2Int(1, 1);



    /// <summary>
    /// Sets this Model's SpriteRenderer to its most up-to date
    /// (attached) component.
    /// </summary>
    public void RefreshRenderer() { modelRenderer = GetComponent<SpriteRenderer>(); }

    /// <summary>
    /// Returns the Sprite occupying this Model's SpriteRenderer
    /// component.
    /// </summary>
    /// <returns>this Model's current Sprite.</returns>
    public Sprite GetSprite() { return modelRenderer.sprite; }

    /// <summary>
    /// Sets the Sprite component of this Model.
    /// </summary>
    /// <param name="s">the Sprite to set to</param>
    public virtual void SetSprite(Sprite s) { if (s != null) modelRenderer.sprite = s; }

    /// <summary>
    /// Sets this Model's SpriteRenderer's color.
    /// </summary>
    /// <param name="color">the color to set to.</param>
    public virtual void SetColor(Color32 newColor)
    {
        if (modelRenderer != null) modelRenderer.color = newColor;
    }

    /// <summary>
    /// Sets this Model's SpriteRenderer component's sorting
    /// order (order in layer).
    /// </summary>
    public void SetSortingOrder(int layer)
    {
        if (layer >= 0) modelRenderer.sortingOrder = layer;
    }

    /// <summary>
    /// Returns this Model's sorting order.
    /// </summary>
    /// <returns>this Model's sorting order.</returns>
    public int GetSortingOrder() { return modelRenderer.sortingOrder; }

    /// <summary>
    /// Sets this Model's current animation track.
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
    /// Returns true if this Model has a valid animation track set up.
    /// </summary>
    /// <returns> true if this Model has a valid animation track set up;
    /// otherwise, false. /// </returns>
    public bool HasAnimationTrack() { return CurrentAnimationTrack != null; }

    /// <summary>
    /// Sets the duration of this Model's current animation.
    /// </summary>
    /// <param name="duration">The duration of the current animation track.</param>
    public void SetAnimationDuration(float duration)
    {
        Assert.IsTrue(duration > 0, "Must have positive animation duration.");
        CurrentAnimationDuration = duration;
    }

    /// <summary>
    /// Returns the length of this Model's current animationt track.
    /// </summary>
    /// <returns>the length of this Model's current animationt track;
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

    /// <summary>
    /// Sets this Model's (X, Y) Tile coordinates.
    /// </summary>
    /// <param name="x">The X-Coordinate.</param>
    /// <param name="y">The Y-Coordinate.</param>
    public void SetTileCoordinates(int x, int y) { coordinates = new Vector2Int(x, y); }

    /// <summary>
    /// Adds all of the (X, Y) Tile coordinates this Model expands on.
    /// </summary>
    /// <param name="x">The expanded X-Coordinate.</param>
    /// <param name="y">The expanded Y-Coordinate.</param>
    public void AddExpandedTileCoordinate(int x, int y)
    {
        if (expandedCoordinates == null) expandedCoordinates = new HashSet<Vector2Int>();
        expandedCoordinates.Add(new Vector2Int(x, y));
    }


    /// <summary>
    /// Returns a copy of the HashSet of this Model's expanded Tile coordinates.
    /// </summary>
    /// <returns>a copy of the HashSet of this Model's expanded Tile coordinates.</returns>
    public HashSet<Vector2Int> GetExpandedTileCoordinates()
    {
        if (expandedCoordinates == null) return new HashSet<Vector2Int>();
        return new HashSet<Vector2Int>(expandedCoordinates);
    }

    /// <summary>
    /// Removes all stored (X, Y) Tile coordinates this Model expands on.
    /// </summary>
    public void WipeExpandedCoordinates()
    {
        if (expandedCoordinates == null) expandedCoordinates = new HashSet<Vector2Int>();
        expandedCoordinates.Clear();
    }

    /// <summary>
    /// Sets the world position of this Model.
    /// </summary>
    /// <param name="pos">the position to set.</param>
    public void SetWorldPosition(Vector3 pos) { transform.position = pos; }

    /// <summary>
    /// Returns the world position of this Model.
    /// </summary>
    /// <returns>the world position of this Model.</returns>
    public Vector3 GetPosition() { return transform.position; }

    /// <summary>
    /// Sets this Model's Transform's local scale.
    /// </summary>
    /// <param name="scale">The scale to set to.</param>
    public void SetSize(Vector3 scale) { transform.localScale = scale; }

    /// <summary>
    /// Returns the X-Coordinate of the Tile on which this Model sits.
    /// </summary>
    /// <returns>the X-Coordinate of the Tile on which this Model sits.</returns>
    public int GetX() { return coordinates.x; }

    /// <summary>
    /// Returns the Y-Coordinate of the Tile on which this Model sits.
    /// </summary>
    /// <returns>the Y-Coordinate of the Tile on which this Model sits.</returns>
    public int GetY() { return coordinates.y; }

    /// <summary>
    /// Returns this Model's Transform component.
    /// </summary>
    /// <returns>this Model's Transform component.</returns>
    public Transform GetTransform() { return transform; }

    /// <summary>
    /// Rotates this Model such that it faces a given direction.
    /// </summary>
    /// <param name="direction">The direction to face.</param>
    public void FaceDirection(Direction direction) { this.direction = direction; }

    /// <summary>
    /// Returns the Direction to which this Model faces.
    /// </summary>
    /// <returns>the Direction to which this Model faces</returns>
    public Direction GetDirection() { return direction; }

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
    /// Called when this Model's collider bumps into
    /// some other collider.
    /// </summary>
    /// <param name="other">The other collider.</param>
    public void OnTriggerEnter2D(Collider2D other)
    {
        //!! Ignore 0 references, this is an event. 
        OnCollision?.Invoke(other);
    }

    /// <summary>
    /// Returns the Collider2D component used by this Model.
    /// </summary>
    /// <returns>the Collider2D component used by this Model.</returns>
    public Collider2D GetColllider() { return modelCollider; }

    /// <summary>
    /// Sets this Model's Collider2D properties, such as its position,
    /// width, and height.
    /// </summary>
    public abstract void SetColliderProperties();

    /// <summary>
    /// Resets this Model's stats to their default values.
    /// </summary>
    public abstract void ResetStats();

    /// <summary>
    /// Adds an effect to this Model's list of active effects.
    /// If this effect is already applied or exceeds the maximum number
    /// of stacks possible, does nothing.
    /// </summary>
    public void ApplyEffect(Effect effect)
    {
        Assert.IsNotNull(effect, "Effect is null.");
        Assert.IsNotNull(activeEffects, "Effects list is null.");
        int maxStacks = effect.MAX_STACKS;
        Type effectTYPE = effect.GetType();
        int sameType = activeEffects.Count(e => e.GetType() == effect.GetType());

        if (maxStacks == 1 && sameType == 1)
        {
            activeEffects.First().RefreshEffect();
            return;
        }

        if (sameType >= maxStacks) return;
        activeEffects.Add(effect);
    }

    /// <summary>
    /// Removes an Effect from this Model's list of
    /// active effects. If this Effect does not exist on this
    /// Model, does nothing.
    /// </summary>
    /// <param name="effect">The effect to remove.</param>
    public void RemoveEffect(Effect effect)
    {
        Assert.IsNotNull(effect, "Effect is null.");
        Assert.IsNotNull(activeEffects, "Effects list is null.");
        Assert.IsNull(effect as Synergy, "Cannot remove synergies. Set tier to 0.");
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
        foreach (Effect effect in activeEffects) { effect.UpdateEffect(); }
    }

    /// <summary>
    /// Returns the Sprite component that represents this Model in
    /// the Inventory.
    /// </summary>
    /// <returns>the Sprite component that represents this Model in
    /// the Inventory.</returns>
    public abstract Sprite GetInventorySprite();

    /// <summary>
    /// Returns a Sprite that represents this Model when it is
    /// being placed.
    /// </summary>
    /// <returns> a Sprite that represents this Model when it is
    /// being placed.</returns>
    public abstract Sprite GetPlacementSprite();

    /// <summary>
    /// Sets this Model's sorting layer.
    /// </summary>
    /// <param name="layer">The layer to set to.</param>
    public void SetSortingLayer(SortingLayer layer)
    {
        if (layer == SortingLayer.DEFAULT) modelRenderer.sortingLayerName = "Default";
        if (layer == SortingLayer.BASE_TILE) modelRenderer.sortingLayerName = "BaseTile";
        if (layer == SortingLayer.FLOORING) modelRenderer.sortingLayerName = "Flooring";
        if (layer == SortingLayer.TREES) modelRenderer.sortingLayerName = "Trees";
        if (layer == SortingLayer.DEFENDERS) modelRenderer.sortingLayerName = "Defenders";
        if (layer == SortingLayer.PROJECTILES) modelRenderer.sortingLayerName = "Projectiles";
        if (layer == SortingLayer.DROPPED_ITEMS) modelRenderer.sortingLayerName = "DroppedItems";
    }
}
