using Tako.Common.Network.Serialization;

namespace Tako.Definitions.Network.Packets;

/// <summary>
/// A client sent to us by the client.
/// </summary>
public interface IClientPacket
{
	/// <summary>
	/// Deserializes this packet.
	/// </summary>
	/// <param name="reader">The network reader.</param>
	void Deserialize(NetworkReader reader);
}
