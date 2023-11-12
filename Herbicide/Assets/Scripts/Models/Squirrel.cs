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
    //--------------------BEGIN STATS----------------------//

    /// <summary>
    /// Starting health of a Squirrel
    /// </summary>
    public override int BASE_HEALTH => 200;

    /// <summary>
    /// Maximum health of a Squirrel
    /// </summary>
    public override int MAX_HEALTH => 200;

    /// <summary>
    /// Minimum health of a Squirrel
    /// </summary>
    public override int MIN_HEALTH => 0;

    /// <summary>
    /// Starting attack range of a Squirrel
    /// </summary>
    public override float BASE_ATTACK_RANGE => 6f;

    /// <summary>
    /// Maximum attack range of a Squirrel
    /// </summary>
    public override float MAX_ATTACK_RANGE => float.MaxValue;

    /// <summary>
    /// Minimum attack range of a Squirrel
    /// </summary>
    public override float MIN_ATTACK_RANGE => 0f;

    /// <summary>
    /// Starting attack speed of a Squirrel
    /// </summary>
    public override float BASE_ATTACK_SPEED => 1f;

    /// <summary>
    /// Maximum attack speed of a Squirrel
    /// </summary>
    public override float MAX_ATTACK_SPEED => float.MaxValue;

    /// <summary>
    /// Minimum attack speed of a Squirrel
    /// </summary>
    public override float MIN_ATTACK_SPEED => 0f;

    //---------------------END STATS-----------------------//

    //------------------BEGIN ANIMATION--------------------//

    /// <summary>
    /// How many seconds a Squirrel's attack animation lasts,
    /// from start to finish. 
    /// </summary>
    public float ATTACK_ANIMATION_DURATION => 1f;

    /// <summary>
    /// How many seconds a Squirrel's idle animation lasts,
    /// from start to finish. 
    /// </summary>
    public float IDLE_ANIMATION_DURATION => 1f;

    //--------------------END ANIMATION--------------------//

    /// <summary>
    /// Name of a Squirrel
    /// </summary>
    public override string NAME => "Squirrel";

    /// <summary>
    /// How much currency it takes to place a Squirrel
    /// </summary>
    public override int COST => 1;

    /// <summary>
    /// Type of a Squirrel
    /// </summary>
    public override DefenderType TYPE => DefenderType.SQUIRREL;

    /// <summary>
    /// Class of a Squirrel
    /// </summary>
    public override DefenderClass CLASS => DefenderClass.TREBUCHET;


    /// <summary>
    /// Called when this Squirrel dies.
    /// </summary>
    public override void OnDie() { return; }

    /// <summary>
    /// Sets this Squirrel's 2D Collider's properties.
    /// </summary>
    public override void SetColliderProperties() { return; }
}
