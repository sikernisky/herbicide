using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a Tree that can host a Defender.
/// </summary>
public class BasicTree : Tree
{
    /// <summary>
    /// Name of a BasicTree.
    /// </summary>
    public override string NAME => "Basic Tree";

    /// <summary>
    /// How much currency it takes to place a BasicTree
    /// </summary>
    public override int COST => 1;

    /// <summary>
    /// Type of a BasicTree.
    /// </summary>
    public override TreeType TYPE => TreeType.BASIC;

    /// <summary>
    /// Starting health of a BasicTree.
    /// </summary>
    public override int BASE_HEALTH => 200;

    /// <summary>
    /// Maximum health of a BasicTree.
    /// </summary>
    public override int MAX_HEALTH => 200;

    /// <summary>
    /// Minimum health of a BasicTree.
    /// </summary>
    public override int MIN_HEALTH => 0;

    /// <summary>
    /// Starting attack range of a BasicTree.
    /// </summary>
    public override float BASE_ATTACK_RANGE => 0;

    /// <summary>
    /// Maximum attack range of a BasicTree.
    /// </summary>
    public override float MAX_ATTACK_RANGE => 0;

    /// <summary>
    /// Minimum attack range of a BasicTree.
    /// </summary>
    public override float MIN_ATTACK_RANGE => 0;

    /// <summary>
    /// Starting attack speed of a BasicTree.
    /// </summary>
    public override float BASE_ATTACK_SPEED => 0;

    /// <summary>
    /// Maximum attack speed of a BasicTree.
    /// </summary>
    public override float MAX_ATTACK_SPEED => throw new System.NotImplementedException();

    /// <summary>
    /// Minimum attack speed of a BasicTree.
    /// </summary>
    public override float MIN_ATTACK_SPEED => throw new System.NotImplementedException();


    /// <summary>
    /// Called when this BasicTree dies.
    /// </summary>
    public override void OnDie() { return; }

    /// <summary>
    /// Sets this BasicTree's 2D Collider's properties.
    /// </summary>
    public override void SetColliderProperties() { return; }
}
