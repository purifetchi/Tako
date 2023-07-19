using Tako.Definitions.Game.Chat;
using Tako.Definitions.Game.Players;
using Tako.Definitions.Game.World;
using Tako.Definitions.Network.Connections;

namespace Tako.Definitions.Network;

/// <summary>
/// An interface for the server.
/// </summary>
public interface IServer
{
	/// <summary>
	/// The current world of the server.
	/// </summary>
	IWorld World { get; }

	/// <summary>
	/// The network manager for this server.
	/// </summary>
	INetworkManager NetworkManager { get; }

	/// <summary>
	/// The dictionary containing all players.
	/// </summary>
	IReadOnlyDictionary<sbyte, IPlayer> Players { get; }

	/// <summary>
	/// The chat instance for this server.
	/// </summary>
	IChat Chat { get; }

	/// <summary>
	/// Adds a player for the given connection.
	/// </summary>
	/// <param name="name">The player's name.</param>
	/// <param name="connection">The connection.</param>
	/// <returns>The player.</returns>
	IPlayer AddPlayer(string name, IConnection connection);

	/// <summary>
	/// Adds an NPC.
	/// </summary>
	/// <param name="name">The name of the npc.</param>
	/// <returns>The npc.</returns>
	IPlayer AddNpc(string name);

	/// <summary>
	/// Shuts down this server.
	/// </summary>
	void Shutdown();
}
