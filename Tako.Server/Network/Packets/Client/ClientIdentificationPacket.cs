using Tako.Common.Network.Serialization;
using Tako.Definitions.Network;
using Tako.Definitions.Network.Capabilities;
using Tako.Definitions.Network.Packets;

namespace Tako.Server.Network.Packets.Client;

/// <summary>
/// The client identification packet.
/// </summary>
public struct ClientIdentificationPacket : IClientPacket
{
	/// <inheritdoc/>
	public int PacketId => 0x0;

	/// <summary>
	/// The protocol version.
	/// </summary>
	public ProtocolVersion ProtocolVersion { get; private set; }

	/// <summary>
	/// The username.
	/// </summary>
	public string Username { get; private set; }

	/// <summary>
	/// The verification key.
	/// </summary>
	public string VerificationKey { get; private set; }

	/// <summary>
	/// Usually reserved for padding but clients compliant with CPE use it to advertise it.
	/// </summary>
	public ClientIdentificationCapability Capabilities { get; private set; }

	/// <inheritdoc/>
	public void Deserialize(ref NetworkReader reader)
	{
		ProtocolVersion = reader.Read<ProtocolVersion>();
		Username = reader.ReadString();
		VerificationKey = reader.ReadString();
		Capabilities = reader.Read<ClientIdentificationCapability>();
	}
}
