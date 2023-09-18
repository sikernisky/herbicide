using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Represents a Tree. A Tree is a surface, but it does not
/// implement the ISurface interface because of its restricted
/// behavior. 
/// </summary>
public abstract class Tree : PlaceableObject, ITargetable, ISurface
{
    /// <summary>
    /// Type of this Tree.
    /// </summary>
    public abstract TreeType TYPE { get; }

    /// <summary>
    /// How long it takes for a Tree to flash its damage animation.
    /// </summary>
    public virtual float DAMAGE_FLASH_TIME => 0.5f;

    /// <summary>
    /// Base health of a Tree.
    /// </summary>
    public virtual int BASE_HEALTH => 200;

    /// <summary>
    /// Upper bound of a Tree's health.
    /// </summary>
    public int MAX_HEALTH => 200;

    /// <summary>
    /// Lower bound of a Tree's health.
    /// </summary>
    public int MIN_HEALTH => 0;

    /// <summary>
    /// How far to push a hosted Defender horizontally
    /// </summary>
    protected virtual float DEFENDER_OFFSET_X => 0f;

    /// <summary>
    /// How far to push a hosted Defender vertically
    /// </summary>
    protected virtual float DEFENDER_OFFSET_Y => .75f;

    /// <summary>
    /// This Tree's neighboring ISurfaces.
    /// </summary>
    private ISurface[] surfaceNeighbors;

    /// <summary>
    /// Health of a Tree
    /// </summary>
    private int health;

    /// <summary>
    /// The Defender on this Tree; null if no Defender is.
    /// </summary>
    private Defender defender;

    /// <summary>
    /// The Defender ghost placing on this Tree.
    /// </summary>
    private GameObject ghostDefender;

    /// <summary>
    /// This Tree's Collider component.
    /// </summary>
    [SerializeField]
    private Collider2D treeCollider;

    /// <summary>
    /// How long this Tree has been flashing damage.
    /// </summary>
    private float damageFlashingTime;


    /// <summary>
    /// Represents a type of Tree.
    /// </summary>
    public enum TreeType
    {
        BASIC
    }

    /// <summary>
    /// Returns a Sprite component that represents this Tree in an 
    /// Inventoryslot.
    /// </summary>
    /// <returns>a Sprite component that represents this Tree in an 
    /// Inventoryslot.</returns>
    public override Sprite GetInventorySprite()
    {
        return TreeFactory.GetTreeInventorySprite(TYPE);
    }

    /// <summary>
    /// Returns a Sprite component that represents this Tree in an 
    /// Inventoryslot.
    /// </summary>
    /// <returns>a Sprite component that represents this Tree in an 
    /// Inventoryslot.</returns>
    public override Sprite GetPlacementSprite()
    {
        return TreeFactory.GetTreePlacedSprite(TYPE);
    }

    /// <summary>
    /// Returns a Tree GameObject that can be placed on the grid.
    /// </summary>
    /// <returns>a Tree GameObject that can be placed on the grid.</returns>
    public override GameObject MakePlaceableObject()
    {
        return Instantiate(TreeFactory.GetTreePrefab(TYPE));
    }

    /// <summary>
    /// Returns a GameObject that holds a SpriteRenderer component with
    /// this Tree's placed Sprite. No other components are
    /// copied. 
    /// </summary>
    /// <returns>A GameObject with a SpriteRenderer component. </returns>
    public override GameObject MakeHollowObject()
    {
        GameObject hollowCopy = new GameObject("Hollow " + name);
        SpriteRenderer hollowRenderer = hollowCopy.AddComponent<SpriteRenderer>();
        hollowRenderer.sprite = TreeFactory.GetTreePlacedSprite(TYPE);

        hollowCopy.transform.position = transform.position;
        hollowCopy.transform.localScale = transform.localScale;

        return hollowCopy;
    }

    /// <summary>
    /// Initializes this Tree when placed on the TileGrid. Sets its Sprite to
    /// its placed Sprite.
    /// </summary>
    /// <param name="neighbors">The PlaceableObjects surrounding this Tree.</param>
    public override void Define(Vector2Int coordinates, PlaceableObject[] neighbors)
    {
        base.Define(coordinates, neighbors);
        SetSortingOrder(10000 - (int)transform.position.y * 100);
        SetSprite(TreeFactory.GetTreePlacedSprite(TYPE));
        ResetStats();
    }


    /// <summary>
    /// Spawns this Tree, setting its values.
    /// </summary>
    public void ResetStats()
    {
        ResetHealth();
    }

    /// <summary>
    /// Subtracts from this Tree's health. If something sits on this Tree, subtracts
    /// from the sitter's health instead.
    /// </summary>
    /// <param name="amount">The amount of health to subtract.</param>
    public void TakeDamage(int amount)
    {
        if (Occupied())
        {
            defender.TakeDamage(amount);
        }
        else if (amount >= 0)
        {
            FlashDamage();
            health = Mathf.Clamp(health - amount, MIN_HEALTH, MAX_HEALTH);
        }
    }

    /// <summary>
    /// Adds to this Tree's health amount.
    /// </summary>
    /// <param name="amount">The amount of health to add.</param>
    public void Heal(int amount)
    {
        if (amount >= 0)
            health = Mathf.Clamp(health + amount, MIN_HEALTH, MAX_HEALTH);
    }

    /// <summary>
    /// Returns this Tree's current health.
    /// </summary>
    /// <returns>this Tree's current health.</returns>
    public int GetHealth()
    {
        return health;
    }

