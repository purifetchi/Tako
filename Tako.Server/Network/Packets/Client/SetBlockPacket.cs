using Tako.Common.Network.Serialization;
using Tako.Definitions.Game.World;
using Tako.Definitions.Network.Packets;

namespace Tako.Server.Network.Packets.Client;

/// <summary>
/// Sent when a user changes a block.
/// </summary>
public struct SetBlockPacket : IClientPacket
{
    /// <summary>
    /// The X coordinate.
    /// </summary>
    public short X { get; set; }

    /// <summary>
    /// The Y coordinate.
    /// </summary>
    public short Y { get; set; }

    /// <summary>
    /// The Z coordinate.
    /// </summary>
    public short Z { get; set; }

    /// <summary>
    /// Indicates whether a block was created (0x01) or destroyed (0x00). 
    /// </summary>
    public BlockChangeMode Mode { get; set; }

    /// <summary>
    /// The block type.
    /// </summary>
    public byte BlockType { get; set; }

    /// <inheritdoc/>
    public void Deserialize(ref NetworkReader reader)
    {
        X = reader.ReadShortBigEndian();
        Y = reader.ReadShortBigEndian();
        Z = reader.ReadShortBigEndian();
        Mode = reader.Read<BlockChangeMode>();
        BlockType = reader.Read<byte>();
    }
}
