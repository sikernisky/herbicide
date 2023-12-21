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
    /// Updates the Tree controlled by this TreeController.
    /// </summary>
    /// <param name="targets">A complete list of ITargetables in the scene.</param>
    protected override void UpdateMob()
    {
        if (!ValidModel()) return;
        if (GetGameState() != GameState.ONGOING) return;
        UpdateStateFSM();
        ElectTarget(FilterTargets(GetAllTargetableObjects()));
    }

    /// <summary>
    /// Returns this TreeController's Tree model.
    /// </summary>
    /// <returns>this TreeController's Tree model.</returns>
    protected Tree GetTree() { return GetModel() as Tree; }

    /// <summary>
    /// Parses the list of all PlaceableObjects in the scene such that it
    /// only contains PlaceableObjects that this TreeController's Tree is allowed
    /// to target. <br></br><br></br>
    /// 
    /// The Tree is allowed to NOTHING. Subcontrollers can override this if
    /// there is some weird Tree that can./// 
    /// </summary>
    /// <param name="targetables">the list of all PlaceableObjects in the scene</param>
    /// <returns>a list containing Enemy PlaceableObjects that this TreeController's Tree can
    /// reach./// </returns>
    protected override List<PlaceableObject> FilterTargets(List<PlaceableObject> targetables)
    {
        Assert.IsNotNull(targetables, "List of targets is null.");
        return new List<PlaceableObject>(); //Empty -- no targets.
    }

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
