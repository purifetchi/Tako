using Tako.Common.Network.Serialization;

namespace Tako.Definitions.Network.Packets;

/// <summary>
/// A packet sent from us to a client.
/// </summary>
public interface IServerPacket
{
	/// <summary>
	/// Serializes this server packet to a network writer.
	/// </summary>
	/// <param name="writer">The network writer.</param>
	void Serialize(NetworkWriter writer);
}
