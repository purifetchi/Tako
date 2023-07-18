using Tako.Common.Network.Serialization;
using Tako.Common.Numerics;
using Tako.Definitions.Network.Packets;

namespace Tako.Server.Network.Packets.Server;

/// <summary>
/// Sent with changes in player position and rotation. Used for sending initial position on the map, and teleportation.
/// </summary>
public struct SetPositionAndOrientationPacket : IServerPacket
{
	/// <inheritdoc/>
	public byte PacketId => 0x08;

	/// <summary>
	/// The player id.
	/// </summary>
	public sbyte PlayerId { get; set; }

	/// <summary>
	/// The X coordinate.
	/// </summary>
	public FShort X { get; set; }

	/// <summary>
	/// The Y coordinate.
	/// </summary>
	public FShort Y { get; set; }

	/// <summary>
	/// The Z coordinate.
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
		writer.WriteFShort(X);
		writer.WriteFShort(Y);
		writer.WriteFShort(Z);
		writer.Write(Yaw);
		writer.Write(Pitch);
	}
}
