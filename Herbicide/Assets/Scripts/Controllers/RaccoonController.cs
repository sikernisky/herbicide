using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

/// <summary>
/// Controls a Raccoon. <br></br>
/// 
/// The RaccoonController is responsible for manipulating its Raccoon and bringing
/// it to life. This includes moving it, choosing targets, playing animations,
/// and more.
/// </summary>
/// <![CDATA[<param name="RaccoonState">]]>
public class RaccoonController : DefenderController<RaccoonController.RaccoonState>
{
    #region Fields

    /// <summary>
    /// States of a Raccoon.
    /// </summary>
    public enum RaccoonState
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
    /// Maximum number of targets a Raccoon can have at once.
    /// </summary>
    protected override int MAX_TARGETS => 1;

    /// <summary>
    /// How many seconds to wait between firing quills.
    /// </summary>
    private const float delayBetweenBlackberries = 0.1f;

    #endregion

    #region Methods

    /// <summary>
    /// Creates a RaccoonController reference.
    /// </summary>
    /// <param name="raccoon">The Raccoon defender.</param>
    /// <returns>a new RaccoonController reference.</returns>
    public RaccoonController(Raccoon raccoon) : base(raccoon) { }

    /// <summary>
    /// Main update loop for the Raccoon.
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
    /// Returns this RaccoonController's Raccoon.
    /// </summary>
    /// <returns>this RaccoonController's Raccoon.</returns>
    private Raccoon GetRaccoon() { return GetMob() as Raccoon; }

