using Tako.Common.Network.Serialization;
using Tako.Definitions.Network.Packets;

namespace Tako.Definitions.Network.Connections;

/// <summary>
/// An interface for a client connection.
/// </summary>
public interface IConnection
{
	/// <summary>
	/// The connection id.
	/// </summary>
	byte ConnectionId { get; }

	/// <summary>
	/// Whether the socket is connected.
	/// </summary>
	bool Connected { get; }

	/// <summary>
	/// Checks whether the socket has any inbound data.
	/// </summary>
	bool HasData();

	/// <summary>
	/// Gets the reader for the data.
	/// </summary>
	/// <returns>The network reader.</returns>
	NetworkReader GetReader();

	/// <summary>
	/// Sends this connection some data.
	/// </summary>
	/// <param name="data">The data.</param>
	void Send(ReadOnlySpan<byte> data);

	/// <summary>
	/// Sends this connection a packet.
	/// </summary>
	/// <param name="packet">The packet.</param>
	void Send(IServerPacket packet);

	/// <summary>
	/// Disconnects this connection.
	/// </summary>
	void Disconnect();
}
