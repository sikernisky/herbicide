using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Represents a Model who works to make the player lose the game.
/// </summary>
public abstract class Enemy : Mob
{
    #region Fields

    /// <summary>
    /// State of an Enemy based on its health.
    /// </summary>
    public enum EnemyHealthState
    {
        HEALTHY,
        DAMAGED,
        CRITICAL
    }

    /// <summary>
    /// Reference to the Enemy's health bar background SpriteRenderer component.
    /// </summary>
    [SerializeField]
    private SpriteRenderer healthBarBackground;

    /// <summary>
    /// Reference to the Enemy's health bar SpriteRenderer component.
    /// </summary> 
    [SerializeField]
    private SpriteRenderer healthBarFill;

    /// <summary>
    /// The time at which this Enemy spawns.
    /// </summary>
    private float spawnTime;

    /// <summary>
    /// Current health state of this Enemy.
    /// </summary>
    private EnemyHealthState healthState;

    /// <summary>
    /// true if the Enemy exited; otherwise, false.
    /// </summary>
    private bool exited;

    /// <summary>
    /// true if the Enemy is exiting; otherwise, false.
    /// </summary>
    private bool exiting;

    /// <summary>
    /// The position of the structure the Enemy is using to exit.
    /// </summary>
    private Vector3 exitPos;

    /// <summary>
    /// true if this Enemy is currently entering; otherwise, false.
    /// </summary>
    private bool entering;

    /// <summary>
    /// true if the Enemy entered; otherwise, false.
    /// </summary>
    private bool entered;

    /// <summary>
    /// the position where the Enemy is entering towards
    /// </summary>
    private Vector3 enteringTowardsPos;

    /// <summary>
    /// Set of active DamageOverTime effects on this Enemy.
    /// </summary>
    private HashSet<DamageOverTime> activeDOTs;

    /// <summary>
    /// The DamageOverTime effects applied to this Enemy this cycle.
    /// </summary>
    private HashSet<DamageOverTime.DOTType> appliedDOTsThisCycle;

    /// <summary>
    /// The Quills stuck in this Enemy.
    /// </summary>
    private HashSet<Quill> quillsStuckInEnemy;

    #endregion

    #region Stats

    /// <summary>
    /// How much a currency collectable dropped by this Enemy on death
    /// is worth.
    /// </summary>
    public virtual int CURRENCY_VALUE_ON_DEATH => 5;

    /// <summary>
    /// The number of lives lost when this Enemy exits.
    /// </summary>
    public virtual int LIVES_LOST_ON_EXIT => 1;

    #endregion

    #region Methods

    /// <summary>
    /// Called when this Enemy appears on the TileGrid and is assigned
    /// an EnemyController (or a subclass of EnemyController).
    /// </summary>
    public override void OnSpawn()
    {
        Assert.IsTrue(ReadyToSpawn(), "Not Ready.");
        base.OnSpawn();

        ToggleHealthBar(false);
        ModelCollider.enabled = true;
        activeDOTs = new HashSet<DamageOverTime>();
        appliedDOTsThisCycle = new HashSet<DamageOverTime.DOTType>();
    }

    /// <summary>
    /// Returns true if this Enemy is due to spawn and it has not yet spawned.
    /// </summary>
    /// <returns>true if this Enemy is due to spawn and it has not yet spawned;
    /// otherwise, returns false. /// </returns>
    public override bool ReadyToSpawn()
    {
        if(!base.ReadyToSpawn()) return false;
        bool timeToSpawn = Time.timeSinceLevelLoad >= GetSpawnTime();
        return timeToSpawn;
    }

    /// <summary>
    /// Updates this Enemy's HealthState based on its current health.
    /// </summary>
    public void UpdateHealthState()
    {
        float currentPercent = (float)GetHealth() / (float)MaxHealth;
        if (currentPercent >= .66f) healthState = EnemyHealthState.HEALTHY;
        else if (currentPercent >= .33f) healthState = EnemyHealthState.DAMAGED;
        else healthState = EnemyHealthState.CRITICAL;
    }

    /// <summary>
    /// Returns the current HealthState of this Enemy.
    /// </summary>
    /// <returns>the current HealthState of this Enemy.</returns>
    public EnemyHealthState GetHealthState() => healthState;

