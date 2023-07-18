using System.Net;
using System.Net.Sockets;
using Tako.Common.Logging;
using Tako.Common.Network.Serialization;
using Tako.Definitions.Network;
using Tako.Definitions.Network.Connections;
using Tako.Definitions.Network.Packets;
using Tako.Server.Logging;
using Tako.Server.Network.Connections;
using Tako.Server.Network.Packets;
using Tako.Server.Network.Packets.Client;

namespace Tako.Server.Network;

/// <summary>
/// The default network manager for Tako. Handles incoming tcp connections.
/// </summary>
public class NetworkManager : INetworkManager
{
	/// <inheritdoc/>
	public IReadOnlyList<IConnection> Connections => _connections;

	/// <inheritdoc/>
	public IPacketProcessor PacketProcessor { get; init; }

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
	/// The outgoing buffer.
	/// </summary>
	private Memory<byte> _outgoingBuffer;

	/// <summary>
	/// Constructs a new network manager from the given address and port.
	/// </summary>
	/// <param name="addr">The address.</param>
	/// <param name="port">The port.</param>
	public NetworkManager(IPAddress addr, int port)
	{
		const int outgoingBufferSizeInBytes = 1024;

		_outgoingBuffer = new Memory<byte>(new byte[outgoingBufferSizeInBytes]);
		PacketProcessor = new PacketProcessor();
		PacketProcessor.RegisterPacket<ClientIdentificationPacket>(OnClientIdentification, 0x00);

		_connections = new List<IConnection>();
		_listener = new TcpListener(addr, port);

		_listener.Start();
		_listener.BeginAcceptSocket(OnIncomingConnection, null);
	}

	/// <inheritdoc/>
	public void SendToAll(IServerPacket packet)
	{
		var writer = new NetworkWriter(_outgoingBuffer.Span);
		packet.Serialize(writer);
		foreach (var connection in Connections)
			connection.Send(ReadOnlySpan<byte>.Empty);
	}

	/// <inheritdoc/>
	public void Receive()
	{
		foreach (var connection in Connections)
		{
			while (connection.HasData())
				PacketProcessor.HandleIncomingPacket(connection.GetReader());
		}
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

	/// <summary>
	/// Handler for the client identification packet.
	/// </summary>
	/// <param name="packet">The packet.</param>
	private void OnClientIdentification(ClientIdentificationPacket packet)
	{
		_logger.Info($"User {packet.Username} with protocol version {packet.ProtocolVersion} wants to log in. [Key={packet.VerificationKey}]");
	}
}
