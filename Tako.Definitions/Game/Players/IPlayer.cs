﻿using System.Numerics;
using Tako.Common.Numerics;
using Tako.Definitions.Network;
using Tako.Definitions.Network.Connections;

namespace Tako.Definitions.Game.Players;

/// <summary>
/// A player.
/// </summary>
public interface IPlayer
{
	/// <summary>
	/// The player id.
	/// </summary>
	sbyte PlayerId { get; }

	/// <summary>
	/// This player's name.
	/// </summary>
	string Name { get; }

	/// <summary>
	/// Is this player the op?
	/// </summary>
	bool Op { get; }

	/// <summary>
	/// The position of the player.
	/// </summary>
	Vector3 Position { get; }

	/// <summary>
	/// The orientation of the player.
	/// </summary>
	Orientation Orientation { get; }

	/// <summary>
	/// The connection this player has.
	/// </summary>
	IConnection? Connection { get; }

	/// <summary>
	/// The realm this player is attached to.
	/// </summary>
	IRealm Realm { get; }

	/// <summary>
	/// Sets the op status.
	/// </summary>
	/// <param name="op">The op status.</param>
	void SetOpStatus(bool op);

	/// <summary>
	/// Sets the position and orientation of this player.
	/// </summary>
	/// <param name="position">The position.</param>
	/// <param name="orientation">The orientation.</param>
	void SetPositionAndOrientation(Vector3 position, Orientation orientation);

	/// <summary>
	/// Teleports a player to a position.
	/// </summary>
	/// <param name="position">The position.</param>
	void Teleport(Vector3 position);

	/// <summary>
	/// Spawns the player at the given position.
	/// </summary>
	/// <param name="position">The position.</param>
	void Spawn(Vector3 position);

	/// <summary>
	/// Disconnects a player.
	/// </summary>
	/// <param name="reason">The reason.</param>
	void Disconnect(string? reason = null);

	/// <summary>
	/// Pings this player.
	/// </summary>
	void Ping();

	/// <summary>
	/// Sets the new realm.
	/// </summary>
	/// <param name="realm">The realm.</param>
	void SetRealm(IRealm realm);
}
