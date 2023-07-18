using Tako.Common.Network.Serialization;

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
	/// Disconnects this connection.
	/// </summary>
	void Disconnect();
}
