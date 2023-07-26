using System.Net.Sockets;
using Tako.Common.Logging;
using Tako.Common.Network.Serialization;
using Tako.Definitions.Network.Connections;
using Tako.Definitions.Network.Packets;
using Tako.Server.Logging;

namespace Tako.Server.Network.Connections;

/// <summary>
/// A TCP socket connection.
/// </summary>
public class SocketConnection : IConnection
{
    /// <inheritdoc/>
    public byte ConnectionId { get; private set; }

    /// <inheritdoc/>
    public sbyte PlayerId { get; set; }

    /// <inheritdoc/>
    public bool Connected => _socket.Connected;

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
        const int KiB = 1024;
        const int bufferSizeInBytes = 128 * KiB;

        ConnectionId = connectionId;
        _socket = socket;
        _receiveBuffer = new Memory<byte>(new byte[bufferSizeInBytes]);
    }

    /// <inheritdoc/>
    public bool HasData()
    {
        return Connected && _socket.Available > 0;
    }

    /// <inheritdoc/>
    public NetworkReader GetReader()
    {
        var read = _socket.Receive(_receiveBuffer.Span);
        return new NetworkReader(_receiveBuffer[..read].Span);
    }

    /// <inheritdoc/>
    public void Disconnect()
    {
        if (!Connected)
            return;

        _socket.Close();
    }

    /// <inheritdoc/>
    public void Send(ReadOnlySpan<byte> data)
    {
        if (!Connected)
            return;

        try
        {
            _socket.Send(data);
        }
        catch (SocketException)
        {
            // NOTE(pref): We do not care about the socket exceptions here.
            //			   Wish I could just disable those...
        }
    }

    /// <inheritdoc/>
    public unsafe void Send(IServerPacket packet)
    {
        _logger.Debug($"Sending packet of type {packet.GetType().Name} to {_socket.RemoteEndPoint}");

        var buffer = stackalloc byte[2048];
        var span = new Span<byte>(buffer, 2048);
        var writer = new NetworkWriter(span);
        packet.Serialize(ref writer);
        Send(span[..writer.Written]);
    }
}
