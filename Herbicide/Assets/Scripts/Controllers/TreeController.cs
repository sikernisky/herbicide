using System;
using UnityEngine;

/// <summary>
/// Controls a Tree. <br></br>
/// 
/// The TreeController is responsible for manipulating its Tree and bringing
/// it to life. This includes moving it, choosing targets, playing animations,
/// and more.
/// </summary>
/// <![CDATA[<param name="TreeState">]]>
public abstract class TreeController<T> : MobController<T> where T : Enum
{
    #region Fields

    /// <summary>
    /// The maximum number of targets a Tree can select at once.
    /// </summary>
    protected override int MAX_TARGETS => 0;

    #endregion

    #region Methods

    /// <summary>
    /// Makes a new TreeController for a Tree.
    /// </summary>
    /// <param name="tree">The Tree controlled by this TreeController.</param>
    public TreeController(Tree tree) : base(tree) { }

    /// <summary>
    /// Returns this TreeController's Tree model.
    /// </summary>
    /// <returns>this TreeController's Tree model.</returns>
    protected Tree GetTree() => GetMob() as Tree;

    /// <summary>
    /// Returns true if this controller's Tree should be destoyed and
    /// set to null.
    /// </summary>
    /// <returns>true if this controller's Tree should be destoyed and
    /// set to null; otherwise, false.</returns>
    public override bool ValidModel() => !GetTree().Dead();

    /// <summary>
    /// Returns true if the Defender can target the Model passed
    /// into this method.
    /// </summary>
    /// <param name="target">The Model to check for targetability.</param>
    /// <returns>true if the Tree can target the Model; otherwise, false. </returns>
    protected override bool CanTargetOtherModel(Model target) => false;

    /// <summary>
    /// Updates the Model's sorting order so that it appears behind Models
    /// before it and before Models behind it.
    /// </summary>
    protected override void FixSortingOrder()
    {
        if (!ValidModel()) return;

        int layer = -Mathf.FloorToInt(GetModel().GetPosition().y);
        if (GetTree().Occupied()) layer -= 1;
        GetModel().SetSortingOrder(layer);
    }

    #endregion
}
