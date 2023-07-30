using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents something that the player can click on
/// to aquire/collect. 
/// </summary>
public abstract class Collectable : MonoBehaviour
{
    /// <summary>
    /// Name of this Collectable.
    /// </summary>
    public abstract string NAME { get; }

    /// <summary>
    /// Does something when the player collects this Collectable.
    /// </summary>
    public abstract void OnCollect();
}
