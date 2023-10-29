using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Represents a Squirrel Defender. It shoots acorns towards
/// enemies. 
/// </summary>
public class Squirrel : Defender
{
    /// <summary>
    /// Type of a Squirrel
    /// </summary>
    public override DefenderType TYPE => DefenderType.SQUIRREL;


    /// <summary>
    /// Class of a Squirrel
    /// </summary>
    public override DefenderClass CLASS => DefenderClass.TREBUCHET;

    /// <summary>
    /// Name of a Squirrel
    /// </summary>
    protected override string NAME => "Squirrel";

    /// <summary>
    /// How much currency it takes to place a Squirrel
    /// </summary>
    protected override int COST => 1;

    /// <summary>
    /// Starting attack speed of a Squirrel
    /// </summary>
    public override float BASE_ATTACK_SPEED => 1f;

    /// <summary>
    /// Starting attack range of a Squirrel.
    /// </summary>
    public override float BASE_ATTACK_RANGE => 15f;

    /// <summary>
    /// Prefab for an acorn Projectile.
    /// </summary>
    [SerializeField]
    private GameObject acorn;




    /// <summary>
    /// Attacks an ITargetable. Handles the time and delay logic for attacking.
    /// </summary>
    /// <param name="targetPosition">The world position of the target.</param>
    /// <returns>A reference to the coroutine.</returns>
    private IEnumerator CoAttack(Vector2 targetPosition)
    {
        float acornDelay = ATTACK_DURATION * ATTACK_DURATION_PERCENTAGE;

        yield return new WaitForSeconds(acornDelay);
        PlayAnimation(DefenderAnimationType.ATTACK);

        //Prep the projectile for the ProjectileController
        GameObject acornOb = Instantiate(acorn.gameObject);
        acornOb.transform.SetParent(transform);
        acornOb.transform.localPosition = new Vector3(0, 0, 1);
        ProjectileController.Shoot(acornOb, targetPosition);
        //SoundController.PlaySoundEffect("squirrelAttack");

        yield return new WaitForSeconds(acornDelay);
        PlayAnimation(DefenderAnimationType.IDLE);
    }

    /// <summary>
    /// Returns the most updated DefenderState of this defender
    /// after considering its environment. 
    /// </summary>
    /// <param name="currentState">The current state of this Defender.</param>
    /// <param name="targetsInRange">The number of targets this Defender
    /// can see. </param>
    /// <returns>the correct, up to date DefenderState of this Defender. </returns>
    public override MobState DetermineState(
        MobState currentState,
        int targetsInRange)
    {
        switch (currentState)
        {
            case MobState.SPAWN:
                return MobState.IDLE;
            case MobState.IDLE:
                if (targetsInRange > 0) return MobState.ATTACK;
                else return MobState.IDLE;
            case MobState.ATTACK:
                if (targetsInRange <= 0) return MobState.IDLE;
                else return MobState.ATTACK;
            case MobState.INACTIVE:
                return MobState.SPAWN;
            default:
                //should not get here
                Debug.Log(currentState);
                throw new System.Exception();
        }
    }

    //------------------STATE LOGIC----------------------//

    /// <summary>
    /// Executes chase logic: does nothing; squirrels are immobile 
    /// and do not chase.
    /// </summary>
    /// <param name="target">The target to "chase."</param>
    public override void Chase(ITargetable target)
    {
        Assert.IsNotNull(target, "Cannot chase a null target.");
        return;
    }

    /// <summary>
    /// Executes idle logic: waits around and does nothing.
    /// </summary>
    public override void Idle()
    {
        return;
    }

    /// <summary>
    /// Executes attack logic: throws an acorn at its target. 
    /// Squirrels shoot one acorn Projectile.
    /// </summary>
    /// <param name="target">The target to attack.</param>
    public override void Attack(ITargetable target)
    {
        Assert.IsNotNull(target, "Cannot attack a null target.");
        FaceTarget(target);
        StartCoroutine(CoAttack(target.GetAttackPosition()));
    }
}
