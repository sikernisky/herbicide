using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Represents a living, reactive PlaceableObject.
/// </summary>
public abstract class Mob : PlaceableObject
{
    #region Fields

    /// <summary>
    /// This Mob's current main action range. 
    /// </summary>
    private float mainActionRange;

    /// <summary>
    /// Current number of times this Mob can perform its main action per second.
    /// </summary>
    private float mainActionSpeed;

    /// <summary>
    /// Current duration of this Mob's main action animation.
    /// </summary>
    private float mainActionAnimationDuration;

    /// <summary>
    /// How many seconds this Mob has to wait before performing its main action again.
    /// </summary>
    private float mainActionCooldownTimer;

    /// <summary>
    /// This Mob's current chase range. 
    /// </summary>
    private float chaseRange;

    /// <summary>
    /// This Mob's current movement speed.
    /// </summary>
    private float movementSpeed;

    /// <summary>
    /// Current duration of this Mob's movement animation.
    /// </summary>
    private float movementAnimationDuration;    

    /// <summary>
    /// true if this Mob is spawned in the scene.
    /// </summary>
    private bool spawned;

    /// <summary>
    /// Where this mob spawned.
    /// </summary>
    private Vector3 spawnPos;

    /// <summary>
    /// The default ability this Mob uses.
    /// </summary>
    [SerializeField]
    private Ability defaultAbility;

    #endregion

    #region Stats

    /// <summary>
    /// Starting amount of range from which this Mob can perform its main action on a target.
    /// </summary>
    public abstract float BASE_MAIN_ACTION_RANGE { get; }

    /// <summary>
    /// Maximum amount of range from which this Mob can perform its main action on a target.
    /// </summary>
    public abstract float MAX_MAIN_ACTION_RANGE { get; }

    /// <summary>
    /// Minimum amount of range from which this Mob can perform its main action on a target.
    /// </summary>
    public abstract float MIN_MAIN_ACTION_RANGE { get; }

    /// <summary>
    /// Starting number of times this Mob can perform its main action per second.
    /// </summary>
    public abstract float BASE_MAIN_ACTION_SPEED { get; }

    /// <summary>
    /// Max number of times this Mob can perform its main action per second.
    /// </summary>
    public abstract float MAX_MAIN_ACTION_SPEED { get; }

    /// <summary>
    /// Min number of times this Mob can perform its main action per second.
    /// </summary>
    public float MIN_MAIN_ACTION_SPEED => 0f;

    /// <summary>
    /// Starting duration of this Mob's main action animation.
    /// </summary>
    public abstract float BASE_MAIN_ACTION_ANIMATION_DURATION { get; }

    /// <summary>
    /// Maximum duration of this Mob's main action animation.
    /// </summary>
    public virtual float MAX_MAIN_ACTION_ANIMATION_DURATION => float.MaxValue;

    /// <summary>
    /// Minimum duration of this Mob's main action animation.
    /// </summary>
    public virtual float MIN_MAIN_ACTION_ANIMATION_DURATION => 0f;

    /// <summary>
    /// Amount of chase range this Mob starts with.
    /// </summary>
    public abstract float BASE_CHASE_RANGE { get; }

    /// <summary>
    /// Most amount of chase range this Mob can have.
    /// </summary>
    public abstract float MAX_CHASE_RANGE { get; }

    /// <summary>
    /// Least amount of chase range this Mob can have.
    /// </summary>
    public abstract float MIN_CHASE_RANGE { get; }

    /// <summary>
    /// Amount of movement speed this Mob starts with.
    /// </summary>
    public abstract float BASE_MOVEMENT_SPEED { get; }

    /// <summary>
    /// Max amount of movement speed this Mob starts with.
    /// </summary>
    public abstract float MAX_MOVEMENT_SPEED { get; }

    /// <summary>
    /// Min amount of movement speed this Mob starts with.
    /// </summary>
    public abstract float MIN_MOVEMENT_SPEED { get; }

    /// <summary>
    /// Starting duration of this Mob's movement animation.
    /// </summary>
    public abstract float BASE_MOVEMENT_ANIMATION_DURATION { get; }

    /// <summary>
    /// Maximum duration of this Mob's movement animation.
    /// </summary>
    public virtual float MAX_MOVEMENT_ANIMATION_DURATION => float.MaxValue;

    /// <summary>
    /// Minimum duration of this Mob's movement animation.
    /// </summary>
    public virtual float MIN_MOVEMENT_ANIMATION_DURATION => 0f;

    /// <summary>
    /// How much faster this Mob moves when entering the scene.
    /// </summary>
    public virtual float ENTERING_MOVEMENT_SPEED => GetMovementSpeed() * 2.5f;

    /// <summary>
    /// By default, Mobs do not occupy Tiles.
    /// </summary>
    public override bool OCCUPIER => false;

    #endregion

    #region Methods

