using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Assertions;

/// <summary>
/// Represents a Boat that flows across the scene. It is
/// part of the shop. 
/// </summary>
public class ShopBoat : Mob
{
    /// <summary>
    /// Name of a ShopBoat.
    /// </summary>
    public override string NAME => "ShopBoat";

    /// <summary>
    /// Type of a ShopBoat.
    /// </summary>
    public override ModelType TYPE => ModelType.SHOP_BOAT;

    /// <summary>
    /// Starting attack range of a ShopBoat.
    /// </summary>
    public override float BASE_ATTACK_RANGE => 0f;

    /// <summary>
    /// Maximum attack range of a ShopBoat.
    /// </summary>
    public override float MAX_ATTACK_RANGE => float.MaxValue;

    /// <summary>
    /// Minimum attack range of a ShopBoat.
    /// </summary>
    public override float MIN_ATTACK_RANGE => 0f;

    /// <summary>
    /// Base attack cooldown of a ShopBoat.
    /// </summary>
    public override float BASE_ATTACK_COOLDOWN => float.MaxValue;

    /// <summary>
    /// Maximum attack cooldown of a ShopBoat.
    /// </summary>
    public override float MAX_ATTACK_COOLDOWN => float.MaxValue;

    /// <summary>
    /// Starting chase range of a ShopBoat.
    /// </summary>
    public override float BASE_CHASE_RANGE => 0f;

    /// <summary>
    /// Maximum chase range of a ShopBoat.
    /// </summary>
    public override float MAX_CHASE_RANGE => float.MaxValue;

    /// <summary>
    /// Minimum chase range of a ShopBoat.
    /// </summary>
    public override float MIN_CHASE_RANGE => float.MinValue;

    /// <summary>
    /// Starting movement speed of a ShopBoat.
    /// </summary>
    public override float BASE_MOVEMENT_SPEED => 1f;

    /// <summary>
    /// Maximum movement speed of a ShopBoat.
    /// </summary>

    public override float MAX_MOVEMENT_SPEED => float.MaxValue;

    /// <summary>
    /// Minimum movement speed of a ShopBoat.
    /// </summary>
    public override float MIN_MOVEMENT_SPEED => 0f;

    /// <summary>
    /// Cost to place a ShopBoat.
    /// </summary>
    public override int COST => throw new System.NotImplementedException();

    /// <summary>
    /// Starting health of a ShopBoat.
    /// </summary>
    public override int BASE_HEALTH => 100;

    /// <summary>
    /// Maximum health of a ShopBoat.
    /// </summary>
    public override int MAX_HEALTH => int.MaxValue;

    /// <summary>
    /// Minimum health of a ShopBoat.
    /// </summary>
    public override int MIN_HEALTH => int.MinValue;

    /// <summary>
    /// The text component on the ShopBoat's sign that displays the
    /// rider's price.
    /// </summary>
    [SerializeField]
    private TMP_Text signPrice;

    /// <summary>
    /// The SpriteRenderer component that displays the rider being sold.
    /// </summary>
    [SerializeField]
    private SpriteRenderer riderRenderer;

    /// <summary>
    /// The Model riding this ShopBoat.
    /// </summary>
    private Model rider;


    /// <summary>
    /// Sets the sprite of the ShopBoat's rider.
    /// </summary>
    /// <param name="s">The Sprite to set to.</param>
    public void SetRider(Model rider)
    {
        Assert.IsNotNull(rider);
        this.rider = rider;
        riderRenderer.sprite = rider.GetBoatSprite();
    }

    /// <summary>
    /// Returns this ShopBoat's Model rider.
    /// </summary>
    /// <returns>this ShopBoat's Model rider.</returns>
    public Model GetRider()
    {
        Assert.IsNotNull(rider);
        return rider;
    }

    /// <summary>
    /// Returns the Sprite that represents the ShopBoat on a
    /// ShopBoat.
    /// </summary>
    /// <returns>the Sprite that represents the ShopBoat on a
    /// ShopBoat.</returns>
    public override Sprite GetBoatSprite() { return null; }

    /// <summary>
    /// Returns the Sprite that represents the ShopBoat when
    /// placing.
    /// </summary>
    /// <returns>the Sprite that represents the ShopBoat when
    /// placing.</returns>
    public override Sprite GetPlacementSprite() { return null; }

    /// <summary>
    /// Returns an instantiated GameObject of this ShopBoat.
    /// </summary>
    /// <returns>an instantiated GameObject of this ShopBoat.</returns>
    public override GameObject MakePlaceableObject() { return null; }

    /// <summary>
    /// Sets the properties of the ShopBoat's 2D Collider component.
    /// </summary>
    public override void SetColliderProperties() { return; }
}
