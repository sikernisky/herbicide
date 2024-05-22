using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Abstract class to represent controllers of Collectables.
/// 
/// The CollectableController is responsible for manipulating its Defender and bringing
/// it to life. This includes moving it, playing animations, and more.
/// </summary>
/// <typeparam name="T">Enum to represent state of the Collectable.</typeparam>
public abstract class CollectableController<T> : ModelController, IStateTracker<T> where T : Enum
{
    /// <summary>
    /// Current state of the Collectable.
    /// </summary>
    private T state;

    /// <summary>
    /// Current animation state of the Collectable.
    /// </summary>
    private T animationState;

    /// <summary>
    /// Total number of collectables assigned since the scene began.
    /// </summary>
    private static int NUM_COLLECTABLES;

    /// <summary>
    /// Starting position of the Collectable when it bobs up and down.
    /// </summary>
    private Vector3 bobStartPosition;

    /// <summary>
    /// Time offset to create varying bobbing animations.
    /// </summary>
    private float timeOffset;

    /// <summary>
    /// true if the Collectable fully drifted upwards.
    /// </summary>
    private bool driftedUp;

    /// <summary>
    /// true if the Collectable is performing a DriftUp lerp.
    /// </summary>
    private bool isDriftingUp = false;

    /// <summary>
    /// When the drift started.
    /// </summary>
    private float driftStartTime;

    /// <summary>
    /// How many seconds the drift should last.
    /// </summary>
    private float driftDuration = .5f;

    /// <summary>
    /// How far the Collectable should drift upwards.
    /// </summary>
    private float driftDistance;

    /// <summary>
    /// The starting scale of the collectable for the DriftUp lerp.
    /// </summary>
    private Vector3 originalScale;

    /// <summary>
    /// The starting position of the collectable for the DriftUp lerp.
    /// </summary>
    private Vector3 originalPosition;

    /// <summary>
    /// Cursor detection range.
    /// </summary>
    private float homingRange = 2f;

    /// <summary>
    /// Collection range.
    /// </summary>
    private float collectionRange = 0.25f;

    /// <summary>
    /// true if the Collectable got close enough to the cursor and should
    /// complete its homing movement. 
    /// </summary>
    private bool isHoming;


    /// <summary>
    /// Assigns a CollectableController to a Collectable Model.
    /// </summary>
    /// <param name="collectable">The Collectable to assign.</param>
    /// <param name="dropPos">Where the Collectable first dropped.</param>

    public CollectableController(Collectable collectable, Vector3 dropPos) : base(collectable)
    {
        NUM_COLLECTABLES++;
        bobStartPosition = dropPos;
        timeOffset = timeOffset = UnityEngine.Random.Range(0f, Mathf.PI * 2);
    }

    /// <summary>
    /// Main update loop for the Collectable.
    /// </summary>
    public override void UpdateController()
    {
        base.UpdateController();
        UpdateStateFSM();
    }

    /// <summary>
    /// Handles a collision between the Collectable and some other
    /// 2D collider.
    /// </summary>
    /// <param name="other">The collider the Collectable hit.</param>
    protected override void HandleCollision(Collider2D other) { return; }

    /// <summary>
    /// Returns true if this controller's Collectable should be destoyed and
    /// set to null.
    /// </summary>
    /// <returns>true if the Collectable should be nullified and destroyed;
    /// otherwise, false. </returns>
    public override bool ValidModel()
    {
        if (GetCollectable().Collected()) return false;
        return true;
    }

    /// <summary>
    /// Returns this controller's Collectable model.
    /// </summary>
    /// <returns>this controller's Collectable model.</returns>
    protected Collectable GetCollectable() { return GetModel() as Collectable; }

    /// <summary>
    /// Returns true if the Collectable is within homing range of the cursor.
    /// </summary>
    /// <returns>true if the Collectable is within homing range of the cursor;
    /// otherwise, false. /// </returns>
    protected bool InHomingRange()
    {
        Vector3 mousePos = InputController.GetWorldMousePosition();
        mousePos.z = 1;

        return Vector3.Distance(mousePos, GetCollectable().GetPosition())
            < homingRange;
    }

    /// <summary>
    /// Returns true if the Collectable is within collection range of the cursor.
    /// </summary>
    /// <returns>true if the Collectable is within collection range of the cursor;
    /// otherwise, false. /// </returns>
    protected bool InCollectionRange()
    {
        Vector3 mousePos = InputController.GetWorldMousePosition();
        mousePos.z = 1;

        return Vector3.Distance(mousePos, GetCollectable().GetPosition())
        < collectionRange;
    }


    //----------------------STATE LOGIC--------------------------//

