using System;

/// <summary>
/// Represents a specific condition or trigger within the game that,
/// when met, causes a predefined action or sequence of actions to be executed.
/// </summary>
public class LevelBehaviourEvent
{
    /// <summary>
    /// The condition that must be met for the event to trigger.
    /// </summary>
    public Func<bool> Condition { get; set; }

    /// <summary>
    /// The action to be executed when the event triggers.
    /// </summary>
    public Action Action { get; set; }

    /// <summary>
    /// Creates a new LevelBehaviourEvent with the specified condition, action,
    /// and sequence status.
    /// </summary>
    /// <param name="condition">The condition that must be met for the event to trigger.</param>
    /// <param name="action">The action to be executed when the event triggers.</param>
    public LevelBehaviourEvent(Func<bool> condition, Action action)
    {
        Condition = condition;
        Action = action;
    }
}
