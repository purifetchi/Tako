using Tako.Common.Network.Serialization;
using Tako.Definitions.Network.Packets;

namespace Tako.Server.Network.Packets.Server;

/// <summary>
/// Notifies the player of incoming level data.
/// </summary>
public struct LevelInitializePacket : IServerPacket
{
	/// <inheritdoc/>
	public byte PacketId => 0x02;

	/// <inheritdoc/>
	public void Serialize(ref NetworkWriter writer)
	{
		writer.Write(PacketId);
	}
}
