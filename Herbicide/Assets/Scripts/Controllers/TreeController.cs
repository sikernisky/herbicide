using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Abstract class to represent controllers of Trees.
/// </summary>
public abstract class TreeController<T> : MobController<T> where T : Enum
{

    /// <summary>
    /// Number of Trees created in the level so far.
    /// </summary>
    private static int NUM_TREES;

    /// <summary>
    /// The maximum number of targets a Tree can select at once.
    /// </summary>
    protected override int MAX_TARGETS => 0;

    /// <summary>
    /// The rate at which the Tree drops it resource.
    /// </summary>
    private float resourceDropInterval;

    /// <summary>
    /// Number of seconds since the Tree last dropped a resource.
    /// </summary>
    private float timeSinceLastDrop;

    /// <summary>
    /// Makes a new TreeController for a Tree.
    /// </summary>
    /// <param name="tree">The Tree controlled by this TreeController.</param>
    public TreeController(Tree tree) : base(tree) { NUM_TREES++; }

    /// <summary>
    /// Returns this TreeController's Tree model.
    /// </summary>
    /// <returns>this TreeController's Tree model.</returns>
    protected Tree GetTree() { return GetModel() as Tree; }

    /// <summary>
    /// Returns true if this controller's Tree should be destoyed and
    /// set to null.
    /// </summary>
    /// <returns>true if this controller's Tree should be destoyed and
    /// set to null; otherwise, false.</returns>
    protected override bool ShouldRemoveModel()
    {
        if (GetTree().GetHealth() <= 0) return true;

        return false;
    }

    /// <summary>
    /// Returns true if the Defender can target the PlaceableObject passed
    /// into this method.
    /// </summary>
    /// <param name="target">The Placeable object to check for targetability.</param>
    /// <returns></returns>
    protected override bool CanTarget(PlaceableObject target)
    {
        return false;
    }

    /// <summary>
    /// Different Trees output different resources; outputs the
    /// correct resource for the Tree model.
    /// </summary>
    protected void EmitResources()
    {
        resourceDropInterval = 1f / GetTree().GetResourceDropRate();
        timeSinceLastDrop += Time.deltaTime;
        if (timeSinceLastDrop >= resourceDropInterval)
        {
            DropResources();
            timeSinceLastDrop = 0;
        }
    }

    /// <summary>
    /// Drops one prefab of the Tree's resource.
    /// </summary> <summary>
    protected abstract void DropResources();
}
