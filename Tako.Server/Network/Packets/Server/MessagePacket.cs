using Tako.Common.Network.Serialization;
using Tako.Definitions.Network.Packets;

namespace Tako.Server.Network.Packets.Server;

/// <summary>
/// Messages sent by chat or from the console.
/// </summary>
public struct MessagePacket : IServerPacket
{
    /// <inheritdoc/>
    public byte PacketId => 0x0d;

    /// <summary>
    /// The player id.
    /// </summary>
    public sbyte PlayerId { get; set; }

    /// <summary>
    /// The message.
    /// </summary>
    public string Message { get; set; }

    /// <inheritdoc/>
    public void Serialize(ref NetworkWriter writer)
    {
        writer.Write(PacketId);
        writer.Write(PlayerId);
        writer.WriteString(Message);
    }
}
