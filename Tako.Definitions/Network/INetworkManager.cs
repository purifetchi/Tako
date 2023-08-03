using Tako.Common.Allocation;
using Tako.Definitions.Network.Connections;
using Tako.Definitions.Network.Packets;

namespace Tako.Definitions.Network;

/// <summary>
/// An interface for the network manager.
/// </summary>
public interface INetworkManager
{
	/// <summary>
	/// The connections that this network manager has.
	/// </summary>
	IReadOnlyList<IConnection> Connections { get; }

	/// <summary>
	/// The packet processor.
	/// </summary>
	IPacketProcessor PacketProcessor { get; }

	/// <summary>
	/// The connection id allocator.
	/// </summary>
	IdAllocator<byte> IdAllocator { get; }

	/// <summary>
	/// Starts listening.
	/// </summary>
	void StartListening();

	/// <summary>
	/// Stops listening.
	/// </summary>
	void StopListening();

	/// <summary>
	/// Sends a packet to all the people.
	/// </summary>
	/// <param name="packet">The packet.</param>
	void SendToAll(IServerPacket packet);

	/// <summary>
	/// Sends a packet to all clients that match.
	/// </summary>
	/// <param name="packet">The packet.</param>
	/// <param name="match">The query.</param>
	void SendToAllThatMatch(IServerPacket packet, Func<IConnection, bool> match);

	/// <summary>
	/// Processes all the current and incoming connections.
	/// </summary>
	void Receive();

	/// <summary>
	/// Adds a new connection.
	/// </summary>
	/// <param name="conn">The connection.</param>
	void AddConnection(IConnection conn);

	/// <summary>
	/// Adds a transport provider.
	/// </summary>
	/// <param name="provider"></param>
	void AddTransportProvider(ITransportProvider provider);
}
