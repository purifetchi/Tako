using Tako.Common.Network.Serialization;
using Tako.Definitions.Game.Players;
using Tako.Definitions.Network.Packets;

namespace Tako.Server.Network.Packets.Server;

/// <summary>
/// Sent when a player is opped/deopped.
/// </summary>
public struct UpdatePlayerTypePacket : IServerPacket
{
	/// <inheritdoc/>
	public byte PacketId => 0x0f;

	/// <summary>
	/// The new player type.
	/// </summary>
	public PlayerType Type { get; set; }

	/// <inheritdoc/>
	public void Serialize(ref NetworkWriter writer)
	{
		writer.Write(PacketId);
		writer.Write(Type);
	}
}
