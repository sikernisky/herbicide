using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System;

/// <summary>
/// Represents something Physical in the game.
/// </summary>
public abstract class Model : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// This Model's active animation track.
    /// </summary>
    public Sprite[] CurrentAnimationTrack { get; private set; }

    /// <summary>
    /// The duration of this Model's active animation.
    /// </summary>
    public float CurrentAnimationDuration { get; private set; }

    /// <summary>
    /// The Direction of the most recent animation track
    /// </summary>
    private Direction directionOfMostRecentAnimation;

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
    /// This Model's Collider component. 
    /// </summary>
    [SerializeField]
    private Collider2D modelCollider;

    /// <summary>
    /// The Direction to which this Model faces.
    /// </summary>
    private Direction direction;

    /// <summary>
    /// true if this Model is holding a Nexus; otherwise, false. 
    /// </summary>
    private bool holdingNexus;

    /// <summary>
    /// The number of cycles of the current animation completed.
    /// </summary>
    private int numCyclesOfCurrentAnimationCompleted;

    /// <summary>
    /// All IEffect instances on this Model.
    /// </summary>
    private List<IEffect> effects = new List<IEffect>();

    /// <summary>
    /// All IEffect instances to add to this Model before the current
    /// logic of ProcessEffects.
    /// </summary>
    private List<IEffect> effectsToAddSafely = new List<IEffect>();

    /// <summary>
    /// All IEffect instances to remove from this Model after the current
    /// logic of ProcessEffects.
    /// </summary>
    private List<IEffect> effectsToRemoveSafely = new List<IEffect>();

    /// <summary>
    /// The current base color of this Model.
    /// </summary>
    private Color32 baseColor;

    #endregion

    #region Stats

    /// <summary>
    /// How much currency is required to buy this Model.
    /// </summary>
    public virtual int COST => 100;

    /// <summary>
    /// Name of this Model.
    /// </summary>
    public string NAME => TYPE.ToString();

    /// <summary>
    /// Type of this Model.
    /// </summary>
    public abstract ModelType TYPE { get; }

    /// <summary>
    /// The (X, Y) dimensions of this Model.
    /// </summary>
    /// <value></value>
    public virtual Vector2Int SIZE => new Vector2Int(1, 1);

    /// <summary>
    /// The offset of this Model's held Nexii. 
    /// </summary>
    public virtual Vector2 HOLDER_OFFSET => new Vector2(0, 0);

    /// <summary>
    /// The base color of this Model.
    /// </summary>
    public virtual Color32 BASE_COLOR => new Color32(255, 255, 255, 255);

    #endregion

    #region Events

    /// <summary>
    /// Delegate for handling collision events.
    /// </summary>
    public delegate void CollisionHandler(Collider2D other);

    /// <summary>
    /// Event triggered upon collision detection.
    /// </summary>
    public event CollisionHandler OnCollision;

    /// <summary>
    /// Event triggered by a ProjectileController when
    /// it hits this model. <br></br>
    /// 
    /// This is different than the OnCollision event because
    /// this event is manually triggered by the ProjectileController
    /// when it detects a collision with this model and its Projectile,
    /// where OnCollision is triggered by Unity's physics engine.
    /// </summary>
    public event Action<Projectile> OnProjectileImpact;

    #endregion

    #region Methods

    /// <summary>
    /// Sets this Model's SpriteRenderer to its most up-to date
    /// (attached) component.
    /// </summary>
    public void RefreshRenderer() => modelRenderer = GetComponent<SpriteRenderer>();

    /// <summary>
    /// Sets this Model's sprite mask interaction.
    /// </summary>
    /// <param name="interaction">the interaction to set to.</param>
    public void SetMaskInteraction(SpriteMaskInteraction interaction) => modelRenderer.maskInteraction = interaction;

    /// <summary>
    /// Returns the Sprite occupying this Model's SpriteRenderer
    /// component.
    /// </summary>
    /// <returns>this Model's current Sprite.</returns>
    public Sprite GetSprite() => modelRenderer.sprite;

    /// <summary>
    /// Sets the Sprite component of this Model.
    /// </summary>
    /// <param name="s">the Sprite to set to</param>
    public virtual void SetSprite(Sprite s) => modelRenderer.sprite = s != null ? s : modelRenderer.sprite;

    /// <summary>
    /// Sets this Model's SpriteRenderer's color.
    /// </summary>
    /// <param name="color">the color to set to.</param>
    public virtual void SetColor(Color32 newColor) => modelRenderer.color = modelRenderer != null ? newColor : modelRenderer.color;

    /// <summary>
    /// Returns this Model's base color.
    /// </summary>
    /// <returns>this Model's SpriteRenderer's color. </returns>
    public Color GetColor() => modelRenderer.color;

    /// <summary>
    /// Sets this Model's base color.
    /// </summary>
    /// <param name="color">the base color to set to.</param>
    public void SetBaseColor(Color32 color) => baseColor = color; 

    /// <summary>
    /// Returns this Model's base color.
    /// </summary>
    /// <returns>this model's base color. </returns>
    public Color GetBaseColor() => baseColor;

    /// <summary>
    /// Sets this Model's SpriteRenderer component's sorting
    /// order (order in layer).
    /// </summary>
    public void SetSortingOrder(int layer) => modelRenderer.sortingOrder = layer;

    /// <summary>
    /// Returns this Model's sorting order.
    /// </summary>
    /// <returns>this Model's sorting order.</returns>
    public int GetSortingOrder() => modelRenderer.sortingOrder;

    /// <summary>
    /// Sets this Model's (X, Y) Tile coordinates.
    /// </summary>
    /// <param name="x">The X-Coordinate.</param>
    /// <param name="y">The Y-Coordinate.</param>
    public void SetTileCoordinates(int x, int y) => coordinates = new Vector2Int(x, y);

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
    public Vector3 GetPosition() => transform.position;

    /// <summary>
    /// Sets this Model's Transform's local scale.
    /// </summary>
    /// <param name="scale">The scale to set to.</param>
    public void SetSize(Vector3 scale) => transform.localScale = scale;

    /// <summary>
    /// Sets this Model's Transform's local rotation.
    /// </summary>
    /// <param name="rotation">The rotation to set to.</param>
    public void SetRotation(Quaternion rotation) => transform.rotation = rotation;

    /// <summary>
    /// Returns the X-Coordinate of the Tile on which this Model sits.
    /// </summary>
    /// <returns>the X-Coordinate of the Tile on which this Model sits.</returns>
    public int GetX() => coordinates.x;

    /// <summary>
    /// Returns the Y-Coordinate of the Tile on which this Model sits.
    /// </summary>
    /// <returns>the Y-Coordinate of the Tile on which this Model sits.</returns>
    public int GetY() => coordinates.y;

    /// <summary>
    /// Returns this Model's Transform component.
    /// </summary>
    /// <returns>this Model's Transform component.</returns>
    public Transform GetTransform() => transform;

    /// <summary>
    /// Returns the position of where attacks directed at this PlaceableObject
    /// should go.
    /// </summary>
    /// <returns>the position of where attacks directed at this PlaceableObject
    /// should go.</returns>
    public virtual Vector3 GetAttackPosition() => GetPosition();

    /// <summary>
    /// Rotates this Model such that it faces a given direction.
    /// </summary>
    /// <param name="direction">The direction to face.</param>
    public void FaceDirection(Direction direction) => this.direction = direction;

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
    public Direction GetDirection() => direction;

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
    /// Subscribes a handler to the ProjectileCollision event.
    /// </summary>
    /// <param name="handler">The collision handler to subscribe.</param>
    public void SubscribeToProjectileCollisionEvent(Action<Projectile> handler) => OnProjectileImpact += handler;

    /// <summary>
    /// Invokes the OnProjectileImpact event, which is handled by the
    /// controller. We do this to avoid circular dependencies.
    /// </summary>
    /// <param name="projectile">The Projectile that hit this Model. </param>
    public void TriggerProjectileCollision(Projectile projectile)
    {
        if (projectile == null) return;
        OnProjectileImpact?.Invoke(projectile);
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
    public Collider2D GetCollider() => modelCollider;

    /// <summary>
    /// Sets this Model's Collider2D properties, such as its position,
    /// width, and height.
    /// </summary>
    public virtual void SetColliderProperties() { }

    /// <summary>
    /// Resets this Model's state.
    /// </summary>
    public virtual void ResetModel()
    {
        OnCollision = null;
        OnProjectileImpact = null;
        SetColor(BASE_COLOR);
    }

    /// <summary>
    /// Sets this Model's sorting layer.
    /// </summary>
    /// <param name="layer">The layer to set to.</param>
    public void SetSortingLayer(SortingLayers layer) => modelRenderer.sortingLayerName = layer.ToString().ToLower();

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
    /// Returns a fresh GameObject that represents this Model. 
    /// </summary>
    /// <returns>a fresh GameObject that represents this Model.</returns>
    public abstract GameObject CreateNew();

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
    public virtual Vector2Int GetPlacementTrackDimensions() => new Vector2Int(16, 16);

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

    /// <summary>
    /// Called when a Nexus determines this Model has picked it up. 
    /// </summary>
    public void OnHoldNexus() => holdingNexus = true;

    /// <summary>
    /// Returns true if this Model is currently holding a Nexus. 
    /// </summary>
    /// <returns></returns>
    public bool IsHoldingNexus() => holdingNexus;

    #endregion

    #region Animation

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
        directionOfMostRecentAnimation = direction;
        if (isNewTrack) numCyclesOfCurrentAnimationCompleted = 0;
    }

    /// <summary>
    /// Returns the Direction of the most recent animation.
    /// </summary>
    /// <returns>the Direction of the most recent animation.</returns>
    public Direction GetDirectionOfMostRecentAnimation() => directionOfMostRecentAnimation;

    /// <summary>
    /// Returns the number of cycles of the current animation completed.
    /// </summary>
    /// <returns>the number of cycles of the current animation completed. </returns>
    public int GetNumCyclesOfCurrentAnimationCompleted() => numCyclesOfCurrentAnimationCompleted;

    /// <summary>
    /// Increments the number of cycles of the current animation completed.
    /// </summary>
    public void IncrementNumCyclesOfCurrentAnimationCompleted() => numCyclesOfCurrentAnimationCompleted++;

    /// <summary>
    /// Returns true if this Model has a valid animation track set up.
    /// </summary>
    /// <returns> true if this Model has a valid animation track set up;
    /// otherwise, false. /// </returns>
    public bool HasAnimationTrack() => CurrentAnimationTrack != null;

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
    public int NumFrames() => HasAnimationTrack() ? CurrentAnimationTrack.Length : 0;


    /// <summary>
    /// Increments the frame count by one; or, if it is already
    /// the final frame in the current animation, sets it to 0.
    /// </summary>
    public void NextFrame() => CurrentFrame = (CurrentFrame + 1 >= NumFrames()) ? 0 : CurrentFrame + 1;

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

    #endregion  

    #region Effects

    /// <summary>
    /// Returns all IEffect instances on this Model.
    /// </summary>
    /// <returns>a list of IEffect instances on this Model. </returns>
    protected List<IEffect> GetEffects() => effects;

    /// <summary>
    /// Adds an IEffect instance to this Model.
    /// </summary>
    /// <param name="effect">the effect to add.</param>
    public void AddEffect(IEffect effect)
    {
        Assert.IsNotNull(effect, "Cannot add a null effect.");
        if(!effect.CanAfflict(this)) return;

        effectsToAddSafely.Add(effect);
    }

    /// <summary>
    /// Removes an IEffect instance from this Model if it
    /// is expired.
    /// </summary>
    /// <param name="effect">the effect to remove. </param>
    private void TryRemoveEffect(IEffect effect)
    {
        Assert.IsNotNull(effect, "Cannot remove a null effect.");
        Assert.IsTrue(effects.Contains(effect), "Effect not found on this model.");

        if(!effect.IsEffectActive) effectsToRemoveSafely.Add(effect);
    }

    /// <summary>
    /// Processes all IEffect instances on this Model. Takes
    /// care of removing expired effects and updating the timers
    /// of active effects.
    /// </summary>
    /// <param name="effects">The effects to process.</param>
    public virtual void ProcessEffects()
    {
        foreach(IEffect effect in effectsToAddSafely)
        {
            effects.Add(effect);
        }
        effectsToAddSafely.Clear();

        GetEffects().ForEach(effect=>TryRemoveEffect(effect));
        GetEffects().ForEach(effect=>effect.UpdateEffect(this));
        
        foreach(IEffect effect in effectsToRemoveSafely)
        {
            effects.Remove(effect);
        }
        effectsToRemoveSafely.Clear();
    }

    #endregion
}