    /// <summary>
    /// Sets the SpawnTime of this Enemy.
    /// </summary>
    /// <param name="time">the time at which this Enemy spawns.</param>
    public void SetSpawnTime(float time) => spawnTime = Mathf.Clamp(time, 0, float.MaxValue);

    /// <summary>
    /// Returns this Enemy's SpawnTime.
    /// </summary>
    /// <returns>this Enemy's SpawnTime.</returns>
    public float GetSpawnTime() => spawnTime;

    /// <summary>
    /// Sets the Enemy as entering.
    /// </summary>
    /// <param name="enteringTowardsPos">The position where the Enemy is entering towards.</param>
    public void SetEntering(Vector3 enteringTowardsPos)
    {
        entered = false;
        entering = true;
        ToggleHealthBar(true);
        SetMaskInteraction(SpriteMaskInteraction.VisibleOutsideMask);
        this.enteringTowardsPos = enteringTowardsPos;
    }

    /// <summary>
    /// Sets the Enemy as entered. 
    /// </summary>
    public void SetEntered()
    {
        entering = false;
        entered = true;
        SetMaskInteraction(SpriteMaskInteraction.None);
    }

    /// <summary>
    /// Returns true if this Enemy is entering.
    /// </summary>
    /// <returns> true if this Enemy is entering; otherwise, false. </returns>
    public bool IsEntering() => entering;

    /// <summary>
    /// Returns true if this Enemy has entered.
    /// </summary>
    /// <returns>true if this Enemy has entered; otherwise, false. </returns>
    public bool IsEntered() => entered; 

    /// <summary>
    /// Returns the position where the Enemy is entering towards.
    /// </summary>
    /// <returns>the position where the Enemy is entering towards.</returns>
    public Vector3 GetEnterPos() => enteringTowardsPos;

    /// <summary>
    /// Returns true if this Enemy escaped.
    /// </summary>
    /// <returns>true if this Enemy escaped; otherwise, false. </returns>
    public bool IsEscaped() => exited;

    /// <summary>
    /// Sets this Enemy as escaped.
    /// </summary>
    public void SetEscaped()
    {
        exiting = false;
        exited = true;
    }

    /// <summary>
    /// Sets the Enemy as exiting, but not yet exited.
    /// </summary>
    /// <param name="exitPos">The position of the structure the
    /// Enemy is using to exit. /// </param>
    public void SetExiting(Vector3 exitPos)
    {
        exited = false;
        exiting = true;
        ToggleHealthBar(false);
        this.exitPos = exitPos;
        SetMaskInteraction(SpriteMaskInteraction.VisibleOutsideMask);
    }

    /// <summary>
    /// Returns true if this Enemy is exiting.
    /// </summary>
    /// <returns>true if this Enemy is exiting; otherwise, false. </returns>
    public bool IsExiting() => exiting;

    /// <summary>
    /// Returns the position of the structure the Enemy is
    /// using to exit.
    /// </summary>
    /// <returns>the position of the structure the Enemy is
    /// using to exit.</returns>
    public Vector3 GetExitPos() => exitPos;

    /// <summary>
    /// Returns the position where Models attacking this
    /// Enemy should aim.
    /// </summary>
    /// <returns>the position where Models attacking this
    /// Enemy should aim.</returns>
    public override Vector3 GetAttackPosition()
    {
        if (IsEntering())
        {
            Debug.Log("returning " + GetEnterPos());
            return GetEnterPos();
        }
        return base.GetAttackPosition();
    }

    /// <summary>
    /// Adjusts this Mob's health by some amount. Updates the health bar.
    /// </summary>
    /// <param name="amount">The amount to adjust by. </param>
    public override void AdjustHealth(float amount)
    {
        base.AdjustHealth(amount);
        float healthPercentage = (float)GetHealth() / BaseHealth;
        healthBarFill.transform.localScale = new Vector3(healthPercentage, 1, 1);
    }

