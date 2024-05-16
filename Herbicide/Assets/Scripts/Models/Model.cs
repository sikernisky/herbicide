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
    /// How much currency is required to buy this Model.
    /// </summary>
    public virtual int COST => 100;

    /// <summary>
    /// Name of this Model.
    /// </summary>
    public abstract string NAME { get; }

    /// <summary>
    /// Type of this Model.
    /// </summary>
    public abstract ModelType TYPE { get; }

    /// <summary>
    /// true if Mobs can hold this Model; otherwise, false.
    /// </summary>
    public virtual bool HOLDABLE => false;

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
    /// The (X, Y) dimensions of this Model.
    /// </summary>
    /// <value></value>
    public virtual Vector2Int SIZE => new Vector2Int(1, 1);

    /// <summary>
    /// The Transform of the Model that picked this Model up; null if
    /// it is not picked up.
    /// </summary>
    private Transform holder;

    /// <summary>
    /// The HOLDER_OFFSET of the model that picked this Model up.
    /// </summary>
    private Vector2 holdingOffset;

    /// <summary>
    /// The offset of this Model's held objects. 
    /// </summary>
    public virtual Vector2 HOLDER_OFFSET => new Vector2(0, 0);

    /// <summary>
    /// The number of cycles of the current animation completed.
    /// </summary>
    private int numCyclesOfCurrentAnimationCompleted;





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
    public void SetSortingOrder(int layer) { modelRenderer.sortingOrder = layer; }

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
    /// <param name="isNewTrack">optionally, set to true if this is a new track.</param>
    public void SetAnimationTrack(Sprite[] track, int startFrame = 0, bool isNewTrack = true)
    {
        Assert.IsNotNull(track, "Cannot set to a null animation track.");
        Assert.IsTrue(track.Length > 0, "Cannot have an empty animation track.");
        CurrentAnimationTrack = track;
        CurrentFrame = startFrame;
        if(isNewTrack) numCyclesOfCurrentAnimationCompleted = 0;
    }

    /// <summary>
    /// Returns the number of cycles of the current animation completed.
    /// </summary>
    /// <returns>the number of cycles of the current animation completed. </returns>
    public int GetNumCyclesOfCurrentAnimationCompleted() { return numCyclesOfCurrentAnimationCompleted; }

    /// <summary>
    /// Increments the number of cycles of the current animation completed.
    /// </summary>
    public void IncrementNumCyclesOfCurrentAnimationCompleted() { numCyclesOfCurrentAnimationCompleted++; }

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
    public virtual HashSet<Vector2Int> GetExpandedTileCoordinates()
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
    public void SetWorldPosition(Vector3 pos)
    {
        pos.z = 1;
        transform.position = pos;
    }

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
    /// Returns the position of where attacks directed at this PlaceableObject
    /// should go.
    /// </summary>
    /// <returns>the position of where attacks directed at this PlaceableObject
    /// should go.</returns>
    public virtual Vector3 GetAttackPosition() { return GetPosition(); }

    /// <summary>
    /// Rotates this Model such that it faces a given direction.
    /// </summary>
    /// <param name="direction">The direction to face.</param>
    public void FaceDirection(Direction direction) { this.direction = direction; }

    /// <summary>
    /// Changes this Model's direction such that it faces some position.
    /// </summary>
    /// <param name="t">The position to face.</param>
    public void FaceDirection(Vector3 t)
    {
        float xDistance = GetPosition().x - t.x;
        float yDistance = GetPosition().y - t.y;
        bool xGreater = Mathf.Abs(xDistance) > Mathf.Abs(yDistance)
            ? true : false;

        if (xGreater && xDistance <= 0) FaceDirection(Direction.EAST);
        if (xGreater && xDistance > 0) FaceDirection(Direction.WEST);
        if (!xGreater && yDistance <= 0) FaceDirection(Direction.NORTH);
        if (!xGreater && yDistance > 0) FaceDirection(Direction.SOUTH);
    }

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
    /// Resets this Model's state.
    /// </summary>
    public abstract void ResetModel();

    /// <summary>
    /// Sets this Model's sorting layer.
    /// </summary>
    /// <param name="layer">The layer to set to.</param>
    public void SetSortingLayer(SortingLayers layer)
    {
        modelRenderer.sortingLayerName = layer.ToString().ToLower();
    }

    /// <summary>
    /// Returns this Model's sorting layer.
    /// </summary>
    /// <returns>This Model's sorting layer. </returns>
    public SortingLayers GetSortingLayer()
    {
        string currentLayerName = modelRenderer.sortingLayerName.ToLower();
        if (Enum.TryParse<SortingLayers>(currentLayerName, true, out SortingLayers result)) return result;
        else return default;
    }

    /// <summary>
    /// Informs this Model that it has been picked up.
    /// </summary>
    /// <param name="holder">The transform of the Model that picked it up.</param>
    /// <param name="holdingOffset">The holder offset of the Model that picked it up.</param>
    ///
    public void PickUp(Transform holder, Vector3 holdingOffset)
    {
        Assert.IsNull(this.holder, "Already picked up.");
        this.holder = holder;
        this.holdingOffset = holdingOffset;
        SetSortingLayer(SortingLayers.PICKEDUPITEMS);
    }

    /// <summary>
    /// Informs this Model that it not picked up anymore.
    /// </summary>
    public void Drop()
    {
        Assert.IsNotNull(holder, "Not picked up.");
        holder = null;
        SetSortingLayer(SortingLayers.GROUNDMOBS);
    }

    /// <summary>
    /// Returns true if this Model is picked up.
    /// </summary>
    /// <returns> true if this Model is picked up; otherwise, false.</returns>
    public bool PickedUp() { return holder != null; }

    /// <summary>
    /// Returns the position of this Model when held. This is the position of
    /// the Model holding it + the Model holding it's HOLDER_OFFSET.
    /// </summary>
    /// <returns>the HOLDER_OFFSET of the Model that picked this Model up.</returns>
    public Vector2 GetHeldPosition()
    {
        return holder.position + new Vector3(holdingOffset.x, holdingOffset.y, 1);
    }

    /// <summary>
    /// Returns the GameObject that represents this Model on the grid.
    /// </summary>
    /// <returns>the GameObject that represents this Model on the grid.
    /// </returns>
    public abstract GameObject Copy();

    /// <summary>
    /// Returns a Sprite that represents this Model when it is
    /// being placed.
    /// </summary>
    /// <returns> a Sprite that represents this Model when it is
    /// being placed.</returns>
    public abstract Sprite[] GetPlacementTrack();

    /// <summary>
    /// Returns the (X, Y) dimensions of this Model's placement track.
    /// </summary>
    /// <returns>the (X, Y) dimensions of this Model's placement track.</returns>
    public virtual Vector2Int GetPlacementTrackDimensions()
    {
        return new Vector2Int(16, 16);
    }

    /// <summary>
    /// Returns the euclidian distance from this Model to another Model.
    /// </summary>
    /// <param name="other">The other model.</param>
    /// <returns>the euclidian distance from this Model to another Model.</returns>
    public float GetDistanceTo(Model other)
    {
        Assert.IsNotNull(other, "Other model is null.");
        return Vector3.Distance(GetPosition(), other.GetPosition());    
    }
}
