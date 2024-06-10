using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Represents a Tree.
/// </summary>
public abstract class Tree : Mob, ISurface
{
    /// <summary>
    /// How far to push a hosted Defender horizontally
    /// </summary>
    public virtual float DEFENDER_OFFSET_X => 0f;

    /// <summary>
    /// How far to push a hosted Defender vertically
    /// </summary>
    public virtual float DEFENDER_OFFSET_Y => .75f;

    /// <summary>
    /// Starting number of Collectable Prefabs this Tree drops each second.
    /// </summary>
    public virtual float BASE_RESOURCE_DROP_RATE => 0f;

    /// <summary>
    /// Maximum number of Collectable Prefabs this Tree can drop each second.
    /// </summary>
    public virtual float MIN_RESOURCE_DROP_RATE => 0f;

    /// <summary>
    /// Minimum number of Collectable Prefabs this Tree can drop each second.
    /// </summary>
    public virtual float MAX_RESOURCE_DROP_RATE => float.MaxValue;

    /// <summary>
    /// This Tree's current resource drop rate.
    /// </summary>
    private float resourceDropRate;

    /// <summary>
    /// This Tree's neighboring ISurfaces.
    /// </summary>
    private ISurface[] surfaceNeighbors;

    /// <summary>
    /// The Defender on this Tree; null if no Defender is.
    /// </summary>
    private Defender defender;

    /// <summary>
    /// The Defender ghost placing on this Tree.
    /// </summary>
    private GameObject ghostDefender;

    /// <summary>
    /// Trees occupy Tiles & Flooring.
    /// </summary>
    public override bool OCCUPIER => true;


    /// <summary>
    /// Resets this Tree's stats to their starting values.
    /// </summary>
    public override void ResetModel()
    {
        base.ResetModel();
        ResetResourceDropRate();
    }

    /// <summary>
    /// If this Tree is not occupied, adjusts its health by amount.
    /// If it is occupied, adjusts its occupant's health instead.  
    /// </summary>
    /// <param name="amount">The amount to adjust by.</param>
    public override void AdjustHealth(float amount)
    {
        if (Occupied()) defender.AdjustHealth(amount);
        else base.AdjustHealth(amount);
    }

    /// <summary>
    /// Returns true if there is an occupant on this Tree.
    /// </summary>
    /// <returns>true if there is an occupant on this Tree; otherwise,
    /// false.</returns>
    public bool Occupied() { return defender != null; }

    /// <summary>
    /// Returns true if a Defender can place on this Tree. If
    /// so, places it.
    /// </summary>
    /// <param name="candidate">The candidate Defender.</param>
    /// <param name="neighbors">This Tree's neighboring ISurfaces.</param>
    /// <returns>true if a Defender was placed on this Tree; otherwise,
    /// false. </returns>
    public void Place(PlaceableObject candidate, ISurface[] neighbors)
    {
        Assert.IsNotNull(candidate, "Placement candidate can't be null.");
        Assert.IsNotNull(neighbors, "Placement candidate's neighbors can't be null.");
        Assert.IsTrue(CanPlace(candidate, neighbors), "Need to make sure placement is valid.");


        SpriteRenderer prefabRenderer = candidate.GetComponent<SpriteRenderer>();

        defender = candidate.GetComponent<Defender>();

        string placeName = defender.NAME.ToLower() + "Place";
        SoundController.PlaySoundEffect(placeName);
        defender.SetTreePosition(GetPosition());
        candidate.transform.SetParent(transform);
        candidate.transform.localPosition =
            new Vector3(DEFENDER_OFFSET_X, DEFENDER_OFFSET_Y, 1);
        candidate.transform.localScale = defender.GetPlacementScale();
        candidate.OnPlace(new Vector2Int(GetX(), GetY()));
    }

