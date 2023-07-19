using Tako.Common.Network.Serialization;
using Tako.Definitions.Network.Packets;

namespace Tako.Server.Network.Packets.Server;

/// <summary>
/// Sent to clients periodically. 
/// The only way a client can disconnect at the moment is to force it closed, which does not let the server know. 
/// The ping packet is used to determine if the connection is still open. 
/// </summary>
public struct PingMessage : IServerPacket
{
	/// <inheritdoc/>
	public byte PacketId => 0x01;

	/// <inheritdoc/>
	public void Serialize(ref NetworkWriter writer)
	{
		writer.Write(PacketId);
	}
}
