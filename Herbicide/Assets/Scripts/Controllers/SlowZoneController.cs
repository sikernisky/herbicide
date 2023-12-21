using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System.Linq;
using UnityEditor.VersionControl;

/// <summary>
/// Class to represent controllers of SlowZones.
/// 
/// The SlowZoneController is responsible for manipulating its SlowZone and bringing
/// it to life. This includes moving it, playing animations, and more.
/// </summary>
public class SlowZoneController : HazardController<SlowZoneController.SlowZoneState>
{
    /// <summary>
    /// Number of SlowZones created since the scene began.
    /// </summary>
    private static int NUM_SLOW_ZONES;

    /// <summary>
    /// The maximum number of targets a SlowZone can select at once.
    /// </summary>
    protected override int MAX_TARGETS => int.MaxValue;

    /// <summary>
    /// Counts the number of seconds in the active animation; resets
    /// on step.
    /// </summary>
    private float activeAnimationCounter;

    /// <summary>
    /// List of targets this SlowZone is currently slowing. 
    /// </summary>
    private HashSet<PlaceableObject> slowedTargets;


    /// <summary>
    /// Different states of a SlowZone.
    /// </summary>
    public enum SlowZoneState
    {
        SPAWN,
        ACTIVE,
        DEAD
    }


    /// <summary>
    /// Assigns a SlowZoneController to a SlowZone.
    /// </summary>
    /// <param name="slowZone">The SlowZone that needs a controller.</param>
    /// <param name="startPos">where the SlowZone starts.</param>
    public SlowZoneController(SlowZone slowZone) : base(slowZone)
    {
        Assert.IsNotNull(slowZone, "SlowZone is null.");
        slowedTargets = new HashSet<PlaceableObject>();
        NUM_SLOW_ZONES++;
    }

    /// <summary>
    /// Main update loop for the SlowZone.
    /// </summary>
    protected override void UpdateMob()
    {
        base.UpdateMob();
        if (!ValidModel()) return;

        ExecuteActiveState();
    }

    /// <summary>
    /// Parses the list of all ITargetables in the scene such that it
    /// only contains ITargetables that this SlowZoneController's SlowZone is allowed
    /// to target. <br></br><br></br>
    /// 
    /// The SlowZone is allowed to target Enemies.
    /// </summary>
    /// <param name="targetables">the list of all ITargetables in the scene</param>
    /// <returns>a list containing SlowZone ITargetables that this SlowZoneController's 
    /// SlowZone can reach. </returns>
    protected override List<PlaceableObject> FilterTargets(List<PlaceableObject> targetables)
    {
        Assert.IsNotNull(targetables, "List of targets is null.");
        List<PlaceableObject> filteredTargets = new List<PlaceableObject>();
        targetables.RemoveAll(t => t == null);
        foreach (PlaceableObject target in targetables)
        {
            Enemy targetAsEnemy = target as Enemy;
            if (targetAsEnemy == null) continue; // Don't add non-Enemies
            if (slowedTargets.Contains(target)) continue; // Don't add already slowed Enemies
            float distanceToTarget = GetSlowZone().DistanceToTarget(targetAsEnemy);
            if (distanceToTarget > GetSlowZone().GetChaseRange()) continue;
            if (!targetAsEnemy.Targetable()) continue;
            filteredTargets.Add(targetAsEnemy);
        }
        return filteredTargets;
    }

    /// <summary>
    /// Handles a collision between the SlowZone and some other
    /// Collider2D.
    /// </summary>
    /// <param name="other">The other Collider2D.</param>
    protected override void HandleCollision(Collider2D other) { return; }

    /// <summary>
    /// Adds an ITargetable to the set of slowed ITargetables.
    /// </summary>
    protected void AddSlowedTarget(PlaceableObject target) { slowedTargets.Add(target); }

    /// <summary>
    /// Returns a copy of the HashSet of slowed ITargetables.
    /// </summary>
    /// <returns>a copy of the HashSet of slowed ITargetables.</returns>
    protected HashSet<PlaceableObject> GetSlowedTargets()
    {
        return new HashSet<PlaceableObject>(slowedTargets);
    }

    /// <summary>
    /// Returns this SlowZoneController's SlowZone model.
    /// </summary>
    /// <returns>this SlowZoneController's SlowZone model</returns>
    protected SlowZone GetSlowZone() { return GetHazard() as SlowZone; }

    //-----------------------STATE LOGIC----------------------//

    /// <summary>
    /// Returns true if two SlowZoneStates are equal.
    /// </summary>
    /// <param name="stateA">The first state.</param>
    /// <param name="stateB">The second state.</param>
    /// <returns>true if the two states are equal; othewise, false.</returns>
    public override bool StateEquals(SlowZoneState stateA, SlowZoneState stateB)
    {
        return stateA == stateB;
    }

    /// <summary>
    /// Updates the state of this SlowZoneController's SlowZone model.
    /// The transitions are: <br></br>
    /// 
    /// SPAWN --> ACTIVE : when spawned <br></br>
    /// ACTIVE --> DEAD : when killed or times out <br></br>
    /// </summary>
    public override void UpdateStateFSM()
    {
        switch (GetState())
        {
            case SlowZoneState.SPAWN:
                SetState(SlowZoneState.ACTIVE);
                break;
            case SlowZoneState.ACTIVE:
                if (GetSlowZone().Expired() || GetSlowZone().Dead())
                    SetState(SlowZoneState.DEAD);
                break;
            case SlowZoneState.DEAD:
                break;
        }
    }

    /// <summary>
    /// Runs logic relevant to the SlowZone's active state.
    /// </summary>
    protected virtual void ExecuteActiveState()
    {
        if (GetState() != SlowZoneState.ACTIVE) return;

        foreach (PlaceableObject target in GetTargets())
        {
            Mob mob = target as Mob;
            target.ApplyEffect(new SlowEffect(mob));
        }
    }

    //---------------------ANIMATION LOGIC----------------------//

    /// <summary>
    /// Adds one chunk of Time.deltaTime to the animation
    /// counter that tracks the current state.
    /// </summary>
    public override void AgeAnimationCounter()
    {
        SlowZoneState state = GetState();
        if (state == SlowZoneState.ACTIVE) activeAnimationCounter += Time.deltaTime;
    }

    /// <summary>
    /// Returns the animation counter for the current state.
    /// </summary>
    /// <returns>the animation counter for the current state.</returns>
    public override float GetAnimationCounter()
    {
        SlowZoneState state = GetState();
        if (state == SlowZoneState.ACTIVE) return activeAnimationCounter;
        else throw new System.Exception("State " + state + " has no counter.");
    }

    /// <summary>
    /// Sets the animation counter for the current state to 0.
    /// </summary>
    public override void ResetAnimationCounter()
    {
        SlowZoneState state = GetState();
        if (state == SlowZoneState.ACTIVE) activeAnimationCounter = 0;
    }
}
