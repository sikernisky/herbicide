using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Represents a GameObject that can be placed on a Tile to expand its
/// behavior and complexity. 
/// </summary>
public abstract class Flooring : Model, ISurface
{
    /// <summary>
    /// Index of the Sprite in tile set this Flooring takes on
    /// </summary>
    private int tilingIndex;

    /// <summary>
    /// Type of this Flooring
    /// </summary>
    protected abstract FlooringType type { get; }

    /// <summary>
    /// Type of Tile this flooring is on
    /// </summary>
    private Tile.TileType typeOn;

    /// <summary>
    /// This Flooring's four neighbors.
    /// </summary>
    private Flooring[] neighbors;

    /// <summary>
    /// This Flooring's SpriteRenderer component
    /// </summary>
    private SpriteRenderer flooringRenderer;

    /// <summary>
    /// The PlaceableObject on this Flooring.
    /// </summary>
    private PlaceableObject occupant;

    /// <summary>
    /// The ghost/preview of something that would place on this Flooring.
    /// </summary>
    private GameObject ghostOccupant;

    /// <summary>
    /// true if an Enemy is on this Flooring; otherwise, false.
    /// </summary>
    private bool occupiedByEnemy;

    /// <summary>
    /// true if this Flooring is defined
    /// </summary>
    private bool defined;

    /// <summary>
    /// Unique types of Floorings in the TileGrid.
    /// </summary>
    public enum FlooringType
    {
        SOIL
    }


    /// <summary>
    /// Defines this Flooring on a Tile, setting its Sprite based on an
    /// index.
    /// </summary>
    /// <param name="typeOn">type of Tile this Flooring sits on</param>
    /// <param name="neighbors">this Flooring's neighbors</param>
    public virtual void Define(int x, int y, Tile.TileType typeOn, ISurface[] neighbors)
    {
        if (neighbors == null) return;
        if (flooringRenderer == null)
            flooringRenderer = GetComponent<SpriteRenderer>();

        defined = true;
        SetTileCoordinates(x, y);
        name = type.ToString() + " (" + GetX() + ", " + GetY() + ")";
        UpdateSurfaceNeighbors(neighbors);
        this.typeOn = typeOn;
    }

    /// <summary>
    /// Sets the neighbors of this Flooring.
    /// </summary>
    /// <param name="newNeighbors">this Floorings's new neighbors.</param>
    public void UpdateSurfaceNeighbors(ISurface[] newNeighbors)
    {
        AssertDefined();
        Flooring[] flooringNeighbors = new Flooring[newNeighbors.Length];
        for (int i = 0; i < newNeighbors.Length; i++)
        {
            flooringNeighbors[i] = newNeighbors[i] as Flooring;
        }
        SetTilingIndex(GetTilingIndex(flooringNeighbors));
        neighbors = flooringNeighbors;
    }

    /// <summary>
    /// Returns this Flooring's four neighbors.
    /// </summary>
    /// <returns>this Flooring's four neighbors.</returns>
    public ISurface[] GetSurfaceNeighbors()
    {
        AssertDefined();
        return neighbors;
    }

    /// <summary>
    /// Returns this Floorings's four neighbors' PlaceableObjects.
    /// </summary>
    /// <returns>this Floorings's four neighbors' PlaceableObjects.</returns>
    public PlaceableObject[] GetPlaceableObjectNeighbors()
    {
        AssertDefined();
        if (neighbors == null) return null;

        PlaceableObject[] placeableNeighbors = new PlaceableObject[4];
        for (int i = 0; i < neighbors.Length; i++)
        {
            ISurface neighbor = neighbors[i];
            if (neighbor != null) placeableNeighbors[i] = neighbor.GetPlaceableObject();
        }

        return placeableNeighbors;
    }

    /// <summary>
    /// Sets this Flooring's tiling index and updates its Sprite to reflect
    /// that index.
    /// </summary>
    /// <param name="newIndex">the new index to set to.</param>
    public void SetTilingIndex(int newIndex)
    {
        AssertDefined();
        if (!FlooringFactory.ValidFlooringIndex(newIndex)) return;
        if (flooringRenderer == null) flooringRenderer = GetComponent<SpriteRenderer>();
        flooringRenderer.sprite = FlooringFactory.GetFlooringSprite(type, newIndex);
        tilingIndex = newIndex;
    }

    /// <summary>
    /// Returns true if this Flooring hosts an IPlaceable.
    /// </summary>
    /// <returns>true if this Flooring hosts an IPlaceable; otherwise, false.
    /// </returns>
    public bool Occupied()
    {
        AssertDefined();
        return occupant != null;
    }


