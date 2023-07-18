using System.Net.Sockets;
using Tako.Definitions.Network.Connections;

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
	private Memory<byte> _receiveBuffer;

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

		var slice = _receiveBuffer[..read];
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
