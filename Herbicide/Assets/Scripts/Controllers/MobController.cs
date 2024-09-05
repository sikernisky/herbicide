using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Controls a Mob. The T generic Enum represents the Mob's State. 
/// These Enums are defined in inheriting controller classes. <br></br>
/// 
/// The MobController is responsible for manipulating its Mob and bringing
/// it to life. This includes moving it, choosing targets, playing animations,
/// and more.
/// </summary>
/// <typeparam name="T">Enum to represent state of the Mob.</typeparam>
public abstract class MobController<T> : ModelController, IStateTracker<T> where T : Enum
{
    #region Fields

    /// <summary>
    /// The list of current targets. Mobs will interperet a "target"
    /// in their own way.  
    /// </summary>
    private List<Model> targets;

    /// <summary>
    /// The list of Models the Mob is holding. 
    /// </summary>
    private List<Model> modelsHolding;

    /// <summary>
    /// The color strength, from 0-1, of the damage flash animation.
    /// </summary>
    private const float FLASH_INTENSITY = .4f;

    /// <summary>
    /// The total time in seconds, from start to finish, of a damage flash
    /// animation.
    /// </summary>
    private const float FLASH_DURATION = .4f;

    /// <summary>
    /// The maximum number of targets the controller can have at once.
    /// </summary>
    protected abstract int MAX_TARGETS { get; }

    /// <summary>
    /// The number of targets this Enemy is allowed to pick up at once.
    /// </summary>
    protected virtual int HOLDING_LIMIT => 1;

    /// <summary>
    /// The Mob's state.
    /// </summary>
    private T state;

    /// <summary>
    /// Where this Mob moves next.
    /// </summary>
    private Vector3? nextMovePos;

    /// <summary>
    /// All the attack speed buff multipliers currently affecting this Mob.
    /// </summary>
    private HashSet<float> attackSpeedBuffMultipliers;

    /// <summary>
    /// true if the Mob controlled by this MobController
    /// needs to parse through all possible targets. This exists
    /// so that we don't need to go through targeting logic for Mobs
    /// that will never use them: StoneWall, for example.
    /// </summary>
    protected virtual bool FINDS_TARGETS => false;

    #endregion

    #region Methods

    /// <summary>
    /// Makes a new MobController for a Mob.
    /// </summary>
    /// <param name="mob">The Mob controlled by this MobController.</param>
    public MobController(Mob mob) : base(mob)
    {
        Assert.IsNotNull(mob, "Mob cannot be null.");
        targets = new List<Model>();
        modelsHolding = new List<Model>();
        attackSpeedBuffMultipliers = new HashSet<float>();
        SpawnMob();
    }

    /// <summary>
    /// Main update loop for the MobController. Updates its model.
    /// </summary>
    /// <param name="gameState">The most recent GameState.</param>
    public override void UpdateController(GameState gameState)
    {
        base.UpdateController(gameState);
        UpdateDamageFlash();
        UpdateMob();
        UpdateStateFSM();
    }

    /// <summary>
    /// Main update loop for the MobController's Mob logic. 
    /// </summary>
    protected virtual void UpdateMob()
    {
        if (!ValidModel()) return;
        if (FINDS_TARGETS) UpdateTargets(GetAllModels(), TileGrid.GetAllTiles());
        GetMob().StepAttackCooldown();
        CalculateAndApplyBuffs();
    }

    /// <summary>
    /// Returns this MobController's Mob model. Inheriting controller
    /// classes use this method to access their Mob; then, they cast
    /// it to its respective type.
    /// </summary>
    /// <returns>this MobController's Mob model.</returns>
    protected Mob GetMob() => GetModel() as Mob;

    /// <summary>
    /// Animates this Controller's model's damage flash effect if it is
    /// playing.
    /// </summary>
    private void UpdateDamageFlash()
    {
        if (GetMob() == null || System.Object.Equals(null, GetMob())) return;

        float remainingFlashTime = GetMob().TimeRemaningInFlashAnimation();
        if (remainingFlashTime <= 0)
        {
            GetModel().SetColor(Color.white); // Explicitly set to white when the flash time ends
            return;
        }
        float newDamageFlashingTime = Mathf.Clamp(remainingFlashTime - Time.deltaTime, 0, FLASH_DURATION);
        GetMob().SetRemainingFlashAnimationTime(newDamageFlashingTime);

        // Simplified lerp target calculation
        float lerpFactor = Mathf.Cos((Mathf.PI * remainingFlashTime) / FLASH_DURATION);
        lerpFactor = Mathf.Clamp(lerpFactor, 0, 1);

        float score = Mathf.Lerp(FLASH_INTENSITY, 1f, lerpFactor);
        byte greenBlueComponent = (byte)(score * 255);
        Color32 color = new Color32(255, greenBlueComponent, greenBlueComponent, 255);
        GetModel().SetColor(color);
    }