    /// <summary>
    /// Called when this Mob activates in the scene.
    /// </summary>
    public virtual void OnSpawn() => spawned = true;

    /// <summary>
    /// Returns this Mob's spawn position in world space.
    /// </summary>
    /// <returns>the position where this Mob spawned.</returns>
    public Vector3 GetSpawnWorldPosition() => spawnPos;

    /// <summary>
    /// Sets where this Mob spawns in the scene, in world space.
    /// </summary>
    /// <param name="spawnPos">Where the mob spawns. </param>
    public void SetSpawnWorldPosition(Vector3 spawnPos) => this.spawnPos = spawnPos;

    /// <summary>
    /// Returns true if this Mob is dead. This is when the Mob
    /// has a non-positive health value and has already been
    /// spawned.
    /// </summary>
    /// <returns>true if this Mob is dead; otherwise, false.</returns>
    public override bool Dead() => GetHealth() <= 0 && spawned;

    /// <summary>
    /// Returns true if this Mob is spawned in the scene.
    /// </summary>
    /// <returns>true if this Mob is spawned in the scene.</returns>
    public bool Spawned() => spawned;

    /// <summary>
    /// Returns this Mob's current main action range.
    /// </summary>
    /// <returns>this Mob's current main action range.</returns>
    public float GetMainActionRange() => mainActionRange;

    /// <summary>
    /// Resets this Mob's main action range to its starting value.
    /// </summary>
    public void ResetMainActionRange() => mainActionRange = BASE_MAIN_ACTION_RANGE;

    /// <summary>
    /// Returns the number of seconds this Mob has to wait before
    /// performing its main action again.
    /// </summary>
    /// <returns>the number of seconds this Mob has to wait before
    /// performing its main action again.</returns>
    public float GetMainActionCooldownRemaining() => mainActionCooldownTimer;

    /// <summary>
    /// Reduces this Mob's main action cooldown by Time.deltaTime.
    /// </summary>
    public void StepMainActionCooldownByDeltaTime() => mainActionCooldownTimer = Mathf.Clamp(GetMainActionCooldownRemaining()
        - Time.deltaTime, MIN_MAIN_ACTION_SPEED, GetResetValueOfMainActionCooldown());

    /// <summary>
    /// Sets this Mob's main action cooldown to a new value. Clamped
    /// between the minimum main action speed and the reset value
    /// of the current main action speed.
    /// </summary>
    /// <param name="timeRemaining"></param>
    public void SetMainActionCooldownRemaining(float timeRemaining)
    {
        float maxCooldown = GetResetValueOfMainActionCooldown();
        mainActionCooldownTimer = Mathf.Clamp(timeRemaining, MIN_MAIN_ACTION_SPEED, maxCooldown);
    }

    /// <summary>
    /// Sets the number of times this Mob can perform its main action per second.
    /// </summary>
    public void SetMainActionSpeed(float mainActionSpeed) => this.mainActionSpeed = mainActionSpeed;

    /// <summary>
    /// Returns this Mob's current main action speed.
    /// </summary>
    /// <returns>this Mob's current main action speed.</returns>
    public float GetMainActionSpeed() => mainActionSpeed;

    /// <summary>
    /// Sets this Mob's main action animation duration to a new value.
    /// </summary>
    /// <param name="mainActionAnimationDuration">the new main action animation duration.</param>
    public void SetMainActionAnimationDuration(float mainActionAnimationDuration) => this.mainActionAnimationDuration =
        Mathf.Clamp(mainActionAnimationDuration, MIN_MAIN_ACTION_ANIMATION_DURATION, MAX_MAIN_ACTION_ANIMATION_DURATION);

    /// <summary>
    /// Returns this Mob's main action animation duration.
    /// </summary>
    /// <returns>this Mob's main action animation duration.</returns>
    public float GetMainActionAnimationDuration() => mainActionAnimationDuration;

    /// <summary>
    /// Resets this Mob's main action cooldown to its currently capped value.
    /// </summary>
    public void RestartMainActionCooldown() => mainActionCooldownTimer = GetResetValueOfMainActionCooldown();

    /// <summary>
    /// Returns the reset value of this Mob's main action cooldown.
    /// </summary>
    /// <returns>the reset value of this Mob's main action cooldown.</returns>
    public float GetResetValueOfMainActionCooldown() => 1f / mainActionSpeed;

    /// <summary>
    /// Resets this Mob's main action speed to its starting value.
    /// </summary>
    public void ResetMainActionSpeed() => mainActionSpeed = BASE_MAIN_ACTION_SPEED;

    /// <summary>
    /// Resets this Mob's main action animation duration to its starting value.
    /// </summary>
    public void ResetMainActionAnimationDuration() => mainActionAnimationDuration = BASE_MAIN_ACTION_ANIMATION_DURATION;

    /// <summary>
    /// Returns this Mob's current chase range.
    /// </summary>
    /// <returns>this Mob's current chase range.</returns>
    public virtual float GetChaseRange() => chaseRange;

