using System.Numerics;
using Tako.Common.Numerics;
using Tako.Definitions.Game.World;
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
		NetworkManager.PacketProcessor.RegisterPacket<Packets.Client.SetBlockPacket>(OnSetBlockPacket, 0x05);
		NetworkManager.PacketProcessor.RegisterPacket<PositionAndOrientationPacket>(OnPositionAndOrientationPacket, 0x08);
		NetworkManager.PacketProcessor.RegisterPacket<Packets.Client.MessagePacket>(OnMessagePacket, 0x0d);
	}

	/// <summary>
	/// Handles the client identification packet.
	/// </summary>
	/// <param name="packet">The packet.</param>
	private void OnClientIdentificationPacket(IConnection conn, ClientIdentificationPacket packet)
	{
		_logger.Info($"User {packet.Username} with protocol version {packet.ProtocolVersion} wants to log in. [Key={packet.VerificationKey}, Capabilities={packet.Capabilities}]");
		var primaryRealm = Realms.First(realm => realm.IsPrimaryRealm);

		var player = AddPlayer(packet.Username, primaryRealm, conn);
		conn.PlayerId = player.PlayerId;
		primaryRealm.MovePlayer(player);
	}

	/// <summary>
	/// Handles the position and orientation packet.
	/// </summary>
	/// <param name="conn">The connection.</param>
	/// <param name="packet">The packet.</param>
	private void OnPositionAndOrientationPacket(IConnection conn, PositionAndOrientationPacket packet)
	{
		// TODO(pref): Optimize!!!
		Realms.First(realm => realm.Players.ContainsKey(conn.PlayerId))?
			.Players.Values
			.FirstOrDefault(player => player.Connection == conn)?
			.SetPositionAndOrientation(
				new Vector3(packet.X, packet.Y, packet.Z), 
				new Orientation(packet.Yaw, packet.Pitch));
	}

	/// <summary>
	/// Handles the set block packet.
	/// </summary>
	/// <param name="conn">The connection.</param>
	/// <param name="packet">The packet.</param>
	private void OnSetBlockPacket(IConnection conn, Packets.Client.SetBlockPacket packet)
	{
		// Find the realm
		// TODO(pref): Optimize!!!
		var realm = Realms.FirstOrDefault(
			realm => realm.Players.ContainsKey(conn.PlayerId));

		var pos = new Vector3Int(packet.X, packet.Y, packet.Z);

		_logger.Debug($"Changing block for realm '{realm?.Name}'");

		switch (packet.Mode)
		{
			case BlockChangeMode.Destroyed:
				realm?.World?.SetBlock(pos, (byte)ClassicBlockType.Air);
				break;

			case BlockChangeMode.Created:
				realm?.World?.SetBlock(pos, packet.BlockType);
				break;
		}
	}

	/// <summary>
	/// Handles the message packet.
	/// </summary>
	/// <param name="conn">The connection.</param>
	/// <param name="packet">The packet.</param>
	private void OnMessagePacket(IConnection conn, Packets.Client.MessagePacket packet)
	{
		var player = Realms.First(realm => realm.Players.ContainsKey(conn.PlayerId))?
			.Players[conn.PlayerId];

		if (player is null)
		{
			_logger.Warn($"An unknown player tried to send a chat messsage. [id={conn.ConnectionId}]");
			return;
		}

		Chat.SendMessage(player, packet.Message);
	}
}
