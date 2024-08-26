/// <summary>
/// Contract for a controller that implements states.
/// </summary>
public interface IStateTracker<T> where T : System.Enum
{
    /// <summary>
    /// Sets the State of this IStateTracker. This helps keep track of
    /// what its Model should do and what it is doing, and it is essential
    /// for the FSM logic. 
    /// </summary>
    /// <param name="state"></param>
    public void SetState(T state);

    /// <summary>
    /// Returns the State of this IStateTracker. This helps keep track of
    /// what its Model should do and what it is doing, and it is essential
    /// for the FSM logic. 
    /// </summary>
    /// <returns>The State of this IStateTracker. </returns>
    public T GetState();

    /// <summary>
    /// Processes this IStateTracker's state FSM to determine the
    /// correct state. Takes the current state and chooses whether
    /// or not to switch to another based on game conditions. /// 
    /// </summary>
    public void UpdateStateFSM();

    /// <summary>
    /// Returns true if two states are equal.
    /// </summary>
    /// <param name="stateA">The first state.</param>
    /// <param name="stateB">The second state</param>
    /// <returns>true if two states are equal; otherwise, false. </returns>
    public bool StateEquals(T stateA, T stateB);
}
