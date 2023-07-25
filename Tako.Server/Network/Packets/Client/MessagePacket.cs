using Tako.Common.Network.Serialization;
using Tako.Definitions.Network.Packets;

namespace Tako.Server.Network.Packets.Client;

/// <summary>
/// Contains chat messages sent by player. 
/// </summary>
public struct MessagePacket : IClientPacket
{
    /// <summary>
    /// Player ID is always -1 (255), referring to itself. 
    /// </summary>
    public sbyte PlayerID { get; set; }

    /// <summary>
    /// The message.
    /// </summary>
    public string Message { get; set; }

    /// <inheritdoc/>
    public void Deserialize(ref NetworkReader reader)
    {
        PlayerID = reader.Read<sbyte>();
        Message = reader.ReadString();
    }
}
