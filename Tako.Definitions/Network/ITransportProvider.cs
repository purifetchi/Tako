namespace Tako.Definitions.Network;

/// <summary>
/// An interface that manages the creation of sockets and listening for incoming connections.
/// </summary>
public interface ITransportProvider
{
    /// <summary>
    /// The network manager.
    /// </summary>
    INetworkManager NetworkManager { get; }

    /// <summary>
    /// Starts listening for incoming connections.
    /// </summary>
    void StartListening();

    /// <summary>
    /// Stops listening for incoming connections.
    /// </summary>
    void StopListening();
}
