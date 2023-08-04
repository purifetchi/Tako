using Tako.Definitions.Game;
using Tako.Definitions.Game.Chat;
using Tako.Definitions.Game.Players;
using Tako.Definitions.Game.World;
using Tako.Definitions.Network.Connections;
using Tako.Definitions.Plugins;
using Tako.Definitions.Plugins.Events;
using Tako.Definitions.Settings;

namespace Tako.Definitions.Network;

/// <summary>
/// An interface for the server.
/// </summary>
public interface IServer
{
	/// <summary>
	/// Is the server active?
	/// </summary>
	bool Active { get; }

	/// <summary>
	/// The time (in seconds) since the server has started.
	/// </summary>
	float Time { get; }

	/// <summary>
	/// The realm manager of this server.
	/// </summary>
	IRealmManager RealmManager { get; }

	/// <summary>
	/// The network manager for this server.
	/// </summary>
	INetworkManager NetworkManager { get; }

	/// <summary>
	/// The chat instance for this server.
	/// </summary>
	IChat Chat { get; }

	/// <summary>
	/// The server settings.
	/// </summary>
	ISettings Settings { get; }

	/// <summary>
	/// The plugin manager.
	/// </summary>
	IPluginManager PluginManager { get; }

	/// <summary>
	/// Sent when the server makes one simulation tick.
	/// </summary>
	IEvent<float> OnServerTick { get; } 

	/// <summary>
	/// Adds a player for the given connection.
	/// </summary>
	/// <param name="name">The player's name.</param>
	/// <param name="realm">The realm.</param>
	/// <param name="connection">The connection.</param>
	/// <returns>The player.</returns>
	IPlayer AddPlayer(string name, IRealm realm, IConnection connection);

	/// <summary>
	/// Shuts down this server.
	/// </summary>
	void Shutdown();
}
