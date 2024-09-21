using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Controls a Porcupine. <br></br>
/// 
/// The PorcupineController is responsible for manipulating its Porcupine and bringing
/// it to life. This includes moving it, choosing targets, playing animations,
/// and more.
/// </summary>
/// <![CDATA[<param name="PorcupineState">]]>
public class PorcupineController : DefenderController<PorcupineController.PorcupineState>
{
    #region Fields

    /// <summary>
    /// States of a Porcupine.
    /// </summary>
    public enum PorcupineState
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
    /// Maximum number of targets a Porcupine can have at once.
    /// </summary>
    protected override int MAX_TARGETS => 1;

    /// <summary>
    /// How many seconds to wait between firing quills.
    /// </summary>
    private const float delayBetweenQuills = 0.05f;

    #endregion

    #region Methods

    /// <summary>
    /// Creates a PorcupineController reference.
    /// </summary>
    /// <param name="porcupine">The Porcupine defender.</param>
    /// <returns>a new PorcupineController reference.</returns>
    public PorcupineController(Porcupine porcupine) : base(porcupine) { }

    /// <summary>
    /// Main update loop for the Porcupine.
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
    /// Returns this PorcupineController's Porcupine.
    /// </summary>
    /// <returns>this PorcupineController's Porcupine.</returns>
    private Porcupine GetPorcupine() { return GetMob() as Porcupine; }

    /// <summary>
    /// Queues a quill to be fired at the Porcupine's target.
    /// </summary>
    /// <returns>A reference to the coroutine.</returns>
    /// <param name="numQuills">The number of quills to fire.</param>
    private IEnumerator FireQuills(int numQuills)
    {
        Assert.IsTrue(delayBetweenQuills >= 0, "Delay needs to be non-negative");

        for (int i = 0; i < numQuills; i++)
        {
            Enemy target = GetTarget() as Enemy;
            if (target == null || !target.Targetable()) yield break; // Invalid target.

            SetNextAnimation(GetPorcupine().ATTACK_ANIMATION_DURATION / numQuills,
                DefenderFactory.GetMainActionTrack(
                    ModelType.PORCUPINE,
                    GetPorcupine().GetDirection(), GetPorcupine().GetTier()));

            GameObject quillPrefab = ProjectileFactory.GetProjectilePrefab(ModelType.QUILL);
            Assert.IsNotNull(quillPrefab);
            Quill quillComp = quillPrefab.GetComponent<Quill>();
            Assert.IsNotNull(quillComp);
            bool doubleQuill = GetPorcupine().GetTier() > 2;
            Vector3 targetPosition = GetTarget().GetAttackPosition();
            QuillController quillController = new QuillController(quillComp, GetPorcupine().GetPosition(), targetPosition, doubleQuill);
            ControllerController.AddModelController(quillController);

            if (i < numQuills - 1) // Wait for the delay between shots unless it's the last one
            {
                yield return new WaitForSeconds(delayBetweenQuills);
            }
        }
    }

    /// <summary>
    /// Returns true if the Porcupine can shoot an quill.
    /// </summary>
    /// <returns>true if the Porcupine can shoot an quill; otherwise,
    /// false.</returns>
    public override bool CanPerformMainAction()
    {
        if (!base.CanPerformMainAction()) return false; //Cooldown
        if (GetState() != PorcupineState.ATTACK) return false; //Not in the attack state.
        Enemy target = GetTarget() as Enemy;
        if (target == null || !target.Targetable()) return false; //Invalid target.
        return true;
    }

    #endregion

    #region State Logic

    /// <summary>
    /// Returns true if two PorcupineStates are equal.
    /// </summary>
    /// <param name="stateA">The first state.</param>
    /// <param name="stateB">The second state.</param>
    /// <returns>true if two PorcupineStates are equal; otherwise, false.</returns>
    public override bool StateEquals(PorcupineState stateA, PorcupineState stateB) => stateA == stateB;

