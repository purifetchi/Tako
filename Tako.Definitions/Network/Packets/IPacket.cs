namespace Tako.Definitions.Network.Packets;

/// <summary>
/// A network packet.
/// </summary>
public interface IPacket
{
	/// <summary>
	/// A packet id.
	/// </summary>
	int PacketId { get; }

	void Serialize();

	void Deserialize();
}
