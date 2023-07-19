using Tako.Definitions.Game.Players;
using Tako.Definitions.Game.World;
using Tako.Definitions.Network;
using Tako.Definitions.Network.Connections;
using Tako.Definitions.Network.Packets;

namespace Tako.Definitions.Game;

/// <summary>
/// A realm containing a world and its players.
/// </summary>
public interface IRealm
{
	/// <summary>
	/// The name of this realm.
	/// </summary>
	string Name { get; }

	/// <summary>
	/// Is this the primary realm?
	/// </summary>
	bool IsPrimaryRealm { get; }

	/// <summary>
	/// The world list.
	/// </summary>
	IWorld World { get; }

	/// <summary>
	/// The server containing this realm.
	/// </summary>
	IServer Server { get; }

	/// <summary>
	/// The players dictionary.
	/// </summary>
	IReadOnlyDictionary<sbyte, IPlayer> Players { get; }

	/// <summary>
	/// Gets the world generator for a realm.
	/// </summary>
	/// <returns>The world generator.</returns>
	IWorldGenerator GetWorldGenerator();

	/// <summary>
	/// Sends this packet to all people within this realm.
	/// </summary>
	/// <param name="packet">The packet.</param>
	void SendToAllWithinRealm(IServerPacket packet);

	/// <summary>
	/// Sends this packet to all people within this realm that match a predicate.
	/// </summary>
	/// <param name="packet">The packet.</param>
	/// <param name="func">The predicate</param>
	void SendToAllWithinRealmThatMatch(IServerPacket packet, Func<IConnection, bool> func);

	/// <summary>
	/// Sets the world for this realm.
	/// </summary>
	/// <param name="world">The world.</param>
	void SetWorld(IWorld world);

	/// <summary>
	/// Moves the player to this realm.
	/// </summary>
	/// <param name="player">The player.</param>
	void MovePlayer(IPlayer player);

	/// <summary>
	/// Gives up a player.
	/// </summary>
	/// <param name="player">The player.</param>
	void GiveUpPlayer(IPlayer player);

	/// <summary>
	/// Heartbeats the players in this realm.
	/// </summary>
	void HeartbeatPlayers();
}
