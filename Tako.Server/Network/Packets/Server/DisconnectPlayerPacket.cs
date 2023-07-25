using Tako.Common.Network.Serialization;
using Tako.Definitions.Network.Packets;

namespace Tako.Server.Network.Packets.Server;

/// <summary>
/// Sent to a player when they're disconnected from the server. 
/// </summary>
public struct DisconnectPlayerPacket : IServerPacket
{
    /// <inheritdoc/>
    public byte PacketId => 0x0e;

    /// <summary>
    /// The reason for disconnection.
    /// </summary>
    public string DisconnectReason { get; set; }

    /// <inheritdoc/>
    public void Serialize(ref NetworkWriter writer)
    {
        writer.Write(PacketId);
        writer.WriteString(DisconnectReason);
    }
}
