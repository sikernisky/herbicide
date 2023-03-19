using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is a prototype class to represent a placeable Tree.
/// It should eventually be deleted.
/// </summary>
public class WhiteTree : Tree
{
    /// <summary>
    /// Name of this WhiteTree.
    /// </summary>
    protected override string NAME => "White Tree";

    /// <summary>
    /// Type of this WhiteTree
    /// </summary>
    public override TreeType type => TreeType.WHITE;
}
