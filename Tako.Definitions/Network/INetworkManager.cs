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
	/// Sends a packet to all the people.
	/// </summary>
	/// <param name="packet">The packet.</param>
	void SendToAll(IServerPacket packet);

	/// <summary>
	/// Processes all the current and incoming connections.
	/// </summary>
	void Receive();
}
