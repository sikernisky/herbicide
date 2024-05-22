using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Controls a Squirrel.
/// 
/// The SquirrelController is responsible for manipulating its Squirrel and bringing
/// it to life. This includes moving it, choosing targets, playing animations,
/// and more.
/// </summary>
public class SquirrelController : DefenderController<SquirrelController.SquirrelState>
{
    /// <summary>
    /// The number of Squirrels spawned so far this scene.
    /// </summary>
    private static int NUM_SQUIRRELS;

    /// <summary>
    /// The maximum number of targets a Squirrel can select at once.
    /// </summary>
    protected override int MAX_TARGETS => 1;

    /// <summary>
    /// State of a Squirrel. 
    /// </summary>
    public enum SquirrelState
    {
        SPAWN,
        IDLE,
        ATTACK,
        INVALID
    }

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
    /// How many seconds to wait between firing acorns.
    /// </summary>
    private const float delayBetweenAcorns = 0.1f;



    /// <summary>
    /// Makes a new SquirrelController.
    /// </summary>
    /// <param name="defender">The Squirrel Defender. </param>
    /// <returns>The created SquirrelController.</returns>
    public SquirrelController(Squirrel squirrel) : base(squirrel)
    {
        NUM_SQUIRRELS++;
    }

    /// <summary>
    /// Main update loop for the Squirrel.
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
    /// Returns this SquirrelController's Squirrel.
    /// </summary>
    /// <returns>this SquirrelController's Squirrel.</returns>
    private Squirrel GetSquirrel() { return GetMob() as Squirrel; }

    /// <summary>
    /// Handles all collisions between this controller's Squirrel
    /// model and some other collider.
    /// </summary>
    /// <param name="other">the other collider.</param>
    protected override void HandleCollision(Collider2D other) { throw new NotImplementedException(); }

    /// <summary>
    /// Returns the Squirrel's target if it has one. 
    /// </summary>
    /// <returns>the Squirrel's target; null if it doesn't have one.</returns>
    private Enemy GetTarget() { return NumTargets() == 1 ? GetTargets()[0] as Enemy : null; }

    /// <summary>
    /// Returns the Squirrel prefab to the SquirrelFactory singleton.
    /// </summary>
    public override void DestroyModel() { SquirrelFactory.ReturnSquirrelPrefab(GetSquirrel().gameObject); }

    //--------------------BEGIN STATE LOGIC----------------------//

    /// <summary>
    /// Returns true if two SquirrelStates are equal.
    /// </summary>
    /// <param name="stateA">The first state.</param>
    /// <param name="stateB">The second state.</param>
    /// <returns>true if two SquirrelStates are equal; otherwise, false.</returns>
    public override bool StateEquals(SquirrelState stateA, SquirrelState stateB)
    {
        return stateA == stateB;
    }

    /// <summary>
    /// Updates the state of this SquirrelController's Squirrel model.
    /// The transitions are: <br></br>
    /// 
    /// SPAWN --> IDLE : always <br></br>
    /// IDLE --> ATTACK : if target in range <br></br>
    /// ATTACK --> IDLE : if no target in range
    /// </summary>
    public override void UpdateStateFSM()
    {
        if (!ValidModel()) return;
        if (GetGameState() != GameState.ONGOING)
        {
            SetState(SquirrelState.IDLE);
            return;
        }



        switch (GetState())
        {
            case SquirrelState.SPAWN:
                if (SpawnStateDone()) SetState(SquirrelState.IDLE);
                break;
            case SquirrelState.IDLE:
                if (GetTarget() == null || !GetTarget().Targetable()) break;
                if (DistanceToTargetFromTree()  <= GetSquirrel().GetAttackRange() &&
                    GetSquirrel().GetAttackCooldown() <= 0) SetState(SquirrelState.ATTACK);
                break;
            case SquirrelState.ATTACK:
                if (GetTarget() == null || !GetTarget().Targetable()) SetState(SquirrelState.IDLE);
                if (GetAnimationCounter() > 0) break;
                if (GetSquirrel().GetAttackCooldown() > 0) SetState(SquirrelState.IDLE);
                else if (DistanceToTarget() > GetSquirrel().GetAttackRange()) SetState(SquirrelState.IDLE);
                break;
            case SquirrelState.INVALID:
                throw new System.Exception("Invalid State.");
        }
    }

    /// <summary>
    /// Runs logic relevant to the Squirrel's spawn state.
    /// </summary>
    protected override void ExecuteSpawnState()
    {
        if (!ValidModel()) return;
        if (GetState() != SquirrelState.SPAWN) return;

        GetSquirrel().FaceDirection(Direction.SOUTH);
        base.ExecuteSpawnState();
    }

