using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a Buttefly Defender. It drops bombs on
/// enemies. 
/// </summary>
public class Butterfly : Defender
{
    //--------------------BEGIN STATS----------------------//

    /// <summary>
    /// Starting health of a Butterfly
    /// </summary>
    public override int BASE_HEALTH => 200;

    /// <summary>
    /// Maximum health of a Butterfly
    /// </summary>
    public override int MAX_HEALTH => 200;

    /// <summary>
    /// Minimum health of a Butterfly
    /// </summary>
    public override int MIN_HEALTH => 0;

    /// <summary>
    /// Starting attack range of a Butterfly
    /// </summary>
    public override float BASE_ATTACK_RANGE => 6f;

    /// <summary>
    /// Maximum attack range of a Butterfly
    /// </summary>
    public override float MAX_ATTACK_RANGE => float.MaxValue;

    /// <summary>
    /// Minimum attack range of a Butterfly
    /// </summary>
    public override float MIN_ATTACK_RANGE => 0f;

    /// <summary>
    /// Starting attack speed of a Butterfly
    /// </summary>
    public override float BASE_ATTACK_SPEED => 1f;

    /// <summary>
    /// Maximum attack speed of a Butterfly
    /// </summary>
    public override float MAX_ATTACK_SPEED => float.MaxValue;

    /// <summary>
    /// Minimum attack speed of a Butterfly
    /// </summary>
    public override float MIN_ATTACK_SPEED => 0f;

    //---------------------END STATS-----------------------//

    //------------------BEGIN ANIMATION--------------------//

    /// <summary>
    /// How many seconds a Butterfly's attack animation lasts,
    /// from start to finish. 
    /// </summary>
    public float ATTACK_ANIMATION_DURATION => 1f;

    /// <summary>
    /// How many seconds a Butterfly's idle animation lasts,
    /// from start to finish. 
    /// </summary>
    public float IDLE_ANIMATION_DURATION => 1f;

    //--------------------END ANIMATION--------------------//

    /// <summary>
    /// Name of a Butterfly
    /// </summary>
    public override string NAME => "Butterfly";

    /// <summary>
    /// How much currency it takes to place a Butterfly
    /// </summary>
    public override int COST => 1;

    /// <summary>
    /// Type of a Butterfly
    /// </summary>
    public override DefenderType TYPE => DefenderType.BUTTERFLY;

    /// <summary>
    /// Class of a Butterfly
    /// </summary>
    public override DefenderClass CLASS => DefenderClass.ANIMYST;


    /// <summary>
    /// Called when this Butterfly dies.
    /// </summary>
    public override void OnDie() { return; }

    /// <summary>
    /// Sets this Butterfly's 2D Collider's properties.
    /// </summary>
    public override void SetColliderProperties() { return; }
}
