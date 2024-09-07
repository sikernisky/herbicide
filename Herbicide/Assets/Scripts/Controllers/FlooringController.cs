using UnityEngine;

/// <summary>
/// Controls ALL Flooring objects in the scene.
/// </summary>
public class FlooringController : ModelController
{
    #region Fields

    #endregion

    #region Methods

    /// <summary>
    /// Gives a Flooring Model a controller.
    /// </summary>
    /// <param name="flooring">The Flooring model that needs a controller.</param>
    public FlooringController(Flooring flooring) : base(flooring) { }

    /// <summary>
    /// Returns true if the FlooringModel should be destroyed and removed.
    /// </summary>
    /// <returns>true if the FlooringModel should be destroyed and removed;
    /// otherwise, false. </returns>
    public override bool ValidModel() => true;

    /// <summary>
    /// Adds one chunk of Time.deltaTime to the animation
    /// counter that tracks the current state.
    /// </summary>
    public override void AgeAnimationCounter() => throw new System.NotImplementedException();

    /// <summary>
    /// Returns the animation counter for the current state.
    /// </summary>
    /// <returns>the animation counter for the current state.</returns>
    public override float GetAnimationCounter() => throw new System.NotImplementedException();
    /// <summary>
    /// Sets the animation counter for the current state to 0.
    /// </summary>
    public override void ResetAnimationCounter() => throw new System.NotImplementedException();

    /// <summary>
    /// Returns the Flooring Model.
    /// </summary>
    /// <returns>the Flooring Model.</returns>
    protected Flooring GetFlooring() => GetModel() as Flooring;

    /// <summary>
    /// Returns the Flooring prefab to the FlooringFactory singleton.
    /// </summary>
    public override void ReturnModelToFactory() => FlooringFactory.ReturnFlooringPrefab(GetFlooring().gameObject);

    #endregion
}