    /// <summary>
    /// Returns true if a PlaceableObject can be placed on this Flooring.
    /// If it can, places the PlaceableObject.
    /// </summary>
    /// <param name="candidate">The PlaceableObject to place.</param>
    /// <param name="neighbors">This Flooring's neighbors.</param>
    /// <returns>true if a PlaceableObject can be placed on this Flooring;
    /// otherwise, false.</returns>
    public bool Place(PlaceableObject candidate, ISurface[] neighbors)
    {
        AssertDefined();
        if (candidate == null || neighbors == null) return false;

        //1. If occupied with a Tree, and the candidate is a defender, pass event to that Tree.
        if (Occupied() && candidate as Defender != null)
        {
            Tree tree = occupant as Tree;
            if (tree != null)
            {
                return tree.Place(candidate as Defender, neighbors);
            }
        }

        //2. Try to place on Flooring
        if (!CanPlace(candidate, neighbors)) return false;

        GameObject prefabClone = candidate.MakePlaceableObject();
        Assert.IsNotNull(prefabClone);
        PlaceableObject placeableObject = prefabClone.GetComponent<PlaceableObject>();
        Assert.IsNotNull(placeableObject);
        SpriteRenderer prefabRenderer = prefabClone.GetComponent<SpriteRenderer>();

        prefabRenderer.sortingOrder = GetY();
        prefabClone.transform.position = transform.position;
        prefabClone.transform.localScale = placeableObject.GetPlacementScale();
        prefabClone.transform.SetParent(transform);
        if (placeableObject.OCCUPIER) occupant = placeableObject;
        placeableObject.OnPlace();
        SetTilingIndex(GetTilingIndex(neighbors));

        //Did we place a Tree? Give it a controller and neighbors.
        Tree placedTree = placeableObject as Tree;
        if (placedTree != null)
        {
            ControllerController.MakeTreeController(placedTree);
            placedTree.UpdateSurfaceNeighbors(neighbors);
        }

        return true;
    }


    /// <summary>
    /// Sets the color of this Tile's SpriteRenderer. If it's floored, passes
    /// this paint command to the flooring instead.
    /// </summary>
    /// <param name="paintColor">the color with which to paint this Tile.</param>
    public override void SetColor(Color32 paintColor) { base.SetColor(paintColor); }

    /// <summary>
    /// Returns true if a PlaceableObject can be placed on this Flooring.
    /// </summary>
    /// <param name="candidate">The PlaceableObject to place.</param>
    /// <param name="neighbors">This Flooring's neighbors.</param>
    /// <returns>true if a PlaceableObject can be placed on this Flooring;
    /// otherwise, false.</returns>
    public bool CanPlace(PlaceableObject candidate, ISurface[] neighbors)
    {
        AssertDefined();
        if (Occupied()) return false;
        if (candidate == null || neighbors == null) return false;
        if (candidate as Squirrel != null) return false;

        return true;
    }

