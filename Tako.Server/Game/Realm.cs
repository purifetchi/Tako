using Tako.Common.Logging;
using Tako.Definitions.Game;
using Tako.Definitions.Game.Players;
using Tako.Definitions.Game.World;
using Tako.Definitions.Network;
using Tako.Definitions.Network.Connections;
using Tako.Definitions.Network.Packets;
using Tako.Server.Game.World;
using Tako.Server.Logging;
using Tako.Server.Network.Packets.Server;

namespace Tako.Server.Game;

/// <summary>
/// A realm that a player can be in.
/// </summary>
public class Realm : IRealm
{
	/// <inheritdoc/>
	public string Name { get; init; } = null!;

	/// <inheritdoc/>
	public bool IsPrimaryRealm { get; init; }

	/// <inheritdoc/>
	public IWorld World { get; private set; } = null!;

	/// <inheritdoc/>
	public IServer Server { get; init; } = null!;

	/// <inheritdoc/>
	public IReadOnlyDictionary<sbyte, IPlayer> Players => _players;

	/// <summary>
	/// The players list for this realm.
	/// </summary>
	private readonly Dictionary<sbyte, IPlayer> _players;

	/// <summary>
	/// The disconnect queue.
	/// </summary>
	private readonly Queue<IPlayer> _disconnectQueue;

	/// <summary>
	/// Last time we've pinged all players.
	/// </summary>
	private long _lastPingTime;

	/// <summary>
	/// A realm.
	/// </summary>
	/// <param name="name"></param>
	/// <param name="isPrimaryRealm"></param>
	/// <param name="world"></param>
	/// <param name="server"></param>
	public Realm(
		string name, 
		bool isPrimaryRealm,
		IServer server)
	{
		Name = name;
		IsPrimaryRealm = isPrimaryRealm;
		Server = server;

		_players = new();
		_disconnectQueue = new();
	}

	/// <inheritdoc/>
	public IWorldGenerator GetWorldGenerator() => new WorldGenerator(this);

	/// <summary>
	/// The logger.
	/// </summary>
	private ILogger<Realm> _logger = LoggerFactory<Realm>.Get();

	/// <inheritdoc/>
	public void SendToAllWithinRealm(IServerPacket packet)
	{
		Server.NetworkManager.SendToAllThatMatch(packet,
			conn => _players.ContainsKey(conn.PlayerId));
	}

	/// <inheritdoc/>
	public void SendToAllWithinRealmThatMatch(
		IServerPacket packet, 
		Func<IConnection, bool> func)
	{
		Server.NetworkManager.SendToAllThatMatch(packet,
			conn => _players.ContainsKey(conn.PlayerId) && func(conn));
	}

	/// <inheritdoc/>
	public void SetWorld(IWorld world)
	{
		// TODO(pref): If we have players in this realm, switch them to this new world.
		World = world;
	}

	/// <inheritdoc/>
	public void MovePlayer(IPlayer player)
	{
		_logger.Info($"Moving player {player.Name} to realm {Name}");

		player.Connection?.Send(new ServerIdentificationPacket
		{
			ProtocolVersion = 7,
			ServerName = Server.Settings.Get("server-name") ?? string.Empty,
			ServerMOTD = Server.Settings.Get("motd") ?? string.Empty,
			Type = PlayerType.Regular
		});

		player.Realm?.GiveUpPlayer(player);
		player.SetRealm(this);

		_players[player.PlayerId] = player;

		if (World is not null)
		{
			World.StreamTo(player.Connection!);
			player.Spawn(World.SpawnPoint);

			SpawnMissingPlayersFor(player);
		}
	}

	/// <inheritdoc/>
	public void GiveUpPlayer(IPlayer player)
	{
		_players.Remove(player.PlayerId);
	
		// Despawn all the existing players.
		// And also despawn this player for them.
		foreach (var realmPlayer in Players.Values)
		{
			player.Connection?.Send(new DespawnPlayerPacket
			{
				PlayerId = realmPlayer.PlayerId,
			});

			realmPlayer.Connection?.Send(new DespawnPlayerPacket 
			{ 
				PlayerId = player.PlayerId 
			});
		}
	}
	
	/// <summary>
	/// Checks if players are still active.
	/// </summary>
	public void HeartbeatPlayers()
	{
		const int pingInterval = 5;

		var time = DateTimeOffset.Now.ToUnixTimeSeconds();
		var shouldPing = time - _lastPingTime > pingInterval;
		if (shouldPing)
			_lastPingTime = time;

		foreach (var player in Players.Values)
		{
			if (player.Connection is null)
				continue;

			if (shouldPing)
				player.Ping();

			if (!player.Connection.Connected)
			{
				_disconnectQueue.Enqueue(player);
				player.Disconnect();
			}
		}

		while (_disconnectQueue.Count > 0)
			_players.Remove(_disconnectQueue.Dequeue().PlayerId);
	}

	/// <summary>
	/// Spawns all the missing players for a player.
	/// </summary>
	/// <param name="player">The player.</param>
	/// <param name="realm">The realm this player is in.</param>
	private void SpawnMissingPlayersFor(IPlayer player)
	{
		foreach (var plr in Players.Values)
		{
			if (plr == player)
				continue;

			player.Connection?.Send(new SpawnPlayerPacket
			{
				PlayerId = plr.PlayerId,
				PlayerName = plr.Name,
				X = plr.Position.X,
				Y = plr.Position.Y,
				Z = plr.Position.Z,
				Pitch = plr.Orientation.Pitch,
				Yaw = plr.Orientation.Yaw
			});
		}
	}

}