    /// <summary>
    /// Queues <c>numBerries</c> amount of berries to be fired at
    /// the Raccoon's target.
    /// </summary>
    /// <returns>A reference to the coroutine.</returns>
    /// <param name="numBerries">The number of berries to fire.</param>
    private IEnumerator FireBerries(int numBerries)
    {
        Assert.IsTrue(delayBetweenBlackberries >= 0, "Delay needs to be non-negative");

        for (int i = 0; i < numBerries; i++)
        {
            Enemy target = GetTarget() as Enemy;
            if (target == null || !target.Targetable()) yield break; // Invalid target.

            SetNextAnimation(GetRaccoon().ATTACK_ANIMATION_DURATION / numBerries,
                DefenderFactory.GetMainActionTrack(
                    ModelType.RACCOON,
                    GetRaccoon().GetDirection(), GetRaccoon().GetTier()));

            int raccoonTier = GetRaccoon().GetTier();
            Vector3 targetPosition = GetTarget().GetAttackPosition();
            Vector3 raccoonPosition = GetRaccoon().GetPosition();

            if (raccoonTier == 1)
            {
                // 100% chance to fire a blackberry at tier 1
                GameObject blackberryPrefab = ProjectileFactory.GetProjectilePrefab(ModelType.BLACKBERRY);
                Assert.IsNotNull(blackberryPrefab, "Blackberry prefab is null.");
                Blackberry blackberryComp = blackberryPrefab.GetComponent<Blackberry>();
                Assert.IsNotNull(blackberryComp, "Blackberry component is missing.");

                BlackberryController blackberryController = new BlackberryController(blackberryComp, raccoonPosition, targetPosition);
                ControllerManager.AddModelController(blackberryController);
            }
            else if (raccoonTier == 2)
            {
                if (Random.value < 0.40f)
                {
                    // 40% chance to fire a raspberry at tier 2
                    GameObject raspberryPrefab = ProjectileFactory.GetProjectilePrefab(ModelType.RASPBERRY);
                    Assert.IsNotNull(raspberryPrefab, "Raspberry prefab is null.");
                    Raspberry raspberryComp = raspberryPrefab.GetComponent<Raspberry>();
                    Assert.IsNotNull(raspberryComp, "Raspberry component is missing.");

                    RaspberryController raspberryController = new RaspberryController(raspberryComp, raccoonPosition, targetPosition);
                    ControllerManager.AddModelController(raspberryController);
                }
                else
                {
                    // 60% chance to fire a blackberry at tier 2
                    GameObject blackberryPrefab = ProjectileFactory.GetProjectilePrefab(ModelType.BLACKBERRY);
                    Assert.IsNotNull(blackberryPrefab, "Blackberry prefab is null.");
                    Blackberry blackberryComp = blackberryPrefab.GetComponent<Blackberry>();
                    Assert.IsNotNull(blackberryComp, "Blackberry component is missing.");

                    BlackberryController blackberryController = new BlackberryController(blackberryComp, raccoonPosition, targetPosition);
                    ControllerManager.AddModelController(blackberryController);
                }
            }
            else if (raccoonTier == 3)
            {
                float chance = Random.value;
                if (chance < 0.20f)
                {
                    // 20% chance to fire a blackberry at tier 3
                    GameObject blackberryPrefab = ProjectileFactory.GetProjectilePrefab(ModelType.BLACKBERRY);
                    Assert.IsNotNull(blackberryPrefab, "Blackberry prefab is null.");
                    Blackberry blackberryComp = blackberryPrefab.GetComponent<Blackberry>();
                    Assert.IsNotNull(blackberryComp, "Blackberry component is missing.");

                    BlackberryController blackberryController = new BlackberryController(blackberryComp, raccoonPosition, targetPosition);
                    ControllerManager.AddModelController(blackberryController);
                }
                else if (chance < 0.70f)
                {
                    // 50% chance to fire a raspberry at tier 3
                    GameObject raspberryPrefab = ProjectileFactory.GetProjectilePrefab(ModelType.RASPBERRY);
                    Assert.IsNotNull(raspberryPrefab, "Raspberry prefab is null.");
                    Raspberry raspberryComp = raspberryPrefab.GetComponent<Raspberry>();
                    Assert.IsNotNull(raspberryComp, "Raspberry component is missing.");

                    RaspberryController raspberryController = new RaspberryController(raspberryComp, raccoonPosition, targetPosition);
                    ControllerManager.AddModelController(raspberryController);
                }
                else
                {
                    // 30% chance to fire a salmonberry at tier 3
                    GameObject salmonberryPrefab = ProjectileFactory.GetProjectilePrefab(ModelType.SALMONBERRY);
                    Assert.IsNotNull(salmonberryPrefab, "Salmonberry prefab is null.");
                    Salmonberry salmonberryComp = salmonberryPrefab.GetComponent<Salmonberry>();
                    Assert.IsNotNull(salmonberryComp, "Salmonberry component is missing.");

                    SalmonberryController salmonberryController = new SalmonberryController(salmonberryComp, raccoonPosition, targetPosition);
                    ControllerManager.AddModelController(salmonberryController);
                }
            }

            if (i < numBerries - 1) // Wait for the delay between shots unless it's the last one
            {
                yield return new WaitForSeconds(delayBetweenBlackberries);
            }
        }
    }

    /// <summary>
    /// Returns true if the Raccoon can shoot an quill.
    /// </summary>
    /// <returns>true if the Raccoon can shoot an quill; otherwise,
    /// false.</returns>
    public override bool CanPerformMainAction()
    {
        if (!base.CanPerformMainAction()) return false; //Cooldown
        if (GetState() != RaccoonState.ATTACK) return false; //Not in the attack state.
        Enemy target = GetTarget() as Enemy;
        if (target == null || !target.Targetable()) return false; //Invalid target.
        return true;
    }

    #endregion

    #region State Logic

    /// <summary>
    /// Returns true if two RaccoonStates are equal.
    /// </summary>
    /// <param name="stateA">The first state.</param>
    /// <param name="stateB">The second state.</param>
    /// <returns>true if two RaccoonStates are equal; otherwise, false.</returns>
    public override bool StateEquals(RaccoonState stateA, RaccoonState stateB) => stateA == stateB;