    /// <summary>
    /// Updates the state of this PorcupineController's Porcupine model.
    /// The transitions are: <br></br>
    /// 
    /// SPAWN --> IDLE : always <br></br>
    /// </summary>
    public override void UpdateFSM()
    {
        if (!ValidModel()) return;
        if (GetGameState() != GameState.ONGOING)
        {
            SetState(PorcupineState.IDLE);
            return;
        }

        Enemy target = GetTarget() as Enemy;
        switch (GetState())
        {
            case PorcupineState.SPAWN:
                if (SpawnStateDone()) SetState(PorcupineState.IDLE);
                break;
            case PorcupineState.IDLE:
                if (target == null || !target.Targetable()) break;
                if (DistanceToTargetFromTree() <= GetPorcupine().GetMainActionRange() &&
                    GetPorcupine().GetMainActionCooldown() <= 0) SetState(PorcupineState.ATTACK);
                break;
            case PorcupineState.ATTACK:
                if (target == null || !target.Targetable()) SetState(PorcupineState.IDLE);
                if (GetAnimationCounter() > 0) break;
                if (GetPorcupine().GetMainActionCooldown() > 0) SetState(PorcupineState.IDLE);
                else if (DistanceToTarget() > GetPorcupine().GetMainActionRange()) SetState(PorcupineState.IDLE);
                break;
            case PorcupineState.INVALID:
                throw new System.Exception("Invalid State.");
        }
    }

    /// <summary>
    /// Runs logic relevant to the Porcupine's spawn state.
    /// </summary>
    protected override void ExecuteSpawnState()
    {
        if (!ValidModel()) return;
        if (GetState() != PorcupineState.SPAWN) return;

        GetPorcupine().FaceDirection(Direction.SOUTH);
        base.ExecuteSpawnState();
    }

    /// <summary>
    /// Runs logic relevant to the Porcupine's idle state.
    /// </summary>
    protected virtual void ExecuteIdleState()
    {
        if (!ValidModel()) return;
        if (GetState() != PorcupineState.IDLE) return;
        Enemy target = GetTarget() as Enemy;
        if (target != null && DistanceToTargetFromTree() <= GetPorcupine().GetMainActionRange())
            FaceTarget();
        else GetPorcupine().FaceDirection(Direction.SOUTH);

        SetNextAnimation(GetPorcupine().IDLE_ANIMATION_DURATION,
            DefenderFactory.GetIdleTrack(
                ModelType.PORCUPINE,
                GetPorcupine().GetDirection(), GetPorcupine().GetTier()));
    }

    /// <summary>
    /// Runs logic relevant to the Porcupine's attacking state.
    /// </summary>
    protected virtual void ExecuteAttackState()
    {
        if (!ValidModel()) return;
        Enemy target = GetTarget() as Enemy;
        if (target == null || !target.Targetable()) return;
        if (GetState() != PorcupineState.ATTACK) return;

        FaceTarget();
        if (!CanPerformMainAction()) return;

        // Calculate the number of quills to fire based on the Porcupine's tier.
        int tier = GetPorcupine().GetTier();
        int numQuillsToFire = 1;
        if (tier == 2) numQuillsToFire = 2;
        else if (tier >= 3) numQuillsToFire = 4;
        GetPorcupine().StartCoroutine(FireQuills(numQuillsToFire));

        // Reset attack animation.
        GetPorcupine().RestartMainActionCooldown();
    }

    #endregion

    #region Animation Logic

    /// <summary>
    /// Adds one chunk of Time.deltaTime to the animation
    /// counter that tracks the current state.
    /// </summary>
    public override void AgeAnimationCounter()
    {
        PorcupineState state = GetState();
        if (state == PorcupineState.IDLE) idleAnimationCounter += Time.deltaTime;
        else if (state == PorcupineState.ATTACK) attackAnimationCounter += Time.deltaTime;
    }

    /// <summary>
    /// Returns the animation counter for the current state.
    /// </summary>
    /// <returns>the animation counter for the current state.</returns>
    public override float GetAnimationCounter()
    {
        PorcupineState state = GetState();
        if (state == PorcupineState.IDLE) return idleAnimationCounter;
        else if (state == PorcupineState.ATTACK) return attackAnimationCounter;
        else return 0;
    }

    /// <summary>
    /// Sets the animation counter for the current state to 0.
    /// </summary>
    public override void ResetAnimationCounter()
    {
        PorcupineState state = GetState();
        if (state == PorcupineState.IDLE) idleAnimationCounter = 0;
        else if (state == PorcupineState.ATTACK) attackAnimationCounter = 0;
    }

    #endregion
}
