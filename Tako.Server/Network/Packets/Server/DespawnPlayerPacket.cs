using Tako.Common.Network.Serialization;
using Tako.Definitions.Network.Packets;

namespace Tako.Server.Network.Packets.Server;

/// <summary>
/// Sent to others when the player disconnects.
/// </summary>
public struct DespawnPlayerPacket : IServerPacket
{
	/// <inheritdoc/>
	public byte PacketId => 0x0c;

	/// <summary>
	/// The player id.
	/// </summary>
	public sbyte PlayerId { get; set; }

	/// <inheritdoc/>
	public void Serialize(ref NetworkWriter writer)
	{
		writer.Write(PacketId);
		writer.Write(PlayerId);
	}
}