    /// <summary>
    /// Returns the closest position from the Mob to some PlaceableObject.<br></br>
    /// 
    /// This is needed because some Models are larger than 1x1. So traditional distance
    /// calculations don't work anymore because they always used bottom left corner.
    /// For example: <br></br>
    /// 
    /// [][]        [][]
    /// [][x]       [x][]
    /// [][] 
    /// <br></br>
    ///</summary>
    /// <param name="target"></param>
    /// <returns></returns>
    protected Vector3 ClosestPositionToTarget(PlaceableObject target)
    {
        float minDistance = float.MaxValue;
        Vector2Int closestPosition = new Vector2Int();
        Vector3 mobPosition = GetMob().transform.position;  // Get the Mob's position

        foreach (var thisCoord in GetMob().GetExpandedTileCoordinates())
        {
            foreach (var targetCoord in target.GetExpandedTileCoordinates())
            {
                float distance = Vector2Int.Distance(thisCoord, targetCoord);
                Vector3 convertedThis = new Vector3(thisCoord.x, thisCoord.y, 1);
                Vector3 targetPosition = new Vector3(targetCoord.x, targetCoord.y, 1);

                if (distance < minDistance && TileGrid.CanReach(mobPosition, targetPosition))
                {
                    minDistance = distance;
                    closestPosition = targetCoord;
                }
            }
        }

        return new Vector3(closestPosition.x, closestPosition.y, 1);
    }

    /// <summary>
    /// Brings this Controller's Mob into life by calling its OnSpawn()
    /// method. 
    /// </summary>
    protected virtual void SpawnMob()
    {
        GetMob().RefreshRenderer();
        GetMob().OnSpawn();
        GetMob().gameObject.SetActive(true);
    }

    /// <summary>
    /// Adds an attack speed multiplier to the Mob.
    /// </summary>
    /// <param name="multiplier">The attack speed multiplier.</param>
    protected void BuffAttackSpeed(float multiplier)
    {
        Assert.IsTrue(multiplier > 0, "Multiplier must be positive.");
        attackSpeedBuffMultipliers.Add(multiplier);
    }

    /// <summary>
    /// Calculates the combined multiplier for each buff type and applies it to the Mob.
    /// Clears each list of multipliers after applying them.
    /// </summary>
    private void CalculateAndApplyBuffs()
    {
        // Attack speed
        float combinedAttackSpeedBuff = 1f;
        foreach (float multiplier in attackSpeedBuffMultipliers)
        {
            combinedAttackSpeedBuff *= multiplier;
        }
        GetMob().SetAttackSpeed(GetMob().BASE_ATTACK_SPEED * combinedAttackSpeedBuff);
        attackSpeedBuffMultipliers.Clear();

        // Put other buffs here
    }

    /// <summary>
    /// Returns true if the Mob can attack this frame.
    /// </summary>
    /// <returns>true if the Mob can attack this frame; 
    /// otherwise, false. </returns>
    public virtual bool CanAttack() { return GetMob().GetAttackCooldown() <= 0; }

    #endregion

    #region State Logic

    /// <summary>
    /// Sets the State of this MobController. This helps keep track of
    /// what its Mob should do and what it is doing, and it is essential
    /// for the FSM logic. 
    /// </summary>
    /// <param name="state">The State to set.</param>
    public void SetState(T state) => this.state = state;

    /// <summary>
    /// Returns the State of this MobController. This helps keep track of
    /// what its Mob should do and what it is doing, and it is essential
    /// for the FSM logic. 
    /// </summary>
    /// <returns>The State of this MobController. </returns>
    public T GetState() => state;

    /// <summary>
    /// Processes this MobController's state FSM to determine the
    /// correct state. Takes the current state and chooses whether
    /// or not to switch to another based on game conditions. /// 
    /// </summary>
    public abstract void UpdateStateFSM();

