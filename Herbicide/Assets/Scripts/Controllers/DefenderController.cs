using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Controls a Defender.
/// </summary>
public class DefenderController
{
    /// <summary>
    /// Total number of DefenderControllers created during this level so far.
    /// </summary>
    private static int NUM_DEFENDERS;

    /// <summary>
    /// The Defender controlled by this DefenderController
    /// </summary>
    private Defender defender;

    /// <summary>
    /// The target this Defender should focus on
    /// </summary>
    private ITargetable target;

    /// <summary>
    /// Timer for keeping track of attacks per second.
    /// </summary>
    private float attackTimer;

    /// <summary>
    /// State of this DefenderController.
    /// </summary>
    private DefenderState state;

    /// <summary>
    /// ITargetabless within the range of the Defender controlled
    /// by this DefenderController
    /// </summary>
    private List<ITargetable> targetsInRange;

    /// <summary>
    /// Unique ID of this DefenderController.
    /// </summary>
    private int id;

    /// <summary>
    /// The current state of the game.
    /// </summary>
    private GameState gameState;




    /// <summary>
    /// FSM to represent an Defender's current state.
    /// </summary>
    protected enum DefenderState
    {
        SPAWN,
        IDLE,
        ATTACK,
        INVALID
    }

    /// <summary>
    /// Makes a new DefenderController for a Defender.
    /// </summary>
    /// <param name="defender">The Defender controlled by this DefenderController.</param>
    public DefenderController(Defender defender)
    {
        Assert.IsNotNull(defender);

        this.defender = defender;
        this.id = NUM_DEFENDERS;
        GetDefender().ResetStats();
        targetsInRange = new List<ITargetable>();
        NUM_DEFENDERS++;
    }

    /// <summary>
    /// Sets the Defender controlled by this DefenderController's target.
    /// </summary>
    /// <param name="targets">All Enemies to potentially target.</param>
    protected void SelectTarget()
    {
        if (!ValidDefender()) return;
        if (TargetsInRange() == null) target = null;
        if (target != null && !target.Dead()) return;

        List<Enemy> potentialTargets = new List<Enemy>();
        foreach (ITargetable candidate in TargetsInRange())
        {
            Enemy enemy = candidate as Enemy;
            if (enemy != null) potentialTargets.Add(enemy);
        }
        int random = Random.Range(0, potentialTargets.Count);
        if (potentialTargets.Count <= 0) target = null;
        else target = potentialTargets[random];
    }

    /// <summary>
    /// Updates the Defender controlled by this DefenderController: <br></br>
    /// 
    /// (1) Finds all targets in range. <br></br>
    /// (2) Updates state.<br></br>
    /// (3) Chooses a target.<br></br>
    /// (4) Tries to attack.<br></br>
    /// </summary>
    public void UpdateDefender(List<ITargetable> targets)
    {
        if (!ValidDefender()) return;

        TryClearDefender();
        UpdateDamageFlash();

        if (gameState == GameState.ONGOING)
        {
            FindTargetsInRange(targets);
            UpdateState();
            SelectTarget();
            TryAttackTarget();
        }
    }

    /// <summary>
    /// Tries to attack the Defender's target. This depends on the
    /// Defender's attack speed and target validity.
    /// </summary>
    public void TryAttackTarget()
    {
        //Safety checks
        if (!ValidDefender()) return;
        if (target == null)
        {
            GetDefender().RotateDefender(Direction.SOUTH);
            return;
        }

        attackTimer -= Time.deltaTime;
        if (GetState() != DefenderState.ATTACK) return;
        if (attackTimer <= 0)
        {
            attackTimer = 1f / GetDefender().GetAttackSpeed();
            GetDefender().Attack(target);
        }
    }

    /// <summary>
    /// Updates the state of this DefenderController.
    /// </summary>
    private void UpdateState()
    {
        if (!ValidDefender()) state = DefenderState.INVALID;

        switch (GetState())
        {
            case DefenderState.SPAWN:
                SetState(DefenderState.IDLE);
                break;
            case DefenderState.IDLE:
                if (NumTargetsInRange() > 0) SetState(DefenderState.ATTACK);
                break;
            case DefenderState.ATTACK:
                if (NumTargetsInRange() <= 0) SetState(DefenderState.IDLE);
                break;
            default:
                //should not get here
                Assert.IsTrue(false);
                break;
        }
    }

