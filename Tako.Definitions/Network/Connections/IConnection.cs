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
	/// Processes the data on this connection.
	/// </summary>
	void Process();

	/// <summary>
	/// Sends this connection some data.
	/// </summary>
	/// <param name="data">The data.</param>
	void Send(ReadOnlyMemory<byte> data);

	/// <summary>
	/// Disconnects this connection.
	/// </summary>
	void Disconnect();
}
