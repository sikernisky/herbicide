using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    /// Attacks a target. Squirrels shoot one acorn Projectile towards their target.
    /// </summary>
    /// <param name="target"></param>
    public override void Attack(ITargetable target)
    {
        if (target == null) return;

        base.Attack(target);
        StartCoroutine(CoAttack(target.GetAttackPosition()));
    }

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
}