    /// <summary>
    /// Returns true if a PlaceableObject can be placed on this Tree. A
    /// PlaceableObject can be placed if it is a Defender, not null, and
    /// this Tree is not already occupied. 
    /// </summary>
    /// <param name="candidate">The candidate PlaceableObject.</param>
    /// <param name="neighbors">This Tree's neighboring ISurfaces.</param>
    /// <returns>true if a PlaceableObject can place on this Tree; otherwise,
    /// returns false. </returns>
    public bool CanPlace(PlaceableObject candidate, ISurface[] neighbors)
    {
        ModelType candidateType = candidate.TYPE;
        List<ModelType> acceptedTypes = new List<ModelType>()
        {
            ModelType.SQUIRREL,
            ModelType.BEAR,
            ModelType.HEDGEHOG
        };

        if (candidate == null || neighbors == null) return false;
        if (Occupied()) return false;
        if (!acceptedTypes.Contains(candidateType)) return false;
        return true;
    }

    /// <summary>
    /// Removes the PlaceableObject from this Tree. This does not
    /// destroy the occupant; that is the responsibility of its controller. 
    /// </summary>
    /// <param name="neighbors">This Tree's neighbors.</param>
    public virtual void Remove(ISurface[] neighbors)
    {
        Assert.IsTrue(CanRemove(neighbors), "Need to check removal validity.");

        defender = null;
    }

    /// <summary>
    /// Returns true if there is a PlaceableObject on this Tree that can be
    /// removed. 
    /// /// </summary>
    /// <param name="neighbors">This Tree's neighbors.</param>
    /// <returns>true if there is a PlaceableObject on this Tree that can be
    /// removed; otherwise, false. </returns>
    public virtual bool CanRemove(ISurface[] neighbors)
    {
        Assert.IsNotNull(neighbors, "Array of neighbors is null.");

        if (!Occupied()) return false;

        return true;
    }

    /// <summary>
    /// Updates this Tree's understanding of its neighboring ISurfaces.
    /// </summary>
    /// <param name="newNeighbors">This Tree's new neighbors.</param>
    public void UpdateSurfaceNeighbors(ISurface[] newNeighbors)
    {
        if (newNeighbors == null) return;
        surfaceNeighbors = newNeighbors;
    }

    /// <summary>
    /// Returns a copy of this Tree's neighboring ISurfaces.
    /// </summary>
    /// <returns>An array containing this Tree's neighboring ISurfaces.</returns>
    public ISurface[] GetSurfaceNeighbors()
    {
        ISurface[] outNeighbors = new ISurface[surfaceNeighbors.Length];
        for (int i = 0; i < surfaceNeighbors.Length; i++)
        {
            outNeighbors[i] = surfaceNeighbors[i];
        }
        return outNeighbors;
    }

    /// <summary>
    /// Returns an array of PlaceableObjects that neighbor this Tree.
    /// </summary>
    /// <returns>An array of PlaceableObjects that neighbor this Tree.</returns>
    public PlaceableObject[] GetPlaceableObjectNeighbors()
    {
        PlaceableObject[] outNeighbors = new PlaceableObject[surfaceNeighbors.Length];
        for (int i = 0; i < surfaceNeighbors.Length; i++)
        {
            PlaceableObject placeable = surfaceNeighbors[i] as PlaceableObject;
            if (placeable != null) outNeighbors[i] = placeable;
        }
        return outNeighbors;
    }

    /// <summary>
    /// Returns the PlaceableObject on this Tree.
    /// </summary>
    /// <returns>The PlaceableObject on this Tree, or null if there
    /// is no PlaceableObject on this Tree. </returns>
    public PlaceableObject GetPlaceableObject() { return defender; }