    /// <summary>
    /// Returns true if two states are equal.
    /// </summary>
    /// <param name="stateA">The first state.</param>
    /// <param name="stateB">The second state</param>
    /// <returns>true if two states are equal; otherwise, false. </returns>
    public abstract bool StateEquals(T stateA, T stateB);

    #endregion

    #region Targeting Logic

    /// <summary>
    /// Returns a copy of the list of this MobController's current targets. Mobs interperet
    /// targets differently; for example, a Squirrel will attack its target,
    /// but some other Mob might heal its target.
    /// 
    /// TODO: Add priority logic, put the most prio targets at the front of the list.
    /// This works with mobs who have multiple targets. 
    /// 
    /// 
    /// </summary>
    /// <returns>this MobController's target.</returns>
    protected List<Model> GetTargets() => new List<Model>(targets);

    /// <summary>
    /// Returns the Mob's most pressing target.
    /// </summary>
    /// <returns>the Mob's most pressing target.</returns>
    protected virtual Model GetTarget() => targets.Count == 0 ? null : targets[0];

    /// <summary>
    /// Clears the Mob's list of targets.
    /// </summary>
    private void ClearTargets() => targets.Clear();

    /// <summary>
    /// Adds a Model to this Mob's list of targets.
    /// </summary>
    /// <param name="targetable">The target to add.</param>
    private void AddTarget(Model targetable)
    {
        Assert.IsNotNull(targetable, "Target cannot be null.");
        Assert.IsFalse(NumTargets() >= MAX_TARGETS, GetMob() +
            " already has " + NumTargets() + " targets.");
        Assert.IsTrue(CanTargetModel(targetable), "Not a valid target.");

        targets.Add(targetable);
    }

    /// <summary>
    /// Runs through all targetable Models and adds each one
    /// this Mob can target to its targets list.
    /// </summary>
    /// <param name="nonTiles">All targetable Models that are not Tiles..</param>
    /// <param name="tiles">All targetable Models that are Tiles.</param>
    private void UpdateTargets(IReadOnlyList<Model> nonTiles, IReadOnlyList<Model> tiles)
    {
        Assert.IsNotNull(nonTiles, "List of targets is null.");

        ClearTargets();

        foreach (Model targetable in nonTiles)
        {
            if (CanTargetModel(targetable) && NumTargets() < MAX_TARGETS)
            {
                AddTarget(targetable);
            }
        }

        foreach (Model targetable in tiles)
        {
            if (CanTargetModel(targetable) && NumTargets() < MAX_TARGETS)
            {
                AddTarget(targetable);
            }
        }

        SortTargets(targets);
    }

    /// <summary>
    /// Sorts the list of targets this Mob has elected to target by
    /// priority. This method is called after UpdateTargets() and
    /// before the Mob acts on its targets.
    /// </summary>
    /// <param name="targets">The list of targets to sort.</param>
    protected virtual void SortTargets(List<Model> targets) { }

    /// <summary>
    /// Returns true if the Mob is allowed to target a Model passed
    /// into this method.
    /// </summary>
    /// <param name="target">A potential target.</param>
    /// <returns>true if the Mob is allowed to target a Model; otherwise,
    /// false.</returns>
    protected abstract bool CanTargetModel(Model target);

    /// <summary>
    /// Returns the current number of targets the Mob has
    /// elected. 
    /// </summary>
    /// <returns>the current number of targets the Mob has
    /// elected.</returns>
    protected int NumTargets() => targets.Count;

    /// <summary>
    /// Returns true if the Mob can hold some Nexus.
    /// </summary>
    /// <returns>true if the Mob can hold the Nexus; otherwise,
    /// false. </returns>
    /// <param name="target">The target to check. </param> 
    protected virtual bool CanHoldTarget(Nexus target)
    {
        if (target == null) return false;
        if (!GetTargets().Contains(target)) return false; // Need to target before holding
        if (NumTargetsHolding() >= HOLDING_LIMIT) return false;
        if (target.PickedUp()) return false;
        if (target.CashedIn()) return false;

        return true;
    }

    /// <summary>
    /// Adds a target to the list of targets the Mob is holding.
    /// </summary>
    /// <param name="target">The target to hold. </param> 
    protected virtual void HoldTarget(Nexus target)
    {
        Assert.IsNotNull(modelsHolding, "List of holding targets is null.");
        Assert.IsTrue(CanHoldTarget(target), "Need to check holding validity.");

        modelsHolding.Add(target);
        target.SetPickedUp(GetMob(), GetMob().HOLDER_OFFSET);

    }

