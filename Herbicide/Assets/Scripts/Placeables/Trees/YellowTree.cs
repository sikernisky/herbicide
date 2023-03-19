using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is a prototype class to represent a placeable Tree.
/// It should eventually be deleted.
/// </summary>
public class YellowTree : Tree
{
    /// <summary>
    /// Name of this YellowTree.
    /// </summary>
    protected override string NAME => "Yellow Tree";

    /// <summary>
    /// Type of this YellowTree
    /// </summary>
    public override TreeType type => TreeType.YELLOW;
}
