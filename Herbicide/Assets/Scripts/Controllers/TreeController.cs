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
    private Tree GetTree() { return GetModel() as Tree; }

    /// <summary>
    /// Returns true if this TreeController's model is a Tree and is 
    /// not NULL.
    /// </summary>
    /// <returns>true if this TreeController's model is a Tree and is 
    /// not NULL.</returns>
    public override bool ValidModel() { return GetTree() != null; }

    /// <summary>
    /// Sets this TreeController's target from a filtered list of ITargetables.
    /// </summary>
    /// <param name="filteredTargetables">a list of ITargables that this TreeController
    /// is allowed to set as its target. /// </param>
    protected override void ElectTarget(List<ITargetable> filteredTargetables)
    {
        if (!ValidModel()) return;
        if (GetTarget() != null) return;
        Assert.IsNotNull(filteredTargetables, "List of targets is null.");

        int random = UnityEngine.Random.Range(0, filteredTargetables.Count);
        if (filteredTargetables.Count == 0) SetTarget(null);
        else SetTarget(filteredTargetables[random]);
    }

    /// <summary>
    /// Parses the list of all ITargetables in the scene such that it
    /// only contains ITargetables that this TreeController's Tree is allowed
    /// to target. <br></br><br></br>
    /// 
    /// The Tree is allowed to NOTHING. Subcontrollers can override this if
    /// there is some weird Tree that can./// 
    /// </summary>
    /// <param name="targetables">the list of all ITargetables in the scene</param>
    /// <returns>a list containing Enemy ITargetables that this TreeController's Tree can
    /// reach./// </returns>
    protected override List<ITargetable> FilterTargets(List<ITargetable> targetables)
    {
        Assert.IsNotNull(targetables, "List of targets is null.");
        return new List<ITargetable>(); //Empty -- no targets.
    }

    /// <summary>
    /// Checks if this TreeController's Tree should be removed from
    /// the game. If so, clears it.
    /// </summary>
    protected override void TryRemoveModel()
    {
        if (!ValidModel()) return;
        if (GetTree().GetHealth() > 0) return;

        GetTree().OnDie();
        GameObject.Destroy(GetTree().gameObject);
        GameObject.Destroy(GetTree());

        //We are done with our Tree.
        RemoveModel();
    }
}
