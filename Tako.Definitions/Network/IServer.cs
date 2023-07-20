using Tako.Definitions.Game;
using Tako.Definitions.Game.Chat;
using Tako.Definitions.Game.Players;
using Tako.Definitions.Game.World;
using Tako.Definitions.Network.Connections;
using Tako.Definitions.Settings;

namespace Tako.Definitions.Network;

/// <summary>
/// An interface for the server.
/// </summary>
public interface IServer
{
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
