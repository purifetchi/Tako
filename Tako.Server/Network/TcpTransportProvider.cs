using System.Net;
using System.Net.Sockets;
using Tako.Common.Logging;
using Tako.Definitions.Network;
using Tako.Definitions.Settings;
using Tako.Server.Network.Connections;

namespace Tako.Server.Network;

/// <summary>
/// A TCP transport provider.
/// </summary>
public class TcpTransportProvider : ITransportProvider
{
    /// <summary>
    /// The network manager.
    /// </summary>
    public INetworkManager NetworkManager { get; init; }

    /// <summary>
    /// The TCP listener.
    /// </summary>
    private readonly TcpListener _listener;

    /// <summary>
    /// The logger.
    /// </summary>
    private readonly ILogger<TcpTransportProvider> _logger = LoggerFactory<TcpTransportProvider>.Get();

    /// <summary>
    /// Creates a new TCP transport provider.
    /// </summary>
    /// <param name="settings">The settings.</param>
    /// <param name="networkManager">The network manager.</param>
    public TcpTransportProvider(
        ISettings settings,
        INetworkManager networkManager)
    {
        const string defaultIp = "127.0.0.1";
        const string defaultPort = "25565";

        _listener = new TcpListener(
            IPAddress.Parse(settings.Get("ip") ?? defaultIp),
            int.Parse(settings.Get("port") ?? defaultPort));

        NetworkManager = networkManager;
    }

    /// <inheritdoc/>
    public void StartListening()
    {
        _listener.Start();
        _listener.BeginAcceptSocket(OnIncomingConnection, null);
    }

    /// <inheritdoc/>
    public void StopListening()
    { 
        _listener.Stop(); 
    }

    /// <summary>
    /// Invoked when we have an incoming connection from the TcpListener.
    /// </summary>
    private void OnIncomingConnection(IAsyncResult? ar)
    {
        var clientSocket = _listener.EndAcceptSocket(ar!);
        _logger.Info($"Accepting incoming connection from {clientSocket.RemoteEndPoint}.");

        NetworkManager.AddConnection(
            new SocketConnection(
                clientSocket, 
                NetworkManager.IdAllocator.GetId()
                ));

        _listener.BeginAcceptSocket(OnIncomingConnection, null);
    }
}
