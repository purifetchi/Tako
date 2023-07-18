using System.Net.Sockets;
using Tako.Common.Logging;
using Tako.Common.Network.Serialization;
using Tako.Definitions.Network.Connections;
using Tako.Server.Logging;
using Tako.Server.Network.Packets.Client;

namespace Tako.Server.Network.Connections;

/// <summary>
/// A TCP socket connection.
/// </summary>
public class SocketConnection : IConnection
{
	/// <inheritdoc/>
	public byte ConnectionId { get; private set; }

	/// <summary>
	/// The socket for this connection.
	/// </summary>
	private readonly Socket _socket;

	/// <summary>
	/// The receive buffer for this connection.
	/// </summary>
	private readonly Memory<byte> _receiveBuffer;

	/// <summary>
	/// The logger.
	/// </summary>
	private readonly ILogger<SocketConnection> _logger = LoggerFactory<SocketConnection>.Get();

	/// <summary>
	/// Constructs a new socket connection given a socket.
	/// </summary>
	/// <param name="socket">The socket.</param>
	/// <param name="connectionId">The connection id to assign to this connection.</param>
	public SocketConnection(Socket socket, byte connectionId)
	{
		const int bufferSizeInBytes = 1024;

		ConnectionId = connectionId;
		_socket = socket;
		_receiveBuffer = new Memory<byte>(new byte[bufferSizeInBytes]);
	}

	/// <inheritdoc/>
	public void Process()
	{
		var read = _socket.Receive(_receiveBuffer.Span);
		if (read < 1)
			return;

		var slice = _receiveBuffer[..read].Span;
		var reader = new NetworkReader(slice);
		var type = reader.Read<byte>();
		_logger.Info($"Got a packet of type {type} from {_socket.RemoteEndPoint}.");

		switch (type)
		{
			case 0x00:
				var id = new ClientIdentificationPacket();
				id.Deserialize(reader);

				_logger.Info($"User {id.Username} with protocol version {id.ProtocolVersion} wants to log in. [Key={id.VerificationKey}]");
				break;
		}
	}

	/// <inheritdoc/>
	public void Disconnect()
	{
		_socket.Close();
	}

	/// <inheritdoc/>
	public void Send(ReadOnlySpan<byte> data)
	{
		_socket.Send(data);
	}
}
