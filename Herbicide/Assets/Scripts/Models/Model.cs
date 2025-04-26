using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System;
using Unity.IO.LowLevel.Unsafe;

/// <summary>
/// Represents something Physical in the game.
/// </summary>
public abstract class Model : MonoBehaviour, IUseable
{
    #region Fields

    /// <summary>
    /// Name of this Model.
    /// </summary>
    public string NAME => TYPE.ToString();

    /// <summary>
    /// Type of this Model.
    /// </summary>
    public abstract ModelType TYPE { get; }

    /// <summary>
    /// The Direction to which this Model faces.
    /// </summary>
    public Direction Direction { get; set; }

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
    private Direction DirectionOfMostRecentAnimation { get; set;}

    /// <summary>
    /// The number of cycles of the current animation completed.
    /// </summary>
    private int NumCyclesOfCurrentAnimationCompleted { get; set; }

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
    /// Backing field for ModelCollider.
    /// </summary>
    [SerializeField]
    private Collider2D modelCollider;

    /// <summary>
    /// This Model's Collider2D component.
    /// </summary>
    public Collider2D ModelCollider => modelCollider;

    /// <summary>
    /// All active IEffect instances on this Model.
    /// </summary>
    protected List<IEffect> ActiveEffects { get; private set; }

    /// <summary>
    /// All IEffect instances to add to this Model before the current
    /// logic of ProcessEffects.
    /// </summary>
    private List<IEffect> EffectsToAddSafely { get; set; }

    /// <summary>
    /// All IEffect instances to remove from this Model after the current
    /// logic of ProcessEffects.
    /// </summary>
    private List<IEffect> EffectsToRemoveSafely { get; set; }

    /// <summary>
    /// Set of all equipment types allowed for this Model.
    /// </summary>
    protected virtual HashSet<ModelType> AllowedEquipmentTypes { get; } = new();

    /// <summary>
    /// Backing field for EquippedItems.
    /// </summary>
    private List<ModelType> _equippedItems = new();

    /// <summary>
    /// The items equipped on this Model.
    /// </summary>
    public List<ModelType> EquippedItems => new(_equippedItems);

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

    /// <summary>
    /// All Collider2Ds that this Model should not interact with.
    /// </summary>
    private HashSet<Collider2D> CollidersToIgnore { get; set; }

    #endregion

    #region Methods

    /// <summary>
    /// Resets this Model's state.
    /// </summary>
    public virtual void ResetModel()
    {
        OnCollision = null;
        OnProjectileImpact = null;
        SetColor(ColorConstants.BaseModelColor);
        InstantiateObjects();
    }

    /// <summary>
    /// Creates the necessary objects for this Model.
    /// </summary>
    private void InstantiateObjects()
    {
        ActiveEffects = new List<IEffect>();
        EffectsToAddSafely = new List<IEffect>();
        EffectsToRemoveSafely = new List<IEffect>();
        CollidersToIgnore = new HashSet<Collider2D>();
    }

    /// <summary>
    /// Equips an item to this Model.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public void Equip(ModelType item)
    {
        Assert.IsTrue(CanEquip(item), "Cannot equip this item.");
        _equippedItems.Add(item);
    }

    /// <summary>
    /// Returns true if this Model can equip a given item.
    /// </summary>
    /// <param name="item">the item to check.</param>
    /// <returns>true if this Model can equip a given item; otherwise,
    /// false. </returns>
    public virtual bool CanEquip(ModelType item)
    {
        if (!ModelTypeHelper.IsEquipment(item)) return false;
        if (_equippedItems == null) return false;
        if (_equippedItems.Count >= GameConstants.MaxEquipmentSlots) return false;
        return AllowedEquipmentTypes.Contains(item);
    }

    /// <summary>
    /// Returns true if this Model is equipped with a given item.
    /// </summary>
    /// <param name="item">the item to check.</param>
    /// <returns>true if this Model is equipped with a given item; otherwise, false.</returns>
    public bool IsEquipped(ModelType item) => EquippedItems.Contains(item);

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
    public void SetColor(Color32 newColor) => modelRenderer.color = newColor;

    /// <summary>
    /// Sets this Model's SpriteRenderer's color.
    /// </summary>
    /// <param name="color">the color to set to.</param>
    public void SetColor(Color newColor) => modelRenderer.color = newColor;

    /// <summary>
    /// Returns this Model's current color.
    /// </summary>
    /// <returns>this Model's current color.</returns>
    public Color32 GetColor() => modelRenderer.color;

