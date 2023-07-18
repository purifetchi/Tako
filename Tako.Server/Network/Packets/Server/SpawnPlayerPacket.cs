using Tako.Common.Network.Serialization;
using Tako.Common.Numerics;
using Tako.Definitions.Network.Packets;

namespace Tako.Server.Network.Packets.Server;

/// <summary>
/// Sent to indicate where a new player is spawning in the world.
/// 
/// This will set the player's spawn point.
/// </summary>
public struct SpawnPlayerPacket : IServerPacket
{
	/// <inheritdoc/>
	public byte PacketId => 0x07;

	/// <summary>
	/// The player id.
	/// </summary>
	public sbyte PlayerId { get; set; }

	/// <summary>
	/// The player name.
	/// </summary>
	public string PlayerName { get; set; }

	/// <summary>
	/// The X position.
	/// </summary>
	public FShort X { get; set; }

	/// <summary>
	/// The Y position.
	/// </summary>
	public FShort Y { get; set; }

	/// <summary>
	/// The Z position.
	/// </summary>
	public FShort Z { get; set; }

	/// <summary>
	/// The yaw.
	/// </summary>
	public byte Yaw { get; set; }

	/// <summary>
	/// The pitch.
	/// </summary>
	public byte Pitch { get; set; }

	/// <inheritdoc/>
	public void Serialize(ref NetworkWriter writer)
	{
		writer.Write(PacketId);
		writer.Write(PlayerId);
		writer.WriteString(PlayerName);
		writer.WriteFShort(X);
		writer.WriteFShort(Y);
		writer.WriteFShort(Z);
		writer.Write(Yaw);
		writer.Write(Pitch);
	}
}
