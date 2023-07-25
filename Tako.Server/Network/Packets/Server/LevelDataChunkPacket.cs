using Tako.Common.Network.Serialization;
using Tako.Definitions.Network.Packets;

namespace Tako.Server.Network.Packets.Server;

/// <summary>
/// Contains a chunk of the gzipped level data, to be concatenated with the rest. 
/// Chunk Data is up to 1024 bytes, padded with 0x00s if less. 
/// </summary>
public struct LevelDataChunkPacket : IServerPacket
{
    /// <inheritdoc/>
    public byte PacketId => 0x03;

    /// <summary>
    /// The chunk length.
    /// </summary>
    public short ChunkLength { get; set; }

    /// <summary>
    /// The chunk data.
    /// </summary>
    public ArraySegment<byte> ChunkData { get; set; }

    /// <summary>
    /// The percentage of completeness.
    /// </summary>
    public byte PercentComplete { get; set; }

    /// <inheritdoc/>
    public void Serialize(ref NetworkWriter writer)
    {
        writer.Write(PacketId);
        writer.WriteShortBigEndian(ChunkLength);
        writer.WriteChunk(ChunkData);
        writer.Write(PercentComplete);
    }
}