    /// <summary>
    /// Sets this Model's SpriteRenderer component's sorting
    /// order (order in layer).
    /// </summary>
    public virtual void SetSortingOrder(int order) => modelRenderer.sortingOrder = order;

    /// <summary>
    /// Returns this Model's sorting order.
    /// </summary>
    /// <returns>this Model's sorting order.</returns>
    public int GetSortingOrder() => modelRenderer.sortingOrder;

    /// <summary>
    /// Sets the world position of this Model.
    /// </summary>
    /// <param name="pos">the position to set.</param>
    public void SetWorldPosition(Vector3 pos) => transform.position = pos;

    /// <summary>
    /// Returns the world position of this Model.
    /// </summary>
    /// <returns>the world position of this Model.</returns>
    public Vector3 GetWorldPosition() => transform.position;

    /// <summary>
    /// Sets this Model's Transform's local rotation.
    /// </summary>
    /// <param name="rotation">The rotation to set to.</param>
    public void SetRotation(Quaternion rotation) => transform.rotation = rotation;

    /// <summary>
    /// Returns the position of where attacks directed at this PlaceableObject
    /// should go.
    /// </summary>
    /// <returns>the position of where attacks directed at this PlaceableObject
    /// should go.</returns>
    public virtual Vector3 GetAttackPosition() => GetWorldPosition();

    /// <summary>
    /// Changes this Model's direction such that it faces a given position.
    /// </summary>
    /// <param name="target">The position to face.</param>
    public void FaceDirection(Vector3 target)
    {
        Vector3 dominantAxis = GetDominantAxis(target);

        if (dominantAxis.x > 0) Direction = Direction.WEST;
        else if (dominantAxis.x < 0) Direction = Direction.EAST;
        else if (dominantAxis.y > 0) Direction = Direction.SOUTH;
        else Direction = Direction.NORTH;
    }

    /// <summary>
    /// Determines whether the x-axis or y-axis has the greater distance.
    /// Returns a vector indicating the dominant direction.
    /// </summary>
    /// <param name="target">The position to compare.</param>
    /// <returns>A Vector2 indicating the dominant direction.</returns>
    private Vector3 GetDominantAxis(Vector3 target)
    {
        float xDistance = GetWorldPosition().x - target.x;
        float yDistance = GetWorldPosition().y - target.y;
        return Mathf.Abs(xDistance) > Mathf.Abs(yDistance)
            ? new Vector3(Mathf.Sign(xDistance), 0)
            : new Vector3(0, Mathf.Sign(yDistance));
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
    public void OnTriggerEnter2D(Collider2D other) => OnCollision?.Invoke(other);

    /// <summary>
    /// Sets this Model's Collider2D properties, such as its position,
    /// width, and height.
    /// </summary>
    public virtual void SetColliderProperties() { }

    /// <summary>
    /// Returns the name of the sorting layer of this Model.
    /// </summary>
    /// <returns>the name of the sorting layer of this Model.</returns>
    public string GetSortingLayerName() => modelRenderer.sortingLayerName;

    /// <summary>
    /// Returns a fresh GameObject that represents this Model. 
    /// </summary>
    /// <returns>a fresh GameObject that represents this Model.</returns>
    public abstract GameObject CreateNew();

    /// <summary>
    /// Clones this Model from another Model, setting all of its
    /// fields to the same values.
    /// </summary>
    /// <param name="m">The Model to clone from.</param>
    public virtual void CloneFrom(Model m) 
    {
        Assert.IsNotNull(m, "Cannot clone from a null model.");
        Direction = m.Direction;
        CurrentAnimationTrack = m.CurrentAnimationTrack;
        CurrentAnimationDuration = m.CurrentAnimationDuration;
        DirectionOfMostRecentAnimation = m.DirectionOfMostRecentAnimation;
        NumCyclesOfCurrentAnimationCompleted = m.NumCyclesOfCurrentAnimationCompleted;
        CurrentFrame = m.CurrentFrame;
        modelRenderer = m.modelRenderer;
        modelCollider = m.modelCollider;
        ActiveEffects = m.ActiveEffects;
        EffectsToAddSafely = m.EffectsToAddSafely;
        EffectsToRemoveSafely = m.EffectsToRemoveSafely;
        _equippedItems = m.EquippedItems;
    }

    /// <summary>
    /// Adds a Collider2D to the list of Colliders2D to ignore.
    /// The projectile will not detonate when colliding with this Collider2D.
    /// </summary>
    /// <param name="other">The collider to ignore. </param>
    public void AddColliderToIgnore(Collider2D other)
    {
        Assert.IsNotNull(other);
        if (CollidersToIgnore == null) CollidersToIgnore = new HashSet<Collider2D>();
        CollidersToIgnore.Add(other);
    }

    /// <summary>
    /// Returns true if this Projectile should ignore a given Collider2D.
    /// </summary>
    /// <param name="other">the Collider2D to check</param>
    /// <returns>true if this Projectile should ignore a given Collider2D;
    /// otherwise, false. </returns>
    public bool IgnoresCollider(Collider2D other)
    {
        if (CollidersToIgnore == null) return false;
        return CollidersToIgnore.Contains(other);
    }

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
    public virtual Vector2Int GetPlacementSize() => new Vector2Int(16, 16);

    /// <summary>
    /// Called when placed.
    /// </summary>
    /// <param name="worldPosition">The world position where this Model was placed.</param>
    public virtual void OnPlace(Vector3 worldPosition) { }

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
        DirectionOfMostRecentAnimation = Direction;
        if (isNewTrack) NumCyclesOfCurrentAnimationCompleted = 0;
    }

