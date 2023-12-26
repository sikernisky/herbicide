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
    public override float BASE_ATTACK_RANGE => 3f;

    /// <summary>
    /// Maximum attack range of a Butterfly
    /// </summary>
    public override float MAX_ATTACK_RANGE => float.MaxValue;

    /// <summary>
    /// Minimum attack range of a Butterfly
    /// </summary>
    public override float MIN_ATTACK_RANGE => 0f;

    /// <summary>
    /// Amount of attack cooldown this Squirrel starts with.
    /// </summary>
    public override float BASE_ATTACK_COOLDOWN => 1f;

    /// <summary>
    /// Most amount of attack cooldown this Squirrel can have.
    /// </summary>
    public override float MAX_ATTACK_COOLDOWN => float.MaxValue;

    /// <summary>
    /// Starting chase range of a Butterfly.
    /// </summary>
    public override float BASE_CHASE_RANGE => float.MaxValue;

    /// <summary>
    /// Maximum chase range of a Butterfly.
    /// </summary>
    public override float MAX_CHASE_RANGE => float.MaxValue;

    /// <summary>
    /// Minimum chase range of a Butterfly.
    /// </summary>
    public override float MIN_CHASE_RANGE => 0f;

    /// <summary>
    /// Starting movement speed of a Butterfly.
    /// </summary>
    public override float BASE_MOVEMENT_SPEED => 2f;

    /// <summary>
    /// Maximum movement speed of a Butterfly.
    /// </summary>
    public override float MAX_MOVEMENT_SPEED => float.MaxValue;

    /// <summary>
    /// Minumum movement speed of a Butterfly.
    /// </summary>
    public override float MIN_MOVEMENT_SPEED => 0f;

    //------------------BEGIN ANIMATION--------------------//

    /// <summary>
    /// How many seconds a Butterfly's attack animation lasts,
    /// from start to finish. 
    /// </summary>
    public float ATTACK_ANIMATION_DURATION => .4f;

    /// <summary>
    /// How many seconds a Butterfly's move animation lasts,
    /// from start to finish. 
    /// </summary>
    public float MOVE_ANIMATION_DURATION => .75f;

    /// <summary>
    /// How many seconds a Butterfly's idle animation lasts,
    /// from start to finish. 
    /// </summary>
    public float IDLE_ANIMATION_DURATION => .5f;

    //--------------------END ANIMATION--------------------//

    /// <summary>
    /// Placement scale of this Butterfly.
    /// </summary>
    protected override Vector3 PLACEMENT_SCALE => new Vector3(.8f, .8f, 1f);

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
    public override ModelType TYPE => ModelType.BUTTERFLY;

    /// <summary>
    /// Class of a Butterfly
    /// </summary>
    public override DefenderClass CLASS => DefenderClass.ANIMYST;


    /// <summary>
    /// Returns true if this Butterfly is targetable; it never is.
    /// </summary>
    /// <returns>true if this Butterfly is targetable; otherwise, false.
    /// </returns>
    public override bool Targetable() { return false; }

    /// <summary>
    /// Sets this Butterfly's 2D Collider's properties.
    /// </summary>
    public override void SetColliderProperties() { return; }

    /// <summary>
    /// Returns an instantiated copy of the Butterfly Model.
    /// </summary>
    /// <returns> an instantiated copy of the Butterfly Model.</returns>
    public override GameObject Copy()
    {
        return Instantiate(ButterflyFactory.GetButterflyPrefab());
    }

    /// <summary>
    /// Returns the Sprite that represents this Butterfly when on a boat.
    /// </summary>
    /// <returns>the Sprite that represents this Butterfly when on a boat.</returns>
    public override Sprite[] GetBoatTrack() { return ButterflyFactory.GetBoatTrack(); }

    /// <summary>
    /// Returns the Sprite that represents this Butterfly when placing.
    /// </summary>
    /// <returns>the Sprite that represents this Butterfly when placing.</returns>
    public override Sprite[] GetPlacementTrack() { return ButterflyFactory.GetPlacementTrack(); }
}
