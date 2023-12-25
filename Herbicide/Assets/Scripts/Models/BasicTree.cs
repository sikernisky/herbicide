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
    public override ModelType TYPE => ModelType.BASIC_TREE;

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
    /// Amount of attack cooldown this BasicTree starts with.
    /// </summary>
    public override float BASE_ATTACK_COOLDOWN => 0;

    /// <summary>
    /// Most amount of attack cooldown this BasicTree can have.
    /// </summary>
    public override float MAX_ATTACK_COOLDOWN => 0;

    /// <summary>
    /// Starting chase range of a BasicTree.
    /// </summary>
    public override float BASE_CHASE_RANGE => 0f;

    /// <summary>
    /// Maximum chase range of a BasicTree.
    /// </summary>
    public override float MAX_CHASE_RANGE => 0f;

    /// <summary>
    /// Minimum chase range of a BasicTree.
    /// </summary>
    public override float MIN_CHASE_RANGE => 0f;

    /// <summary>
    /// Starting movement speed of a BasicTree.
    /// </summary>
    public override float BASE_MOVEMENT_SPEED => 0f;

    /// <summary>
    /// Maximum movement speed of a BasicTree.
    /// </summary>
    public override float MAX_MOVEMENT_SPEED => 0f;

    /// <summary>
    /// Minumum movement speed of a BasicTree.
    /// </summary>
    public override float MIN_MOVEMENT_SPEED => 0f;

    /// <summary>
    /// Sets this BasicTree's 2D Collider's properties.
    /// </summary>
    public override void SetColliderProperties() { return; }
}
