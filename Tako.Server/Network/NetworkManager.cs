using System.Net;
using System.Net.Sockets;
using Tako.Common.Allocation;
using Tako.Common.Logging;
using Tako.Common.Network.Serialization;
using Tako.Definitions.Network;
using Tako.Definitions.Network.Connections;
using Tako.Definitions.Network.Packets;
using Tako.Server.Logging;
using Tako.Server.Network.Connections;
using Tako.Server.Network.Packets;

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
    /// The allocator for connection ids.
    /// </summary>
    private readonly IdAllocator<byte> _idAllocator;

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
    /// The queue for connections that should be removed.
    /// </summary>
    private Queue<IConnection> _removeQueue;

    /// <summary>
    /// Constructs a new network manager from the given address and port.
    /// </summary>
    /// <param name="addr">The address.</param>
    /// <param name="port">The port.</param>
    public NetworkManager(IPAddress addr, int port)
    {
        const int outgoingBufferSizeInBytes = 1024;

        _idAllocator = new(byte.MaxValue);

        _outgoingBuffer = new Memory<byte>(new byte[outgoingBufferSizeInBytes]);
        PacketProcessor = new PacketProcessor();

        _connections = new List<IConnection>();
        _listener = new TcpListener(addr, port);

        _listener.Start();
        _listener.BeginAcceptSocket(OnIncomingConnection, null);

        _removeQueue = new Queue<IConnection>();
    }

    /// <inheritdoc/>
    public void SendToAll(IServerPacket packet)
    {
        var writer = new NetworkWriter(_outgoingBuffer.Span);
        packet.Serialize(ref writer);
        foreach (var connection in Connections)
            connection.Send(_outgoingBuffer.Span[..writer.Written]);
    }

    /// <inheritdoc/>
    public void SendToAllThatMatch(IServerPacket packet, Func<IConnection, bool> query)
    {
        var writer = new NetworkWriter(_outgoingBuffer.Span);
        packet.Serialize(ref writer);

        foreach (var connection in Connections)
        {
            if (query(connection))
                connection.Send(_outgoingBuffer.Span[..writer.Written]);
        }
    }

    /// <inheritdoc/>
    public void Receive()
    {
        foreach (var connection in Connections)
        {
            if (!connection.Connected)
            {
                _removeQueue.Enqueue(connection);
                break;
            }

            while (connection.HasData())
            {
                var reader = connection.GetReader();
                while (reader.HasDataLeft && connection.Connected)
                    PacketProcessor.HandleIncomingPacket(ref reader, connection);
            }
        }

        // Remove all the gone connections.
        while (_removeQueue.Count > 0)
        {
            var conn = _removeQueue.Dequeue();
            conn!.Disconnect();
            _connections.Remove(conn!);

            _idAllocator.ReleaseId(conn!.ConnectionId);
        }
    }

    /// <summary>
    /// Invoked when we have an incoming connection from the TcpListener.
    /// </summary>
    private void OnIncomingConnection(IAsyncResult? ar)
    {
        var clientSocket = _listener.EndAcceptSocket(ar!);
        _logger.Info($"Accepting incoming connection from {clientSocket.RemoteEndPoint}.");

        _connections.Add(new SocketConnection(clientSocket, _idAllocator.GetId()));

        // Renew the socket accepting.
        _listener.BeginAcceptSocket(OnIncomingConnection, null);
    }
}
