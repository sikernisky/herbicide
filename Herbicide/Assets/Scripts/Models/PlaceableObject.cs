using UnityEngine;
using System.Linq;
using UnityEngine.Assertions;
using System.Collections.Generic;
using System;

/// <summary>
/// Abstract class for something unreactive that can be placed on the
/// TileGrid. These objects can be targeted, but only have
/// one base animation / state. <br></br>
/// 
/// If the PlaceableObject should
/// react to the world and update its animation based off
/// game events, please inherit from Mob. 
/// </summary>
public abstract class PlaceableObject : Model
{
    /// <summary>
    /// How much currency is required to place this PlaceableObject.
    /// </summary>
    public abstract int COST { get; }

    /// <summary>
    /// The scale of this PlaceableObject when placed.
    /// </summary>
    protected virtual Vector3 PLACEMENT_SCALE => Vector3.one;

    /// <summary>
    /// true if this PlaceableObject occupies a Tile, preventing
    /// further placement; otherwise, false.
    /// </summary>
    public abstract bool OCCUPIER { get; }

    /// <summary>
    /// Returns this PlaceableObject's placement scale.
    /// </summary>
    /// <returns>this PlaceableObject's placement scale.</returns>
    public Vector3 GetPlacementScale() { return PLACEMENT_SCALE; }

    /// <summary>
    /// Returns the GameObject that represents this PlaceableObject on the grid.
    /// </summary>
    /// <returns>the GameObject that represents this PlaceableObject on the grid.
    /// </returns>
    public abstract GameObject MakePlaceableObject();

    /// <summary>
    /// Returns a GameObject that holds a SpriteRenderer component with
    /// this PlaceableObject's placed Sprite. No other components are
    /// copied. 
    /// </summary>
    /// <returns>A GameObject with a SpriteRenderer component. </returns>
    public virtual GameObject MakeHollowObject()
    {
        GameObject hollowCopy = new GameObject("Hollow " + NAME);
        SpriteRenderer hollowRenderer = hollowCopy.AddComponent<SpriteRenderer>();
        hollowRenderer.sprite = GetSprite();
        hollowCopy.transform.position = transform.position;
        hollowCopy.transform.localScale = transform.localScale;
        return hollowCopy;
    }

    /// <summary>
    /// Returns true if this PlaceableObject is "Dead", which implementers
    /// define in their own way.
    /// </summary>
    /// <param name="GetHealth("></param>
    /// <returns>true if this PlaceableObject is "Dead"; otherwise,
    /// false. </returns>
    public abstract bool Dead();

    /// <summary>
    /// Returns true if something can target this PlaceableObject. As 
    /// a base/default, this is when this PlaceableObject is not dead. 
    /// </summary>
    /// <returns>true if something can target this PlaceableObject;
    /// otherwise, false</returns>
    public virtual bool Targetable() { return !Dead(); }

    /// <summary>
    /// Returns the position of where attacks directed at this PlaceableObject
    /// should go.
    /// </summary>
    /// <returns>the position of where attacks directed at this PlaceableObject
    /// should go.</returns>
    public virtual Vector3 GetAttackPosition() { return GetPosition(); }

    /// <summary>
    /// Called when this PlaceableObject is placed.
    /// </summary>
    public virtual void OnPlace() { return; }
}
