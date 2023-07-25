using Tako.Definitions.Plugins.Events;

namespace Tako.Server.Plugins.Events;

/// <summary>
/// An event.
/// </summary>
/// <typeparam name="TEventData">The event type.</typeparam>
public class Event<TEventData> : IEvent<TEventData>
{
    /// <summary>
    /// The handlers.
    /// </summary>
    private List<Func<TEventData, EventHandlingResult>>? _handlers;

    /// <inheritdoc/>
    public void Subscribe(Func<TEventData, EventHandlingResult> handler)
    {
        _handlers ??= new();
        _handlers.Add(handler);
    }

    /// <inheritdoc/>
    public void Unsubscribe(Func<TEventData, EventHandlingResult> handler)
    {
        _handlers ??= new();
        _handlers.Remove(handler);
    }

    /// <inheritdoc/>
    public bool Send(TEventData data)
    {
        if (_handlers is null)
            return true;

        foreach (var handler in _handlers)
        {
            if (handler(data) == EventHandlingResult.Break)
                return false;
        }

        return true;
    }
}
