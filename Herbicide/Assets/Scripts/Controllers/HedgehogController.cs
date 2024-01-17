using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Controls a Hedgehog.
/// 
/// The HedgehogController is responsible for manipulating its Hedgehog and bringing
/// it to life. This includes moving it, choosing targets, playing animations,
/// and more.
/// </summary>
public class HedgehogController : DefenderController<HedgehogController.HedgehogState>
{
    /// <summary>
    /// Maximum number of targets a Hedgehog can have.
    /// </summary>
    protected override int MAX_TARGETS => 1;

    /// <summary>
    /// All targets the Hedgehog has attacked.
    /// </summary>
    private HashSet<Model> attackedTargets;

    /// <summary>
    /// Counts the number of seconds in the idle animation; resets
    /// on step.
    /// </summary>
    private float idleAnimationCounter;

    /// <summary>
    /// Counts the number of seconds in the attack animation; resets
    /// on step.
    /// </summary>
    private float attackAnimationCounter;

    /// <summary>
    /// States of a Hedgehog.
    /// </summary>
    public enum HedgehogState
    {
        SPAWN,
        IDLE,
        ATTACK
    }


    /// <summary>
    /// Assigns a Hedgehog a controller.
    /// </summary>
    /// <param name="hedgehog">The Hedgehog to assign. </param>
    public HedgehogController(Hedgehog hedgehog) : base(hedgehog)
    {
        attackedTargets = new HashSet<Model>();
    }

    /// <summary>
    /// Main update loop for the Hedgehog.
    /// </summary>
    protected override void UpdateMob()
    {
        base.UpdateMob();
        if (!ValidModel()) return;

        ExecuteSpawnState();
        ExecuteIdleState();
        ExecuteAttackState();
    }

    /// <summary>
    /// Returns true if the Hedgehog can target the Model passed
    /// into this method.
    /// </summary>
    /// <param name="target">The Placeable object to check for targetability.</param>
    /// <returns>true if the Hedgehog can target the Model; otherwise, false. </returns>
    protected override bool CanTarget(Model target)
    {
        GrassTile grassTile = target as GrassTile;
        if (grassTile == null) return false;
        if (grassTile.Occupied()) return false;
        if (grassTile.HostsNexusHole()) return false;
        if (grassTile.Floored()) return false;
        if (GetHedgehog().DistanceToTargetFromTree(grassTile) > GetHedgehog().GetAttackRange()) return false;
        if (attackedTargets.Contains(target)) return false;

        return true;
    }

    /// <summary>
    /// Returns the Hedgehog model.
    /// </summary>
    /// <returns>this controller's Hedgehog model.</returns>
    protected Hedgehog GetHedgehog() { return GetMob() as Hedgehog; }

    /// <summary>
    /// Returns the Hedgehog's target if it has one. 
    /// </summary>
    /// <returns>the Hedgehog's target; null if it doesn't have one.</returns>
    private GrassTile GetTarget() { return NumTargets() == 1 ? GetTargets()[0] as GrassTile : null; }

    //--------------------BEGIN STATE LOGIC----------------------//

    /// <summary>
    /// Returns true if two HedgehogStates are equal.
    /// </summary>
    /// <param name="stateA">The first state.</param>
    /// <param name="stateB">The second state.</param>
    /// <returns>true if two HedgehogStates are equal; otherwise, false.</returns>
    public override bool StateEquals(HedgehogState stateA, HedgehogState stateB)
    {
        return stateA == stateB;
    }


    /// <summary>
    /// Updates the state of this HedgehogController's Hedgehog model.
    /// The transitions are: <br></br>
    /// 
    /// SPAWN --> IDLE : always <br></br>
    /// IDLE --> ATTACK : if target in range <br></br>
    /// ATTACK --> IDLE : if no target in range
    /// </summary>
    public override void UpdateStateFSM()
    {
        if (!ValidModel()) return;

        switch (GetState())
        {
            case HedgehogState.SPAWN:
                if (SpawnStateDone()) SetState(HedgehogState.IDLE);
                break;
            case HedgehogState.IDLE:
                if (GetTarget() == null || GetTarget().Occupied()) break;
                if (GetHedgehog().DistanceToTargetFromTree(GetTarget()) > GetHedgehog().GetAttackRange()) break;
                if (GetHedgehog().GetAttackCooldown() > 0) break;
                SetState(HedgehogState.ATTACK);
                break;
            case HedgehogState.ATTACK:
                if (GetTarget() == null || GetTarget().Occupied()) SetState(HedgehogState.IDLE);
                if (GetAnimationCounter() > 0) break;
                if (GetHedgehog().GetAttackCooldown() > 0) SetState(HedgehogState.IDLE);
                else if (GetHedgehog().DistanceToTarget(GetTarget())
                    > GetHedgehog().GetAttackRange()) SetState(HedgehogState.IDLE);
                break;
        }
    }
    /// <summary>
    /// Runs logic relevant to the Hedgehog's spawn state.
    /// </summary>
    protected override void ExecuteSpawnState()
    {
        if (!ValidModel()) return;
        if (GetState() != HedgehogState.SPAWN) return;

        GetHedgehog().FaceDirection(Direction.SOUTH);
        base.ExecuteSpawnState();
    }