    /// <summary>
    /// Updates the list of ITargetables within the attack range of the Defender
    /// controlled by this DefenderController. Also updates the number of
    /// targets in range.
    /// </summary>
    /// <param name="targets">All potential ITargetables.</param>
    private void FindTargetsInRange(List<ITargetable> targets)
    {
        //Safety checks
        if (!ValidDefender()) return;
        if (targets == null) return;

        //Filter out targets that are (1) not enemies and (2) not in range
        List<ITargetable> targetablesInRange = new List<ITargetable>();
        foreach (ITargetable target in targets)
        {
            Enemy enemy = target as Enemy;
            if (enemy == null) continue;
            if (GetDefender().DistanceToTarget(enemy) <= GetDefender().GetAttackRange())
                targetablesInRange.Add(target);
        }
        targetsInRange = targetablesInRange;
        //Debug.Log(string.Join(", ", targetsInRange));
    }

    /// <summary>
    /// Returns the number of targets within the range of the Defender controlled by this
    /// DefenderController.
    /// </summary>
    /// <returns>the number of targets within the range of the Defender controlled by this
    /// DefenderController.</returns>
    protected int NumTargetsInRange()
    {
        if (!ValidDefender()) return -1;

        return targetsInRange.Count;
    }

    /// <summary>
    /// Informs this DefenderController of the most recent GameState so
    /// that it knows how to update its Defender.
    /// </summary>
    /// <param name="state">The most recent GameState.</param>
    public void InformOfGameState(GameState state)
    {
        gameState = state;
    }

    /// <summary>
    /// Returns a list of ITargetables within the range of the Defender controlled by this
    /// DefenderController.
    /// </summary>
    /// <returns>a list of ITargetables within the range of the Defender controlled by this
    /// DefenderController.</returns>
    protected List<ITargetable> TargetsInRange()
    {
        if (!ValidDefender()) return null;

        return targetsInRange;
    }

    /// <summary>
    /// Returns the state of this DefenderController.
    /// </summary>
    /// <returns>the state of this DefenderController.</returns>
    protected DefenderState GetState()
    {
        if (!ValidDefender()) return DefenderState.INVALID;

        return state;
    }

    /// <summary>
    /// Sets the state of this DefenderController.
    /// /// </summary>
    /// <param name="newState">the new state to set to.</param>
    /// <returns></returns>
    protected void SetState(DefenderState newState)
    {
        if (!ValidDefender()) return;

        state = newState;
    }

    /// <summary>
    /// Returns true if the Defender controlled by this DefenderController
    /// is not null.
    /// </summary>
    /// <returns>true if the Defender controlled by this DefenderController
    /// is not null; otherwise, returns false.</returns>
    private bool ValidDefender()
    {
        return GetDefender() != null;
    }

    /// <summary>
    /// Returns this DefenderController's Defender reference. 
    /// </summary>
    /// <returns>the reference to this DefenderController's defender.</returns>
    private Defender GetDefender()
    {
        return defender;
    }

    /// <summary>
    /// Kills the Defender controlled by this DefenderController.
    /// </summary>
    protected virtual void KillDefender()
    {
        if (!ValidDefender()) return;
        if (GetDefender().GetHealth() > 0) return;

        GetDefender().Die();
        GameObject.Destroy(GetDefender().gameObject);
        GameObject.Destroy(GetDefender());
    }

    /// <summary>
    /// Checks if this DefenderController's Defender should be removed from
    /// the game. If so, clears it.
    /// </summary>
    private void TryClearDefender()
    {
        if (GetDefender().GetHealth() <= 0) KillDefender();
    }

    /// <summary>
    /// Returns true if this DefenderController is no longer needed and
    /// should be removed by the ControllerController.<br></br>
    /// 
    /// By default, this returns true if the Defender controlled by this
    /// EnemyController is dead.
    /// </summary>
    /// <returns>true if this DefenderController is no longer needed and
    /// should be removed by the ControllerController; otherwise,
    /// returns false.</returns>
    public virtual bool ShouldRemoveController()
    {
        if (!ValidDefender()) return true;
        return false;
    }

    /// <summary>
    /// Plays this DefenderController's Damage flash effect
    /// if necessary.
    /// </summary>
    private void UpdateDamageFlash()
    {
        if (GetDefender().GetDamageFlashingTime() > 0)
        {
            const float intensity = .4f;
            float defenderFlashTime = GetDefender().DAMAGE_FLASH_TIME;
            float newDamageFlashingTime = GetDefender().GetDamageFlashingTime() - Time.deltaTime;
            GetDefender().SetDamageFlashingTime(Mathf.Clamp(newDamageFlashingTime, 0, defenderFlashTime));
            float score = Mathf.Lerp(
                intensity,
                1f,
                Mathf.Abs(GetDefender().GetDamageFlashingTime() - defenderFlashTime / 2f) * (intensity * 10f)
            );
            byte greenBlueComponent = (byte)(score * 255);
            Color32 color = new Color32(255, greenBlueComponent, greenBlueComponent, 255);
            GetDefender().SetColor(color);
        }
    }
}