    /// <summary>
    /// Sets the state of the Collectable.
    /// </summary>
    /// <param name="state">The state to set to.</param>
    public void SetState(T state) { this.state = state; }

    /// <summary>
    /// Returns true if two states are equal.
    /// </summary>
    /// <param name="stateA">The first state.</param>
    /// <param name="stateB">The second state.</param>
    /// <returns>true if two states are equal; otherwise, false.</returns>
    public abstract bool StateEquals(T stateA, T stateB);

    /// <summary>
    /// Returns the state of the Collectable.
    /// </summary>
    /// <returns>the state of the Collectable.</returns>
    public T GetState() { return state; }

    /// <summary>
    /// Updates the state of this CollectableController's Collectable model.
    /// </summary>
    public abstract void UpdateStateFSM();

    //---------------------ANIMATION LOGIC-----------------------//

    /// <summary>
    /// Returns the Collectable's current animation state.
    /// </summary>
    /// <returns>the Collectable's current animation state.</returns>
    public T GetAnimationState() { return animationState; }

    /// <summary>
    /// Sets the Collectable's current animation state.
    /// </summary>
    /// <param name="animationState">The animation state to set.</param>
    public void SetAnimationState(T animationState) { this.animationState = animationState; }

    //---------------------BOBBING LOGIC-----------------------//

    /// <summary>
    /// Bobs the Collectable up and down.
    /// </summary>
    protected virtual void BobUpAndDown()
    {
        float t = (Mathf.Cos((Time.time + timeOffset) * GetCollectable().BOB_SPEED) + 1) * 0.5f;
        float newY = Mathf.Lerp(-GetCollectable().BOB_HEIGHT, GetCollectable().BOB_HEIGHT, t);
        Vector3 newPosition = new Vector3(
            bobStartPosition.x,
            bobStartPosition.y + newY,
            bobStartPosition.z);
        GetCollectable().SetWorldPosition(newPosition);
    }


    /// <summary>
    /// Drifts the Collectable upwards in a straight line, slowly
    /// reducing its size until it completely dissapears.  
    /// </summary>
    /// <param name="distance">How far to drift up.</param>
    protected virtual void Drift(float distance)
    {
        if (!isDriftingUp)
        {
            // Start drifting
            isDriftingUp = true;
            driftStartTime = Time.time;
            driftDistance = distance;
            originalScale = GetCollectable().transform.localScale;
            originalPosition = GetCollectable().GetPosition();
        }
        else
        {
            // Continue drifting
            float elapsedTime = Time.time - driftStartTime;
            if (elapsedTime < driftDuration)
            {
                AnimationCurve driftCurve = GetCollectable().GetDriftUpCurve();

                // Interpolate position
                float newY = Mathf.Lerp(originalPosition.y, originalPosition.y + driftDistance, elapsedTime / driftDuration);
                Vector3 newPosition = new Vector3(originalPosition.x, newY, originalPosition.z);
                GetCollectable().SetWorldPosition(newPosition);

                // Scale based on animation curve
                float scaleMultiplier = driftCurve.Evaluate(elapsedTime / driftDuration);
                Vector3 newScale = originalScale * scaleMultiplier;
                GetCollectable().transform.localScale = newScale;
            }
            else driftedUp = true;
        }
    }

    /// <summary>
    /// Returns true if the Collectable fully drifted upwards.
    /// </summary>
    /// <returns>true if the Collectable fully drifted upwards;
    /// otherwise, false. </returns>
    protected bool DriftedUp() { return driftedUp; }

    /// <summary>
    /// Homes the collectable towards the cursor.
    /// </summary>
    protected virtual void MoveTowardsCursor()
    {
        if (!InHomingRange() && !isHoming) return;
        isHoming = true;
        float minSpeed = 4f;
        Vector2 mousePos = InputController.GetWorldMousePosition();
        float distanceToCursor = Vector2.Distance(GetCollectable().GetPosition(), mousePos);
        float normalizedDistance = Mathf.Clamp01(distanceToCursor / homingRange);
        float speedMultiplier = GetCollectable().GetHomingCurve().Evaluate(normalizedDistance);
        float movementSpeed = Mathf.Lerp(minSpeed, GetCollectable().HOME_SPEED, speedMultiplier) * Time.deltaTime;
        Vector2 currentPosition = GetCollectable().GetPosition();
        float distanceToMouse = Vector2.Distance(currentPosition, mousePos);
        float epsilon = 0.01f;
        if (distanceToMouse > epsilon)
        {
            Vector2 newPosition = Vector2.MoveTowards(currentPosition, mousePos, movementSpeed);
            GetCollectable().SetWorldPosition(newPosition);
        }
    }
}