    /// <summary>
    /// Runs logic relevant to the Hedgehog's idle state.
    /// </summary>
    protected virtual void ExecuteIdleState()
    {
        if (!ValidModel()) return;
        if (GetState() != HedgehogState.IDLE) return;

        if (GetGameState() != GameState.ONGOING)
        {
            SetState(HedgehogState.IDLE);
            return;
        }

        if (GetTarget() != null) GetHedgehog().FaceTarget(GetTarget());
        Direction direction = GetHedgehog().GetDirection();
        Sprite[] idleTrack = HedgehogFactory.GetIdleTrack(direction);
        GetHedgehog().SetAnimationTrack(idleTrack);
        if (GetAnimationState() != HedgehogState.IDLE) GetHedgehog().SetAnimationTrack(idleTrack);
        else GetHedgehog().SetAnimationTrack(idleTrack, GetHedgehog().CurrentFrame);
        SetAnimationState(HedgehogState.IDLE);

        //Step the animation.
        StepAnimation();
        GetHedgehog().SetSprite(GetHedgehog().GetSpriteAtCurrentFrame());
    }

    /// <summary>
    /// Runs logic relevant to the Squirrel's attacking state.
    /// </summary>
    protected virtual void ExecuteAttackState()
    {
        if (!ValidModel()) return;
        if (GetTarget() == null) return;
        if (GetState() != HedgehogState.ATTACK) return;

        // Animation Logic.
        GetHedgehog().FaceTarget(GetTarget());

        float duration = Mathf.Min(GetHedgehog().ATTACK_ANIMATION_DURATION,
            GetHedgehog().GetAttackCooldownCap());
        GetHedgehog().SetAnimationDuration(duration);
        Direction direction = GetHedgehog().GetDirection();
        Sprite[] attackTrack = HedgehogFactory.GetAttackTrack(direction);
        GetHedgehog().SetAnimationTrack(attackTrack);
        SetAnimationState(HedgehogState.ATTACK);
        GetHedgehog().SetSprite(GetHedgehog().GetSpriteAtCurrentFrame());
        StepAnimation();

        if (!CanAttack()) return;

        // Make a BasicTreeSeed and a BasicTreeSeedController.
        GameObject basicTreeSeedPrefab = BasicTreeSeedFactory.GetBasicTreeSeedPrefab();
        Assert.IsNotNull(basicTreeSeedPrefab);
        GameObject clonedBasicTreeSeed = GameObject.Instantiate(basicTreeSeedPrefab);
        Assert.IsNotNull(clonedBasicTreeSeed);
        BasicTreeSeed clonedBasicTreeSeedComp = clonedBasicTreeSeed.GetComponent<BasicTreeSeed>();
        Assert.IsNotNull(clonedBasicTreeSeedComp);
        Vector3 targetPosition = GetTarget().GetAttackPosition();
        BasicTreeSeedController basicTreeSeedController = new BasicTreeSeedController(
            clonedBasicTreeSeedComp, GetHedgehog().GetPosition(), targetPosition);
        AddModelControllerForExtrication(basicTreeSeedController);
        attackedTargets.Add(GetTarget());


        // Reset attack animation.
        GetHedgehog().ResetAttackCooldown();
    }

    /// <summary>
    /// Returns true if the Squirrel can shoot an acorn.
    /// </summary>
    /// <returns>true if the Squirrel can shoot an acorn; otherwise,
    /// false.</returns>
    public override bool CanAttack()
    {
        if (!base.CanAttack()) return false; //Cooldown
        if (GetState() != HedgehogState.ATTACK) return false; //Not in the attack state.
        if (GetTarget() == null) return false; //Invalid target.
        return true;
    }


    /// <summary>
    /// Handles a collision between the Hedgehog and some other 2D collider.
    /// </summary>
    /// <param name="other">The 2D Collider the Hedgehog collided with. </param>
    protected override void HandleCollision(Collider2D other) { return; }

    /// <summary>
    /// Adds one chunk of Time.deltaTime to the animation
    /// counter that tracks the current state.
    /// </summary>
    public override void AgeAnimationCounter()
    {
        HedgehogState state = GetState();
        if (state == HedgehogState.IDLE) idleAnimationCounter += Time.deltaTime;
        else if (state == HedgehogState.ATTACK) attackAnimationCounter += Time.deltaTime;
    }

    /// <summary>
    /// Returns the animation counter for the current state.
    /// </summary>
    /// <returns>the animation counter for the current state.</returns>
    public override float GetAnimationCounter()
    {
        HedgehogState state = GetState();
        if (state == HedgehogState.IDLE) return idleAnimationCounter;
        else if (state == HedgehogState.ATTACK) return attackAnimationCounter;
        else throw new System.Exception("State " + state + " has no counter.");
    }

    /// <summary>
    /// Sets the animation counter for the current state to 0.
    /// </summary>
    public override void ResetAnimationCounter()
    {
        HedgehogState state = GetState();
        if (state == HedgehogState.IDLE) idleAnimationCounter = 0;
        else if (state == HedgehogState.ATTACK) attackAnimationCounter = 0;
    }
}
