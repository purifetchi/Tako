using Tako.Definitions.Plugins.Events;

namespace Tako.Server.Plugins.Events;

/// <summary>
/// An event.
/// </summary>
/// <typeparam name="TEventData">The event type.</typeparam>
public class Event<TEventData> : IEvent<TEventData>
{
    /// <summary>
    /// The handlers. Kept as a list of WeakRefs, so we don't keep plugins around if we unload them.
    /// </summary>
    private List<WeakReference<Func<TEventData, EventHandlingResult>>>? _handlers;

    /// <inheritdoc/>
    public void Subscribe(Func<TEventData, EventHandlingResult> handler)
    {
        _handlers ??= new();
        _handlers.Add(new(handler));
    }

    /// <inheritdoc/>
    public void Unsubscribe(Func<TEventData, EventHandlingResult> handler)
    {
        _handlers ??= new();

        var weakRef = _handlers.First(h => h.TryGetTarget(out var target) && handler == target);
        _handlers.Remove(weakRef);
    }

    /// <inheritdoc/>
    public bool Send(TEventData data)
    {
        if (_handlers is null)
            return true;

        foreach (var handler in _handlers)
        {
            if (!handler.TryGetTarget(out var handlerFunc))
                continue;

            if (handlerFunc(data) == EventHandlingResult.Break)
                return false;
        }

        return true;
    }
}