    /// <summary>
    /// Returns the number of targets the Mob is currently holding.
    /// </summary>
    /// <returns>the number of targets the Mob is currently holding.</returns>
    protected int NumTargetsHolding()
    {
        Assert.IsNotNull(modelsHolding, "List of holding targets is null.");

        return modelsHolding.Count;
    }

    /// <summary>
    /// Returns a copy of the list of targets the Mob is currently holding.
    /// </summary>
    /// <returns>a copy of the list of targets the Mob is currently holding.</returns>
    protected List<Model> GetHeldTargets() => new List<Model>(modelsHolding);

    /// <summary>
    /// Returns the distance from this Controller's Mob to its first target.
    /// </summary>
    /// <returns>the distance from this Controller's Mob to its first target. </returns>
    protected float DistanceToTarget()
    {
        Assert.IsNotNull(GetTargets());

        if (GetTargets().Count == 0) return float.MaxValue;

        Model target = GetTargets()[0];
        Assert.IsNotNull(target);

        return Vector2.Distance(GetModel().GetPosition(), target.GetPosition());
    }

    /// <summary>
    /// Makes the Mob face its first target.
    /// </summary>
    protected void FaceTarget()
    {
        if (GetTargets().Count == 0) return;
        Model target = GetTarget();
        if (target != null) GetMob().FaceDirection(target.GetPosition());
    }

    #endregion

    #region Movement Logic

    /// <summary>
    /// Moves the Mob towards the next movement position in a linear manner.
    /// </summary>
    protected void MoveLinearlyTowardsMovePos()
    {
        if (GetNextMovePos() == null) return;
        MoveLinearlyTowards(GetNextMovePos().Value, GetMob().GetMovementSpeed());
    }

    /// <summary>
    /// Moves the Mob towards the next movement position in manner such that it
    /// looks like it is falling into a hole.
    /// </summary>
    /// <param name="acceleration">How fast to accelerate towards the movement target.</param>

    protected void FallIntoMovePos(float acceleration)
    {
        if (GetNextMovePos() == null) return;
        FallTowards(GetNextMovePos().Value, GetMob().GetMovementSpeed(), acceleration);
    }

    /// <summary>
    /// Moves the Mob towards the next movement position in manner such that it
    /// looks like it popping out of a hole.
    /// </summary>
    /// <param name="startPosition">Where the mob is popping from.</param>
    protected void PopOutOfMovePos(Vector3 startPosition)
    {
        if (GetNextMovePos() == null) return;
        PopFrom(startPosition, GetNextMovePos().Value, GetMob().GetMovementSpeed());
    }

    /// <summary>
    /// Moves the Mob towards the next movement position in a parabolic manner.
    /// </summary>
    protected void MoveParabolicallyTowardsMovePos()
    {
        if (GetNextMovePos() == null) return;
        MoveParabolicallyTowards(GetNextMovePos().Value, GetMob().GetMovementSpeed());
    }

    /// <summary>
    /// Sets where the Mob should move next.
    /// </summary>
    /// <param name="nextPos">where the Mob should move next.</param>
    protected virtual void SetNextMovePos(Vector3? nextPos)
    {
        nextMovePos = nextPos;

        Vector3 newParabolicTarget = new Vector3(nextMovePos.Value.x, nextMovePos.Value.y, 1);
        float newParabolicScale = GetMob().GetMovementSpeed() /
            Vector3.Distance(GetMob().GetPosition(), newParabolicTarget);

        ResetParabolicFields(newParabolicScale, newParabolicTarget);
    }

    /// <summary>
    /// Returns where the Mob should move next.
    /// </summary>
    /// <returns>where the Mob should move next.</returns>
    protected Vector3? GetNextMovePos() { return nextMovePos; }

    /// <summary>
    /// Returns true if the Mob's position is at the spot it
    /// is trying to move towards.
    /// </summary>
    /// <returns>true if the Mob made it to its movement destination;
    /// otherwise, false. </returns>
    protected bool ReachedMovementTarget()
    {
        if (GetNextMovePos() == null) return true;

        Vector3 nextMovePos = new Vector3(
            GetNextMovePos().Value.x,
            GetNextMovePos().Value.y,
            1
        );

        return Vector3.Distance(GetMob().GetPosition(), nextMovePos) < 0.1f;
    }

    #endregion
}
