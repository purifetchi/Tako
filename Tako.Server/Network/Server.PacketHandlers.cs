using System.Numerics;
using Tako.Common.Numerics;
using Tako.Definitions.Game.World;
using Tako.Definitions.Network;
using Tako.Definitions.Network.Connections;
using Tako.Server.Game.Players;
using Tako.Server.Network.Packets.Client;

namespace Tako.Server.Network;
public partial class Server
{
    /// <summary>
    /// Registers all packet handlers.
    /// </summary>
    private void RegisterHandlers()
    {
        NetworkManager.PacketProcessor.RegisterPacket<ClientIdentificationPacket>(OnClientIdentificationPacket, 0x00);
        NetworkManager.PacketProcessor.RegisterPacket<SetBlockPacket>(OnSetBlockPacket, 0x05);
        NetworkManager.PacketProcessor.RegisterPacket<PositionAndOrientationPacket>(OnPositionAndOrientationPacket, 0x08);
        NetworkManager.PacketProcessor.RegisterPacket<MessagePacket>(OnMessagePacket, 0x0d);
    }

    /// <summary>
    /// Handles the client identification packet.
    /// </summary>
    /// <param name="packet">The packet.</param>
    private void OnClientIdentificationPacket(IConnection conn, ClientIdentificationPacket packet)
    {
        _logger.Info($"User {packet.Username} with protocol version {packet.ProtocolVersion} wants to log in. [Key={packet.VerificationKey}, Capabilities={packet.Capabilities}]");

        if (packet.ProtocolVersion != ProtocolVersion.Version7)
        {
            conn?.Send(new Packets.Server.DisconnectPlayerPacket
            {
                DisconnectReason = DisconnectMessages.PROTOCOL_MISMATCH
            });
            conn?.Disconnect();
            return;
        }

        if (_authenticatePlayers &&
            _heartbeatService?.AuthenticatePlayer(packet.VerificationKey, packet.Username) != true)
        {
            _logger.Warn($"Player {packet.Username} failed authentication.");

            conn?.Send(new Packets.Server.DisconnectPlayerPacket
            {
                DisconnectReason = DisconnectMessages.INVALID_AUTHENTICATION_KEY
            });
            conn?.Disconnect();
            return;
        }

        var primaryRealm = RealmManager.GetDefaultRealm()!;

        var player = AddPlayer(packet.Username, primaryRealm, conn);
        conn.PlayerId = player.PlayerId;
        primaryRealm.MovePlayer(player);
    }

    /// <summary>
    /// Handles the position and orientation packet.
    /// </summary>
    /// <param name="conn">The connection.</param>
    /// <param name="packet">The packet.</param>
    private void OnPositionAndOrientationPacket(IConnection conn, PositionAndOrientationPacket packet)
    {
        var player = RealmManager.FindPlayerInRealms(conn.PlayerId);
        player?.SetPositionAndOrientation(
                new Vector3(packet.X, packet.Y, packet.Z),
                new Orientation(packet.Yaw, packet.Pitch));
    }

    /// <summary>
    /// Handles the set block packet.
    /// </summary>
    /// <param name="conn">The connection.</param>
    /// <param name="packet">The packet.</param>
    private void OnSetBlockPacket(IConnection conn, Packets.Client.SetBlockPacket packet)
    {
        // Find the realm
        var player = RealmManager.FindPlayerInRealms(conn.PlayerId);
        if (player is null)
            return;

        var realm = player.Realm;
        if (realm is null || realm.World is null)
            return;

        var pos = new Vector3Int(packet.X, packet.Y, packet.Z);

        _logger.Debug($"Changing block for realm '{realm.Name}'");

        // Get the block definitions.
        var blockId = realm
            .World
            .GetBlock(pos);

        var oldBlock = player.Realm
            .BlockManager
            .GetBlockById(blockId)!;

        var newBlock = player.Realm
            .BlockManager
            .GetBlockById(packet.BlockType);

        if (newBlock is null)
        {
            player.Disconnect(string.Format(DisconnectMessages.INVALID_BLOCK_ID, blockId));
            return;
        }

        switch (packet.Mode)
        {
            case BlockChangeMode.Destroyed:
                if (!oldBlock.CanBreak(player))
                {
                    Chat.SendServerMessageTo(player, "&cSorry, you cannot break this block.");
                    realm.World?.SetBlock(pos, oldBlock.Id);
                    break;
                }

                realm.World?.SetBlock(pos, (byte)ClassicBlockType.Air);
                break;

            case BlockChangeMode.Created:
                if (!newBlock.CanPlace(player))
                {
                    Chat.SendServerMessageTo(player, "&cSorry, you cannot place this block.");
                    realm.World?.SetBlock(pos, oldBlock.Id);
                    break;
                }

                realm.World?.SetBlock(pos, packet.BlockType);
                break;
        }
    }

    /// <summary>
    /// Handles the message packet.
    /// </summary>
    /// <param name="conn">The connection.</param>
    /// <param name="packet">The packet.</param>
    private void OnMessagePacket(IConnection conn, Packets.Client.MessagePacket packet)
    {
        var player = RealmManager.FindPlayerInRealms(conn.PlayerId);
        if (player is null)
        {
            _logger.Warn($"An unknown player tried to send a chat messsage. [id={conn.ConnectionId}]");
            return;
        }

        Chat.SendMessage(player, packet.Message);
    }
}
