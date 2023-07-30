using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A serializable class to store Sound data.
/// </summary>
[System.Serializable]
public class Sound
{
    /// <summary>
    /// true if this Sound is a music clip; otherwise, it's a 
    /// sound effect.
    /// </summary>
    [SerializeField]
    private bool isMusic;

    /// <summary>
    /// The file to play for this Sound.
    /// </summary>
    [SerializeField]
    private AudioClip clip;

    /// <summary>
    /// The name / identifier of this sound. 
    /// </summary>
    [SerializeField]
    private string soundName;

    /// <summary>
    /// Returns the AudioClip associated with this Sound.
    /// </summary>
    /// <returns>this Sound's AudioClip.</returns>
    public AudioClip GetClip()
    {
        return clip;
    }

    /// <summary>
    /// Returns this Sound's name.
    /// </summary>
    /// <returns>the name of this Sound.</returns>
    public string GetName()
    {
        return soundName;
    }

    /// <summary>
    /// Returns true if this Sound is a music clip or false
    /// if it is a sound effect clip.
    /// </summary>
    /// <returns>true if this Sound is music; false if it is a
    /// sound effect. </returns>
    public bool IsMusic()
    {
        return isMusic;
    }
}