    /// <summary>
    /// Adjusts this Mob's health by some amount over time. Updates the health bar.
    /// </summary>
    /// <param name="amount">The amount to adjust by. </param>
    public void AdjustHealth(DamageOverTime dot)
    {
        Assert.IsNotNull(dot, "DamageOverTime is null.");

        activeDOTs.Add(dot);
    }

    /// <summary>
    /// Updates the DamageOverTime effects on this Enemy.
    /// </summary>
    public void UpdateDamageOverTimeEffects()
    {
        if (activeDOTs == null) activeDOTs = new HashSet<DamageOverTime>();
        if (appliedDOTsThisCycle == null) appliedDOTsThisCycle = new HashSet<DamageOverTime.DOTType>();

        foreach (DamageOverTime dot in activeDOTs)
        {
            bool shouldAdjustHealth = dot.UpdateDamageOverTime();

            if (!dot.DoesStack())
            {
                if (appliedDOTsThisCycle.Contains(dot.GetDOTType())) continue;
                appliedDOTsThisCycle.Add(dot.GetDOTType());
            }

            if (shouldAdjustHealth) AdjustHealth(-dot.GetDamage());
        }

        activeDOTs.RemoveWhere(dot => dot.IsFinished());
        appliedDOTsThisCycle.Clear();
    }

    /// <summary>
    /// Toggle the visibility of the Enemy's health bar.
    /// </summary>
    public void ToggleHealthBar(bool showHealthBar)
    {
        Assert.IsNotNull(healthBarBackground, "HealthBarBackground is null.");
        Assert.IsNotNull(healthBarFill, "HealthBarFill is null.");
        healthBarBackground.enabled = showHealthBar;
        healthBarFill.enabled = showHealthBar;
    }

    /// <summary>
    /// Returns true if the Enemy's health bar is visible; otherwise, false.
    /// </summary>
    /// <returns>true if the Enemy's health bar is visible; otherwise, false.</returns>
    public bool IsHealthBarVisible() => healthBarBackground.enabled || healthBarFill.enabled;

    /// <summary>
    /// Sticks a Quill in this Enemy.
    /// </summary>
    /// <param name="quill">the Quill with which to stick this Enemy. </param>
    public void StickWithQuill(Quill quill)
    {
        Assert.IsNotNull(quill, "Quill is null.");
        if (quillsStuckInEnemy == null) quillsStuckInEnemy = new HashSet<Quill>();
        Assert.IsFalse(quillsStuckInEnemy.Contains(quill), "Quill already stuck in enemy.");
        quillsStuckInEnemy.Add(quill);
    }

    /// <summary>
    /// Returns a copy of the set of Quills stuck in this Enemy as a list.
    /// </summary>
    /// <returns>a copy of the set of Quills stuck in this Enemy as a list.</returns>
    public List<Quill> GetQuillsStuckInEnemy()
    {
        if (quillsStuckInEnemy == null) return new List<Quill>();
        else return new List<Quill>(quillsStuckInEnemy);
    }

    /// <summary>
    /// Removes all Quills stuck in this Enemy by setting them to exploded.
    /// Exploded Quills are not valid and will be removed by their
    /// QuillControllers.
    /// </summary>
    public void RemoveQuills()
    {
        if (quillsStuckInEnemy == null) return;
        foreach (Quill quill in quillsStuckInEnemy)
        {
            if (quill == null) continue;
            quill.SetExploded();
        }
    }

    /// <summary>
    /// Returns true if this Enemy is targetable; otherwise, false.
    /// </summary>
    /// <returns>true if this Enemy is targetable; otherwise, false.</returns>
    public override bool Targetable()
    {
        if (IsEntering() || !IsEntered()) return false;
        return base.Targetable();
    }

    /// <summary>
    /// Returns a fresh copy of this Enemy from the object pool.
    /// </summary>
    /// <returns>a fresh copy of this Enemy from the object pool.</returns>
    public override GameObject CreateNew() => EnemyFactory.GetEnemyPrefab(TYPE);

    /// <summary>
    /// Returns the sprite that represents this Enemy when placing.
    /// </summary>
    /// <returns>the sprite that represents this Enemy when placing.</returns>
    public override Sprite[] GetPlacementTrack() => EnemyFactory.GetPlacementTrack(TYPE);

    #endregion
}