    /// <summary>
    /// Runs logic relevant to the Squirrel's idle state.
    /// </summary>
    protected virtual void ExecuteIdleState()
    {
        if (!ValidModel()) return;
        if (GetState() != SquirrelState.IDLE) return;

        if(GetTarget() != null && DistanceToTargetFromTree() <= GetSquirrel().GetAttackRange()) 
             FaceTarget();
        else GetSquirrel().FaceDirection(Direction.SOUTH);

        SetAnimation(GetSquirrel().IDLE_ANIMATION_DURATION, SquirrelFactory.GetIdleTrack(GetSquirrel().GetDirection(), GetSquirrel().GetTier()));
    }

    /// <summary>
    /// Runs logic relevant to the Squirrel's attacking state.
    /// </summary>
    protected virtual void ExecuteAttackState()
    {
        if (!ValidModel()) return;
        if (GetTarget() == null || !GetTarget().Targetable()) return;
        if (GetState() != SquirrelState.ATTACK) return;

        // Animation Logic.

        FaceTarget();
        SetAnimation(GetSquirrel().ATTACK_ANIMATION_DURATION, SquirrelFactory.GetAttackTrack(GetSquirrel().GetDirection(), GetSquirrel().GetTier()));

        if (!CanAttack()) return;

        int numAcornsToFire = GetSquirrel().HasUpgrade(UpgradeType.DOUBLE_SHOT) ? 2 : 1;
        numAcornsToFire = GetSquirrel().HasUpgrade(UpgradeType.TRIPLE_SHOT) ? 3 : numAcornsToFire;
        GetSquirrel().StartCoroutine(FireAcorns(numAcornsToFire));

        // Reset attack animation.
        GetSquirrel().RestartAttackCooldown();
    }

    /// <summary>
    /// Returns true if the Squirrel can shoot an acorn.
    /// </summary>
    /// <returns>true if the Squirrel can shoot an acorn; otherwise,
    /// false.</returns>
    public override bool CanAttack()
    {
        if (!base.CanAttack()) return false; //Cooldown
        if (GetState() != SquirrelState.ATTACK) return false; //Not in the attack state.
        if (GetTarget() == null || !GetTarget().Targetable()) return false; //Invalid target.
        return true;
    }

    /// <summary>
    /// Adds one chunk of Time.deltaTime to the animation
    /// counter that tracks the current state.
    /// </summary>
    public override void AgeAnimationCounter()
    {
        SquirrelState state = GetState();
        if (state == SquirrelState.IDLE) idleAnimationCounter += Time.deltaTime;
        else if (state == SquirrelState.ATTACK) attackAnimationCounter += Time.deltaTime;
    }

    /// <summary>
    /// Returns the animation counter for the current state.
    /// </summary>
    /// <returns>the animation counter for the current state.</returns>
    public override float GetAnimationCounter()
    {
        SquirrelState state = GetState();
        if (state == SquirrelState.IDLE) return idleAnimationCounter;
        else if (state == SquirrelState.ATTACK) return attackAnimationCounter;
        else throw new System.Exception("State " + state + " has no counter.");
    }

    /// <summary>
    /// Sets the animation counter for the current state to 0.
    /// </summary>
    public override void ResetAnimationCounter()
    {
        SquirrelState state = GetState();
        if (state == SquirrelState.IDLE) idleAnimationCounter = 0;
        else if (state == SquirrelState.ATTACK) attackAnimationCounter = 0;
    }

    /// <summary>
    /// Queues an acorn to be fired at the Squirrel's target.
    /// </summary>
    /// <returns>A reference to the coroutine.</returns>
    /// <param name="numAcorns">The number of acorns to fire.</param>
    private IEnumerator FireAcorns(int numAcorns)
    {
        Assert.IsTrue(delayBetweenAcorns >= 0, "Delay needs to be non-negative");

        for(int i = 0; i < numAcorns; i++)
        {
            if(GetTarget() == null || !GetTarget().Targetable()) yield break; // Invalid target.

            GameObject acornPrefab = AcornFactory.GetAcornPrefab();
            Assert.IsNotNull(acornPrefab);
            Acorn acornComp = acornPrefab.GetComponent<Acorn>();
            Assert.IsNotNull(acornComp);
            Vector3 targetPosition = GetTarget().GetAttackPosition();
            AcornController acornController = new AcornController(acornComp, GetSquirrel().GetPosition(), targetPosition);
            ControllerController.AddModelController(acornController);

            if (i < numAcorns - 1) // Wait for the delay between shots unless it's the last one
            {
                yield return new WaitForSeconds(delayBetweenAcorns);
            }
        }
    }
}