    /// <summary>
    /// Returns true if there is an IPlaceable on this Flooring that can
    /// be removed. If so, removes the IPlaceable.
    /// </summary>
    /// <param name="neighbors">This Flooring's neighbors.</param>
    /// <returns>true if an IPlaceable can be removed from this Flooring;
    /// otherwise, false.</returns>
    public bool Remove(ISurface[] neighbors)
    {
        AssertDefined();
        if (!Occupied() || neighbors == null) return false;

        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Returns the index representing the correct Sprite in this 
    /// Flooring's tile set. The correct sprite is determined by whether its
    /// neighbors are null or valid. <br></br>
    /// </summary>
    /// <param name="neighbors">this Flooring's neighbors</param>
    /// <returns>the index representing the correct Sprite in this 
    /// Flooring's tile set.</returns>
    protected abstract int GetTilingIndex(ISurface[] neighbors);

    /// <summary>
    /// Returns this Tile's TileType.
    /// </summary>
    /// <returns>this Tile's TileType.</returns>
    public FlooringType GetTileType()
    {
        AssertDefined();
        return type;
    }

    /// <summary>
    /// Asserts that this Flooring is defined.
    /// </summary>
    public void AssertDefined()
    {
        Assert.IsTrue(defined);
    }

    /// <summary>
    /// Returns the PlaceableObject on this Flooring.
    /// </summary>
    /// <returns>the PlaceableObject on this Flooring; null if there
    /// is none</returns>
    public PlaceableObject GetPlaceableObject() { return occupant; }

    /// <summary>
    /// Determines whether a PlaceableObject object can be potentially placed
    /// on this Flooring. This method is invoked alongside GhostPlace() during a
    /// hover or placement action to validate the placement feasibility.
    /// </summary>
    /// <param name="ghost">The PlaceableObject object that we are
    /// trying to virtually place on this TiFlooringle.</param>
    /// <returns>true if the PlaceableObject object can be placed on this Flooring;
    /// otherwise, false.</returns>
    public bool CanGhostPlace(PlaceableObject ghost)
    {
        return !Occupied() && ghostOccupant == null && ghost != null
            && CanPlace(ghost, GetSurfaceNeighbors());
    }

    /// <summary>
    /// Provides a visual simulation of placing a PlaceableObject on
    /// this Flooring and is called during a hover / placement action.
    /// This method does not carry out actual placement of the PlaceableObject on
    /// this Flooring. Instead, it displays a potential placement scenario.
    /// </summary>
    /// <param name="ghost">The PlaceableObject object that we are
    /// trying to virtually place on this Flooring.</param>
    /// <returns> true if the ghost place was successful; otherwise,
    /// false. </returns> 
    public bool GhostPlace(PlaceableObject ghost)
    {
        //Occupied by a Tree? Pass the ghost place.
        if (Occupied())
        {
            Tree treeOcc = GetPlaceableObject() as Tree;
            if (treeOcc != null) return treeOcc.GhostPlace(ghost);
        }

        if (!CanGhostPlace(ghost)) return false;

        GameObject hollowCopy = (ghost as PlaceableObject).MakeHollowObject();
        Assert.IsNotNull(hollowCopy);
        SpriteRenderer hollowRenderer = hollowCopy.GetComponent<SpriteRenderer>();
        Assert.IsNotNull(hollowRenderer);

        hollowRenderer.sortingOrder = GetY();
        hollowRenderer.color = new Color32(255, 255, 255, 200);
        hollowCopy.transform.position = transform.position;
        hollowCopy.transform.localScale = Vector3.one;
        hollowCopy.transform.SetParent(transform);

        ghostOccupant = hollowCopy;
        return true;
    }


    /// <summary>
    /// Removes all visual simulations of placing an IPlaceable on this
    /// Flooring. If there are none, does nothing.
    /// </summary>
    public void GhostRemove()
    {
        Tree treeOcc = GetPlaceableObject() as Tree;
        if (treeOcc != null) treeOcc.GhostRemove();

        if (ghostOccupant == null) return;

        Destroy(ghostOccupant);
        ghostOccupant = null;
    }

    /// <summary>
    /// Sets whether an Enemy is on this Flooring.
    /// </summary>
    /// <param name="occupiedByEnemy">true if an Enemy is 
    /// on this Flooring; otherwise, false.</param>
    public void SetOccupiedByEnemy(bool occupiedByEnemy)
    {
        this.occupiedByEnemy = occupiedByEnemy;
    }

    /// <summary>
    /// Returns true if an Enemy is on this Flooring.
    /// </summary>
    /// <returns>true if an Enemy is on this Flooring; otherwise,
    /// returns false.</returns>
    public bool OccupiedByEnemy() { return occupiedByEnemy; }

    /// <summary>
    /// Returns true if the ghost occupant on this Flooring is not null.
    /// </summary>
    /// <returns>true if the ghost occupant on this Flooring is not null;
    /// otherwise, false.</returns>
    public bool HasActiveGhostOccupant()
    {
        if (Occupied())
        {
            Tree treeOc = occupant as Tree;
            if (treeOc != null) return treeOc.HasActiveGhostOccupant();
        }
        return ghostOccupant != null;
    }

    /// <summary>
    /// Returns true if a pathfinder can walk across this Flooring.
    /// </summary>
    /// <returns>true if a pathfinder can walk across this Flooring;
    /// otherwise, false.</returns>
    public bool IsWalkable() { return !Occupied(); }

    /// <summary>
    /// Resets this Tile's stats to their starting values.
    /// </summary>
    public override void ResetStats() { return; }

    /// <summary>
    /// Sets the 2D Collider properties of this Tile.
    /// </summary>
    public override void SetColliderProperties() { return; }

    /// <summary>
    /// Returns the Sprite component that represents this Flooring in
    /// the Inventory.
    /// </summary>
    /// <returns>the Sprite component that represents this Flooring in
    /// the Inventory.</returns>
    public override Sprite GetInventorySprite()
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Returns a Sprite that represents this Flooring when it is
    /// being placed.
    /// </summary>
    /// <returns> a Sprite that represents this Flooring when it is
    /// being placed.</returns>
    public override Sprite GetPlacementSprite()
    {
        throw new System.NotImplementedException();
    }
}
