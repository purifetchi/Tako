using Tako.Common.Network.Serialization;
using Tako.Definitions.Game.Players;
using Tako.Definitions.Network;
using Tako.Definitions.Network.Packets;

namespace Tako.Server.Network.Packets.Server;

/// <summary>
/// The identification packet we send to the client.
/// </summary>
public struct ServerIdentificationPacket : IServerPacket
{
	/// <inheritdoc/>
	public byte PacketId => 0x00;

	/// <summary>
	/// The protocol version.
	/// </summary>
	public ProtocolVersion ProtocolVersion { get; set; }

	/// <summary>
	/// The server name.
	/// </summary>
	public string ServerName { get; set; }

	/// <summary>
	/// The server motd.
	/// </summary>
	public string ServerMOTD { get; set; }

	/// <summary>
	/// The player type.
	/// </summary>
	public PlayerType Type { get; set; }

	/// <inheritdoc/>
	public void Serialize(ref NetworkWriter writer)
	{
		writer.Write(PacketId);
		writer.Write(ProtocolVersion);
		writer.WriteString(ServerName);
		writer.WriteString(ServerMOTD);
		writer.Write(Type);
	}
}