    /// <summary>
    /// Provides a visual simulation of placing a Defender on
    /// this Tree and is called during a hover / placement action.
    /// This method does not carry out actual placement of the Defender on
    /// this Tree. Instead, it displays a potential placement scenario.
    /// </summary>
    /// <param name="ghost">The PlaceableObject (Defender) object that we are
    /// trying to virtually place on this Tile.</param>
    /// <returns> true if the ghost place was successful; otherwise,
    /// false. </returns> 
    public bool GhostPlace(PlaceableObject ghost)
    {
        if (!CanGhostPlace(ghost)) return false;

        GameObject hollowCopy = (ghost as PlaceableObject).MakeHollowObject();
        Assert.IsNotNull(hollowCopy);
        SpriteRenderer hollowRenderer = hollowCopy.GetComponent<SpriteRenderer>();
        Assert.IsNotNull(hollowRenderer);

        hollowRenderer.sortingLayerName = "collectables";
        hollowRenderer.sortingOrder = GetSortingOrder() + 1;
        hollowRenderer.color = new Color32(255, 255, 255, 200);
        hollowCopy.transform.SetParent(transform);
        hollowCopy.transform.localPosition =
            new Vector3(DEFENDER_OFFSET_X, DEFENDER_OFFSET_Y, 1);
        hollowCopy.transform.localScale = Vector3.one;
        hollowCopy.transform.SetParent(transform);

        ghostDefender = hollowCopy;
        return true;
    }

    /// <summary>
    /// Determines whether a PlaceableObject object can be potentially placed
    /// on this Tree. This method is invoked alongside GhostPlace() during a
    /// hover or placement action to validate the placement feasibility.
    /// </summary>
    /// <param name="ghost">The PlaceableObject (Defender) object that we are
    /// trying to virtually place on this Tree.</param>
    /// <returns>true if the PlaceableObject (Defender) object can be placed on this Tree;
    /// otherwise, false.</returns>
    public bool CanGhostPlace(PlaceableObject ghost)
    {
        return !Occupied() && ghostDefender == null && ghost as Defender != null
            && CanPlace(ghost, GetSurfaceNeighbors());
    }

    /// <summary>
    /// Removes all visual simulations of placing a PlaceableObject on this
    /// Tree. If there are none, does nothing.
    /// </summary>
    public void GhostRemove()
    {
        if (ghostDefender == null) return;

        Destroy(ghostDefender);
        ghostDefender = null;
    }

    /// <summary>
    /// Sets whether an Enemy is on this Tree.
    /// </summary>
    /// <param name="occupiedByEnemy">whether an Enemy is on this Tree.</param>
    public void SetOccupiedByEnemy(bool occupiedByEnemy) { return; }

    /// <summary>
    /// Returns true if an Enemy is on this Tree.
    /// </summary>
    /// <returns>true if an Enemy is on this Tree; otherwise,
    /// false.</returns>
    public virtual bool OccupiedByEnemy() { return false; }

    /// <summary>
    /// Returns true if the ghost occupant on this Tree is not null.
    /// </summary>
    /// <returns>true if the ghost occupant on this Tree is not null;
    /// otherwise, false.</returns>
    public bool HasActiveGhostOccupant() { return ghostDefender != null; }

    /// <summary>
    /// Returns true if a pathfinder can walk across this Tree.
    /// </summary>
    /// <returns>true if a pathdfinder can walk across this Tree;
    /// otherwise, false.</returns>
    public bool IsWalkable() { return false; }

    /// <summary>
    /// Resets this Tree's resource drop rate to its base value.
    /// </summary>
    protected void ResetResourceDropRate() { resourceDropRate = BASE_RESOURCE_DROP_RATE; }

    /// <summary>
    /// Adds some amount to this Tree's resource drop rate.
    /// </summary>
    /// <param name="amount">The amount to add.</param>
    public void AdjustResourceDropRate(float amount)
    {
        resourceDropRate = Mathf.Clamp(resourceDropRate + amount,
         MIN_RESOURCE_DROP_RATE,
         MAX_RESOURCE_DROP_RATE);
    }

    /// <summary>
    /// Returns this Tree's current resource drop rate.
    /// </summary>
    /// <returns>this Tree's current resource drop rate.</returns>
    public float GetResourceDropRate() { return resourceDropRate; }
}