    /// <summary>
    /// Resets this Mob's chase range to its starting value.
    /// </summary>
    public void ResetChaseRange() => chaseRange = BASE_CHASE_RANGE;

    /// <summary>
    /// Returns this Mob's current movement speed.
    /// </summary>
    /// <returns>this Mob's current movement speed.</returns>
    public float GetMovementSpeed() => movementSpeed;

    /// <summary>
    /// Resets this Mob's movement speed to its starting value.
    /// </summary>
    public void ResetMovementSpeed() => movementSpeed = BASE_MOVEMENT_SPEED;

    /// <summary>
    /// Sets this Mob's movement speed to a new value.
    /// </summary>
    /// <param name="movementSpeed">the new movement speed value.</param>
    public virtual void SetMovementSpeed(float movementSpeed) => this.movementSpeed = 
        Mathf.Clamp(movementSpeed, MIN_MOVEMENT_SPEED, MAX_MOVEMENT_SPEED);

    /// <summary>
    /// Returns this Mob's current movement animation duration.
    /// </summary>
    /// <returns>this Mob's current movement animation duration.</returns>
    public float GetMovementAnimationDuration() => movementAnimationDuration;

    /// <summary>
    /// Sets this Mob's movement animation duration to a new value.
    /// </summary>
    /// <param name="movementAnimationDuration">the new movement animation duration.</param>
    public void SetMovementAnimationDuration(float movementAnimationDuration) => this.movementAnimationDuration = 
        Mathf.Clamp(movementAnimationDuration, MIN_MOVEMENT_ANIMATION_DURATION, MAX_MOVEMENT_ANIMATION_DURATION);

    /// <summary>
    /// Resets this Mob's movement animation duration to its starting value.
    /// </summary>
    public void ResetMovementAnimationDuration() => movementAnimationDuration = BASE_MOVEMENT_ANIMATION_DURATION;

    /// <summary>
    /// Returns this Mob's current ability.
    /// </summary>
    /// <returns>this Mob's current ability.</returns>
    public virtual Ability GetAbility() => defaultAbility;

    /// <summary>
    /// Resets this Mob's stats to their default values.
    /// </summary>
    public override void ResetModel()
    {
        base.ResetModel();
        ResetMainActionRange();
        ResetChaseRange();
        ResetMovementSpeed();
        ResetMainActionSpeed();
        ResetMainActionAnimationDuration();
        ResetMovementAnimationDuration();
        SetLocalScale(TileGrid.TILE_SIZE, TileGrid.TILE_SIZE);
    }

    #endregion

    #region Effects

    /// <summary>
    /// Processes all IEffect instances on this Model.
    /// </summary>
    public override void ProcessEffects()
    {
        base.ProcessEffects();
        ProcessMovementSpeedEffects();
        ProcessAttackSpeedEffects();
    }

    /// <summary>
    /// Processes all IMovementSpeedEffect instances on this Model.
    /// Updates the Model's stats based on the effects.
    /// </summary>
    protected virtual void ProcessMovementSpeedEffects()
    {
        float totalSpeedModifier = 0;
        bool chilled = false;
        foreach (IEffect effect in GetEffects())
        {
            if (effect is IMovementSpeedEffect speedEffect)
            {
                totalSpeedModifier += speedEffect.MovementSpeedMagnitude;
            }
            if (effect is IceChunkEffect)
            {
                chilled = true;
            }
        }
        if (chilled) SetBaseColor(new Color32(100, 100, 255, 255));
        else SetBaseColor(BASE_COLOR);
        SetMovementSpeed(Mathf.Clamp(BASE_MOVEMENT_SPEED * (1 + totalSpeedModifier), MIN_MOVEMENT_SPEED, MAX_MOVEMENT_SPEED));
        SetMovementAnimationDuration(Mathf.Clamp(BASE_MOVEMENT_ANIMATION_DURATION / (1 + totalSpeedModifier), MIN_MOVEMENT_ANIMATION_DURATION, MAX_MOVEMENT_ANIMATION_DURATION));
    }

    /// <summary>
    /// Processes all IAttackSpeedEffect instances on this Model.
    /// Updates the Model's stats based on the effects. Note that this
    /// is specific to attacks and does not apply to all main actions.
    /// </summary>
    protected virtual void ProcessAttackSpeedEffects()
    {
        float totalAttackSpeedModifier = 0;
        foreach (IEffect effect in GetEffects())
        {
            if (effect is IAttackSpeedEffect attackSpeedEffect)
            {
                totalAttackSpeedModifier += attackSpeedEffect.AttackSpeedMagnitude;
            }
        }
        mainActionSpeed = Mathf.Clamp(BASE_MAIN_ACTION_SPEED * (1 + totalAttackSpeedModifier), MIN_MAIN_ACTION_SPEED, MAX_MAIN_ACTION_SPEED);
    }

    #endregion
}
