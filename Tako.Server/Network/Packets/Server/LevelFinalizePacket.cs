using Tako.Common.Network.Serialization;
using Tako.Definitions.Network.Packets;

namespace Tako.Server.Network.Packets.Server;

/// <summary>
/// Sent after level data is complete and gives map dimensions.
/// The y coordinate is how tall the map is.
/// </summary>
public struct LevelFinalizePacket : IServerPacket
{
    /// <inheritdoc/>
    public byte PacketId => 0x04;

    /// <summary>
    /// The x size.
    /// </summary>
    public short XSize { get; set; }

    /// <summary>
    /// The Y size.
    /// </summary>
    public short YSize { get; set; }

    /// <summary>
    /// The Z size.
    /// </summary>
    public short ZSize { get; set; }

    /// <inheritdoc/>
    public void Serialize(ref NetworkWriter writer)
    {
        writer.Write(PacketId);
        writer.WriteShortBigEndian(XSize);
        writer.WriteShortBigEndian(YSize);
        writer.WriteShortBigEndian(ZSize);
    }
}
