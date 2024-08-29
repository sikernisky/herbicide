using UnityEngine;

/// <summary>
/// Represents a Quill projectile.
/// </summary>
public class Quill : Projectile
{
    #region Fields

    /// <summary>
    /// The Quill after it hits a target. Used to start
    /// the explosion. 
    /// </summary>
    [SerializeField]
    public GameObject piercingQuill;

    #endregion

    #region Stats

    /// <summary>
    /// ModelType of an Quill.
    /// </summary>
    public override ModelType TYPE => ModelType.QUILL;

    /// <summary>
    /// Starting speed of an Quill.
    /// </summary>
    public override float BASE_SPEED => 18f;

    /// <summary>
    /// Maximum speed of an Quill.
    /// </summary>
    public override float MAX_SPEED => float.MaxValue;

    /// <summary>
    /// Minimum speed of an Quill.
    /// </summary>
    public override float MIN_SPEED => 0f;

    /// <summary>
    /// Starting damage of an Quill.
    /// </summary>
    public override int BASE_DAMAGE => 3; //default: 3

    /// <summary>
    /// How much damage an Quill does to the targets behind the first target.
    /// </summary>
    public int PIERCING_DAMAGE => 3;

    /// <summary>
    /// Maximum damage of an Quill.
    /// </summary>
    public override int MAX_DAMAGE => int.MaxValue;

    /// <summary>
    /// Minimum damage of an Quill.
    /// </summary>
    public override int MIN_DAMAGE => 0;

    /// <summary>
    /// Lifespan of an Quill.
    /// </summary>
    public override float LIFESPAN => float.MaxValue;

    /// <summary>
    /// How many seconds an Quill's move animation lasts,
    /// from start to finish. 
    /// </summary>
    public override float MOVE_ANIMATION_DURATION => 0f;

    #endregion

    #region Methods

    /// <summary>
    /// Returns the GameObject that represents this Quill on the grid.
    /// </summary>
    /// <returns>the GameObject that represents this Quill on the grid.
    /// </returns>
    public override GameObject Copy() => ProjectileFactory.GetProjectilePrefab(TYPE);

    /// <summary>
    /// Returns a Sprite that represents this Quill when it is
    /// being placed.
    /// </summary>
    /// <returns> a Sprite that represents this Quill when it is
    /// being placed.</returns>
    public override Sprite[] GetPlacementTrack() => ProjectileFactory.GetPlacementTrack(TYPE);

    /// <summary>
    /// Returns the GameObject that represents this Quill when it is
    /// piercing a target.
    /// </summary>
    /// <returns>the GameObject that represents this Quill when it is
    /// piercing a target.</returns>
    public GameObject GetPiercingQuill() 
    {
        GameObject piercingQuillCopy= Instantiate(piercingQuill);
        piercingQuillCopy.SetActive(true);
        return piercingQuillCopy;     
    }

    #endregion
}
