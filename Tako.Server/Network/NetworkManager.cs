using System.Net;
using System.Net.Sockets;
using Tako.Common.Logging;
using Tako.Definitions.Network;
using Tako.Definitions.Network.Connections;
using Tako.Definitions.Network.Packets;
using Tako.Server.Logging;
using Tako.Server.Network.Connections;

namespace Tako.Server.Network;

/// <summary>
/// The default network manager for Tako. Handles incoming tcp connections.
/// </summary>
public class NetworkManager : INetworkManager
{
	/// <inheritdoc/>
	public IReadOnlyList<IConnection> Connections => _connections;

	/// <summary>
	/// The TCP listener.
	/// </summary>
	private readonly TcpListener _listener;

	/// <summary>
	/// The list of active connections.
	/// </summary>
	private readonly List<IConnection> _connections;

	/// <summary>
	/// The logger.
	/// </summary>
	private readonly ILogger<NetworkManager> _logger = LoggerFactory<NetworkManager>.Get();

	/// <summary>
	/// Constructs a new network manager from the given address and port.
	/// </summary>
	/// <param name="addr">The address.</param>
	/// <param name="port">The port.</param>
	public NetworkManager(IPAddress addr, int port)
	{
		_connections = new List<IConnection>();
		_listener = new TcpListener(addr, port);

		_listener.Start();
		_listener.BeginAcceptSocket(OnIncomingConnection, null);
	}

	/// <inheritdoc/>
	public void SendToAll(IPacket packet)
	{
		packet.Serialize(new Common.Network.Serialization.NetworkWriter());
		foreach (var connection in Connections)
			connection.Send(ReadOnlySpan<byte>.Empty);
	}

	/// <inheritdoc/>
	public void Receive()
	{
		foreach (var connection in Connections)
			connection.Process();
	}

	/// <summary>
	/// Invoked when we have an incoming connection from the TcpListener.
	/// </summary>
	private void OnIncomingConnection(IAsyncResult? ar)
	{
		var clientSocket = _listener.EndAcceptSocket(ar!);
		_logger.Info($"Accepting incoming connection from {clientSocket.RemoteEndPoint}.");

		_connections.Add(new SocketConnection(clientSocket, 0));

		// Renew the socket accepting.
		_listener.BeginAcceptSocket(OnIncomingConnection, null);
	}
}
