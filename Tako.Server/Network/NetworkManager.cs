using Tako.Common.Allocation;
using Tako.Common.Logging;
using Tako.Common.Network.Serialization;
using Tako.Definitions.Network;
using Tako.Definitions.Network.Connections;
using Tako.Definitions.Network.Packets;
using Tako.Server.Logging;
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

    /// <inheritdoc/>
    public IdAllocator<byte> IdAllocator { get; init; }

    /// <summary>
    /// The list of active connections.
    /// </summary>
    private readonly List<IConnection> _connections;

    /// <summary>
    /// The list of transport providers.
    /// </summary>
    private readonly List<ITransportProvider> _transportProviders;

    /// <summary>
    /// The logger.
    /// </summary>
    private readonly ILogger<NetworkManager> _logger = LoggerFactory<NetworkManager>.Get();

    /// <summary>
    /// The outgoing buffer.
    /// </summary>
    private readonly Memory<byte> _outgoingBuffer;

    /// <summary>
    /// The queue for connections that should be removed.
    /// </summary>
    private readonly Queue<IConnection> _removeQueue;

    /// <summary>
    /// Constructs a new network manager from the given address and port.
    /// </summary>
    /// <param name="addr">The address.</param>
    /// <param name="port">The port.</param>
    public NetworkManager()
    {
        const int outgoingBufferSizeInBytes = 1024;

        IdAllocator = new(byte.MaxValue);

        _outgoingBuffer = new Memory<byte>(new byte[outgoingBufferSizeInBytes]);
        PacketProcessor = new PacketProcessor();
        
        _connections = new();
        _transportProviders = new();

        _removeQueue = new();
    }

    /// <inheritdoc/>
    public void StartListening()
    {
        foreach (var provider in _transportProviders)
            provider.StartListening();
    }

    /// <inheritdoc/>
    public void StopListening()
    {
        foreach (var connection in _connections)
            connection.Disconnect();

        foreach (var provider in _transportProviders)
            provider.StopListening();

        _connections.Clear();
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

            IdAllocator.ReleaseId(conn!.ConnectionId);
        }
    }

    /// <inheritdoc/>
    public void AddConnection(IConnection connection)
    {
        _connections.Add(connection);
    }

    /// <inheritdoc/>
    public void AddTransportProvider(ITransportProvider provider)
    {
        _logger.Info($"Added transport provider of type {provider.GetType().Name}");
        _transportProviders.Add(provider);
    }
}
