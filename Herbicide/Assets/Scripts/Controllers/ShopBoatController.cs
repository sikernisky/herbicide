using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Controls a ShopBoat.
/// 
/// The ShopBoatController is responsible for manipulating its ShopBoat and bringing
/// it to life. This includes moving it, choosing targets, playing animations,
/// and more.
/// </summary>
public class ShopBoatController : MobController<ShopBoatController.ShopBoatState>
{
    /// <summary>
    /// States of a ShopBoat.
    /// </summary>
    public enum ShopBoatState
    {
        SPAWN,
        CRUISE_FULL,
        CRUISE_EMPTY
    }

    /// <summary>
    /// The rider the ShopBoat is selling.
    /// </summary>
    private Model rider;

    /// <summary>
    /// Main update loop for the ShopBoat.
    /// </summary>
    protected override void UpdateMob()
    {
        base.UpdateMob();
        ExecuteCruiseFullState();
    }

    /// <summary>
    /// The maximum number of targets a ShopBoat can have.
    /// </summary>
    protected override int MAX_TARGETS => int.MaxValue;

    /// <summary>
    /// Assigns a ShopBoat to a ShopBoatController.
    /// </summary>
    /// <param name="shopBoat">The ShopBoat to assign.</param>
    /// <param name="item">The Model being sold..</param>
    public ShopBoatController(ShopBoat shopBoat) : base(shopBoat)
    {
        Assert.IsNotNull(shopBoat.GetRider(), "You need to set the rider first.");
    }


    /// <summary>
    /// Returns the ShopBoat Model.
    /// </summary>
    /// <returns>the ShopBoat Model.</returns>
    protected ShopBoat GetBoat() { return GetMob() as ShopBoat; }


    /// <summary>
    /// Handles a collision between the ShopBoat and some other
    /// 2D Collider.
    /// </summary>
    /// <param name="other">The other Collider2D. </param>
    protected override void HandleCollision(Collider2D other) { return; }

    /// <summary>
    /// Returns true if the ShopBoat should be set to null and
    /// destroyed.
    /// </summary>
    /// <returns>true if the ShopBoat should be removed; otherwise,
    /// false. </returns>
    protected override bool ShouldRemoveModel() { return GetBoat().Dead(); }

    //--------------------STATE LOGIC----------------------//

    /// <summary>
    /// Returns true if two ShopBoatStates are equal.
    /// </summary>
    /// <param name="stateA">The first state.</param>
    /// <param name="stateB">The second state.</param>
    /// <returns>true if two ShopBoatStates are equal; otherwise, 
    /// false. </returns>
    public override bool StateEquals(ShopBoatState stateA, ShopBoatState stateB)
    {
        return stateA == stateB;
    }

    /// <summary>
    /// Updates the state of this ShopBoatController's ShopBoat model.
    /// The transitions are: <br></br>
    /// 
    /// SPAWN --> CRUISE_FULL : always
    /// CRUISE_FULL --> CRUISE_EMPTY : when occupant purchased
    /// </summary>
    public override void UpdateStateFSM()
    {
        switch (GetState())
        {
            case ShopBoatState.SPAWN:
                SetState(ShopBoatState.CRUISE_FULL);
                break;
            case ShopBoatState.CRUISE_FULL:
                break;
            case ShopBoatState.CRUISE_EMPTY:
                break;
        }
    }

    /// <summary>
    /// Runs logic for the ShopBoat's CruiseFull state.
    /// </summary>
    protected virtual void ExecuteCruiseFullState()
    {
        if (GetState() != ShopBoatState.CRUISE_FULL) return;
        if (!ValidModel()) return;

        // For now, just go right.
        ShopBoat shopBoat = GetBoat();
        float boatSpeed = shopBoat.GetMovementSpeed();
        float deltaTime = Time.deltaTime;
        Vector3 newPosition = shopBoat.GetPosition();
        newPosition.x += boatSpeed * deltaTime; // Move to the right
        shopBoat.SetWorldPosition(newPosition);
    }

    //--------------------END STATE LOGIC----------------------//


    /// <summary>
    /// Returns true if the ShopBoat can target the PlaceableObject passed
    /// into this method.
    /// </summary>
    /// <param name="target">The target to check.</param>
    /// <returns>true if the ShopBoat can target the PlaceableObject; otherwise,
    /// false.</returns>
    protected override bool CanTarget(PlaceableObject target)
    {
        return false;
    }

    /// <summary>
    /// Adds one chunk of Time.deltaTime to the animation
    /// counter that tracks the current state.
    /// </summary>
    public override void AgeAnimationCounter() { return; }

    /// <summary>
    /// Returns the animation counter for the current state.
    /// </summary>
    /// <returns>the animation counter for the current state.</returns>
    public override float GetAnimationCounter() { return default; }

    /// <summary>
    /// Sets the animation counter for the current state to 0.
    /// </summary>
    public override void ResetAnimationCounter() { return; }

}
