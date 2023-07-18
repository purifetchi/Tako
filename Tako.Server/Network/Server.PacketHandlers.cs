using Tako.Definitions.Network.Connections;
using Tako.Server.Network.Packets.Client;
using Tako.Server.Network.Packets.Server;

namespace Tako.Server.Network;
public partial class Server
{
	/// <summary>
	/// Registers all packet handlers.
	/// </summary>
	private void RegisterHandlers()
	{
		NetworkManager.PacketProcessor.RegisterPacket<ClientIdentificationPacket>(OnClientIdentificationPacket, 0x00);
		NetworkManager.PacketProcessor.RegisterPacket<PositionAndOrientationPacket>(OnPositionAndOrientationPacket, 0x08);
	}

	/// <summary>
	/// Handles the client identification packet.
	/// </summary>
	/// <param name="packet">The packet.</param>
	private void OnClientIdentificationPacket(IConnection conn, ClientIdentificationPacket packet)
	{
		_logger.Info($"User {packet.Username} with protocol version {packet.ProtocolVersion} wants to log in. [Key={packet.VerificationKey}]");
		conn.Send(new ServerIdentificationPacket
		{
			ProtocolVersion = packet.ProtocolVersion,
			ServerName = ServerName,
			ServerMOTD = MOTD,
			Type = Definitions.Game.Players.PlayerType.Regular
		});

		World?.StreamTo(conn);
		AddPlayer(packet.Username, conn)
			.Spawn(new System.Numerics.Vector3(20, 12, 20));
	}

	/// <summary>
	/// Handles the position and orientation packet.
	/// </summary>
	/// <param name="conn">The connection.</param>
	/// <param name="packet">The packet.</param>
	private void OnPositionAndOrientationPacket(IConnection conn, PositionAndOrientationPacket packet)
	{
		Players.Values
			.FirstOrDefault(player => player.Connection == conn)?
			.SetPosition(new System.Numerics.Vector3(packet.X, packet.Y, packet.Z));
	}
}