    /// <summary>
    /// Returns the world position of this Tree.
    /// </summary>
    public Vector3 GetPosition()
    {
        return transform.position;
    }

    /// <summary>
    /// Sets this Tree's health to its starting value.
    /// </summary>
    public void ResetHealth()
    {
        health = BASE_HEALTH;
    }

    /// <summary>
    /// Called when this Tree dies.
    /// </summary>
    public virtual void Die()
    {
        return;
    }

    /// <summary>
    /// Returns true if there is an occupant on this Tree.
    /// </summary>
    /// <returns>true if there is an occupant on this Tree; otherwise,
    /// false.</returns>
    public bool Occupied()
    {
        return defender != null;
    }

    /// <summary>
    /// Flashes this Tree to signal it has taken damage.
    /// </summary>
    public void FlashDamage()
    {
        SetDamageFlashingTime(DAMAGE_FLASH_TIME);
    }

    /// <summary>
    /// Returns this Tree's Collider component.
    /// </summary>
    /// <returns>this Tree's Collider component.</returns>
    public Collider2D GetColllider()
    {
        return treeCollider;
    }

    /// <summary>
    /// Returns the position at which an IAttackable will aim at when
    /// attacking this Tree.
    /// </summary>
    /// <returns>this Tree's attack position.</returns>
    public Vector3 GetAttackPosition()
    {
        return transform.position;
    }

    /// <summary>
    /// Returns true if this Tree is dead. This Tree is dead if
    /// its health is below or at zero.
    /// </summary>
    /// <returns>true if this Tree is dead; otherwise, false.</returns>
    public bool Dead()
    {
        return GetHealth() == 0;
    }

    /// <summary>
    /// Returns true if a Defender can place on this Tree. If
    /// so, places it.
    /// </summary>
    /// <param name="candidate">The candidate Defender.</param>
    /// <param name="neighbors">This Tree's neighboring ISurfaces.</param>
    /// <returns>true if a Defender was placed on this Tree; otherwise,
    /// false. </returns>
    public bool Place(PlaceableObject candidate, ISurface[] neighbors)
    {
        if (!CanPlace(candidate, neighbors)) return false;

        GameObject prefabClone = (candidate as Defender).MakePlaceableObject();
        Assert.IsNotNull(prefabClone);
        Defender defenderComponent = prefabClone.GetComponent<Defender>();
        Assert.IsNotNull(defenderComponent);
        SpriteRenderer prefabRenderer = prefabClone.GetComponent<SpriteRenderer>();

        defender = defenderComponent;

        string placeName = defender.GetName().ToLower() + "Place";
        SoundController.PlaySoundEffect(placeName);
        defenderComponent.SetSortingOrder(GetSortingOrder() + 1);
        prefabClone.transform.SetParent(transform);
        prefabClone.transform.localPosition =
            new Vector3(DEFENDER_OFFSET_X, DEFENDER_OFFSET_Y, 1);
        prefabClone.transform.localScale = Vector3.one;
        ControllerController.MakeDefenderController(defenderComponent);

        return true;
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
        if (candidate == null) return false;
        if (Occupied()) return false;
        if (candidate as Defender == null) return false;
        return true;
    }

    /// <summary>
    /// Removes a Defender off this Tree if there is one.
    /// </summary>
    /// <param name="neighbors">This Tree's neighboring ISurfaces.</param>
    /// <returns>true if the remove was successful; otherwise, false.
    /// </returns>
    public bool Remove(ISurface[] neighbors)
    {
        if (!Occupied()) return false;

        defender = null;
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
    public PlaceableObject GetPlaceableObject()
    {
        return defender;
    }

    /// <summary>
    /// Asserts that this Tree is defined on an ISurface.
    /// </summary>
    public void AssertDefined()
    {
        Assert.IsTrue(Defined());
    }

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

        hollowRenderer.sortingLayerName = "Trees";
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
    public void SetOccupiedByEnemy(bool occupiedByEnemy)
    {
        return;
    }

    /// <summary>
    /// Returns true if an Enemy is on this Tree.
    /// </summary>
    /// <returns>true if an Enemy is on this Tree; otherwise,
    /// false.</returns>
    public bool OccupiedByEnemy()
    {
        return false;
    }

    /// <summary>
    /// Returns true if the ghost occupant on this Tree is not null.
    /// </summary>
    /// <returns>true if the ghost occupant on this Tree is not null;
    /// otherwise, false.</returns>
    public bool HasActiveGhostOccupant()
    {
        return ghostDefender != null;
    }

    /// <summary>
    /// Returns true if a pathfinder can walk across this Tree.
    /// </summary>
    /// <returns>true if a pathdfinder can walk across this Tree;
    /// otherwise, false.</returns>
    public bool IsWalkable()
    {
        return false;
    }

    /// <summary>
    /// Sets the number of seconds that this Tree has been
    /// flashing its damage effect.
    /// </summary>
    /// <param name="time">The number of seconds this Tree
    /// has been playing its damage effect </param>
    public void SetDamageFlashingTime(float time)
    {
        if (time < 0 || time > DAMAGE_FLASH_TIME) return;
        damageFlashingTime = time;
    }

    /// <summary>
    /// Returns the number of seconds that this Tree has
    /// been playing its damage flash effect.
    /// </summary>
    /// <returns>the number of seconds that this Tree has
    /// been playing its damage flash effect.</returns>
    public float GetDamageFlashingTime()
    {
        return damageFlashingTime;
    }

}
