﻿using Tako.Definitions.Game;
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
	/// The realms inside of this server.
	/// </summary>
	IReadOnlyList<IRealm> Realms { get; }

	/// <summary>
	/// The network manager for this server.
	/// </summary>
	INetworkManager NetworkManager { get; }

	/// <summary>
	/// The chat instance for this server.
	/// </summary>
	IChat Chat { get; }

	/// <summary>
	/// Creates a new realm.
	/// </summary>
	/// <param name="name">The name of the realm.</param>
	/// <param name="primary">Is it a primary realm?</param>
	/// <returns>The realm.</returns>
	IRealm CreateRealm(string name, bool primary = false);

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
