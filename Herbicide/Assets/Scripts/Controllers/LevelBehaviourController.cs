using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Controls any level-specific behaviours for the
/// current level.
/// </summary>
public abstract class LevelBehaviourController : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// Reference to the LevelBehaviourController singleton.
    /// </summary>
    private static LevelBehaviourController instance;

    /// <summary>
    /// List of events that must be triggered in order.
    /// </summary>
    protected Queue<LevelBehaviourEvent> sequentialEvents = new Queue<LevelBehaviourEvent>();
    
    /// <summary>
    /// List of events that can be triggered in any order.
    /// </summary>
    protected List<LevelBehaviourEvent> dynamicEvents = new List<LevelBehaviourEvent>();

    /// <summary>
    /// true if the tutorial is paused; otherwise, false.
    /// </summary>
    private static bool isPaused;

    #endregion

    #region Methods

    /// <summary>
    /// Finds and sets the LevelBehaviourController singleton.
    /// </summary>
    /// <param name="levelController">The LevelController singleton.</param>
    public static void SetSingleton(LevelController levelController)
    {
        if (levelController == null) return;

        LevelBehaviourController[] levelBehaviourControllers = FindObjectsOfType<LevelBehaviourController>();
        Assert.IsNotNull(levelBehaviourControllers, "Array of LevelBehaviourControllers is null.");

        // Not all levels have a LevelBehaviourController.
        if(levelBehaviourControllers.Length == 0) return;
        instance = levelBehaviourControllers[0];
        instance.InitializeLevelBehaviorEvents();
    }

    /// <summary>
    /// Main update loop for the LevelBehaviourController. Calls
    /// the instance's UpdateLevelBehaviour method.
    /// </summary>
    public static void UpdateLevelBehaviourController()
    {
        if(instance == null) return;

        instance.ProcessDynamicEvents();
        instance.ProcessSequentialEvents();

        instance.UpdateLevelBehaviourInstance();
    }

    /// <summary>
    /// Main update loop for the LevelBehaviourController. Calls
    /// event consequence methods
    /// </summary>
    protected abstract void UpdateLevelBehaviourInstance();

    /// <summary>
    /// Called when the game is quitting. Destroys the LevelBehaviourController.
    /// </summary>
    public static void OnQuit()
    {
        instance = null;
    }

    /// <summary>
    /// Runs through each dynamic LevelBehaviourEvent and checks
    /// whether the event's condition has been met. If the condition
    /// has been met, the event's action is executed and the event
    /// is removed from the list of dynamic events.
    /// </summary>
    private void ProcessDynamicEvents()
    {
        for (int i = dynamicEvents.Count - 1; i >= 0; i--)
        {
            LevelBehaviourEvent levelBehaviourEvent = dynamicEvents[i];
            if (levelBehaviourEvent.Condition())
            {
                levelBehaviourEvent.Action();
                dynamicEvents.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// Checks whether the next sequential event's condition has been
    /// met. If the condition has been met, the event's action is executed
    /// and the event is removed from the list of sequential events.
    /// </summary>
    private void ProcessSequentialEvents()
    {
        if(sequentialEvents.Count == 0) return;

        LevelBehaviourEvent currentEvent = sequentialEvents.Peek();
        if(currentEvent.Condition())
        {
            currentEvent.Action();
            sequentialEvents.Dequeue();
        }
    }

    /// <summary>
    /// Adds a sequential event to the list of sequential events.
    /// </summary>
    /// <param name="seqEvent">The sequential event to add.</param>
    protected void AddSequentialEvent(LevelBehaviourEvent seqEvent) => sequentialEvents.Enqueue(seqEvent);

    /// <summary>
    /// Adds a dynamic event to the list of dynamic events.
    /// </summary>
    /// <param name="dynamicEvent">The dynamic event to add.</param>
    protected void AddDynamicEvent(LevelBehaviourEvent dynamicEvent) => dynamicEvents.Add(dynamicEvent);

    /// <summary>
    /// Sets up the level-specific events for the current level.
    /// </summary>
    protected abstract void InitializeLevelBehaviorEvents();

    /// <summary>
    /// Sets whether the LevelBehaviourController is pausing
    /// the level.
    /// </summary>
    /// <param name="pause">true if the LevelBehaviourController is
    /// pausing the level; otherwise, false. </param>
    protected void SetPaused(bool pause) => isPaused = pause;

    /// <summary>
    /// Returns true if some level-specific event is currently pausing
    /// the level; otherwise, false.
    /// </summary>
    /// <returns>true if some level-specific event is currently pausing
    /// the level; otherwise, false.</returns>
    public static bool IsPaused() => isPaused;

    #endregion
}
