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
    /// The animation counter for when the boat is moving.
    /// </summary>
    private float cruiseAnimationCounter;

    /// <summary>
    /// Main update loop for the ShopBoat.
    /// </summary>
    protected override void UpdateMob()
    {
        base.UpdateMob();
        ExecuteCruiseFullState();
        ExecuteCruiseEmptyState();
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
    protected override bool ShouldRemoveModel()
    {
        if (GetBoat().Dead()) return true;

        return false;
    }

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
                if (GetBoat().Purchased()) SetState(ShopBoatState.CRUISE_EMPTY);
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

        GetBoat().SetAnimationDuration(GetBoat().MOVE_ANIMATION_DURATION);
        Sprite[] cruiseTrack = ShopBoatFactory.GetMovementTrack();
        if (GetAnimationState() != ShopBoatState.CRUISE_FULL) GetBoat().SetAnimationTrack(cruiseTrack);
        else GetBoat().SetAnimationTrack(cruiseTrack, GetBoat().CurrentFrame);
        SetAnimationState(ShopBoatState.CRUISE_FULL);

        StepAnimation();
        GetBoat().SetSprite(GetBoat().GetSpriteAtCurrentFrame());

        WaveMove();

        // Enough money to buy, start placing.
        if (ModelClickedUp() && EconomyController.GetBalance() >= GetBoat().GetRiderPrice()
            && GetGameState() == GameState.ONGOING)
        {
            GetBoat().BuyRider();
            EconomyController.Withdraw(GetBoat().GetRiderPrice());

            if (PlacementController.Placing()) return;

            //Start the placement event
            Model occupant = GetBoat().GetRider();
            if (occupant == null) return;
            PlacementController.StartPlacingObject(occupant);
        }
        GetBoat().UpdateSignPrice();
    }

    /// <summary>
    /// Runs logic for the ShopBoat's CruiseEmpty state.
    /// </summary>
    protected virtual void ExecuteCruiseEmptyState()
    {
        if (GetState() != ShopBoatState.CRUISE_EMPTY) return;
        if (!ValidModel()) return;

        StepAnimation();
        GetBoat().SetSprite(GetBoat().GetSpriteAtCurrentFrame());

        WaveMove();
    }

    //--------------------END STATE LOGIC----------------------//

    /// <summary>
    /// Moves the shopboat model to its destination in a wave-like
    /// motion.
    /// </summary>
    private void WaveMove()
    {
        ShopBoat shopBoat = GetBoat();
        float boatSpeed = shopBoat.GetMovementSpeed();
        float deltaTime = Time.deltaTime;

        Vector3 newPosition = shopBoat.GetPosition();

        // Move to the right for now.
        newPosition.x += boatSpeed * deltaTime;

        // Waves
        float waveAmplitude = .002f; // Height
        float waveFrequency = 2.0f; // Speed
        newPosition.y += waveAmplitude * Mathf.Sin(Time.time * waveFrequency);

        shopBoat.SetWorldPosition(newPosition);
    }

    /// <summary>
    /// Returns true if the ShopBoat can target the Model passed
    /// into this method.
    /// </summary>
    /// <param name="target">The Model to check.</param>
    /// <returns>true if the ShopBoat can target the Model; otherwise,
    /// false.</returns>
    protected override bool CanTarget(Model target) { return false; }

    /// <summary>
    /// Adds one chunk of Time.deltaTime to the animation
    /// counter that tracks the current state.
    /// </summary>
    public override void AgeAnimationCounter()
    {
        ShopBoatState state = GetState();
        if (state == ShopBoatState.CRUISE_FULL) cruiseAnimationCounter += Time.deltaTime;
        else if (state == ShopBoatState.CRUISE_EMPTY) cruiseAnimationCounter += Time.deltaTime;
    }

    /// <summary>
    /// Returns the animation counter for the current state.
    /// </summary>
    /// <returns>the animation counter for the current state.</returns>
    public override float GetAnimationCounter()
    {
        ShopBoatState state = GetState();
        if (state == ShopBoatState.CRUISE_FULL) return cruiseAnimationCounter;
        else if (state == ShopBoatState.CRUISE_EMPTY) return cruiseAnimationCounter;
        else throw new System.Exception("State " + state + " has no counter.");
    }

    /// <summary>
    /// Sets the animation counter for the current state to 0.
    /// </summary>
    public override void ResetAnimationCounter()
    {
        ShopBoatState state = GetState();
        if (state == ShopBoatState.CRUISE_FULL) cruiseAnimationCounter = 0;
        else if (state == ShopBoatState.CRUISE_EMPTY) cruiseAnimationCounter = 0;
    }

}
