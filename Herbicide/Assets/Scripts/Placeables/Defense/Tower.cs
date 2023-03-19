using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a placeable Tower. Does not require flooring.
/// </summary>
public class Tower : PlaceableObject
{
    //TODO: Implement in future sprint.
    protected override string NAME => throw new System.NotImplementedException();


    public override Sprite GetInventorySprite()
    {
        throw new System.NotImplementedException();
    }

    public override Sprite GetPlacementSprite()
    {
        throw new System.NotImplementedException();
    }

    public override GameObject MakePlaceableObject()
    {
        throw new System.NotImplementedException();
    }
}
