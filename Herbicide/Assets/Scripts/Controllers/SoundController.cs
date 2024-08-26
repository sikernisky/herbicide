using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Controls the sound in the game.
/// </summary>
public class SoundController : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// Reference to the SoundController singleton.
    /// </summary>
    private static SoundController instance;

    /// <summary>
    /// AudioSource component for music.
    /// </summary>
    [SerializeField]
    private AudioSource musicSource;

    /// <summary>
    /// AudioSource component for sound effects.
    /// </summary>
    [SerializeField]
    private AudioSource effectSource;

    /// <summary>
    /// All sound effects this SoundController can play.
    /// </summary>
    [SerializeField]
    private List<Sound> effectSounds;

    /// <summary>
    /// All music this SoundController can play.
    /// </summary>
    [SerializeField]
    private List<Sound> musicSounds;

    #endregion

    #region Methods

    /// <summary>
    /// Finds and sets the SoundController singleton for a level.
    /// </summary>
    /// <param name="levelController">The LevelController singleton.</param>
    public static void SetSingleton(LevelController levelController)
    {
        if (levelController == null) return;
        if (instance != null) return;

        SoundController[] soundControllers = FindObjectsOfType<SoundController>();
        Assert.IsNotNull(soundControllers, "Array of EconomyControllers is null.");
        Assert.AreEqual(1, soundControllers.Length);
        instance = soundControllers[0];
    }

    /// <summary>
    /// Finds and sets the SoundController singleton for the MainMenu.
    /// </summary>
    /// <param name="mainMenuController">The LevelController singleton.</param>
    public static void SetSingleton(MainMenuController mainMenuController)
    {
        if (mainMenuController == null) return;
        if (instance != null) return;

        SoundController[] soundControllers = FindObjectsOfType<SoundController>();
        Assert.IsNotNull(soundControllers, "Array of EconomyControllers is null.");
        Assert.AreEqual(1, soundControllers.Length);
        instance = soundControllers[0];
    }

    /// <summary>
    /// Finds and sets the SoundController singleton for the SkillMenu.
    /// </summary>
    /// <param name="skillMenuController">The SkillMenuController singleton.</param>
    public static void SetSingleton(CollectionMenuController skillMenuController)
    {
        if (skillMenuController == null) return;
        if (instance != null) return;

        SoundController[] soundControllers = FindObjectsOfType<SoundController>();
        Assert.IsNotNull(soundControllers, "Array of EconomyControllers is null.");
        Assert.AreEqual(1, soundControllers.Length);
        instance = soundControllers[0];
    }

    /// <summary>
    /// Plays a SoundEffect. If the SoundController fails to
    /// recognize the effect's name, it does nothing.
    /// </summary>
    /// <param name="soundName">The name of the sound effect to
    /// play.</param>
    public static void PlaySoundEffect(string soundName)
    {
        AudioClip effectToPlay = null;
        foreach (Sound s in instance.effectSounds)
        {
            if (s.GetName() == soundName && !s.IsMusic())
            {
                effectToPlay = s.GetClip();
            }
        }
        if (effectToPlay == null) return;
        instance.effectSource.PlayOneShot(effectToPlay, 1.0f);
    }

    /// <summary>
    /// Sets the music volume.
    /// </summary>
    /// <param name="volume">The volume to set.</param> 
    public static void SetMusicVolume(float volume)
    {
        Assert.IsTrue(volume >= 0.0f && volume <= 1.0f, "Volume must be between 0 and 1.");

        instance.musicSource.volume = volume;
    }

    /// <summary>
    /// Sets the soundfx volume.
    /// </summary>
    /// <param name="volume">The volume to set.</param> 
    public static void SetSoundFXVolume(float volume)
    {
        Assert.IsTrue(volume >= 0.0f && volume <= 1.0f, "Volume must be between 0 and 1.");

        instance.effectSource.volume = volume;
    }

    #endregion
}
