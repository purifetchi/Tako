using System.Diagnostics.CodeAnalysis;
using Tako.Common.Logging;
using Tako.Common.Network.Serialization;
using Tako.Definitions.Network.Connections;
using Tako.Definitions.Network.Packets;
using Tako.Server.Game.Players;

namespace Tako.Server.Network.Packets;

/// <summary>
/// The class responsible for processing incoming packets.
/// </summary>
public class PacketProcessor : IPacketProcessor
{
    /// <summary>
    /// Contains data responsible for handling incoming packets.
    /// </summary>
    /// <typeparam name="TPacket">The type of the incoming packet.</typeparam>
    private class PacketHandlingData
    {
        /// <summary>
        /// The factory method, creating the packet.
        /// </summary>
        public Func<IClientPacket> Factory { get; init; }

        /// <summary>
        /// The handler for the packet.
        /// </summary>
        public Action<IConnection, IClientPacket> Handler { get; init; }

        /// <summary>
        /// Creates a new packet handling data.
        /// </summary>
        /// <param name="factory">The factory method.</param>
        /// <param name="handler">The handler method.</param>
        public PacketHandlingData(Func<IClientPacket> factory, Action<IConnection, IClientPacket> handler)
        {
            Factory = factory;
            Handler = handler;
        }
    }

    /// <summary>
    /// The handlers array.
    /// </summary>
    private readonly Dictionary<byte, PacketHandlingData> _handlers = new Dictionary<byte, PacketHandlingData>();

    /// <summary>
    /// The logger.
    /// </summary>
    private readonly ILogger<PacketProcessor> _logger = LoggerFactory<PacketProcessor>.Get();

    /// <inheritdoc/>
    public void HandleIncomingPacket(ref NetworkReader reader, IConnection conn)
    {
        var id = reader.Read<byte>();

        if (!_handlers.TryGetValue(id, out var handler))
        {
            _logger.Warn($"Unknown packet of id 0x{id:X2}.");
            return;
        }

        var packet = handler.Factory();

        try
        {
            packet.Deserialize(ref reader);
        }
        catch
        {
            _logger.Warn($"Packet corruption detected while trying to deserialize packet of id 0x{id:X2}. Disconnecting client {conn?.ConnectionId}.");

            conn?.Send(new Server.DisconnectPlayerPacket
            {
                DisconnectReason = DisconnectMessages.CORRUPTED_PACKET_SENT
            });
            conn?.Disconnect();
            return;
        }

        //_logger.Debug($"Handling packet of id 0x{id:X2} and type {packet.GetType().Name}.");
        handler.Handler(conn, packet);
    }

    /// <inheritdoc/>
    public void RegisterPacket<TPacket>([NotNull] Action<IConnection, TPacket> handler, byte id)
        where TPacket : IClientPacket, new()
    {
        // Create the handler.
        // TODO(pref): Is there a better way of handling the packet than to create another layer of indirection?
        var handlerData = new PacketHandlingData(
            () => new TPacket(),
            (conn, packet) => handler(conn, (TPacket)packet));

        _handlers.Add(id, handlerData);
    }
}
