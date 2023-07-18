using Tako.Common.Network.Serialization;

namespace Tako.Definitions.Network.Packets;

/// <summary>
/// A network packet.
/// </summary>
public interface IPacket
{
	/// <summary>
	/// The packet id.
	/// </summary>
	int PacketId { get; }

	/// <summary>
	/// Serializes this packet into a network writer.
	/// </summary>
	/// <param name="writer">The network writer.</param>
	void Serialize(NetworkWriter writer);

	/// <summary>
	/// Deserializes this packet from a network reader.
	/// </summary>
	void Deserialize();
}
