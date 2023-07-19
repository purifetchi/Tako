using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tako.Common.Network.Serialization;
using Tako.Definitions.Network.Packets;

namespace Tako.Server.Network.Packets.Server;

/// <summary>
/// Sent to indicate a block change by physics or by players. 
/// </summary>
public struct SetBlockPacket : IServerPacket
{
	/// <inheritdoc/>
	public byte PacketId => 0x06;

	/// <summary>
	/// The X coordinate.
	/// </summary>
	public short X { get; set; }

	/// <summary>
	/// The Y coordinate.
	/// </summary>
	public short Y { get; set; }

	/// <summary>
	/// The Z coordinate.
	/// </summary>
	public short Z { get; set; }

	/// <summary>
	/// The block type.
	/// </summary>
	public byte BlockType { get; set; }

	/// <inheritdoc/>
	public void Serialize(ref NetworkWriter writer)
	{
		writer.Write(PacketId);
		writer.WriteShortBigEndian(X);
		writer.WriteShortBigEndian(Y);
		writer.WriteShortBigEndian(Z);
		writer.Write(BlockType);
	}
}