    /// <summary>
    /// Updates the state of this RaccoonController's Raccoon model.
    /// The transitions are: <br></br>
    /// 
    /// SPAWN --> IDLE : always <br></br>
    /// </summary>
    public override void UpdateFSM()
    {
        if (!ValidModel()) return;
        if (GetGameState() != GameState.ONGOING)
        {
            SetState(RaccoonState.IDLE);
            return;
        }

        Enemy target = GetTarget() as Enemy;
        switch (GetState())
        {
            case RaccoonState.SPAWN:
                if (SpawnStateDone()) SetState(RaccoonState.IDLE);
                break;
            case RaccoonState.IDLE:
                if (target == null || !target.Targetable()) break;
                if (DistanceToTargetFromTree() <= GetRaccoon().GetMainActionRange() &&
                    GetRaccoon().GetMainActionCooldownRemaining() <= 0) SetState(RaccoonState.ATTACK);
                break;
            case RaccoonState.ATTACK:
                if (target == null || !target.Targetable()) SetState(RaccoonState.IDLE);
                if (GetAnimationCounter() > 0) break;
                if (GetRaccoon().GetMainActionCooldownRemaining() > 0) SetState(RaccoonState.IDLE);
                else if (DistanceToTarget() > GetRaccoon().GetMainActionRange()) SetState(RaccoonState.IDLE);
                break;
            case RaccoonState.INVALID:
                throw new System.Exception("Invalid State.");
        }
    }

    /// <summary>
    /// Runs logic relevant to the Raccoon's spawn state.
    /// </summary>
    protected override void ExecuteSpawnState()
    {
        if (!ValidModel()) return;
        if (GetState() != RaccoonState.SPAWN) return;

        GetRaccoon().FaceDirection(Direction.SOUTH);
        base.ExecuteSpawnState();
    }

    /// <summary>
    /// Runs logic relevant to the Raccoon's idle state.
    /// </summary>
    protected virtual void ExecuteIdleState()
    {
        if (!ValidModel()) return;
        if (GetState() != RaccoonState.IDLE) return;
        Enemy target = GetTarget() as Enemy;
        if (target != null && DistanceToTargetFromTree() <= GetRaccoon().GetMainActionRange())
            FaceTarget();
        else GetRaccoon().FaceDirection(Direction.SOUTH);

        SetNextAnimation(GetRaccoon().IDLE_ANIMATION_DURATION,
            DefenderFactory.GetIdleTrack(
                ModelType.RACCOON,
                GetRaccoon().GetDirection(), GetRaccoon().GetTier()));
    }

    /// <summary>
    /// Runs logic relevant to the Raccoon's attacking state.
    /// </summary>
    protected virtual void ExecuteAttackState()
    {
        if (!ValidModel()) return;
        Enemy target = GetTarget() as Enemy;
        if (target == null || !target.Targetable()) return;
        if (GetState() != RaccoonState.ATTACK) return;

        FaceTarget();
        if (!CanPerformMainAction()) return;

        // Calculate the number of quills to fire based on the Raccoon's tier.
        int tier = GetRaccoon().GetTier();
        int numBerriesToFire = 1;
        if (tier == 2) numBerriesToFire = 2;
        else if (tier >= 3) numBerriesToFire = 4;
        GetRaccoon().StartCoroutine(FireBerries(numBerriesToFire));

        // Reset attack animation.
        GetRaccoon().RestartMainActionCooldown();
    }

    #endregion

    #region Animation Logic

    /// <summary>
    /// Adds one chunk of Time.deltaTime to the animation
    /// counter that tracks the current state.
    /// </summary>
    public override void AgeAnimationCounter()
    {
        RaccoonState state = GetState();
        if (state == RaccoonState.IDLE) idleAnimationCounter += Time.deltaTime;
        else if (state == RaccoonState.ATTACK) attackAnimationCounter += Time.deltaTime;
    }

    /// <summary>
    /// Returns the animation counter for the current state.
    /// </summary>
    /// <returns>the animation counter for the current state.</returns>
    public override float GetAnimationCounter()
    {
        RaccoonState state = GetState();
        if (state == RaccoonState.IDLE) return idleAnimationCounter;
        else if (state == RaccoonState.ATTACK) return attackAnimationCounter;
        else return 0;
    }

    /// <summary>
    /// Sets the animation counter for the current state to 0.
    /// </summary>
    public override void ResetAnimationCounter()
    {
        RaccoonState state = GetState();
        if (state == RaccoonState.IDLE) idleAnimationCounter = 0;
        else if (state == RaccoonState.ATTACK) attackAnimationCounter = 0;
    }

    #endregion
}
