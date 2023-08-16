using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a Tree that can host a Defender.
/// </summary>
public class BasicTree : Tree
{
    /// <summary>
    /// Type of a BasicTree.
    /// </summary>
    public override TreeType TYPE => TreeType.BASIC;

    /// <summary>
    /// Name of a BasicTree.
    /// </summary>
    protected override string NAME => "Basic Tree";

    /// <summary>
    /// How much currency it takes to place a BasicTree
    /// </summary>
    protected override int COST => 1;
}
