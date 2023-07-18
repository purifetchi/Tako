using Tako.Definitions.Network.Connections;
using Tako.Server.Network.Packets.Client;

namespace Tako.Server.Network;
public partial class Server
{
	/// <summary>
	/// Registers all packet handlers.
	/// </summary>
	private void RegisterHandlers()
	{
		NetworkManager.PacketProcessor.RegisterPacket<ClientIdentificationPacket>(OnClientIdentificationPacket, 0x00);
	}

	/// <summary>
	/// Handles the client identification packet.
	/// </summary>
	/// <param name="packet">The packet.</param>
	private void OnClientIdentificationPacket(IConnection conn, ClientIdentificationPacket packet)
	{
		_logger.Info($"User {packet.Username} with protocol version {packet.ProtocolVersion} wants to log in. [Key={packet.VerificationKey}]");
		AddPlayer(packet.Username, conn);

		World?.StreamTo(conn);
	}
}