    /// <summary>
    /// Returns the Direction of the most recent animation.
    /// </summary>
    /// <returns>the Direction of the most recent animation.</returns>
    public Direction GetDirectionOfMostRecentAnimation() => DirectionOfMostRecentAnimation;

    /// <summary>
    /// Returns the number of cycles of the current animation completed.
    /// </summary>
    /// <returns>the number of cycles of the current animation completed. </returns>
    public int GetNumCyclesOfCurrentAnimationCompleted() => NumCyclesOfCurrentAnimationCompleted;

    /// <summary>
    /// Increments the number of cycles of the current animation completed.
    /// </summary>
    public void IncrementNumCyclesOfCurrentAnimationCompleted() => NumCyclesOfCurrentAnimationCompleted++;

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

    /// <summary>
    /// Returns true if this Model has the maximum number of stacks
    /// of a given IEffect instance.
    /// </summary>
    /// <param name="effect">The IEffect to check. </param>
    /// <returns>true if this Model has the maximum number of stacks
    /// of a given IEffect instance; otherwise, false.</returns>
    private bool HasMaximumStacksOfEffect(IEffect effect) => ActiveEffects.FindAll(e=>e.GetType() == effect.GetType()).Count >= effect.MaxStacks;

    /// <summary>
    /// Adds an IEffect instance to this Model.
    /// </summary>
    /// <param name="effect">the effect to add.</param>
    public void AddEffect(IEffect effect)
    {
        Assert.IsNotNull(effect, "Cannot add a null effect.");
        if(!effect.CanAfflict(this)) return;
        if (HasMaximumStacksOfEffect(effect))
        {
            if(effect.MaxStacks == 1)
            {
                // If the effect is a one-time effect, refresh it.
                IEffect existingEffect = ActiveEffects.Find(e=>e.GetType() == effect.GetType());
                TimedEffect timedEffect = existingEffect as TimedEffect;
                if(timedEffect != null) timedEffect.RefreshEffect();
            }
            return;
        }

        EffectsToAddSafely.Add(effect);
    }

    /// <summary>
    /// Removes an IEffect instance from this Model.
    /// </summary>
    /// <param name="effect">the effect to remove. </param>
    public void RemoveEffect(IEffect effect)
    {
        Assert.IsNotNull(effect, "Cannot remove a null effect.");
        Assert.IsTrue(ActiveEffects.Contains(effect), "Effect not found on this model.");

        if(!effect.IsEffectActive) EffectsToRemoveSafely.Add(effect);
    }

    /// <summary>
    /// Processes all IEffect instances on this Model. Takes
    /// care of removing expired effects and updating the timers
    /// of active effects.
    /// </summary>
    /// <param name="effects">The effects to process.</param>
    public virtual void ProcessEffects()
    {
        foreach(IEffect effect in EffectsToAddSafely)
        {
            ActiveEffects.Add(effect);
        }
        EffectsToAddSafely.Clear();

        ActiveEffects.ForEach(effect=>RemoveEffect(effect));
        ActiveEffects.ForEach(effect=>effect.UpdateEffect(this));
        
        foreach(IEffect effect in EffectsToRemoveSafely)
        {
            ActiveEffects.Remove(effect);
        }
        EffectsToRemoveSafely.Clear();
    }

    #endregion
}
