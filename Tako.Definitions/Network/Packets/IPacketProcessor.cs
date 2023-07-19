using Tako.Common.Network.Serialization;
using Tako.Definitions.Network.Connections;

namespace Tako.Definitions.Network.Packets;

/// <summary>
/// An interface for the class responsible for managing unpacking packets and executing the handlers for them.
/// </summary>
public interface IPacketProcessor
{
	/// <summary>
	/// Registers a handler for an incoming client packet.
	/// </summary>
	/// <typeparam name="TPacket">The type of the client packet.</typeparam>
	/// <param name="handler">The handler to execute.</param>
	void RegisterPacket<TPacket>(Action<IConnection, TPacket> handler, byte id) 
		where TPacket: IClientPacket, new();

	/// <summary>
	/// Handles an incoming packet.
	/// </summary>
	/// <param name="reader">The reader to read from.</param>
	/// <param name="conn">The connection it belongs to.</param>
	void HandleIncomingPacket(ref NetworkReader reader, IConnection conn);
}
