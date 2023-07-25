namespace Tako.Definitions.Plugins.Events;

/// <summary>
/// An event.
/// </summary>
public interface IEvent<TEventData>
{
    /// <summary>
    /// Subscribes to the envent with the given handler.
    /// </summary>
    /// <param name="handler">The handler.</param>
    void Subscribe(Func<TEventData, EventHandlingResult> handler);

    /// <summary>
    /// Unsubscribes a handler from the event.
    /// </summary>
    /// <param name="handler">The handler.</param>
    void Unsubscribe(Func<TEventData, EventHandlingResult> handler);

    /// <summary>
    /// Sends the data to this event and notifies all the subscribers.
    /// <br />
    /// If any of the subscribers responds with a <see cref="EventHandlingResult"/> of break, the event stops propagating.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <returns>Whether the event was accepted by all the subscribers.</returns>
    bool Send(TEventData data);
}
